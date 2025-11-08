using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Services;

public class SearchHistoryService : ISearchHistoryService
{
    private readonly ISearchHistoryRepository _searchHistoryRepository;

    public SearchHistoryService(ISearchHistoryRepository searchHistoryRepository)
    {
        _searchHistoryRepository = searchHistoryRepository;
    }

    public async Task<SearchHistoryDto> SaveSearchAsync(string userId, string city, WeatherSearchDto searchDto, CurrentWeatherDto weatherData)
    {
        var searchHistory = new SearchHistory
        {
            UserId = userId,
            City = weatherData.City,
            Country = weatherData.Country,
            SearchDate = DateTime.UtcNow,
            WeatherCondition = weatherData.Condition,
            Temperature = weatherData.Temperature,
            Description = weatherData.Description,
            Period = searchDto.Period
        };

        var saved = await _searchHistoryRepository.CreateAsync(searchHistory);

        return new SearchHistoryDto
        {
            Id = saved.Id,
            City = saved.City,
            Country = saved.Country,
            SearchDate = saved.SearchDate,
            WeatherCondition = saved.WeatherCondition,
            Temperature = saved.Temperature,
            Description = saved.Description,
            Period = saved.Period
        };
    }

    public async Task<IEnumerable<SearchHistoryDto>> GetUserSearchHistoryAsync(string userId)
    {
        var histories = await _searchHistoryRepository.GetByUserIdAsync(userId);
        
        return histories.Select(h => new SearchHistoryDto
        {
            Id = h.Id,
            City = h.City,
            Country = h.Country,
            SearchDate = h.SearchDate,
            WeatherCondition = h.WeatherCondition,
            Temperature = h.Temperature,
            Description = h.Description,
            Period = h.Period
        });
    }

    public async Task<UserStatisticsDto> GetUserStatisticsAsync(string userId)
    {
        var topCitiesDict = await _searchHistoryRepository.GetTopCitiesByUserIdAsync(userId, 3);
        var recentSearches = await _searchHistoryRepository.GetRecentByUserIdAsync(userId, 3);
        var weatherDistribution = await _searchHistoryRepository.GetWeatherConditionDistributionAsync(userId);

        return new UserStatisticsDto
        {
            TopCities = topCitiesDict.Select(kvp => new TopCityDto
            {
                City = kvp.Key,
                SearchCount = kvp.Value
            }).ToList(),
            RecentSearches = recentSearches.Select(h => new SearchHistoryDto
            {
                Id = h.Id,
                City = h.City,
                Country = h.Country,
                SearchDate = h.SearchDate,
                WeatherCondition = h.WeatherCondition,
                Temperature = h.Temperature,
                Description = h.Description,
                Period = h.Period
            }).ToList(),
            WeatherDistribution = weatherDistribution.Select(kvp => new WeatherConditionStatsDto
            {
                Condition = kvp.Key,
                Count = kvp.Value
            }).ToList()
        };
    }
}
