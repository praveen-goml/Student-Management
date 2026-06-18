using Microsoft.Extensions.DependencyInjection;
using StudentManagement.Core.Interfaces;
using StudentManagement.Core.Services;
using StudentManagement.Infrastructure.ExternalApi;
using StudentManagement.Infrastructure.Repositories;
using StudentManagement.Presentation;
using Serilog;
using Microsoft.Extensions.Logging;

var serviceCollection = new ServiceCollection();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(
        "Logging/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();

serviceCollection.AddSingleton<IStudentRepository, JsonStudentRepository>();
serviceCollection.AddSingleton<IStudentService, StudentService>();
serviceCollection.AddSingleton<ConsoleMenu>();
serviceCollection.AddHttpClient<IExteneralApi, ExternalApiService>();
serviceCollection.AddLogging(builder =>
{
    builder.ClearProviders();
    builder.AddSerilog(Log.Logger, dispose: true);
});

var provider = serviceCollection.BuildServiceProvider();

var menu = provider.GetRequiredService<ConsoleMenu>();

Console.WriteLine("Dependency Injection Configured Successfully!");

try
{
    await menu.Start();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
