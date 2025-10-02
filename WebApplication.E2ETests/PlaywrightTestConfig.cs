namespace WebApplication.WebApplication.E2ETests
{
    [SetUpFixture]
    public class PlaywrightTestConfig
    {
        [OneTimeSetUp]
        public void GlobalSetup()
        {
            Console.WriteLine("🎭 Configuration globale des tests Playwright");
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
            Console.WriteLine("🎭 Nettoyage global des tests Playwright");
        }
    }
}
