namespace FSR.DigitalTwin.App.Common.Interfaces;

public interface IDigitalTwinLayerBridge {
    IDigitalTwinLayerOperational Operational { get; init; }
}