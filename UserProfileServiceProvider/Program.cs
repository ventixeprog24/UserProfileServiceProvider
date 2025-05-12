using Microsoft.EntityFrameworkCore;
using UserProfileService.Services;
using UserProfileServiceProvider.Data.Contexts;
using UserProfileServiceProvider.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>(x =>
    x.UseSqlServer(builder.Configuration.GetConnectionString("UserProfileDb")));
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();

builder.Services.AddGrpc();
builder.Services.AddMemoryCache();

var app = builder.Build();

app.MapGrpcService<ProfileService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();