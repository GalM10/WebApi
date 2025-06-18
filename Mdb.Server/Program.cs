using Mdb.Core;
using Mdb.DAL;
using Mdb.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var connectionString = configuration["Database:ConnectionString"];
var databaseName = configuration["Database:DatabaseName"];

builder.Services.AddSingleton<IDbFactory>(new DbFactory(connectionString, databaseName));
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

const string baseUrl = "/api";

var api = app.MapGroup($"{baseUrl}/users")
    .WithOpenApi();

api.MapGet("/", (IUserRepository userRepository) =>
{
    var res = userRepository.GetAllAsync();
    return res is null
        ? Results.NotFound()
        : Results.Ok(res);

});

api.MapPost("/", async (IUserRepository userRepository, User user) =>
{
    await userRepository.CreateAsync(user);
    return Results.Created($"{baseUrl}/users/{user.Id}", user);
});

api.MapPut("/", async (IUserRepository userRepository, User user) =>
{
    await userRepository.UpdateAsync(user);
    return Results.Ok(user);
});

api.MapDelete("/{userId:guid}", async (IUserRepository userRepository, Guid userId) =>
{
    await userRepository.DeleteAsync(userId);
    return Results.Ok();
});

api.MapGet("/{userId:guid}", async (IUserRepository userRepository, Guid userId) =>
{
    var result = await userRepository.GetByAsync(userId);
    return result is null
        ? Results.NotFound()
        : Results.Ok(result);
});

api.MapGet("/search/{searchTerm}", async (IUserRepository userRepository, string searchTerm) =>
{
    var result = await userRepository.SearchAsync(searchTerm);
    return result is null || result.Count == 0
        ? Results.NotFound()
        : Results.Ok(result);
});


await app.RunAsync();

