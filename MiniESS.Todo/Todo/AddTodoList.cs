using MediatR;
using MiniESS.Core.Commands;
using MiniESS.Todo.Todo.ReadModels;
using MiniESS.Todo.Todo.WriteModels;

namespace MiniESS.Todo.Todo;

public class AddTodoListInputModel : IRequest<AddTodoListResponseModel>
{
    public string Title { get; set; }
}

public class AddTodoListResponseModel
{
    public Guid CreatedTodoListId { get; set; }
}

public class AddTodoListHandler : IRequestHandler<AddTodoListInputModel, AddTodoListResponseModel>
{
    private readonly ReadonlyDbContext _readDb;
    private readonly CommandProcessor _commandProcessor;

    public AddTodoListHandler(ReadonlyDbContext readDb, CommandProcessor commandProcessor)
    {
        _readDb = readDb;
        _commandProcessor = commandProcessor;
    }
    
    public async Task<AddTodoListResponseModel> Handle(
        AddTodoListInputModel request, 
        CancellationToken cancellationToken)
    {
        var streamId = Guid.NewGuid();
        await _commandProcessor.ProcessAndCommit(
            new TodoListCommands.Create(streamId, request.Title), 
            cancellationToken);

        return new AddTodoListResponseModel { CreatedTodoListId = streamId };
    }
}
