namespace StudentManagement.Core.Models;

public class Students
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Grade Grade { get; set; }
    
    public string ExternalData { get; set; } = string.Empty;
}