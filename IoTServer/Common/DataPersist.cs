using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IoTServer.Common
{
    public class DataPersist
    {
        string prefix;
        static ConcurrentDictionary<string, string> data = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix">前缀</param> 
        public DataPersist(string prefix)
        {
            this.prefix = $"iot_{prefix}_";
        }

        /// <summary>
        /// 读
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Read(string key)
        {
            key = prefix + key;
            if (data.ContainsKey(key))
            {
                return data[key];
            }
            return string.Empty;
        }

        public string Read(int key)
        {
            return Read(key.ToString());
        }

        /// <summary>
        /// 写
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Write(string key, string value)
        {
            key = prefix + key;
            if (data.ContainsKey(key))
            {
                data[key] = value;
            }
            else
            {
                data.TryAdd(key, value);
            }
        }

        public void Write(int key, string value)
        {
            Write(key.ToString(), value);
        }

        /// <summary>
        /// 清空
        /// </summary>
        public static void Clear()
        {
            data = new ConcurrentDictionary<string, string>();

            var filePath = @"C:\IoTClient\ConnectionConfig.Data";
            if (File.Exists(filePath)) File.Delete(filePath);
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public static void SaveData()
        {
            var path = @"C:\IoTClient";
            var filePath = path + @"\IoTClient.Data";
            var dataString = JsonConvert.SerializeObject(data);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fileStream))
                {
                    sw.Write(dataString);
                }
            }
        }

        /// <summary>
        /// 初始化加载数据
        /// </summary>
        public static void LoadData()
        {
            var dataString = string.Empty;
            var path = @"C:\IoTClient";
            var filePath = path + @"\IoTClient.Data";
            if (File.Exists(filePath))
                dataString = File.ReadAllText(filePath);
            else
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                File.SetAttributes(path, FileAttributes.Hidden);
            }
            if (!string.IsNullOrWhiteSpace(dataString))
                data = JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(dataString);
        }
    }
}
