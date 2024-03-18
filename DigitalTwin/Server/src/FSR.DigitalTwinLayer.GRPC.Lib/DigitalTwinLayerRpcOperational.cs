using AasCore.Aas3_0;
using AdminShellNS.Models;
using FSR.DigitalTwin.App.Common.Interfaces;
using Grpc.Core;

namespace FSR.DigitalTwinLayer.GRPC.Lib;

public class DigitalTwinLayerRpcOperational : IDigitalTwinLayerOperational
{
    private IAsyncBidirectionalStream<ExecutionState, OperationInvocation>? _invokeRequestStream;
    private IAsyncBidirectionalStream<OperationResult, string>? _resultRequestStream;
    private IAsyncBidirectionalStream<ExecutionState, string>? _statusRequestStream;

    public IAsyncBidirectionalStream<ExecutionState, OperationInvocation> InvokeRequestStream { 
        get => _invokeRequestStream ?? throw new RpcException(Status.DefaultCancelled, "No connection");
        set => _invokeRequestStream ??= value;
    }
    public IAsyncBidirectionalStream<OperationResult, string> ResultRequestStream { 
        get => _resultRequestStream ?? throw new RpcException(Status.DefaultCancelled, "No connection");
        set => _resultRequestStream ??= value;
    }
    public IAsyncBidirectionalStream<ExecutionState, string> StatusRequestStream { 
        get => _statusRequestStream ?? throw new RpcException(Status.DefaultCancelled, "No connection");
        set => _statusRequestStream ??= value;
    }

    public async Task<ExecutionState> GetExecutionStateAsync(string requestId)
    {
        if (_statusRequestStream == null) {
            throw new RpcException(Status.DefaultCancelled, "No connection");
        }
        try {
            await _statusRequestStream.WriteAsync(requestId);
            await _statusRequestStream.MoveNext();
            return _statusRequestStream.Current;
        }
        catch (ObjectDisposedException) {
            return ExecutionState.FailedEnum;
        }
    }

    public async Task<OperationResult> GetResultAsync(string requestId)
    {
        if (_resultRequestStream == null) {
            throw new RpcException(Status.DefaultCancelled, "No connection");
        }

        OperationResult result;
        try {
            await _resultRequestStream.WriteAsync(requestId);
            await _resultRequestStream.MoveNext();
            result = _resultRequestStream.Current;
        }
        catch (ObjectDisposedException) {
            result = new OperationResult() { 
                Success = false,
                Message = "Lost connection"
            };
        }
        return result;
    }

    public async Task<ExecutionState> InvokeAsync(IOperation operation, int? timestamp, string requestId, string? handleId = null)
    {
        OperationInvocation invocation = new OperationInvocation() {
            RequestId = requestId,
            InputVariables = operation.InputVariables,
            InoutputVariables = operation.InoutputVariables,
            Timestamp = timestamp,
            IsAsync = handleId != null,
            HandleId = handleId
        };
        if (_invokeRequestStream == null) {
            throw new RpcException(Status.DefaultCancelled, "No connection");
        }
        try {
            await _invokeRequestStream.WriteAsync(invocation);
            await _invokeRequestStream.MoveNext();
            return _invokeRequestStream.Current;
        }
        catch (ObjectDisposedException) {
            return ExecutionState.FailedEnum;
        }
    }
}