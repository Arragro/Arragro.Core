using Arragro.Core.Common.Tests.BusinessRules.UseCases;
using Arragro.TestBase;
using System;
using Xunit;

namespace Arragro.Core.Common.Tests.BusinessRules.UnitTests
{
    public class BusinessRulesBaseUnitTest
    {
        [Fact]
        public void TestBusinessRulesBaseFailsWhenUsingNullRepository()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    var businessRulesBase = new ModelFooService(null);
                });
        }
    }
}
