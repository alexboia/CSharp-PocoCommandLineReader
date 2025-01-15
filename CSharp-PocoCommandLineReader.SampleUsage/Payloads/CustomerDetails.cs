using System.Collections.Generic;
using System.ComponentModel;

namespace CSharp_PocoCommandLineReader.SampleUsage.Payloads
{
	public class CustomerDetails : CustomerSummary
	{
		[Description( "List of total amount for each of the customer's latest orders" )]
		public decimal [] LastOrderAmounts
		{
			get; set;
		}

		[Description( "Customer keywords" )]
		public List<string> Keywords
		{
			get; set;
		}

		protected override string GetDescription()
		{
			return $"{base.GetDescription()}; " +
				$"LOAMNTS=({string.Join<decimal>(",", LastOrderAmounts ?? new decimal[0])}); " +
				$"KWDS=({string.Join<string>(",", Keywords ?? new List<string>())})";
		}
	}
}
