using FSR.DigitalTwin.App.Common.Interfaces;

namespace FSRVirtual.GRPC.Lib;

public class AsyncRpcStreamWriter<T> : IAsyncStreamWriter<T>
{
    private Grpc.Core.IServerStreamWriter<T> _streamWriter;

    public AsyncRpcStreamWriter(Grpc.Core.IServerStreamWriter<T> streamWriter) {
        _streamWriter = streamWriter;
    }

    public Task WriteAsync(T message) => _streamWriter.WriteAsync(message);
}

public class AsyncRpcStreamReader<T> : IAsyncStreamReader<T>
{
    private Grpc.Core.IAsyncStreamReader<T> _streamReader;

    public AsyncRpcStreamReader(Grpc.Core.IAsyncStreamReader<T> streamReader) {
        _streamReader = streamReader;
    }

    public T Current => _streamReader.Current;

    public Task<bool> MoveNext(CancellationToken cancellationToken) => _streamReader.MoveNext(cancellationToken);
}

public class AsyncBidirectionRpcStream<IncomingT, OutgoingT> : IAsyncBidirectionalStream<IncomingT, OutgoingT>
{
    public IAsyncStreamReader<IncomingT> Incoming { get; init; }
    public IAsyncStreamWriter<OutgoingT> Outgoing { get; init; }

    public AsyncBidirectionRpcStream(Grpc.Core.IAsyncStreamReader<IncomingT> streamReader, Grpc.Core.IServerStreamWriter<OutgoingT> streamWriter) {
        Incoming = new AsyncRpcStreamReader<IncomingT>(streamReader);
        Outgoing = new AsyncRpcStreamWriter<OutgoingT>(streamWriter);
    }
}