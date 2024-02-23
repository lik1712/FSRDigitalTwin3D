using FSRAas.GRPC.Lib.V3;
using FSRAas.GRPC.Lib.V3.Services;
using FSRAas.GRPC.Lib.V3.Services.Operational;

namespace FSR.DigitalTwin.App.Common.Interfaces;

public interface IVirtualizationLayerOutgoing {
    IObservable<OperationInvokeRequest> OperationInvokeRequests { get; }
    IObservable<OperationRequest> OperationResultRequests { get; }
    IObservable<OperationRequest> ExecutionStateRequests { get; }

    Task<ExecutionState> InvokeOperationAsync(OperationPayloadDTO operation, int? timestamp, string requestId, string? handleId = null);
    Task<OperationResult> GetOperationResultAsync(string requestId);
    Task<ExecutionState> GetExecutionStateAsync(string requestId);
}

public interface IVirtualizationLayerIncoming {
    IObservable<OperationResult> OperationResults { get; }
    IObservable<ExecutionState> ExecutionStates { get; }

    void OnOperationResultReceived(OperationResult result);
    void OnExecutionStateReceived(ExecutionState state);
}

public interface IVirtualizationLayerBridge {
    IVirtualizationLayerOutgoing Outgoing { get; init; }
    IVirtualizationLayerIncoming Incoming { get; init; }

}