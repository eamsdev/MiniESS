using System.Reflection;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MiniESS.Infrastructure;
using MiniESS.Todo.Configuration;
using MiniESS.Todo.Exceptions;
using MiniESS.Todo.Todo.ReadModels;
using MiniESS.Todo.Todo.WriteModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var eventStoreDbConnStr = builder.Configuration.GetConnectionString("EventStoreDb");
var eventStoreSerializationAssemblies = new List<Assembly> { typeof(TodoListAggregateRoot).Assembly };
builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerDocument();
builder.Services.AddTransient(sp => new ReadonlyDbContext(sp.GetRequiredService<TodoDbContext>()));
builder.Services.AddEventSourcingRepository<TodoListAggregateRoot>();
builder.Services.AddProblemDetails(opt =>
    {
        opt.MapToStatusCode<DomainException>(StatusCodes.Status400BadRequest);
        opt.MapToStatusCode<NotFoundException>(StatusCodes.Status404NotFound);
    })
    .AddControllers()
    .AddProblemDetailsConventions()
    .AddJsonOptions(x => x.JsonSerializerOptions.IgnoreNullValues = true);
builder.Services.AddMediatR(typeof(TodoItemAggregate).GetTypeInfo().Assembly);
builder.Services.AddEventSourcing(option =>
{
    option.ConnectionString = eventStoreDbConnStr;
    option.SerializableAssemblies = eventStoreSerializationAssemblies;
}).AddProjectionService();
builder.Services.AddDbContext<TodoDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MiniEssDb")));
builder.Services.AddProjector<TodoListAggregateRoot, TodoListProjector>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseHttpLogging();
}

app.UseStaticFiles();
app.UseRouting();
app.UseOpenApi();
app.UseSwaggerUi3();
await app.BootstrapDbContext();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();

namespace MiniESS.Todo
{
    public partial class Program
    {
    }
}