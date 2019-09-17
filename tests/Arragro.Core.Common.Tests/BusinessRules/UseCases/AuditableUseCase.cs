using Arragro.Core.Common.BusinessRules;
using Arragro.TestBase;
using System;
using Xunit;

namespace Arragro.Core.Common.Tests.BusinessRules.UseCases
{
    public class AuditableUseCase
    {
        [Fact]
        public void AuditableUseCaseTestFoo()
        {
            var entityFoo = new ModelFooInt
            {
                Id = 1,
                CreatedBy = 1,
                ModifiedBy = 1,
                CreatedDate = DateTimeOffset.UtcNow,
                ModifiedDate = DateTimeOffset.UtcNow
            };
        }

        [Fact]
        public void AuditableUseCaseTestBar()
        {
            var userId = Guid.NewGuid();
            var entityBar = new ModelFooGuid
            {
                Id = 1,
                CreatedBy = userId,
                ModifiedBy = userId,
                CreatedDate = DateTimeOffset.UtcNow,
                ModifiedDate = DateTimeOffset.UtcNow
            };
        }
    }
}
