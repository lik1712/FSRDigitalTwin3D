using FSR.DigitalTwin.App.Common.Interfaces;

namespace FSR.DigitalTwinLayer.GRPC.Lib;

public class DigitalTwinLayerRpcBridge : IDigitalTwinLayerBridge
{
    public IDigitalTwinLayerOperational Operational { get; init; }

    public DigitalTwinLayerRpcBridge() { 
        Operational = new DigitalTwinLayerRpcOperational();
    }
}