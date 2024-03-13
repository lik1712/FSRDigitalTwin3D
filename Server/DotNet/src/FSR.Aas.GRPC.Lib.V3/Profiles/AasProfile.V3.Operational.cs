using AdminShellNS.Models;
using AutoMapper;
using FSR.Aas.GRPC.Lib.V3.Common.Utils;
using FSR.Aas.GRPC.Lib.V3.Services.Operational;

namespace FSR.DigitalTwin.App.Profiles;

public class OperationalProfile : Profile {

    public OperationalProfile()
    {
        CreateMap<OperationStatus, ExecutionState>().ConvertUsing((src, c, ctxt) => (ExecutionState) src.ExecutionState);
        
        CreateMap<FSR.Aas.GRPC.Lib.V3.Services.OperationResult, OperationResult>()
            .AfterMap((x, y) => Nullability.SetEmptyPropertiesToNull(y));
        
        CreateMap<string, OperationRequest>().ConvertUsing((src, c, ctxt) => new OperationRequest() { RequestId = src });
        
        CreateMap<OperationInvocation, OperationInvokeRequest>();
    }

}