using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace CSharp_PocoCommandLineReader
{
	public class PocoCommandLineReader<T> where T : class, new()
	{
		private readonly Dictionary<PropertyInfo, string> _readableProperties =
		   new Dictionary<PropertyInfo, string>();

		private readonly Dictionary<PropertyInfo, ICreateCollection> _collectionProperties =
		   new Dictionary<PropertyInfo, ICreateCollection>();

		private static readonly Dictionary<Type, Func<string, object, object>> _parsers =
		   new Dictionary<Type, Func<string, object, object>>();

		static PocoCommandLineReader()
		{
			_parsers.Add( typeof( int ), AsInteger );
			_parsers.Add( typeof( uint ), AsUnsignedInteger );

			_parsers.Add( typeof( short ), AsShort );
			_parsers.Add( typeof( ushort ), AsUnsignedShort );

			_parsers.Add( typeof( long ), AsLong );
			_parsers.Add( typeof( ulong ), AsUnsignedLong );

			_parsers.Add( typeof( string ), AsString );
			_parsers.Add( typeof( bool ), AsBoolean );

			_parsers.Add( typeof( float ), AsFloat );
			_parsers.Add( typeof( double ), AsDouble );
			_parsers.Add( typeof( decimal ), AsDecimal );

			_parsers.Add( typeof( char ), AsChar );
			_parsers.Add( typeof( byte ), AsByte );
			_parsers.Add( typeof( sbyte ), AsSignedByte );

			_parsers.Add( typeof( Guid ), AsGuid );
		}

		public PocoCommandLineReader()
		{
			PropertyInfo [] properties = typeof( T )
			   .GetProperties( BindingFlags.Public | BindingFlags.Instance )
			   .ToArray() ?? new PropertyInfo [ 0 ];

			foreach (PropertyInfo property in properties)
			{
				if (!IsBindableProperty( property, out ICreateCollection collectionFactory ))
					continue;

				DescriptionAttribute descriptionAttribute = property
				   .GetCustomAttribute<DescriptionAttribute>();

				string description = descriptionAttribute?.Description;
				if (string.IsNullOrWhiteSpace( description ))
					description = property.Name;

				_readableProperties.Add( property,
				   description );

				if (collectionFactory != null)
					_collectionProperties.Add( property, collectionFactory );
			}
		}

		public T Read( string globalPrompt )
		{
			T obj = new T();

			if (!string.IsNullOrEmpty( globalPrompt ))
			{
				Console.WriteLine();
				ColoredConsole.WriteLine( $"{globalPrompt:blue}" );
			}
			else
				Console.WriteLine();

			foreach (KeyValuePair<PropertyInfo, string> bindPropsInfo in _readableProperties)
			{
				object readValue;
				PropertyInfo property = bindPropsInfo.Key;
				string propertyName = bindPropsInfo.Value;

				if (!_collectionProperties.TryGetValue( property, out ICreateCollection collectionFactory ))
				{
					object defaultValue = property
					   .GetValue( obj, null );

					readValue = ReadValue( propertyName,
					   property.PropertyType,
					   defaultValue,
					   indentCount: 1 );
				}
				else
					readValue = ReadCollection( propertyName,
					   collectionFactory );

				property.SetValue( obj, readValue, null );
			}

			return obj;
		}

		private object ReadValue( string valueName, Type ofType, object defaultValue, int indentCount )
		{
			string defaultValueDesc = defaultValue != null
			   ? defaultValue.ToString()
			   : string.Empty;

			string indent = new string( '\t', indentCount );
			ColoredConsole.Write( $"{indent}Enter {valueName:darkgray} [{defaultValueDesc:darkgray}]> " );

			string readLine = Console.ReadLine();
			Func<string, object, object> parser = _parsers [ ofType ];

			object readValue = parser.Invoke( readLine, defaultValue );
			return readValue;
		}

		private object ReadCollection( string valueName, ICreateCollection collectionFactory )
		{
			ColoredConsole.WriteLine( $"\tEnter {valueName:darkgray}> " );

			ColoredConsole.Write( $"\t\tEnter number of elements [{"0":darkgray}]: " );
			int size = (int) AsInteger( Console.ReadLine(), 0 );

			collectionFactory.CreateCollection( size );
			for (int i = 0; i < size; i++)
			{
				object readValue = ReadValue( $"{valueName}#{i}",
				   collectionFactory.ElementType,
				   collectionFactory.DefaultValue,
				   indentCount: 2 );

				collectionFactory.AddElementToCollection( readValue );
			}

			return collectionFactory.Collection;
		}

		private bool IsBindableProperty( PropertyInfo property, out ICreateCollection collectionFactory )
		{
			Type propertyType = property.PropertyType;
			collectionFactory = null;

			return property.CanRead
			   && property.CanWrite
			   && (
				  _parsers.ContainsKey( propertyType ) ||
				  IsCollectionOfKnownType( propertyType, out collectionFactory )
			   );
		}

		private bool IsCollectionOfKnownType( Type propertyType, out ICreateCollection collectionFactory )
		{
			if (propertyType.IsArray)
			{
				Type elementType = propertyType.GetElementType();
				if (elementType != null && _parsers.ContainsKey( elementType ))
				{
					collectionFactory = new ArrayCollectionFactory( elementType );
					return true;
				}
			}

			if (propertyType.IsGenericType)
			{
				Type genericType = propertyType
				   .GetGenericTypeDefinition();

				Type elementType = propertyType
				   .GetGenericArguments()
				   .FirstOrDefault();

				if (genericType == typeof( IEnumerable<> )
				   || genericType == typeof( IList<> )
				   || genericType == typeof( List<> ))
				{
					if (elementType != null && _parsers.ContainsKey( elementType ))
					{
						collectionFactory = new ListCollectionFactory( elementType );
						return true;
					}
				}
			}

			collectionFactory = null;
			return false;
		}

		private static object AsInteger( string readLine, object defaultValue )
		{
			if (!int.TryParse( readLine, out int value ))
				value = (int) defaultValue;

			return value;
		}

		private static object AsUnsignedInteger( string readLine, object defaultValue )
		{
			if (!uint.TryParse( readLine, out uint value ))
				value = (uint) defaultValue;

			return value;
		}

		private static object AsShort( string readLine, object defaultValue )
		{
			if (!short.TryParse( readLine, out short value ))
				value = (short) defaultValue;

			return value;
		}

		private static object AsUnsignedShort( string readLine, object defaultValue )
		{
			if (!ushort.TryParse( readLine, out ushort value ))
				value = (ushort) defaultValue;

			return value;
		}

		private static object AsLong( string readLine, object defaultValue )
		{
			if (!long.TryParse( readLine, out long value ))
				value = (long) defaultValue;

			return value;
		}

		private static object AsUnsignedLong( string readLine, object defaultValue )
		{
			if (!ulong.TryParse( readLine, out ulong value ))
				value = (ulong) defaultValue;

			return value;
		}

		private static object AsBoolean( string readLine, object defaultValue )
		{
			if (!bool.TryParse( readLine, out bool value ))
			{
				switch (readLine?.ToLower())
				{
					case "yes":
					case "da"://RO
					case "1":
						value = true;
						break;
					case "no":
					case "nu"://RO
					case "0":
						value = false;
						break;
					default:
						value = (bool) defaultValue;
						break;
				}
			}

			return value;
		}

		public static object AsGuid( string readLine, object defaultValue )
		{
			if ("new()".Equals( readLine?.ToLower() ))
				return Guid.NewGuid();

			if (!Guid.TryParse( readLine, out Guid value ))
				value = (Guid) defaultValue;

			return value;
		}

		private static object AsString( string readLine, object defaultValue )
		{
			if (string.IsNullOrEmpty( readLine ))
				return (string) defaultValue;
			return readLine;
		}

		private static object AsByte( string readLine, object defaultValue )
		{
			if (!byte.TryParse( readLine, out byte value ))
				value = (byte) defaultValue;

			return value;
		}

		private static object AsSignedByte( string readLine, object defaultValue )
		{
			if (!sbyte.TryParse( readLine, out sbyte value ))
				value = (sbyte) defaultValue;

			return value;
		}

		private static object AsChar( string readLine, object defaultValue )
		{
			if (string.IsNullOrEmpty( readLine ))
				return (char) defaultValue;

			return readLine [ 0 ];
		}

		private static object AsFloat( string readLine, object defaultValue )
		{
			if (!float.TryParse( readLine, out float value ))
				value = (float) defaultValue;

			return value;
		}

		private static object AsDouble( string readLine, object defaultValue )
		{
			if (!double.TryParse( readLine, out double value ))
				value = (double) defaultValue;

			return value;
		}

		private static object AsDecimal( string readLine, object defaultValue )
		{
			if (!decimal.TryParse( readLine, out decimal value ))
				value = (decimal) defaultValue;

			return value;
		}
	}
}
