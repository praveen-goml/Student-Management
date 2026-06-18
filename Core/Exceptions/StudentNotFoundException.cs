using System;

namespace StudentManagement.Core.Exceptions;

public class StudentNotFoundException : Exception
{
    public StudentNotFoundException(int id)
        : base($"Student with ID {id} not found")
    {
    }

    public StudentNotFoundException(string? message)
        : base(message)
    {
    }
}

