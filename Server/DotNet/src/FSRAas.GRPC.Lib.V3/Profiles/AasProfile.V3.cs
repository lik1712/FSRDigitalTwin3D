namespace FSRAas.GRPC.Lib.V3.Profiles;

using AasCore.Aas3_0;
using AutoMapper;
using FSRAas.GRPC.Lib.V3;
using FSRAas.GRPC.Lib.V3.Common.Utils;
using FSRAas.GRPC.Lib.V3.Services;
using Google.Protobuf;
using Extension = AasCore.Aas3_0.Extension;

public class AssetAdministrationShellProfile : Profile {
    public AssetAdministrationShellProfile()
    {
        // Necessary because protobuf does not have a concept of 'null'
        CreateMap<string, string>().ConvertUsing(s => s ?? string.Empty);

        // Convert System.Byte[] to Google.Protobuf.ByteString
        CreateMap<byte[], ByteString>().ConvertUsing(bytes => ByteString.CopyFrom(bytes));
        CreateMap<ByteString, byte[]>().ConvertUsing(bytes => bytes.ToArray());

        // Map SubmodelElementDTO <-> DataElementDTO
        CreateMap<SubmodelElementDTO, DataElementDTO>();
        CreateMap<DataElementDTO, SubmodelElementDTO>();

        CreateDomainMappings();
        CreateModelMappings();
    }

    private void CreateDomainMappings() {
        CreateMap<IExtension, ExtensionDTO>();
        CreateMap<IAdministrativeInformation, AdministrativeInformationDTO>();
        CreateMap<IQualifier, QualifierDTO>();
        CreateMap<IAssetAdministrationShell, AssetAdministrationShellDTO>();
        CreateMap<IAssetInformation, AssetInformationDTO>();
        CreateMap<IResource, ResourceDTO>();
        CreateMap<ISpecificAssetId, SpecificAssetIdDTO>();
        CreateMap<ISubmodel, SubmodelDTO>();
        CreateMap<ISubmodelElement, SubmodelElementDTO>()
            .ConvertUsing((src, dst, context) => SubmodelElementMapping.Map(src, context.Mapper));
        CreateMap<IDataElement, DataElementDTO>().ConvertUsing((src, c, context) => {
            SubmodelElementDTO smModel = SubmodelElementMapping.Map(src, context.Mapper);
            return context.Mapper.Map<DataElementDTO>(smModel);
        });
        CreateMap<IOperationVariable, OperationVariableDTO>();
        CreateMap<IEventPayload, EventPayloadDTO>();
        CreateMap<IConceptDescription, ConceptDescriptionDTO>();
        CreateMap<IReference, ReferenceDTO>();
        CreateMap<IKey, KeyDTO>();
        CreateMap<IAbstractLangString, LangStringDTO>();
        CreateMap<IEmbeddedDataSpecification, EmbeddedDataSpecificationDTO>();
        CreateMap<ILevelType, LevelTypeDTO>();
        CreateMap<IValueReferencePair, ValueReferencePairDTO>();
        CreateMap<IValueList, ValueListDTO>();
        CreateMap<IDataSpecificationContent, DataSpecificationContentDTO>();

        CreateMap<IO.Swagger.Models.PagedResultPagingMetadata, PagedResultPagingMetadata>();
    }

