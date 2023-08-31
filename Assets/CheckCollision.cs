using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.XR;

public class CheckCollision : MonoBehaviour
{
    private MeshFilter pointCloudMeshFilter = null;
    private MeshCollider selectorCollider;
    private Mesh pointCloudMesh;
    private Color[] colors;

    public bool checkCollision = false;

    InputDevice rightHand;

    private IEnumerator Start()
    {
        while (pointCloudMeshFilter == null || selectorCollider == null)
        {
            pointCloudMeshFilter = gameObject.GetComponent<MeshFilter>();
            selectorCollider = GameObject.Find("PointCloudInteractor").GetComponent<MeshCollider>();
            yield return null; // Wait for the next frame
        }
        pointCloudMesh = pointCloudMeshFilter.sharedMesh;

        var characteristics = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        var devices = new List<InputDevice>();
        bool controllerFound = false;

        while (controllerFound == false)
        {
            InputDevices.GetDevicesWithCharacteristics(characteristics, devices);
            foreach (var device in devices)
            {
                if (device.name.Contains("HP Reverb"))
                {
                    rightHand = device;
                    Debug.Log($"{rightHand.name} : {rightHand.characteristics}");
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


    void Update()
    {
        bool tempState = rightHand.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonState) && primaryButtonState;
        
        if (tempState || checkCollision)
        {
            Debug.Log("selecting Feature");
            pointCloudMesh = pointCloudMeshFilter.sharedMesh;
            // Get the intersecting vertices
            colors = GetIntersectingVertices(pointCloudMesh, selectorCollider);

            pointCloudMesh.colors = colors;
            pointCloudMeshFilter.mesh = pointCloudMesh;
        }
    }

    private Color[] GetIntersectingVertices(Mesh mesh, MeshCollider collider)
    {
        Color[] colors = mesh.colors;
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Vector3 vert = gameObject.transform.TransformPoint(mesh.vertices[i]);
            if (IsInCollider(collider, vert))
            {
                colors[i].r = 0f; colors[i].g = 0f; colors[i].b = 1f;
            }
        }
        return colors;
    }

    private bool IsInCollider(Collider collider, Vector3 point)
    {
        if (collider.ClosestPoint(point) == point)
        {
            return true;
        }
        return false;
    }


    /*public bool IsInCollider(MeshCollider other, Vector3 point)
    https://answers.unity.com/questions/1600764/point-inside-mesh.html
    {
        Vector3 from = (Vector3.up * 5000f);
        Vector3 dir = (point - from).normalized;
        float dist = Vector3.Distance(from, point);
        //fwd      
        int hit_count = Cast_Till(from, point, other);
        //back
        dir = (from - point).normalized;
        hit_count += Cast_Till(point, point + (dir * dist), other);

        if (hit_count % 2 == 1)
        {
            return (true);
        }
        return (false);
    }*/

    /* int Cast_Till(Vector3 from, Vector3 to, MeshCollider other)
   {
       int counter = 0;
       Vector3 dir = (to - from).normalized;
       float dist = Vector3.Distance(from, to);
       bool Break = false;
       while (!Break)
       {
           Break = true;
           RaycastHit[] hit = Physics.RaycastAll(from, dir, dist);
           for (int tt = 0; tt < hit.Length; tt++)
           {
               if (hit[tt].collider == other)
               {
                   counter++;
                   from = hit[tt].point + dir.normalized * .001f;
                   dist = Vector3.Distance(from, to);
                   Break = false;
                   break;
               }
           }
       }
       return (counter);
   }*/

    /*Color[] GetIntersectingVertices(Mesh mesh, BoxCollider collider)
    {
        Color[] colors = mesh.colors;
        
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Vector3 vert = gameObject.transform.TransformPoint(mesh.vertices[i]);
            if (collider.bounds.Contains(vert))
            {
                colors[i].r = 0f; colors[i].g = 0f; colors[i].b = 1f;
            }
        }

        return colors;
    }*/
}
