namespace Sample07.Basic.ConsoleApp.ModelMapping.ViewModels;

public sealed record NormalVm
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public int Age { get; set; }
}