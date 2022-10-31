using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MiniESS.Core;
using MiniESS.Projection;
using MiniESS.Todo.Configuration;
using MiniESS.Todo.Todo;
using MiniESS.Todo.Todo.ReadModels;
using MiniESS.Todo.Todo.WriteModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var eventStoreDbConnStr = builder.Configuration.GetConnectionString("EventStoreDb");
var eventStoreSerializationAssemblies = new List<Assembly> { typeof(TodoListAggregateRoot).Assembly };
builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerDocument();
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
builder.Services.AddDbContext<TodoDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MiniEssDb")));
builder.Services.AddProjector<TodoListAggregateRoot, TodoListProjector>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
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