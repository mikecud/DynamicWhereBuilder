# DynamicWhereBuilder [![Build Status](https://travis-ci.com/mikecud/DynamicWhereBuilder.svg?branch=master)](https://travis-ci.com/mikecud/DynamicWhereBuilder)

DynamicWhereBuilder is a free library that let's you build complex LINQ where queries using special sentences called "QueryParts". First you create the list of sentences, than you put them as a parameter to .Where() method.

Works with any IEnumerable and supports popular ORMs like Entity Framework.

## Installation

DynamicWhereBuilder is available on NuGet. Just search for "DynamicWhereBuilder" in NuGet Package Manager or use command:

```
Install-Package DynamicWhereBuilder
```

You can also use `dotnet` CLI:

```
dotnet add package DynamicWhereBuilder
```

## Usage

First things first, you are going to need this usings:

```csharp
using DynamicWhereBuilder;
using DynamicWhereBuilder.Models.QueryPart;
```

Now let's prepare some class to test it!

```csharp
public class ExampleClass
{
    public ExampleClass(int id, string value)
    {
        this.Id = id;
        this.Value = value;
    }

    public int Id { get; set; }
    public string Value { get; set; }
}
```

And make a List using it:

```csharp
var list = new List<ExampleClass>()
{
    new ExampleClass(1, "valid"),
    new ExampleClass(1, "invalid"),
    new ExampleClass(2, "valid"),
    new ExampleClass(2, "invalid"),
};
```

So, how do you create queries using DynamicWhereBuilder? Let's say we would like achieve the same results as these two queries:

```csharp
var result1Linq = list.Where(x => x.Id == 1);
var result2Linq = list.Where(x => x.Id == 1 || (x.Id == 2 && x.Value == "valid"));
```

We should create two lists of QueryParts, use them as .Where() param and test whether result is the same when we used traditional LINQ.

```csharp
var result1QueryParts = new List<QueryPart<ExampleClass>>(); // x =>
result1QueryParts.Add(new QueryPart<ExampleClass>(null, x => x.Id == 1, null, null)); // x.Id == 1

var result2QueryParts = new List<QueryPart<ExampleClass>>(); // x =>
result2QueryParts.Add(new QueryPart<ExampleClass>(null, x => x.Id == 1, null, LogicalOperator.Or)); // x.Id == 1 ||
result2QueryParts.Add(new QueryPart<ExampleClass>(Parenthesis.Open, x => x.Id == 2, null, LogicalOperator.And)); // (x.Id == 2 &&
result2QueryParts.Add(new QueryPart<ExampleClass>(null, x => x.Value == "valid", Parenthesis.Close, null)); // x.Value == "valid")

var result1DynamicWhereBuilder = list.Where(result1QueryParts);
var result2DynamicWhereBuilder = list.Where(result2QueryParts);

Assert.Equal(result1Linq, result1DynamicWhereBuilder); // success!
Assert.Equal(result2Linq, result2DynamicWhereBuilder); // success!
```

As you can see QueryPart takes 4 parameters:

- `initialParenthesis` - provide `Parenthesis.Open` for `"("` or `Parenthesis.Close` for `")"` or null if you want to skip it
- `expression` - LINQ query or null to skip it
- `closingParenthesis` - provide `Parenthesis.Open` for `"("` or `Parenthesis.Close` for `")"` or null to skip it
- `logicalOperator` - provide `LogicalOperator.And` for `"&&"` or `LogicalOperator.Or` for `"||"` or null to skip it

Basically DynamicWhereBuilder takes all QueryParts in order, then it takes arguments you have provided in then (in order) and tries to build the query. 
That's it. You got everything you need. Now it's up to you how to use it.

## License

Dual licensed under ["The Unlicense"](LICENSE.md) and ["MIT"](LICENSE.MIT.md) .

"The Unlicense" is recommended for most cases, because it let's you use DynamicWhereBuilder with no restrictions under no conditions. If "The Unlicense" is for some reason not suitable for you, take it under MIT license. 

In any case, feel free to use this piece of software for any purposes (including commercial use).
