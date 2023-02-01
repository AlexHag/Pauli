using FluentAssertions;
using server.Services;

namespace server.Tests;

public class PauliHelperTest
{
    [Fact]
    public void RandomStringShouldBeCorrectLength()
    {
        // Act
        var pauliHelper = new PauliHelper();

        // Arrange
        var randomString = pauliHelper.RandomString(6);

        // Assert
        randomString.Length.Should().Be(6);
    }

    [Fact]
    public void SHA256ShouldReturnCorrectSHA256()
    {
        // Act
        var pauliHelper = new PauliHelper();

        // Arrange
        var helloHash = pauliHelper.HashString("hello");

        // Assert
        helloHash.Should().Be("2cf24dba5fb0a30e26e83b2ac5b9e29e1b161e5c1fa7425e73043362938b9824");
    }
}