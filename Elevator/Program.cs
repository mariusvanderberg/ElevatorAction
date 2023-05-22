using Autofac;
using ElevatorAction.Application;
using ElevatorAction.ConsoleUI;
using ElevatorAction.ConsoleUI.Helpers;
using ElevatorAction.ConsoleUI.Interfaces;
using ElevatorAction.Domain.Interfaces;

// Create an instance of ContainerBuilder
var builder = new ContainerBuilder();

// Register types and dependencies
builder.RegisterType<ElevatorControlService>().As<IElevatorControlService>();
builder.RegisterType<ElevatorService>().As<IElevatorService>();
builder.RegisterType<InputManager>().As<IInputManager>();
builder.RegisterType<OutputManager>().As<IOutputManager>();
builder.RegisterType<Simulator>().As<ISimulator>();

// Build the container
var container = builder.Build();

// Resolve and use the dependencies
using (var scope = container.BeginLifetimeScope())
{
    var myService = scope.Resolve<ISimulator>();
    myService.Start();
}