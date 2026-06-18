namespace StudentManagement.Core.Interfaces;

public interface IExteneralApi
{
    Task<string> GetExternalDataAsync();
}