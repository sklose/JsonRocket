[![Build status](https://ci.appveyor.com/api/projects/status/krwp2slk49htm6j7?svg=true)](https://ci.appveyor.com/project/sklose/jsonrocket) [![NuGet](https://img.shields.io/nuget/v/JsonRocket.svg)](https://www.nuget.org/packages/JsonRocket) [![NuGet](https://img.shields.io/nuget/dt/JsonRocket.svg)](https://www.nuget.org/packages/JsonRocket)

# JsonRocket

Json Rocket is a fast JSON parser with the goal to extract pieces of information from a JSON message. It is not a fully fledged deserializer that reads JSON into DTO classes.

The goals of this project are:
* ultra fast parsing of a JSON data
* no heap allocations while parsing
* find/extract multiple values from a JSON message with at most one full tokenization

However, non-goals are:
* proper JSON validation: the code takes a lot of shortcuts for faster parsing that can potentially read invalid JSON. It is guaranteed thought that malformed JSON will not cause any Exceptions.

# Installation

```powershell
PM> Install-Package JsonRocket
```

## Tokenization

The following JSON snippet can be tokenized like this

```json
{
  "Id": 1,
  "Name": "Joh Doe",
  "Address": {
    "Street": "1st Avenue",
    "City": "Springfield"
  }
}
```

```csharp
var json = Encoding.UTF8.GetBytes(""); // insert above snippet here
var tokenizer = new JsonRocket.Tokenizer();
tokenizer.Reset(json);
while (tokenizer.MoveNext())
{
  Console.WriteLine(tokenizer.Current);
}
```

the output will be

```
ObjectStart
Key
Integer
Key
String
Key
ObjectStart
Key
String
Key
String
ObjectEnd
ObjectEnd
```

## Value Extraction

The following snippet extracts the Name (```John Doe```) and the City (```Springfield```) from the JSON. This happens with a single iteration through the tokenizer.

```csharp
var trie = new JsonRocket.Trie();
trie.Add("Name");
trie.Add("Address.City");
var extractor = new JsonRocket.Extractor(trie);

var json = Encoding.UTF8.GetBytes(""); // insert above snippet here
var tokenizer = new JsonRocket.Tokenizer();
tokenizer.Reset(json);

var result = new List<JsonRocket.ExtractedValue>();
extractor.ReadFrom(tokenizer, result);
Console.WriteLine(result[0].Value.ReadString()); // prints 'John Doe'
Console.WriteLine(result[1].Value.ReadString()); // prints 'Springfield'
```
