using Arragro.Core.Common.RulesExceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

            public List<Again> Against { get; set; } = new List<Again>();

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

                RulesException.ErrorMessages.Add("This is a fake error message");
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
                    something.Elses.Add(new Else { Against = new List<Again> { new Again() } });
                    something.Validate();
                }
                catch (RulesExceptionCollection ex)
                {
                    var somethingRulesException = ex.RulesExceptions.Single(x => x.TypeName == typeof(Something).Name);
                    Assert.Single(somethingRulesException.ErrorMessages);
                    Assert.Equal(6, ex.RulesExceptions.Count);
                    var exceptionDto = ex.GetRulesExceptionDto();

                    Assert.Single(exceptionDto.ErrorMessages);
                    Assert.Equal(3, exceptionDto.Errors.Count);
                    Assert.Single(exceptionDto.RulesExceptionListContainers);
                    Assert.Single(exceptionDto.RulesExceptionListContainers[0].RulesExceptionListContainers);
                    Assert.Equal(2, exceptionDto.RulesExceptionListContainers[0].Errors.Count());
                    Assert.Single(exceptionDto.RulesExceptionListContainers[0].RulesExceptionListContainers.Where(x => x.IsRoot));
                    Assert.Single(exceptionDto.RulesExceptionListContainers[0].RulesExceptionListContainers.Single(x => x.IsRoot).Errors);

                    throw;
                }
            });
        }

        [Fact]
        public void validate_fail_case_sensitive_keys()
        {
            var rulesException = new RulesException();
            rulesException.ErrorFor("menu", "test");
            rulesException.ErrorFor("Menu", "test");
            var rulesExceptions = new List<RulesException> { rulesException };

            Assert.Throws<ArgumentException>(() =>
            {
                new RulesExceptionCollection(rulesExceptions).GetRulesExceptionDto();
            });

            var rulesExceptionCollection = new RulesExceptionCollection(rulesExceptions).GetRulesExceptionDto(false);
        }

        public class ArrayCanError
        {
            public List<string> Values;
        }

        [Fact]
        public void validate_rulesexceptioncollection_fails()
        {
            var arrayCanError = new ArrayCanError { Values = new List<string> { { "Item 1" } } };
            var rulesException = new RulesException();
            rulesException.ErrorFor("Values[0]", "Test Error");
            var rulesExceptions = new List<RulesException> { rulesException };
            var rulesExceptionCollection = new RulesExceptionCollection(rulesExceptions);
            var rulesExceptionDto = rulesExceptionCollection.GetRulesExceptionDto();
        }
    }
}
