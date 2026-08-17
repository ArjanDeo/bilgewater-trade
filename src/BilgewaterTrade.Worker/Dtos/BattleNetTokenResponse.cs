using Newtonsoft.Json;

namespace BilgewaterTrade.Worker.Dtos;

public class BattleNetTokenResponse
{
    [JsonProperty("access_token")]
    public required string AccessToken { get; set; }

    [JsonProperty("token_type")]
    public required string TokenType { get; set; }

    [JsonProperty("expires_in")]
    public required int ExpiresIn { get; set; }

    [JsonProperty("sub")]
    public required string Sub { get; set; }
}