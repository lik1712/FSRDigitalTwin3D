using FSR.GRPC.Lib.V3.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace FSR.GRPC.Lib.V3.Common;

public static class GrpcService {
    public static void MapGrpcServices(this IEndpointRouteBuilder endpoints) {
        endpoints.MapGrpcService<AssetAdministrationShellRepositoryRpcService>();
        endpoints.MapGrpcService<SubmodelRepositoryRpcService>();
        endpoints.MapGrpcService<AssetAdministrationShellRpcService>();
        endpoints.MapGrpcService<SubmodelRpcService>();
    }
}