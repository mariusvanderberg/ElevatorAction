using Autofac;
using ElevatorAction.Application;
using ElevatorAction.ConsoleUI;
using ElevatorAction.ConsoleUI.Helpers;
using ElevatorAction.ConsoleUI.Interfaces;
using ElevatorAction.Domain.Entities;
using ElevatorAction.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Build the Autofac container
var container = BuildContainer();

// Resolve and use the dependencies
using (var scope = container.BeginLifetimeScope())
{
    // Resolve the elevator background service
    var elevatorBackgroundService = scope.Resolve<IHostedService>();

    // Start the elevator background service
    await elevatorBackgroundService.StartAsync(new CancellationToken());

    var myService = scope.Resolve<ISimulator>();
    await myService.Start();
}

static IContainer BuildContainer()
{
    // Create an instance of ContainerBuilder
    var builder = new ContainerBuilder();

    // Register types and dependencies
    builder.RegisterType<ElevatorControlService>()
     .As<IElevatorControlService>()
     .AsImplementedInterfaces()
     .SingleInstance();
    //builder.RegisterType<ElevatorControlService>().As<IElevatorControlService>();
    builder.RegisterType<ElevatorService>().As<IElevatorService>();
    builder.RegisterType<InputManager>().As<IInputManager>();
    builder.RegisterType<OutputManager>().As<IOutputManager>();
    builder.RegisterType<Simulator>().As<ISimulator>();

    // Uhm
    //builder.RegisterType<ElevatorService>().As<IElevatorService>().SingleInstance();

    // Assuming you have a Floor implementation class
    //builder.RegisterType<Floor>().AsSelf();

    //// Register the floor entities
    //builder.RegisterInstance(new List<Floor>
    //    {
    //        // Add more floors if needed
    //    }).As<List<Floor>>();

    //builder.RegisterInstance(new List<IElevatorService>
    //{
    //    // Add more floors if needed
    //}).As<List<IElevatorService>>();

    // Register the elevator background service
    builder.RegisterType<ElevatorBackgroundService>().As<IHostedService>();

    // Build the container
    var container = builder.Build();

    return container;
}