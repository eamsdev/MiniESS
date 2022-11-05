using AutoFixture;
using FluentAssertions;
using MiniESS.Todo.Tests.Extensions;
using MiniESS.Todo.Tests.Utils;
using MiniESS.Todo.Todo;

namespace MiniESS.Todo.Tests;

public class QueryTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    public QueryTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTodosReturns200OkAndExpectedContent()
    {
        // Arrange
        
        // Act
        var response = await _client.GetRouteAsync("Todo");
        var responseContent = await response.DeserializeContentAsync<GetTodoLists.ViewModel>();

        // Assert
        response.EnsureSuccessStatusCode();
        responseContent.Should().NotBeNull();
        responseContent.TodoLists.Should().BeEmpty();
    }
}