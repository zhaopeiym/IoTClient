using System;
using System.Collections.Generic;
using System.Linq;
using Talk.Redis;

namespace IoTServer.Common
{
    public class DataPersist
    {
        /// <summary>
        /// 是否使用redis对数据持久化，否则存内存
        /// </summary>
        private bool RedisPersist = true;
        RedisManager redisManager;
        static Dictionary<string, string> data = new Dictionary<string, string>();

        public DataPersist(string redisConfig)
        {
            RedisPersist = !string.IsNullOrWhiteSpace(redisConfig);
            if (RedisPersist)
                redisManager = new RedisManager(1, redisConfig);
        }

        /// <summary>
        /// 读
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Read(string key)
        {
            key = "iot_" + key;
            if (RedisPersist)
            {
                return redisManager.GetString(key);
            }
            else if (data.Keys.Contains(key))
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
            key = "iot_" + key;
            if (RedisPersist)
            {
                redisManager.Set(key, value);
            }
            else if (data.Keys.Contains(key))
            {
                data[key] = value;
            }
            else
            {
                data.Add(key, value);
            }
        }

        public void Write(int key, string value)
        {
            Write(key.ToString(), value);
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            if (RedisPersist)
            {
                //TODO
            }
            else
            {
                data = new Dictionary<string, string>();
            }
        }
    }
}
