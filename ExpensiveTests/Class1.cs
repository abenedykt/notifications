using System.Linq;
using Notifications.DataAccessLayer;
using Notifications.DataAccessLayer.RavenClass;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Linq;
using Xunit;

namespace ExpensiveTests
{
    public class Class1
    {
        private DocumentStore documentStore;

        private IDocumentSession _session;

        [Fact]
        public void ExpensiveTest()
        {
            documentStore = new DocumentStore
            {
                Url = "http://localhost:8080"
            };

            documentStore.Initialize();

            _session = documentStore.OpenSession();
            var result = _session.Query<RavenNotification>()
                        .Where(x => x.Sender.EmployeeId == 404)
                        .ToList();
            //var result = (from item in _session.Query<RavenNotification>()
            //              where item.Sender.EmployeeId == 404
            //              select item).ToList();
            var items = result.Count;

        }
    }
}
