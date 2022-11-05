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
        var responseContent = await response.DeserializeContentAsync<GetTodoListsViewModel>();

        // Assert
        response.EnsureSuccessStatusCode();
        responseContent.Should().NotBeNull();
        responseContent.TodoLists.Should().BeEmpty();
    }

    [Fact]
    public async Task AddTodoAndGetTodosReturns200OkAndExpectedContent()
    {
        // Arrange
        var todoListName = Guid.NewGuid().ToString();
        var inputModel = new AddTodoListInputModel { Title = todoListName };

        // Act
        (await _client.PostRouteAsJsonAsync("Todo", inputModel)).EnsureSuccessStatusCode();
        var getResponse = await _client.GetRouteAsync("Todo");
        var getResponseContent = await getResponse.DeserializeContentAsync<GetTodoListsViewModel>();

        // Assert
        getResponse.EnsureSuccessStatusCode();
        getResponseContent.Should().NotBeNull();
        getResponseContent.TodoLists.Should().ContainSingle(x => x.Title == todoListName);
        getResponseContent.TodoLists.Single(x => x.Title == todoListName).TodoItems.Should().BeEmpty();
    }
    
    [Fact]
    public async Task AddTodoAndGetSpecificTodoReturns200OkAndExpectedContent()
    {
        // Arrange
        var todoListName = Guid.NewGuid().ToString();
        var inputModel = new AddTodoListInputModel { Title = todoListName };

        // Act
        var addResponse = await _client.PostRouteAsJsonAsync("Todo", inputModel);
        var createdItemId = (await addResponse.DeserializeContentAsync<AddTodoListResponseModel>()).CreatedTodoListId;
        var getResponse = await _client.GetRouteAsync($"Todo/{createdItemId}");
        var getResponseContent = await getResponse.DeserializeContentAsync<GetTodoListViewModel>();

        // Assert
        getResponse.EnsureSuccessStatusCode();
        getResponseContent.Should().NotBeNull();
        getResponseContent.TodoList.Should().NotBeNull();
        getResponseContent.TodoList.Title.Should().Be(todoListName);
        getResponseContent.TodoList.TodoItems.Should().BeEmpty();
    }
    
    [Fact]
    public async Task CanAddTodoItem()
    {
        // Arrange
        var todoListName = Guid.NewGuid().ToString();
        var addTodoListInputModel = new AddTodoListInputModel { Title = todoListName };
        var addTodoItemInputModel = new AddTodoItemInputModel { Description = "foobar" };

        // Act
        var addResponse = await _client.PostRouteAsJsonAsync("Todo", addTodoListInputModel);
        var createdTodoListId = (await addResponse.DeserializeContentAsync<AddTodoListResponseModel>()).CreatedTodoListId;
        var addItemResponse = await _client.PostRouteAsJsonAsync($"Todo/{createdTodoListId}/items", addTodoItemInputModel);
        var getResponse = await _client.GetRouteAsync($"Todo/{createdTodoListId}");
        var getResponseContent = await getResponse.DeserializeContentAsync<GetTodoListViewModel>();

        // Assert
        addItemResponse.EnsureSuccessStatusCode();
        getResponseContent.TodoList.TodoItems.Should().ContainSingle();
        getResponseContent.TodoList.TodoItems.Single().Id.Should().Be(1);
        getResponseContent.TodoList.TodoItems.Single().Description.Should().Be("foobar");
        getResponseContent.TodoList.TodoItems.Single().IsCompleted.Should().BeFalse();
    }
}