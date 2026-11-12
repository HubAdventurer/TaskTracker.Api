using System.Net;
using RestSharp;
using Xunit;

namespace TaskTracker.Api.Tests;

public class TasksApiTests : IDisposable
{
    private const string TasksRoute = "api/tasks";
    private const string Title = "PATCH test task";
    private const string Description = "Keep this description";

    private readonly RestClient _client = new(
        Environment.GetEnvironmentVariable("TASKTRACKER_TEST_BASE_URL")
        ?? "http://localhost:5118");

    [Fact]
    public async Task Patch_CompletionOnly_SavesFalseAndPreservesOtherFields()
    {
        var id = await CreateCompletedTaskAsync();
        var request = new RestRequest($"{TasksRoute}/{id}", Method.Patch)
            .AddJsonBody(new { isCompleted = false });

        var response = await _client.ExecuteAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var saved = await GetTaskAsync(id);
        Assert.False(saved.IsCompleted);
        Assert.Equal(Title, saved.Title);
        Assert.Equal(Description, saved.Description);
    }

    [Fact]
    public async Task Patch_TitleOverLimit_RejectsAllChanges()
    {
        var id = await CreateCompletedTaskAsync();
        var request = new RestRequest($"{TasksRoute}/{id}", Method.Patch)
            .AddJsonBody(new { title = new string('A', 101), isCompleted = false });

        var response = await _client.ExecuteAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var saved = await GetTaskAsync(id);
        Assert.Equal(Title, saved.Title);
        Assert.Equal(Description, saved.Description);
        Assert.True(saved.IsCompleted);
    }

    [Fact]
    public async Task GetTasks_UnsupportedSortBy_ReturnsBadRequest()
    {
        var request = new RestRequest(TasksRoute, Method.Get)
            .AddQueryParameter("sortBy", "priority");

        var response = await _client.ExecuteAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Invalid sortBy value", response.Content ?? "");
    }

    private async Task<int> CreateCompletedTaskAsync()
    {
        var createRequest = new RestRequest(TasksRoute, Method.Post)
            .AddJsonBody(new { title = Title, description = Description });

        var created = await _client.ExecuteAsync<TaskResponse>(createRequest);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var task = Assert.IsType<TaskResponse>(created.Data);
        Assert.True(task.Id > 0);

        // POST creates incomplete tasks. PUT sets up the completed state.
        var completeRequest = new RestRequest($"{TasksRoute}/{task.Id}", Method.Put)
            .AddJsonBody(new { title = Title, description = Description, isCompleted = true });

        var completed = await _client.ExecuteAsync(completeRequest);
        Assert.Equal(HttpStatusCode.NoContent, completed.StatusCode);
        Assert.True((await GetTaskAsync(task.Id)).IsCompleted);
        return task.Id;
    }

    private async Task<TaskResponse> GetTaskAsync(int id)
    {
        var response = await _client.ExecuteAsync<TaskResponse>(
            new RestRequest($"{TasksRoute}/{id}", Method.Get));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var task = Assert.IsType<TaskResponse>(response.Data);
        Assert.Equal(id, task.Id);
        return task;
    }

    public void Dispose() => _client.Dispose();

    public sealed record TaskResponse(int Id, string Title, string? Description, bool IsCompleted);
}
