using MediatR;
using Microsoft.EntityFrameworkCore;
using MiniESS.Todo.Exceptions;
using MiniESS.Todo.Todo.ReadModels;

namespace MiniESS.Todo.Todo;

public class GetTodoListQueryModel : IRequest<GetTodoListViewModel>
{
    public Guid Id { get; private init; }

    public static GetTodoListQueryModel FromId(Guid id) => new GetTodoListQueryModel { Id = id };
}

public class GetTodoListViewModel
{
    public TodoListViewModel TodoList { get; init; }
}

public class GetTodoListHandler : IRequestHandler<GetTodoListQueryModel, GetTodoListViewModel>
{
    private readonly ReadonlyDbContext _readDb;

    public GetTodoListHandler(ReadonlyDbContext readDb)
    {
        _readDb = readDb;
    }
    
    public async Task<GetTodoListViewModel> Handle(GetTodoListQueryModel request, CancellationToken cancellationToken)
    {
        var todoList = await _readDb
            .Set<TodoList>()
            .Include(x => x.TodoItems)
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        
        if (todoList is null)
            throw new NotFoundException($"TodoList with id {request.Id} does not exist");
        
        return new GetTodoListViewModel
        {
            TodoList = new TodoListViewModel
            {
                StreamId = todoList.Id,
                Title = todoList.Title,
                TodoItems = todoList.TodoItems.Select(y => new TodoItemViewModel
                {
                    Id = y.Id,
                    Description = y.Description,
                    IsCompleted = y.IsComplete,
                    Order = y.ItemNumber
                }).ToList()
            }
        };
    }
}
