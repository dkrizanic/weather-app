using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests.Infrastructure;

public class SearchHistoryRepositoryTests
{
    private ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_Should_AddSearchHistoryToDatabase()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new SearchHistoryRepository(context);
        var searchHistory = new SearchHistory
        {
            UserId = "user123",
            City = "London",
            Country = "UK",
            Temperature = 15.5,
            WeatherCondition = "Cloudy",
            Description = "Light clouds",
            SearchDate = DateTime.UtcNow
        };

        // Act
        var result = await repository.CreateAsync(searchHistory);
        await context.SaveChangesAsync();

        // Assert
        result.Should().NotBeNull();
        result.City.Should().Be("London");
        var saved = await context.SearchHistories.FirstOrDefaultAsync();
        saved.Should().NotBeNull();
        saved!.City.Should().Be("London");
    }

    [Fact]
    public async Task GetByUserIdAsync_Should_ReturnUserSearchHistories()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new SearchHistoryRepository(context);
        var userId = "user123";
        
        var histories = new List<SearchHistory>
        {
            new SearchHistory { UserId = userId, City = "London", SearchDate = DateTime.UtcNow.AddHours(-2) },
            new SearchHistory { UserId = userId, City = "Paris", SearchDate = DateTime.UtcNow.AddHours(-1) },
            new SearchHistory { UserId = "other", City = "Berlin", SearchDate = DateTime.UtcNow }
        };

        context.SearchHistories.AddRange(histories);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByUserIdAsync(userId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(s => s.UserId == userId);
    }

    [Fact]
    public async Task GetRecentByUserIdAsync_Should_ReturnLimitedResults()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new SearchHistoryRepository(context);
        var userId = "user123";
        
        for (int i = 0; i < 10; i++)
        {
            context.SearchHistories.Add(new SearchHistory
            {
                UserId = userId,
                City = $"City{i}",
                SearchDate = DateTime.UtcNow.AddHours(-i)
            });
        }
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetRecentByUserIdAsync(userId, 5);

        // Assert
        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetTopCitiesByUserIdAsync_Should_ReturnCitiesOrderedBySearchCount()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new SearchHistoryRepository(context);
        var userId = "user123";
        
        var histories = new List<SearchHistory>
        {
            new SearchHistory { UserId = userId, City = "London", SearchDate = DateTime.UtcNow },
            new SearchHistory { UserId = userId, City = "London", SearchDate = DateTime.UtcNow },
            new SearchHistory { UserId = userId, City = "London", SearchDate = DateTime.UtcNow },
            new SearchHistory { UserId = userId, City = "Paris", SearchDate = DateTime.UtcNow },
            new SearchHistory { UserId = userId, City = "Paris", SearchDate = DateTime.UtcNow },
            new SearchHistory { UserId = userId, City = "Berlin", SearchDate = DateTime.UtcNow }
        };

        context.SearchHistories.AddRange(histories);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetTopCitiesByUserIdAsync(userId, 5);

        // Assert
        result.Should().HaveCount(3);
        result["London"].Should().Be(3);
        result["Paris"].Should().Be(2);
        result["Berlin"].Should().Be(1);
    }

    [Fact]
    public async Task GetWeatherConditionDistributionAsync_Should_ReturnConditionCounts()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new SearchHistoryRepository(context);
        var userId = "user123";
        
        var histories = new List<SearchHistory>
        {
            new SearchHistory { UserId = userId, WeatherCondition = "Cloudy", SearchDate = DateTime.UtcNow },
            new SearchHistory { UserId = userId, WeatherCondition = "Cloudy", SearchDate = DateTime.UtcNow },
            new SearchHistory { UserId = userId, WeatherCondition = "Sunny", SearchDate = DateTime.UtcNow }
        };

        context.SearchHistories.AddRange(histories);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetWeatherConditionDistributionAsync(userId);

        // Assert
        result.Should().HaveCount(2);
        result["Cloudy"].Should().Be(2);
        result["Sunny"].Should().Be(1);
    }
}
