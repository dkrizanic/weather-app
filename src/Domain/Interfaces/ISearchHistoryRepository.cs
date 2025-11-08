using Domain.Entities;

namespace Domain.Interfaces;

public interface ISearchHistoryRepository
{
    Task<SearchHistory> CreateAsync(SearchHistory searchHistory);
    Task<IEnumerable<SearchHistory>> GetByUserIdAsync(string userId);
    Task<IEnumerable<SearchHistory>> GetRecentByUserIdAsync(string userId, int count);
    Task<Dictionary<string, int>> GetTopCitiesByUserIdAsync(string userId, int count);
    Task<Dictionary<string, int>> GetWeatherConditionDistributionAsync(string userId);
}
