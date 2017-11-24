using Arragro.TestBase;
using Arragro.Core.EntityFrameworkCore;

namespace Arragro.Core.EntityFrameworkCore.IntegrationTests
{
    public class CompositeFooRepository : DbContextRepositoryBase<CompositeFoo>
    {
        public CompositeFooRepository(FooContext fooContext) : base(fooContext) { }
    }
}
