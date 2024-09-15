using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sample04.Basic.ConsoleApp.WorkWithDatabase.DbContexts;
using Sample04.Basic.ConsoleApp.WorkWithDatabase.Entities;
using Sample04.Basic.ConsoleApp.WorkWithDatabase.Students;
using Sample04.Basic.ConsoleApp.WorkWithDatabase.Universities;
using System.Reflection;
using TTSS.Core;
using TTSS.Core.Data;
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

// Create new students
var provider = services.BuildServiceProvider();
var studentRepository = provider.GetRequiredService<IRepository<Student>>();
await studentRepository.InsertAsync(new Student { Id = Guid.NewGuid().ToString(), FullName = "John Doe", GPA = 3.5 });
await studentRepository.InsertAsync(new Student { Id = Guid.NewGuid().ToString(), FullName = "Jane Doe", GPA = 3.8 });
await studentRepository.InsertAsync(new Student { Id = Guid.NewGuid().ToString(), FullName = "Jim Doe", GPA = 3.2 });

// Create a new teacher via handler and tie the students to the teacher
var hub = provider.GetRequiredService<IMessagingHub>();
await hub.SendAsync(new TeacherCreate { FullName = "Dr.Smith", Salary = 50000 });

// Show the students and teachers
await hub.SendAsync(new ShowPersonnelList());