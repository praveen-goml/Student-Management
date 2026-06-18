using System;

namespace StudentManagement.Core.Exceptions;

public class DuplicateStudentException : Exception
{
    public DuplicateStudentException(int id)
        : base($"Student with ID {id} already exists")
    {
    }

    public DuplicateStudentException(string? message)
        : base(message)
    {
    }
}

