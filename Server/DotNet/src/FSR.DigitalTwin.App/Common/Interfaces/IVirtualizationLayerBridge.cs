namespace FSR.DigitalTwin.App.Common.Interfaces;

public interface IVirtualizationLayerBridge {
    bool IsConnected { get; }
    IVirtualizationLayerOperational Operational { get; init; }

    Task Disconnect();
}