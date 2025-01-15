using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_PocoCommandLineReader
{
	public interface ICreateCollection
	{
		void CreateCollection( int size );

		void AddElementToCollection( object element );

		IEnumerable Collection
		{
			get;
		}

		object DefaultValue
		{
			get;
		}

		Type ElementType
		{
			get;
		}
	}
}
