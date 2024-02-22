using AasCore.Aas3_0;
using IO.Swagger.Models;

namespace FSR.DigitalTwin.App.Common.Interfaces;

public interface IVirtualizationLayerBridge {

    Task<ExecutionState> InvokeOperationAsync(IOperation operation, int? timestamp, string requestId, string? handleId = null);
    Task<OperationResult> GetOperationResultAsync(string requestId);
    Task<ExecutionState> GetExecutionStateAsync(string handleId);

}