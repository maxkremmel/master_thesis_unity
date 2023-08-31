using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using RosSharp.RosBridgeClient;

public class PointCloudRenderer : MonoBehaviour
{
    public PointCloudSubscriber subscriber;
    public LandMarkPublisher publisher;

    // Mesh stores the positions and colours of every point in the cloud
    // The renderer and filter are used to display it
    Mesh mesh;
    MeshRenderer meshRenderer;
    MeshFilter mf;

    // The size, positions and colours of each of the pointcloud
    public float pointSize = 1.0f;
    public bool forceFreezMesh = false;
    private bool freezMesh = false;


    [Header("MAKE SURE THESE LISTS ARE MINIMISED OR EDITOR WILL CRASH")]
    private Vector3[] positions = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 1, 0) };
    private Color[] colours = new Color[] { new Color(0f, 0f, 0f), new Color(0f, 0f, 0f) };

    //public Transform offset; // Put any gameobject that faciliatates adjusting the origin of the pointcloud in VR.

    public bool inverse;
    public string tf_frame = "base_link";
   
    InputDevice leftHand;

    private bool lastPrimaryButtonState;

    private IEnumerator Start()
    {
        // Give all the required components to the gameObject
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        mf = gameObject.AddComponent<MeshFilter>();
        meshRenderer.material = new Material(Shader.Find("Point Cloud/Disk"));
        meshRenderer.material.SetFloat("_PointSize", pointSize);
        mesh = new Mesh
        {
            // Use 32 bit integer values for the mesh, allows for stupid amount of vertices (2,147,483,647 I think?)
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
        };

        //transform.SetPositionAndRotation(offset.position, offset.rotation);

        var characteristics = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        var devices = new List<InputDevice>();
        bool controllerFound = false;
        
        while (controllerFound == false)
        {
            InputDevices.GetDevicesWithCharacteristics(characteristics, devices);
            foreach (var device in devices)
            {
                if (device.name.Contains("HP Reverb"))
                {
                    leftHand = device;
                    Debug.Log($"{leftHand.name} : {leftHand.characteristics}");
                    controllerFound = true;
                }
                else
                {
                    Debug.Log("Left Controller not found.");
                }
            }
            yield return null;
        }
    }

  
    void UpdateMesh()
    {
        bool tempState = leftHand.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonState) && primaryButtonState || forceFreezMesh;

        if (lastPrimaryButtonState == false && tempState == true)
        {
            freezMesh = !freezMesh;
            if(freezMesh == false)//unfreez
            {
                //publish selected sub mesh to ROS
                publisher.PubLMtoROS();   
            }
        }
        lastPrimaryButtonState = tempState;
        

        if (!freezMesh)
        {
            
            Transform odom = GameObject.Find(tf_frame).GetComponent<Transform>();
            if (odom != null && !subscriber.GetIsTransformed())
            {
                positions = subscriber.GetPCL();

                Vector3 temp = new(odom.position.x, odom.position.y, odom.position.z);
                transform.SetPositionAndRotation(temp, odom.rotation);

                colours = subscriber.GetPCLColor();
                if (positions == null)
                {
                    return;
                }
                mesh.Clear();
                mesh.vertices = positions;
                mesh.colors = colours;
                int[] indices = new int[positions.Length];

                for (int i = 0; i < positions.Length; i++)
                {
                    indices[i] = i;
                }

                mesh.SetIndices(indices, MeshTopology.Points, 0);
                mf.mesh = mesh;

                subscriber.SetIsTransformed(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //transform.SetPositionAndRotation(offset.position, offset.rotation);   
        UpdateMesh();
    }
}
