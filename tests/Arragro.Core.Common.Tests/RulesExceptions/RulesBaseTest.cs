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

        public class RulesBaseParams
        {
            public bool ThrowException { get; set; }
        }

        public class Again : RulesBase<Again>, IRulesBase<RulesBaseParams>
        {
            public string Bar { get; set; }

            public void Validate(RulesBaseParams parameters)
            {
                ValidateModelPropertiesAndBuildRulesException(this);

                if (string.IsNullOrEmpty(Bar))
                {
                    RulesException.ErrorFor(x => x.Bar, "Please supply a Bar");
                }

                if (parameters.ThrowException)
                    RulesException.ThrowException();
            }
        }

        public class Else : RulesBase<Else>, IRulesBase<RulesBaseParams>
        {
            public string Foo { get; set; }

            public Again Again { get; set; } = new Again();

            public List<Again> Agains { get; set; } = new List<Again>();

            public void Validate(RulesBaseParams parameters)
            {
                ValidateModelPropertiesAndBuildRulesException(this);

                if (string.IsNullOrEmpty(Foo))
                {
                    RulesException.ErrorFor(x => x.Foo, "Please supply a Foo");
                }

                if (parameters.ThrowException)
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
                ValidateModelPropertiesAndBuildRulesException(this);
                var parameters = new RulesBaseParams { ThrowException = false };

                RulesException.ErrorFor(x => x.Value, "This is a fake error");

                var rulesExceptionCollection = ValidateModelPropertiesAndBuildRulesExceptionCollection<RulesBaseParams>(this, parameters);
                                
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
                    something.Elses.Add(new Else { Agains = new List<Again> { new Again() } });
                    something.Validate();
                }
                catch (RulesExceptionCollection ex)
                {
                    Assert.Equal(6, ex.RulesExceptions.Count);
                    var exceptionDto = ex.GetRulesExceptionDto();
                    throw;
                }
            });
        }
    }
}
