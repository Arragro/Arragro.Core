using Arragro.Core.Common.Helpers;
using System.ComponentModel;
using Xunit;

namespace Arragro.Core.Common.Tests.Helpers.UnitTests
{
    public enum TestEnum
    {
        [Description("This is my description")]
        Test,
        EmptyTest
    }


    public class EnumHelperTests
    {
        [Fact]
        public void test_the_enum_description_returns()
        {
            Assert.Equal("This is my description", TestEnum.Test.GetDescription());
        }

        [Fact]
        public void test_the_enum_returns_enum_name()
        {
            Assert.Equal(TestEnum.EmptyTest.GetDescription(), TestEnum.EmptyTest.ToString());
        }
    }
}
