using MongoDB.Bson;

namespace Notifications.DataAccessLayer.MongoClass
{
    class MongoEmployee
    {
        public ObjectId  Id { get; set; }
        public int EmployeeId { get; set; }
        public string Name { get; set; }
    }
}
