using Microsoft.AspNetCore.Mvc;

namespace MiniESS.Todo.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoController : ControllerBase
{
    private readonly ILogger<TodoController> _logger;

    public TodoController(ILogger<TodoController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IEnumerable<int>> Get()
    {
        throw new NotImplementedException();
    }
}