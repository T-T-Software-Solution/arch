using Sample04.Basic.ConsoleApp.WorkWithDatabase.Entities;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;

namespace Sample04.Basic.ConsoleApp.WorkWithDatabase.Students;

public sealed record TeacherCreate : IRequesting
{
    public required string FullName { get; init; }
    public double Salary { get; init; }
}

file sealed class Handler(IRepository<Teacher> teacherRepo, IRepository<Student> studentRepo) : RequestHandlerAsync<TeacherCreate>
{
    public override async Task HandleAsync(TeacherCreate request, CancellationToken cancellationToken = default)
    {
        var allStudents = studentRepo.Get(cancellationToken);

        var entity = new Teacher
        {
            Id = Guid.NewGuid().ToString(),
            FullName = request.FullName,
            Salary = request.Salary,
            Students = allStudents.ToList()
        };
        await teacherRepo.InsertAsync(entity, cancellationToken);
    }
}
