namespace BusinessLogic
{
    public class Class1
    {
        public void test()
        {
            var factory = new Factory();
            var repository = factory.GetDataRepository();
            repository.Save();
        } 
    }
}