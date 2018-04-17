using Arragro.TestBase;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Arragro.Core.Common.Tests.Repository.UseCase
{
    public class IRepositoryUseCase
    {
        public readonly List<ModelFoo> ModelFoos;

        public IRepositoryUseCase()
        {
            ModelFoos = ModelFoos.InitialiseAndLoadModelFoos();
        }
        
        [Fact]
        public void RepositoryUseCaseMoq()
        {
            var modelFooRepository = MockHelper.GetMockRepository(ModelFoos);
            Assert.Equal("Test 1", modelFooRepository.Find(1).Name);
            Assert.Equal("Test 2", modelFooRepository.Find(2).Name);
            modelFooRepository.InsertOrUpdate(new ModelFoo { Name = "Test 3" }, true);
            Assert.Equal("Test 3", modelFooRepository.Find(3).Name);
            modelFooRepository.InsertOrUpdate(new ModelFoo { Id = 3, Name = "Testy" }, false);
            Assert.Equal(3, modelFooRepository.All().Count());
            modelFooRepository.Delete(3);
            Assert.Equal(2, modelFooRepository.All().Count());
        }
    }
}
