using FSR.DigitalTwin.App.Common.Interfaces;
using FSRAas.GRPC.Lib.V3.Services;
using FSRAas.GRPC.Lib.V3.Services.Operational;
using Grpc.Core;

namespace FSRVirtual.GRPC.Lib.Services;

public class YourAwesomeService : YourService.YourServiceBase {

    private IVirtualizationLayerBridge _virtualizationLayer;

    public YourAwesomeService(IVirtualizationLayerBridge virtualizationLayer) {
        _virtualizationLayer = virtualizationLayer ?? throw new NullReferenceException();
    }

    public override async Task OpenChannel(Grpc.Core.IAsyncStreamReader<ServerRequest> requestStream, IServerStreamWriter<ServerResponse> responseStream, ServerCallContext context)
    {
        ServerResponse response = new () { Message = "Hello World" };
        await responseStream.WriteAsync(response);

        await foreach (var request in requestStream.ReadAllAsync())
        {
            Console.WriteLine("[Message from client]: " + request.Message);
        }
        Console.WriteLine("Closing channel to client!");
    }
}

public class VirtualLayerOperationRpcService : VirtualLayerOperationService.VirtualLayerOperationServiceBase {
    private IVirtualizationLayerBridge _virtualizationLayer;

    public VirtualLayerOperationRpcService(IVirtualizationLayerBridge virtualizationLayer) {
        _virtualizationLayer = virtualizationLayer ?? throw new NullReferenceException();
    }

    public override async Task OpenOperationInvocationStream(Grpc.Core.IAsyncStreamReader<OperationStatus> requestStream, IServerStreamWriter<OperationInvokeRequest> responseStream, ServerCallContext context)
    {
        Console.WriteLine("[Server]: Opened invoke channel to client...");
        var stream = new AsyncBidirectionRpcStream<OperationStatus, OperationInvokeRequest>(requestStream, responseStream);
        _virtualizationLayer.Operational.InvokeRequestStream = stream;

        while (_virtualizationLayer.IsConnected) {
            await Task.Delay(100);
        }

        Console.WriteLine("[Server]: Done!");
    }

    public override async Task OpenOperationResultStream(Grpc.Core.IAsyncStreamReader<OperationResult> requestStream, IServerStreamWriter<OperationRequest> responseStream, ServerCallContext context)
    {
        Console.WriteLine("[Server]: Opened result channel to client...");
    }

    public override async Task OpenExecutionStateStream(Grpc.Core.IAsyncStreamReader<OperationStatus> requestStream, IServerStreamWriter<OperationRequest> responseStream, ServerCallContext context)
    {
        Console.WriteLine("[Server]: Opened operation execution status channel to client...");
    }

    public override Task<CloseResponse> CloseStreamsAndDisconnect(CloseRequest request, ServerCallContext context)
    {
        Console.WriteLine("[Server]: Closing connection to client...");
        _virtualizationLayer.Disconnect();
        return Task.FromResult(new CloseResponse());
    }
}