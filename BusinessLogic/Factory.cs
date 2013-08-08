using DataAccessLayer;

namespace BusinessLogic
{
    public class Factory
    {
        public IDataRepository GetDataRepository()
        {
            return new InMemmoryRepository();
        }
    }
}