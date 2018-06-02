using Arragro.Core.Common.RulesExceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

            Assert.Equal(2, foo.ErrorMessages.Count);
        }

        public class Else : RulesBase<Else>, IRulesBase
        {
            public string Value { get; set; }

            public void Validate(bool throwException = true)
            {
                ValidateModelPropertiesAndBuildRulesException(this);

                if (string.IsNullOrEmpty(Value))
                {
                    RulesException.ErrorFor(x => x.Value, "Please supply a Value");
                }

                if (throwException)
                    RulesException.ThrowException();
            }
        }

        public class Something : RulesBase<Something>
        {
            [Required]
            [MaxLength(10)]
            public string Value { get; set; }
            public Else Else { get; set; } = new Else();
            public List<Else> Elses { get; set; } = new List<Else>();

            public void Validate()
            {
                var rulesExceptionCollection = new RulesExceptionCollection();
                ValidateModelPropertiesAndBuildRulesException(this);
                rulesExceptionCollection.RulesExceptions.Add(RulesException);

                Else.Validate(false);
                rulesExceptionCollection.RulesExceptions.Add(Else.RulesException);

                for (var i = 0; i < Elses.Count; i++)
                {
                    var e = Elses[i];
                    e.Validate(false);
                    e.RulesException.Prefix = $"Elses[{i}]";
                    rulesExceptionCollection.RulesExceptions.Add(e.RulesException);
                }
                
                rulesExceptionCollection.ThrowException();
            }
        }

        [Fact]
        public void validate_fail_no_value()
        {
            Assert.Throws<RulesExceptionCollection>(() =>
            {
                try
                {
                    var something = new Something();
                    something.Elses.Add(new Else());
                    something.Validate();
                }
                catch (RulesExceptionCollection ex)
                {
                    Assert.Equal(3, ex.RulesExceptions.Count);
                    var exceptionDto = ex.GetRulesExceptionDto();
                    throw;
                }
            });
        }
    }
}
