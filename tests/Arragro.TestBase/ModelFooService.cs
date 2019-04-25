using Arragro.Core.Common.BusinessRules;
using Arragro.Core.Common.Exceptions;
using Arragro.Core.Common.Interfaces;
using Arragro.Core.Common.Repository;
using Arragro.Core.Common.ServiceBase;
using System;
using System.Linq;

namespace Arragro.TestBase
{
    public class ModelFooService : Service<IRepository<ModelFoo>, ModelFoo>
    {
        public const string DuplicateName = "There is already a Model Foo with that name in the repository";
        public const string RequiredName = "The Name field is required.";
        public const string RangeLengthName = "The Name field must have between 3 and 6 characters";

        public ModelFooService(
            IRepository<ModelFoo> modelFooRepository)
            : base(modelFooRepository)
        {
        }

        /*
         * This function would be implemented further down the chain as
         * BusinessRulesBase provides the structure, not the implementation
         * which would be custom per Model.
         *
         * This would occur on a InsertOrUpdate at the service layer.
         */

        protected override void ValidateModelRules(ModelFoo modelFoo)
        {
            if (Repository.All()
                    .Where(x => x.Id != modelFoo.Id
                             && x.Name == modelFoo.Name).Any())
                RulesException.ErrorFor(x => x.Name, DuplicateName);

            if (!String.IsNullOrEmpty(modelFoo.Name) &&
                (modelFoo.Name.Length < 2 || modelFoo.Name.Length > 6))
                RulesException.ErrorFor(c => c.Name, RangeLengthName);
        }

        public object ValidateAndInsertOrUpdate(object empty)
        {
            throw new NotImplementedException();
        }

        protected override ModelFoo InsertOrUpdate(ModelFoo model)
        {
            model = Repository.InsertOrUpdate(model, model.Id == default(int));
            return model;
        }
    }
}