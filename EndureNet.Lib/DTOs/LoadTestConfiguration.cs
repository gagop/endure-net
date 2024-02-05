namespace EndureNet.Lib.DTOs;

public record LoadTestConfiguration
{
    public int NumberOfUsers { get; set; } = 1;
    public TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(2);
    public TimeSpan ThinkTime { get; set; } = TimeSpan.FromSeconds(1);
    public TimeSpan RandomThinkTime { get; set; } = TimeSpan.FromSeconds(1);
}