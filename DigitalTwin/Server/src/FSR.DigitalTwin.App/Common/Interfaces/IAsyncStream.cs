namespace FSR.DigitalTwin.App.Common.Interfaces;

public interface IAsyncStreamWriter<in T>
{
    //
    // Summary:
    //     Writes a message asynchronously. Only one write can be pending at a time.
    //
    // Parameters:
    //   message:
    //     The message to be written. Cannot be null.
    Task WriteAsync(T message);

    //
    // Summary:
    //     Writes a message asynchronously. Only one write can be pending at a time.
    //
    // Parameters:
    //   message:
    //     The message to be written. Cannot be null.
    //
    //   cancellationToken:
    //     Cancellation token that can be used to cancel the operation.
    Task WriteAsync(T message, CancellationToken cancellationToken)
    {
        if (cancellationToken.CanBeCanceled)
        {
            throw new NotSupportedException("Cancellation of stream writes is not supported.");
        }

        return WriteAsync(message);
    }
}

public interface IAsyncStreamReader<out T>
{
    //
    // Summary:
    //     Gets the current element in the iteration.
    T Current { get; }

    //
    // Summary:
    //     Advances the reader to the next element in the sequence, returning the result
    //     asynchronously.
    //
    // Parameters:
    //   cancellationToken:
    //     Cancellation token that can be used to cancel the operation.
    //
    // Returns:
    //     Task containing the result of the operation: true if the reader was successfully
    //     advanced to the next element; false if the reader has passed the end of the sequence.
    Task<bool> MoveNext(CancellationToken cancellationToken);
}

public interface IAsyncBidirectionalStream<IncomingT, OutgoingT>
{
    IAsyncStreamReader<IncomingT> Incoming { get; init; }
    IAsyncStreamWriter<OutgoingT> Outgoing { get; init; }

    IncomingT Current { get => Incoming.Current; }
    Task<bool> MoveNext(CancellationToken cancellationToken = default) => Incoming.MoveNext(cancellationToken);
    Task WriteAsync(OutgoingT message) => Outgoing.WriteAsync(message);
    Task WriteAsync(OutgoingT message, CancellationToken cancellationToken) => Outgoing.WriteAsync(message, cancellationToken);
}