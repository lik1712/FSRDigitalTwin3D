using System.Collections.Generic;
using System.Threading.Tasks;
using FSR.Aas.GRPC.Lib.V3;

namespace FSR.DigitalTwin.Unity.Workspace.Digital.Interfaces {

    /// <summary>
    /// This interface represents an I4.0 asset, which is administrated via an administration shell
    /// The asset consists of a set of asset components. An asset component cannot be another asset,
    /// but it can of course contain a submodel linking to another asset.
    /// </summary>
    public interface IDigitalTwinAsset {
        
        List<IDigitalTwinComponent> Components { get; }

        string AdminShellId { get; }
        Task<AssetAdministrationShellDTO> GetAssetAdministrationShellAsync();
        Task<bool> HasConnectionAsync();

    }

} // END namespace FSR.DigitalTwin.Unity.Workspace.Digital.Interfaces