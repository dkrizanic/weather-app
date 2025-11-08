using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Application.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;
    private readonly ISearchHistoryService _searchHistoryService;

    public WeatherController(IWeatherService weatherService, ISearchHistoryService searchHistoryService)
    {
        _weatherService = weatherService;
        _searchHistoryService = searchHistoryService;
    }

    [HttpGet("current/{city}")]
    public async Task<ActionResult<CurrentWeatherDto>> GetCurrentWeather(string city)
    {
        try
        {
            var weather = await _weatherService.GetCurrentWeatherAsync(city);
            return Ok(weather);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("current/coordinates")]
    public async Task<ActionResult<CurrentWeatherDto>> GetCurrentWeatherByCoordinates([FromQuery] double lat, [FromQuery] double lon)
    {
        try
        {
            var weather = await _weatherService.GetCurrentWeatherByCoordinatesAsync(lat, lon);
            return Ok(weather);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("forecast")]
    public async Task<ActionResult<ForecastDto>> GetForecast([FromBody] WeatherSearchDto searchDto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var forecast = await _weatherService.GetForecastAsync(searchDto.City);
            
            // Get current weather to save in search history
            var currentWeather = await _weatherService.GetCurrentWeatherAsync(searchDto.City);
            
            // Save search history
            await _searchHistoryService.SaveSearchAsync(userId, searchDto.City, searchDto, currentWeather);

            return Ok(forecast);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<SearchHistoryDto>>> GetSearchHistory()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var history = await _searchHistoryService.GetUserSearchHistoryAsync(userId);
            return Ok(history);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("statistics")]
    public async Task<ActionResult<UserStatisticsDto>> GetStatistics()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var stats = await _searchHistoryService.GetUserStatisticsAsync(userId);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
