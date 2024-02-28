using FSRAas.GRPC.Lib.V3;
using FSRAas.GRPC.Lib.V3.Services;
using FSRAas.GRPC.Lib.V3.Services.Operational;
using Grpc.Core;

namespace FSR.DigitalTwin.App.Common.Interfaces;

public interface IVirtualizationLayerOutgoing {
    IAsyncStreamWriter<OperationInvokeRequest>? InvokeRequests { set; get; }
    IAsyncStreamWriter<OperationRequest>? ResultRequests { set; get; }
    IAsyncStreamWriter<OperationRequest>? StatusRequests { set; get; }
}

public interface IVirtualizationLayerIncoming {
    IAsyncStreamReader<OperationStatus>? InvokeResponses { set; get; }
    IAsyncStreamReader<OperationResult>? ResultResponses { set; get; }
    IAsyncStreamReader<OperationStatus>? StatusRequestStream { set; get; }
}

public interface IVirtualizationLayerBridge {
    IVirtualizationLayerOutgoing Outgoing { get; init; }
    IVirtualizationLayerIncoming Incoming { get; init; }
    bool IsConnected { set; get; }

    Task<OperationStatus> InvokeOperationAsync(OperationPayloadDTO operation, int? timestamp, string requestId, string? handleId = null);
    Task<OperationResult> GetOperationResultAsync(string requestId);
    Task<OperationStatus> GetExecutionStateAsync(string requestId);

}