import grpc

from Protos.Services.AssetAdministrationShellService_pb2_grpc import AssetAdministrationShellServiceStub
from Protos.Services.AdminShellRepositoryService_pb2_grpc import AssetAdministrationShellRepositoryServiceStub
from Protos.Services.SubmodelRepositoryService_pb2_grpc import SubmodelRepositoryServiceStub
from Protos.Services.SubmodelService_pb2_grpc import SubmodelServiceStub
from Protos.Services.Connection.DigitalTwinLayerConnectionService_pb2_grpc import DigitalTwinLayerConnectionServiceStub
from Protos.Services.Operational.DigitalTwinLayerOperationalService_pb2_grpc import DigitalTwinLayerOperationalServiceStub


class AdminShellApiServiceClient:

    def __init__(self, channel : grpc.Channel) -> None:
        self._admin_shell = AssetAdministrationShellServiceStub(channel)
        self._admin_shell_repo = AssetAdministrationShellRepositoryServiceStub(channel)
        self._submodel = SubmodelServiceStub(channel)
        self._submodel_repo = SubmodelRepositoryServiceStub(channel)
    
    @property
    def admin_shell(self) -> AssetAdministrationShellServiceStub:
        return self._admin_shell

    @property
    def admin_shell_repo(self) -> AssetAdministrationShellRepositoryServiceStub:
        return self._admin_shell_repo
    
    @property
    def submodel(self) -> SubmodelServiceStub:
        return self._submodel

    @property
    def submodel_repo(self) -> SubmodelRepositoryServiceStub:
        return self._submodel_repo


class DigitalTwinLayerServiceClient:

    def __init__(self, channel : grpc.Channel) -> None:
        self._connection = DigitalTwinLayerConnectionServiceStub(channel)
        self._operational = DigitalTwinLayerOperationalServiceStub(channel)
    
    @property
    def operational(self) -> DigitalTwinLayerOperationalServiceStub:
        return self._operational

    @property
    def connection(self) -> DigitalTwinLayerConnectionServiceStub:
        return self._connection
    
    async def run_operational():
        pass
