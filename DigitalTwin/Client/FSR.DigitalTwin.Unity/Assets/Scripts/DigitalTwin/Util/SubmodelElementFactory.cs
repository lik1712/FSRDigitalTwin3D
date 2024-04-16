using System;
using System.Collections.Generic;
using System.Linq;
using FSR.Aas.GRPC.Lib.V3;
using Google.Protobuf;
using UnityEngine;

namespace FSR.DigitalTwin.Util
{

    public class SubmodelElementFactory {
        public SubmodelElementDTO Create(SubmodelElementType type, params object[] args) {
            switch (type) {
                case SubmodelElementType.AnnotatedRelationshipElement:
                    return CreateAnnotatedRelationshipElement((ReferenceDTO)args[0], (ReferenceDTO)args[1], (List<DataElementDTO>)args[2]);
                case SubmodelElementType.BasicEventElement:
                    return CreateBasicEventElement((ReferenceDTO)args[0], (Direction)args[1], (StateOfEvent)args[2], args[3].ToString(), (ReferenceDTO)args[4], args[5].ToString(), args[6].ToString(), args[7].ToString());
                case SubmodelElementType.Blob:
                    return CreateBlob((byte[])args[0], args[1].ToString());
                case SubmodelElementType.Capability:
                    return CreateCapability();
                case SubmodelElementType.DataElement:
                    return CreateDataElement(type, args); // You need to handle this one accordingly, as it doesn't take specific arguments
                case SubmodelElementType.Entity:
                    return CreateEntity((List<SubmodelElementDTO>)args[0], (EntityType)args[1], args[2].ToString(), (List<SpecificAssetIdDTO>)args[3]);
                case SubmodelElementType.EventElement:
                    return CreateEventElement();
                case SubmodelElementType.File:
                    return CreateFile(args[0].ToString(), args[1].ToString());
                case SubmodelElementType.MultiLanguageProperty:
                    return CreateMultiLanguageProperty((List<LangStringDTO>)args[0], (ReferenceDTO)args[1]);
                case SubmodelElementType.Operation:
                    return CreateOperation((List<OperationVariableDTO>)args[0], (List<OperationVariableDTO>)args[1], (List<OperationVariableDTO>)args[2]);
                case SubmodelElementType.Property:
                    return CreateProperty((ReferenceDTO)args[0], args[1].ToString(), (DataTypeDefXsd)args[2]);
                case SubmodelElementType.Range:
                    return CreateRange((DataTypeDefXsd)args[0], args[1].ToString(), args[2].ToString());
                case SubmodelElementType.ReferenceElement:
                    return CreateReferenceElement((ReferenceDTO)args[0]);
                case SubmodelElementType.RelationshipElement:
                    return CreateRelationshipElement((ReferenceDTO)args[0], (ReferenceDTO)args[1]);
                case SubmodelElementType.SubmodelElementList:
                    return CreateSubmodelElementList((bool)args[0], (ReferenceDTO)args[1], (SubmodelElementType)args[2], (DataTypeDefXsd)args[3], (List<SubmodelElementDTO>)args[4]);
                case SubmodelElementType.SubmodelElementCollection:
                    return CreateSubmodelElementCollection((List<SubmodelElementDTO>)args[0]);
                default:
                    Debug.LogError("Unsupported submodel element type: " + type);
                    return null;
            }
        }

        private static SubmodelElementDTO CreateAnnotatedRelationshipElement(ReferenceDTO first, ReferenceDTO second, List<DataElementDTO> annotations) {
            var sme = new SubmodelElementDTO() {
                SubmodelElementType = SubmodelElementType.AnnotatedRelationshipElement,
                AnnotatedRelationshipElement = new AnnotatedRelationshipElementPayloadDTO() {
                    First = first,
                    Second = second
                }
            };
            sme.AnnotatedRelationshipElement.Annotations.AddRange(annotations);
            return sme;
        }

        private static SubmodelElementDTO CreateBasicEventElement(ReferenceDTO observed, Direction direction, StateOfEvent state, string messageTopic, ReferenceDTO messageBroker, string lastUpdate, string minInterval, string maxInterval) {
            return new SubmodelElementDTO() {
                SubmodelElementType = SubmodelElementType.BasicEventElement,
                BasicEventElement = new BasicEventElementPayloadDTO() {
                    Observed = observed,
                    Direction = direction,
                    State = state,
                    MessageTopic = messageTopic,
                    MessageBroker = messageBroker,
                    LastUpdate = lastUpdate,
                    MinInterval = minInterval,
                    MaxInterval = maxInterval
                }
            };
        }

        private static SubmodelElementDTO CreateBlob(byte[] value, string contentType) {
            var sme = new SubmodelElementDTO() {
                SubmodelElementType = SubmodelElementType.Blob,
                Blob = new BlobPayloadDTO() { ContentType = contentType }
            };
            sme.Blob.Value = ByteString.CopyFrom(value);
            return sme;
        }

        private static SubmodelElementDTO CreateCapability() {
            return new SubmodelElementDTO() {
                SubmodelElementType = SubmodelElementType.Capability,
                Capability = new CapabilityPayloadDTO()
            };
        }


