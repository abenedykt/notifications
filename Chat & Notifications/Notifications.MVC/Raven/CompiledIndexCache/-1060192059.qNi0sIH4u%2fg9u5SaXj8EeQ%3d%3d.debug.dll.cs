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


public class Index_Auto_2fSqlEmployees_2fByEmployeeId : Raven.Database.Linq.AbstractViewGenerator
{
	public Index_Auto_2fSqlEmployees_2fByEmployeeId()
	{
		this.ViewText = @"from doc in docs.SqlEmployees
select new { EmployeeId = doc.EmployeeId }";
		this.ForEntityNames.Add("SqlEmployees");
		this.AddMapDefinition(docs => 
			from doc in docs
			where string.Equals(doc["@metadata"]["Raven-Entity-Name"], "SqlEmployees", System.StringComparison.InvariantCultureIgnoreCase)
			select new {
				EmployeeId = doc.EmployeeId,
				__document_id = doc.__document_id
			});
		this.AddField("EmployeeId");
		this.AddField("__document_id");
		this.AddQueryParameterForMap("EmployeeId");
		this.AddQueryParameterForMap("__document_id");
		this.AddQueryParameterForReduce("EmployeeId");
		this.AddQueryParameterForReduce("__document_id");
	}
}
