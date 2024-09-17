using Sample04.Basic.ConsoleApp.WorkWithDatabase.Entities;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;

namespace Sample04.Basic.ConsoleApp.WorkWithDatabase.Universities;

public sealed record StudentCreate(int Count) : IRequesting<IEnumerable<string>>;

file sealed class Handler(IRepository<Student> studentRepo) : RequestHandlerAsync<StudentCreate, IEnumerable<string>>
{
    public override async Task<IEnumerable<string>> HandleAsync(StudentCreate request, CancellationToken cancellationToken = default)
    {
        var random = new Random();
        var entities = new List<Student>();
        for (var i = 0; i < request.Count; i++)
        {
            var number = i + 1;
            var entity = new Student
            {
                Id = $"S{number:00}",
                FullName = $"Student {number}",
                GPA = random.Next(0, 4),
            };
            entities.Add(entity);
        }

        await studentRepo.InsertBulkAsync(entities, cancellationToken);
        return entities.Select(it => it.Id);
    }
}