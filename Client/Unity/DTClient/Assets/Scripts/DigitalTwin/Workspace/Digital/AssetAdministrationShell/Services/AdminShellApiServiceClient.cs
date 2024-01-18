using FSRAas.GRPC.Lib.V3.Services.AssetAdministrationShellRepository;
using FSRAas.GRPC.Lib.V3.Services.AssetAdministrationShellService;
using FSRAas.GRPC.Lib.V3.Services.SubmodelRepository;
using FSRAas.GRPC.Lib.V3.Services.SubmodelService;
using Grpc.Core;

namespace FSR.Workspace.Digital.AAS {

/// <summary>
/// API service client according to 'Details of the Asset Administration Shell - Part 2'
/// </summary>
public class AdminShellApiServiceClient {
    
    public AssetAdministrationShellService.AssetAdministrationShellServiceClient AdminShell { get; }
    public AssetAdministrationShellRepositoryService.AssetAdministrationShellRepositoryServiceClient AdminShellRepo { get; }
    public SubmodelService.SubmodelServiceClient Submodel { get; }
    public SubmodelRepositoryService.SubmodelRepositoryServiceClient SubmodelRepo { get; }

    public AdminShellApiServiceClient (Channel channel) {
        AdminShell = new(channel);
        AdminShellRepo = new(channel);
        Submodel = new(channel);
        SubmodelRepo = new(channel);
    }
}

} // END namespace FSR.Workspace.Digital.AAS