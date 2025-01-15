using System.ComponentModel;

namespace CSharp_PocoCommandLineReader.SampleUsage.Payloads
{
	public class CustomerSummary
	{
		[Description( "First name" )]
		public string FirstName
		{
			get; set;
		}

		[Description( "Last name" )]
		public string LastName
		{
			get; set;
		}

		[Description( "Total order count" )]
		public int OrderCount
		{
			get; set;
		}

		[Description( "Total order amount" )]
		public decimal TotalOrderAmount
		{
			get; set;
		}

		[Description( "Latitude" )]
		public double Latitude
		{
			get; set;
		}

		[Description( "Longitude" )]
		public double Longitude
		{
			get; set;
		}

		[Description( "Locked out" )]
		public bool IsLockedOut
		{
			get; set;
		}

		protected virtual string GetDescription()
		{
			return $"LN={LastName}; " +
				$"FN={FirstName}; " +
				$"OCNT={OrderCount}; " +
				$"TAMNT={TotalOrderAmount}; " +
				$"LAT={Latitude}; " +
				$"LNG={Longitude}; " +
				$"LKD={IsLockedOut}";
		}

		public override string ToString()
		{
			return $"[{GetDescription()}]";
		}
	}
}
