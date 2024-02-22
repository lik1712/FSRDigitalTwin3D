using System.Security.Claims;
using AasCore.Aas3_0;
using AasOperationInvocation;
using AasxServer;
using AasxServerStandardBib.Exceptions;
using AasxServerStandardBib.Interfaces;
using AasxServerStandardBib.Logging;
using AdminShellNS.Exceptions;
using AutoMapper;
using FSRAas.GRPC.Lib.V3.Services;
using FSRAas.GRPC.Lib.V3.Services.SubmodelService;
using Grpc.Core;
using IO.Swagger.Lib.V3.Interfaces;
using IO.Swagger.Lib.V3.SerializationModifiers.Mappers;
using IO.Swagger.Lib.V3.Services;
using IO.Swagger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace FSRAas.GRPC.Lib.V3;

public class SubmodelRpcService : SubmodelService.SubmodelServiceBase {

    private readonly IAppLogger<SubmodelRpcService> _logger;
    private readonly IBase64UrlDecoderService _decoderService;
    private readonly ISubmodelService _submodelService;
    private readonly IReferenceModifierService _referenceModifierService;
    private readonly IJsonQueryDeserializer _jsonQueryDeserializer;
    private readonly IMappingService _mappingService;
    private readonly IPathModifierService _pathModifierService;
    private readonly ILevelExtentModifierService _levelExtentModifierService;
    private readonly IPaginationService _paginationService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IMapper _mapper;

