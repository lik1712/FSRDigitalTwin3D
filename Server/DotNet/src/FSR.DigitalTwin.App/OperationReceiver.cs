using AasCore.Aas3_0;
using AasOperationInvocation;
using AdminShellNS;
using AdminShellNS.Models;
using FSR.DigitalTwin.App.Common.Interfaces;

namespace FSR.DigitalTwin.App;

public class OperationReceiver : IOperationReceiver
{
    private readonly IVirtualizationLayerBridge _virtualLayer;

    public OperationReceiver(IVirtualizationLayerBridge virtualLayer) {
        _virtualLayer = virtualLayer ?? throw new NullReferenceException();
    }

    public OperationResult OnOperationInvoke(IOperation operation, int? timestamp, string requestId)
    {
        _virtualLayer.Operational.InvokeAsync(operation, timestamp, requestId).GetAwaiter().GetResult();
        return _virtualLayer.Operational.GetResultAsync(requestId).GetAwaiter().GetResult();
    }

    public async Task<OperationResult> OnOperationInvokeAsync(string handleId, IOperation operation, int? timestamp, string requestId)
    {
        OperationInvoker.UpdateExecutionState(handleId, ExecutionState.InitiatedEnum);
        await _virtualLayer.Operational.InvokeAsync(operation, timestamp, requestId, handleId);

        OperationInvoker.UpdateExecutionState(handleId, ExecutionState.RunningEnum);
        var result = await _virtualLayer.Operational.GetResultAsync(requestId);
        
        OperationInvoker.UpdateExecutionState(handleId, result.ExecutionState);
        return result;
    }
}