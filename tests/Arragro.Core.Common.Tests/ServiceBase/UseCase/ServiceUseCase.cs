using Arragro.Core.Common.BusinessRules;
using Arragro.Core.Common.Repository;
using Arragro.Core.Common.RulesExceptions;
using Arragro.Core.Common.ServiceBase;
using Arragro.TestBase;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Arragro.Core.Common.Tests.Services.UseCase
{
    public class ServiceUseCase
    {
        public readonly List<ModelFoo> ModelFoos;

        public ServiceUseCase()
        {
            ModelFoos = ModelFoos.InitialiseAndLoadModelFoos();
        }

        [Fact]
        public void ModelFooServiceInsertOrUpdateUseCase()
        {
            var modelFooRepository = MockHelper.GetMockRepository(ModelFoos);
            var modelFooService = new ModelFooService(modelFooRepository);

            Assert.Throws<RulesException<ModelFoo>>(
                () =>
                {
                    try
                    {
                        modelFooService.ValidateModel(new ModelFoo());
                    }
                    catch (RulesException<ModelFoo> ex)
                    {
                        Assert.Single(ex.Errors);
                        Assert.Equal(ModelFooService.RequiredName, ex.Errors[0].Message);
                        throw;
                    }
                });

            modelFooService = new ModelFooService(modelFooRepository);

            modelFooService.ValidateAndInsertOrUpdate(new ModelFoo { Name = "Test" });
            Assert.Equal(3, ModelFoos.Count);
            Assert.Equal("Test", ModelFoos[2].Name);
        }
    }
}