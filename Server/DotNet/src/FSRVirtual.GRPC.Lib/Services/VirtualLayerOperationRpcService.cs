using FSR.DigitalTwin.App.Common.Interfaces;
using FSRVirtual.GRPC.Lib.Operation;
using Grpc.Core;

namespace FSRVirtual.GRPC.Lib.Services;

public class VirtualLayerOperationRpcService : YourService.YourServiceBase {

    private IVirtualizationLayerBridge _virtualizationLayer;

    public VirtualLayerOperationRpcService(IVirtualizationLayerBridge virtualizationLayer) {
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