        private static SubmodelElementDTO CreateDataElement(SubmodelElementType type, params object[] args) {
            var sme = new SubmodelElementDTO() {
                SubmodelElementType = SubmodelElementType.DataElement,
                DataElement = new DataElementPayloadDTO()
            };
            return sme;
        }

        private static SubmodelElementDTO CreateEntity(List<SubmodelElementDTO> statements, EntityType entityType, string globalAssetId, List<SpecificAssetIdDTO> specificAssetIds) {
            var sme = new SubmodelElementDTO() {
                SubmodelElementType = SubmodelElementType.Entity,
                Entity = new EntityPayloadDTO() {
                    EntityType = entityType,
                    GlobalAssetId = globalAssetId,
                }
            };
            sme.Entity.Statements.AddRange(statements);
            sme.Entity.SpecificAssetIds.AddRange(specificAssetIds);
            return sme;
        }

        private static SubmodelElementDTO CreateEventElement() {
            return new SubmodelElementDTO() {
                SubmodelElementType = SubmodelElementType.EventElement,
                EventElement = new EventElementPayloadDTO()
            };
        }

        private static SubmodelElementDTO CreateFile(string value, string contentType) {
            return new SubmodelElementDTO() {
                SubmodelElementType = SubmodelElementType.File,
                File = new FilePayloadDTO() { Value = value, ContentType = contentType }
            };
        }

        private static SubmodelElementDTO CreateMultiLanguageProperty(List<LangStringDTO> values, ReferenceDTO valueId) {
            var sme = new SubmodelElementDTO() {
                SubmodelElementType = SubmodelElementType.MultiLanguageProperty,
                MultiLanguageProperty = new MultiLanguagePropertyPayloadDTO() { ValueId = valueId }
            };
            sme.MultiLanguageProperty.Value.AddRange(values);
            return sme;
        }

        private static SubmodelElementDTO CreateOperation(List<OperationVariableDTO> inputVariables, List<OperationVariableDTO> outputVariables, List<OperationVariableDTO> inoutVariables) {
            var sme = new SubmodelElementDTO() {
                SubmodelElementType = SubmodelElementType.Operation,
                Operation = new OperationPayloadDTO() {}
            };
            sme.Operation.InputVariables.AddRange(inputVariables);
            sme.Operation.InoutVariables.AddRange(inoutVariables);
            sme.Operation.OutputVariables.AddRange(outputVariables);
            return sme;
        }

        private static SubmodelElementDTO CreateProperty(ReferenceDTO valueId, string value, DataTypeDefXsd valueType) {
            return new SubmodelElementDTO() {
                SubmodelElementType = SubmodelElementType.Property,
                Property = new PropertyPayloadDTO() { ValueId = valueId, Value = value, ValueType = valueType }
            };
        }

        private static SubmodelElementDTO CreateRange(DataTypeDefXsd valueType, string min, string max) {
            return new SubmodelElementDTO() {
                SubmodelElementType = SubmodelElementType.Range,
                Range = new RangePayloadDTO() { ValueType = valueType, Min = min, Max = max }
            };
        }

        private static SubmodelElementDTO CreateReferenceElement(ReferenceDTO value) {
            return new SubmodelElementDTO() {
                SubmodelElementType = SubmodelElementType.ReferenceElement,
                ReferenceElement = new ReferenceElementPayloadDTO() { Value = value }
            };
        }

        private static SubmodelElementDTO CreateRelationshipElement(ReferenceDTO first, ReferenceDTO second) {
            return new SubmodelElementDTO() {
                SubmodelElementType = SubmodelElementType.RelationshipElement,
                RelationshipElement = new RelationshipElementPayloadDTO() {
                    First = first,
                    Second = second
                }
            };
        }

        private static SubmodelElementDTO CreateSubmodelElementList(bool orderRelevant, ReferenceDTO semanticIdListElement, SubmodelElementType typeValueListElement, DataTypeDefXsd valueTypeListElement, List<SubmodelElementDTO> value) {
            var sme = new SubmodelElementDTO() {
                SubmodelElementType = SubmodelElementType.SubmodelElementList,
                SubmodelElementList = new SubmodelElementListPayloadDTO() {
                    OrderRelevant = orderRelevant,
                    SemanticIdListElement = semanticIdListElement,
                    TypeValueListElement = typeValueListElement,
                    ValueTypeListElement = valueTypeListElement,
                }
            };
            sme.SubmodelElementList.Value.AddRange(value);
            return sme;
        }

        private static SubmodelElementDTO CreateSubmodelElementCollection(List<SubmodelElementDTO> value) {
            var sme = new SubmodelElementDTO() {
                SubmodelElementType = SubmodelElementType.SubmodelElementCollection,
                SubmodelElementCollection = new SubmodelElementCollectionPayloadDTO() { }
            };
            sme.SubmodelElementCollection.Value.AddRange(value);
            return sme;
        }

    }

} // namespace FSR.Aas.GRPC.Lib.V3.Util