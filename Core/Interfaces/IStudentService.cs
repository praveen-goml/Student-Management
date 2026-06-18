using StudentManagement.Core.Models;

namespace StudentManagement.Core.Interfaces;

public interface IStudentService
{
    void AddStudent(Students student);
    List<Students> GetAllStudents();
    Students? GetStudentById(int id);
    void UpdateStudent(Students student);
    void DeleteStudent(int id);
    
    List<Students> GetStudentsByGrade(Grade grade);
    
    List<string> SortStudentsByName();
    
    Grade CalculateAverageGrade();
    
    
}