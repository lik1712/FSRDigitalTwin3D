using AdminShellNS.Models;
using FSR.DigitalTwin.App.Common.Interfaces;
using FSRAas.GRPC.Lib.V3;
using FSRAas.GRPC.Lib.V3.Services.Operational;
using Grpc.Core;

namespace FSRVirtual.GRPC.Lib;

public class VirtualLayerRpcOperational : IVirtualizationLayerOperational
{
    private IAsyncBidirectionalStream<OperationStatus, OperationInvokeRequest>? _invokeRequestStream;
    private IAsyncBidirectionalStream<OperationResult, OperationRequest>? _resultRequestStream;
    private IAsyncBidirectionalStream<OperationStatus, OperationRequest>? _statusRequestStream;

    public IAsyncBidirectionalStream<OperationStatus, OperationInvokeRequest> InvokeRequestStream { 
        get => _invokeRequestStream ?? throw new RpcException(Status.DefaultCancelled, "No connection");
        set => _invokeRequestStream ??= value;
    }
    public IAsyncBidirectionalStream<OperationResult, OperationRequest> ResultRequestStream { 
        get => _resultRequestStream ?? throw new RpcException(Status.DefaultCancelled, "No connection");
        set => _resultRequestStream ??= value;
    }
    public IAsyncBidirectionalStream<OperationStatus, OperationRequest> StatusRequestStream { 
        get => _statusRequestStream ?? throw new RpcException(Status.DefaultCancelled, "No connection");
        set => _statusRequestStream ??= value;
    }

    public Task OnDisconnect()
    {
        _invokeRequestStream = null;
        _resultRequestStream = null;
        _statusRequestStream = null;
        return Task.CompletedTask;
    }

    public Task<OperationStatus> GetExecutionStateAsync(string requestId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult> GetResultAsync(string requestId)
    {
        throw new NotImplementedException();
    }

    public async Task<OperationStatus> InvokeAsync(OperationPayloadDTO operation, int? timestamp, string requestId, string? handleId = null)
    {
        OperationInvokeRequest request = new() {
            RequestId = requestId,
            Timestamp = timestamp ?? int.MaxValue,
            IsAsync = handleId != null,
            HandleId = handleId ?? ""
        };
        request.InputVariables.AddRange(operation.InputVariables);
        request.InoutVariables.AddRange(operation.InoutVariables);

        if (_invokeRequestStream == null) {
            throw new RpcException(Status.DefaultCancelled, "No connection");
        }
        await _invokeRequestStream.WriteAsync(request);
        await _invokeRequestStream.MoveNext();
        return _invokeRequestStream.Current;
    }

    public bool HasConnection()
    {
        return _invokeRequestStream != null || _resultRequestStream != null || _statusRequestStream != null;
    }
}