using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Tests.Domain;

public class UserEntityTests
{
    [Fact]
    public void User_Should_CreateWithValidProperties()
    {
        // Arrange & Act
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashedpassword",
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        user.Username.Should().Be("testuser");
        user.Email.Should().Be("test@example.com");
        user.PasswordHash.Should().Be("hashedpassword");
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void User_Should_InitializeSearchHistoriesCollection()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        user.SearchHistories.Should().NotBeNull();
        user.SearchHistories.Should().BeEmpty();
    }

    [Fact]
    public void User_Should_GenerateNewGuidForId()
    {
        // Arrange & Act
        var user1 = new User();
        var user2 = new User();

        // Assert
        user1.Id.Should().NotBeNullOrEmpty();
        user2.Id.Should().NotBeNullOrEmpty();
        user1.Id.Should().NotBe(user2.Id);
    }
}
