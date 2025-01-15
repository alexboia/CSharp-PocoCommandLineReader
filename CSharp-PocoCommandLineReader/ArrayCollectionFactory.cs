using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_PocoCommandLineReader
{
	public class ArrayCollectionFactory : ICreateCollection
	{
		private readonly Type _elementType;

		private Array _collection;

		private int _currentIndex = 0;

		private int _size = 0;

		private object _defaultValue;

		public ArrayCollectionFactory( Type elementType )
		{
			_elementType = elementType;
			_defaultValue = _elementType.IsValueType
			   ? Activator.CreateInstance( _elementType )
			   : null;
		}

		public void CreateCollection( int size )
		{
			_collection = Array.CreateInstance( _elementType, size );
			_currentIndex = 0;
			_size = size;
		}

		public void AddElementToCollection( object element )
		{
			if (_collection == null)
				throw new NotSupportedException( "Collection not initialized" );

			if (_currentIndex >= _size)
				throw new IndexOutOfRangeException( $"Tried to add beyond array size of {_size}." );

			_collection.SetValue( Convert.ChangeType( element, _elementType ), _currentIndex++ );
		}

		public IEnumerable Collection => _collection;

		public object DefaultValue => _defaultValue;

		public Type ElementType => _elementType;
	}
}
