namespace Sample15.Basic.WebApp.WorkWithDatabase.Entities.ViewModels;

public sealed record TeacherVm
{
    public required string Id { get; set; }
    public required string FullName { get; set; }
    public double Salary { get; set; }
    public IEnumerable<string> SuperviseeIds { get; set; } = [];
}