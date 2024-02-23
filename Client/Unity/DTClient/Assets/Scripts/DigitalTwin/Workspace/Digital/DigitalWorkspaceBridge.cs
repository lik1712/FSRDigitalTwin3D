using System;
using System.Linq;
using System.Threading.Tasks;
using FSRAas.GRPC.Lib.V3;
using FSRAas.GRPC.Lib.V3.Services;
using FSRAas.GRPC.Lib.V3.Services.AssetAdministrationShellRepository;
using FSRAas.GRPC.Lib.V3.Services.SubmodelRepository;
using FSRAas.GRPC.Lib.V3.Services.SubmodelService;
using FSRAas.GRPC.Lib.V3.Services.Operational;
using FSR.Workspace.Digital.Services;
using Grpc.Core;
using UnityEngine;
using Unity.VisualScripting;

namespace FSR.Workspace.Digital {

public class DigitalWorkspaceBridge : MonoBehaviour
{
    [SerializeField] private string digitalWorkspaceAddr = "127.0.0.1";
    [SerializeField] private int digitalWorkspacePort = 5001;

    public Channel RpcChannel { get; }
    public AdminShellApiServiceClient AasApiClient { get; }
    public VirtualLayerOperationService.VirtualLayerOperationServiceClient Operational { get; }

    public DigitalWorkspaceBridge() {
        RpcChannel = new Channel(digitalWorkspaceAddr, digitalWorkspacePort, ChannelCredentials.Insecure);
        AasApiClient = new AdminShellApiServiceClient(RpcChannel);
        Operational = new(RpcChannel);
    }

    async void Start() {
        Debug.Log("Running data channel test...");
        await DigitalWorkspaceExampleRequests.RunDataExchangeTest(AasApiClient, Operational, RpcChannel);
        Debug.Log("Done!");
    }

}


// ============================================================= //
// Test Requests to AAS
// ============================================================= //
public class DigitalWorkspaceExampleRequests {
    public static async Task RunDataExchangeTest(AdminShellApiServiceClient client, VirtualLayerOperationService.VirtualLayerOperationServiceClient operational, Channel channel) {
        var stream = operational.OpenOperationInvocationStream();

        // await stream.ResponseStream.MoveNext();
        // var next = stream.ResponseStream.Current;
        // Debug.Log("[From server]: " + next.RequestId);
        
        // await stream.RequestStream.WriteAsync(new OperationStatus() { RequestId = "Aloha!", ExecutionState = ExecutionState.Running });
        await stream.RequestStream.CompleteAsync();

        InvokeOperationSyncRequest invokeRequest = new() {
            SubmodelId = "aHR0cHM6Ly93d3cuaHMtZW1kZW4tbGVlci5kZS9pZHMvc20vNjQ5NF8yMTYyXzUwMzJfMjgxMw",
            Timestamp = 0,
            RequestId = "MyRequestId::1",
        };
        invokeRequest.Path.Add(new KeyDTO() { Type = KeyTypes.Operation, Value = "pick_and_place" });
        var invokeResponse = client.Submodel.InvokeOperationSync(invokeRequest);
        if (invokeResponse.StatusCode == 200) {
            Debug.Log("Successfully invoked operation synchronously!");
        }
        else {
            Debug.Log("Failed invocation = " + invokeResponse.StatusCode);
            return;
        }
    }
}
// ============================================================= //
// END - Test Requests to AAS
// ============================================================= //


} // END namespace FSR.Workspace.Digital