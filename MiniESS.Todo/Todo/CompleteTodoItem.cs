using MediatR;
using Microsoft.EntityFrameworkCore;
using MiniESS.Core.Aggregate;
using MiniESS.Core.Commands;
using MiniESS.Infrastructure.Repository;
using MiniESS.Todo.Exceptions;
using MiniESS.Todo.Todo.ReadModels;
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
    private readonly ReadonlyDbContext _readDb;
    private readonly CommandProcessor _commandProcessor;

    public CompleteTodoItemHandler(ReadonlyDbContext readDb, CommandProcessor commandProcessor)
    {
        _readDb = readDb;
        _commandProcessor = commandProcessor;
    }

    public async Task<CompleteTodoItemResponseModel> Handle(CompleteTodoItemInputModel request, CancellationToken cancellationToken)
    {
        var todoList = await _readDb
            .Set<ReadModels.TodoList>()
            .Include(x => x.TodoItems)
            .SingleOrDefaultAsync(x => x.Id == request.TodoListId, cancellationToken: cancellationToken);
        
        if (todoList is null)
            throw new NotFoundException($"TodoList with stream id {request.TodoListId!.Value} not found.");

        await _commandProcessor.ProcessAndCommit(new TodoListCommands.CompleteTodoItem(todoList.Id, request.TodoItemId.Value), cancellationToken);

        return new CompleteTodoItemResponseModel
        {
            TodoListId = request.TodoListId.Value,
            TodoItemId = request.TodoItemId.Value
        };
    }
}