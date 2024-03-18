using AasxServerStandardBib.Logging;
using FSR.DigitalTwin.App.Common.Interfaces;
using FSR.DigitalTwin.App.Common.Models;
using FSR.DigitalTwin.App.Interfaces;
using Microsoft.Extensions.Logging;

namespace FSR.DigitalTwin.App.Services;

public class DigitalTwinLayerService : IDigitalTwinLayerService
{
    private readonly Dictionary<DigitalTwinMode, IDigitalTwinLayerBridge> _layers = [];
    private static readonly ILogger _logger = ApplicationLogging.CreateLogger("DigitalTwinLayerService");

    public DigitalTwinMode Mode { get; set; }
    public IDigitalTwinLayerBridge Layer => GetLayerByMode(Mode);

    public DigitalTwinLayerService()
    {
        Mode = DigitalTwinMode.DT_MODE_VIRTUAL;
    }

    public bool AddLayer<T>(DigitalTwinMode mode) where T : IDigitalTwinLayerBridge, new()
    {
        if (!HasLayer(mode)) {
            _logger.LogDebug($"Adding new DT layer for mode {mode}");
            _layers[mode] = new T();
            return true;
        }
        return false;
    }

    public IDigitalTwinLayerBridge GetLayerByMode(DigitalTwinMode mode)
    {
        return HasLayer(mode) ? _layers[mode] : throw new NullReferenceException();
    }

    public bool HasLayer(DigitalTwinMode mode)
    {
        return _layers.ContainsKey(mode);
    }

    public bool RemoveLayer(DigitalTwinMode mode)
    {
        return _layers.Remove(mode);
    }

    public void SetDefaultMode(DigitalTwinMode mode)
    {
        Mode = mode;
    }
}