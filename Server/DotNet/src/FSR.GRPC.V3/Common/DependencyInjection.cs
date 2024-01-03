using FSR.GRPC.V3.Common.Utils;
using FSR.GRPC.V3.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FSR.GRPC.V3.Common;

public class GrpcService {
    public static void MapServices(IEndpointRouteBuilder endpoints) {
        endpoints.MapGrpcService<AssetAdministrationShellRepositoryRpcService>();
        endpoints.MapGrpcService<SubmodelRepositoryRpcService>();
        endpoints.MapGrpcService<AssetAdministrationShellRpcService>();
        endpoints.MapGrpcService<SubmodelRpcService>();
    }

    public static void AddTo(IServiceCollection services) {
        services.AddAutoMapper(AppAssembly.GetAssembly());
        services.AddGrpc();
    }
}