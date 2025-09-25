using FluentAssertions;
using game_of_life_api.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using game_of_life_api;
using System.Net;
using System.Net.Http.Json;

namespace game_of_life_api.integrationtests.Controllers;

public class BoardsControllerTests
{
    private readonly HttpClient _client;

    public BoardsControllerTests()
    {
        var factory = new WebApplicationFactory<Program>();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Upload_Should_Return201AndId()
    {
        var req = new UploadBoardRequest
        {
            Name = "test",
            Rows = 3,
            Cols = 3,
            Cells = new[]
            {
                new[] { false, true, false },
                new[] { false, true, false },
                new[] { false, true, false }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/boards", req);

        response.StatusCode.Should().Be(HttpStatusCode.OK); // or 201 if we switch CreatedAtAction
        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        body.Should().ContainKey("id");
    }

    [Fact]
    public async Task Next_Should_ReturnCorrectBoard()
    {
        // Arrange - upload a vertical blinker
        var req = new UploadBoardRequest
        {
            Rows = 3,
            Cols = 3,
            Cells = new[]
            {
            new[] { false, true,  false },
            new[] { false, true,  false },
            new[] { false, true,  false }
        }
        };

        var upload = await _client.PostAsJsonAsync("/api/boards", req);
        upload.EnsureSuccessStatusCode();

        var uploadBody = await upload.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var id = uploadBody!["id"].ToString();

        // Act - call next
        var response = await _client.PostAsync($"/api/boards/{id}/next", null);
        response.EnsureSuccessStatusCode();

        var boardResponse = await response.Content.ReadFromJsonAsync<BoardResponse>();

        // Assert
        boardResponse.Should().NotBeNull();
        boardResponse!.Rows.Should().Be(3);
        boardResponse.Cols.Should().Be(3);

        // Expect horizontal blinker
        var expected = new[]
        {
            new[] { false, false, false },
            new[] { true,  true,  true  },
            new[] { false, false, false }
        };

        boardResponse.State.Should().BeEquivalentTo(expected);
    }


    [Fact]
    public async Task Advance_Should_ReturnCorrectBoard()
    {
        var req = new UploadBoardRequest
        {
            Rows = 3,
            Cols = 3,
            Cells = new[]
            {
                new[] { false, true, false },
                new[] { false, true, false },
                new[] { false, true, false }
            }
        };

        var upload = await _client.PostAsJsonAsync("/api/boards", req);
        var body = await upload.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var id = body!["id"].ToString();

        // Advance 2 steps
        var response = await _client.PostAsync($"/api/boards/{id}/advance?steps=2", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        result.Should().ContainKey("state");
    }

    [Fact]
    public async Task Final_Should_ReturnStableOrExtinct()
    {
        var req = new UploadBoardRequest
        {
            Rows = 2,
            Cols = 2,
            Cells = new[]
            {
                new[] { true, true },
                new[] { true, true }
            }
        };

        var upload = await _client.PostAsJsonAsync("/api/boards", req);
        var body = await upload.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var id = body!["id"].ToString();

        var response = await _client.PostAsync($"/api/boards/{id}/final?maxAttempts=50", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        result.Should().ContainKey("reason");
        result!["reason"].ToString().Should().Be("Stable");
    }

    public class BoardResponse
    {
        public Guid Id { get; set; }
        public int Rows { get; set; }
        public int Cols { get; set; }
        public bool[][] State { get; set; } = Array.Empty<bool[]>();
    }
}
