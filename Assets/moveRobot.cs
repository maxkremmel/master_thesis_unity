using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using RosSharp.RosBridgeClient;

public class moveRobot : MonoBehaviour
{
    private string tf_frame = "base_link";

    // Update is called once per frame
    void Update()
    {
        Transform odom = GameObject.Find(tf_frame).GetComponent<Transform>();

        if (odom != null)
        {
            Vector3 temp = new(odom.position.x, odom.position.y, odom.position.z);
            transform.SetPositionAndRotation(temp, odom.rotation);
        }
    }
}
