using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sample04.Basic.ConsoleApp.WorkWithDatabase.DbContexts;
using Sample04.Basic.ConsoleApp.WorkWithDatabase.Students;
using Sample04.Basic.ConsoleApp.WorkWithDatabase.Universities;
using System.Reflection;
using TTSS.Core;
using TTSS.Core.Messaging;
using TTSS.Infra.Data.Sql;

// Register TTSS Core
var services = new ServiceCollection();
services
    .RegisterTTSSCore([Assembly.GetExecutingAssembly()]);

// Register the DbContext to use in-memory database
var connBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
var connection = new SqliteConnection(connBuilder.ConnectionString);
services
    .SetupSqlDatabase(it => it.UseSqlite(connection, opt => opt.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)))
        .AddDbContext<UniversityDbContext>()
    .Build();

// Create new 8 students
var provider = services.BuildServiceProvider();
var hub = provider.GetRequiredService<IMessagingHub>();
var studentIds = await hub.SendAsync(new StudentCreate(8));

// Delete the last student
var isSuccess = await hub.SendAsync(new StudentDelete(studentIds.Last()));
Console.WriteLine($"Is the last student deleted? {isSuccess}");

// Create a new teacher via and tie the students to the teacher
await hub.SendAsync(new TeacherCreate { FullName = "Dr.Smith", Salary = 50000, StudentIds = studentIds });

// Show the students and teachers
await hub.SendAsync(new ShowPersonnelList());

// Delete the first student
isSuccess = await hub.SendAsync(new StudentDelete(studentIds.First(), true));
Console.WriteLine($"Is the first student deleted? {isSuccess}");

// Show the students and teachers
await hub.SendAsync(new ShowPersonnelList());

// Key takeaways from this example:
// 1. Register DbContext in the service collection (Lines 21-23).
// 2. Use IRequesting<T> to access a database table.
// 3. CreatedDate, UpdatedDate, and DeletedDate are auto-stamped.