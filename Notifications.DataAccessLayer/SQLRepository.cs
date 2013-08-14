using System.Collections.Generic;
using System.Linq;
using Notifications.Base;
using Notifications.BusiessLogic;

namespace Notifications.DataAccessLayer
{
    public class SqlRepository : IDataRepository
    {
        readonly ContextNotifications _context = new ContextNotifications();


        public List<IEmployee> GetLogin()
        {
            var result = (from item in _context.Employees
                where item.IfLogin
                select new Employee
                {

                }).Cast<IEmployee>().ToList();
            return result;

        }



        public void AddNotification(INotification notification)
        {
            using(_context)
            {
                var sender = _context.Employees.Find(notification.SenderId);
                    
                 
                var sqlNotification = new SqlNotification
                {
                    Sender = sender,
                    Date =  notification.Date,
                    Content = notification.Content,
                };

                _context.Notifications.Add(sqlNotification);
                _context.SaveChanges();

            }
        }

        public void AddMessage(IMessage message)
        {
            using(_context)
            {
                var sender = _context.Employees.Find(message.SenderId);
                var recepient = _context.Employees.Find(message.RecepientId);
                _context.Messages.Add(new SqlMessage
                {
                    Content = message.Content,
                    Date =  message.Date,
                    Receiver = recepient,
                    Sender = sender
                });
                _context.SaveChanges();
            }
        }

        public List<INotification> GetReceiveNotifications(int receiverId)
        {
            using (_context)
            {
               var result = (from item in _context.Employees
                              where item.EmployeeId == receiverId
                              select item.ReceiveNotification).Cast<INotification>().ToList();
                return result;
            }  
        }

        public List<INotification> GetSendNotifications(int senderId)
        {
            using (_context)
            {
                var result = (from item in _context.Notifications
                              where item.SenderId == senderId
                              select new Notification
                              {
                                  
                              }).Cast<INotification>().ToList();
                return result;
            }                 
        }


        public List<IMessage> GetMessages(int employeeId1, int employeeId2)
        {
            using (_context)
            {
                var result = (from item in _context.Messages
                              where ((item.SenderId == employeeId1 && item.ReceiverId == employeeId2) ||( item.SenderId == employeeId2 && item.ReceiverId == employeeId1))
                              select new Message
                              {
                                  
                              }).Cast<IMessage>().ToList();
                return result;
            }  
        }
    }
}