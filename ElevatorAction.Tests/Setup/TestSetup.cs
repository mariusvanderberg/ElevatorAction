using Autofac;
using ElevatorAction.Application;
using ElevatorAction.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;

namespace ElevatorAction.Tests.Setup
{
    [SetUpFixture]
    public class TestSetup
    {
        public IConfiguration Configuration;
        protected ILifetimeScope Scope;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var builder = CreateContainerBuilder();

            // Mock the input manager
            var mockInputManager = new Mock<IInputManager>();
            mockInputManager.Setup(m => m.NumberInput(It.IsAny<string>())).Returns(7); // Set a default value

            // Register the mocked input manager instance
            builder.RegisterInstance(mockInputManager.Object).As<IInputManager>();

            // Build the container
            var container = builder.Build();

            // Set the container
            SetContainer(container);

            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        // Here we can add scope, dtatabase, resolvers, etc.
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Scope.Dispose();
        }

        protected void ReplaceServiceWithMock<TService>(Mock<TService> mockService) where TService : class
        {
            // Create a new ContainerBuilder
            var builder = CreateContainerBuilder();

            // Register the mock service instance
            builder.RegisterInstance(mockService.Object).As<TService>();

            // Build the container
            var container = builder.Build();

            Scope.Dispose();

            // Set the container
            SetContainer(container);
        }

        protected T Resolve<T>() where T : class
        {
            return Scope.Resolve<T>();
        }

        protected void SetContainer(IContainer container)
        {
            Scope = container.BeginLifetimeScope();
        }

        private ContainerBuilder CreateContainerBuilder()
        {
            // Create an instance of ContainerBuilder
            var builder = new ContainerBuilder();

            var taskDelayMock = new Mock<IAsyncDelayer>(); // Create a mock of Task.Delay

            // Ensure delays do not happen during tests
            taskDelayMock.Setup(m => m.Delay(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(0));

            builder.RegisterInstance(taskDelayMock.Object).As<IAsyncDelayer>();

            // Register types and dependencies
            builder.RegisterType<ElevatorControlService>()
             .As<IElevatorControlService>()
             .AsImplementedInterfaces()
             .SingleInstance();

            return builder;
        }
    }
}
