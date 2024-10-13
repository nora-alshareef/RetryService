using System.Globalization;
using DelegateRetry.Services;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;



builder.Services.AddLogging();

// in order to use delegate retry service , load config and put this line and specify your db connection 
// Load configuration
var configuration = builder.Configuration;
builder.Services.AddRetryServices(builder.Configuration, connectionString => new SqlConnection(connectionString));

//*********************************************************
// Add Swagger Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

//*********************************************************
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
    
}

// Run the application
app.MapControllers();
app.Run();

