using Microsoft.Extensions.DependencyInjection;
using StudentManagement.Core.Interfaces;
using StudentManagement.Core.Models;
using StudentManagement.Core.Services;
using StudentManagement.Infrastructure.Repositories;
using StudentManagement.Presentation;

var serviceCollection = new ServiceCollection();

serviceCollection.AddSingleton<IStudentRepository, JsonStudentRepository>();
serviceCollection.AddSingleton<IStudentService, StudentService>();
serviceCollection.AddSingleton<ConsoleMenu>();

var provider = serviceCollection.BuildServiceProvider();

var studentService =
    provider.GetRequiredService<IStudentService>();
var menu = provider.GetRequiredService<ConsoleMenu>();


Console.WriteLine("Dependency Injection Configured Successfully!");

menu.Start();