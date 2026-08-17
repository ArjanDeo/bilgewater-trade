using Newtonsoft.Json;

namespace BilgewaterTrade.Worker.Dtos;

public class ConnectedRealmIndexResponseDto
{
    [JsonProperty("connected_realms")]
    public List<ConnectedRealmRefDto> ConnectedRealms { get; set; }
}

public class ConnectedRealmRefDto
{
    [JsonProperty("href")]
    public string Href { get; set; }
    public int Id =>
        int.Parse(Href.Split("/connected-realm/")[1].Split('?')[0]);
}