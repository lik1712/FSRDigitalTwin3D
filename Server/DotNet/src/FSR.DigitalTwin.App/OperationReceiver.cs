using AasCore.Aas3_0;
using AdminShellNS;
using AdminShellNS.Models;
using FSR.DigitalTwin.App.Common.Interfaces;

namespace FSR.DigitalTwin.App;

public class OperationReceiver : IOperationReceiver
{
    private IVirtualizationLayerBridge _virtualLayer;

    public OperationReceiver(IVirtualizationLayerBridge virtualLayer) {
        _virtualLayer = virtualLayer ?? throw new NullReferenceException();
    }

    public OperationResult OnOperationInvoke(IOperation operation, int? timestamp, string requestId)
    {
        _virtualLayer.InvokeOperationAsync(operation, timestamp, requestId).GetAwaiter().GetResult();
        return new OperationResult() { Success = false };
    }

    public async Task<OperationResult> OnOperationInvokeAsync(string handleId, IOperation operation, int? timestamp, string requestId)
    {
        await _virtualLayer.InvokeOperationAsync(operation, timestamp, requestId, handleId);
        return new OperationResult() { Success = false };
    }
}