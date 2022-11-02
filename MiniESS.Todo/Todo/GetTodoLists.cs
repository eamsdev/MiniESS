using MediatR;
using Microsoft.EntityFrameworkCore;
using MiniESS.Todo.Todo.ReadModels;

namespace MiniESS.Todo.Todo;

public static class GetTodoLists
{
    public class QueryModel : IRequest<ViewModel>
    { }

    public class ViewModel
    {
        public List<TodoListViewModel> TodoLists { get; init; }
    }

    public class Handler : IRequestHandler<QueryModel, ViewModel>
    {
        private readonly ReadonlyDbContext _readDb;

        public Handler(ReadonlyDbContext readDb)
        {
            _readDb = readDb;
        }
        
        public async Task<ViewModel> Handle(QueryModel request, CancellationToken cancellationToken)
        {
            var todoLists = await _readDb.Set<TodoList>().ToListAsync(cancellationToken: cancellationToken);
            return new ViewModel
            {
                TodoLists = todoLists.Select(x => new TodoListViewModel
                {
                    StreamId = x.Id,
                    Title = x.Title,
                    TodoItems = x.TodoItems.Select(y => new TodoItemViewModel
                    {
                        Id = y.Id,
                        Description = y.Description,
                        IsCompleted = y.IsComplete,
                        Order = y.ItemNumber
                    }).ToList()
                }).ToList()
            };
        }
    }
}