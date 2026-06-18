using System;

namespace StudentManagement.Core.Exceptions;

public class InvalidGradeException : Exception
{
    public InvalidGradeException(string grade)
        : base($"Invalid grade: {grade}")
    {
    }

    public InvalidGradeException(string? message, Exception? inner)
        : base(message, inner)
    {
    }
}

