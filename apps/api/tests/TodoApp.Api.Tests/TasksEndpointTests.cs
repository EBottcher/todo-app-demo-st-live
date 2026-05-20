using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using TodoApp.Application.Dtos;
using Xunit;

namespace TodoApp.Api.Tests;

public class TasksEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public TasksEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_returns_ok()
    {
        var res = await _client.GetAsync("/api/health");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task Create_and_list_task_round_trip()
    {
        var create = await _client.PostAsJsonAsync("/api/tasks",
            new CreateTaskDto("Integration test", "from xunit"), JsonOpts);
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var dto = await create.Content.ReadFromJsonAsync<TaskDto>(JsonOpts);
        Assert.NotNull(dto);

        var list = await _client.GetFromJsonAsync<List<TaskDto>>("/api/tasks", JsonOpts);
        Assert.NotNull(list);
        Assert.Contains(list!, t => t.Id == dto!.Id);
    }
}
