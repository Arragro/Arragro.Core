using Arragro.TestBase;
using Arragro.Core.EntityFrameworkCore;

namespace Arragro.Core.EntityFrameworkCore.IntegrationTests
{
    public class ModelFooRepository : DbContextRepositoryBase<ModelFoo>
    {
        public ModelFooRepository(FooContext fooContext) : base(fooContext) { }
    }
}
