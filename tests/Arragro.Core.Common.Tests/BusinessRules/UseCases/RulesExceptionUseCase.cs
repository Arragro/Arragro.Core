using Arragro.Core.Common.BusinessRules;
using Arragro.Core.Common.RulesExceptions;
using System.Linq;
using Xunit;

namespace Arragro.Core.Common.Tests.BusinessRules.UseCases
{
    public class RulesExceptionUseCase
    {
        private class ModelBar
        {
            public int ModelBarId { get; set; }
            public string Name { get; set; }
        }

        private class ModelFoo
        {
            public int ModelFooId { get; set; }
            public string Name { get; set; }

            public ModelBar ModelBar { get; set; }
        }

        [Fact]
        public void RulesExceptionUseCaseModelFoo()
        {
            var modelFoo = new ModelFoo
            {
                ModelFooId = 1,
                Name = "Test",
                ModelBar = new ModelBar
                {
                    ModelBarId = 1,
                    Name = "Test"
                }
            };

            var rulesException = new RulesException<ModelFoo>();

            // This will apply to the model as whole and should be used for 
            // scenarios where there are multiple issues against another object.
            rulesException.ErrorForModel("There is already a ModelFoo with this Id and Name");
            Assert.Single(rulesException.Errors);

            // Should be used for property issues.
            rulesException.ErrorFor(x => x.Name, "The Name is not Unique");
            rulesException.ErrorFor(x => x.ModelBar.Name, "The Name is not Unique");

            rulesException.ErrorFor("Name", "Another Error");

            var errorMessage = rulesException.ToString();
            Assert.Equal(4, rulesException.Errors.Count());
        }
    }
}
