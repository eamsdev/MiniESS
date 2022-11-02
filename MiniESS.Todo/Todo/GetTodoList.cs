using MediatR;
using Microsoft.EntityFrameworkCore;
using MiniESS.Todo.Exceptions;
using MiniESS.Todo.Todo.ReadModels;

namespace MiniESS.Todo.Todo;

public static class GetTodoList
{
    public class QueryModel : IRequest<ViewModel>
    {
        public Guid Id { get; private init; }

        public static QueryModel FromId(Guid id) => new QueryModel { Id = id };
    }

    public class ViewModel
    {
        public TodoListViewModel TodoList { get; init; }
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
            var todoList = await _readDb.Set<TodoList>().SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
            if (todoList is null)
                throw new NotFoundException($"TodoList with id {request.Id} does not exist");
            
            return new ViewModel
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
}