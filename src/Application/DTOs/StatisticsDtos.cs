namespace Application.DTOs;

public class SearchHistoryDto
{
    public int Id { get; set; }
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public DateTime SearchDate { get; set; }
    public string WeatherCondition { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
}

public class TopCityDto
{
    public string City { get; set; } = string.Empty;
    public int SearchCount { get; set; }
}

public class WeatherConditionStatsDto
{
    public string Condition { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class UserStatisticsDto
{
    public List<TopCityDto> TopCities { get; set; } = new();
    public List<SearchHistoryDto> RecentSearches { get; set; } = new();
    public List<WeatherConditionStatsDto> WeatherDistribution { get; set; } = new();
}
