using CongEspVilaGuilhermeApi;
using CongEspVilaGuilhermeApi.AppCore.Repositories;
using CongEspVilaGuilhermeApi.AppCore.Services;
using CongEspVilaGuilhermeApi.Domain.Repositories;
using CongEspVilaGuilhermeApi.Domain.Services;
using CongEspVilaGuilhermeApi.Domain.UseCases;
using Microsoft.AspNetCore.Cors;

var builder = WebApplication.CreateBuilder(args);

Settings.LoadFromConfigFiles(builder.Configuration);

var originAllowed = new string[]{
    "localhost", "127.0.0.1"
};

builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(
            policy =>
            {
                policy.SetIsOriginAllowed(origin => {
                    var host = new Uri(origin).Host;
                    Console.WriteLine($"[SetIsOriginAllowed] host: {host}");
                    return originAllowed.Contains(host);
                })
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
    });

// Add services to the container.
builder.Services.AddScoped<IEmailService, GmailService>();
builder.Services.AddScoped<IUserRepository, DynamoDbUserRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<TerritoryJsonRepository>();
builder.Services.AddScoped<DynamoDbTerritoryRepository>();
builder.Services.AddScoped<ITerritoryRepository, DynamoDbTerritoryRepository>();
builder.Services.AddScoped<TerritoryRepositoryValidationService>();
builder.Services.AddScoped<UserUseCases>();
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
    await userUseCases.InitializeAdminUserAsync();

    var repositoryValidator = scope.ServiceProvider.GetService<TerritoryRepositoryValidationService>();
    await repositoryValidator.ValidateDataOnDynamoDb();
}


app.UseCors();
app.UseHttpsRedirection();
app.UseCongVilaguilhermeAuthentication();
app.UseExceptionHandler();
app.MapControllers();
app.Run();
