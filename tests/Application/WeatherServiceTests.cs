using Application.DTOs;
using Application.Interfaces;
using FluentAssertions;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using Xunit;

namespace Tests.Application;

public class WeatherServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly IWeatherService _service;

    public WeatherServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://api.openweathermap.org/")
        };

        _configurationMock = new Mock<IConfiguration>();
        _configurationMock.Setup(c => c["OpenWeather:ApiKey"]).Returns("test-api-key");

        _service = new WeatherService(_httpClient, _configurationMock.Object);
    }

    [Fact]
    public async Task GetCurrentWeatherAsync_Should_ReturnWeatherData()
    {
        // Arrange
        var city = "London";
        var responseContent = new
        {
            name = "London",
            sys = new { country = "UK" },
            main = new { temp = 15.5, feels_like = 14.0, humidity = 70 },
            weather = new[] { new { main = "Clouds", description = "cloudy", icon = "04d" } },
            wind = new { speed = 5.5 }
        };

        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(responseContent))
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _service.GetCurrentWeatherAsync(city);

        // Assert
        result.Should().NotBeNull();
        result.City.Should().Be("London");
        result.Country.Should().Be("UK");
        result.Temperature.Should().Be(15.5);
        result.Humidity.Should().Be(70);
        result.Condition.Should().Be("Clouds");
        result.Description.Should().Be("cloudy");
        result.WindSpeed.Should().Be(5.5);
    }

    [Fact]
    public async Task GetCurrentWeatherAsync_Should_ThrowException_WhenCityNotFound()
    {
        // Arrange
        var city = "InvalidCity";
        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = new StringContent("{\"message\":\"city not found\"}")
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetCurrentWeatherAsync(city));
    }

    [Fact]
    public async Task GetForecastAsync_Should_ReturnForecastData()
    {
        // Arrange
        var city = "London";
        var responseContent = new
        {
            city = new { name = "London", country = "UK" },
            list = new[]
            {
                new
                {
                    dt = 1704110400L, // 2024-01-01 12:00:00 UTC
                    dt_txt = "2024-01-01 12:00:00",
                    main = new { temp = 15.5, temp_min = 14.0, temp_max = 17.0, humidity = 70 },
                    weather = new[] { new { main = "Clouds", description = "cloudy", icon = "04d" } },
                    wind = new { speed = 5.5 }
                },
                new
                {
                    dt = 1704196800L, // 2024-01-02 12:00:00 UTC
                    dt_txt = "2024-01-02 12:00:00",
                    main = new { temp = 16.0, temp_min = 15.0, temp_max = 18.0, humidity = 65 },
                    weather = new[] { new { main = "Clear", description = "sunny", icon = "01d" } },
                    wind = new { speed = 4.0 }
                }
            }
        };

        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(responseContent))
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _service.GetForecastAsync(city);

        // Assert
        result.Should().NotBeNull();
        result.City.Should().Be("London");
        result.Country.Should().Be("UK");
        result.Days.Should().HaveCount(2);
        result.Days.First().Temp.Should().Be(15.5);
        result.Days.Last().Temp.Should().Be(16.0);
    }

    [Fact]
    public async Task GetForecastAsync_Should_ThrowException_WhenCityNotFound()
    {
        // Arrange
        var city = "InvalidCity";
        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = new StringContent("{\"message\":\"city not found\"}")
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetForecastAsync(city));
    }

    [Fact]
    public async Task GetCurrentWeatherByCoordinatesAsync_Should_ReturnWeatherData()
    {
        // Arrange
        var lat = 51.5074;
        var lon = -0.1278;
        var responseContent = new
        {
            name = "London",
            sys = new { country = "UK" },
            main = new { temp = 15.5, feels_like = 14.0, humidity = 70 },
            weather = new[] { new { main = "Clouds", description = "cloudy", icon = "04d" } },
            wind = new { speed = 5.5 }
        };

        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(responseContent))
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _service.GetCurrentWeatherByCoordinatesAsync(lat, lon);

        // Assert
        result.Should().NotBeNull();
        result.City.Should().Be("London");
        result.Temperature.Should().Be(15.5);
    }
}
