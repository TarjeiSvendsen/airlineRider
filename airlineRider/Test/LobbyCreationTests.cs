using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using airlineRider.DAL;
using airlineRider.Models.Auth;
using airlineRider.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace airlineRider.Test;


public class LobbyCreationTests
{
    private WebApplicationFactory<Program> _factory;
    private string AccessToken { get; set; }

    private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };
    private List<int> createdLobbyIds { get; set; } = new();

    [SetUp]
    public async Task Setup()
    {
        _factory = new TestingWebApplicationFactory();   
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        var requestBody = new LoginRequest();
        requestBody.Username = "test";
        requestBody.Password = "test";

        var response = await client.PostAsync("/api/user/login", new StringContent(JsonSerializer.Serialize(requestBody),Encoding.UTF8,"application/json"));
        var responseBody = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseBody,_serializerOptions);
        AccessToken = tokenResponse.AccessToken;
    }

    [Test]
    public async Task MakeSureLobbyIsCreatedSuccessfully()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization
            = new AuthenticationHeaderValue("Bearer", AccessToken);

        var lobbyDto = new LobbyCreationDto("test", "test", "...", "", true, 
            new LobbyCreationSettingsDto("BCS1",1,10000,DateOnly.Parse("2000-01-01"),DateOnly.Parse("2010-01-01")));
        var jsonString = JsonSerializer.Serialize(lobbyDto);
        var response = await client.PostAsync("/api/lobby/new",new StringContent(jsonString,Encoding.UTF8,"application/json"));
        var responseBody = await response.Content.ReadAsStringAsync();
        var responseDto = JsonSerializer.Deserialize<LobbyPublicDetailDto>(responseBody,_serializerOptions);
        createdLobbyIds.Add(responseDto.Id);
        Assert.That(responseDto.Slug, Is.EqualTo("test"));
    }


    [TearDown]
    public async Task Completion()
    {
        using var scope = _factory.Services.CreateScope();
        var lobbyService = scope.ServiceProvider.GetService<LobbyService>();
        foreach (int id in createdLobbyIds)
        {
            await lobbyService.DeleteLobbyAsync(id);
        }
    }
}