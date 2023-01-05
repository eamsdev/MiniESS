using MediatR;
using MiniESS.Core.Aggregate;
using MiniESS.Infrastructure.Repository;
using MiniESS.Todo.Exceptions;
using MiniESS.Todo.Todo.WriteModels;

namespace MiniESS.Todo.Todo;

public class CompleteTodoItemInputModel : IRequest<CompleteTodoItemResponseModel>
{
    public Guid? TodoListId { get; set; }
    public int? TodoItemId { get; set; }
}

public class CompleteTodoItemResponseModel
{
    public Guid TodoListId { get; set; }
    public int TodoItemId { get; set; }
}

public class CompleteTodoItemHandler : IRequestHandler<CompleteTodoItemInputModel, CompleteTodoItemResponseModel>
{
    private readonly IAggregateRepository<TodoListAggregateRoot> _repository;

    public CompleteTodoItemHandler(IAggregateRepository<TodoListAggregateRoot> repository)
    {
        _repository = repository;
    }

    public async Task<CompleteTodoItemResponseModel> Handle(CompleteTodoItemInputModel request, CancellationToken cancellationToken)
    {
        var todoList = await _repository.LoadAsync(request.TodoListId!.Value, cancellationToken);
        if (todoList is null)
            throw new NotFoundException($"TodoList with stream id {request.TodoListId!.Value} not found.");
        
        todoList.CompleteTodoItem(request.TodoItemId!.Value);
        await _repository.PersistAsyncAndAwaitProjection(todoList, cancellationToken);

        return new CompleteTodoItemResponseModel
        {
            TodoListId = request.TodoListId.Value,
            TodoItemId = request.TodoItemId.Value
        };
    }
}