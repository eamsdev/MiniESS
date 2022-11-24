using MediatR;
using MiniESS.Core.Repository;
using MiniESS.Todo.Exceptions;
using MiniESS.Todo.Todo.WriteModels;

namespace MiniESS.Todo.Todo;

public class AddTodoItemInputModel : IRequest<AddTodoItemResponseModel>
{
    public Guid? TodoListId { get; set; }
    public string Description { get; set; }
}

public class AddTodoItemResponseModel
{
    public Guid TodoListId { get; set; }
    public int CreatedTodoItemId { get; set; }
}

public class AddTodoItemHandler : IRequestHandler<AddTodoItemInputModel, AddTodoItemResponseModel>
{
    private readonly IAggregateRepository<TodoListAggregateRoot> _repository;

    public AddTodoItemHandler(IAggregateRepository<TodoListAggregateRoot> repository)
    {
        _repository = repository;
    }

    public async Task<AddTodoItemResponseModel> Handle(AddTodoItemInputModel request, CancellationToken cancellationToken)
    {
        var todoList = await _repository.LoadAsync(request.TodoListId!.Value, cancellationToken);
        if (todoList is null)
            throw new NotFoundException($"TodoList with stream id {request.TodoListId!.Value} not found.");
        
        todoList.AddTodoItem(request.Description);
        await _repository.PersistAsyncAndAwaitProjection(todoList, cancellationToken);

        return new AddTodoItemResponseModel
        {
            TodoListId = todoList.StreamId,
            CreatedTodoItemId = todoList.TodoItems.Last().ItemNumber
        };
    }
}