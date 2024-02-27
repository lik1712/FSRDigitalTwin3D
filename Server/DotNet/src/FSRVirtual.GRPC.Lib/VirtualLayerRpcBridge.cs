using FSR.DigitalTwin.App.Common.Interfaces;
using FSRAas.GRPC.Lib.V3;
using FSRAas.GRPC.Lib.V3.Services;
using FSRAas.GRPC.Lib.V3.Services.Operational;
using Grpc.Core;

namespace FSRVirtual.GRPC.Lib;

public class VirtualLayerRpcOutgoing : IVirtualizationLayerOutgoing
{
    public IAsyncStreamWriter<OperationInvokeRequest>? InvokeRequests { get; set; }
    public IAsyncStreamWriter<OperationRequest>? ResultRequests { get; set; }
    public IAsyncStreamWriter<OperationRequest>? StatusRequests { get; set; }
    public bool IsConnected { get; set; } = false;
}

public class VirtualLayerRpcIncoming : IVirtualizationLayerIncoming
{
    public IAsyncStreamReader<OperationStatus>? InvokeResponses { get; set; }
    public IAsyncStreamReader<OperationResult>? ResultResponses { get; set; }
    public IAsyncStreamReader<OperationStatus>? StatusRequestStream { get; set; }
}

public class VirtualLayerRpcBridge : IVirtualizationLayerBridge
{
    public IVirtualizationLayerOutgoing Outgoing { get; init; }
    public IVirtualizationLayerIncoming Incoming { get; init; }

    public VirtualLayerRpcBridge() {
        Outgoing = new VirtualLayerRpcOutgoing();
        Incoming = new VirtualLayerRpcIncoming();
    }

    public Task<OperationStatus> GetExecutionStateAsync(string requestId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult> GetOperationResultAsync(string requestId)
    {
        throw new NotImplementedException();
    }

    public async Task<OperationStatus> InvokeOperationAsync(OperationPayloadDTO operation, int? timestamp, string requestId, string? handleId = null)
    {
        OperationInvokeRequest request = new() {
            RequestId = requestId,
            Timestamp = timestamp ?? int.MaxValue,
            IsAsync = handleId != null,
            HandleId = handleId ?? ""
        };
        request.InputVariables.AddRange(operation.InputVariables);
        request.InoutVariables.AddRange(operation.InoutVariables);

        if (Outgoing.InvokeRequests == null || Incoming.InvokeResponses == null) {
            throw new NullReferenceException();
        }
        await Outgoing.InvokeRequests.WriteAsync(request);
        await Incoming.InvokeResponses.MoveNext();
        return Incoming.InvokeResponses.Current;

        // return new OperationStatus { ExecutionState = ExecutionState.Failed, RequestId = requestId };
    }
}