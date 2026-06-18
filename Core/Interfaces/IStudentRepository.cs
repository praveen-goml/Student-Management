using StudentManagement.Core.Models;

namespace StudentManagement.Core.Interfaces;

public interface IStudentRepository
{
    void Add(Students student);
    List<Students> GetAll();
    Students? GetById (int id);
    void Update(Students student);
    void Delete(int id);
}