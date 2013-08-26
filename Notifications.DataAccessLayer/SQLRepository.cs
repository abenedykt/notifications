using System.Collections.Generic;
using System.Linq;
using Notifications.Base;
using Notifications.BusiessLogic;
using System;
using System.Diagnostics;
using System.Data.Entity;


namespace Notifications.DataAccessLayer
{
    public class SqlRepository : IDataRepository
    {
        ContextNotifications _context = new ContextNotifications();


        public void AddNotification(INotification notification)
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

                foreach (var i in notification.ReceiversIds)
                {
                    var receiver = _context.Employees.Find(i);

                    SqlReceiversOfNotification receiverOfNotification = new SqlReceiversOfNotification
                    {
                        Receiver = receiver,
                        ReceivingNotification = sqlNotification
                    };

                    sqlNotification.Receivers.Add(receiverOfNotification);
                    _context.ReceiversOfNotifications.Add(receiverOfNotification);
                    _context.SaveChanges();
                }


            
        }

        public void AddMessage(IMessage message)
        {

            var sender = _context.Employees.Find(message.SenderId);
            var recepient = _context.Employees.Find(message.ReceiverId);

            var sqlMessage = new SqlMessage
            {
                Content = message.Content,
                Date = message.Date,
                Receiver = recepient,
                Sender = sender
            };

            _context.Messages.Add(sqlMessage);
            _context.SaveChanges();            
        }

        public List<INotification> GetReceiveNotifications(int receiverId)
        {
            var result = (from item in _context.Employees
                          join item2 in _context.ReceiversOfNotifications on item.EmployeeId equals item2.ReceiverId
                          where item.EmployeeId == receiverId
                          select new Notification
                          {
                          }).Cast<INotification>().ToList();
            return result;
            
        }

        public List<INotification> GetSendNotifications(int senderId)
        {
            var result = (from item in _context.Notifications
                          where item.SenderId == senderId
                          select new Notification
                          {
                          }).Cast<INotification>().ToList();
            return result;

        }

        public List<IMessage> GetMessages(int employeeId1, int employeeId2)
        {

            var result = (from item in _context.Messages
                          where ((item.SenderId == employeeId1 && item.ReceiverId == employeeId2) || (item.SenderId == employeeId2 && item.ReceiverId == employeeId1))
                          select new Message
                          {                         
                          }).Cast<IMessage>().ToList();
            return result;

        }
    }
}