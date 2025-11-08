using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests.Infrastructure;

public class UserRepositoryTests
{
    private ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_Should_AddUserToDatabase()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new UserRepository(context);
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashedpassword"
        };

        // Act
        var result = await repository.CreateAsync(user);
        await context.SaveChangesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Username.Should().Be("testuser");
        var savedUser = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        savedUser.Should().NotBeNull();
        savedUser!.Username.Should().Be("testuser");
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnUser_WhenExists()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new UserRepository(context);
        var userId = Guid.NewGuid().ToString();
        var user = new User
        {
            Id = userId,
            Username = "testuser",
            Email = "test@example.com"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(userId);
        result.Username.Should().Be("testuser");
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnNull_WhenNotExists()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new UserRepository(context);

        // Act
        var result = await repository.GetByIdAsync("nonexistent");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByUsernameAsync_Should_ReturnUser_WhenExists()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new UserRepository(context);
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByUsernameAsync("testuser");

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be("testuser");
    }

    [Fact]
    public async Task GetByEmailAsync_Should_ReturnUser_WhenExists()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new UserRepository(context);
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByEmailAsync("test@example.com");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task ExistsAsync_Should_ReturnTrue_WhenUserExists()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new UserRepository(context);
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync("test@example.com");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_Should_ReturnFalse_WhenUserNotExists()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new UserRepository(context);

        // Act
        var result = await repository.ExistsAsync("nonexistent@example.com");

        // Assert
        result.Should().BeFalse();
    }
}
