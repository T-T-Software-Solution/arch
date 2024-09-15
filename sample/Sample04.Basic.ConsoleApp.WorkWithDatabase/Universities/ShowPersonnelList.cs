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
        Console.WriteLine("Teachers:");
        var teachers = teacherRepo.Get().ToList();
        foreach (var item in teachers)
        {
            Console.WriteLine($"- {item.FullName},\tSalary: {item.Salary},\tCreated: {item.CreatedDate}");
        }

        Console.WriteLine("Students:");
        var students = studentRepo.Get().ToList();
        foreach (var item in students)
        {
            Console.WriteLine($"- {item.FullName},\tGPA: {item.GPA},\tAdvisorName: {item.Teacher?.FullName}, Created: {item.CreatedDate}");
        }
    }
}