    private void CreateModelMappings() {
        CreateMap<ExtensionDTO, Extension>()
            .AfterMap((x, y) => Nullability.SetEmptyPropertiesToNull(y));
        CreateMap<AdministrativeInformationDTO, AdministrativeInformation>()
            .AfterMap((x, y) => Nullability.SetEmptyPropertiesToNull(y));
        CreateMap<QualifierDTO, Qualifier>()
            .AfterMap((x, y) => Nullability.SetEmptyPropertiesToNull(y));
        CreateMap<AssetAdministrationShellDTO, AssetAdministrationShell>()
            .AfterMap((x, y) => Nullability.SetEmptyPropertiesToNull(y));
        CreateMap<AssetInformationDTO, AssetInformation>()
            .AfterMap((x, y) => Nullability.SetEmptyPropertiesToNull(y));
        CreateMap<ResourceDTO, Resource>()
            .AfterMap((x, y) => Nullability.SetEmptyPropertiesToNull(y));
        CreateMap<SpecificAssetIdDTO, SpecificAssetId>()
            .AfterMap((x, y) => Nullability.SetEmptyPropertiesToNull(y));
        CreateMap<SubmodelDTO, Submodel>()
            .AfterMap((x, y) => Nullability.SetEmptyPropertiesToNull(y));
        CreateMap<SubmodelElementDTO, ISubmodelElement>()
            .ConvertUsing((src, c, context) => SubmodelElementMapping.Map(src, context.Mapper) ?? c);
        CreateMap<DataElementDTO, IDataElement>().ConvertUsing((src, c, context) => {
            SubmodelElementDTO smModel = context.Mapper.Map<SubmodelElementDTO>(src);
            var sm = SubmodelElementMapping.Map(smModel, context.Mapper);
            return sm is IDataElement dataElement ? dataElement : c;
        });
        CreateMap<OperationVariableDTO, OperationVariable>()
            .AfterMap((x, y) => Nullability.SetEmptyPropertiesToNull(y));
        CreateMap<EventPayloadDTO, EventPayload>()
            .AfterMap((x, y) => Nullability.SetEmptyPropertiesToNull(y));
        CreateMap<ConceptDescriptionDTO, ConceptDescription>()
            .AfterMap((x, y) => Nullability.SetEmptyPropertiesToNull(y));
        CreateMap<ReferenceDTO, Reference>()
            .AfterMap((x, y) => Nullability.SetEmptyPropertiesToNull(y));
        CreateMap<KeyDTO, Key>()
            .AfterMap((x, y) => Nullability.SetEmptyPropertiesToNull(y));
        CreateMap<LangStringDTO, LangStringNameType>()
            .AfterMap((x, y) => Nullability.SetEmptyPropertiesToNull(y));
        CreateMap<LangStringDTO, LangStringDefinitionTypeIec61360>()
            .AfterMap((x, y) => Nullability.SetEmptyPropertiesToNull(y));
        CreateMap<LangStringDTO, LangStringPreferredNameTypeIec61360>()
            .AfterMap((x, y) => Nullability.SetEmptyPropertiesToNull(y));
        CreateMap<LangStringDTO, LangStringTextType>()
            .AfterMap((x, y) => Nullability.SetEmptyPropertiesToNull(y));
        CreateMap<EmbeddedDataSpecificationDTO, EmbeddedDataSpecification>()
            .AfterMap((x, y) => Nullability.SetEmptyPropertiesToNull(y));
        CreateMap<LevelTypeDTO, LevelType>()
            .AfterMap((x, y) => Nullability.SetEmptyPropertiesToNull(y));
        CreateMap<ValueReferencePairDTO, ValueReferencePair>()
            .AfterMap((x, y) => Nullability.SetEmptyPropertiesToNull(y));
        CreateMap<ValueListDTO, ValueList>()
            .AfterMap((x, y) => Nullability.SetEmptyPropertiesToNull(y));
        CreateMap<DataSpecificationContentDTO, DataSpecificationIec61360>()
            .AfterMap((x, y) => Nullability.SetEmptyPropertiesToNull(y));
    }
}

