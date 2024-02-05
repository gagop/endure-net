// See https://aka.ms/new-console-template for more information

using EndureNet.Lib;

var scenario = ScenarioBuilder.Create()
    .WithNumberOfUsers(30)
    .WithDuration(TimeSpan.FromSeconds(60))
    .WithThinkTime(TimeSpan.FromSeconds(1))
    .WithRandomThinkTime(TimeSpan.FromSeconds(1))
    .Then(_ => new HttpRequestMessage().To("http://localhost:5036/WeatherForecast").WithMethod(HttpMethod.Get))
    .Then(response => new HttpRequestMessage().To("http://localhost:5036/WeatherForecast").WithMethod(HttpMethod.Post)
        .WithJsonContentFromPreviousMessage(response))
    .Build();

scenario.Run();

var statistics = scenario.GenerateSummary();

//Display the results
Console.WriteLine("Load test summary");
Console.WriteLine($"Average response time: {statistics.AverageResponseTime}ms");
Console.WriteLine();
Console.WriteLine("Number of responses with specific HTTP status codes");
foreach (var kvp in statistics.HttpStatusCodeCounts) Console.WriteLine($"{kvp.Key}: {kvp.Value}");

Console.WriteLine();
Console.WriteLine($"Number of errors: {statistics.TotalErrors}");
Console.WriteLine($"Total number of requests: {statistics.TotalRequests}");

foreach (var kvp in statistics.ErrorCounts) Console.WriteLine($"{kvp.Key}: {kvp.Value}");