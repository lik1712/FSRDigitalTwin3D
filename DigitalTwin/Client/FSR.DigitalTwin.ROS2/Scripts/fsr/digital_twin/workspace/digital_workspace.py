import grpc
from Protos.Services.Connection.DigitalTwinLayerConnectionService_pb2 import ConnectRequest, AbortConnectionRequest
from fsr.digital_twin.abc.workspace import *
from fsr.digital_twin.workspace.services import AdminShellApiServiceClient, DigitalTwinLayerServiceClient

from Protos.Services.AdminShellRepositoryService_pb2 import GetAllAssetAdministrationShellsRpcRequest

class DigitalWorkspaceBridge:

    def __init__(self):
        self._address = "127.0.0.1"
        self._port = 5001
        self._rpc_channel : grpc.Channel = None
        self._aas_api_service_client : AdminShellApiServiceClient = None
        self._layer_service_client : DigitalTwinLayerServiceClient = None
    
    @property
    def aas_api(self) -> AdminShellApiServiceClient:
        return self._aas_api_service_client

    @property
    def layer(self) -> DigitalTwinLayerServiceClient:
        return self._layer_service_client

    def connect_to_api(self, addr : str, port : int):
        self._address = addr
        self._port = port
        self._rpc_channel = grpc.insecure_channel(self._address + ":" + str(self._port))
        self._aas_api_service_client = AdminShellApiServiceClient(self._rpc_channel)

        # For quick testing...
        request = GetAllAssetAdministrationShellsRpcRequest()
        request.outputModifier.cursor = ""
        request.outputModifier.limit = 32
        request.outputModifier.level = 0
        request.outputModifier.content = 0
        request.outputModifier.extent = 0
        shells = self._aas_api_service_client.admin_shell_repo.GetAllAssetAdministrationShells(request)
        print("[Client]: Recieved Admin Shells: " + str(shells))
    
    def connect(self, addr : str, port : int):
        self._address = addr
        self._port = port
        
        self._rpc_channel = grpc.insecure_channel(self._address + ":" + str(self._port))
        self._aas_api_service_client = AdminShellApiServiceClient(self._rpc_channel)
        self._layer_service_client = DigitalTwinLayerServiceClient(self._rpc_channel)
        connectRequest = ConnectRequest()
        connectRequest.type = 1
        connectResponse = self.layer.connection.Connect(connectRequest)
        if not connectResponse.success:
            raise RuntimeError(connectResponse.message)
    
    def disconnect(self):
        request = AbortConnectionRequest()
        request.layerId = "DTLayer::0"
        request.type = 1
        abortResponse = self.layer.connection.AbortConnection(request)
        if not abortResponse.success:
            raise RuntimeError(abortResponse.message)


class DigitalWorkspace(Workspace):
    def __init__(self, addr : str = "127.0.0.1", port : int = 5001):
        self._addr = addr
        self._port = port
        self._api_bridge : DigitalWorkspaceBridge = DigitalWorkspaceBridge()
    
    @property
    def api_bridge(self) -> DigitalWorkspaceBridge:
        return self._api_bridge
    
    def _get_kind(self):
        return WorkspaceKind.DIGITAL
    
    def connect(self):
        # self._api_bridge.connect_to_api(self._addr, self._port)
        self._api_bridge.connect(self._addr, self._port)
    
    def disconnect(self):
        self._api_bridge.disconnect()

    