var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

string[] summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/summaries/{maximum}", (int maximum) =>
{
    return summaries.Take(maximum);
}).AddEndpointFilter(async (context, next) =>
{
    int maximum = context.GetArgument<int>(0);
    if(maximum < 1) return Results.Problem("Hey!!! don't cheat me, value mmust be more than 0!");
    return await next(context);
}).AddEndpointFilter<CheckMaximum>();

app.UseHttpsRedirection();


app.Run();

internal class CheckMaximum : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        int maximum = context.GetArgument<int>(0);
        int total = 50;
        if(maximum > total) return Results.NotFound($"OH!!! Sorry we don't have {maximum} elements");
        return await next(context);
    }
}

