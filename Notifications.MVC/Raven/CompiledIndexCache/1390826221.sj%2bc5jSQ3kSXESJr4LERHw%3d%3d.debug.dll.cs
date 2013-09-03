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


public class Index_Auto_2fRavenMessages_2fByDateAndReceiver_EmployeeIdAndSender_EmployeeIdSortByDate : Raven.Database.Linq.AbstractViewGenerator
{
	public Index_Auto_2fRavenMessages_2fByDateAndReceiver_EmployeeIdAndSender_EmployeeIdSortByDate()
	{
		this.ViewText = @"from doc in docs.RavenMessages
select new { Receiver_EmployeeId = doc.Receiver.EmployeeId, Sender_EmployeeId = doc.Sender.EmployeeId, Date = doc.Date }";
		this.ForEntityNames.Add("RavenMessages");
		this.AddMapDefinition(docs => 
			from doc in docs
			where string.Equals(doc["@metadata"]["Raven-Entity-Name"], "RavenMessages", System.StringComparison.InvariantCultureIgnoreCase)
			select new {
				Receiver_EmployeeId = doc.Receiver.EmployeeId,
				Sender_EmployeeId = doc.Sender.EmployeeId,
				Date = doc.Date,
				__document_id = doc.__document_id
			});
		this.AddField("Receiver_EmployeeId");
		this.AddField("Sender_EmployeeId");
		this.AddField("Date");
		this.AddField("__document_id");
		this.AddQueryParameterForMap("Receiver.EmployeeId");
		this.AddQueryParameterForMap("Sender.EmployeeId");
		this.AddQueryParameterForMap("Date");
		this.AddQueryParameterForMap("__document_id");
		this.AddQueryParameterForReduce("Receiver.EmployeeId");
		this.AddQueryParameterForReduce("Sender.EmployeeId");
		this.AddQueryParameterForReduce("Date");
		this.AddQueryParameterForReduce("__document_id");
	}
}
