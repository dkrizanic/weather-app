using Application.DTOs;

namespace Application.Interfaces;

public interface IWeatherService
{
    Task<CurrentWeatherDto> GetCurrentWeatherAsync(string city);
    Task<CurrentWeatherDto> GetCurrentWeatherByCoordinatesAsync(double lat, double lon);
    Task<ForecastDto> GetForecastAsync(string city);
}
