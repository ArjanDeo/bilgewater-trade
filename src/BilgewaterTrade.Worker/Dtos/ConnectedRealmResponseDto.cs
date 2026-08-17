using Newtonsoft.Json;

namespace BilgewaterTrade.Worker.Dtos;

public class ConnectedRealmResponseDto
{
    public int Id { get; set; }

    [JsonProperty("realms")]
    public List<RealmDto> Realms { get; set; }
}

public class RealmDto
{
    public int Id { get; set; }
    public string Slug { get; set; }

    [JsonProperty("name")]
    public Dictionary<string, string> Name { get; set; }
}