public class SubmodelElementMapping {
    public static SubmodelElementDTO Map(ISubmodelElement element, IRuntimeMapper mapper) {
        SubmodelElementDTO model = new() {
            Category = element.Category ?? "",
            IdShort = element.IdShort ?? "",
            SemanticId = mapper.Map<ReferenceDTO>(element.SemanticId),
            SubmodelElementType = SubmodelElementType.SubmodelElement
        };
        model.Extensions.AddRange(element.Extensions?.Select(x => mapper.Map<ExtensionDTO>(x)) ?? []);
        model.DisplayName.AddRange(element.DisplayName?.Select(x => mapper.Map<LangStringDTO>(x)) ?? []);
        model.Description.AddRange(element.Description?.Select(x => mapper.Map<LangStringDTO>(x)) ?? []);
        model.SupplementalSemanticIds.AddRange(element.SupplementalSemanticIds?.Select(x => mapper.Map<ReferenceDTO>(x)) ?? []);
        model.Qualifiers.AddRange(element.Qualifiers?.Select(x => mapper.Map<QualifierDTO>(x)) ?? []);
        model.EmbeddedDataSpecifications.AddRange(element.EmbeddedDataSpecifications?.Select(x => mapper.Map<EmbeddedDataSpecificationDTO>(x)) ?? []);

        TypeSwitch.From(element)
            .Case<IRelationshipElement>(e => {
                RelationshipElementPayloadDTO dto = new() {
                    First = mapper.Map<ReferenceDTO>(e.First),
                    Second = mapper.Map<ReferenceDTO>(e.First)
                };
                model.SubmodelElementType = SubmodelElementType.RelationshipElement;
                model.RelationshipElement = dto;
            })
            .Case<ISubmodelElementList>(e => {
                SubmodelElementListPayloadDTO dto = new() {
                    OrderRelevant = e.OrderRelevant ?? true,
                    SemanticIdListElement = mapper.Map<ReferenceDTO>(e.SemanticIdListElement),
                    TypeValueListElement = (SubmodelElementType) e.TypeValueListElement,
                    ValueTypeListElement = (GRPC.Lib.V3.DataTypeDefXsd)(e.ValueTypeListElement ?? 0)
                };
                dto.Value.AddRange(e.Value?.Select(x => Map(x, mapper)) ?? []);
                model.SubmodelElementType = SubmodelElementType.SubmodelElementList;
                model.SubmodelElementList = dto;
            })
            .Case<ISubmodelElementCollection>(e => {
                SubmodelElementCollectionPayloadDTO dto = new() {  };
                dto.Value.AddRange(e.Value?.Select(x => Map(x, mapper)) ?? []);
                model.SubmodelElementType = SubmodelElementType.SubmodelElementCollection;
                model.SubmodelElementCollection = dto;
            })
            .Case<IDataElement>(_ => {
                DataElementPayloadDTO dto = new() {  };
                model.SubmodelElementType = SubmodelElementType.DataElement;
                model.DataElement = dto;
            })
            .Case<IEntity>(e => {
                EntityPayloadDTO dto = new() {
                    EntityType = (GRPC.Lib.V3.EntityType)e.EntityType,
                    GlobalAssetId = e.GlobalAssetId
                };
                dto.Statements.AddRange(e.Statements?.Select(x => Map(x, mapper)) ?? []);
                dto.SpecificAssetIds.AddRange(e.SpecificAssetIds?.Select(x => mapper.Map<SpecificAssetIdDTO>(x)) ?? []);
                model.SubmodelElementType = SubmodelElementType.Entity;
                model.Entity = dto;
            })
            .Case<IEventElement>(e => {
                EventElementPayloadDTO dto = new() { };
                model.SubmodelElementType = SubmodelElementType.EventElement;
                model.EventElement = dto;
            })
            .Case<IOperation>(e => {
                OperationPayloadDTO dto = new() { };
                dto.InputVariables.AddRange(e.InputVariables?.Select(x => mapper.Map<OperationVariableDTO>(x)) ?? []);
                dto.OutputVariables.AddRange(e.OutputVariables?.Select(x => mapper.Map<OperationVariableDTO>(x)) ?? []);
                dto.InoutVariables.AddRange(e.InoutputVariables?.Select(x => mapper.Map<OperationVariableDTO>(x)) ?? []);
                model.SubmodelElementType = SubmodelElementType.Operation;
                model.Operation = dto;
            })
            .Case<ICapability>(e => {
                CapabilityPayloadDTO dto = new() {  };
                model.SubmodelElementType = SubmodelElementType.Capability;
                model.Capability = dto;
            })
            .Case<IAnnotatedRelationshipElement>(e => {
                AnnotatedRelationshipElementPayloadDTO dto = new() { };
                dto.Annotations.AddRange(e.Annotations?.Select(x => mapper.Map<DataElementDTO>(x)) ?? []);
            })
            .Case<IProperty>(e => {
                PropertyPayloadDTO dto = new() {
                    ValueType = (GRPC.Lib.V3.DataTypeDefXsd)e.ValueType,
                    Value = e.Value,
                    ValueId = mapper.Map<ReferenceDTO>(e.ValueId)
                };
                model.SubmodelElementType = SubmodelElementType.Property;
                model.Property = dto;
            })
            .Case<IMultiLanguageProperty>(e => {
                MultiLanguagePropertyPayloadDTO dto = new() {
                    ValueId = mapper.Map<ReferenceDTO>(e.ValueId),
                };
                dto.Value.AddRange(e.Value?.Select(x => mapper.Map<LangStringDTO>(x)) ?? []);
                model.SubmodelElementType = SubmodelElementType.MultiLanguageProperty;
                model.MultiLanguageProperty = dto;
            })
            .Case<IRange>(e => {
                RangePayloadDTO dto = new() {
                    ValueType = (GRPC.Lib.V3.DataTypeDefXsd)e.ValueType,
                    Min = e.Min,
                    Max = e.Min
                };
                model.SubmodelElementType = SubmodelElementType.Range;
                model.Range = dto;
            })
            .Case<IReferenceElement>(e => {
                ReferenceElementPayloadDTO dto = new() {
                    Value = mapper.Map<ReferenceDTO>(e.Value)
                };
                model.SubmodelElementType = SubmodelElementType.ReferenceElement;
                model.ReferenceElement = dto;
            })
            .Case<IBlob>(e => {
                BlobPayloadDTO dto = new() {
                    Value = ByteString.CopyFrom(e.Value),
                    ContentType = e.ContentType
                };
                model.SubmodelElementType = SubmodelElementType.Blob;
                model.Blob = dto;
            })
            .Case<IFile>(e => {
                FilePayloadDTO dto = new() {
                    Value = e.Value,
                    ContentType = e.ContentType
                };
                model.SubmodelElementType = SubmodelElementType.File;
                model.File = dto;
            })
            .Case<IBasicEventElement>(e => {
                BasicEventElementPayloadDTO dto = new() {
                    Observed = mapper.Map<ReferenceDTO>(e.Observed),
                    Direction = (GRPC.Lib.V3.Direction)e.Direction,
                    State = (GRPC.Lib.V3.StateOfEvent)e.State,
                    MessageTopic = e.MessageTopic,
                    MessageBroker = mapper.Map<ReferenceDTO>(e.MessageBroker),
                    LastUpdate = e.LastUpdate,
                    MinInterval = e.MinInterval,
                    MaxInterval = e.MaxInterval,
                };
                model.SubmodelElementType = SubmodelElementType.BasicEventElement;
                model.BasicEventElement = dto;
            });

        return model;
    }

