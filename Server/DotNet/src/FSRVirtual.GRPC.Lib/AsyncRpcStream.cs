using AutoMapper;
using FSR.DigitalTwin.App.Common.Interfaces;

namespace FSRVirtual.GRPC.Lib;

public class AsyncRpcStreamWriter<T, MappedT> : IAsyncStreamWriter<T>
{
    private readonly Grpc.Core.IServerStreamWriter<MappedT> _streamWriter;
    private readonly IMapper _mapper;

    public AsyncRpcStreamWriter(IMapper mapper, Grpc.Core.IServerStreamWriter<MappedT> streamWriter) {
        _mapper = mapper ?? throw new NotImplementedException();
        _streamWriter = streamWriter;
    }

    public Task WriteAsync(T message) => _streamWriter.WriteAsync(_mapper.Map<MappedT>(message));
}

public class AsyncRpcStreamReader<T, MappedT> : IAsyncStreamReader<T>
{
    private readonly Grpc.Core.IAsyncStreamReader<MappedT> _streamReader;
    private readonly IMapper _mapper;

    public AsyncRpcStreamReader(IMapper mapper, Grpc.Core.IAsyncStreamReader<MappedT> streamReader) {
        _mapper = mapper ?? throw new NotImplementedException();
        _streamReader = streamReader;
    }

    public T Current => _mapper.Map<T>(_streamReader.Current);

    public Task<bool> MoveNext(CancellationToken cancellationToken) => _streamReader.MoveNext(cancellationToken);
}

public class AsyncBidirectionRpcStream<InT, InMappedT, OutT, OutMappedT> : IAsyncBidirectionalStream<InT, OutT>
{
    public AsyncBidirectionRpcStream(IMapper mapper, Grpc.Core.IAsyncStreamReader<InMappedT> streamReader, Grpc.Core.IServerStreamWriter<OutMappedT> streamWriter) {
        Incoming = new AsyncRpcStreamReader<InT, InMappedT>(mapper, streamReader);
        Outgoing = new AsyncRpcStreamWriter<OutT, OutMappedT>(mapper, streamWriter);
    }

    public IAsyncStreamReader<InT> Incoming { get; init; }
    public IAsyncStreamWriter<OutT> Outgoing { get; init; }
}