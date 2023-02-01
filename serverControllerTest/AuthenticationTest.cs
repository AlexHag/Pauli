using System.Text.Json;
using System.Net;
using System.Text;
using Xunit.Abstractions;
using FluentAssertions;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace serverControllerTest;

public class AuthenticationTest
{
    ITestOutputHelper _outputHelper;
    public AuthenticationTest(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public async void RegisterRandomUserShouldReturnOk()
    {
        // Arrange
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        Random random = new Random();
        var randomUsername = new string(Enumerable.Repeat(chars, 10)
            .Select(s => s[random.Next(s.Length)]).ToArray());

        var user = new { username = randomUsername, password = "password" };
        var content = new StringContent(JsonSerializer.Serialize(user), System.Text.Encoding.UTF8, "application/json");
        HttpClient _httpClient = new() { BaseAddress = new Uri("http://localhost:5137") };
        
        // Act
        var response = await _httpClient.PostAsync("api/register", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async void RegisterWithExistingUsernameReturnsBadRequest()
    {
        // Arrange
        var user = new { username = "existinguser", password = "password" };
        var content = new StringContent(JsonSerializer.Serialize(user), System.Text.Encoding.UTF8, "application/json");
        HttpClient _httpClient = new() { BaseAddress = new Uri("http://localhost:5137") };
        
        // Act
        var response = await _httpClient.PostAsync("api/register", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async void LoginWrongPasswordShouldReturnBadRequest()
    {
        // Arrange
        var user = new { username = "alex", password = "wrong" };
        var content = new StringContent(JsonSerializer.Serialize(user), System.Text.Encoding.UTF8, "application/json");
        HttpClient _httpClient = new() { BaseAddress = new Uri("http://localhost:5137") };
        
        // Act
        var response = await _httpClient.PostAsync("api/login", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async void LoginShouldReturnJWT()
    {
        // Arrange
        var user = new { username = "alex", password = "password" };
        var content = new StringContent(JsonSerializer.Serialize(user), System.Text.Encoding.UTF8, "application/json");
        HttpClient _httpClient = new() { BaseAddress = new Uri("http://localhost:5137") };
        
        // Act
        var response = await _httpClient.PostAsync("api/login", content);

        _outputHelper.WriteLine(response.Content.ReadAsStringAsync().Result);
        response.Content.ReadAsStringAsync().Result.Should().StartWith("{\"token\":\"ey");
    }

    [Fact]
    public async void LoginAndGetProtectedResourceWithValidTokenShouldReturnUsername()
    {
        // Arrange
        var user = new { username = "alex", password = "password" };
        var content = new StringContent(JsonSerializer.Serialize(user), System.Text.Encoding.UTF8, "application/json");
        HttpClient _httpClient = new() { BaseAddress = new Uri("http://localhost:5137") };
        
        // Act
        var LoginResponse = await _httpClient.PostAsync("api/login", content);
        var ResponseToken = LoginResponse.Content.ReadAsStringAsync().Result;
        var Token = JObject.Parse(ResponseToken)["token"].ToString();

        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
        
        var ProtectedResourceResponse = await _httpClient.GetAsync("api/Secure");

        // Assert
        Assert.Equal(HttpStatusCode.OK, ProtectedResourceResponse.StatusCode);
        ProtectedResourceResponse.Content.ReadAsStringAsync().Result.Should().Be("Hello alex");
    }

    [Fact]
    public async void LoginAndGetProtectedResourceWithInvalidTokenShouldReturnUnauthorized()
    {
        // Arrange
        HttpClient _httpClient = new() { BaseAddress = new Uri("http://localhost:5137") };
        
        // Act
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + "eyJhbGciOiJIUzI1NiIsInR5c");        
        var ProtectedResourceResponse = await _httpClient.GetAsync("api/Secure");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, ProtectedResourceResponse.StatusCode);
    }


}