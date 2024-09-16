namespace Sample07.Basic.ConsoleApp.ModelMapping.ViewModels;

public sealed record EasyVm
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public DateTime BirthDate { get; set; }
}