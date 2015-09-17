
# DotWrapper


## Description

DotWrapper is an open source library that turns your program into a data wrapper. It takes the information from any data source (file, web address, raw data) and wraps it onto a file on the disk.


## Features

### Resolve chain

A Wrap is the object representation of a wrapper. It contains a set of chunks that are characterized by their raw data and a chain of resolvers that can pack/unpack the data as needed.

The default resolve chain is as follows :

```csharp
new CompressionResolver(new CryptoResolver(data));
```

Which means that :
- While packing, the data will first be encrypted then compressed
- While unpacking, the data will be decompressed then decrypted

### Data resolvers

The resolvers can be linked arbitrarily amoung the following :

- **CryptoResolver** : Encrypts or decrypts the data.

This resolver has the following properties :

| Property |      Generated     |       Editable     |       Packed       |
|:-------- |:------------------:|:------------------:|:------------------:|
| Password | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Salt     | :heavy_minus_sign: | :heavy_check_mark: | :heavy_minus_sign: |
| IV       | :heavy_check_mark: | :heavy_minus_sign: | :heavy_check_mark: |
| Key      | :heavy_check_mark: | :heavy_minus_sign: | :heavy_minus_sign: |

The cryptographic algorithm will also be packed and can be chosen from these (defaults to AES) :

| Algorithm      | KeySize | BlockSize |
|:-------------- |:-------:|:---------:|
| Rijndael (AES) |   256   |    128    |
| DES            |   256   |    64     |
| TripleDES      |  3*64   |    64     |
| RC2            |   128   |    64     |

- **CompressionResolver** : The data will be compressed or decompressed using gzip.

- **NetResolver** : The data will be fetched on internet at the given address. This resolver has, obviously, no way of editing back the source.

- **Base64Resolver** : Will convert a data to/from Base64.

- **FileResolver** : Gets or sets the data from a file path.

### More obfuscation : bytewise transforms

Additionally, all resolvers packed fields and properties will undergo bytewise involution transforms (such as `T(T(b)) = b`) to prevent any data from being read directly on the file.

The transform is defined at Chunk-level and is not packed with the data. There are currently four available : Xor, EvenXor, OddXor and Reverse.


## Getting Started

There are two examples attached to the solution to help you getting started.

Otherwise, here are a few code samples :

### Creating a Wrap

```csharp
// Create an empty Wrap.
Wrap wrap = new Wrap();
// Add a Chunk named "mychunk" with string data.
byte[] data = Encoding.Default.GetBytes("DotWrapping!");
wrap.Chunks.Add(new Chunk("mychunk", data));
// Output wrap to file "SuperWrap".
wrap.Write("SuperWrap");
```

### Reading a Wrap

```csharp
// Read a Wrap from file "SuperWrap".
Wrap wrap = Wrap.Read("SuperWrap");
// Gets a Chunk's data.
byte[] data = wrap.GetChunk("mychunk").Data;
// Output data to the console.
Console.WriteLine(Encoding.Default.GetString(data));
```

### Custom resolver (NetResolver)

```csharp
var url = "https://raw.githubusercontent.com/lelimacon/DotWrapper/master/DotWrapper.Test/TestFiles/myBase64message.txt";
// To unpack, the data will be fetched on the internet then decoded from base64.
var chunk = new Chunk("mychunk", new NetResolver(url, new Base64Resolver()));
// Create wrap and save it to a file named "SuperWrap".
new Wrap(null, new List<Chunk> { chunk }).Write("SuperWrap");
// Read wrap and extract data from chunk "mychunk".
var data = Wrap.Read("SuperWrap").GetChunk("mychunk").Data;
// Output data to the console.
Console.WriteLine(Encoding.Default.GetString(data));
```

### A tiny file wrapper!

```csharp
private static void Main(string[] args)
{
	// If one argument, consider it a path and add the chunk.
	if (args.Length == 1)
	{
		Wrap wrap = Wrap.This();
		wrap.Chunks.Add(new Chunk(args[0], File.ReadAllBytes(args[0])));
		string exePath = Assembly.GetExecutingAssembly().Location;
		wrap.Write(exePath.Remove(exePath.Length - 4) + "-new.exe");
		return;
	}
	// Display this wrap's chunks information.
	var chunks = Wrap.This().Chunks;
	Console.WriteLine("This wrap contains {0} chunks(s).", chunks.Count);
	foreach (Chunk chunk in chunks)
		Console.WriteLine("----\n{0}\n----\n{1}",
			chunk.Name, Encoding.ASCII.GetString(chunk.Data));
}
```


## Dependencies

All assemblies require :
- .NET framework 4.5

DotWrapper
- [NUnit](http://www.nunit.org/) (MIT License)

MyFileWrapper
- [ILRepack](https://github.com/gluck/il-repack) (Apache License)
- [ILRepack.MSBuild.Task](https://github.com/peters/ILRepack.MSBuild.Task)


## Example Projects

### MyPrivateNotepad

MyPrivateNotepad is a WPF application that provides the user with a crypted notepad. All notes are opened in new tabs and require a password.
The data is saved in a file in the same directory (named data.mpn).

### MyFileWrapper

A console application that provides the user with a portable file wrapper.
That is, the executable can wrap a file, remove it, or dump the data to a given destination and execute the file.
Most of the operations require the executable to duplicate itself to another file.

**Usage** : wrapper.exe COMMANDS OPTIONS

| Command    | Options                       |
|:---------- |:----------------------------- |
| **wrap**   | [-n NAME] [-o PATH] [-E] PATH |
| **remove** | [-n NAME] [-o PATH]           |
| **rename** | [-o PATH] OLDNAME NEWNAME     |
| **dump**   | [-n NAME] [-E -o PATH | -L]   |
| **info**   | [-n NAME]                     |

| Option | Description |
|:------ |:----------- |
| **-n** | Specify a name. If not specified, the file name will be choosen |
| **-o** | Specify output folder or path whith -n. If not specified, a default file name will be set |
| **-E** | Execute executable content or mark data as executable |
| **-L** | Dump to console |

Note : The majority of the commands will run on all files when -n is not specified.

## License

DotWrapper is licensed under the [MIT license](https://github.com/lelimacon/DotWrapper/blob/master/LICENSE.txt).
