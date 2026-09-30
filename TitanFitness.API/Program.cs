using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TitanFitness.Application.Common;
using TitanFitness.Application.Dashboard.Queries;
using TitanFitness.Domain.Common;
using TitanFitness.Infrastructure.Data;
using TitanFitness.Infrastructure.Repositories;
using TitanFitness.WebAPI.Contracts.Plans;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(GetTodayCheckInsQuery).Assembly));



builder.Services.AddValidatorsFromAssembly(
    typeof(CreatePlanRequest).Assembly);


builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>));


builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();



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
