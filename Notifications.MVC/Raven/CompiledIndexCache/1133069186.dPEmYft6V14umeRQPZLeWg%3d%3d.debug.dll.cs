using Raven.Abstractions;
using Raven.Database.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System;
using Raven.Database.Linq.PrivateExtensions;
using Lucene.Net.Documents;
using System.Globalization;
using System.Text.RegularExpressions;
using Raven.Database.Indexing;


public class Index_Auto_2fRavenNotifications_2fBySender_EmployeeId : Raven.Database.Linq.AbstractViewGenerator
{
	public Index_Auto_2fRavenNotifications_2fBySender_EmployeeId()
	{
		this.ViewText = @"from doc in docs.RavenNotifications
select new { Sender_EmployeeId = doc.Sender.EmployeeId }";
		this.ForEntityNames.Add("RavenNotifications");
		this.AddMapDefinition(docs => 
			from doc in docs
			where string.Equals(doc["@metadata"]["Raven-Entity-Name"], "RavenNotifications", System.StringComparison.InvariantCultureIgnoreCase)
			select new {
				Sender_EmployeeId = doc.Sender.EmployeeId,
				__document_id = doc.__document_id
			});
		this.AddField("Sender_EmployeeId");
		this.AddField("__document_id");
		this.AddQueryParameterForMap("Sender.EmployeeId");
		this.AddQueryParameterForMap("__document_id");
		this.AddQueryParameterForReduce("Sender.EmployeeId");
		this.AddQueryParameterForReduce("__document_id");
	}
}
