using AutoMapper;
using Sample15.Basic.WebApp.WorkWithDatabase.Entities;
using Sample15.Basic.WebApp.WorkWithDatabase.Entities.ViewModels;

namespace Sample15.Basic.WebApp.WorkWithDatabase;

sealed class MappingProfileRegistrar : Profile
{
    public MappingProfileRegistrar()
    {
        CreateMap<Student, StudentVm>()
            .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher.FullName));

        CreateMap<Teacher, TeacherVm>()
            .ForMember(dest => dest.SuperviseeIds, opt => opt.MapFrom(src => src.Students.Select(it => it.Id)));
    }
}