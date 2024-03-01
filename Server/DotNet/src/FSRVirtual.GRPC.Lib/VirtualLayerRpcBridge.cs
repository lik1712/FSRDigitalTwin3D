using FSR.DigitalTwin.App.Common.Interfaces;

namespace FSRVirtual.GRPC.Lib;

public class VirtualLayerRpcBridge : IVirtualizationLayerBridge
{
    public bool IsConnected { get => HasConnection(); }
    public IVirtualizationLayerOperational Operational { get; init; }

    public VirtualLayerRpcBridge() {
        Operational = new VirtualLayerRpcOperational();
    }

    public Task Disconnect()
    {
        return Operational.OnDisconnect();
    }

    private bool HasConnection() {
        return Operational.HasConnection();
    }
}