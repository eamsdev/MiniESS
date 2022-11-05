using MediatR;
using MiniESS.Core.Repository;
using MiniESS.Todo.Todo.WriteModels;

namespace MiniESS.Todo.Todo;

public static class AddTodoList
{
    public class InputModel : IRequest<ResponseModel>
    {
        
    }

    public class ResponseModel
    {
        
    }
    
    public class Query : IRequestHandler<InputModel, ResponseModel>
    {
        private readonly IAggregateRepository<TodoListAggregateRoot> _repository;

        public Query(IAggregateRepository<TodoListAggregateRoot> repository)
        {
            _repository = repository;
        }
        
        public Task<ResponseModel> Handle(InputModel request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}