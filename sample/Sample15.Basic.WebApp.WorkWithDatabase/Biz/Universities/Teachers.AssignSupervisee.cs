using Sample15.Basic.WebApp.WorkWithDatabase.Entities;
using System.Net;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Sample15.Basic.WebApp.WorkWithDatabase.Biz.Universities;

public class TeachersAssignSupervisee : IHttpRequesting
{
    public required string TeacherId { get; set; }
    public required string StudentId { get; set; }
}

file sealed class Handler(IRepository<Teacher> teacherRepo, IRepository<Student> studentRepo) : HttpRequestHandlerAsync<TeachersAssignSupervisee>
{
    public override async Task<IHttpResponse> HandleAsync(TeachersAssignSupervisee request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.TeacherId)
            || string.IsNullOrWhiteSpace(request.StudentId))
        {
            return Response(HttpStatusCode.BadRequest, "Invalid input data");
        }

        var teacher = await teacherRepo.GetByIdAsync(request.TeacherId, cancellationToken);
        if (teacher is null)
        {
            return Response(HttpStatusCode.Gone, "Teacher not found");
        }

        var student = await studentRepo.GetByIdAsync(request.StudentId, cancellationToken);
        if (student is null)
        {
            return Response(HttpStatusCode.Gone, "Student not found");
        }

        teacher.Students.Add(student);
        var ack = await teacherRepo.UpdateAsync(teacher, cancellationToken);

        return ack
            ? Response(HttpStatusCode.OK)
            : Response(HttpStatusCode.InternalServerError, "Cannot assign the student to the teacher");
    }
}