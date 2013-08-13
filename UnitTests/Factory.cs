using DataAccessLayer;

namespace UnitTests
{
    public class Factory
    {
        public IDataRepository GetDataRepository()
        {
            return new SQLRepository();
        }
    }
}