    public static ISubmodelElement? Map(SubmodelElementDTO elementModel, IRuntimeMapper mapper) {
        ISubmodelElement? element = null;

        switch (elementModel.SubmodelElementType) {
            case SubmodelElementType.RelationshipElement: {
                Reference? first = mapper.Map<Reference>(elementModel.RelationshipElement.First);
                Reference? second = mapper.Map<Reference>(elementModel.RelationshipElement.Second);
                if (first == null || second == null)
                {
                    return null;
                }
                element = new RelationshipElement(first, second);
            }
            break;
            case SubmodelElementType.SubmodelElementList: {
                bool orderRelevant = elementModel.SubmodelElementList.OrderRelevant;
                Reference? semanticIdListElement = mapper.Map<Reference>(elementModel.SubmodelElementList.SemanticIdListElement);
                AasSubmodelElements typeValueListElement = (AasSubmodelElements)elementModel.SubmodelElementList.TypeValueListElement;
                List<ISubmodelElement> value = new();
                foreach (var x in elementModel.SubmodelElementList.Value) {
                    var mapped = Map(x, mapper);
                    if (mapped == null) {
                        return null;
                    } 
                    value.Add(mapped);
                }
                element = new SubmodelElementList(typeValueListElement)
                {
                    OrderRelevant = orderRelevant,
                    SemanticIdListElement = semanticIdListElement,
                    Value = value
                };
            }
            break;
            case SubmodelElementType.SubmodelElementCollection: {
                List<ISubmodelElement> value = new();
                foreach (var x in elementModel.SubmodelElementCollection.Value) {
                    var mapped = Map(x, mapper);
                    if (mapped == null) {
                        return null;
                    } 
                    value.Add(mapped);
                }
                element = new SubmodelElementCollection()
                {
                    Value = value
                };
            }
            break;
            case SubmodelElementType.Property: {
                AasCore.Aas3_0.DataTypeDefXsd valueType = (AasCore.Aas3_0.DataTypeDefXsd)elementModel.Property.ValueType;
                string? value = elementModel.Property.Value;
                Reference? valueId = mapper.Map<Reference>(elementModel.Property.ValueId);
                element = new Property(valueType) {
                    Value = value,
                    ValueId = valueId
                };
            }
            break;
            case SubmodelElementType.MultiLanguageProperty:
            {
                List<ILangStringTextType>? value = elementModel.MultiLanguageProperty.Value
                    .Select(x => mapper.Map<LangStringTextType>(x)).ToList<ILangStringTextType>();
                Reference? valueId = mapper.Map<Reference>(elementModel.MultiLanguageProperty.ValueId);
                element = new MultiLanguageProperty()
                {
                    Value = value,
                    ValueId = valueId
                };
            }
            break;
            case SubmodelElementType.Range:
            {
                AasCore.Aas3_0.DataTypeDefXsd valueType = (AasCore.Aas3_0.DataTypeDefXsd)elementModel.Range.ValueType;
                string? min = elementModel.Range.Min;
                string? max = elementModel.Range.Max;
                element = new Range(valueType) {
                    Min = min,
                    Max = max
                };
            }
            break;
            case SubmodelElementType.ReferenceElement:
            {
                Reference? value = mapper.Map<Reference>(elementModel.ReferenceElement.Value);
                element = new ReferenceElement() {
                    Value = value
                };
            }
            break;
            case SubmodelElementType.Blob:
            {
                byte[]? value = mapper.Map<byte[]>(elementModel.Blob.Value);
                string? contentType = elementModel.Blob.ContentType;
                if (contentType == null) {
                    return null;
                }
                element = new Blob(contentType) {
                    Value = value,
                };
            }
            break;
            case SubmodelElementType.File:
            {
                string? value = elementModel.File.Value;
                string? contentType = elementModel.File.ContentType;
                if (contentType == null) {
                    return null;
                }
                element = new File(contentType) {
                    Value = value
                };
            }
            break;
            case SubmodelElementType.AnnotatedRelationshipElement:
            {
                Reference? first = mapper.Map<Reference>(elementModel.AnnotatedRelationshipElement.First);
                Reference? second = mapper.Map<Reference>(elementModel.AnnotatedRelationshipElement.Second);
                if (first == null || second == null) {
                    return null;
                }
                List<IDataElement> annotations = new();
                foreach (var x in elementModel.AnnotatedRelationshipElement.Annotations) {
                    var mapped = mapper.Map<IDataElement>(x);
                    if (mapped == null) {
                        return null;
                    } 
                    annotations.Add(mapped);
                }
                element = new AnnotatedRelationshipElement(first, second) {
                    Annotations = annotations
                };
            }
            break;
            case SubmodelElementType.Entity:
            {
                AasCore.Aas3_0.EntityType entityType = (AasCore.Aas3_0.EntityType)elementModel.Entity.EntityType;
                string? globalAssetId = elementModel.Entity.GlobalAssetId;
                List<SpecificAssetId>? specificAssetIds = elementModel.Entity.SpecificAssetIds
                    .Select(x => mapper.Map<SpecificAssetId>(x)).ToList();
                List<ISubmodelElement> statements = new();
                foreach (var x in elementModel.Entity.Statements) {
                    var mapped = Map(x, mapper);
                    if (mapped == null) {
                        return null;
                    } 
                    statements.Add(mapped);
                }
                element = new Entity(entityType)
                {
                    Statements = statements,
                    GlobalAssetId = globalAssetId,
                    SpecificAssetIds = specificAssetIds?.ToList<ISpecificAssetId>()
                };
            }
            break;
            case SubmodelElementType.BasicEventElement:
            {
                Reference? observed = mapper.Map<Reference>(elementModel.BasicEventElement.Observed);
                AasCore.Aas3_0.Direction direction = (AasCore.Aas3_0.Direction)elementModel.BasicEventElement.Direction;
                AasCore.Aas3_0.StateOfEvent state = (AasCore.Aas3_0.StateOfEvent)elementModel.BasicEventElement.State;
                string? messageTopic = elementModel.BasicEventElement.MessageTopic;
                Reference? messageBroker = mapper.Map<Reference>(elementModel.BasicEventElement.MessageBroker);
                string? lastUpdate = elementModel.BasicEventElement.LastUpdate;
                string? minInterval = elementModel.BasicEventElement.MinInterval;
                string? maxInterval = elementModel.BasicEventElement.MaxInterval;
                if (observed == null) {
                    return null;
                }
                element = new BasicEventElement(observed, direction, state) {
                    MessageTopic = messageTopic,
                    MessageBroker = messageBroker,
                    LastUpdate = lastUpdate,
                    MinInterval = minInterval,
                    MaxInterval = maxInterval
                };
            }
            break;
            case SubmodelElementType.Operation:
            {
                var inputVariables = elementModel.Operation.InputVariables
                    .Select(x => mapper.Map<OperationVariable>(x)).ToList();
                var outputVariables = elementModel.Operation.OutputVariables
                    .Select(x => mapper.Map<OperationVariable>(x)).ToList();
                var inoutVariables = elementModel.Operation.InoutVariables
                    .Select(x => mapper.Map<OperationVariable>(x)).ToList();
                element = new Operation()
                {
                    InputVariables = inputVariables?.ToList<IOperationVariable>(),
                    OutputVariables = outputVariables?.ToList<IOperationVariable>(),
                    InoutputVariables = inoutVariables?.ToList<IOperationVariable>()
                };
            }
            break;
            case SubmodelElementType.Capability:
                // intentionally left empty
                break;

        } // END switch

        if (element == null) {
            return null;
        }
        element.Extensions ??= [];
        element.Extensions.AddRange(elementModel.Extensions?.Select(x => mapper.Map<Extension>(x)) ?? []);
        element.Category = elementModel.Category;
        element.IdShort = elementModel.IdShort;
        element.DisplayName ??= [];
        element.DisplayName.AddRange(elementModel.Extensions?.Select(x => mapper.Map<LangStringNameType>(x)) ?? []);
        element.Description ??= [];
        element.Description.AddRange(elementModel.Description?.Select(x => mapper.Map<LangStringTextType>(x)) ?? []);
        element.SemanticId = mapper.Map<Reference>(elementModel.SemanticId);
        element.SupplementalSemanticIds ??= [];
        element.SupplementalSemanticIds.AddRange(elementModel.SupplementalSemanticIds?.Select(x => mapper.Map<Reference>(x)) ?? []);
        element.Qualifiers ??= [];
        element.Qualifiers.AddRange(elementModel.Qualifiers?.Select(x => mapper.Map<Qualifier>(x)) ?? []);
        element.EmbeddedDataSpecifications ??= [];
        element.EmbeddedDataSpecifications.AddRange(elementModel.EmbeddedDataSpecifications?.Select(x => mapper.Map<EmbeddedDataSpecification>(x)) ?? []);

        Nullability.SetEmptyPropertiesToNull(element);

        return element;
    }
}
