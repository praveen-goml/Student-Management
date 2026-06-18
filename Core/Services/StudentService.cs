using StudentManagement.Core.Interfaces;
using StudentManagement.Core.Models;

namespace StudentManagement.Core.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repository;


    public StudentService(IStudentRepository repository)
    {
        _repository = repository;
    }


    public void AddStudent(Students student)
    {
        if (string.IsNullOrWhiteSpace(student.Name))
        {
            throw new ArgumentException("Student name cannot be empty.");
        }

        var existingstudent = _repository.GetById(student.Id);
        if (existingstudent != null)
        {
            throw new Exception("Student ID already exists");
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
            throw new Exception("Student not found");
        }

        _repository.Update(student);
    }
    
    public List<string> SortStudentsByName()
    {
        var students = _repository.GetAll();
        var sortedStudents = students.OrderBy(s => s.Name).Select(s => s.Name).ToList();

        return sortedStudents;

    }
    
    public List<Students> GetStudentsByGrade(Grade grade)
    {
        return _repository.GetAll()
            .Where(g => g.Grade == grade)
            .ToList();
    }


    public void DeleteStudent(int id)
    {
        var student = _repository.GetById(id);

        if (student == null)
        {
            throw new Exception("Student not found");
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