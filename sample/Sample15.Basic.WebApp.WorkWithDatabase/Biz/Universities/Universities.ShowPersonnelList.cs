using AutoMapper;
using Sample15.Basic.WebApp.WorkWithDatabase.Biz.Universities.ViewModels;
using Sample15.Basic.WebApp.WorkWithDatabase.Entities;
using Sample15.Basic.WebApp.WorkWithDatabase.Entities.ViewModels;
using System.Net;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Sample15.Basic.WebApp.WorkWithDatabase.Biz.Universities;

public sealed record UniversitiesShowPersonnelList : IHttpRequesting<PersonnelListVm>;

file sealed class Handler(IRepository<Teacher> teacherRepo, IRepository<Student> studentRepo, IMapper mapper)
    : HttpRequestHandler<UniversitiesShowPersonnelList, PersonnelListVm>
{
    public override IHttpResponse<PersonnelListVm> Handle(UniversitiesShowPersonnelList request)
    {
        var teachers = teacherRepo
            .Include(it => it.Students)
            .Get()
            .Select(mapper.Map<TeacherVm>)
            .ToList();

        var allStudents = studentRepo.Get()
            .ToList();

        var activeStudents = allStudents
            .Where(it => it.DeletedDate is null)
            .Select(mapper.Map<StudentVm>);

        var deletedStudents = allStudents
            .Where(it => it.DeletedDate is not null)
            .Select(mapper.Map<StudentVm>);

        var vm = new PersonnelListVm
        {
            Teachers = teachers,
            ActiveStudents = activeStudents,
            DeletedStudents = deletedStudents,
        };
        return Response(HttpStatusCode.OK, vm);
    }
}