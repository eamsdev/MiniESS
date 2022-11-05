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

    public TodoController(ILogger<TodoController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<GetTodoLists.ViewModel>> GetTodoLists()
    {
        return await _mediator.Send(new GetTodoLists.QueryModel());
    }
    
    [HttpPost]
    public async Task<ActionResult<GetTodoLists.ViewModel>> AddTodoList()
    {
        return await _mediator.Send(new GetTodoLists.QueryModel());
    }
    
    [HttpGet]
    [Route("{todoId:guid}")]
    public async Task<ActionResult<GetTodoList.ViewModel>> GetTodoList(Guid todoId)
    {
        return await _mediator.Send(Todo.GetTodoList.QueryModel.FromId(todoId));
    }
}