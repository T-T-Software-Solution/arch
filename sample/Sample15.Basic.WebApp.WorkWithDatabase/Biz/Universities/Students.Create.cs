using AutoMapper;
using Sample15.Basic.WebApp.WorkWithDatabase.Entities;
using Sample15.Basic.WebApp.WorkWithDatabase.Entities.ViewModels;
using System.Net;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Sample15.Basic.WebApp.WorkWithDatabase.Biz.Universities;

public sealed record StudentsCreate : IHttpRequesting<StudentVm>
{
    public required string FullName { get; set; }
    public double GPA { get; set; }
}

file sealed class Handler(IRepository<Student> studentRepo, IMapper mapper) : HttpRequestHandlerAsync<StudentsCreate, StudentVm>
{
    public override async Task<IHttpResponse<StudentVm>> HandleAsync(StudentsCreate request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.FullName)
            || (request.GPA < 0 || request.GPA > 4))
        {
            return Response(HttpStatusCode.BadRequest, "Invalid input data");
        }

        var entity = new Student
        {
            Id = Guid.NewGuid().ToString(),
            FullName = request.FullName,
            GPA = request.GPA,
        };
        await studentRepo.InsertAsync(entity, cancellationToken);
        var vm = mapper.Map<StudentVm>(entity);
        return Response(HttpStatusCode.Created, vm);
    }
}