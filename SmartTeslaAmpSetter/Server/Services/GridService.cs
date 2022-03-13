﻿namespace SmartTeslaAmpSetter.Server.Services;

public class GridService
{
    private readonly ILogger<GridService> _logger;
    private readonly IConfiguration _configuration;

    public GridService(ILogger<GridService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<int> GetCurrentOverage()
    {
        _logger.LogTrace("{method}()", nameof(GetCurrentOverage));
        using var httpClient = new HttpClient();
        var requestUri = _configuration.GetValue<string>("CurrentPowerToGridUrl");
        var response = await httpClient.GetAsync(
                requestUri)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        if (int.TryParse(result, out var overage))
        {
            return overage;
        }

        throw new InvalidCastException($"Could not parse result {result} from uri {requestUri} to integer");
    }

    public async Task<int?> GetCurrentInverterPower()
    {
        _logger.LogTrace("{method}()", nameof(GetCurrentInverterPower));
        using var httpClient = new HttpClient();
        var requestUri = _configuration.GetValue<string>("CurrentInverterPowerUrl");
        if (requestUri == null)
        {
            return null;
        }
        var response = await httpClient.GetAsync(
                requestUri)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        if (int.TryParse(result, out var overage))
        {
            return overage;
        }

        throw new InvalidCastException($"Could not parse result {result} from uri {requestUri} to integer");
    }
}