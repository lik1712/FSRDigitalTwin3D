using AutoMapper;
using FSR.Aas.GRPC.Lib.V3.Services;
using FSR.Aas.GRPC.Lib.V3.Services.Operational;
using Grpc.Core;
using FSR.DigitalTwin.App.Interfaces;

namespace FSR.DigitalTwinLayer.GRPC.Lib.Services;

public class DigitalTwinLayerOperationalRpcService : DigitalTwinLayerOperationalService.DigitalTwinLayerOperationalServiceBase {
    private readonly IDigitalTwinLayerService _layerService;
    private readonly IMapper _mapper;

    public DigitalTwinLayerOperationalRpcService(IMapper mapper, IDigitalTwinLayerService layerService) {
        _mapper = mapper ?? throw new NullReferenceException();
        _layerService = layerService ?? throw new NullReferenceException();
        if (!_layerService.HasLayer(_layerService.Mode)) { // TODO Remove later!
            _layerService.AddLayer<DigitalTwinLayerRpcBridge>(_layerService.Mode);
        }
    }

    public override async Task OpenOperationInvocationStream(Grpc.Core.IAsyncStreamReader<OperationStatus> requestStream, IServerStreamWriter<OperationInvokeRequest> responseStream, ServerCallContext context)
    {
        Console.WriteLine("[Server]: Opened invoke channel to client...");
        var stream = new AsyncBidirectionRpcStream<AdminShellNS.Models.ExecutionState, OperationStatus, AdminShellNS.Models.OperationInvocation, OperationInvokeRequest>(_mapper, requestStream, responseStream);
        _layerService.Layer.Operational.InvokeRequestStream = stream;

        while (_layerService.HasLayer(_layerService.Mode)) {
            await Task.Delay(100);
        }

        Console.WriteLine("[Server]: Done!");
    }

    public override async Task OpenOperationResultStream(Grpc.Core.IAsyncStreamReader<OperationResult> requestStream, IServerStreamWriter<OperationRequest> responseStream, ServerCallContext context)
    {
        Console.WriteLine("[Server]: Opened result channel to client...");

        var stream = new AsyncBidirectionRpcStream<AdminShellNS.Models.OperationResult, OperationResult, string, OperationRequest>(_mapper, requestStream, responseStream);
        _layerService.Layer.Operational.ResultRequestStream = stream;

        while (_layerService.HasLayer(_layerService.Mode)) {
            await Task.Delay(100);
        }

        Console.WriteLine("[Server]: Done!");
    }

    public override async Task OpenExecutionStateStream(Grpc.Core.IAsyncStreamReader<OperationStatus> requestStream, IServerStreamWriter<OperationRequest> responseStream, ServerCallContext context)
    {
        Console.WriteLine("[Server]: Opened operation execution status channel to client...");

        var stream = new AsyncBidirectionRpcStream<AdminShellNS.Models.ExecutionState, OperationStatus, string, OperationRequest>(_mapper, requestStream, responseStream);
        _layerService.Layer.Operational.StatusRequestStream = stream;

        while (_layerService.HasLayer(_layerService.Mode)) {
            await Task.Delay(100);
        }

        Console.WriteLine("[Server]: Done!");
    }

    public override Task<CloseResponse> CloseStreamsAndDisconnect(CloseRequest request, ServerCallContext context)
    {
        Console.WriteLine("[Server]: Closing connection to client...");
        _layerService.RemoveLayer(_layerService.Mode);
        return Task.FromResult(new CloseResponse());
    }
}