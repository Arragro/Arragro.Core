using Arragro.Core.Common.Repository;
using Arragro.Core.Common.RulesExceptions;
using Arragro.Core.Common.ServiceBase;
using Arragro.Core.EntityFrameworkCore;
using Arragro.Core.EntityFrameworkCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Arragro.Core.Common.Tests.BusinessRules.UnitTests
{
    public class RulesExceptionTestCase
    {
        public class ValidateTestArrayItem
        {
            public int ValidateTestArrayItemId { get; set; }
            [Required]
            public string ArrayItemValue { get; set; }
        }

        public class ValidateTest
        {
            public int ValidateTestId { get; set; }

            [Range(0, 1)]
            public decimal DecimalProperty { get; set; }

            [Required]
            public string StringProperty { get; set; }
            
            public List<ValidateTestArrayItem> ValidateTestArrayItems { get; set; }

            public ValidateTest()
            {
                ValidateTestArrayItems = new List<ValidateTestArrayItem>();
            }
        }

        public class ValidateTestService : Service<IRepository<ValidateTest>, ValidateTest>
        {
            public ValidateTestService(IRepository<ValidateTest> repository)
                : base(repository)
            {
            }

            protected override void ValidateModelRules(ValidateTest model, params object[] otherValues)
            {
            }

            protected override ValidateTest InsertOrUpdate(ValidateTest model)
            {
                throw new System.NotImplementedException();
            }
        }

        public class ValidateTestRepository : DbContextRepositoryAllIncludingBase<ValidateTest>
        {
            public ValidateTestRepository(IBaseContext baseContext) : base(baseContext)
            {
            }
        }

        public class ValidateTestContext : BaseContext
        {
            public virtual DbSet<ValidateTest> UrlRoutes { get; set; }


            public ValidateTestContext(DbContextOptions<ValidateTestContext> options)
                : base(options)
            {
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                if (!optionsBuilder.IsConfigured)
                {
                    optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;");
                }
            }
        }

        [Fact]
        public void RulesException_validation_converts_successfully()
        {
            var options = new DbContextOptionsBuilder<ValidateTestContext>()
                .UseInMemoryDatabase("RulesException")
                .Options;

            // Run the test against one instance of the context
            using (var context = new ValidateTestContext(options))
            {
                var validateTestRepository = new ValidateTestRepository(context);
                var validateTestService = new ValidateTestService(validateTestRepository);
                var validateTest = new ValidateTest
                {
                    StringProperty = String.Empty,
                    DecimalProperty = 2M,
                    ValidateTestArrayItems = { new ValidateTestArrayItem(), new ValidateTestArrayItem() }
                };


                Assert.Throws<RulesException<ValidateTest>>(
                    () =>
                    {
                        try
                        {
                            validateTestService.ValidateModel(validateTest);
                        }
                        catch (RulesException<ValidateTest> ex)
                        {
                            var rulesExceptionDto = new RulesExceptionDto(ex);
                            System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(rulesExceptionDto, Formatting.Indented));
                            var errorDict = ex.GetErrorDictionary();
                            Assert.Equal(2, errorDict.Count);
                            Assert.True(errorDict.ContainsKey("DecimalProperty"));
                            Assert.True(errorDict.ContainsKey("StringProperty"));
                            Assert.Equal(2, ex.Errors.Count);
                            throw;
                        }
                    });
            }

        }
    }
}