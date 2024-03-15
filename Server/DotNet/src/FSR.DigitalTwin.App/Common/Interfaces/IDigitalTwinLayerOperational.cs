using AasCore.Aas3_0;
using AdminShellNS.Models;

namespace FSR.DigitalTwin.App.Common.Interfaces;

public interface IDigitalTwinLayerOperational {
    IAsyncBidirectionalStream<ExecutionState, OperationInvocation> InvokeRequestStream { set; get; }
    IAsyncBidirectionalStream<OperationResult, string> ResultRequestStream { set; get; }
    IAsyncBidirectionalStream<ExecutionState, string> StatusRequestStream { set; get; }

    Task<ExecutionState> InvokeAsync(IOperation operation, int? timestamp, string requestId, string? handleId = null);
    Task<OperationResult> GetResultAsync(string requestId);
    Task<ExecutionState> GetExecutionStateAsync(string requestId);
}
