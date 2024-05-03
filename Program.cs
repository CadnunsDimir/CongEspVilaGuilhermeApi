using CongEspVilaGuilhermeApi;
using CongEspVilaGuilhermeApi.AppCore.Repositories;
using CongEspVilaGuilhermeApi.AppCore.Services;
using CongEspVilaGuilhermeApi.Domain.Repositories;
using CongEspVilaGuilhermeApi.Domain.Services;
using CongEspVilaGuilhermeApi.Domain.UseCases;

var builder = WebApplication.CreateBuilder(args);

Settings.LoadFromConfigFiles(builder.Configuration);

var originAllowed = new string[]{
    "localhost", "127.0.0.1", Settings.FrontAppHost
};

builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(
            policy =>
            {
                policy.SetIsOriginAllowed(origin => {
                    var host = new Uri(origin).Host;
                    var allowedHost = originAllowed.Contains(host);
                    Console.WriteLine($"[SetIsOriginAllowed] host: {host}, allowed: {allowedHost}");
                    return allowedHost;
                })
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
    });

builder.Services.AddMemoryCache();

// Add services to the container.
builder.Services.AddScoped<IEmailService, GmailService>();
builder.Services.AddScoped<IUserRepository, DynamoDbUserRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<TerritoryJsonRepository>();
builder.Services.AddScoped<DynamoDbTerritoryRepository>();
builder.Services.AddScoped<ITerritoryRepository, DynamoDbTerritoryRepository>();
builder.Services.AddScoped<TerritoryRepositoryValidationService>();
builder.Services.AddScoped<ICacheService, MemoryCacheService>();
builder.Services.AddScoped<TerritoryUseCases>();
builder.Services.AddScoped<UserUseCases>();
builder.Services.AddScoped<OnlineTsvSyncService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCongVilaguilhermeAuthentication();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var userUseCases = scope.ServiceProvider.GetService<UserUseCases>();
    var repositoryValidator = scope.ServiceProvider.GetService<TerritoryRepositoryValidationService>();
    await userUseCases!.InitializeAdminUserAsync();    
    await repositoryValidator!.ValidateDataOnDynamoDb();
}


app.UseCors();
app.UseCongVilaguilhermeAuthentication();
app.UseExceptionHandler();
app.MapControllers();
app.Run();
