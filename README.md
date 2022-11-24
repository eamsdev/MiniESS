# MiniESS

MiniESS is an Event Sourcing Micro-framework that utilises EventStoreDB as the event store and uses Entity Framework Core to manage the read database.

## Motivations

MiniESS was created as a learning exercise to understand the internal workings of the event sourcing architecture, as such, its aim was to complete this goal with the smallest amount of code as it is practical. This means using off the shelve products such as EventStoreDB to manage events and events subscriptions.

![image architecture](./architecture.svg)

## Running the Todo list sample application

Run `docker-compose up`, navigate to `localhost:8090`

Or if you want to run the application from the IDE or from host machine

Run `docker-compose -f docker-compose.db-only.yml up`, navigate to `localhost:8090`

![image demo](./example.png)

You can navigate to `localhost:2113` to interact with EventStoreDB Ui

## Getting Started

The (micro-)framework is still under development and is still in its early stage, thus the packages have not been published for public use. However, the source code is available via this repository. Both MiniESS.Projection and MiniESS.Core are required for the full event sourcing functionality.

For a full working example with React frontend, see MiniESS.Todo project.

To use MiniESS, you will need the register the following services.

```cs
builder.Services.AddEventSourcing(option =>
{
    option.ConnectionString = eventStoreDbConnStr; 
    option.SerializableAssemblies = eventStoreSerializationAssemblies;
});

builder.Services.AddProjectionService(option =>
{
    option.ConnectionString = eventStoreDbConnStr;
    option.SerializableAssemblies = eventStoreSerializationAssemblies;
});
```

`SerializableAssemblies` are the assemblies of the domain events that will need to be serialized.
`AddProjectionService` will enable the background service that manages projections from new events.

Note: you will need to configure EF Core dbcontext yourself, like the following

```cs
builder.Services.AddDbContext<TodoDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MiniEssDb")));
```

### Write Models

With MiniESS, write models inherits from the `BaseAggregateRoot<T>` class.

To begin, register your write models with the service collections, this will register the corresponding Aggregate Repository used for persisting write-models.

```cs
builder.Services.AddEventSourcingRepository<TodoListAggregateRoot>();
```

Note: A private constructor is required for the framework to be able to hydrate the write-models.

```cs
public class TodoListAggregateRoot : BaseAggregateRoot<TodoListAggregateRoot>
{
    private TodoListAggregateRoot(Guid streamId) : base(streamId)
    {
    }

    private TodoListAggregateRoot(Guid streamId, string title) : base(streamId)
    {
        if (title.Length == 0)
            throw new DomainException("Title cannot be null or empty for a Todo List");
        
        AddEvent(new TodoListEvents.TodoListCreated(this, title));
    }

    ...
```

When manipulating events, you are required to call the AddEvent method.

```cs
public void AddTodoItem(string description)
{
    var nextItemNumber = TodoItems.Count;
    AddEvent(new TodoListEvents.TodoItemAdded(this, nextItemNumber, description));
}
```

The added/published events are applied the write model via the `Apply(IDomainEvent @event)` method.

```cs
protected override void Apply(IDomainEvent @event)
{ 
    switch (@event)
    {
        case TodoListEvents.TodoItemAdded todoItemAdded:
            TodoItems.Add(TodoItemAggregate.Create(todoItemAdded.ItemNumber, todoItemAdded.Description));
            break;
        case TodoListEvents.TodoListCreated todoItemCreated:
            Title = todoItemCreated.Title;
            TodoItems = new List<TodoItemAggregate>();
            break;
        case TodoListEvents.TodoItemCompleted todoItemCompleted:
            TodoItems.Single(x => x.ItemNumber == todoItemCompleted.ItemNumber).Complete();
            break;
        default:
            throw new ArgumentOutOfRangeException(nameof(@event));
    }
}
```

Events correlated to the changes to the write models are not published to the EventStoreDb until the write model is persisted via the AggregateRepository.

```cs
await _repository.PersistAsyncAndAwaitProjection(
            TodoListAggregateRoot.Create(streamId, request.Title), 
            cancellationToken);
```

or

```cs
await _repository.PersistAsync(
            TodoListAggregateRoot.Create(streamId, request.Title), 
            cancellationToken);
```

See section *"Eventual vs Strong Consistency"* for explanations for the bahaviours of each persisting methods.

## Read Models

With MiniESS, read model projectors inherits from the `ProjectorBase<T>` class.

To begin, register your write models with the service collections, this will register the corresponding projectors used by the framwork.

```cs
builder.Services.AddProjector<TodoListAggregateRoot, TodoListProjector>();
```

Next, implement your projector, and the correspoding interfaces for each events your projector needs to handle.

```cs
public class TodoListProjector :
    ProjectorBase<TodoListAggregateRoot>,
    IProject<TodoListEvents.TodoListCreated>,
    IProject<TodoListEvents.TodoItemAdded>,
    IProject<TodoListEvents.TodoItemCompleted>
{
    public TodoListProjector(
        TodoDbContext context, 
        IServiceProvider serviceProvider, 
        ILogger<TodoListProjector> logger) : base(context, serviceProvider, logger)
    {
    }

    ...
```

Projection is simple, implement the interface. Call the `SaveChangeAsync()` method when done with projecting.

```cs
public async Task ProjectEvent(TodoListEvents.TodoListCreated domainEvent, CancellationToken token)
{
    var todoList = new TodoList
    {
        Id = domainEvent.AggregateId,
        Title = domainEvent.Title,
        TodoItems = new List<TodoItem>()
    };

    await Repository<TodoList>().AddAsync(todoList, token);
    await SaveChangesAsync();
}
```

## Considerations

### Eventual vs Strong Consistency

*Work in progress*

### Catch up projections

*Work in progress*

### Snapshots

*Work in progress*

## Acknowledgements

MiniESS takes inspiration from the following open-source projects:

1. [David Guida - Event Sourcing in .NET Core Series](https://www.davidguida.net/event-sourcing-in-net-core-part-1-a-gentle-introduction/)
2. [Revo Framework](https://docs.revoframework.net)
3. [oskardudycz - EventSourcing.NetCore tutorials](https://github.com/oskardudycz/EventSourcing.NetCore)

## License

> MIT License
>
> Copyright (c) 2022 Eams.dev
>
> Permission is hereby granted, free of charge, to any person obtaining a copy
> of this software and associated documentation files (the "Software"), to deal
> in the Software without restriction, including without limitation the rights
> to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
> copies of the Software, and to permit persons to whom the Software is
> furnished to do so, subject to the following conditions:
>
> The above copyright notice and this permission notice shall be included in all
> copies or substantial portions of the Software.
>
> THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
> IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
> FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
> AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
> LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
> OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
> SOFTWARE.
