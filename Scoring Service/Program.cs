using Microsoft.EntityFrameworkCore;
using Scoring_Service.Configurations.Conditions;
using Scoring_Service.Data;
using Scoring_Service.Services;
using Scoring_Service.Services.Conditions;
using Scoring_Service.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Db Context
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurations
builder.Services.Configure<AgeConditionConfiguration>(
    builder.Configuration.GetSection("Application:Conditions:AgeCondition"));
builder.Services.Configure<CitizenshipConditionConfiguration>(
    builder.Configuration.GetSection("Application:Conditions:CitizenshipCondition"));
builder.Services.Configure<SalaryConditionConfiguration>(
    builder.Configuration.GetSection("Application:Conditions:SalaryCondition"));

// Mappers 
builder.Services.AddAutoMapper(typeof(Program));

// Services 
builder.Services.AddScoped<ICondition, AgeCondition>();
builder.Services.AddScoped<ICondition, CitizenshipCondition>();
builder.Services.AddScoped<ICondition, SalaryCondition>();
builder.Services.AddScoped<ScoringService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) 
{ 
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
