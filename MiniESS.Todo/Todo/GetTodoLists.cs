﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using MiniESS.Core.Repository;
using MiniESS.Todo.Todo.ReadModels;
using MiniESS.Todo.Todo.WriteModels;

namespace MiniESS.Todo.Todo;

public class GetTodoListsQueryModel : IRequest<GetTodoListsViewModel>
{ }

public class GetTodoListsViewModel
{
    public List<TodoListViewModel> TodoLists { get; init; }
}

public class GetTodoListsHandler : IRequestHandler<GetTodoListsQueryModel, GetTodoListsViewModel>
{
    private readonly IAggregateRepository<TodoListAggregateRoot> _repository;
    private readonly ReadonlyDbContext _readDb;

    public GetTodoListsHandler(IAggregateRepository<TodoListAggregateRoot> repository, ReadonlyDbContext readDb)
    {
        _repository = repository;
        _readDb = readDb;
    }
    
    public async Task<GetTodoListsViewModel> Handle(
        GetTodoListsQueryModel request, 
        CancellationToken cancellationToken)
    {
        var todoLists = await _readDb
            .Set<TodoList>()
            .Include(x => x.TodoItems)
            .ToListAsync(cancellationToken: cancellationToken);
        
        return new GetTodoListsViewModel
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