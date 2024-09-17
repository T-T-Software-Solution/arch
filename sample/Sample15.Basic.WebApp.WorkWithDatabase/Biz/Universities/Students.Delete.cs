using Sample15.Basic.WebApp.WorkWithDatabase.Entities;
using System.Net;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;
using TTSS.Core.Services;

namespace Sample15.Basic.WebApp.WorkWithDatabase.Biz.Universities;

public sealed record StudentsDelete(string StudentId, bool IsHardDelete = false) : IHttpRequesting;

file sealed class Handler(IDateTimeService dateTimeService, IRepository<Student> studentRepo) : HttpRequestHandlerAsync<StudentsDelete>
{
    public override async Task<IHttpResponse> HandleAsync(StudentsDelete request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.StudentId))
        {
            return Response(HttpStatusCode.BadRequest, "Invalid input data");
        }

        // ACK = Acknowledge of the operation
        var ack = false;
        if (request.IsHardDelete)
        {
            // Hard delete
            ack = await studentRepo.DeleteAsync(request.StudentId, cancellationToken);
        }
        else
        {
            // Soft delete
            var entity = await studentRepo.GetByIdAsync(request.StudentId, cancellationToken);
            if (entity is null)
            {
                return Response(HttpStatusCode.Gone, "Student not found");
            }

            entity.DeletedDate = dateTimeService.UtcNow;

            ack = await studentRepo.UpdateAsync(entity, cancellationToken);
        }
        return ack
            ? Response(HttpStatusCode.OK)
            : Response(HttpStatusCode.InternalServerError, "Cannot delete the student");
    }
}