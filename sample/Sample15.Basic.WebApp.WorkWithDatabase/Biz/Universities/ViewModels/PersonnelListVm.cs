using Sample15.Basic.WebApp.WorkWithDatabase.Entities.ViewModels;

namespace Sample15.Basic.WebApp.WorkWithDatabase.Biz.Universities.ViewModels;

public sealed record PersonnelListVm
{
    public IEnumerable<TeacherVm> Teachers { get; set; }
    public IEnumerable<StudentVm> ActiveStudents { get; set; }
    public IEnumerable<StudentVm> DeletedStudents { get; set; }
}