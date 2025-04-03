using Mediator.Lib.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCustomMediator();

var app = builder.Build();
app.UseAuthorization();
app.MapControllers();
app.Run();
