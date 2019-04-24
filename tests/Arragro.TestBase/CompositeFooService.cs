using Arragro.Core.Common.BusinessRules;
using Arragro.Core.Common.Repository;
using Arragro.Core.Common.ServiceBase;
using System;
using System.Linq;

namespace Arragro.TestBase
{
    public class CompositeFooService : Service<IRepository<CompositeFoo>, CompositeFoo>
    {
        public const string DuplicateName = "There is already a Model Foo with that name in the repository";
        public const string RequiredName = "The Name field is required.";
        public const string RangeLengthName = "The Name field must have between 3 and 6 characters";

        public CompositeFooService(IRepository<CompositeFoo> compositeFooRepository)
            : base(compositeFooRepository)
        {
        }

        /*
         * This function would be implemented further down the chain as
         * BusinessRulesBase provides the structure, not the implementation
         * which would be custom per Model.
         *
         * This would occur on a InsertOrUpdate at the service layer.
         */

        protected override void ValidateModelRules(CompositeFoo compositeFoo, params object[] otherValues)
        {
            if (Repository.All()
                    .Where(x => x.Id != compositeFoo.Id
                             && x.Name == compositeFoo.Name).Any())
                RulesException.ErrorFor(x => x.Name, DuplicateName);

            if (!String.IsNullOrEmpty(compositeFoo.Name) &&
                (compositeFoo.Name.Length < 2 || compositeFoo.Name.Length > 6))
                RulesException.ErrorFor(c => c.Name, RangeLengthName);
        }

        protected override CompositeFoo InsertOrUpdate(CompositeFoo compositeFoo)
        {
            compositeFoo = Repository.InsertOrUpdate(compositeFoo, compositeFoo.Id == default(int) && compositeFoo.SecondId == default(int));
            return compositeFoo;
        }

        public CompositeFoo InsertOrUpdate(CompositeFoo compositeFoo, bool add)
        {
            compositeFoo = Repository.InsertOrUpdate(compositeFoo, add);
            return compositeFoo;
        }
    }
}