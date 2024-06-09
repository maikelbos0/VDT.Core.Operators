# VDT.Core.Operators

Operators that process streams of published values from operand streams, allowing you to subscribe to the output stream for handling any sorts of events in a
streamlined way. Create operand streams of the types you want to process, apply the required operators to those streams and subscribe to the results.
Conceptually this is similar to piping observables, except an operand stream doesn't own the data it publishes - it's merely a conduit that's used for
publishing, piping and subscribing. Below example uses a series of operators to ensure string values don't get published more than once every half a second and
can be parsed as integers before subscribing to the resulting integer stream.

## Features

- Asynchronous subscription to streams of values
- Asynchronous piping of values through a variety of operators to transform them
- Easily extensible with your own operators

## Options

Each operand stream can be provided with an `OperandStreamOptions` object to specify how subscribers are interacted with.

- `ReplayWhenSubscribing` toggles the setting to publish all previously published values to a new subscriber when it is added

## Operators

- `Debounce` delays and throttles output values
- `Filter` discards output values based on a predicate
- `Flatten` subscribes to a stream of streams and outputs values published by the child streams
- `GroupBy` groups published values by using a key selector
- `Iterate` loops over received values of `IEnumerable<T>`
- `Map` transforms values
- `Merge` merges two or more streams
- `QueueThrottle` throttles output, queueing received values
- `QueueZip` outputs tuples of values published by two streams, queueing received values
- `Throttle` throttles output, discarding old received values
- `Zip` outputs tuples of values published by two streams, discarding old received values

## Example

Below example uses a series of operators to ensure string values don't get published more than once every half a second and can be parsed as integers before
subscribing to the resulting integer stream.

```
&lt;input type="text" @oninput="async args => await valueStream.Publish(args.Value!.ToString()!)" /&gt;

@code {
    private readonly IOperandStream&lt;string&gt; valueStream = new OperandStream&lt;string&gt;();

    protected override void OnAfterRender(bool firstRender) {
        if (firstRender) {
            valueStream
                .Debounce(500)
                .Map(value => new { IsValid = int.TryParse(value, out var result), Result = result })
                .Filter(value => value.IsValid)
                .Map(value => value.Result)
                .Subscribe(value => {
                    // Handle received integer values
                });
        }
    }
}
```

## Custom operators

Although many common operators are available out of the box, it is simple to create your own by implementing either
`IOperator&lt;TValue, TTransformedValue&gt;` to transform values without any initialization, or 
`IOperator&lt;TValue, TTransformedValue, TInitializationData&gt;` to add an initialization method with data to the operator. For ease of use, you can then also
create extension methods for `IOperandStream&lt;TValue&lt;` that pipes values through your operator to the target stream.
