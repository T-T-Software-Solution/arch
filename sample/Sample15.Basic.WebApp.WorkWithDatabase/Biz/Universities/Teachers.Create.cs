using AutoMapper;
using Sample15.Basic.WebApp.WorkWithDatabase.Entities;
using Sample15.Basic.WebApp.WorkWithDatabase.Entities.ViewModels;
using System.Net;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Sample15.Basic.WebApp.WorkWithDatabase.Biz.Universities;

public sealed record TeachersCreate : IHttpRequesting<TeacherVm>
{
    public required string FullName { get; init; }
    public double Salary { get; init; }
}

file sealed class Handler(IRepository<Teacher> teacherRepo, IMapper mapper) : HttpRequestHandlerAsync<TeachersCreate, TeacherVm>
{
    public override async Task<IHttpResponse<TeacherVm>> HandleAsync(TeachersCreate request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.FullName)
            || request.Salary < 0)
        {
            return Response(HttpStatusCode.BadRequest, "Invalid input data");
        }

        var entity = new Teacher
        {
            Id = Guid.NewGuid().ToString(),
            FullName = request.FullName,
            Salary = request.Salary,
        };
        await teacherRepo.InsertAsync(entity, cancellationToken);
        var vm = mapper.Map<TeacherVm>(entity);
        return Response(HttpStatusCode.Created, vm);
    }
}