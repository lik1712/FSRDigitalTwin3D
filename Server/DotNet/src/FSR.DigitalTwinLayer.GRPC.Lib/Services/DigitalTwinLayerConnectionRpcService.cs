using FSR.DigitalTwin.App.Common.Models;
using FSR.DigitalTwin.App.Interfaces;
using FSR.DigitalTwinLayer.GRPC.Lib.Services.Connection;
using Grpc.Core;

namespace FSR.DigitalTwinLayer.GRPC.Lib.Services;

public class DigitalTwinLayerConnectionRpcService : DigitalTwinLayerConnectionService.DigitalTwinLayerConnectionServiceBase {
    
    private readonly IDigitalTwinLayerService _layerService;

    public DigitalTwinLayerConnectionRpcService(IDigitalTwinLayerService layerService)
    {
        _layerService = layerService ?? throw new NullReferenceException();
    }
    
    public override Task<ConnectResponse> Connect(ConnectRequest request, ServerCallContext context)
    {
        var mode = (DigitalTwinMode) request.Type;
        var response = new ConnectResponse() {
            Success = true,
            Message = "Connection established",
            LayerId = "DTLayer::0" // TODO Manage and check via layer bridge
        };
        if (_layerService.HasLayer(mode)) {
            response.Success = false;
            response.Message = "Already established a connection under the specified layer mode. Abort connection first!";
            response.LayerId = "DTLayer::-1";
            return Task.FromResult(response);
        }
        if (!_layerService.AddLayer<DigitalTwinLayerRpcBridge>(mode)) {
            throw new RpcException(Status.DefaultCancelled, "Connection failed");
        }
        return Task.FromResult(response);
    }

    public override Task<AbortConnectionResponse> AbortConnection(AbortConnectionRequest request, ServerCallContext context)
    {
        var mode = (DigitalTwinMode) request.Type;
        var response = new AbortConnectionResponse() {
            Success = true,
            Message = "Connection aborted",
        };
        if (!_layerService.HasLayer(mode) || request.LayerId != "DTLayer::0") {
            response.Success = false;
            response.Message = "No connection";
            return Task.FromResult(response);
        }
        if (!_layerService.RemoveLayer(mode)) {
            throw new RpcException(Status.DefaultCancelled, "Connection failed");
        }
        return Task.FromResult(response);
    }
}