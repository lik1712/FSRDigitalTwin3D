using AdminShellNS.Models;
using FSRAas.GRPC.Lib.V3;
using FSRAas.GRPC.Lib.V3.Services.Operational;

namespace FSR.DigitalTwin.App.Common.Interfaces;

public interface IVirtualizationLayerOperational {
    IAsyncBidirectionalStream<OperationStatus, OperationInvokeRequest> InvokeRequestStream { set; get; }
    IAsyncBidirectionalStream<OperationResult, OperationRequest> ResultRequestStream { set; get; }
    IAsyncBidirectionalStream<OperationStatus, OperationRequest> StatusRequestStream { set; get; }

    Task OnDisconnect();
    bool HasConnection();

    Task<OperationStatus> InvokeAsync(OperationPayloadDTO operation, int? timestamp, string requestId, string? handleId = null);
    Task<OperationResult> GetResultAsync(string requestId);
    Task<OperationStatus> GetExecutionStateAsync(string requestId);
}
