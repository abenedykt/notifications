using Notifications.Base;
using System;
using System.Collections.Generic;

namespace Notifications.BusiessLogic
{
    public class Application : IApplication
    {
        private IFactory _factory;

        public Application(IFactory factory)
        {
            _factory = factory;
        }

        public void BrodcastNotification(string content, int senderId, List<int> receiversIds, DateTime date)
        {
            INotification notification = new Notification(){ 
                Content= content, 
                Date= date, 
                SenderId= senderId,
                ReceiversIds = receiversIds
            };
           
            _factory.AddNotification(notification);

        }

        public void SendMessage(string content, int senderId, int receiverId, DateTime date)
        {
            IMessage message = new Message()
            {
                Content = content,
                SenderId = senderId,
                ReceiverId = receiverId,
                Date = date
            };

            _factory.AddMessage(message);

        }


        public string[,] GetReceiveNotifications(int employeeId)
        {
            List<INotification> noteList = _factory.GetReceiveNotifications(employeeId);
            string[,] notifications= new string[noteList.Count,3];
            int i=0;

            foreach (var note in noteList)
            {
                notifications[i, 0] = note.Date.ToLongTimeString() + ", " + note.Date.ToLongDateString();
                notifications[i, 1] = note.SenderName;
                notifications[i, 2] = note.Content; 
                i++;
            }

            return notifications;
        }

        public string[,] GetSendNotifications(int employeeId)
        {
            List<INotification> noteList = _factory.GetSendNotifications(employeeId);

            string[,] notifications = new string[noteList.Count, 3];
            int i = 0;

            foreach (var note in noteList)
            {
                string receivers= "";
                notifications[i, 0] = note.Date.ToLongTimeString() + ", " + note.Date.ToLongDateString();

                if(note.ReceiversNames.Count>0)
                    receivers = note.ReceiversNames[0];
                for (int j = 1; j < note.ReceiversNames.Count; j++)
                    receivers += ", " + note.ReceiversNames[j];

                notifications[i, 1] = receivers;

                notifications[i, 2] = note.Content;
                i++;
            }

            return notifications;
        }

        public List<IMessage> GetMessages(int employeeId1, int employeeId2)
        {
            return _factory.GetMessages(employeeId1, employeeId2);
        }




    }
}