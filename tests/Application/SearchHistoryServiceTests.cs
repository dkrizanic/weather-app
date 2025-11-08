using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Infrastructure.Services;
using Moq;
using Xunit;

namespace Tests.Application;

public class SearchHistoryServiceTests
{
    private readonly Mock<ISearchHistoryRepository> _repositoryMock;
    private readonly ISearchHistoryService _service;

    public SearchHistoryServiceTests()
    {
        _repositoryMock = new Mock<ISearchHistoryRepository>();
        _service = new SearchHistoryService(_repositoryMock.Object);
    }

    [Fact]
    public async Task SaveSearchAsync_Should_SaveSearchHistory()
    {
        // Arrange
        var userId = "user123";
        var city = "London";
        var searchDto = new WeatherSearchDto { City = city, Period = "current" };
        var weatherDto = new CurrentWeatherDto
        {
            City = "London",
            Country = "UK",
            Temperature = 15.5,
            Condition = "Cloudy",
            Description = "Light clouds",
            Humidity = 70,
            WindSpeed = 5.5
        };

        var savedHistory = new SearchHistory
        {
            Id = 1,
            UserId = userId,
            City = weatherDto.City,
            Country = weatherDto.Country,
            Temperature = weatherDto.Temperature,
            WeatherCondition = weatherDto.Condition,
            Description = weatherDto.Description,
            Period = searchDto.Period,
            SearchDate = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<SearchHistory>()))
            .ReturnsAsync(savedHistory);

        // Act
        var result = await _service.SaveSearchAsync(userId, city, searchDto, weatherDto);

        // Assert
        result.Should().NotBeNull();
        result.City.Should().Be("London");
        result.Country.Should().Be("UK");
        result.Temperature.Should().Be(15.5);
        result.WeatherCondition.Should().Be("Cloudy");
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<SearchHistory>()), Times.Once);
    }

    [Fact]
    public async Task GetUserSearchHistoryAsync_Should_ReturnSearchHistory()
    {
        // Arrange
        var userId = "user123";
        var searchHistories = new List<SearchHistory>
        {
            new SearchHistory
            {
                Id = 1,
                UserId = userId,
                City = "London",
                Country = "UK",
                Temperature = 15.5,
                WeatherCondition = "Cloudy",
                Description = "Light clouds",
                SearchDate = DateTime.UtcNow
            },
            new SearchHistory
            {
                Id = 2,
                UserId = userId,
                City = "Paris",
                Country = "FR",
                Temperature = 18.0,
                WeatherCondition = "Sunny",
                Description = "Clear sky",
                SearchDate = DateTime.UtcNow.AddHours(-1)
            }
        };

        _repositoryMock
            .Setup(r => r.GetByUserIdAsync(userId))
            .ReturnsAsync(searchHistories);

        // Act
        var result = await _service.GetUserSearchHistoryAsync(userId);

        // Assert
        result.Should().HaveCount(2);
        result.First().City.Should().Be("London");
        result.Last().City.Should().Be("Paris");
    }

    [Fact]
    public async Task GetUserStatisticsAsync_Should_ReturnStatistics()
    {
        // Arrange
        var userId = "user123";
        
        var topCities = new Dictionary<string, int>
        {
            { "London", 5 },
            { "Paris", 3 }
        };

        var recentSearches = new List<SearchHistory>
        {
            new SearchHistory
            {
                Id = 1,
                UserId = userId,
                City = "London",
                Country = "UK",
                Temperature = 15.5,
                WeatherCondition = "Cloudy",
                Description = "Light clouds",
                SearchDate = DateTime.UtcNow
            }
        };

        var weatherDistribution = new Dictionary<string, int>
        {
            { "Cloudy", 10 },
            { "Sunny", 5 }
        };

        _repositoryMock
            .Setup(r => r.GetTopCitiesByUserIdAsync(userId, 3))
            .ReturnsAsync(topCities);

        _repositoryMock
            .Setup(r => r.GetRecentByUserIdAsync(userId, 3))
            .ReturnsAsync(recentSearches);

        _repositoryMock
            .Setup(r => r.GetWeatherConditionDistributionAsync(userId))
            .ReturnsAsync(weatherDistribution);

        // Act
        var result = await _service.GetUserStatisticsAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.TopCities.Should().HaveCount(2);
        result.TopCities.First().City.Should().Be("London");
        result.TopCities.First().SearchCount.Should().Be(5);
        result.RecentSearches.Should().HaveCount(1);
        result.WeatherDistribution.Should().HaveCount(2);
        result.WeatherDistribution.First().Condition.Should().Be("Cloudy");
        result.WeatherDistribution.First().Count.Should().Be(10);
    }
}
