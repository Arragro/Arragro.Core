using Arragro.Core.Common.BusinessRules;
using Arragro.Core.Common.Repository;
using Arragro.TestBase;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Arragro.Core.Common.Tests.BusinessRules.UnitTests
{
    /*
     * Not replicating the functionality demonstrated in BusinessRulesUseCase as the
     * AuditableBusinessRulesBase inherits BusinessRulesBase
     */
    public class AuditableBusinessRulesUnitTests
    {
        public class ModelFooIntService : AuditableBusinessRulesBase<IRepository<ModelFooInt>, ModelFooInt, int>
        {
            public ModelFooIntService(IRepository<ModelFooInt> modelFooRepository) : base(modelFooRepository) { }

            public ModelFooInt TestAddOrUpdateAudit(ModelFooInt modelFooInt, int userId, bool add)
            {
                AddOrUpdateAudit(modelFooInt, userId, add);
                return modelFooInt;
            }
        }

        public class ModelFooGuidService : AuditableBusinessRulesBase<IRepository<ModelFooGuid>, ModelFooGuid, Guid>
        {
            public ModelFooGuidService(IRepository<ModelFooGuid> modelFooRepository) : base(modelFooRepository) { }

            public ModelFooGuid TestAddOrUpdateAudit(ModelFooGuid modelFooGuid, Guid userId, bool add)
            {
                AddOrUpdateAudit(modelFooGuid, userId, add);
                return modelFooGuid;
            }
        }

        [Fact]
        public void TestAuditableFunctionalityAgainstIntModel()
        {
            var startDateTime = DateTime.UtcNow.AddMilliseconds(-5);

            var mockRepository = new Mock<IRepository<ModelFooInt>>();
            var modelFooIntService = new ModelFooIntService(mockRepository.Object);

            var model = modelFooIntService.TestAddOrUpdateAudit(new ModelFooInt { Id = 1 }, 1, true);
            Assert.Equal(1, model.CreatedBy);
            Assert.True(model.CreatedDate > startDateTime);
            Assert.Equal(1, model.ModifiedBy);
            Assert.True(model.ModifiedDate == model.CreatedDate);

            Thread.Sleep(5);

            model = modelFooIntService.TestAddOrUpdateAudit(model, 2, false);
            Assert.Equal(1, model.CreatedBy);
            Assert.True(model.CreatedDate > startDateTime);
            Assert.Equal(2, model.ModifiedBy);
            Assert.True(model.ModifiedDate > model.CreatedDate);
        }

        [Fact]
        public void TestAuditableFunctionalityAgainstGuidModel()
        {
            var startDateTime = DateTime.UtcNow.AddSeconds(-1);

            var mockRepository = new Mock<IRepository<ModelFooGuid>>();
            var modelFooGuidService = new ModelFooGuidService(mockRepository.Object);

            var user1 = Guid.NewGuid();
            var user2 = Guid.NewGuid();

            var model = modelFooGuidService.TestAddOrUpdateAudit(new ModelFooGuid { Id = 1 }, user1, true);
            Assert.Equal(user1, model.CreatedBy);
            Assert.True(model.CreatedDate > startDateTime);
            Assert.Equal(user1, model.ModifiedBy);
            Assert.True(model.ModifiedDate == model.CreatedDate);

            Thread.Sleep(5);

            model = modelFooGuidService.TestAddOrUpdateAudit(model, user2, false);
            Assert.Equal(user1, model.CreatedBy);
            Assert.True(model.CreatedDate > startDateTime);
            Assert.Equal(user2, model.ModifiedBy);
            Assert.True(model.ModifiedDate > model.CreatedDate);
        }
    }
}
