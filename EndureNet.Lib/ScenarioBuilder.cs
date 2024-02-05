using EndureNet.Lib.DTOs;
using Microsoft.Extensions.Logging;

namespace EndureNet.Lib;

public class ScenarioBuilder
{
    private readonly LoadTestConfiguration _config = new();
    private TestStep? _firstStep;
    private TestStep? _lastStep;

    private ILogger _logger;

    private ScenarioBuilder()
    {
        _logger = LoggerFactory
            .Create(logging => logging.AddConsole())
            .CreateLogger<ScenarioBuilder>();
    }

    /// <summary>
    /// Creates a new instance of the ScenarioBuilder.
    /// </summary>
    /// <returns>A new ScenarioBuilder instance.</returns>
    public static ScenarioBuilder Create()
    {
        return new ScenarioBuilder();
    }
    
    /// <summary>
    /// Configures the number of users to simulate in the load test.
    /// </summary>
    /// <param name="numberOfUsers">The number of users.</param>
    /// <returns>The current instance of ScenarioBuilder for method chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if numberOfUsers is less than or equal to 0.</exception>
    public ScenarioBuilder WithNumberOfUsers(int numberOfUsers)
    {
        if (numberOfUsers <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfUsers));

        _config.NumberOfUsers = numberOfUsers;
        return this;
    }

    /// <summary>
    /// Configures the duration of the load test.
    /// </summary>
    /// <param name="duration">The duration of the test.</param>
    /// <returns>The current instance of ScenarioBuilder for method chaining.</returns>
    public ScenarioBuilder WithDuration(TimeSpan duration)
    {
        _config.Duration = duration;
        return this;
    }

    /// <summary>
    /// Sets a fixed think time between requests in the scenario.
    /// </summary>
    /// <param name="thinkTime">The think time to wait between requests.</param>
    /// <returns>The current instance of ScenarioBuilder for method chaining.</returns>
    public ScenarioBuilder WithThinkTime(TimeSpan thinkTime)
    {
        _config.ThinkTime = thinkTime;
        return this;
    }

    /// <summary>
    /// Sets a random think time between requests to simulate more realistic user behavior.
    /// </summary>
    /// <param name="randomThinkTime">The maximum random think time.</param>
    /// <returns>The current instance of ScenarioBuilder for method chaining.</returns>
    public ScenarioBuilder WithRandomThinkTime(TimeSpan randomThinkTime)
    {
        _config.RandomThinkTime = randomThinkTime;
        return this;
    }

    /// <summary>
    /// Adds a step to the load test scenario. Each step represents a single HTTP request.
    /// </summary>
    /// <param name="requestFactory">A function that generates an HttpRequestMessage optionally using the response from the previous request.</param>
    /// <returns>The current instance of ScenarioBuilder for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if requestFactory is null.</exception>
    public ScenarioBuilder Then(Func<HttpResponseMessage?, HttpRequestMessage> requestFactory)
    {
        if (requestFactory == null) throw new ArgumentNullException(nameof(requestFactory));

        var step = TestStep.CreateRequest(requestFactory);
        if (_firstStep == null)
        {
            _firstStep = _lastStep = step;
            return this;
        }

        _lastStep.Next = step;
        _lastStep = step;
        return this;
    }

    /// <summary>
    /// Builds the load test scenario with the configured steps and settings.
    /// </summary>
    /// <returns>A LoadTest instance ready to be executed.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no steps have been defined in the scenario.</exception>
    public LoadTest Build()
    {
        if (_firstStep == null) throw new InvalidOperationException("No steps defined");

        return new LoadTest(_firstStep, _config);
    }
}