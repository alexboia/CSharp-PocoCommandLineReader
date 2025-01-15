using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_PocoCommandLineReader
{
	//Courtesy of: https://github.com/raminrahimzada/cConsole
	//Renamed and tweaked a bit for my environment.
	//Many thanks!
	public class ColoredConsole : IFormatProvider, ICustomFormatter
	{
		private const char COLOR_SEPARATOR = ':';

		private const char SEPARATOR = (char) 65535;

		private static readonly Type _customFormatterType;

		private static readonly Dictionary<string, ConsoleColor> _colors;

		private static readonly ColoredConsole _instance = new ColoredConsole();

		static ColoredConsole()
		{
			_customFormatterType = typeof( ICustomFormatter );
			_colors = new Dictionary<string, ConsoleColor>();

			foreach (object val in Enum.GetValues( typeof( ConsoleColor ) ))
			{
				_colors.Add( val.ToString().ToLowerInvariant(), (ConsoleColor) val );
			}
		}

		private ColoredConsole()
		{
			return;
		}

		public object GetFormat( Type formatType )
		{
			return _customFormatterType == formatType ? this : null;
		}

		private static ConsoleColor? GetColor( string color )
		{
			if (string.IsNullOrWhiteSpace( color ))
				return null;
			if (_colors.TryGetValue( color.Trim().ToLowerInvariant(), out var consoleColor ))
			{
				return consoleColor;
			}

			throw new Exception( $"System.ConsoleColor enum does not have a member named {color}" );
		}

		private static (ConsoleColor? foreground, ConsoleColor? background)? GetColors( string colors )
		{
			if (string.IsNullOrWhiteSpace( colors ))
				return null;
			if (colors.Contains( COLOR_SEPARATOR ))
			{
				var split = colors.Split( COLOR_SEPARATOR );
				var foreground = GetColor( split [ 0 ] );
				var background = GetColor( split [ 1 ] );
				return (foreground, background);
			}

			var single = GetColor( colors );
			return (single, null);
		}

		public string Format( string format, object arg, IFormatProvider formatProvider )
		{
			if (format == null)
				return arg?.ToString();
			var sb = new StringBuilder();
			sb.Append( SEPARATOR );
			sb.Append( format );
			sb.Append( SEPARATOR );
			sb.Append( arg );
			sb.Append( SEPARATOR );
			return sb.ToString();
		}

		public static void WriteLine( FormattableString f )
		{
			Write( f );
			Console.WriteLine();
		}

		public static void Write( FormattableString f )
		{
			lock (_instance)
			{
				WriteInternal( f );
			}
		}

		private static void WriteInternal( FormattableString f )
		{
			var str = f.ToString( _instance );
			var sb = new StringBuilder();
			var format = new StringBuilder();
			var arg = new StringBuilder();
			bool? state = null;//null->normal,true->separator start,2->separator end

			var defaultForegroundColor = Console.ForegroundColor;
			var defaultBackgroundColor = Console.BackgroundColor;

			void Print( ConsoleColor? foreground, ConsoleColor? background, string text )
			{
				Console.ForegroundColor = foreground ?? defaultForegroundColor;
				Console.BackgroundColor = background ?? defaultBackgroundColor;
				Console.Write( text );
			}

			foreach (var ch in str)
			{
				switch (state)
				{
					case null when ch == SEPARATOR:
						Print( null, null, sb.ToString() );
						sb.Clear();
						state = true;
						break;
					case null:
						sb.Append( ch );
						break;
					case true when ch == SEPARATOR:
						state = false;
						break;
					case true:
						format.Append( ch );
						break;
					case false when ch == SEPARATOR:
						state = null;
						var colors = GetColors( format.ToString() );
						Print( colors?.foreground, colors?.background, arg.ToString() );
						format.Clear();
						arg.Clear();
						break;
					case false:
						arg.Append( ch );
						break;
				}
			}

			if (format.Length > 0 && arg.Length > 0)
			{
				var colors = GetColors( format.ToString() );
				Print( colors?.foreground, colors?.background, arg.ToString() );
				format.Clear();
			}

			if (sb.Length > 0)
			{
				Print( null, null, sb.ToString() );
			}

			Console.ForegroundColor = defaultForegroundColor;
			Console.BackgroundColor = defaultBackgroundColor;
		}
	}
}
