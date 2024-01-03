using System.Threading.Tasks;
using FSR.Workspace.Digital.AAS;
using Grpc.Core;
using UnityEngine;

namespace FSR.Workspace.Digital {

public class DigitalWorkspaceBridge : MonoBehaviour
{
    [SerializeField] private string digitalWorkspaceAddr = "127.0.0.1";
    [SerializeField] private int digitalWorkspacePort = 5001;

    public Channel RpcChannel { get; }
    public AdminShellApiServiceClient AasApiClient { get; }

    public DigitalWorkspaceBridge() {
        RpcChannel = new Channel(digitalWorkspaceAddr, digitalWorkspacePort, ChannelCredentials.Insecure);
        AasApiClient = new AdminShellApiServiceClient(RpcChannel);
    }


    // =========== EXAMPLE REQUESTS =========== //
    void Start() {
        // await RunExampleRequests__(RpcChannel);
    }

    // private static async Task RunExampleRequests__(Channel channel) {
    //     var aasRepo = new AssetAdministrationShellRepositoryService
    //         .AssetAdministrationShellRepositoryServiceClient(channel);

    //     GetAllAssetAdministrationShellsRpcRequest request = new();
    //     using (var call = aasRepo.GetAllAssetAdministrationShells(request)) {
    //         while (await call.ResponseStream.MoveNext()) {
    //             var aas = call.ResponseStream.Current;
    //             Debug.Log("Found AAS with unique id: " + aas.Id);
    //         }
    //     }

    //     var smRepo = new SubmodelRepositoryService
    //         .SubmodelRepositoryServiceClient(channel);

    //     GetAllSubmodelsRpcRequest request4 = new();
    //     using (var call = smRepo.GetAllSubmodels(request4)) {
    //         while (await call.ResponseStream.MoveNext()) {
    //             var sm = call.ResponseStream.Current;
    //             Debug.Log("Found Submodel with unique id: " + sm.Id + " and " + sm.SubmodelElements.Count + " submodel elements.");
    //         }
    //     }

    //     var submodelService = new SubmodelService.SubmodelServiceClient(channel);

    //     GetAllSubmodelElementsRpcRequest request1 = new() { SubmodelId = "fsr.sm1.net" };
    //     using (var call = submodelService.GetAllSubmodelElements(request1)) {
    //         while (await call.ResponseStream.MoveNext()) {
    //             var elem = call.ResponseStream.Current;
    //             Debug.Log("Found Submodel element with unique short id: " + elem.IdShort + " (type: " + elem.SubmodelElementType + ")");
    //         }
    //     } 

    //     GetSubmodelElementByPathRpcRequest request2 = new() { SubmodelId = "fsr.sm1.net" };
    //     request2.Path.Add(new KeyModel() { Type = KeyTypes.SubmodelElement, Value = "MyCollection" });
    //     request2.Path.Add(new KeyModel() { Type = KeyTypes.SubmodelElement, Value = "TestText" });
    //     var res = submodelService.GetSubmodelElementByPath(request2);
    //     if (res.Success) {
    //         Debug.Log("Found Submodel Element: " + res.SubmodelElement.IdShort + " with type " + res.SubmodelElement.SubmodelElementType);
    //     }

    //     PostSubmodelElementByPathRpcRequest request3 = new() { SubmodelId = "fsr.sm1.net" };
    //     request3.Path.Add(new KeyModel() { Type = KeyTypes.SubmodelElement, Value = "MyCollection" });
    //     request3.Path.Add(new KeyModel() { Type = KeyTypes.SubmodelElement, Value = "MyRange" });
    //     request3.SubmodelId = "fsr.sm1.net";
    //         request3.SubmodelElement = new SubmodelElementModel
    //         {
    //             IdShort = "MyRange",
    //             SubmodelElementType = SubmodelElementType.Range,
    //             Range = new RangeDTO
    //             {
    //                 ValueType = DataTypeDefXsd.Float,
    //                 Min = "0.0",
    //                 Max = "1.0"
    //             }
    //         };
    //         var res0 = await submodelService.PostSubmodelElementByPathAsync(request3);
    //     if (res0.Success) {
    //         Debug.Log("Posted Submodel Element: " + res0.SubmodelElement.IdShort + " with type " + res0.SubmodelElement.SubmodelElementType);
    //     }
    //     else {
    //         Debug.Log("Failed posting SubmodelElement!");
    //     }

    //     // =========== END Example requests =========== //
    // }

}

} // END namespace FSR.Workspace.Digital