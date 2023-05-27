using Microsoft.Extensions.Configuration;

namespace ElevatorAction.Tests
{
    [SetUpFixture]
    public class TestSetup
    {
        public IConfiguration Configuration;

        // Here we can add scope, dtatabase, resolvers, etc.

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }
    }
}
