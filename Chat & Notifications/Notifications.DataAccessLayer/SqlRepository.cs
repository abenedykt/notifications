using System;
using System.Collections.Generic;
using System.Linq;
using Notifications.Base;
using Notifications.BusiessLogic;
using Notifications.DataAccessLayer.SqlClass;

namespace Notifications.DataAccessLayer
{
    public class SqlRepository : IDataRepository
    {
        private readonly ContextNotifications _context = new ContextNotifications();

        public string AddNotification(INotification notification)
        {
            SqlEmployee sender = _context.Employees.Find(notification.SenderId);

            var sqlNotification = new SqlNotification
            {
                Sender = sender,
                Date = notification.Date,
                Content = notification.Content,
            };

            _context.Notifications.Add(sqlNotification);
            _context.SaveChanges();

            foreach (
                SqlReceiversOfNotification receiverOfNotification in
                    notification.ReceiversIds.Select(i => _context.Employees.Find(i))
                        .Select(receiver => new SqlReceiversOfNotification
                        {
                            Receiver = receiver,
                            ReceiverId = receiver.EmployeeId,
                            ReceivingNotification = sqlNotification,
                            NotificationId = sqlNotification.NotificationId,
                            WhenRead = DateTime.Now
                        }))
            {
                sqlNotification.Receivers.Add(receiverOfNotification);
                _context.ReceiversOfNotifications.Add(receiverOfNotification);
                _context.SaveChanges();
            }
            return sqlNotification.NotificationId.ToString();
        }

        public void AddMessage(IMessage message)
        {
            SqlEmployee sender = _context.Employees.Find(message.SenderId);
            SqlEmployee recepient = _context.Employees.Find(message.ReceiverId);

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

        public List<INotification> GetReceiveNotifications(string receiverId)
        {
            List<INotification> result = (from item in _context.ReceiversOfNotifications
                join notes in _context.Notifications on item.NotificationId equals notes.NotificationId
                join employees in _context.Employees on notes.SenderId equals employees.EmployeeId
                where item.ReceiverId == receiverId
                select new Notification
                {
                    Content = notes.Content,
                    Date = notes.Date,
                    SenderName = employees.Name
                }).AsEnumerable().Cast<INotification>().ToList();
            return result;
        }

        public List<INotification> GetSendNotifications(string senderId)
        {
            List<INotification> result = (from notes in _context.Notifications
                where notes.SenderId == senderId
                select new Notification
                {
                    NotificationId = notes.NotificationId,
                    Content = notes.Content,
                    Date = notes.Date
                }).AsEnumerable().Cast<INotification>().ToList();

            foreach (INotification notes in result)
            {
                notes.ReceiversNames = GetReceivers(notes.NotificationId);
            }


            return result;
        }

        public List<IMessage> GetMessages(string employeeId1, string employeeId2)
        {
            List<IMessage> result = (from item in _context.Messages
                where
                    ((item.SenderId == employeeId1 && item.ReceiverId == employeeId2) ||
                     (item.SenderId == employeeId2 && item.ReceiverId == employeeId1))
                orderby item.Date
                select new Message
                {
                    Date = item.Date,
                    Content = item.Content,
                    SenderName = item.Sender.Name,
                    ReceiverName = item.Receiver.Name
                }).AsEnumerable().Cast<IMessage>().ToList();

            return result;
        }

        public void AddTimeofReading(string notificationId, string receiverId)
        {


            SqlReceiversOfNotification result = (from item in _context.ReceiversOfNotifications
                                                 where item.NotificationId == notificationId && item.ReceiverId == receiverId
                select item).FirstOrDefault();

            result.WhenRead = DateTime.Now;
            _context.SaveChanges();
        }


        public void AddEmployee(IEmployee employee)
        {
            SqlEmployee result = _context.Employees.Find(employee.EmployeeId);

            if (result == null)
            {
                var newEmployee = new SqlEmployee
                {
                    Name = employee.Name
                };
                _context.Employees.Add(newEmployee);
                _context.SaveChanges();
            }
        }

        public List<string> GetReceivers(string notesId)
        {
            List<string> result = (from item in _context.ReceiversOfNotifications
                where item.NotificationId == notesId
                select item.Receiver.Name).ToList();
            return result;
        }
    }
}