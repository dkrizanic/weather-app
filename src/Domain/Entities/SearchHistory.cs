namespace Domain.Entities;

public class SearchHistory
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public DateTime SearchDate { get; set; } = DateTime.UtcNow;
    public string WeatherCondition { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
}
