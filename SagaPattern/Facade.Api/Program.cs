using Microsoft.EntityFrameworkCore;
using Saga.Infrastructure.Abstractions;
using Saga.Infrastructure.Messaging;
using Saga.Offer.DataAccess.Contextes;
using Saga.Offer.Interfaces;
using Saga.Offer.Repository;
using Saga.Offer.Workers.Commit;
using Saga.Offer.Workers.Rollback;
using Saga.Orchistrator;
using Saga.Orchistrator.DataAccess.Contexts;
using Saga.Orchistrator.Repo;
using Saga.Orchisttor.Repo;
using Saga.Order.DataAccess.Cpntexts;
using Saga.Order.Interfaces;
using Saga.Order.Repository;
using Saga.Order.Workers.Commit;
using Saga.Order.Workers.RollBack;
using Saga.Submission.DataAccess.Contextes;
using Saga.Submission.Interfaces;
using Saga.Submission.Repository;
using Saga.Submission.Workers.Commit;
using Saga.Submission.Workers.RollBack;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IMessageBrokerFactory, MessageBrokerFactory>();
            
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<ITrackSubmissionRepository, TrackSubmissionRepository>();
builder.Services.AddSingleton<IOfferRepository ,OfferRepository>();
builder.Services.AddSingleton<ISagaStateRepo, SagaStateRepo>();

builder.Services.AddSingleton<IOrchestrator, Orchestrator>();
builder.Services.AddSingleton<ICreateOrderCommitWorker, CreateOrderCommitWorker>();
builder.Services.AddSingleton<ICreateOrderRollbackWorker, CreateOrderRollbackWorker>();
builder.Services.AddSingleton<IUpdateTrackSubmissionStatusCommitWorker, UpdateSubmissionStatusCommitWorker>();
builder.Services.AddSingleton<IUpdateTrackSubmissionStatusRollbackWorker, UpdateTrackSubmissionStatusRollbackWorker>();
builder.Services.AddSingleton<IUpdateOfferStatusCommitWorker,UpdateOfferStatusCommitWorker>();
builder.Services.AddSingleton<IUpdateOfferStatusRollbackWorker, UpdateOfferStatusRollbackWorker>();
builder.Services.AddDbContext<OrderDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Order");
    options.UseNpgsql(connectionString);
}, ServiceLifetime.Singleton);
builder.Services.AddDbContext<OfferDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Offer");
    options.UseNpgsql(connectionString);
}, ServiceLifetime.Singleton);
builder.Services.AddDbContext<SubmissionDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Submission");
    options.UseNpgsql(connectionString);
}, ServiceLifetime.Singleton);
builder.Services.AddDbContext<SagaDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Saga");
    options.UseNpgsql(connectionString);
}, ServiceLifetime.Singleton);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Facade.Api v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();