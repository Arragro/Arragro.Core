using Arragro.Core.Common.Repository;
using Arragro.TestBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

namespace Arragro.Core.EntityFrameworkCore.IntegrationTests
{
    public class IntegrationTests
    {
        private readonly DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder<FooContext>().UseInMemoryDatabase();

        private void WithDbContext(Action<FooContext> action)
        {
            using (var context = new FooContext(optionsBuilder.Options))
            {
                action.Invoke(context);
            }
        }

        [Fact]
        public void add_record_to_database()
        {
            WithDbContext(x =>
                {
                    x.ModelFoos.Add(new ModelFoo { Name = "Test 2" });
                    x.SaveChanges();

                    var modelFoo = x.ModelFoos.Single(y => y.Name == "Test 2");
                    Assert.Equal("Test 2", modelFoo.Name);
                    Assert.NotSame(default(int), modelFoo.Id);
                });
        }

        [Fact]
        public void use_ModelFooService_to_interact()
        {
            using (var context = new FooContext(optionsBuilder.Options))
            {
                var modelFooRepository = new ModelFooRepository(context);
                var modelFooService = new ModelFooService(modelFooRepository);

                var modelFoo = modelFooService.ValidateAndInsertOrUpdate(new ModelFoo { Name = "Test 1" });
                modelFooService.SaveChanges();

                Assert.NotSame(default(int), modelFoo);
            }
        }

        [Fact]
        public void composite_foo_tests()
        {
            using (var context = new FooContext(optionsBuilder.Options))
            {
                var compositeFooRepository = new CompositeFooRepository(context);
                var compositeFooService = new CompositeFooService(compositeFooRepository);

                var modelFoo = compositeFooService.InsertOrUpdate(new CompositeFoo { Name = "Test 1" }, true);
                compositeFooService.SaveChanges();

                modelFoo = compositeFooService.Find(0, 0);

                Assert.NotNull(modelFoo);
            }
        }
    }
}
