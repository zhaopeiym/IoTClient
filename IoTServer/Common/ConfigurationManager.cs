using Microsoft.Extensions.Configuration;
using System.IO;

namespace IoTServer.Common
{
    public static class ConfigurationManager
    {
        public readonly static IConfiguration Configuration;

        static ConfigurationManager() =>
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetSection<T>(this string key) where T : class, new()
        {
            return Configuration.GetSection(key).Get<T>();
        }

        /// <summary>
        /// 获取配置文件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetConfig(this string key, string defaultValue = "")
        {
            var value = Configuration.GetValue(key, defaultValue);
            if (string.IsNullOrWhiteSpace(value))
            {
                if (!string.IsNullOrWhiteSpace(defaultValue))
                    return defaultValue?.Trim();
                throw new System.Exception($"获取配置{key}异常");
            }
            return value?.Trim();
        }
    }
}
