using StudentManagement.Core.Models;

namespace StudentManagement.Presentation;
using StudentManagement.Core.Interfaces;
using System;
public class ConsoleMenu
{
    private readonly IStudentService _studentService;

    public ConsoleMenu(IStudentService studentService)
    {
        _studentService = studentService;
    }
    
    public void Start()
    {
        while (true)
        {
            Console.WriteLine("\n===== Student Management System =====");
            Console.WriteLine("1. Add Student");
            Console.WriteLine("2. View All Students");
            Console.WriteLine("3. Search Student By ID");
            Console.WriteLine("4. Update Student");
            Console.WriteLine("5. Delete Student");
            Console.WriteLine("6. View Students By Grade");
            Console.WriteLine("7. Sort Students By Name");
            Console.WriteLine("8. Calculate Average Grade");
            Console.WriteLine("9. Group Students By Grade");
            Console.WriteLine("0. Exit");

            Console.Write("Enter your choice: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    
                    AddStudent();
                    break;

                case "2":
                    GetAllStudents();
                    break;

                case "3":
                    GetStudentById();
                    break;

                case "4":
                    UpdateStudent();
                    break;

                case "5":
                    DeleteStudent();
                    break;
                
                case "6":
                    GetStudentsByGrade();
                    break;
                case "7":
                    SortStudentsByName();
                    break;
                case "8":
                    CalculateStudentAverage();
                    break;
                
                case "9":
                    GropStudentsByGrade();
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        }
    }

    private void AddStudent()
    {
        Console.WriteLine("Enter student ID: ");
        int id = int.Parse(Console.ReadLine()!);
        
        Console.WriteLine("Enter student name: ");
        string name = Console.ReadLine()!;
        
        Console.WriteLine("Enter student grade: ");
        string grade = Console.ReadLine()!;

        if (!Enum.TryParse(grade, true, out Grade res))
        {
            Console.WriteLine("Invalid grade.");
            return;
        }
        
        Students student = new Students
        {
            Id = id,
            Name = name,
            Grade = res
        };

        _studentService.AddStudent(student);
        Console.WriteLine("Student added successfully.");
    }
    private void GropStudentsByGrade()
    {
        Console.WriteLine("Enter student grade: ");
        string grade = Console.ReadLine()!;

        if (!Enum.TryParse(grade, true, out Grade res))
        {
            Console.WriteLine("Invalid grade.");
            return;
        }
        
        var students = _studentService.GetStudentsByGrade(res);

        foreach (var student in students)
        {
            Console.WriteLine($"{student.Id}: {student.Name} - {student.Grade}");
        }
    }
    
    private List<string> SortStudentsByName()
    {
        var sortedStudents = _studentService.SortStudentsByName();

        foreach (var student in sortedStudents)
        {
            Console.WriteLine(student);
        }

        return sortedStudents;
    }

    private void GetAllStudents()
    {
        var students = _studentService.GetAllStudents();

        foreach (var student in students)
        {
            Console.WriteLine($"{student.Id}: {student.Name} - {student.Grade}");
        }
    }

    private void GetStudentById()
    {
        Console.WriteLine("Enter student ID: ");
        int id = int.Parse(Console.ReadLine()!);

        var student = _studentService.GetStudentById(id);

        if (student == null)
        {
            Console.WriteLine("Student not found.");
            return;
        }

        Console.WriteLine($"{student.Id}: {student.Name} - {student.Grade}");
    }

    private void UpdateStudent()
    {
        Console.WriteLine("Enter student ID: ");
        int id = int.Parse(Console.ReadLine()!);

        Console.WriteLine("Enter student name: ");
        string name = Console.ReadLine()!;

        Console.WriteLine("Enter student grade: ");
        string grade = Console.ReadLine()!;

        if (!Enum.TryParse(grade, true, out Grade res))
        {
            Console.WriteLine("Invalid grade.");
            return;
        }

        Students student = new Students
        {
            Id = id,
            Name = name,
            Grade = res
        };

        _studentService.UpdateStudent(student);
        Console.WriteLine("Student updated successfully.");
    }

    private void DeleteStudent()
    {
        Console.WriteLine("Enter student ID: ");
        int id = int.Parse(Console.ReadLine()!);

        _studentService.DeleteStudent(id);
        Console.WriteLine("Student deleted successfully.");
    }
    
    private void GetStudentsByGrade()
    {
        Console.WriteLine("Enter student grade: ");
        string grade = Console.ReadLine()!;

        if (!Enum.TryParse(grade, true, out Grade res))
        {
            Console.WriteLine("Invalid grade.");
            return;
        }
        
        var students = _studentService.GetStudentsByGrade(res);

        foreach (var student in students)
        {
            Console.WriteLine($"{student.Id}: {student.Name} - {student.Grade}");
        }
    }

    private void CalculateStudentAverage()
    {
       Grade res=_studentService.CalculateAverageGrade();
       Console.WriteLine($"Average Grade: {res}");
    }
}