using AasCore.Aas3_0;
using AasOperationInvocation;
using AdminShellNS;
using AdminShellNS.Models;
using AutoMapper;
using FSR.DigitalTwin.App.Common.Interfaces;
using FSRAas.GRPC.Lib.V3;

namespace FSR.DigitalTwin.App;

public class OperationReceiver : IOperationReceiver
{
    private readonly IVirtualizationLayerBridge _virtualLayer;
    private IMapper _mapper;

    public OperationReceiver(IVirtualizationLayerBridge virtualLayer, IMapper mapper) {
        _virtualLayer = virtualLayer ?? throw new NullReferenceException();
        _mapper = mapper ?? throw new NullReferenceException();
    }

    public OperationResult OnOperationInvoke(IOperation operation, int? timestamp, string requestId)
    {
        var sme = _mapper.Map<SubmodelElementDTO>(operation);
        _virtualLayer.Operational.InvokeAsync(sme.Operation, timestamp, requestId).GetAwaiter().GetResult();
        var result = _virtualLayer.Operational.GetResultAsync(requestId).GetAwaiter().GetResult();
        return _mapper.Map<OperationResult>(result);
        // return new OperationResult() { RequestId = requestId, ExecutionState = ExecutionState.FailedEnum, Message = "Test Default", Success = false };
    }

    public async Task<OperationResult> OnOperationInvokeAsync(string handleId, IOperation operation, int? timestamp, string requestId)
    {
        var sme = _mapper.Map<SubmodelElementDTO>(operation);

        OperationInvoker.UpdateExecutionState(handleId, ExecutionState.InitiatedEnum);
        await _virtualLayer.Operational.InvokeAsync(sme.Operation, timestamp, requestId, handleId);

        OperationInvoker.UpdateExecutionState(handleId, ExecutionState.RunningEnum);
        var result = await _virtualLayer.Operational.GetResultAsync(requestId);
        
        var res = _mapper.Map<OperationResult>(result);
        OperationInvoker.UpdateExecutionState(handleId, res.ExecutionState);
        return res;
        // return new OperationResult() { RequestId = requestId, ExecutionState = ExecutionState.FailedEnum, Message = "Test Default", Success = false };
    }
}