    public SubmodelRpcService(IAppLogger<SubmodelRpcService> logger, IBase64UrlDecoderService decoderService, ISubmodelService submodelService, IReferenceModifierService referenceModifierService, IJsonQueryDeserializer jsonQueryDeserializer, IMappingService mappingService, IPathModifierService pathModifierService, ILevelExtentModifierService levelExtentModifierService, IPaginationService paginationService, IAuthorizationService authorizationService, IMapper mapper)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _decoderService = decoderService ?? throw new ArgumentNullException(nameof(decoderService));
        _submodelService = submodelService ?? throw new ArgumentNullException(nameof(submodelService));
        _referenceModifierService = referenceModifierService ?? throw new ArgumentNullException(nameof(referenceModifierService));
        _jsonQueryDeserializer = jsonQueryDeserializer ?? throw new ArgumentNullException(nameof(jsonQueryDeserializer));
        _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));
        _pathModifierService = pathModifierService ?? throw new ArgumentNullException(nameof(pathModifierService));
        _levelExtentModifierService = levelExtentModifierService ?? throw new ArgumentNullException(nameof(pathModifierService));
        _paginationService = paginationService ?? throw new ArgumentNullException(nameof(pathModifierService));
        _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public override Task<GetSubmodelRpcResponse> GetSubmodel(GetSubmodelRpcRequest request, ServerCallContext context)
    {
        var decodedSubmodelIdentifier = _decoderService.Decode("submodelIdentifier", request.SubmodelId);

        _logger.LogInformation($"Received request to get the Submodel with id {request.SubmodelId}.");

        GetSubmodelRpcResponse response = new();

        ISubmodel submodel;
        try {
            submodel = _submodelService.GetSubmodelById(decodedSubmodelIdentifier);
        }
        catch(NotFoundException) {
            response.StatusCode = 404;
            return Task.FromResult(response);
        }

        var authResult = _authorizationService.AuthorizeAsync(context.GetHttpContext().User, submodel, "SecurityPolicy").Result;
        if (!authResult.Succeeded) {
            response.StatusCode = 403;
            return Task.FromResult(response);
        }

        var output = _levelExtentModifierService.ApplyLevelExtent(
            submodel, (LevelEnum)request.OutputModifier.Level, (ExtentEnum)request.OutputModifier.Extent);
        if (output is not ISubmodel) {
            throw new NullReferenceException("should not happen");
        }
        ISubmodel result = (ISubmodel) output;

        if (request.OutputModifier.Content == OutputContent.Normal) {
            response.Payload = _mapper.Map<SubmodelDTO>(submodel);
        }
        else if (request.OutputModifier.Content == OutputContent.Reference) {
            var reference = _referenceModifierService.GetReferenceResult(submodel);
            response.Reference = _mapper.Map<ReferenceDTO>(reference);
        }
        else {
            response.StatusCode = 400;
        }

        return Task.FromResult(response);
    }

    public override Task<GetAllSubmodelElementsRpcResponse> GetAllSubmodelElements(GetAllSubmodelElementsRpcRequest request, ServerCallContext context)
    {
        var decodedSubmodelIdentifier = _decoderService.Decode("submodelIdentifier", request.SubmodelId);

        GetAllSubmodelElementsRpcResponse response = new();

        _logger.LogInformation($"Received a request to get all the submodel elements from submodel with id {decodedSubmodelIdentifier}");
        if (!Program.noSecurity)
        {
            var submodel = _submodelService.GetSubmodelById(decodedSubmodelIdentifier);
            var authResult = _authorizationService.AuthorizeAsync(context.GetHttpContext().User, submodel, "SecurityPolicy").Result;
            if (!authResult.Succeeded)
            {
                response.StatusCode = 403;
                return Task.FromResult(response);
            }
        }

        var submodelElements = _submodelService.GetAllSubmodelElements(decodedSubmodelIdentifier);
        var smePaginatedList = _paginationService.GetPaginatedList(submodelElements, new PaginationParameters(request.OutputModifier.Cursor, request.OutputModifier.Limit));
        
        response.StatusCode = 400;
        response.PagingMetaData = _mapper.Map<Services.PagedResultPagingMetadata>(smePaginatedList.paging_metadata);

        if (request.OutputModifier.Content == OutputContent.Normal) {
            var smeLevelExtent = _levelExtentModifierService.ApplyLevelExtent(smePaginatedList.result, (LevelEnum)request.OutputModifier.Level, (ExtentEnum)request.OutputModifier.Extent);
            response.Payload.AddRange(smeLevelExtent.Select(x => _mapper.Map<SubmodelElementDTO>((ISubmodelElement) x)) ?? []);
        }
        else if (request.OutputModifier.Content == OutputContent.Reference) {
            var references = _referenceModifierService.GetReferenceResult(smePaginatedList.result.ConvertAll(a => (IReferable)a));
            response.Reference.AddRange(references.Select(x => _mapper.Map<ReferenceDTO>(x)) ?? []);
        }
        else if (request.OutputModifier.Content == OutputContent.Path) {
            var paths = _pathModifierService.ToIdShortPath(submodelElements);
            response.Path.AddRange(paths.Select(ToIdShortDotSeparatedPath) ?? []);
        }
        else if (request.OutputModifier.Content == OutputContent.Value) {
            var smeLevelExtent = _levelExtentModifierService.ApplyLevelExtent(smePaginatedList.result, (LevelEnum)request.OutputModifier.Level, (ExtentEnum)request.OutputModifier.Extent);
            var smeValues = _mappingService.Map(smeLevelExtent, "value");
            throw new NotImplementedException(); // TODO Transferring values still WIP
        }
        else {
            response.StatusCode = 400;
        }

        return Task.FromResult(response);
    }

    private string ToIdShortDotSeparatedPath(IEnumerable<string> path) {
        string res = "";
        bool first = true;

        foreach(string idShort in path) {
            if (!first) {
                res += ".";
            }
            first = false;
            res += idShort;
        }

        return res;
    }

    public override Task<GetSubmodelElementByPathRpcResponse> GetSubmodelElementByPath(GetSubmodelElementByPathRpcRequest request, ServerCallContext context)
    {
        var decodedSubmodelIdentifier = _decoderService.Decode("submodelIdentifier", request.SubmodelId);
        _logger.LogInformation($"Received request to path of the submodel element at {request.SubmodelId} from a submodel with id {decodedSubmodelIdentifier}");

        GetSubmodelElementByPathRpcResponse response = new();

        string idShortPath = ToIdShortDotSeparatedPath(request.Path.Select(x => x.Value));
        var submodelElement = _submodelService.GetSubmodelElementByPath(decodedSubmodelIdentifier, idShortPath);
        if (!Program.noSecurity)
        {
            var submodel = _submodelService.GetSubmodelById(decodedSubmodelIdentifier);
            context.GetHttpContext().User.Claims.ToList().Add(new Claim("idShortPath", submodel.IdShort + "." + idShortPath));
            var claimsList = new List<Claim>(context.GetHttpContext().User.Claims)
            {
                new Claim("IdShortPath", submodel.IdShort + "." + idShortPath)
            };
            var identity = new ClaimsIdentity(claimsList, "AasSecurityAuth");
            var principal = new System.Security.Principal.GenericPrincipal(identity, null);
            var authResult = _authorizationService.AuthorizeAsync(principal, submodel, "SecurityPolicy").Result;
            if (!authResult.Succeeded)
            {
                response.StatusCode = 403;
                return Task.FromResult(response);
            }
        }

        if (request.OutputModifier.Content == OutputContent.Normal) {
            var submodelElementLevel = _levelExtentModifierService.ApplyLevelExtent(submodelElement, (LevelEnum)request.OutputModifier.Level);
            response.Payload = _mapper.Map<SubmodelElementDTO>((ISubmodelElement) submodelElementLevel);
        }
        else if (request.OutputModifier.Content == OutputContent.Reference) {
            var reference = _referenceModifierService.GetReferenceResult(submodelElement);
            response.Reference = _mapper.Map<ReferenceDTO>(reference);
        }
        else if (request.OutputModifier.Content == OutputContent.Path) {
            var path = _pathModifierService.ToIdShortPath(submodelElement);
            response.Path = ToIdShortDotSeparatedPath(path);
        }
        else if (request.OutputModifier.Content == OutputContent.Value) {
             var submodelElementLevel = _levelExtentModifierService.ApplyLevelExtent(submodelElement, (LevelEnum)request.OutputModifier.Level);
            var smeValues = _mappingService.Map(submodelElementLevel, "value");
            throw new NotImplementedException(); // TODO Transferring values still WIP
        }
        else {
            response.StatusCode = 400;
        }

        return Task.FromResult(response);
    }

    public override Task<PutSubmodelRpcResponse> PutSubmodel(PutSubmodelRpcRequest request, ServerCallContext context)
    {
        ISubmodel submodel = _mapper.Map<Submodel>(request.Submodel);
        var decodedSubmodelIdentifier = _decoderService.Decode("submodelIdentifier", request.SubmodelId);

        var response = new PutSubmodelRpcResponse();

        try {
            _submodelService.ReplaceSubmodelById(decodedSubmodelIdentifier, submodel);
            response.StatusCode = 200;
            response.Submodel = _mapper.Map<SubmodelDTO>(request.Submodel);
        }
        catch (NotFoundException) {
            response.StatusCode = 404;
        }
        catch (MetamodelVerificationException) {
            response.StatusCode = 400;
        }

        return Task.FromResult(response);
    }

    public override Task<PostSubmodelElementRpcResponse> PostSubmodelElement(PostSubmodelElementRpcRequest request, ServerCallContext context)
    {
        var decodedSubmodelIdentifier = _decoderService.Decode("submodelIdentifier", request.SubmodelId);
        _logger.LogInformation($"Received request to create a new submodel element in the submodel with id {decodedSubmodelIdentifier}");
        
        var body = _mapper.Map<ISubmodelElement>(request.SubmodelElement);

        PostSubmodelElementRpcResponse response = new();
        
        if (!Program.noSecurity)
        {
            var submodel = _submodelService.GetSubmodelById(decodedSubmodelIdentifier);
            var claimsList = new List<Claim>(context.GetHttpContext().User.Claims)
            {
                new Claim("IdShortPath", submodel.IdShort + "." + body.IdShort)
            };
            var identity = new ClaimsIdentity(claimsList, "AasSecurityAuth");
            var principal = new System.Security.Principal.GenericPrincipal(identity, null);
            var authResult = _authorizationService.AuthorizeAsync(principal, submodel, "SecurityPolicy").Result;
            if (!authResult.Succeeded)
            {
                response.StatusCode = 403;
                return Task.FromResult(response);
            }
        }

        try {
            var res = _submodelService.CreateSubmodelElement(decodedSubmodelIdentifier, body, true);
            response.StatusCode = 201;
            response.SubmodelElement = _mapper.Map<SubmodelElementDTO>(res);
        }
        catch (DuplicateException) {
            response.StatusCode = 403;
        }

        return Task.FromResult(response);
    }

    public override Task<PostSubmodelElementByPathRpcResponse> PostSubmodelElementByPath(PostSubmodelElementByPathRpcRequest request, ServerCallContext context)
    {
        var decodedSubmodelIdentifier = _decoderService.Decode("submodelIdentifier", request.SubmodelId);
        string idShortPath = ToIdShortDotSeparatedPath(request.Path.Select(x => x.Value));
        
        var body = _mapper.Map<ISubmodelElement>(request.SubmodelElement);

        _logger.LogInformation($"Received request to create a new submodel element at {idShortPath} in the submodel with id {decodedSubmodelIdentifier}");
        
        PostSubmodelElementByPathRpcResponse response = new();
        
        if (!Program.noSecurity)
        {
            var submodel = _submodelService.GetSubmodelById(decodedSubmodelIdentifier);
            context.GetHttpContext().User.Claims.ToList().Add(new Claim("idShortPath", submodel.IdShort + "." + idShortPath));
            var claimsList = new List<Claim>(context.GetHttpContext().User.Claims)
            {
                new Claim("IdShortPath", submodel.IdShort + "." + idShortPath)
            };
            var identity = new ClaimsIdentity(claimsList, "AasSecurityAuth");
            var principal = new System.Security.Principal.GenericPrincipal(identity, null);
            var authResult = _authorizationService.AuthorizeAsync(principal, submodel, "SecurityPolicy").Result;
            if (!authResult.Succeeded)
            {
                response.StatusCode = 401;
                return Task.FromResult(response);
            }
        }

        try {
            var res = _submodelService.CreateSubmodelElementByPath(decodedSubmodelIdentifier, idShortPath, true, body);
            response.StatusCode = 201;
            response.SubmodelElement = _mapper.Map<SubmodelElementDTO>(res);
        }
        catch (DuplicateException) {
            response.StatusCode = 401;
        }

        return Task.FromResult(response);
    }

    public override Task<PutSubmodelElementByPathRpcResponse> PutSubmodelElementByPath(PutSubmodelElementByPathRpcRequest request, ServerCallContext context)
    {
        var decodedSubmodelIdentifier = _decoderService.Decode("submodelIdentifier", request.SubmodelId);
        string idShortPath = ToIdShortDotSeparatedPath(request.Path.Select(x => x.Value));
        
        var body = _mapper.Map<ISubmodelElement>(request.SubmodelElement);

        _logger.LogInformation($"Received request to replace a submodel element at {idShortPath} deom the submodel with id {decodedSubmodelIdentifier}.");
        
        PutSubmodelElementByPathRpcResponse response = new();
        
        if (!Program.noSecurity)
        {
            var submodel = _submodelService.GetSubmodelById(decodedSubmodelIdentifier);
            var claimsList = new List<Claim>(context.GetHttpContext().User.Claims)
            {
                new Claim("IdShortPath", submodel.IdShort + "." + body.IdShort)
            };
            var identity = new ClaimsIdentity(claimsList, "AasSecurityAuth");
            var principal = new System.Security.Principal.GenericPrincipal(identity, null);
            var authResult = _authorizationService.AuthorizeAsync(principal, submodel, "SecurityPolicy").Result;
            if (!authResult.Succeeded)
            {
                response.StatusCode = 401;
                return Task.FromResult(response);
            }
        }

        _submodelService.ReplaceSubmodelElementByPath(decodedSubmodelIdentifier, idShortPath, body);
        try {
            _submodelService.ReplaceSubmodelElementByPath(decodedSubmodelIdentifier, idShortPath, body);
            response.StatusCode = 200;
            response.SubmodelElement = _mapper.Map<SubmodelElementDTO>(request.SubmodelElement);
        }
        catch (NotFoundException) {
            response.StatusCode = 404;
        }
        catch (MetamodelVerificationException) {
            response.StatusCode = 400;
        }

        return Task.FromResult(response);
    }

    public override Task<SetSubmodelElementValueByPathRpcResponse> SetSubmodelElementValueByPath(SetSubmodelElementValueByPathRpcRequest request, ServerCallContext context)
    {
        // TODO: Setting values directly is still unfeatured
        throw new NotImplementedException();
    }

    public override Task<DeleteSubmodelElementByPathRpcResponse> DeleteSubmodelElementByPath(DeleteSubmodelElementByPathRpcRequest request, ServerCallContext context)
    {
        var decodedSubmodelIdentifier = _decoderService.Decode("submodelIdentifier", request.SubmodelId);
        string idShortPath = ToIdShortDotSeparatedPath(request.Path.Select(x => x.Value));
        DeleteSubmodelElementByPathRpcResponse response = new();
        if (!Program.noSecurity)
        {
            var submodel = _submodelService.GetSubmodelById(decodedSubmodelIdentifier);
            context.GetHttpContext().User.Claims.ToList().Add(new Claim("idShortPath", submodel.IdShort + "." + idShortPath));
            var claimsList = new List<Claim>(context.GetHttpContext().User.Claims)
            {
                new Claim("IdShortPath", submodel.IdShort + "." + idShortPath)
            };
            var identity = new ClaimsIdentity(claimsList, "AasSecurityAuth");
            var principal = new System.Security.Principal.GenericPrincipal(identity, null);
            var authResult = _authorizationService.AuthorizeAsync(principal, submodel, "SecurityPolicy").Result;
            if (!authResult.Succeeded)
            {
                response.StatusCode = 401;
                return Task.FromResult(response);
            }
        }

        _logger.LogInformation($"Received a request to delete a submodel element at {idShortPath} from submodel with id {decodedSubmodelIdentifier}");

        try {
            _submodelService.DeleteSubmodelElementByPath(decodedSubmodelIdentifier, idShortPath);
            response.StatusCode = 204;
        }
        catch (NotFoundException) {
            response.StatusCode = 404;
        }
        return Task.FromResult(response);
    }

    public override Task<InvokeOperationAsyncResponse> InvokeOperationAsync(InvokeOperationAsyncRequest request, ServerCallContext context)
    {
        var decodedSubmodelIdentifier = _decoderService.Decode("submodelIdentifier", request.SubmodelId);
        _logger.LogInformation($"Received request to invoke operation at {request.SubmodelId} from a submodel with id {decodedSubmodelIdentifier}");

        InvokeOperationAsyncResponse response = new();

        string idShortPath = ToIdShortDotSeparatedPath(request.Path.Select(x => x.Value));
        if (!Program.noSecurity)
        {
            var submodel = _submodelService.GetSubmodelById(decodedSubmodelIdentifier);
            context.GetHttpContext().User.Claims.ToList().Add(new Claim("idShortPath", submodel.IdShort + "." + idShortPath));
            var claimsList = new List<Claim>(context.GetHttpContext().User.Claims)
            {
                new Claim("IdShortPath", submodel.IdShort + "." + idShortPath)
            };
            var identity = new ClaimsIdentity(claimsList, "AasSecurityAuth");
            var principal = new System.Security.Principal.GenericPrincipal(identity, null);
            var authResult = _authorizationService.AuthorizeAsync(principal, submodel, "SecurityPolicy").Result;
            if (!authResult.Succeeded)
            {
                response.StatusCode = 403;
                return Task.FromResult(response);
            }
        }

        try {
            List<OperationVariable> inputArguments = request.InputArguments.Select(x => _mapper.Map<OperationVariable>(x)).ToList();
            List<OperationVariable> inoutputArguments = request.InoutputArguments.Select(x => _mapper.Map<OperationVariable>(x)).ToList();
            var handle = _submodelService.InvokeOperationAsync(decodedSubmodelIdentifier, idShortPath, inputArguments, inoutputArguments, request.Timestamp, request.RequestId);
            response.Payload = handle.HandleId;
            response.StatusCode = 200;
        }
        catch (NotFoundException) {
            response.StatusCode = 404;
        }
        
        return Task.FromResult(response);
    }

    public override Task<InvokeOperationSyncResponse> InvokeOperationSync(InvokeOperationSyncRequest request, ServerCallContext context)
    {
        var decodedSubmodelIdentifier = _decoderService.Decode("submodelIdentifier", request.SubmodelId);
        _logger.LogInformation($"Received request to invoke operation at {request.SubmodelId} from a submodel with id {decodedSubmodelIdentifier}");

        InvokeOperationSyncResponse response = new();

        string idShortPath = ToIdShortDotSeparatedPath(request.Path.Select(x => x.Value));
        if (!Program.noSecurity)
        {
            var submodel = _submodelService.GetSubmodelById(decodedSubmodelIdentifier);
            context.GetHttpContext().User.Claims.ToList().Add(new Claim("idShortPath", submodel.IdShort + "." + idShortPath));
            var claimsList = new List<Claim>(context.GetHttpContext().User.Claims)
            {
                new Claim("IdShortPath", submodel.IdShort + "." + idShortPath)
            };
            var identity = new ClaimsIdentity(claimsList, "AasSecurityAuth");
            var principal = new System.Security.Principal.GenericPrincipal(identity, null);
            var authResult = _authorizationService.AuthorizeAsync(principal, submodel, "SecurityPolicy").Result;
            if (!authResult.Succeeded)
            {
                response.StatusCode = 403;
                return Task.FromResult(response);
            }
        }

        try {
            List<OperationVariable> inputArguments = request.InputArguments.Select(x => _mapper.Map<OperationVariable>(x)).ToList();
            List<OperationVariable> inoutputArguments = request.InoutputArguments.Select(x => _mapper.Map<OperationVariable>(x)).ToList();
            var result = _submodelService.InvokeOperationSync(decodedSubmodelIdentifier, idShortPath, inputArguments, inoutputArguments, request.Timestamp, request.RequestId);
            response.StatusCode = 200;
        }
        catch (NotFoundException) {
            response.StatusCode = 404;
        }
        
        return Task.FromResult(response);
    }

    public override Task<GetOperationAsyncResultResponse> GetOperationAsyncResult(GetOperationAsyncResultRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"Received request get status/result from invocation {request.HandleId}");

        GetOperationAsyncResultResponse response = new();
        var result = OperationInvoker.GetAsyncResult(request.HandleId);

        _logger.LogInformation($"Invocation {request.HandleId} has current status {result.ExecutionState}");

        return Task.FromResult(response);
    }
}