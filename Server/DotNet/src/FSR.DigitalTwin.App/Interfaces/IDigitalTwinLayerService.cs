using FSR.DigitalTwin.App.Common.Interfaces;
using FSR.DigitalTwin.App.Common.Models;

namespace FSR.DigitalTwin.App.Interfaces;

public interface IDigitalTwinLayerService 
{
    DigitalTwinMode Mode { set; get; }
    IDigitalTwinLayerBridge Layer { get; }

    IDigitalTwinLayerBridge GetLayerByMode(DigitalTwinMode mode);
    bool AddLayer<T>(DigitalTwinMode mode) where T : IDigitalTwinLayerBridge, new();
    bool HasLayer(DigitalTwinMode mode);
    bool RemoveLayer(DigitalTwinMode mode);
    void SetDefaultMode(DigitalTwinMode mode);
}