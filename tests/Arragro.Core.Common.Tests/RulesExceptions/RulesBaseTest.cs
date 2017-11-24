using Arragro.Core.Common.RulesExceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Arragro.Core.Common.Tests.RulesExceptions
{
    public class Foo : RulesBase<Foo>
    {
        public string Test { get; set; }
    }

    public class RulesBaseTest
    {
        [Fact]
        public void can_serialize_and_deserialize()
        {
            var foo = new Foo { Test = "Test" };
            var dictFoo = new Dictionary<string, Foo> { { "test", foo } };
            var json = JsonConvert.SerializeObject(dictFoo, Formatting.None, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            var type = Type.GetType($"System.Collections.Generic.Dictionary`2[[System.String],[Arragro.Core.Common.Tests.RulesExceptions.Foo]]");

            var obj = JsonConvert.DeserializeObject(json, type);
        }

        [Fact]
        public void rules_base()
        {
            var foo = new RulesException<Foo>();
            foo.ErrorForModel("Test1");
            foo.ErrorForModel("Test2");

            Assert.Equal(foo.ErrorMessages.Count, 2);
        }
    }
}
