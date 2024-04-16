using System;
using System.Threading.Tasks;
using FSR.Aas.GRPC.Lib.V3;
using FSR.Aas.GRPC.Lib.V3.Services;
using FSR.Aas.GRPC.Lib.V3.Services.SubmodelRepository;
using FSR.DigitalTwin.Unity.Workspace.Digital.Interfaces;
using FSR.DigitalTwin.Util;
using UnityEngine;

namespace FSR.DigitalTwin.Unity.Workspace.Digital {

    /// <summary>
    /// This interface represents an I4.0 asset, which is administrated via an administration shell
    /// The asset consists of a set of asset components. An asset component cannot be another asset,
    /// but it can of course contain a submodel linking to another asset.
    /// </summary>
    public abstract class DigitalTwinComponent : MonoBehaviour, IDigitalTwinComponent
    {
        [SerializeField] private string submodelId = "";
        [SerializeField] private DigitalWorkspace digitalWorkspace;

        public string SubmodelId => submodelId;
        public DigitalWorkspace DigitalWorkspace => digitalWorkspace;
        public bool IsConnected => HasConnectionAsync().GetAwaiter().GetResult();

        public async Task<SubmodelDTO> GetSubmodelAsync()
        {
            var aasApi = DigitalWorkspace.ApiBridge.AasApi;
            GetSubmodelByIdRpcRequest request = new() {
                Id = Base64Converter.ToBase64(SubmodelId),
                OutputModifier = new OutputModifier() {
                    Level = OutputLevel.Deep, Content = OutputContent.Normal, Cursor = "", 
                    Extent = OutputExtent.WithBlobValue, Limit = 32
                }
            };
            var response = await aasApi.SubmodelRepo.GetSubmodelByIdAsync(request);
            if (response.StatusCode != 200) {
                throw new NullReferenceException("Submodel not found!");
            }
            return response.Payload;
        }

        public async Task<bool> HasConnectionAsync()
        {
            var aasApi = DigitalWorkspace.ApiBridge.AasApi;
            GetSubmodelByIdRpcRequest request = new() {
                Id = Base64Converter.ToBase64(SubmodelId),
                OutputModifier = new OutputModifier() {
                    Level = OutputLevel.Core, Content = OutputContent.Reference, Cursor = "", 
                    Extent = OutputExtent.WithoutBlobValue, Limit = 1
                }
            };
            var response = await aasApi.SubmodelRepo.GetSubmodelByIdAsync(request);
            return response.StatusCode == 200;
        }

        public virtual int OnPushData() 
        {
            return OnPushDataAsync().GetAwaiter().GetResult(); 
        }

        public abstract Task<int> OnPushDataAsync();

        public virtual int OnSynchronizeData()
        {
            return OnSynchronizeDataAsync().GetAwaiter().GetResult();
        }

        public abstract Task<int> OnSynchronizeDataAsync();
    }

} // END namespace FSR.DigitalTwin.Unity.Workspace.Digital.Interfaces