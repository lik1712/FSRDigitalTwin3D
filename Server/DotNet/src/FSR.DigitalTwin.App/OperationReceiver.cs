using AasCore.Aas3_0;
using AasOperationInvocation;
using AdminShellNS;
using AdminShellNS.Models;
using FSR.DigitalTwin.App.Common.Interfaces;

namespace FSR.DigitalTwin.App;

public class OperationReceiver : IOperationReceiver
{
    private readonly IDigitalTwinLayerBridge _layer;

    public OperationReceiver(IDigitalTwinLayerBridge layer) {
        _layer = layer ?? throw new NullReferenceException();
    }

    public OperationResult OnOperationInvoke(IOperation operation, int? timestamp, string requestId)
    {
        _layer.Operational.InvokeAsync(operation, timestamp, requestId).GetAwaiter().GetResult();
        return _layer.Operational.GetResultAsync(requestId).GetAwaiter().GetResult();
    }

    public async Task<OperationResult> OnOperationInvokeAsync(string handleId, IOperation operation, int? timestamp, string requestId)
    {
        OperationInvoker.UpdateExecutionState(handleId, ExecutionState.InitiatedEnum);
        await _layer.Operational.InvokeAsync(operation, timestamp, requestId, handleId);

        OperationInvoker.UpdateExecutionState(handleId, ExecutionState.RunningEnum);
        var result = await _layer.Operational.GetResultAsync(requestId);
        
        OperationInvoker.UpdateExecutionState(handleId, result.ExecutionState);
        return result;
    }
}