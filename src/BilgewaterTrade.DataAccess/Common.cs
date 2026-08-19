using Newtonsoft.Json;

namespace BilgewaterTrade.DataAccess;

public static class Common
{
    public enum TimeLeft
    {
        Short,
        Medium,
        Long,
        [JsonProperty("very_long")]
        VeryLong
    }
}