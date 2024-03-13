namespace FSR.DigitalTwin.App.Common.Interfaces;

public interface IDigitalTwinLayerBridge {
    bool IsConnected { get; }
    IDigitalTwinLayerOperational Operational { get; init; }

    Task Disconnect();
}