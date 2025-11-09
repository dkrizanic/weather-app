using Application.DTOs;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Infrastructure.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string BaseUrl = "https://api.openweathermap.org/data/2.5";

    public WeatherService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenWeather:ApiKey"] ?? throw new Exception("OpenWeather API Key not configured");
    }

    public async Task<CurrentWeatherDto> GetCurrentWeatherAsync(string city)
    {
        var url = $"{BaseUrl}/weather?q={city}&appid={_apiKey}&units=metric";
        var response = await _httpClient.GetAsync(url);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to fetch weather data for {city}");
        }

        var json = await response.Content.ReadAsStringAsync();
        var weatherData = JsonSerializer.Deserialize<JsonElement>(json);

        return new CurrentWeatherDto
        {
            City = weatherData.GetProperty("name").GetString() ?? city,
            Country = weatherData.GetProperty("sys").GetProperty("country").GetString() ?? "",
            Temperature = weatherData.GetProperty("main").GetProperty("temp").GetDouble(),
            FeelsLike = weatherData.GetProperty("main").GetProperty("feels_like").GetDouble(),
            Humidity = weatherData.GetProperty("main").GetProperty("humidity").GetInt32(),
            WindSpeed = weatherData.GetProperty("wind").GetProperty("speed").GetDouble(),
            Condition = weatherData.GetProperty("weather")[0].GetProperty("main").GetString() ?? "",
            Description = weatherData.GetProperty("weather")[0].GetProperty("description").GetString() ?? "",
            Icon = weatherData.GetProperty("weather")[0].GetProperty("icon").GetString() ?? "",
            DateTime = DateTime.UtcNow
        };
    }

    public async Task<CurrentWeatherDto> GetCurrentWeatherByCoordinatesAsync(double lat, double lon)
    {
        var url = $"{BaseUrl}/weather?lat={lat}&lon={lon}&appid={_apiKey}&units=metric";
        var response = await _httpClient.GetAsync(url);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to fetch weather data for coordinates {lat}, {lon}");
        }

        var json = await response.Content.ReadAsStringAsync();
        var weatherData = JsonSerializer.Deserialize<JsonElement>(json);

        return new CurrentWeatherDto
        {
            City = weatherData.GetProperty("name").GetString() ?? "Unknown",
            Country = weatherData.GetProperty("sys").GetProperty("country").GetString() ?? "",
            Temperature = weatherData.GetProperty("main").GetProperty("temp").GetDouble(),
            FeelsLike = weatherData.GetProperty("main").GetProperty("feels_like").GetDouble(),
            Humidity = weatherData.GetProperty("main").GetProperty("humidity").GetInt32(),
            WindSpeed = weatherData.GetProperty("wind").GetProperty("speed").GetDouble(),
            Condition = weatherData.GetProperty("weather")[0].GetProperty("main").GetString() ?? "",
            Description = weatherData.GetProperty("weather")[0].GetProperty("description").GetString() ?? "",
            Icon = weatherData.GetProperty("weather")[0].GetProperty("icon").GetString() ?? "",
            DateTime = DateTime.UtcNow
        };
    }

    public async Task<ForecastDto> GetForecastAsync(string city)
    {
        var url = $"{BaseUrl}/forecast?q={city}&appid={_apiKey}&units=metric";
        var response = await _httpClient.GetAsync(url);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to fetch forecast data for {city}");
        }

        var json = await response.Content.ReadAsStringAsync();
        var forecastData = JsonSerializer.Deserialize<JsonElement>(json);

        var cityData = forecastData.GetProperty("city");
        var list = forecastData.GetProperty("list");

        // Group by day and take one forecast per day (around noon)
        var dailyForecasts = new Dictionary<string, ForecastDayDto>();

        foreach (var item in list.EnumerateArray())
        {
            var dt = DateTimeOffset.FromUnixTimeSeconds(item.GetProperty("dt").GetInt64()).DateTime;
            var dateKey = dt.Date.ToString("yyyy-MM-dd");

            // Take the forecast closest to noon (12:00)
            if (!dailyForecasts.ContainsKey(dateKey) || Math.Abs(dt.Hour - 12) < Math.Abs(dailyForecasts[dateKey].Date.Hour - 12))
            {
                dailyForecasts[dateKey] = new ForecastDayDto
                {
                    Date = dt,
                    Temp = item.GetProperty("main").GetProperty("temp").GetDouble(),
                    TempMin = item.GetProperty("main").GetProperty("temp_min").GetDouble(),
                    TempMax = item.GetProperty("main").GetProperty("temp_max").GetDouble(),
                    Humidity = item.GetProperty("main").GetProperty("humidity").GetInt32(),
                    WindSpeed = item.GetProperty("wind").GetProperty("speed").GetDouble(),
                    Condition = item.GetProperty("weather")[0].GetProperty("main").GetString() ?? "",
                    Description = item.GetProperty("weather")[0].GetProperty("description").GetString() ?? "",
                    Icon = item.GetProperty("weather")[0].GetProperty("icon").GetString() ?? "",
                    Precipitation = item.TryGetProperty("rain", out var rain) && rain.TryGetProperty("3h", out var rain3h) 
                        ? rain3h.GetDouble() 
                        : 0
                };
            }
        }

        return new ForecastDto
        {
            City = cityData.GetProperty("name").GetString() ?? city,
            Country = cityData.GetProperty("country").GetString() ?? "",
            Days = dailyForecasts.Values.OrderBy(d => d.Date).ToList()
        };
    }
}
