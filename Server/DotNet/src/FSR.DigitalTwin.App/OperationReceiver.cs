using AasCore.Aas3_0;
using AdminShellNS;
using AdminShellNS.Models;
using AutoMapper;
using FSR.DigitalTwin.App.Common.Interfaces;
using FSRAas.GRPC.Lib.V3;

namespace FSR.DigitalTwin.App;

public class OperationReceiver : IOperationReceiver
{
    private IVirtualizationLayerBridge _virtualLayer;
    private IMapper _mapper;

    public OperationReceiver(IVirtualizationLayerBridge virtualLayer, IMapper mapper) {
        _virtualLayer = virtualLayer ?? throw new NullReferenceException();
        _mapper = mapper ?? throw new NullReferenceException();
    }

    public OperationResult OnOperationInvoke(IOperation operation, int? timestamp, string requestId)
    {
        var sme = _mapper.Map<SubmodelElementDTO>(operation);
        _virtualLayer.Outgoing.InvokeOperationAsync(sme.Operation, timestamp, requestId).GetAwaiter().GetResult();
        return new OperationResult() { Success = false };
    }

    public async Task<OperationResult> OnOperationInvokeAsync(string handleId, IOperation operation, int? timestamp, string requestId)
    {
        var sme = _mapper.Map<SubmodelElementDTO>(operation);
        await _virtualLayer.Outgoing.InvokeOperationAsync(sme.Operation, timestamp, requestId, handleId);
        return new OperationResult() { Success = false };
    }
}