using AasCore.Aas3_0;
using AasxServerStandardBib.Exceptions;
using AasxServerStandardBib.Interfaces;
using AasxServerStandardBib.Logging;
using AdminShellNS.Exceptions;
using AutoMapper;
using FSR.Aas.GRPC.Lib.V3.Services.AssetAdministrationShellRepository;
using Grpc.Core;
using IO.Swagger.Lib.V3.Interfaces;
using IO.Swagger.Lib.V3.SerializationModifiers.Mappers;
using IO.Swagger.Models;
using Microsoft.AspNetCore.Authorization;

namespace FSR.Aas.GRPC.Lib.V3.Services;

public class AssetAdministrationShellRepositoryRpcService : AssetAdministrationShellRepositoryService.AssetAdministrationShellRepositoryServiceBase {
    
    private readonly IAppLogger<AssetAdministrationShellRepositoryRpcService> _logger;
    private readonly IAssetAdministrationShellService _aasService;
    private readonly IBase64UrlDecoderService _decoderService;
    private readonly IAasRepositoryApiHelperService _aasRepoApiHelper;
    private readonly IReferenceModifierService _referenceModifierService;
    private readonly IMappingService _mappingService;
    private readonly IPathModifierService _pathModifierService;
    private readonly ILevelExtentModifierService _levelExtentModifierService;
    private readonly IPaginationService _paginationService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IMapper _mapper;
    
