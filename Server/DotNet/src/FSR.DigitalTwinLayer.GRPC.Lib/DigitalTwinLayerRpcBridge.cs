using FSR.DigitalTwin.App.Common.Interfaces;

namespace FSR.DigitalTwinLayer.GRPC.Lib;

public class DigitalTwinLayerRpcBridge : IDigitalTwinLayerBridge
{
    public bool IsConnected { get => HasConnection(); }
    public IDigitalTwinLayerOperational Operational { get; init; }

    public DigitalTwinLayerRpcBridge() {
        Operational = new DigitalTwinLayerRpcOperational();
    }

    public Task Disconnect()
    {
        return Operational.OnDisconnect();
    }

    private bool HasConnection() {
        return Operational.HasConnection();
    }
}