using System.Threading.Tasks;
using FSR.Aas.GRPC.Lib.V3;

namespace FSR.DigitalTwin.Unity.Workspace.Digital.Interfaces {

    /// <summary>
    /// This interface represents an I4.0 asset component. It is directly linked to a submodel from the AAS metamodel.
    /// Its task is to receive and send data to the digital representation of the workspace.
    /// </summary>
    public interface IDigitalTwinComponent
    {
        string SubmodelId { get; }
        Task<bool> HasConnectionAsync();
        Task<SubmodelDTO> GetSubmodelAsync();

        Task<int> OnSynchronizeDataAsync();
        int OnSynchronizeData();

        Task<int> OnPushDataAsync();
        int OnPushData();

    }

} // END namespace FSR.DigitalTwin.Unity.Workspace.Digital.Interfaces