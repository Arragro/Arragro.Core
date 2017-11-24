using Arragro.Core.Common.Repository;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace Arragro.TestBase
{
    public class MockHelper
    {
        /// <summary>
        /// Builds a Mock repository as an example of how the pattern will work
        /// in the service layer.
        /// </summary>
        /// <returns>IRepository<ModelFoo, int></returns>
        public static IRepository<ModelFoo> GetMockRepository(List<ModelFoo> modelFoos)
        {
            //Instantiate the Mock
            var moqRepository = new Mock<IRepository<ModelFoo>>();

            //Build the methods exposed by the interface (not all will be required, but I have done anyway.
            //Later, this pattern can be put into a Base class using generics to make it work with any model.
            moqRepository.Setup(x => x.Find(It.IsAny<object[]>())).Returns((object[] ids) => modelFoos.SingleOrDefault(x => x.Id == (int)ids.First()));
            moqRepository.Setup(x => x.Delete(It.IsAny<object[]>())).Callback((object[] ids) =>
            {
                var modelFoo = modelFoos.SingleOrDefault(f => f.Id == (int)ids.First());
                if (modelFoo != null)
                    modelFoos.Remove(modelFoo);
            });
            moqRepository.Setup(x => x.All()).Returns(modelFoos.AsQueryable());
            moqRepository.Setup(x => x.InsertOrUpdate(It.IsAny<ModelFoo>(), It.IsAny<bool>()))
                .Returns((ModelFoo modelFoo, bool add) =>
                {
                    if (add)
                    {
                        modelFoo.Id = modelFoos.Max(f => f.Id) + 1;
                        modelFoos.Add(modelFoo);
                    }
                    else
                    {
                        modelFoos[modelFoos.FindIndex(f => f.Id == modelFoo.Id)] = modelFoo;
                    }
                    return modelFoo;
                });
            moqRepository.Setup(x => x.SaveChanges()).Returns(0);

            return moqRepository.Object;
        }
    }
}
