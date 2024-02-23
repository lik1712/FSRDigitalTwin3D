using System.Reactive.Linq;
using System.Reactive.Subjects;
using FSR.DigitalTwin.App.Common.Interfaces;
using FSRAas.GRPC.Lib.V3;
using FSRAas.GRPC.Lib.V3.Services;
using FSRAas.GRPC.Lib.V3.Services.Operational;

namespace FSRVirtual.GRPC.Lib;

public class VirtualLayerRpcOutgoing : IVirtualizationLayerOutgoing
{
    public IObservable<OperationInvokeRequest> OperationInvokeRequests { get => _operationInvokeRequests; }
    public IObservable<OperationRequest> OperationResultRequests { get => _operationResultRequests; }
    public IObservable<OperationRequest> ExecutionStateRequests { get => _executionStateRequests; }

    private readonly IVirtualizationLayerBridge _virtualLayer;
    private readonly Subject<OperationInvokeRequest> _operationInvokeRequests;
    private readonly Subject<OperationRequest> _operationResultRequests;
    private readonly Subject<OperationRequest> _executionStateRequests;

    public VirtualLayerRpcOutgoing(IVirtualizationLayerBridge virtualLayer) {
        _virtualLayer = virtualLayer;
        _operationInvokeRequests = new();
        _operationResultRequests = new();
        _executionStateRequests = new();
    }

    public async Task<ExecutionState> GetExecutionStateAsync(string requestId)
    {
        OperationRequest request = new() { RequestId = requestId };
        var stateAsync = _virtualLayer.Incoming.ExecutionStates.FirstAsync().PublishLast();
        stateAsync.Connect();
        _executionStateRequests.OnNext(request);
        return await stateAsync;
    }

    public async Task<OperationResult> GetOperationResultAsync(string requestId)
    {
        OperationRequest request = new() { RequestId = requestId };
        var resultAsync = _virtualLayer.Incoming.OperationResults.FirstAsync().PublishLast();
        resultAsync.Connect();
        _operationResultRequests.OnNext(request);
        return await resultAsync;
    }

    public async Task<ExecutionState> InvokeOperationAsync(OperationPayloadDTO operation, int? timestamp, string requestId, string? handleId = null)
    {
        OperationInvokeRequest request = new() {
            RequestId = requestId,
            Timestamp = timestamp ?? int.MaxValue,
            IsAsync = handleId != null,
            HandleId = handleId ?? ""
        };
        request.InputVariables.AddRange(operation.InputVariables);
        request.InoutVariables.AddRange(operation.InoutVariables);

        var stateAsync = _virtualLayer.Incoming.ExecutionStates.FirstAsync().PublishLast();
        stateAsync.Connect();
        _operationInvokeRequests.OnNext(request);
        return await stateAsync; // This await blocks due to obvious reasons!
    }
}

public class VirtualLayerRpcIncoming : IVirtualizationLayerIncoming
{
    private IVirtualizationLayerBridge _virtualLayer;
    private readonly Subject<OperationResult> _operationResults;
    private readonly Subject<ExecutionState> _executionStates;

    public IObservable<OperationResult> OperationResults { get => _operationResults; }
    public IObservable<ExecutionState> ExecutionStates { get => _executionStates; }

    public VirtualLayerRpcIncoming(IVirtualizationLayerBridge virtualLayer) {
        _virtualLayer = virtualLayer;
        _operationResults = new();
        _executionStates = new();
    }

    public void OnExecutionStateReceived(ExecutionState state)
    {
        _executionStates.OnNext(state);
    }

    public void OnOperationResultReceived(OperationResult result)
    {
        _operationResults.OnNext(result);
    }
}

public class VirtualLayerRpcBridge : IVirtualizationLayerBridge
{
    public IVirtualizationLayerOutgoing Outgoing { get; init; }
    public IVirtualizationLayerIncoming Incoming { get; init; }

    public VirtualLayerRpcBridge() {
        Outgoing = new VirtualLayerRpcOutgoing(this);
        Incoming = new VirtualLayerRpcIncoming(this);
    }

}