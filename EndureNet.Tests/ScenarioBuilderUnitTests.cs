using EndureNet.Lib;
using Shouldly;

namespace EndureNet.Tests;

public class ScenarioBuilderUnitTests
{
    [Fact]
    public void Then_NullRequestFactory_ThrowsArgumentNullException()
    {
        // Arrange
        var builder = ScenarioBuilder.Create();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => { builder.Then(null); });
    }

    [Fact]
    public void Then_ValidRequestFactory_AddsStep()
    {
        // Arrange
        var builder = ScenarioBuilder.Create();
        Func<HttpResponseMessage?, HttpRequestMessage> requestFactory = response => new HttpRequestMessage();
        builder.Then(requestFactory);

        // Act
        var loadTest = builder.Build();

        // Assert
        loadTest.ShouldNotBeNull();
        loadTest.GetFirstStep().ShouldNotBeNull();
        loadTest.GetFirstStep().RequestFactory.ShouldBe(requestFactory);
    }

    [Fact]
    public void Build_NoStepsDefined_ThrowsInvalidOperationException()
    {
        // Arrange
        var builder = ScenarioBuilder.Create();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => { builder.Build(); });
    }

    [Fact]
    public void Build_StepsDefined_ReturnsLoadTest()
    {
        // Arrange
        var builder = ScenarioBuilder.Create();
        Func<HttpResponseMessage?, HttpRequestMessage> requestFactory = response => new HttpRequestMessage();
        builder.WithDuration(TimeSpan.FromSeconds(60))
            .WithNumberOfUsers(30)
            .WithRandomThinkTime(TimeSpan.FromSeconds(1))
            .WithThinkTime(TimeSpan.FromSeconds(1))
            .Then(requestFactory);

        // Act
        var loadTest = builder.Build();

        // Assert
        var config = loadTest.GetConfiguration();
        config.ThinkTime.ShouldBe(TimeSpan.FromSeconds(1));
        config.RandomThinkTime.ShouldBe(TimeSpan.FromSeconds(1));
        config.Duration.ShouldBe(TimeSpan.FromSeconds(60));
        config.NumberOfUsers.ShouldBe(30);
    }
}