using System.Text.Json;
using StudentManagement.Core.Interfaces;
using StudentManagement.Core.Models;
using Microsoft.Extensions.Logging;
using StudentManagement.Core.Services;

namespace StudentManagement.Infrastructure.Repositories;

public class JsonStudentRepository :IStudentRepository
{
    
    private readonly string _filePath = ResolveFilePath();
    private readonly ILogger<StudentService> _logger;
    public JsonStudentRepository(ILogger<StudentService> logger)
    {
        _logger = logger;
    }

    private static string ResolveFilePath()
    {
        var currentDirectory = new DirectoryInfo(AppContext.BaseDirectory);

        while (currentDirectory != null)
        {
            var projectFile = Path.Combine(currentDirectory.FullName, "StudentManagement.csproj");
            var solutionFile = Path.Combine(currentDirectory.FullName, "StudentManagement.sln");

            if (File.Exists(projectFile) || File.Exists(solutionFile))
            {
                return Path.Combine(currentDirectory.FullName, "Data", "Students.json");
            }

            currentDirectory = currentDirectory.Parent;
        }

        return Path.Combine(Directory.GetCurrentDirectory(), "Data", "Students.json");
    }
    
    private List<Students> LoadStudents()
    {
        if (!File.Exists(_filePath))
        {
            return new List<Students>();
        }

        string json = File.ReadAllText(_filePath);
        List<Students>? studentsList = JsonSerializer.Deserialize<List<Students>>(json);

        return studentsList ?? new List<Students>();
    }
    
    private void SaveStudents(List<Students> students)
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

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
        _logger.LogInformation("Student added successfully");
        
        SaveStudents(students);
    }

    public List<Students> GetAll()
    {
        _logger.LogInformation("All students retrieved successfully");
        return LoadStudents();
    }

    public Students? GetById(int id)
    {
        var studentList = LoadStudents();
        var student = studentList.FirstOrDefault(s => s.Id == id);
    
        _logger.LogInformation($"Student retrieved successfully by {id}");
        return student;
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
            _logger.LogInformation("Student with id {existingStudent.Id} updated successfully");
            SaveStudents(students);
        }
        else
        {
           _logger.LogInformation("Student with id {existingStudent.Id} already exists");
            
        }
    }

    public void Delete(int id)
    {
        var students = LoadStudents();
        var studentToRemove = students.FirstOrDefault(s => s.Id == id);
        if (studentToRemove != null)
        {
            students.Remove(studentToRemove);
            _logger.LogInformation("Student with id {existingStudent.Id} deleted successfully");
            SaveStudents(students);
        }
        else
        {
            
            _logger.LogInformation("Student with id {existingStudent.Id} not found");
        }
    }
    
    
    
}