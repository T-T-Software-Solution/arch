using Sample04.Basic.ConsoleApp.WorkWithDatabase.Entities;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Services;

namespace Sample04.Basic.ConsoleApp.WorkWithDatabase.Universities;

public sealed record StudentDelete(string StudentId, bool IsHardDelete = false) : IRequesting<bool>;

file sealed class Handler(IDateTimeService dateTimeService, IRepository<Student> studentRepo) : RequestHandlerAsync<StudentDelete, bool>
{
    public override async Task<bool> HandleAsync(StudentDelete request, CancellationToken cancellationToken = default)
    {
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
                return false;
            }

            entity.DeletedDate = dateTimeService.UtcNow;

            ack = await studentRepo.UpdateAsync(entity, cancellationToken);
        }
        return ack;
    }
}