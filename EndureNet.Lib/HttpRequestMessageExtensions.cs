using System.Text;

namespace EndureNet.Lib;

public static class HttpRequestMessageExtensions
{
    public static HttpRequestMessage WithAuthorization(this HttpRequestMessage request, string token)
    {
        request.Headers.Add("Authorization", $"Bearer {token}");
        return request;
    }

    public static HttpRequestMessage WithBodyFromPreviousResponse(this HttpRequestMessage request,
        HttpResponseMessage response)
    {
        request.Content = new StringContent(response.Content.ReadAsStringAsync().Result);
        return request;
    }

    public static HttpRequestMessage WithJsonContentFromPreviousMessage(this HttpRequestMessage request,
        HttpResponseMessage response)
    {
        request.Content =
            new StringContent(response.Content.ReadAsStringAsync().Result, Encoding.UTF8, "application/json");
        return request;
    }

    public static HttpRequestMessage WithMethod(this HttpRequestMessage request, HttpMethod method)
    {
        request.Method = method;
        return request;
    }

    public static HttpRequestMessage WithHeader(this HttpRequestMessage request, string name, string value)
    {
        request.Headers.Add(name, value);
        return request;
    }

    public static HttpRequestMessage To(this HttpRequestMessage request, string url)
    {
        request.RequestUri = new Uri(url);
        return request;
    }

    public static HttpRequestMessage WithContent(this HttpRequestMessage request, HttpContent content)
    {
        request.Content = content;
        return request;
    }
}