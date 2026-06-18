using StudentManagement.Core.Interfaces;
using StudentManagement.Core.Models;

namespace StudentManagement.Core.Services;
using StudentManagement.Core.Exceptions;
using Microsoft.Extensions.Logging;

public class StudentService : IStudentService
{
    private readonly ILogger<StudentService> _logger;
    private readonly IStudentRepository _repository;
    private readonly IExteneralApi _externalApiService;

    public StudentService(
        IStudentRepository repository,
        IExteneralApi externalApiService,
        ILogger<StudentService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _externalApiService = externalApiService ?? throw new ArgumentNullException(nameof(externalApiService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void AddStudent(Students student)
    {
        if (string.IsNullOrWhiteSpace(student.Name))
        {
            _logger.LogInformation("Studen name is empty");
            throw new ArgumentException("Student name cannot be empty.");
        }

        var existingstudent = _repository.GetById(student.Id);
        if (existingstudent != null)
        {
            _logger.LogInformation($"Student ID {existingstudent.Id} already exists");
            throw new DuplicateStudentException(existingstudent.Id);
        }

        _repository.Add(student);
    }

    public List<Students> GetAllStudents()
    {
        return _repository.GetAll();
    }

    public Students? GetStudentById(int id)
    {
        return _repository.GetById(id);
    }

    public void UpdateStudent(Students student)
    {
        if (string.IsNullOrWhiteSpace(student.Name))
        {
            throw new ArgumentException("Student name cannot be empty");
        }

        var existingStudent = _repository.GetById(student.Id);

        if (existingStudent == null)
        {
            throw new StudentNotFoundException(student.Id);
        }

        _repository.Update(student);
    }
    
    public List<string> SortStudentsByName()
    {
        var students = _repository.GetAll();
        var sortedStudents = students.OrderBy(s => s.Name).Select(s => s.Name).ToList();
        _logger.LogInformation("Students sorted by name successfully");
        return sortedStudents;

    }
    
    public async Task FetchExternalDataAsync()
    {
        var students = _repository.GetAll();
        

        var tasks = students.Select(async student =>
        {
            student.ExternalData =
                await _externalApiService.GetExternalDataAsync();
        });
        _logger.LogInformation("External data fetched successfully for students");

        await Task.WhenAll(tasks);

        foreach (Students student in students)
        {
            _repository.Update(student);
        }
        _logger.LogInformation("Students updated successfully with external data");
    }
    
    public List<Students> GetStudentsByGrade(Grade grade)
    {
        _logger.LogInformation($"Students retrieved successfully by grade {grade}");
        return _repository.GetAll()
            .Where(g => g.Grade == grade)
            .ToList();
    }

    public void DeleteStudent(int id)
    {
        var student = _repository.GetById(id);

        if (student == null)
        {
            throw new StudentNotFoundException(id);
        }

        _repository.Delete(id);
    }
    
    public Grade CalculateAverageGrade()
    {
        double avg= _repository
            .GetAll()
            .Average(student => student.Grade switch
            {
                Grade.A => 4,
                Grade.B => 3,
                Grade.C => 2,
                Grade.D => 1,
                Grade.F => 0,
                _ => 0
            });
        _logger.LogInformation("Average grade calculated successfully");
        
        return avg switch
        {
            >= 3.5 => Grade.A,
            >= 2.5 => Grade.B,
            >= 1.5 => Grade.C,
            >= 0.5 => Grade.D,
            _ => Grade.F
        };
    }
    
    public IEnumerable<object> GroupStudentsByGrade()
    {
        _logger.LogInformation("Students grouped by grade successfully");
        return _repository
            .GetAll()
            .GroupBy(student => student.Grade)
            .Select(group => new
            {
                Grade = group.Key,
                Count = group.Count()
            });
    }

}