using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using StudentManagement.Core.Models;
using Microsoft.Extensions.Logging;

namespace StudentManagement.Infrastructure.ExternalApi;
using StudentManagement.Core.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;

public class ExternalApiService : IExteneralApi
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ExternalApiService> _logger;
    private const int MaxRetries = 3;
    private const int RetryDelayMs = 1000;
    private const int TimeoutSeconds = 5;

    public ExternalApiService(HttpClient httpClient, ILogger<ExternalApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> GetExternalDataAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Fetching external data...");

        try
        {
            var apiChoice = Random.Shared.Next(3);
            string result = apiChoice switch
            {
                0 => await FetchFromJsonPlaceholderWithRetryAsync(),
                1 => await FetchFromReqResWithRetryAsync(),
                _ => await FetchFromQuotableWithRetryAsync()
            };

            stopwatch.Stop();
            _logger.LogInformation($"External data fetched successfully in {stopwatch.ElapsedMilliseconds} ms");
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError($"Failed to fetch external data after {stopwatch.ElapsedMilliseconds} ms: {ex.Message}");
            throw;
        }
    }

    private async Task<string> FetchFromJsonPlaceholderWithRetryAsync()
    {
        return await RetryWithDelayAsync(async (token) =>
        {
            int id = Random.Shared.Next(1, 101);
            var response = await _httpClient.GetFromJsonAsync<Post>(
                $"https://jsonplaceholder.typicode.com/posts/{id}",
                cancellationToken: token
            );
            _logger.LogInformation($"JSONPlaceholder result: {response?.Title}");
            return response?.Title ?? "No data from JSONPlaceholder";
        });
    }

    private async Task<string> FetchFromReqResWithRetryAsync()
    {
        return await RetryWithDelayAsync(async (token) =>
        {
            int id = Random.Shared.Next(1, 13);
            var response = await _httpClient.GetAsync(
                $"https://reqres.in/api/users/{id}",
                HttpCompletionOption.ResponseContentRead,
                token
            );
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(token);
            using (JsonDocument doc = JsonDocument.Parse(content))
            {
                var firstName = doc.RootElement.GetProperty("data").GetProperty("first_name").GetString() ?? "Unknown";
                var lastName = doc.RootElement.GetProperty("data").GetProperty("last_name").GetString() ?? "User";
                _logger.LogInformation($"ReqRes result: {firstName} {lastName}");
                return $"{firstName} {lastName}";
            }
        });
    }

    private async Task<string> FetchFromQuotableWithRetryAsync()
    {
        return await RetryWithDelayAsync(async (token) =>
        {
            var response = await _httpClient.GetAsync(
                "https://api.quotable.io/random",
                HttpCompletionOption.ResponseContentRead,
                token
            );
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(token);
            using (JsonDocument doc = JsonDocument.Parse(content))
            {
                var quote = doc.RootElement.GetProperty("content").GetString() ?? "No quote available";
                _logger.LogInformation($"Quotable result: {quote}");
                return quote.Length > 100 ? quote.Substring(0, 100) + "..." : quote;
            }
        });
    }

    private async Task<string> RetryWithDelayAsync(Func<CancellationToken, Task<string>> operation)
    {
        Exception? lastException = null;

        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                _logger.LogInformation($"Attempt {attempt}/{MaxRetries}");
                
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(TimeoutSeconds)))
                {
                    var stopwatch = Stopwatch.StartNew();
                    var result = await operation(cts.Token);
                    stopwatch.Stop();
                    _logger.LogInformation($"Attempt {attempt} completed in {stopwatch.ElapsedMilliseconds} ms");
                    return result;
                }
            }
            catch (OperationCanceledException ex)
            {
                lastException = ex;
                _logger.LogWarning($"Attempt {attempt} cancelled/timed out: {ex.Message}");
                
                if (attempt < MaxRetries)
                {
                    _logger.LogInformation($"Waiting {RetryDelayMs}ms before retry...");
                    await Task.Delay(RetryDelayMs);
                }
            }
            catch (HttpRequestException ex)
            {
                lastException = ex;
                _logger.LogWarning($"Attempt {attempt} failed with HTTP error: {ex.Message}");
                
                if (attempt < MaxRetries)
                {
                    _logger.LogInformation($"Waiting {RetryDelayMs}ms before retry...");
                    await Task.Delay(RetryDelayMs);
                }
            }
            catch (Exception ex)
            {
                lastException = ex;
                _logger.LogWarning($"Attempt {attempt} failed: {ex.Message}");
                
                if (attempt < MaxRetries)
                {
                    _logger.LogInformation($"Waiting {RetryDelayMs}ms before retry...");
                    await Task.Delay(RetryDelayMs);
                }
            }
        }

        _logger.LogError($"All {MaxRetries} attempts failed. Last error: {lastException?.Message}");
        throw new Exception($"Failed to fetch external data after {MaxRetries} attempts", lastException);
    }
}