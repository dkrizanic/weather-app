using Application.DTOs;

namespace Application.Interfaces;

public interface ISearchHistoryService
{
    Task<SearchHistoryDto> SaveSearchAsync(string userId, string city, WeatherSearchDto searchDto, CurrentWeatherDto weatherData);
    Task<IEnumerable<SearchHistoryDto>> GetUserSearchHistoryAsync(string userId);
    Task<UserStatisticsDto> GetUserStatisticsAsync(string userId);
}
