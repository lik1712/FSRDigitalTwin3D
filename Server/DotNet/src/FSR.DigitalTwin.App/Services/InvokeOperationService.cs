// using AasCore.Aas3_0;
// using AdminShellNS;
// using AdminShellNS.Models;
// using FSR.DigitalTwin.App.Interfaces;

// namespace FSR.DigitalTwin.App.Services;

// public class InvokeOperationService : IInvokeOperationService
// {
//     public List<IOperationReceiver> Receivers { get; init; }

//     public InvokeOperationService() 
//     {
//         Receivers = [];
//     }

//     public void AddReceiver<T>() where T : IOperationReceiver, new()
//     {
//         Receivers.Add(new T());
//     }

//     public OperationResult OnOperationInvoke(IOperation operation, int? timestamp, string requestId)
//     {
//         OperationResult result = new();
//         foreach (IOperationReceiver receiver in Receivers) {
//             result = receiver.OnOperationInvoke(operation, timestamp, requestId);
//             if (!result.Success) {
//                 break;
//             }
//             if (DateTime.UtcNow.Ticks > (timestamp ?? DateTime.MaxValue.Ticks)) {
//                 result.Success = false;
//                 result.ExecutionState = ExecutionState.Timeout;
//                 result.Message = "The requested operation timed out on server";
//                 result.RequestId = requestId;
//                 break;
//             }
//         }
//         return result;
//     }

//     public Task OnOperationInvokeAsync(OperationResult inoutResult, IOperation operation, int? timestamp, string requestId)
//     {
//         return Task.CompletedTask;
//     }
// }