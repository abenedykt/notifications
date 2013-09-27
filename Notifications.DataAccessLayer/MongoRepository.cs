using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Notifications.Base;
using Notifications.BusiessLogic;
using Notifications.DataAccessLayer.MongoClass;


namespace Notifications.DataAccessLayer
{
    class MongoRepository :IDataRepository
    {
        
        
        public MongoRepository(MongoStringConnection mongoConnection)
        {     
        }
      
        public int AddNotification(INotification notification)
        {
            throw new NotImplementedException();
        }

        public void AddMessage(IMessage message)
        {
            throw new NotImplementedException();
        }

        public List<INotification> GetReceiveNotifications(int receiverId)
        {
            throw new NotImplementedException();
        }

        public List<INotification> GetSendNotifications(int senderId)
        {
            throw new NotImplementedException();
        }

        public List<IMessage> GetMessages(int employeeId1, int employeeId2)
        {
            throw new NotImplementedException();
        }

        public void AddTimeofReading(int notificationId, int receiverId)
        {
            throw new NotImplementedException();
        }

        public void AddEmployee(IEmployee employee)
        {
            throw new NotImplementedException();
        }
    }
}
