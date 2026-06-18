using System.Text.Json;
using StudentManagement.Core.Interfaces;
using StudentManagement.Core.Models;

namespace StudentManagement.Infrastructure.Repositories;

public class JsonStudentRepository :IStudentRepository
{
    private readonly string _filePath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "students.json");
    
    private List<Students> LoadStudents()
    {
        string json = File.ReadAllText(_filePath);
        List<Students>? studentsList = JsonSerializer.Deserialize<List<Students>>(json);

        return studentsList ?? new List<Students>();
    }
    
    private void SaveStudents(List<Students> students)
    {
        string json = JsonSerializer.Serialize(
            students,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        File.WriteAllText(_filePath, json);
    }
    
    public void Add(Students student)
    {
        var students = LoadStudents();
        students.Add(student);
        
        SaveStudents(students);
    }

    public List<Students> GetAll()
    {
        return LoadStudents();
    }

    public Students? GetById(int id)
    {
        var students = LoadStudents();

        return students.FirstOrDefault(student => student.Id == id);
    }
    public void Update(Students student)
    {
        var students = LoadStudents();
        var existingStudent=students.FirstOrDefault(s => s.Id == student.Id);
        if (existingStudent != null)
        {
            existingStudent.Name = student.Name;
            existingStudent.Grade = student.Grade;
            existingStudent.ExternalData = student.ExternalData;
            
            SaveStudents(students);
        }
    }

    public void Delete(int id)
    {
        var students = LoadStudents();
        var studentToRemove = students.FirstOrDefault(s => s.Id == id);
        if (studentToRemove != null)
        {
            students.Remove(studentToRemove);
            SaveStudents(students);
        }
    }
    
    
    
}