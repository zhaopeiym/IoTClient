using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.BACnet;
using System.IO.BACnet.Storage;
using System.Linq;
using System.Net.NetworkInformation;

namespace IoTServer.Servers.BACnet
{
    public class BACnetServer
    {
        private DeviceStorage m_storage;
        private BacnetClient m_ip_server;
        private uint deviceId = 1234;
        private static uint increaseId = 1;

        private Dictionary<BacnetObjectId, List<Subscription>> m_subscriptions = new Dictionary<BacnetObjectId, List<Subscription>>();
        private object m_lockObject = new object();
        private BacnetSegmentations m_supported_segmentation = BacnetSegmentations.SEGMENTATION_BOTH;

        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start(string localEndpointIp = "")
        {
            try
            {
                //init

                PhysicalAddress macAddr =
                (
                    from netiface in NetworkInterface.GetAllNetworkInterfaces()
                    where ((netiface.OperationalStatus == OperationalStatus.Up) && ((netiface.NetworkInterfaceType == NetworkInterfaceType.Ethernet) || (netiface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)))
                    select netiface.GetPhysicalAddress()
                ).FirstOrDefault();

                if (macAddr != null)
                {
                    byte[] mac = macAddr.GetAddressBytes();
                    deviceId = ((uint)mac[5] + (uint)((mac[4] << 8) & 0x3F)) << 6;
                }
                // Un bricollage du dimanche vite fait ici !
                deviceId = deviceId + increaseId++;

                m_storage = DeviceStorage.Load("IoTServer.DeviceStorage.xml", deviceId);

                var str = JsonConvert.SerializeObject(m_storage);

                m_storage.ChangeOfValue += new DeviceStorage.ChangeOfValueHandler(m_storage_ChangeOfValue);
                m_storage.ReadOverride += new DeviceStorage.ReadOverrideHandler(m_storage_ReadOverride);

                //create udp service point
                BacnetIpUdpProtocolTransport udp_transport = new BacnetIpUdpProtocolTransport(47808, false, localEndpointIp: localEndpointIp);       //set to true to force "single socket" usage
                m_ip_server = new BacnetClient(udp_transport);

                m_ip_server.OnWhoIs += new BacnetClient.WhoIsHandler(OnWhoIs);
                m_ip_server.OnReadPropertyRequest += new BacnetClient.ReadPropertyRequestHandler(OnReadPropertyRequest);
                m_ip_server.OnWritePropertyRequest += new BacnetClient.WritePropertyRequestHandler(OnWritePropertyRequest);
                m_ip_server.OnReadPropertyMultipleRequest += new BacnetClient.ReadPropertyMultipleRequestHandler(OnReadPropertyMultipleRequest);
                m_ip_server.OnSubscribeCOV += new BacnetClient.SubscribeCOVRequestHandler(OnSubscribeCOV);
                m_ip_server.OnSubscribeCOVProperty += new BacnetClient.SubscribeCOVPropertyRequestHandler(OnSubscribeCOVProperty);
                m_ip_server.OnTimeSynchronize += new BacnetClient.TimeSynchronizeHandler(OnTimeSynchronize);
                m_ip_server.OnDeviceCommunicationControl += new BacnetClient.DeviceCommunicationControlRequestHandler(OnDeviceCommunicationControl);
                m_ip_server.OnReinitializedDevice += new BacnetClient.ReinitializedRequestHandler(OnReinitializedDevice);
                m_ip_server.Start();

                //send greeting
                m_ip_server.Iam(m_storage.DeviceId, m_supported_segmentation);

            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {

        }
        public BacnetValue GetBacObjectPresentValue(BacnetObjectId id)
        {
            // L'index 0 c'est le nombre de valeurs associées à la propriété
            // L'index 1 pour la première valeur
            // L'index System.IO.BACnet.Serialize.ASN1.BACNET_ARRAY_ALL pour tout le tableau
            IList<BacnetValue> val = null;
            m_storage.ReadProperty(id, BacnetPropertyIds.PROP_PRESENT_VALUE, 1, out val);
            return val[0];
        }

        public void SetBacObjectPresentValue(BacnetObjectId id, BacnetValue bv)
        {
            // On est sur des valeurs simples, la comparaison est possible ici sans problème
            if (GetBacObjectPresentValue(id).Value.ToString() == bv.Value.ToString())
                return;

            // L'index 0 c'est le nombre de valeurs associées à la propriété
            // L'index 1 pour la première valeur
            // L'index System.IO.BACnet.Serialize.ASN1.BACNET_ARRAY_ALL pour tout le tableau
            IList<BacnetValue> val = new BacnetValue[1] { bv };
            m_storage.WriteProperty(id, BacnetPropertyIds.PROP_PRESENT_VALUE, 1, val, true);
        }

        // Ici les remplacement de la lecture de quelques élements
        private void m_storage_ReadOverride(BacnetObjectId object_id, BacnetPropertyIds property_id, uint array_index, out IList<BacnetValue> value, out DeviceStorage.ErrorCodes status, out bool handled)
        {
            handled = true;
            value = new BacnetValue[0];
            status = DeviceStorage.ErrorCodes.Good;


            if (object_id.Type == BacnetObjectTypes.OBJECT_DEVICE && property_id == BacnetPropertyIds.PROP_OBJECT_LIST)
            {
                if (array_index == 0)
                {
                    //object list count 
                    value = new BacnetValue[] { new BacnetValue(BacnetApplicationTags.BACNET_APPLICATION_TAG_UNSIGNED_INT, (uint)m_storage.Objects.Length) };
                }
                else if (array_index != System.IO.BACnet.Serialize.ASN1.BACNET_ARRAY_ALL)
                {
                    //object list index 
                    value = new BacnetValue[] { new BacnetValue(BacnetApplicationTags.BACNET_APPLICATION_TAG_OBJECT_ID, new BacnetObjectId(m_storage.Objects[array_index - 1].Type, m_storage.Objects[array_index - 1].Instance)) };
                }
                else
                {
                    //object list whole
                    BacnetValue[] list = new BacnetValue[m_storage.Objects.Length];
                    for (int i = 0; i < list.Length; i++)
                    {
                        list[i].Tag = BacnetApplicationTags.BACNET_APPLICATION_TAG_OBJECT_ID;
                        list[i].Value = new BacnetObjectId(m_storage.Objects[i].Type, m_storage.Objects[i].Instance);
                    }
                    value = list;
                }
            }
            else if (object_id.Type == BacnetObjectTypes.OBJECT_DEVICE && object_id.Instance == m_storage.DeviceId && property_id == BacnetPropertyIds.PROP_PROTOCOL_OBJECT_TYPES_SUPPORTED)
            {
                BacnetValue v = new BacnetValue();
                v.Tag = BacnetApplicationTags.BACNET_APPLICATION_TAG_BIT_STRING;
                BacnetBitString b = new BacnetBitString();
                b.SetBit((byte)BacnetObjectTypes.MAX_ASHRAE_OBJECT_TYPE, false); //set all false
                b.SetBit((byte)BacnetObjectTypes.OBJECT_ANALOG_INPUT, true);
                b.SetBit((byte)BacnetObjectTypes.OBJECT_DEVICE, true);
                b.SetBit((byte)BacnetObjectTypes.OBJECT_ANALOG_VALUE, true);
                b.SetBit((byte)BacnetObjectTypes.OBJECT_CHARACTERSTRING_VALUE, true);
                b.SetBit((byte)BacnetObjectTypes.OBJECT_MULTI_STATE_VALUE, true);
                b.SetBit((byte)BacnetObjectTypes.OBJECT_BINARY_VALUE, true);
                v.Value = b;
                value = new BacnetValue[] { v };
            }
            else if (object_id.Type == BacnetObjectTypes.OBJECT_DEVICE && object_id.Instance == m_storage.DeviceId && property_id == BacnetPropertyIds.PROP_PROTOCOL_SERVICES_SUPPORTED)
            {
                BacnetValue v = new BacnetValue();
                v.Tag = BacnetApplicationTags.BACNET_APPLICATION_TAG_BIT_STRING;
                BacnetBitString b = new BacnetBitString();
                b.SetBit((byte)BacnetServicesSupported.MAX_BACNET_SERVICES_SUPPORTED, false); //set all false
                b.SetBit((byte)BacnetServicesSupported.SERVICE_SUPPORTED_SUBSCRIBE_COV, true);

                b.SetBit((byte)BacnetServicesSupported.SERVICE_SUPPORTED_I_AM, true);
                b.SetBit((byte)BacnetServicesSupported.SERVICE_SUPPORTED_WHO_IS, true);
                b.SetBit((byte)BacnetServicesSupported.SERVICE_SUPPORTED_READ_PROP_MULTIPLE, true);
                b.SetBit((byte)BacnetServicesSupported.SERVICE_SUPPORTED_READ_PROPERTY, true);
                b.SetBit((byte)BacnetServicesSupported.SERVICE_SUPPORTED_WRITE_PROPERTY, true);
                b.SetBit((byte)BacnetServicesSupported.SERVICE_SUPPORTED_CONFIRMED_COV_NOTIFICATION, true);
                b.SetBit((byte)BacnetServicesSupported.SERVICE_SUPPORTED_UNCONFIRMED_COV_NOTIFICATION, true);
                b.SetBit((byte)BacnetServicesSupported.SERVICE_SUPPORTED_SUBSCRIBE_COV_PROPERTY, true);
                b.SetBit((byte)BacnetServicesSupported.SERVICE_SUPPORTED_REINITIALIZE_DEVICE, true);
                b.SetBit((byte)BacnetServicesSupported.SERVICE_SUPPORTED_DEVICE_COMMUNICATION_CONTROL, true);
                b.SetBit((byte)BacnetServicesSupported.SERVICE_SUPPORTED_TIME_SYNCHRONIZATION, true);
                b.SetBit((byte)BacnetServicesSupported.SERVICE_SUPPORTED_UTC_TIME_SYNCHRONIZATION, true);
                v.Value = b;
                value = new BacnetValue[] { v };
            }
            else if (object_id.Type == BacnetObjectTypes.OBJECT_DEVICE && object_id.Instance == m_storage.DeviceId && property_id == BacnetPropertyIds.PROP_SEGMENTATION_SUPPORTED)
            {
                BacnetValue v = new BacnetValue();
                v.Tag = BacnetApplicationTags.BACNET_APPLICATION_TAG_ENUMERATED;
                v.Value = (uint)BacnetSegmentations.SEGMENTATION_BOTH;
                value = new BacnetValue[] { v };
            }
            else if (object_id.Type == BacnetObjectTypes.OBJECT_DEVICE && object_id.Instance == m_storage.DeviceId && property_id == BacnetPropertyIds.PROP_SYSTEM_STATUS)
            {
                BacnetValue v = new BacnetValue();
                v.Tag = BacnetApplicationTags.BACNET_APPLICATION_TAG_ENUMERATED;
                v.Value = (uint)BacnetDeviceStatus.OPERATIONAL;      //can we be in any other mode I wonder?
                value = new BacnetValue[] { v };
            }
            else if (object_id.Type == BacnetObjectTypes.OBJECT_DEVICE && object_id.Instance == m_storage.DeviceId && property_id == BacnetPropertyIds.PROP_ACTIVE_COV_SUBSCRIPTIONS)
            {
                List<BacnetValue> list = new List<BacnetValue>();
                foreach (KeyValuePair<BacnetObjectId, List<Subscription>> entry in m_subscriptions)
                {
                    foreach (Subscription sub in entry.Value)
                    {
                        //encode
                        System.IO.BACnet.Serialize.EncodeBuffer buffer = new System.IO.BACnet.Serialize.EncodeBuffer();
                        BacnetCOVSubscription cov = new BacnetCOVSubscription();
                        cov.Recipient = sub.reciever_address;
                        cov.subscriptionProcessIdentifier = sub.subscriberProcessIdentifier;
                        cov.monitoredObjectIdentifier = sub.monitoredObjectIdentifier;
                        cov.monitoredProperty = sub.monitoredProperty;
                        cov.IssueConfirmedNotifications = sub.issueConfirmedNotifications;
                        cov.TimeRemaining = (uint)sub.lifetime - (uint)(DateTime.Now - sub.start).TotalMinutes;
                        cov.COVIncrement = sub.covIncrement;
                        System.IO.BACnet.Serialize.ASN1.encode_cov_subscription(buffer, cov);

                        //add
                        BacnetValue v = new BacnetValue();
                        v.Tag = BacnetApplicationTags.BACNET_APPLICATION_TAG_COV_SUBSCRIPTION;
                        v.Value = buffer.ToArray();
                        list.Add(v);
                    }
                }
                value = list;
            }
            else if (object_id.Type == BacnetObjectTypes.OBJECT_OCTETSTRING_VALUE && object_id.Instance == 0 && property_id == BacnetPropertyIds.PROP_PRESENT_VALUE)
            {
                //this is our huge blob
                BacnetValue v = new BacnetValue();
                v.Tag = BacnetApplicationTags.BACNET_APPLICATION_TAG_OCTET_STRING;
                byte[] blob = new byte[2000];
                for (int i = 0; i < blob.Length; i++)
                    blob[i] = (i % 2 == 0) ? (byte)'A' : (byte)'B';
                v.Value = blob;
                value = new BacnetValue[] { v };
            }
            else if (object_id.Type == BacnetObjectTypes.OBJECT_GROUP && property_id == BacnetPropertyIds.PROP_PRESENT_VALUE)
            {
                //get property list
                IList<BacnetValue> properties;
                if (m_storage.ReadProperty(object_id, BacnetPropertyIds.PROP_LIST_OF_GROUP_MEMBERS, System.IO.BACnet.Serialize.ASN1.BACNET_ARRAY_ALL, out properties) != DeviceStorage.ErrorCodes.Good)
                {
                    value = new BacnetValue[] { new BacnetValue(BacnetApplicationTags.BACNET_APPLICATION_TAG_ERROR, new BacnetError(BacnetErrorClasses.ERROR_CLASS_DEVICE, BacnetErrorCodes.ERROR_CODE_INTERNAL_ERROR)) };
                }
                else
                {
                    List<BacnetValue> _value = new List<BacnetValue>();
                    foreach (BacnetValue p in properties)
                    {
                        if (p.Value is BacnetReadAccessSpecification)
                        {
                            BacnetReadAccessSpecification prop = (BacnetReadAccessSpecification)p.Value;
                            BacnetReadAccessResult result = new BacnetReadAccessResult();
                            result.objectIdentifier = prop.objectIdentifier;
                            List<BacnetPropertyValue> result_values = new List<BacnetPropertyValue>();
                            foreach (BacnetPropertyReference r in prop.propertyReferences)
                            {
                                BacnetPropertyValue prop_value = new BacnetPropertyValue();
                                prop_value.property = r;
                                if (m_storage.ReadProperty(prop.objectIdentifier, (BacnetPropertyIds)r.propertyIdentifier, r.propertyArrayIndex, out prop_value.value) != DeviceStorage.ErrorCodes.Good)
                                {
                                    prop_value.value = new BacnetValue[] { new BacnetValue(BacnetApplicationTags.BACNET_APPLICATION_TAG_ERROR, new BacnetError(BacnetErrorClasses.ERROR_CLASS_DEVICE, BacnetErrorCodes.ERROR_CODE_INTERNAL_ERROR)) };
                                }
                                result_values.Add(prop_value);
                            }
                            result.values = result_values;
                            _value.Add(new BacnetValue(BacnetApplicationTags.BACNET_APPLICATION_TAG_READ_ACCESS_RESULT, result));
                        }
                    }
                    value = _value;
                }
            }
            else
            {
                handled = false;
            }
        }

        private class Subscription
        {
            public BacnetClient reciever;
            public BacnetAddress reciever_address;
            public uint subscriberProcessIdentifier;
            public BacnetObjectId monitoredObjectIdentifier;
            public BacnetPropertyReference monitoredProperty;
            public bool issueConfirmedNotifications;
            public uint lifetime;
            public DateTime start;
            public float covIncrement;
            public Subscription(BacnetClient reciever, BacnetAddress reciever_address, uint subscriberProcessIdentifier, BacnetObjectId monitoredObjectIdentifier, BacnetPropertyReference property, bool issueConfirmedNotifications, uint lifetime, float covIncrement)
            {
                this.reciever = reciever;
                this.reciever_address = reciever_address;
                this.subscriberProcessIdentifier = subscriberProcessIdentifier;
                this.monitoredObjectIdentifier = monitoredObjectIdentifier;
                this.monitoredProperty = property;
                this.issueConfirmedNotifications = issueConfirmedNotifications;
                this.lifetime = lifetime;
                this.start = DateTime.Now;
                this.covIncrement = covIncrement;
            }
            public int GetTimeRemaining()
            {

                if (lifetime == 0) return 0;

                uint elapse = (uint)(DateTime.Now - start).TotalSeconds;

                if (lifetime > elapse)
                    return (int)(lifetime - elapse);
                else

                    return -1;

            }
        }

        private void RemoveOldSubscriptions()
        {
            LinkedList<BacnetObjectId> to_be_deleted = new LinkedList<BacnetObjectId>();
            foreach (KeyValuePair<BacnetObjectId, List<Subscription>> entry in m_subscriptions)
            {
                for (int i = 0; i < entry.Value.Count; i++)
                {
                    // Modif F. Chaxel <0 modifié == 0
                    if (entry.Value[i].GetTimeRemaining() < 0)
                    {
                        entry.Value.RemoveAt(i);
                        i--;
                    }
                }
                if (entry.Value.Count == 0)
                    to_be_deleted.AddLast(entry.Key);
            }
            foreach (BacnetObjectId obj_id in to_be_deleted)
                m_subscriptions.Remove(obj_id);
        }

        // C'est que la souscription est ajoutée à la liste
        // ou retrouvée est remise en état
        private Subscription HandleSubscriptionRequest(BacnetClient sender, BacnetAddress adr, byte invoke_id, uint subscriberProcessIdentifier, BacnetObjectId monitoredObjectIdentifier, uint property_id, bool cancellationRequest, bool issueConfirmedNotifications, uint lifetime, float covIncrement)
        {
            //remove old leftovers
            RemoveOldSubscriptions();

            //find existing
            List<Subscription> subs = null;
            Subscription sub = null;
            if (m_subscriptions.ContainsKey(monitoredObjectIdentifier))
            {
                subs = m_subscriptions[monitoredObjectIdentifier];
                foreach (Subscription s in subs)
                {
                    // Modif FC
                    if (s.reciever.Equals(sender) && s.reciever_address.Equals(adr) && s.monitoredObjectIdentifier.Equals(monitoredObjectIdentifier) && s.monitoredProperty.propertyIdentifier == property_id)
                    {
                        sub = s;
                        break;
                    }
                }
            }

            //cancel
            if (cancellationRequest && sub != null)
            {
                subs.Remove(sub);
                if (subs.Count == 0)
                    m_subscriptions.Remove(sub.monitoredObjectIdentifier);

                //send confirm
                // F. Chaxel : a supprimer, c'est fait par l'appellant
                sender.SimpleAckResponse(adr, BacnetConfirmedServices.SERVICE_CONFIRMED_SUBSCRIBE_COV, invoke_id);

                return null;
            }

            //create if needed
            if (sub == null)
            {
                sub = new Subscription(sender, adr, subscriberProcessIdentifier, monitoredObjectIdentifier, new BacnetPropertyReference((uint)BacnetPropertyIds.PROP_ALL, System.IO.BACnet.Serialize.ASN1.BACNET_ARRAY_ALL), issueConfirmedNotifications, lifetime, covIncrement);

                if (subs == null)
                {
                    subs = new List<Subscription>();
                    m_subscriptions.Add(sub.monitoredObjectIdentifier, subs);
                }
                subs.Add(sub);
            }

            //update perhaps
            sub.issueConfirmedNotifications = issueConfirmedNotifications;
            sub.lifetime = lifetime;
            sub.start = DateTime.Now;

            return sub;
        }

        private void OnSubscribeCOV(BacnetClient sender, BacnetAddress adr, byte invoke_id, uint subscriberProcessIdentifier, BacnetObjectId monitoredObjectIdentifier, bool cancellationRequest, bool issueConfirmedNotifications, uint lifetime, BacnetMaxSegments max_segments)
        {

            lock (m_lockObject)
            {
                try
                {
                    //create 
                    Subscription sub = HandleSubscriptionRequest(sender, adr, invoke_id, subscriberProcessIdentifier, monitoredObjectIdentifier, (uint)BacnetPropertyIds.PROP_ALL, cancellationRequest, issueConfirmedNotifications, lifetime, 0);

                    //send confirm
                    sender.SimpleAckResponse(adr, BacnetConfirmedServices.SERVICE_CONFIRMED_SUBSCRIBE_COV, invoke_id);

                    //also send first values
                    if (!cancellationRequest)
                    {
                        System.Threading.ThreadPool.QueueUserWorkItem((o) =>
                        {

                            IList<BacnetPropertyValue> values;
                            if (m_storage.ReadPropertyAll(sub.monitoredObjectIdentifier, out values))
                                if (!sender.Notify(adr, sub.subscriberProcessIdentifier, m_storage.DeviceId, sub.monitoredObjectIdentifier, (uint)sub.GetTimeRemaining(), sub.issueConfirmedNotifications, values))
                                    Trace.TraceError("Couldn't send notify");
                        }, null);
                    }
                }
                catch (Exception)
                {
                    sender.ErrorResponse(adr, BacnetConfirmedServices.SERVICE_CONFIRMED_SUBSCRIBE_COV, invoke_id, BacnetErrorClasses.ERROR_CLASS_DEVICE, BacnetErrorCodes.ERROR_CODE_OTHER);
                }
            }
        }

        private void OnSubscribeCOVProperty(BacnetClient sender, BacnetAddress adr, byte invoke_id, uint subscriberProcessIdentifier, BacnetObjectId monitoredObjectIdentifier, BacnetPropertyReference monitoredProperty, bool cancellationRequest, bool issueConfirmedNotifications, uint lifetime, float covIncrement, BacnetMaxSegments max_segments)
        {
            lock (m_lockObject)
            {
                try
                {
                    //create 
                    Subscription sub = HandleSubscriptionRequest(sender, adr, invoke_id, subscriberProcessIdentifier, monitoredObjectIdentifier, (uint)BacnetPropertyIds.PROP_ALL, cancellationRequest, issueConfirmedNotifications, lifetime, covIncrement);

                    //send confirm
                    sender.SimpleAckResponse(adr, BacnetConfirmedServices.SERVICE_CONFIRMED_SUBSCRIBE_COV_PROPERTY, invoke_id);

                    //also send first values
                    if (!cancellationRequest)
                    {
                        System.Threading.ThreadPool.QueueUserWorkItem((o) =>
                        {
                            IList<BacnetValue> _values;
                            m_storage.ReadProperty(sub.monitoredObjectIdentifier, (BacnetPropertyIds)sub.monitoredProperty.propertyIdentifier, sub.monitoredProperty.propertyArrayIndex, out _values);
                            List<BacnetPropertyValue> values = new List<BacnetPropertyValue>();
                            BacnetPropertyValue tmp = new BacnetPropertyValue();
                            tmp.property = sub.monitoredProperty;
                            tmp.value = _values;
                            values.Add(tmp);
                            if (!sender.Notify(adr, sub.subscriberProcessIdentifier, m_storage.DeviceId, sub.monitoredObjectIdentifier, (uint)sub.GetTimeRemaining(), sub.issueConfirmedNotifications, values))
                                Trace.TraceError("Couldn't send notify");
                        }, null);
                    }
                }
                catch (Exception)
                {
                    sender.ErrorResponse(adr, BacnetConfirmedServices.SERVICE_CONFIRMED_SUBSCRIBE_COV_PROPERTY, invoke_id, BacnetErrorClasses.ERROR_CLASS_DEVICE, BacnetErrorCodes.ERROR_CODE_OTHER);
                }
            }
        }

        private void m_storage_ChangeOfValue(DeviceStorage sender, BacnetObjectId object_id, BacnetPropertyIds property_id, uint array_index, IList<BacnetValue> value)
        {
            System.Threading.ThreadPool.QueueUserWorkItem((o) =>
            {
                lock (m_lockObject)
                {
                    //remove old leftovers
                    RemoveOldSubscriptions();

                    //find subscription
                    if (!m_subscriptions.ContainsKey(object_id)) return;
                    List<Subscription> subs = m_subscriptions[object_id];

                    //convert
                    List<BacnetPropertyValue> values = new List<BacnetPropertyValue>();
                    BacnetPropertyValue tmp = new BacnetPropertyValue();
                    tmp.property = new BacnetPropertyReference((uint)property_id, array_index);
                    tmp.value = value;
                    values.Add(tmp);

                    //send to all
                    foreach (Subscription sub in subs)
                    {
                        if (sub.monitoredProperty.propertyIdentifier == (uint)BacnetPropertyIds.PROP_ALL || sub.monitoredProperty.propertyIdentifier == (uint)property_id)
                        {
                            //send notify
                            if (!sub.reciever.Notify(sub.reciever_address, sub.subscriberProcessIdentifier, m_storage.DeviceId, sub.monitoredObjectIdentifier, (uint)sub.GetTimeRemaining(), sub.issueConfirmedNotifications, values))
                                Trace.TraceError("Couldn't send notify");
                        }
                    }
                }
            }, null);
        }

        private void HandleSegmentationResponse(BacnetClient sender, BacnetAddress adr, byte invoke_id, BacnetMaxSegments max_segments, Action<BacnetClient.Segmentation> transmit)
        {
            BacnetClient.Segmentation segmentation = sender.GetSegmentBuffer(max_segments);

            //send first
            transmit(segmentation);

            if (segmentation == null || segmentation.buffer.result == System.IO.BACnet.Serialize.EncodeResult.Good) return;

            //start new thread to handle the segment sequence
            System.Threading.ThreadPool.QueueUserWorkItem((o) =>
            {
                byte old_max_info_frames = sender.Transport.MaxInfoFrames;
                sender.Transport.MaxInfoFrames = segmentation.window_size;      //increase max_info_frames, to increase throughput. This might be against 'standard'
                while (true)
                {
                    bool more_follows = (segmentation.buffer.result & System.IO.BACnet.Serialize.EncodeResult.NotEnoughBuffer) > 0;

                    //wait for segmentACK
                    if ((segmentation.sequence_number - 1) % segmentation.window_size == 0 || !more_follows)
                    {
                        if (!sender.WaitForAllTransmits(sender.TransmitTimeout))
                        {
                            Trace.TraceWarning("Transmit timeout");
                            break;
                        }
                        byte current_number = segmentation.sequence_number;
                        if (!sender.WaitForSegmentAck(adr, invoke_id, segmentation, sender.Timeout))
                        {
                            Trace.TraceWarning("Didn't get segmentACK");
                            break;
                        }
                        if (segmentation.sequence_number != current_number)
                        {
                            Trace.WriteLine("Oh, a retransmit", null);
                            more_follows = true;
                        }
                    }
                    else
                    {
                        //a negative segmentACK perhaps
                        byte current_number = segmentation.sequence_number;
                        sender.WaitForSegmentAck(adr, invoke_id, segmentation, TimeSpan.FromSeconds(0));      //don't wait
                        if (segmentation.sequence_number != current_number)
                        {
                            Trace.WriteLine("Oh, a retransmit", null);
                            more_follows = true;
                        }
                    }

                    if (more_follows)
                        lock (m_lockObject) transmit(segmentation);
                    else
                        break;
                }
                sender.Transport.MaxInfoFrames = old_max_info_frames;
            });
        }

        private void OnDeviceCommunicationControl(BacnetClient sender, BacnetAddress adr, byte invoke_id, uint time_duration, uint enable_disable, string password, BacnetMaxSegments max_segments)
        {
            switch (enable_disable)
            {
                case 0:
                    Trace.TraceInformation("Enable communication? Sure!");
                    sender.SimpleAckResponse(adr, BacnetConfirmedServices.SERVICE_CONFIRMED_DEVICE_COMMUNICATION_CONTROL, invoke_id);
                    break;
                case 1:
                    Trace.TraceInformation("Disable communication? ... smile and wave (ignored)");
                    sender.SimpleAckResponse(adr, BacnetConfirmedServices.SERVICE_CONFIRMED_DEVICE_COMMUNICATION_CONTROL, invoke_id);
                    break;
                case 2:
                    Trace.TraceWarning("Disable initiation? I don't think so!");
                    sender.ErrorResponse(adr, BacnetConfirmedServices.SERVICE_CONFIRMED_DEVICE_COMMUNICATION_CONTROL, invoke_id, BacnetErrorClasses.ERROR_CLASS_DEVICE, BacnetErrorCodes.ERROR_CODE_OTHER);
                    break;
                default:
                    Trace.TraceError("Now, what is this device_communication code: " + enable_disable + "!!!!");
                    sender.ErrorResponse(adr, BacnetConfirmedServices.SERVICE_CONFIRMED_DEVICE_COMMUNICATION_CONTROL, invoke_id, BacnetErrorClasses.ERROR_CLASS_DEVICE, BacnetErrorCodes.ERROR_CODE_OTHER);
                    break;
            }
        }

        private void OnReinitializedDevice(BacnetClient sender, BacnetAddress adr, byte invoke_id, BacnetReinitializedStates state, string password, BacnetMaxSegments max_segments)
        {
            Trace.TraceInformation("So you wanna reboot me, eh? Pfff! (" + state.ToString() + ")");
            sender.SimpleAckResponse(adr, BacnetConfirmedServices.SERVICE_CONFIRMED_REINITIALIZE_DEVICE, invoke_id);
        }

        private void OnTimeSynchronize(BacnetClient sender, BacnetAddress adr, DateTime dateTime, bool utc)
        {
            Trace.TraceInformation("Uh, a new date: " + dateTime.ToString());
        }

        private void OnReadPropertyMultipleRequest(BacnetClient sender, BacnetAddress adr, byte invoke_id, IList<BacnetReadAccessSpecification> properties, BacnetMaxSegments max_segments)
        {
            lock (m_lockObject)
            {
                try
                {
                    IList<BacnetPropertyValue> value;
                    List<BacnetReadAccessResult> values = new List<BacnetReadAccessResult>();
                    foreach (BacnetReadAccessSpecification p in properties)
                    {
                        if (p.propertyReferences.Count == 1 && p.propertyReferences[0].propertyIdentifier == (uint)BacnetPropertyIds.PROP_ALL)
                        {
                            if (!m_storage.ReadPropertyAll(p.objectIdentifier, out value))
                            {
                                sender.ErrorResponse(adr, BacnetConfirmedServices.SERVICE_CONFIRMED_READ_PROP_MULTIPLE, invoke_id, BacnetErrorClasses.ERROR_CLASS_OBJECT, BacnetErrorCodes.ERROR_CODE_UNKNOWN_OBJECT);
                                return;
                            }
                        }
                        else
                            m_storage.ReadPropertyMultiple(p.objectIdentifier, p.propertyReferences, out value);
                        values.Add(new BacnetReadAccessResult(p.objectIdentifier, value));
                    }

                    HandleSegmentationResponse(sender, adr, invoke_id, max_segments, (seg) =>
                    {
                        sender.ReadPropertyMultipleResponse(adr, invoke_id, seg, values);
                    });
                }
                catch (Exception)
                {
                    sender.ErrorResponse(adr, BacnetConfirmedServices.SERVICE_CONFIRMED_READ_PROP_MULTIPLE, invoke_id, BacnetErrorClasses.ERROR_CLASS_DEVICE, BacnetErrorCodes.ERROR_CODE_OTHER);
                }
            }
        }

        private void OnWritePropertyRequest(BacnetClient sender, BacnetAddress adr, byte invoke_id, BacnetObjectId object_id, BacnetPropertyValue value, BacnetMaxSegments max_segments)
        {
            BacnetPropertyIds PropId = (BacnetPropertyIds)value.property.propertyIdentifier;

            bool AllowWrite =
                (object_id.Equals("OBJECT_ANALOG_VALUE:0") && (PropId == BacnetPropertyIds.PROP_OUT_OF_SERVICE)) ||
                (object_id.Equals("OBJECT_ANALOG_VALUE:0") && (PropId == BacnetPropertyIds.PROP_PRESENT_VALUE)) ||
                (object_id.Equals("OBJECT_ANALOG_VALUE:1") && (PropId == BacnetPropertyIds.PROP_PRESENT_VALUE)) ||
                (object_id.Equals("OBJECT_ANALOG_VALUE:2") && (PropId == BacnetPropertyIds.PROP_PRESENT_VALUE)) ||
                (object_id.Equals("OBJECT_ANALOG_VALUE:3") && (PropId == BacnetPropertyIds.PROP_PRESENT_VALUE)) ||
                (object_id.Equals("OBJECT_CHARACTERSTRING_VALUE:1") && (PropId == BacnetPropertyIds.PROP_PRESENT_VALUE)) ||
                (object_id.Equals("OBJECT_CHARACTERSTRING_VALUE:2") && (PropId == BacnetPropertyIds.PROP_PRESENT_VALUE)) ||
                (object_id.Equals("OBJECT_CHARACTERSTRING_VALUE:3") && (PropId == BacnetPropertyIds.PROP_PRESENT_VALUE)) ||
                (object_id.Equals("OBJECT_MULTI_STATE_VALUE:0") && (PropId == BacnetPropertyIds.PROP_PRESENT_VALUE));

            if (AllowWrite == false)
            {
                sender.ErrorResponse(adr, BacnetConfirmedServices.SERVICE_CONFIRMED_WRITE_PROPERTY, invoke_id, BacnetErrorClasses.ERROR_CLASS_DEVICE, BacnetErrorCodes.ERROR_CODE_WRITE_ACCESS_DENIED);
                return;
            }

            lock (m_lockObject)
            {
                try
                {
                    // Modif FC

                    DeviceStorage.ErrorCodes code = m_storage.WriteCommandableProperty(object_id, (BacnetPropertyIds)value.property.propertyIdentifier, value.value[0], value.priority);

                    if (code == DeviceStorage.ErrorCodes.NotForMe)
                        code = m_storage.WriteProperty(object_id, (BacnetPropertyIds)value.property.propertyIdentifier, value.property.propertyArrayIndex, value.value);

                    if (code == DeviceStorage.ErrorCodes.Good)
                        sender.SimpleAckResponse(adr, BacnetConfirmedServices.SERVICE_CONFIRMED_WRITE_PROPERTY, invoke_id);
                    else
                        if (code == DeviceStorage.ErrorCodes.WriteAccessDenied)
                        sender.ErrorResponse(adr, BacnetConfirmedServices.SERVICE_CONFIRMED_WRITE_PROPERTY, invoke_id, BacnetErrorClasses.ERROR_CLASS_DEVICE, BacnetErrorCodes.ERROR_CODE_WRITE_ACCESS_DENIED);
                    else
                        sender.ErrorResponse(adr, BacnetConfirmedServices.SERVICE_CONFIRMED_WRITE_PROPERTY, invoke_id, BacnetErrorClasses.ERROR_CLASS_DEVICE, BacnetErrorCodes.ERROR_CODE_OTHER);
                }
                catch (Exception)
                {
                    sender.ErrorResponse(adr, BacnetConfirmedServices.SERVICE_CONFIRMED_WRITE_PROPERTY, invoke_id, BacnetErrorClasses.ERROR_CLASS_DEVICE, BacnetErrorCodes.ERROR_CODE_OTHER);
                }
            }
        }

        private void OnReadPropertyRequest(BacnetClient sender, BacnetAddress adr, byte invoke_id, BacnetObjectId object_id, BacnetPropertyReference property, BacnetMaxSegments max_segments)
        {
            lock (m_lockObject)
            {
                try
                {
                    IList<BacnetValue> value;
                    DeviceStorage.ErrorCodes code = m_storage.ReadProperty(object_id, (BacnetPropertyIds)property.propertyIdentifier, property.propertyArrayIndex, out value);
                    if (code == DeviceStorage.ErrorCodes.Good)
                        sender.ReadPropertyResponse(adr, invoke_id, sender.GetSegmentBuffer(max_segments), object_id, property, value);
                    else
                        sender.ErrorResponse(adr, BacnetConfirmedServices.SERVICE_CONFIRMED_READ_PROPERTY, invoke_id, BacnetErrorClasses.ERROR_CLASS_DEVICE, BacnetErrorCodes.ERROR_CODE_OTHER);
                }
                catch (Exception)
                {
                    sender.ErrorResponse(adr, BacnetConfirmedServices.SERVICE_CONFIRMED_READ_PROPERTY, invoke_id, BacnetErrorClasses.ERROR_CLASS_DEVICE, BacnetErrorCodes.ERROR_CODE_OTHER);
                }
            }
        }

        private void OnWhoIs(BacnetClient sender, BacnetAddress adr, int low_limit, int high_limit)
        {
            lock (m_lockObject)
            {
                if (low_limit != -1 && m_storage.DeviceId < low_limit) return;
                else if (high_limit != -1 && m_storage.DeviceId > high_limit) return;
                else sender.Iam(m_storage.DeviceId, m_supported_segmentation);
            }
        }
    }

}
