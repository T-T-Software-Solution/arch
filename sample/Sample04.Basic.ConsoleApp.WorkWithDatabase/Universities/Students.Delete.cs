using Sample04.Basic.ConsoleApp.WorkWithDatabase.Entities;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Services;

namespace Sample04.Basic.ConsoleApp.WorkWithDatabase.Universities;

public sealed record StudentDelete(string StudentId) : IRequesting<bool>;

file sealed class Handler(IDateTimeService dateTimeService, IRepository<Student> studentRepo) : RequestHandlerAsync<StudentDelete, bool>
{
    public override async Task<bool> HandleAsync(StudentDelete request, CancellationToken cancellationToken = default)
    {
        var entity = await studentRepo.GetByIdAsync(request.StudentId, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        // Soft delete the student
        entity.DeletedDate = dateTimeService.UtcNow;

        // Acknowledge the update
        var ack = await studentRepo.UpdateAsync(entity, cancellationToken);
        return ack;
    }
}