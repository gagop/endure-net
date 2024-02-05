# endure-net - simple library for quick load tests
EndureNet is a lightweight, user-friendly library to assist developers in configuring and executing load tests on their web applications and APIs. It enables creation of scenarios, each comprising at least one HTTP request. Users can incorporate multiple requests that occur in sequence, allowing a request to utilize the response from the previous request. We refer to such sequence as "scenario" representing a single user performing a sequence of requests. The example below demonstrates the library's usage.

### Installation
To get started with endure-net, install the NuGet package using the following command:
```
dotnet add package EndureNet.Lib 
```

### Example usage
```csharp
using EndureNet.Lib;

var scenario = ScenarioBuilder.Create()
    .WithNumberOfUsers(30)
    .WithDuration(TimeSpan.FromSeconds(60))
    .WithThinkTime(TimeSpan.FromSeconds(1))
    .WithRandomThinkTime(TimeSpan.FromSeconds(1))
    .Then(_ => new HttpRequestMessage(HttpMethod.Get, "http://localhost:5036/WeatherForecast"))
    .Then(response => new HttpRequestMessage(HttpMethod.Post, "http://localhost:5036/WeatherForecast")
        .WithJsonContentFromPreviousMessage(response))
    .Build();

await scenario.Run();

var statistics = scenario.GenerateSummary();

Console.WriteLine("Load test summary:");
Console.WriteLine($"Average response time: {statistics.AverageResponseTime}ms");
Console.WriteLine($"Total number of requests: {statistics.TotalRequests}");
Console.WriteLine("Number of responses with specific HTTP status codes:");
foreach (var kvp in statistics.HttpStatusCodeCounts)
{
    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
}
Console.WriteLine($"Number of errors: {statistics.TotalErrors}");