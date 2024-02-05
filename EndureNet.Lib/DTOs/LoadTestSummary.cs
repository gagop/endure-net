using System.Net;

namespace EndureNet.Lib.DTOs;

public record LoadTestSummary
{
    public double AverageResponseTime { get; set; }
    public Dictionary<HttpStatusCode, int> HttpStatusCodeCounts { get; set; } = new();
    public int TotalErrors { get; set; }
    public int TotalRequests { get; set; }
    public Dictionary<string, int> ErrorCounts { get; set; } = new();
}