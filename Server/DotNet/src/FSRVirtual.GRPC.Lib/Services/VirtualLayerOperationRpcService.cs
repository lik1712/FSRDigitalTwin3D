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

    public override async Task OpenChannel(IAsyncStreamReader<ServerRequest> requestStream, IServerStreamWriter<ServerResponse> responseStream, ServerCallContext context)
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

    public override async Task OpenOperationInvocationStream(IAsyncStreamReader<OperationStatus> requestStream, IServerStreamWriter<OperationInvokeRequest> responseStream, ServerCallContext context)
    {
        Console.WriteLine("[Server]: Opened invoke channel to client...");
        var subscription = _virtualizationLayer.Outgoing.OperationInvokeRequests.Subscribe(async (request) => {
            await responseStream.WriteAsync(request);
        });
        await foreach (var response in requestStream.ReadAllAsync())
        {
            _virtualizationLayer.Incoming.OnExecutionStateReceived(response.ExecutionState);
        }
        subscription.Dispose();
    }

    public override async Task OpenOperationResultStream(IAsyncStreamReader<OperationResult> requestStream, IServerStreamWriter<OperationRequest> responseStream, ServerCallContext context)
    {
        Console.WriteLine("[Server]: Opened result channel to client...");
        var subscription = _virtualizationLayer.Outgoing.OperationResultRequests.Subscribe(async (request) => {
            await responseStream.WriteAsync(request);
        });
        await foreach (var response in requestStream.ReadAllAsync())
        {
            _virtualizationLayer.Incoming.OnOperationResultReceived(response);
        }
        subscription.Dispose();
    }

    public override async Task OpenExecutionStateStream(IAsyncStreamReader<OperationStatus> requestStream, IServerStreamWriter<OperationRequest> responseStream, ServerCallContext context)
    {
        Console.WriteLine("[Server]: Opened operation execution status channel to client...");
        var subscription = _virtualizationLayer.Outgoing.ExecutionStateRequests.Subscribe(async (request) => {
            await responseStream.WriteAsync(request);
        });
        await foreach (var response in requestStream.ReadAllAsync())
        {
            _virtualizationLayer.Incoming.OnExecutionStateReceived(response.ExecutionState);
        }
        subscription.Dispose();
    }
}