    public AssetAdministrationShellRepositoryRpcService(IAppLogger<AssetAdministrationShellRepositoryRpcService> logger, IAssetAdministrationShellService aasService, IBase64UrlDecoderService decoderService, IAasRepositoryApiHelperService aasRepoApiHelper, IReferenceModifierService referenceModifierService, IMappingService mappingService, IPathModifierService pathModifierService, ILevelExtentModifierService levelExtentModifierService, IPaginationService paginationService, IAuthorizationService authorizationService, IMapper mapper) {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _aasService = aasService ?? throw new ArgumentNullException(nameof(aasService));
        _decoderService = decoderService ?? throw new ArgumentNullException(nameof(decoderService));
        _aasRepoApiHelper = aasRepoApiHelper ?? throw new ArgumentNullException(nameof(aasRepoApiHelper));
        _referenceModifierService = referenceModifierService ?? throw new ArgumentNullException(nameof(referenceModifierService));
        _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));
        _pathModifierService = pathModifierService ?? throw new ArgumentNullException(nameof(pathModifierService));
        _levelExtentModifierService = levelExtentModifierService ?? throw new ArgumentNullException(nameof(levelExtentModifierService));
        _paginationService = paginationService ?? throw new ArgumentNullException(nameof(levelExtentModifierService));
        _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public override Task<GetAllAssetAdministrationShellsRpcResponse> GetAllAssetAdministrationShells(GetAllAssetAdministrationShellsRpcRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"Received the request to get all Asset Administration Shells.");

        var aasList = _aasService.GetAllAssetAdministrationShells();
        var aasPaginatedList = _paginationService.GetPaginatedList(aasList, new PaginationParameters(request.OutputModifier.Cursor, request.OutputModifier.Limit));

        var response = new GetAllAssetAdministrationShellsRpcResponse() { 
            StatusCode = 200,
            PagingMetaData = _mapper.Map<PagedResultPagingMetadata>(aasPaginatedList.paging_metadata),
        };

        if (request.OutputModifier.Content == OutputContent.Normal) {
            response.Payload.AddRange(aasPaginatedList.result.Select(x => _mapper.Map<AssetAdministrationShellDTO>(x)) ?? []);
        }
        else if (request.OutputModifier.Content == OutputContent.Reference) {
            var references = _referenceModifierService.GetReferenceResult(aasPaginatedList.result.ConvertAll(a => (IReferable)a));
            response.Reference.AddRange(references.Select(x => _mapper.Map<ReferenceDTO>(x)) ?? []);
        }
        else {
            response.StatusCode = 400;
        }

        return Task.FromResult(response);
    }

    public override Task<GetAssetAdministrationShellByIdRpcResponse> GetAssetAdministrationShellById(GetAssetAdministrationShellByIdRpcRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"Received request to get the AAS with id {request.Id}.");

        var decodedAasIdentifier = _decoderService.Decode("aasIdentifier", request.Id);

        GetAssetAdministrationShellByIdRpcResponse response = new();

        try {
            var aas = _aasService.GetAssetAdministrationShellById(decodedAasIdentifier);
            response.StatusCode = 200;
            if (request.OutputModifier.Content == OutputContent.Normal) {
                response.Payload = _mapper.Map<AssetAdministrationShellDTO>(aas);
            }
            else if (request.OutputModifier.Content == OutputContent.Reference) {
                var reference = _referenceModifierService.GetReferenceResult(aas);
                response.Reference = _mapper.Map<ReferenceDTO>(reference);
            }
            else {
                response.StatusCode = 400;
            }
        }
        catch (NotFoundException) {
            response.StatusCode = 404;
        }
        return Task.FromResult(response);
    }

    public override Task<GetAllAssetAdministrationShellsByAssetIdRpcResponse> GetAllAssetAdministrationShellsByAssetId(GetAllAssetAdministrationShellsByAssetIdRpcRequest request, ServerCallContext context)
    {
        SpecificAssetId specificAssetId = new(request.KeyIdentifier, request.Key);

        _logger.LogInformation($"Received the request to get all Asset Administration Shells by AssetId.");

        var aasList = _aasService.GetAllAssetAdministrationShells([specificAssetId]);
        var aasPaginatedList = _paginationService.GetPaginatedList(aasList, new PaginationParameters(request.OutputModifier.Cursor, request.OutputModifier.Limit));

        var response = new GetAllAssetAdministrationShellsByAssetIdRpcResponse() { 
            StatusCode = 200,
            PagingMetaData = _mapper.Map<PagedResultPagingMetadata>(aasPaginatedList.paging_metadata),
        };

        if (request.OutputModifier.Content == OutputContent.Normal) {
            response.Payload.AddRange(aasPaginatedList.result.Select(x => _mapper.Map<AssetAdministrationShellDTO>(x)) ?? []);
        }
        else if (request.OutputModifier.Content == OutputContent.Reference) {
            var references = _referenceModifierService.GetReferenceResult(aasPaginatedList.result.ConvertAll(a => (IReferable)a));
            response.Reference.AddRange(references.Select(x => _mapper.Map<ReferenceDTO>(x)) ?? []);
        }
        else {
            response.StatusCode = 400;
        }

        return Task.FromResult(response);
    }

    public override Task<GetAllAssetAdministrationShellsByIdShortRpcResponse> GetAllAssetAdministrationShellsByIdShort(GetAllAssetAdministrationShellsByIdShortRpcRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"Received the request to get all Asset Administration Shells by IdShort.");

        var aasList = _aasService.GetAllAssetAdministrationShells(null, request.IdShort);
        var aasPaginatedList = _paginationService.GetPaginatedList(aasList, new PaginationParameters(request.OutputModifier.Cursor, request.OutputModifier.Limit));

        var response = new GetAllAssetAdministrationShellsByIdShortRpcResponse() { 
            StatusCode = 200,
            PagingMetaData = _mapper.Map<PagedResultPagingMetadata>(aasPaginatedList.paging_metadata),
        };

        if (request.OutputModifier.Content == OutputContent.Normal) {
            response.Payload.AddRange(aasPaginatedList.result.Select(x => _mapper.Map<AssetAdministrationShellDTO>(x)) ?? []);
        }
        else if (request.OutputModifier.Content == OutputContent.Reference) {
            var references = _referenceModifierService.GetReferenceResult(aasPaginatedList.result.ConvertAll(a => (IReferable)a));
            response.Reference.AddRange(references.Select(x => _mapper.Map<ReferenceDTO>(x)) ?? []);
        }
        else {
            response.StatusCode = 400;
        }

        return Task.FromResult(response);
    }

    public override Task<PostAssetAdministrationShellRpcResponse> PostAssetAdministrationShell(PostAssetAdministrationShellRpcRequest request, ServerCallContext context)
    {
        IAssetAdministrationShell aas = _mapper.Map<AssetAdministrationShell>(request.Aas);

        var response = new PostAssetAdministrationShellRpcResponse();

        try {
            var output = _aasService.CreateAssetAdministrationShell(aas);
            response.StatusCode = 201;
            response.Aas = _mapper.Map<AssetAdministrationShellDTO>(output);
        }
        catch (DuplicateException) {
            response.StatusCode = 400;
        }

        return Task.FromResult(response);
    }

    public override Task<PutAssetAdministrationShellByIdRpcResponse> PutAssetAdministrationShellById(PutAssetAdministrationShellByIdRpcRequest request, ServerCallContext context)
    {
        IAssetAdministrationShell aas = _mapper.Map<AssetAdministrationShell>(request.Aas);
        var decodedAasIdentifier = _decoderService.Decode("aasIdentifier", request.Aas.Id);

        var response = new PutAssetAdministrationShellByIdRpcResponse();

        try {
            _aasService.ReplaceAssetAdministrationShellById(decodedAasIdentifier, aas);
            response.StatusCode = 200;
            response.Aas = _mapper.Map<AssetAdministrationShellDTO>(request.Aas);
        }
        catch (NotFoundException) {
            response.StatusCode = 404;
        }
        catch (MetamodelVerificationException) {
            response.StatusCode = 400;
        }

        return Task.FromResult(response);
    }

    public override Task<DeleteAssetAdministrationShellByIdRpcResponse> DeleteAssetAdministrationShellById(DeleteAssetAdministrationShellByIdRpcRequest request, ServerCallContext context)
    {
        var decodedAasIdentifier = _decoderService.Decode("aasIdentifier", request.Id);

        _logger.LogInformation($"Received request to delete AAS with id {decodedAasIdentifier}");
        
        var response = new DeleteAssetAdministrationShellByIdRpcResponse();
        try {
            _aasService.DeleteAssetAdministrationShellById(decodedAasIdentifier);
            response.StatusCode = 204;
        }
        catch (NotFoundException) {
            response.StatusCode = 404;
        }

        return Task.FromResult(response);
    }
}