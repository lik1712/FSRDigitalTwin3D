using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using FSR.Aas.GRPC.Lib.V3;
using FSR.Aas.GRPC.Lib.V3.Services;
using FSR.Aas.GRPC.Lib.V3.Services.SubmodelService;
using FSR.DigitalTwin.Unity.Workspace.Digital;
using FSR.DigitalTwin.Unity.Workspace.Virtual.Sensors;
using FSR.DigitalTwin.Util;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

namespace FSR.DigitalTwin.Unity.Workspace.Virtual.Actors {

    /// <summary>
    /// A cobot is a simple kinematic (robot arm) consisting of (for now) rotational joints.
    /// Using a set of joints, the kinematic can be controlled and transformed internally and externally
    /// </summary>
    public class CobotKinematic : DigitalTwinComponent
    {
        [SerializeField] private List<JointSensorSource> joints;

        private readonly SubmodelElementFactory submodelElementFactory = new();

        public JointSensorSource[] Joints => joints.ToArray();
        public bool IsListening { get; set; } = false;

        private SubmodelElementDTO CreateJointOrientationSubmodelElement() {
            List<SubmodelElementDTO> jointProperties = new();
            for (int i = 0; i < joints.Count; i++) {
                JointSensorSource joint = joints[i];
                if (joint.JointType == EJointType.REVOLUTE) {
                    RevoluteJointSensorSource rotationalJoint = joint as RevoluteJointSensorSource;
                    var prop = submodelElementFactory.Create(SubmodelElementType.Property, null, rotationalJoint.Z.Value, DataTypeDefXsd.Float);
                    prop.IdShort = "joint_" + i + "_z";
                    jointProperties.Add(prop);
                }
            }
            var jointPropertyCollection = submodelElementFactory.Create(SubmodelElementType.SubmodelElementCollection, jointProperties);
            jointPropertyCollection.IdShort = "orientation_parameters";
            jointPropertyCollection.Description.Add(new LangStringDTO() { Language = "en", Text = "Orientation parameters of joints"});
            jointPropertyCollection.DisplayName.Add(new LangStringDTO() { Language = "en", Text = "Orientation Parameters"});

            return jointPropertyCollection;
        }

        public override async Task<int> OnSynchronizeDataAsync()
        {
            if (!await HasConnectionAsync()) {
                Debug.LogError("[Client]: Missing Operational submodel!");
                return -1;
            }
            GetSubmodelElementByPathRpcRequest request = new() {
                SubmodelId = Base64Converter.ToBase64(SubmodelId),
                OutputModifier = new OutputModifier() {
                    Level = OutputLevel.Deep, Content = OutputContent.Normal, Cursor = "", 
                    Extent = OutputExtent.WithoutBlobValue, Limit = 16
                }
            };
            request.Path.Add(new KeyDTO { Type = KeyTypes.SubmodelElementCollection, Value = "orientation_parameters" });
            var response = await DigitalWorkspace.ApiBridge.AasApi.Submodel.GetSubmodelElementByPathAsync(request);
            if (response.StatusCode >= 400) {
                Debug.LogError("[Client]: Unable to receive orientation!");
            }

            Hashtable jointProperties = new();
            foreach (SubmodelElementDTO sme in response.Payload.SubmodelElementCollection.Value) {
                jointProperties[sme.IdShort] = sme;
            }

            for (int i = 0; i < joints.Count; i++) {
                JointSensorSource joint = joints[i];
                if (joint.JointType == EJointType.REVOLUTE) {
                    if (!jointProperties.ContainsKey("joint_" + i + "_z")) 
                        continue;
                    SubmodelElementDTO sme = (SubmodelElementDTO) jointProperties["joint_" + i + "_z"];
                    float z = float.Parse(sme.Property.Value, new CultureInfo("de-DE")); // CultureInfo.InvariantCulture.NumberFormat);
                    if (joint.JointType == EJointType.REVOLUTE) {
                        (joint as RevoluteJointSensorSource).JointBody.SetDriveTarget(ArticulationDriveAxis.X, z);
                    }
                }
            }

            return 0;
        }

        public override async Task<int> OnPushDataAsync()
        {
            if (!await HasConnectionAsync()) {
                Debug.LogError("[Client]: Missing Operational submodel!");
                return -1;
            }
            PutSubmodelElementByPathRpcRequest request = new() {
                SubmodelId = Base64Converter.ToBase64(SubmodelId),
                SubmodelElement = CreateJointOrientationSubmodelElement(),
            };
            request.Path.Add(new KeyDTO { Type = KeyTypes.SubmodelElementCollection, Value = "orientation_parameters" });
            var response = await DigitalWorkspace.ApiBridge.AasApi.Submodel.PutSubmodelElementByPathAsync(request);
            if (response.StatusCode >= 400) {
                Debug.LogError("[Client]: Unable to upload orientation!");
            }
            return response.StatusCode;
        }

        void Start() {
            DigitalWorkspace.ApiBridge.ObserveConnected.Subscribe(async _ => {
                PostSubmodelElementRpcRequest request = new() {
                    SubmodelId = Base64Converter.ToBase64(SubmodelId),
                    SubmodelElement = CreateJointOrientationSubmodelElement()
                };
                var response = await DigitalWorkspace.ApiBridge.AasApi.Submodel.PostSubmodelElementAsync(request);
                if (response.StatusCode == 403) {
                    Debug.Log("[Client]: Initial joint orientation data already present...");
                }
            })
            .AddTo(this);

            Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
                .Where(_ => DigitalWorkspace.ApiBridge.IsConnected)
                .Subscribe(async _ =>  { 
                    if (IsListening) { 
                        await OnSynchronizeDataAsync(); 
                    } else {
                        await OnPushDataAsync();
                    } 
                })
                .AddTo(this);

        }
    }
}