using Grpc.Core;

using Operational = FSR.Aas.GRPC.Lib.V3.Services.Operational.DigitalTwinLayerOperationalService.DigitalTwinLayerOperationalServiceClient;
using Connection = FSR.DigitalTwinLayer.GRPC.Lib.Services.Connection.DigitalTwinLayerConnectionService.DigitalTwinLayerConnectionServiceClient;
using System.Threading.Tasks;
using UnityEngine;
using FSR.Aas.GRPC.Lib.V3.Services.Operational;
using FSR.Aas.GRPC.Lib.V3.Services;


namespace FSR.DigitalTwin.Unity.Workspace.Digital.Services {

/// <summary>
/// Services for inter-layer communication
/// </summary>
public class DigitalTwinLayerServiceClient {
    
    public Operational Operational { get; }
    public Connection Connection { get; }
    
    public DigitalTwinLayerServiceClient (Channel channel) {
        Operational = new Operational(channel);
        Connection = new Connection(channel);
    }

    public async Task RunOperational () {
        var invokeStream = Operational.OpenOperationInvocationStream();
        var resultStream = Operational.OpenOperationResultStream();
        var statusStream = Operational.OpenExecutionStateStream();

        while (await invokeStream.ResponseStream.MoveNext()) {
            Debug.Log("[From server]: " + invokeStream.ResponseStream.Current.RequestId);
            await invokeStream.RequestStream.WriteAsync(new OperationStatus { ExecutionState = ExecutionState.Completed, RequestId = invokeStream.ResponseStream.Current.RequestId });
            
            await resultStream.ResponseStream.MoveNext();
            Debug.Log("[From server]: " + resultStream.ResponseStream.Current.RequestId);
            Debug.Log("[From server]: Number of input parameters is " + invokeStream.ResponseStream.Current.InputVariables.Count);
            Debug.Log("[From server]: Number of in-output parameters is " + invokeStream.ResponseStream.Current.InoutVariables.Count);

            await Task.Delay(3000); // Do some work...

            await resultStream.RequestStream.WriteAsync(new OperationResult() {
                Success = true,
                RequestId = resultStream.ResponseStream.Current.RequestId,
                ExecutionState = ExecutionState.Completed,
                Message = "Test operation finished!"
            });
        }
    }
}

} // END namespace FSR.Workspace.Digital.Services