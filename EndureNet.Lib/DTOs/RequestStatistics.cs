using System.Net;

namespace EndureNet.Lib.DTOs;

public record RequestStatistics
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration => EndTime - StartTime;
    public HttpStatusCode StatusCode { get; set; }
    public string? ErrorMessage { get; set; }
    public int UserId { get; set; }
}