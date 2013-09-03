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


public class Index_Auto_2fSqlNotifications_2fBySenderId : Raven.Database.Linq.AbstractViewGenerator
{
	public Index_Auto_2fSqlNotifications_2fBySenderId()
	{
		this.ViewText = @"from doc in docs.SqlNotifications
select new { SenderId = doc.SenderId }";
		this.ForEntityNames.Add("SqlNotifications");
		this.AddMapDefinition(docs => 
			from doc in docs
			where string.Equals(doc["@metadata"]["Raven-Entity-Name"], "SqlNotifications", System.StringComparison.InvariantCultureIgnoreCase)
			select new {
				SenderId = doc.SenderId,
				__document_id = doc.__document_id
			});
		this.AddField("SenderId");
		this.AddField("__document_id");
		this.AddQueryParameterForMap("SenderId");
		this.AddQueryParameterForMap("__document_id");
		this.AddQueryParameterForReduce("SenderId");
		this.AddQueryParameterForReduce("__document_id");
	}
}
