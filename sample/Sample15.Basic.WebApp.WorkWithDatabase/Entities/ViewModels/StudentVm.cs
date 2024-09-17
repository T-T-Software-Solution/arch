namespace Sample15.Basic.WebApp.WorkWithDatabase.Entities.ViewModels;

public sealed record StudentVm
{
    public required string Id { get; set; }
    public required string FullName { get; set; }
    public double GPA { get; set; }
    public required string TeacherName { get; set; }
}