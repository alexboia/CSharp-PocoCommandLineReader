# CSharp-PocoCommandLineReader

Automate reading simple POCOs from command line. And by simple I mean *really* simple:

- either built-in types;
- or collection of scalar values.

![Sample usage](/screenshots/Sample.gif?raw=true)

It is more of a fun project, a lame kata, if you will, to practice the basics, but it has proved useful in some manual testing scenarios.
I do hope it will help others as well.

## Support

Supported built-in types:

- `int`
- `uint`
- `short`
- `ushort`
- `long`
- `ulong`
- `string`
- `bool`
- `float`
- `double`
- `decimal`
- `char`
- `byte`
- `sbyte`
- `System.Guid`.

Supported collection types (typed colllections, type can be any of the above):
- typed enumerables: `IEnumerable<T>` which is populated as a `List<T>`;
- lists - `IList<T>` and `List<T>`;
- typed arrays - `T[]`.

## Rules

- public instance properties are searched for, inheritance is observed;
- must have a public parameterless constructor;
- most of the values are parsed with the available `TryParse` methods for that respective type, with the following exceptions:
	- for a `Guid`, if entering `new()`, a new guid will be generated, otherwise has to be manually inserted as everything else;
	- for `bool`, the following values are also supported:
		- truthy values: `yes`, `da`, `1`;
		- falsy values: `no`, `nu`, `0`.
- default values for regular properties are whatever they are when the object gets created;
- default value types for the element of a collection is:
	- `Activator.CreateInstance( elementType )` if it's a value type;
	- `null` if not.
- collection must have sizes specified up-front;
- one may decorate properties with `System.ComponentModel.DescriptionAttribute` to customize the prompts, although one *does not have to*.

## Bells and whistles

I used this [nifty little class](https://github.com/raminrahimzada/cConsole) by @raminrahimzada to provide colored console output when rendering the prompts.

## How to use

No library to install. Just copy the source files wherever you need and then do something like this:

```csharp
PocoCommandLineReader<CustomerSummary> reader = new PocoCommandLineReader<CustomerSummary>();
CustomerSummary obj = reader.Read( "Read customer summary" );
```

Also see samples ([`CSharp-PocoCommandLineReader.SampleUsage`](https://github.com/alexboia/CSharp-PocoCommandLineReader/blob/main/CSharp-PocoCommandLineReader.SampleUsage/Program.cs)).

## What's next

Nothing: unless I find something's wrong with it or that I want some improvements, I don't plan on developing on a regular basis. Feel free to fork and use as you like.