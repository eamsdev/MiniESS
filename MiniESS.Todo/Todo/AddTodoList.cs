using MediatR;
using MiniESS.Core.Aggregate;
using MiniESS.Infrastructure.Repository;
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
    private readonly IAggregateRepository<TodoListAggregateRoot> _repository;

    public AddTodoListHandler(IAggregateRepository<TodoListAggregateRoot> repository)
    {
        _repository = repository;
    }
    
    public async Task<AddTodoListResponseModel> Handle(
        AddTodoListInputModel request, 
        CancellationToken cancellationToken)
    {
        var streamId = Guid.NewGuid();
        await _repository.PersistAsyncAndAwaitProjection(
            TodoListAggregateRoot.Create(streamId, request.Title), 
            cancellationToken);

        return new AddTodoListResponseModel { CreatedTodoListId = streamId };
    }
}
