using CSharp_PocoCommandLineReader.SampleUsage.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_PocoCommandLineReader.SampleUsage
{
	public class Program
	{
		public static void Main( string [] args )
		{
			Console.WriteLine( "Available commands: customer-summary, customer-details, quit" );

			while (true)
			{
				Console.Write( "command>" );

				bool done = false;
				string command = Console.ReadLine();

				switch (command)
				{
					case "customer-summary":
						ReadCustomerSummary();
						break;
					case "customer-details":
						ReadCustomerDetails();
						break;

					case "quit":
						done = true;
						break;
				}

				if (done)
					break;
			}

			Console.WriteLine( "Press any key to continue..." );
			Console.ReadKey();
		}

		private static void ReadCustomerSummary()
		{
			PocoCommandLineReader<CustomerSummary> reader =
				new PocoCommandLineReader<CustomerSummary>();

			CustomerSummary obj = reader.Read( "Read customer summary" );
			Confirm( obj );
		}

		private static void Confirm(object obj)
		{
			Console.WriteLine();
			Console.WriteLine( $"READ:{obj}" );
			Console.WriteLine();
		}

		private static void ReadCustomerDetails()
		{
			PocoCommandLineReader<CustomerDetails> reader =
				new PocoCommandLineReader<CustomerDetails>();

			CustomerDetails obj = reader.Read( "Read customer details" );
			Confirm( obj );
		}
	}
}
