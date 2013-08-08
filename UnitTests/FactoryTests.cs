using DataAccessLayer;
using Xunit;

namespace UnitTests
{
    public class FactoryTests
    {
        [Fact]
        public void GetDataRepository_returns_propper_type()
        {
            //arrange
            var factory = new Factory();

            //act
            var repository = factory.GetDataRepository();

            //assert
            Assert.IsAssignableFrom<IDataRepository>(repository);
        } 
    }
}