// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
        private readonly string _theAnswer;
        private readonly string _conn;

        public AnswerProvider(string theAnswer, string conn)
        {
            _theAnswer = theAnswer;
            _conn = conn;
        }

        public string GetAnswer()
        {
            return _theAnswer;
        }

        public string GetConnectionString()
        {
            return _conn;
        }
    }

    #endregion
}