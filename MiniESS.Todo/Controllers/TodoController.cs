using MediatR;
using Microsoft.AspNetCore.Mvc;
using MiniESS.Todo.Todo;

namespace MiniESS.Todo.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TodoController> _logger;

    public TodoController(
        ILogger<TodoController> logger, 
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<GetTodoListsViewModel>> GetTodoLists()
    {
        return await _mediator.Send(new GetTodoListsQueryModel());
    }
    
    [HttpPost]
    public async Task<ActionResult<AddTodoListResponseModel>> AddTodoList(
        [FromBody] AddTodoListInputModel inputModel)
    {
        return await _mediator.Send(inputModel);
    }
    
    [HttpPost]
    [Route("{todoId:guid}/items")]
    public async Task<ActionResult<AddTodoItemResponseModel>> AddTodoItem(
        Guid todoId, 
        [FromBody] AddTodoItemInputModel inputModel)
    {
        return await _mediator.Send(new AddTodoItemInputModel
        {
            TodoListId = todoId, Description = inputModel.Description
        });
    }
    
    [HttpGet]
    [Route("{todoId:guid}")]
    public async Task<ActionResult<GetTodoListViewModel>> GetTodoList(Guid todoId)
    {
        return await _mediator.Send(GetTodoListQueryModel.FromId(todoId));
    }
}