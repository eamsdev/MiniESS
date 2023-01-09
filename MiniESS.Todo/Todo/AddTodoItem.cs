using MediatR;
using Microsoft.EntityFrameworkCore;
using MiniESS.Core.Commands;
using MiniESS.Todo.Exceptions;
using MiniESS.Todo.Todo.ReadModels;
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
    private readonly ReadonlyDbContext _readDb;
    private readonly CommandProcessor _commandProcessor;

    public AddTodoItemHandler(ReadonlyDbContext readDb, CommandProcessor commandProcessor)
    {
        _readDb = readDb;
        _commandProcessor = commandProcessor;
    }

    public async Task<AddTodoItemResponseModel> Handle(AddTodoItemInputModel request, CancellationToken cancellationToken)
    {
        var todoList = await _readDb
            .Set<ReadModels.TodoList>()
            .SingleOrDefaultAsync(x => x.Id == request.TodoListId, cancellationToken: cancellationToken);
        
        if (todoList is null)
            throw new NotFoundException($"TodoList with stream id {request.TodoListId!.Value} not found.");

        var addTodoItemCommand = new TodoListCommands.AddTodoItem(todoList.Id, request.Description);
        await _commandProcessor.ProcessAndCommit(addTodoItemCommand, cancellationToken);
        
        todoList = await _readDb
            .Set<ReadModels.TodoList>()
            .Include(x => x.TodoItems)
            .SingleAsync(x => x.Id == request.TodoListId, cancellationToken: cancellationToken);
        
        return new AddTodoItemResponseModel
        {
            TodoListId = todoList.Id,
            CreatedTodoItemId = todoList.TodoItems.Last().ItemNumber, 
        };
    }
}