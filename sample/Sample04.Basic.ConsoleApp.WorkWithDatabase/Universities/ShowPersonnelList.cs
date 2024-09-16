using Sample04.Basic.ConsoleApp.WorkWithDatabase.Entities;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;

namespace Sample04.Basic.ConsoleApp.WorkWithDatabase.Universities;

public sealed record ShowPersonnelList : IRequesting;

file sealed class Handler(IRepository<Teacher> teacherRepo, IRepository<Student> studentRepo) : RequestHandler<ShowPersonnelList>
{
    public override void Handle(ShowPersonnelList request)
    {
        Console.WriteLine();
        Console.WriteLine("Teachers:");
        var teachers = teacherRepo.Get().ToList();
        foreach (var item in teachers)
        {
            Console.WriteLine($"- {item.FullName},\tSalary: {item.Salary},\tCreated: {item.CreatedDate}");
        }
        Console.WriteLine();

        var allStudents = studentRepo.Get().ToList();

        Console.WriteLine("Active Students:");
        var activeStudents = allStudents.Where(it => it.DeletedDate is null);
        foreach (var item in activeStudents)
        {
            Console.WriteLine($"- {item.FullName},\tGPA: {item.GPA},\tAdvisorName: {item.Teacher?.FullName}, Created: {item.CreatedDate}");
        }
        Console.WriteLine();

        Console.WriteLine("Deleted Students:");
        var deletedStudents = allStudents.Where(it => it.DeletedDate is not null);
        foreach (var item in deletedStudents)
        {
            Console.WriteLine($"- {item.FullName},\tGPA: {item.GPA},\tAdvisorName: {item.Teacher?.FullName}, Created: {item.CreatedDate}, Deleted: {item.DeletedDate}");
        }
        Console.WriteLine(new string('-', 60));
        Console.WriteLine();
    }
}