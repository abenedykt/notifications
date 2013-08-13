using DataAccessLayer;
using NSubstitute;
using Xunit;

namespace UnitTests
{
    public class Class1
    {
        [Fact]
        public void FactMethodName()
        {
            //arrange
            var repo = Substitute.For<IDataRepository>();
            //repo = new InMemmoryRepository();
            var zapisywaczka = new Zapisywaczka(repo);

            //act
            zapisywaczka.Zapisz();

            //assert
            //repo.Received().Save();
        }
    }

    public class Zapisywaczka
    {
        private readonly IDataRepository _repository;

        public Zapisywaczka(IDataRepository repository)
        {
            _repository = repository;
        }

        public void Zapisz()
        {
            //_repository.Save();
        }
    }
}