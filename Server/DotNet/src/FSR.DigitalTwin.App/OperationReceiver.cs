using AasCore.Aas3_0;
using AasOperationInvocation;
using AdminShellNS;
using AdminShellNS.Models;
using FSR.DigitalTwin.App.Interfaces;

namespace FSR.DigitalTwin.App;

public class OperationReceiver : IOperationReceiver
{
    private readonly IDigitalTwinLayerService _layerService;

    public OperationReceiver(IDigitalTwinLayerService layerService) {
        _layerService = layerService ?? throw new NullReferenceException();
    }

    public OperationResult OnOperationInvoke(IOperation operation, int? timestamp, string requestId)
    {
        _layerService.Layer.Operational.InvokeAsync(operation, timestamp, requestId).GetAwaiter().GetResult();
        return _layerService.Layer.Operational.GetResultAsync(requestId).GetAwaiter().GetResult();
    }

    public async Task<OperationResult> OnOperationInvokeAsync(string handleId, IOperation operation, int? timestamp, string requestId)
    {
        OperationInvoker.UpdateExecutionState(handleId, ExecutionState.InitiatedEnum);
        await _layerService.Layer.Operational.InvokeAsync(operation, timestamp, requestId, handleId);

        OperationInvoker.UpdateExecutionState(handleId, ExecutionState.RunningEnum);
        var result = await _layerService.Layer.Operational.GetResultAsync(requestId);
        
        OperationInvoker.UpdateExecutionState(handleId, result.ExecutionState);
        return result;
    }
}