using NUnit.Framework;

namespace dcp.CastleWindsor.Tests
{
    [TestFixture]
    public class IntegrationTests
    {
        [Test]
        public void CustomWindsorContainer_PropertyFromAppSettings_PropertyIsLoaded()
        {
            var windsor = new DcpWindsorContainer("Windsor.xml");
            var answerProvider = windsor.Resolve<IAnswerProvider>();

            var theAnswer = answerProvider.GetAnswer();

            Assert.That(theAnswer, Is.EqualTo("42"));
        }

        [Test]
        public void CustomWindsorContainer_PropertyFromConnectionStringSettings_PropertyIsLoaded()
        {
            var windsor = new DcpWindsorContainer("Windsor.xml");
            var answerProvider = windsor.Resolve<IAnswerProvider>();

            var theAnswer = answerProvider.GetConnectionString();

            Assert.That(theAnswer, Is.EqualTo("connectionString"));
        }
    }

    #region Support classes

    public interface IAnswerProvider
    {
        string GetAnswer();
        string GetConnectionString();
    }

    public class AnswerProvider : IAnswerProvider
    {
        private readonly string theAnswer;
        private readonly string conn;

        public AnswerProvider(string theAnswer, string conn)
        {
            this.theAnswer = theAnswer;
            this.conn = conn;
        }

        public string GetAnswer()
        {
            return theAnswer;
        }

        public string GetConnectionString()
        {
            return conn;
        }
    }

    #endregion
}