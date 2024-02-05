using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Net;
using EndureNet.Lib.DTOs;
using Microsoft.Extensions.Logging;

namespace EndureNet.Lib;

public class LoadTest
{
    private readonly LoadTestConfiguration _configuration;

    private readonly TestStep _firstStep;
    private readonly ILogger<LoadTest> _logger;
    private readonly ConcurrentBag<RequestStatistics> _requestStatistics = new();

    /// <summary>
    /// Initializes a new instance of the LoadTest class.
    /// </summary>
    /// <param name="firstStep">The first step in the test scenario.</param>
    /// <param name="configuration">The configuration for the load test.</param>
    public LoadTest(TestStep firstStep, LoadTestConfiguration configuration)
    {
        _firstStep = firstStep ?? throw new ArgumentNullException(nameof(firstStep));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = LoggerFactory
            .Create(logging => logging.AddConsole())
            .CreateLogger<LoadTest>();
    }

    /// <summary>
    /// Executes the load test based on the configured steps and user count.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public Task Run()
    {
        _logger.LogInformation("Starting load test...");
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(_configuration.Duration);

        var users = Enumerable.Range(1, _configuration.NumberOfUsers);

        var token = cancellationTokenSource.Token;
        try
        {
            Parallel.ForEach(users, new ParallelOptions { CancellationToken = cancellationTokenSource.Token },
                (userId, state) =>
                {
                    _logger.LogInformation($"User {userId} started");
                    RunUserScenario(userId, token).Wait(token);
                });
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Load test was stopped");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during load test");
        }

        _logger.LogInformation("Load test finished");
        return Task.CompletedTask;
    }

    private async Task RunUserScenario(int userId, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested) await ExecuteScenario(userId, _firstStep, cancellationToken);
    }

    private async Task ExecuteScenario(int userId, TestStep step, CancellationToken cancellationToken)
    {
        using var client = new HttpClient();

        var currentStep = step;
        HttpResponseMessage? lastResponseMessage = null;
        while (currentStep != null && !cancellationToken.IsCancellationRequested)
        {
            var thinkTime = (int)_configuration.ThinkTime.TotalMilliseconds;
            var randomThinkTime = new Random().Next((int)_configuration.RandomThinkTime.TotalMilliseconds);
            await Task.Delay(thinkTime + randomThinkTime, cancellationToken);

            if (step.RequestFactory == null) throw new InvalidOperationException("Request factory is not set");

            var stats = new RequestStatistics
            {
                StartTime = DateTime.UtcNow,
                UserId = userId
            };

            try
            {
                var requestMessage = currentStep.RequestFactory!(lastResponseMessage);
                lastResponseMessage = await client.SendAsync(requestMessage, cancellationToken);
                stats.StatusCode = lastResponseMessage.StatusCode;
            }
            catch (Exception ex)
            {
                stats.ErrorMessage = ex.ToString();
                stats.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError(ex, "Error during request");
            }
            finally
            {
                stats.EndTime = DateTime.UtcNow;
                _requestStatistics.Add(stats);
            }

            currentStep = currentStep.Next;
        }
    }

    /// <summary>
    /// Retrieves an immutable array of all request statistics collected during the load test.
    /// </summary>
    /// <returns>An ImmutableArray containing the request statistics.</returns>
    public ImmutableArray<RequestStatistics> GetRequestsStatistics()
    {
        return _requestStatistics.ToImmutableArray();
    }

    /// <summary>
    /// Generates a summary of the load test, including average response time, status code counts, and error counts.
    /// </summary>
    /// <returns>A LoadTestSummary object containing the summary of the test results.</returns>
    public LoadTestSummary GenerateSummary()
    {
        var summary = new LoadTestSummary();

        if (!_requestStatistics.Any()) return summary;

        summary.AverageResponseTime = _requestStatistics.Average(stat => stat.Duration.TotalMilliseconds);
        foreach (var stat in _requestStatistics)
            if (!summary.HttpStatusCodeCounts.TryAdd(stat.StatusCode, 1))
                summary.HttpStatusCodeCounts[stat.StatusCode]++;

        summary.TotalErrors = _requestStatistics.Count(stat => stat.ErrorMessage != null);
        foreach (var errorStat in _requestStatistics.Where(stat => stat.ErrorMessage != null))
        {
            if (string.IsNullOrEmpty(errorStat.ErrorMessage)) continue;

            if (!summary.ErrorCounts.TryAdd(errorStat.ErrorMessage, 1)) summary.ErrorCounts[errorStat.ErrorMessage]++;
        }

        summary.TotalRequests = _requestStatistics.Count;

        return summary;
    }

    /// <summary>
    /// Gets the configuration settings used for this load test.
    /// </summary>
    /// <returns>The LoadTestConfiguration object used for this load test.</returns>
    public LoadTestConfiguration GetConfiguration()
    {
        return _configuration;
    }

    /// <summary>
    /// Gets the first step in the test scenario.
    /// </summary>
    /// <returns>The first TestStep defined in the load test.</returns>
    public TestStep GetFirstStep()
    {
        return _firstStep;
    }
}