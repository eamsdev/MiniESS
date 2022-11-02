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
    public async Task<GetTodo.ViewModel> Get(GetTodo.QueryModel queryModel)
    {
        return await _mediator.Send(queryModel);
    }
}