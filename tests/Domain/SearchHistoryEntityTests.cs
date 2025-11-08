using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Tests.Domain;

public class SearchHistoryEntityTests
{
    [Fact]
    public void SearchHistory_Should_CreateWithValidProperties()
    {
        // Arrange & Act
        var searchHistory = new SearchHistory
        {
            Id = 1,
            UserId = "user123",
            City = "London",
            Country = "UK",
            Temperature = 15.5,
            WeatherCondition = "Cloudy",
            Description = "Light clouds",
            SearchDate = DateTime.UtcNow,
            Period = "current"
        };

        // Assert
        searchHistory.Id.Should().Be(1);
        searchHistory.UserId.Should().Be("user123");
        searchHistory.City.Should().Be("London");
        searchHistory.Country.Should().Be("UK");
        searchHistory.Temperature.Should().Be(15.5);
        searchHistory.WeatherCondition.Should().Be("Cloudy");
        searchHistory.SearchDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SearchHistory_Should_HaveDefaultSearchDate()
    {
        // Arrange & Act
        var searchHistory = new SearchHistory
        {
            UserId = "user123",
            City = "Paris"
        };

        // Assert
        searchHistory.SearchDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
