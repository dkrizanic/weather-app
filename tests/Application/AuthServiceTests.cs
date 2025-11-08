using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Tests.Application;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly IAuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _configurationMock = new Mock<IConfiguration>();
        
        var jwtSettingsSection = new Mock<IConfigurationSection>();
        jwtSettingsSection.Setup(s => s["SecretKey"]).Returns("SuperSecretKeyForJwtTokenGeneration123456");
        jwtSettingsSection.Setup(s => s["Issuer"]).Returns("WeatherApp");
        jwtSettingsSection.Setup(s => s["Audience"]).Returns("WeatherAppUsers");
        
        _configurationMock.Setup(c => c.GetSection("JwtSettings")).Returns(jwtSettingsSection.Object);
        
        _authService = new AuthService(_userRepositoryMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_Should_ThrowException_WhenEmailExists()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "newuser",
            Email = "existing@example.com",
            Password = "Password123!"
        };

        _userRepositoryMock
            .Setup(r => r.ExistsAsync(registerDto.Email))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _authService.RegisterAsync(registerDto));
        exception.Message.Should().Contain("email already exists");
    }

    [Fact]
    public async Task RegisterAsync_Should_CreateUser_WhenDataIsValid()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "newuser",
            Email = "new@example.com",
            Password = "Password123!"
        };

        _userRepositoryMock
            .Setup(r => r.ExistsAsync(registerDto.Email))
            .ReturnsAsync(false);

        var createdUser = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = "hashedpassword"
        };

        _userRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync(createdUser);

        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.UserId.Should().NotBeNullOrEmpty();
        result.Username.Should().Be(registerDto.Username);
        result.Email.Should().Be(registerDto.Email);
        _userRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_Should_ThrowException_WhenUserNotFound()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "nonexistent@example.com",
            Password = "Password123!"
        };

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _authService.LoginAsync(loginDto));
        exception.Message.Should().Contain("Invalid email or password");
    }

    [Fact]
    public async Task LoginAsync_Should_ReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = "testuser",
            Email = loginDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(loginDto.Password)
        };

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.UserId.Should().Be(user.Id);
        result.Username.Should().Be(user.Username);
        result.Email.Should().Be(user.Email);
    }

    [Fact]
    public void GenerateJwtToken_Should_ReturnValidToken()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var email = "test@example.com";
        var username = "testuser";

        // Act
        var token = _authService.GenerateJwtToken(userId, email, username);

        // Assert
        token.Should().NotBeNullOrEmpty();
    }
}
