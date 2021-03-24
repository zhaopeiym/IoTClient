namespace IoTClient.Tool.Model
{
    public class VersionCheckInput
    {
        /// <summary>
        /// 当前版本
        /// </summary>
        public float CurrentVersion { get; set; } = 0.44f;

        /// <summary>
        /// 忽略版本
        /// </summary>
        public float? IgnoreVersion { get; set; }
    }
}
