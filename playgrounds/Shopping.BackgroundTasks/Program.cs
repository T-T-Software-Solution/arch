using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shopping.BackgroundTasks.Biz.Learns;
using Shopping.Shared;
using Shopping.Shared.DbContexts;
using Shopping.Shared.Interceptors;
using Shopping.Shared.Requests.Learns;
using System.Reflection;
using TTSS.Core;
using TTSS.Infra.Data.Sql;

var app = await TTSSBuilder.BuildAsync(args, builder =>
{
    var assemblies = new[]
    {
        Assembly.GetExecutingAssembly(),
        typeof(MappingProfileRegistrar).Assembly,
    };

    var dbConnection = builder.Configuration?.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string is not found.");

    // Register services
    builder.Services
        .RegisterTTSSCore(assemblies)
        .RegisterRemoteRequest(dbConnection, assemblies);

    // Register databases
    //builder.Services
    //    .SetupSqlDatabase(it => it.UseNpgsql(dbConnection))
    //        .AddDbContext<ShoppingDbContext>()
    //    .Build(cfg => cfg.Register<AuditInterceptor>());
});

//var logger = app.GetLogger<Program>();

//var totalProducts = await app.MessagingHub.SendAsync(new CountAllProducts());
//logger.LogInformation("Total products in the database is: {@totalProducts}", totalProducts);

//var timer = new System.Timers.Timer(TimeSpan.FromSeconds(2));
//timer.Elapsed += async (_, _) =>
//{
//    Console.WriteLine("----------- SEND -----------");
//    var bus = app.ScopedServiceProvider.GetRequiredService<IBus>();
//    //var sender = await bus.GetSendEndpoint(new($"db://localhost/{nameof(GreetingHandler)}"));
//    var sender = await bus.GetSendEndpoint(new($"db://localhost/{nameof(Greeting)}"));
//    await sender.Send<Greeting>(new Greeting { Message = "Hello, world!" }, ctx => ctx.CorrelationId = Guid.NewGuid());
//    //Console.WriteLine("--- Publish ---");
//    //await bus.Publish(new Greeting { Message = "Hi!" }, ctx => ctx.CorrelationId = Guid.NewGuid());

//    //var result = await bus.Request<Ping, Pong>(new Ping(5, 7));
//    //logger.LogInformation(result.Message.ToString());
//    //var pingSender = await bus.Request<Ping,Pong>.GetSendEndpoint(new($"db://localhost/{nameof(PingHandler)}"));
//    //pingSender
//};
//timer.Start();

app.Run();