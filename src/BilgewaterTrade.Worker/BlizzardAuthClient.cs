using BilgewaterTrade.Worker.Dtos;
using Pathoschild.Http.Client;

namespace BilgewaterTrade.Worker;

public class BlizzardAuthClient
{
    private readonly FluentClient _fluentClient;
    
    private readonly IConfiguration _config;
    private readonly string _blizzardClientId;
    private readonly string _blizzardClientSecret;
    
    private string? _cachedToken;
    private DateTimeOffset _expiresAt = DateTimeOffset.MinValue;

    public BlizzardAuthClient(FluentClient fluentClient, IConfiguration config)
    {
        
        _fluentClient = fluentClient;
        _config = config;
        _blizzardClientId = _config.GetValue<string>("BlizzardClientID");
        _blizzardClientSecret = _config.GetValue<string>("BlizzardClientSecret");
    }

    public async Task<string> GetAccessTokenAsync()
    {
        if (_cachedToken != null && DateTimeOffset.UtcNow < _expiresAt)
            return _cachedToken;

        var tokenResponse = await _fluentClient
            .PostAsync("https://oauth.battle.net/token")
            .WithBasicAuthentication(_blizzardClientId, _blizzardClientSecret)
            .WithArgument("grant_type", "client_credentials")
            .As<BattleNetTokenResponse>();
        
        _cachedToken = tokenResponse.AccessToken;
        _expiresAt = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 60); // 60s safety buffer

        return _cachedToken;
    }
}