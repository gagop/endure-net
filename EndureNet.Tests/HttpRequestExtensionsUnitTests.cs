using EndureNet.Lib;
using Shouldly;

namespace EndureNet.Tests;

public class HttpRequestExtensionsUnitTests
{
    [Fact]
    public void WithAuthorization_AddsAuthorizationHeader()
    {
        // Arrange
        var request = new HttpRequestMessage();
        var token = "dummyToken";

        // Act
        var result = request.WithAuthorization(token);

        // Assert
        result.Headers.ShouldContain(e => e.Key == "Authorization" && e.Value.First() == $"Bearer {token}");
    }

    [Fact]
    public async Task WithBodyFromPreviousResponse_SetsContentFromResponse()
    {
        // Arrange
        var request = new HttpRequestMessage();
        var response = new HttpResponseMessage();
        response.Content = new StringContent("DummyContent");

        // Act
        var result = request.WithBodyFromPreviousResponse(response);

        // Assert
        result.ShouldNotBeNull();
        var content = await result.Content?.ReadAsStringAsync()!;
        content.ShouldBe("DummyContent");
    }

    [Fact]
    public async Task WithJsonContentFromPreviousMessage_SetsJsonContentFromResponse()
    {
        // Arrange
        var request = new HttpRequestMessage();
        var response = new HttpResponseMessage();
        response.Content = new StringContent("{\"key\": \"value\"}");

        // Act
        var result = request.WithJsonContentFromPreviousMessage(response);

        // Assert
        result.ShouldNotBeNull();
        var content = await result.Content?.ReadAsStringAsync()!;
        content.ShouldBe("{\"key\": \"value\"}");
        result.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");
    }

    [Fact]
    public void WithMethod_SetsRequestMethod()
    {
        // Arrange
        var request = new HttpRequestMessage();
        var method = HttpMethod.Get;

        // Act
        var result = request.WithMethod(method);

        // Assert
        result.Method.ShouldBe(HttpMethod.Get);
    }

    [Fact]
    public void WithHeader_AddsCustomHeader()
    {
        // Arrange
        var request = new HttpRequestMessage();
        var name = "CustomHeader";
        var value = "CustomValue";

        // Act
        var result = request.WithHeader(name, value);

        // Assert
        result.Headers.ShouldContain(e => e.Key == name && e.Value.First() == value);
    }

    [Fact]
    public void To_SetsRequestUri()
    {
        // Arrange
        var request = new HttpRequestMessage();
        var url = "https://example.com";

        // Act
        var result = request.To(url);

        // Assert
        result.RequestUri.ShouldBe(result.RequestUri);
    }

    [Fact]
    public async Task WithContent_SetsHttpContent()
    {
        // Arrange
        var request = new HttpRequestMessage();
        var content = new StringContent("DummyContent");

        // Act
        var result = request.WithContent(content);

        // Assert
        result.ShouldNotBeNull();
        var contentFromRequest = await result.Content?.ReadAsStringAsync()!;
        contentFromRequest.ShouldBe("DummyContent");
    }
}