 using FluentAssertions;
 using MiniESS.Todo.Exceptions;
 using MiniESS.Todo.Todo;

 namespace MiniESS.Todo.Tests;

public class TodoListAggregateTests
{
    [Fact]
    public void CanCreateAggregateTest()
    {
        // Arrange
        // Act
        var todoList = TodoListAggregateRoot.Create(Guid.NewGuid());

        // Assert
        todoList.TodoItems.Should().BeEmpty();

    }
    
    [Fact]
    public void CanAddTodoItemTest()
    {
        // Arrange
        const string todoItemDescription = "foobar";
        var todoList = TodoListAggregateRoot.Create(Guid.NewGuid());
        
        // Act
        todoList.AddTodoItem(todoItemDescription);

        // Assert
        todoList.TodoItems.Should().Contain(x => x.Description == todoItemDescription);
        todoList.TodoItems.Count.Should().Be(1);
    }
    
    [Fact]
    public void CanAddMultipleTodoItemsTest()
    {
        // Arrange
        const string todoItemDescription1 = "foobar1";
        const string todoItemDescription2 = "foobar2";
        var todoList = TodoListAggregateRoot.Create(Guid.NewGuid());
        
        // Act
        todoList.AddTodoItem(todoItemDescription1);
        todoList.AddTodoItem(todoItemDescription2);

        // Assert
        todoList.TodoItems.Should().Contain(x => x.Description == todoItemDescription1);
        todoList.TodoItems.Should().Contain(x => x.Description == todoItemDescription2);
        todoList.TodoItems.Count.Should().Be(2);
    }
    
    [Fact]
    public void CanCompleteTodoItemsTests()
    {
        // Arrange
        const string todoItemDescription1 = "foobar1";
        const string todoItemDescription2 = "foobar2";
        var todoList = TodoListAggregateRoot.Create(Guid.NewGuid());
        
        // Act
        todoList.AddTodoItem(todoItemDescription1);
        todoList.AddTodoItem(todoItemDescription2);
        todoList.CompleteTodoItem(todoList.TodoItems.First().Id);

        // Assert
        todoList.TodoItems.Should().ContainSingle(x => x.IsCompleted);
        todoList.TodoItems
            .SingleOrDefault(x => x.IsCompleted && x.Id == todoList.TodoItems.First().Id)
            .Should()
            .NotBeNull();
        todoList.TodoItems.Count.Should().Be(2);
    }
    
    [Fact]
    public async void CompleteNonExistingTodoItemThrowsDomainException()
    {
        // Arrange
        const string todoItemDescription = "foobar";
        var todoList = TodoListAggregateRoot.Create(Guid.NewGuid());
        todoList.AddTodoItem(todoItemDescription);
        
        // Act
        // Assert
        await Assert.ThrowsAsync<DomainException>(async () =>
        {
            todoList.CompleteTodoItem(99);
            await Task.CompletedTask;
        });
    }
    
    [Fact]
    public async void CompleteAlreadyCompletedTodoItemThrowsDomainException()
    {
        // Arrange
        const string todoItemDescription = "foobar";
        var todoList = TodoListAggregateRoot.Create(Guid.NewGuid());
        todoList.AddTodoItem(todoItemDescription);
        
        // Act
        // Assert
        await Assert.ThrowsAsync<DomainException>(async () =>
        {
            var itemId = todoList.TodoItems.First().Id;
            todoList.CompleteTodoItem(itemId);
            todoList.CompleteTodoItem(itemId);
            await Task.CompletedTask;
        });
    }
}                                                                      