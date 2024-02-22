using AasCore.Aas3_0;
using FSR.DigitalTwin.App.Common.Interfaces;
using IO.Swagger.Models;

namespace FSRVirtual.GRPC.Lib;

public class VirtualLayerRpcBridge : IVirtualizationLayerBridge
{
    public Task<ExecutionState> GetExecutionStateAsync(string handleId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult> GetOperationResultAsync(string requestId)
    {
        throw new NotImplementedException();
    }

    public Task<ExecutionState> InvokeOperationAsync(IOperation operation, int? timestamp, string requestId, string? handleId = null)
    {
        throw new NotImplementedException();
    }
}