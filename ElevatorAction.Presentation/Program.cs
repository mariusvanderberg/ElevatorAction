using Autofac;
using ElevatorAction.Application;
using ElevatorAction.ConsoleUI;
using ElevatorAction.ConsoleUI.Helpers;
using ElevatorAction.ConsoleUI.Interfaces;
using ElevatorAction.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

// Create an instance of IConfigurationBuilder
var configBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Build the configuration
var configuration = configBuilder.Build();

// Build the Autofac container
var container = BuildContainer(configuration);

bool useBackgroundPoller = configuration.GetValue<bool>("BackgroundQueueProcessor:Enabled");

// Resolve and use the dependencies
using (var scope = container.BeginLifetimeScope())
{
    if (useBackgroundPoller)
    {
        // Resolve the elevator background service
        var elevatorBackgroundService = scope.Resolve<IHostedService>();

        // Start the elevator background service
        await elevatorBackgroundService.StartAsync(new CancellationToken());
    }

    var myService = scope.Resolve<ISimulator>();
    await myService.Start();
}

static IContainer BuildContainer(IConfiguration configuration)
{
    // Create an instance of ContainerBuilder
    var builder = new ContainerBuilder();

    // Register the IConfiguration instance
    builder.RegisterInstance(configuration).As<IConfiguration>();

    // Register types and dependencies
    builder.RegisterType<ElevatorControlService>()
     .As<IElevatorControlService>()
     .AsImplementedInterfaces()
     .SingleInstance();
    builder.RegisterType<ElevatorService>().As<IElevatorService>();
    builder.RegisterType<InputManager>().As<IInputManager>();
    builder.RegisterType<OutputManager>().As<IOutputManager>();
    builder.RegisterType<Simulator>().As<ISimulator>();

    // Register the elevator background service
    builder.RegisterType<ElevatorBackgroundService>().As<IHostedService>();

    // Build the container
    var container = builder.Build();

    return container;
}