using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FSR.Aas.GRPC.Lib.V3;
using FSR.Aas.GRPC.Lib.V3.Services;
using FSR.Aas.GRPC.Lib.V3.Services.AssetAdministrationShellRepository;
using FSR.DigitalTwin.Unity.Workspace.Digital.Interfaces;
using FSR.DigitalTwin.Util;
using UnityEngine;

namespace FSR.DigitalTwin.Unity.Workspace.Digital {

    /// <summary>
    /// This interface represents an I4.0 asset, which is administrated via an administration shell
    /// The asset consists of a set of asset components. An asset component cannot be another asset,
    /// but it can of course contain a submodel linking to another asset.
    /// </summary>
    public abstract class DigitalTwinAsset : MonoBehaviour, IDigitalTwinAsset
    {
        [SerializeField] private string adminShellId = "";
        [SerializeField] private DigitalTwinComponent[] aspects;
        [SerializeField] private DigitalWorkspace digitalWorkspace;

        public List<IDigitalTwinComponent> Components => aspects.ToList<IDigitalTwinComponent>();
        public string AdminShellId => adminShellId;
        public DigitalWorkspace DigitalWorkspace => digitalWorkspace;
        public bool IsConnected => HasConnectionAsync().GetAwaiter().GetResult();

        public async Task<AssetAdministrationShellDTO> GetAssetAdministrationShellAsync()
        {
            var aasApi = DigitalWorkspace.ApiBridge.AasApi;
            GetAssetAdministrationShellByIdRpcRequest request = new() {
                Id = Base64Converter.ToBase64(AdminShellId),
                OutputModifier = new OutputModifier() {
                    Level = OutputLevel.Deep, Content = OutputContent.Normal, Cursor = "", 
                    Extent = OutputExtent.WithBlobValue, Limit = 32
                }
            };
            var response = await aasApi.AdminShellRepo.GetAssetAdministrationShellByIdAsync(request);
            Debug.Log("Requested Admin Shell with id '" + AdminShellId + "', Status " + response.StatusCode);
            if (response.StatusCode != 200) {
                throw new NullReferenceException("Requested AAS does not exist!");
            }
            return response.Payload;
        }

        public async Task<bool> HasConnectionAsync() {
            var aasApi = DigitalWorkspace.ApiBridge.AasApi;
            GetAssetAdministrationShellByIdRpcRequest request = new() {
                Id = Base64Converter.ToBase64(AdminShellId),
                OutputModifier = new OutputModifier() {
                    Level = OutputLevel.Core, Content = OutputContent.Reference, Cursor = "", 
                    Extent = OutputExtent.WithoutBlobValue, Limit = 1
                }
            };
            var response = await aasApi.AdminShellRepo.GetAssetAdministrationShellByIdAsync(request);
            return response.StatusCode == 200;
        }
    }

} // END namespace FSR.DigitalTwin.Unity.Workspace.Digital.Interfaces