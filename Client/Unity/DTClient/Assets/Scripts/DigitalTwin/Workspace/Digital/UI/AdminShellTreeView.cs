using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FSR.Workspace.Digital.Services;
using FSRAas.GRPC.Lib.V3;
using FSRAas.GRPC.Lib.V3.Services;
using FSRAas.GRPC.Lib.V3.Services.AssetAdministrationShellRepository;
using FSRAas.GRPC.Lib.V3.Services.SubmodelRepository;
using UnityEngine;

namespace FSR.Workspace.Digital.UI {

public class AdminShellTreeView : MonoBehaviour {

    [SerializeField]
    private Treeview treeView;
    private AdminShellApiServiceClient aasClient;

    private static OutputModifier Default = new OutputModifier() {
        Cursor = "",
        Limit = 1000,
        Level = OutputLevel.Deep,
        Content = OutputContent.Normal,
        Extent = OutputExtent.WithoutBlobValue
    };

    async void Start() {
        aasClient = DigitalWorkspace.Instance.ApiBridge.AasApiClient;
        await OnUpdateTreeViewAsync();
    }

    private class SubmodelElementModelCollection {
        public SubmodelElementDTO[] value { get; set; }
    }

    private async Task OnUpdateTreeViewAsync() {
        treeView.Root.Text = "[Environment (Digital Workspace)]";
        var aasNode = treeView.Root.AddChild("[AdminShells]", ReturnedNode.Created);
        var submodelNode = treeView.Root.AddChild("[Submodels]", ReturnedNode.Created);

        GetAllAssetAdministrationShellsRpcRequest request1 = new() { OutputModifier = Default };
        var response = await aasClient.AdminShellRepo.GetAllAssetAdministrationShellsAsync(request1);
        List<AssetAdministrationShellDTO> shellModels = response.Payload.ToList();

        GetAllSubmodelsRpcRequest request2 = new() { OutputModifier = Default };
        var subModelsRes = await aasClient.SubmodelRepo.GetAllSubmodelsAsync(request2);
        List<SubmodelDTO> submodelModels = subModelsRes.Payload.ToList();

        Action<SubmodelElementDTO, Node> submodelElementNodeTree = (e, node) => {};
        submodelElementNodeTree = (e, node) => {
            var elemNode = node.AddChild(e.IdShort, ReturnedNode.Created);
            // TODO Make prettier!
            if (e.SubmodelElementType == SubmodelElementType.SubmodelElementList) {
                foreach (var child in e.SubmodelElementList.Value) {
                    submodelElementNodeTree(child, elemNode);
                }
            }
            if (e.SubmodelElementType == SubmodelElementType.SubmodelElementCollection) {
                foreach (var child in e.SubmodelElementCollection.Value) {
                    submodelElementNodeTree(child, elemNode);
                }
            }
        };
        Action<SubmodelDTO, Node> submodelNodeTree = (sm, node) => {
            var smNode = node.AddChild(sm.IdShort, ReturnedNode.Created);
            foreach (var elem in sm.SubmodelElements) {
                submodelElementNodeTree(elem, smNode);
            }
        };
        Action<AssetAdministrationShellDTO, Node> aasNodeTree = (aas, node) => {
            var aasNode = node.AddChild(aas.IdShort, ReturnedNode.Created);
            var submodelIds = aas.Submodels.Select(x => x.Keys.First().Value);
            var refSubmodelModels = submodelModels.Where(x => submodelIds.Contains(x.Id));
            foreach (var submodel in refSubmodelModels) {
                submodelNodeTree(submodel, aasNode);
            }
        };

        foreach (var aasModel in shellModels) {
            aasNodeTree(aasModel, aasNode);
        }
        foreach (var submodelModel in submodelModels) {
            submodelNodeTree(submodelModel, submodelNode);
        }
    }
}

} // END namespace FSR.Workspace.Digital.UI