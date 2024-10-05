namespace TTSS.Core.Facades.Models;

public class Student
{
    public int Age { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public bool IsMale { get; set; }
    public double GPA { get; set; }
    public StudentType Type { get; set; }
}

public enum StudentType
{
    Regular = 1,
    Exchange = 2,
}