using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_PocoCommandLineReader
{
	public class ListCollectionFactory : ICreateCollection
	{
		private readonly Type _elementType;

		private IList _collection;

		private int _size;

		private object _defaultValue;

		public ListCollectionFactory( Type elementType )
		{
			_elementType = elementType;
			_defaultValue = _elementType.IsValueType
			   ? Activator.CreateInstance( _elementType )
			   : null;
		}

		public void CreateCollection( int size )
		{
			Type collectionType = typeof( List<> ).MakeGenericType( _elementType );
			_collection = (IList) Activator.CreateInstance( collectionType, size );
			_size = size;
		}

		public void AddElementToCollection( object element )
		{
			if (_collection == null)
				throw new NotSupportedException( "Collection not initialized" );

			if (_collection.Count >= _size)
				throw new IndexOutOfRangeException( $"Tried to add beyond list size of {_size}." );

			_collection.Add( Convert.ChangeType( element, _elementType ) );
		}

		public IEnumerable Collection => _collection;

		public object DefaultValue => _defaultValue;

		public Type ElementType => _elementType;
	}
}
