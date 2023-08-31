using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(RosConnector))]
    public class LandMarkPublisher : UnityPublisher<MessageTypes.Sensor.PointCloud2>
    {
        
        public string FrameId = "Unity";
        private MessageTypes.Sensor.PointCloud2 message;
        protected override void Start()
        {
            base.Start();
            InitializeMessage();
        }

        private void InitializeMessage()
        {
            message = new MessageTypes.Sensor.PointCloud2
            {
                header = new MessageTypes.Std.Header { frame_id = FrameId, seq = 0},
                height = 1,
                fields = new MessageTypes.Sensor.PointField[] {
                    new MessageTypes.Sensor.PointField("x", 0, 7, 1),
                    new MessageTypes.Sensor.PointField("y", 4, 7, 1),
                    new MessageTypes.Sensor.PointField("z", 8, 7, 1) },
                is_bigendian = true,
                point_step = 12,
                is_dense = false
            };
        }
        public void PubLMtoROS()
        {
            Transform odom = GameObject.Find("velodyne").GetComponent<Transform>();
            GameObject pointCloudObject = GameObject.Find("PointCloud");
            Mesh pointCloud = pointCloudObject.GetComponent<MeshFilter>().mesh;
            Mesh newLandMark = new();
            List<Vector3> vertices = new();

            for (int i = 0; i < pointCloud.vertices.Length; i++)
            {
                if (pointCloud.colors[i].b == 1f) // if point is blue;
                {
                    vertices.Add(pointCloud.vertices[i]);
                }
            }
            newLandMark.vertices = vertices.ToArray();

            int size = newLandMark.vertices.Length;
            message.width = (uint)size;
            message.row_step = message.width * message.point_step;
            byte[] byteArray = new byte[size*message.point_step];

            int x_posi;
            int y_posi;
            int z_posi;

            for (int i = 0; i < size; i++)
            {
                x_posi = i * (int)message.point_step + 0;
                y_posi = i * (int)message.point_step + 4;
                z_posi = i * (int)message.point_step + 8;

                Vector3 vert = pointCloudObject.transform.TransformPoint(newLandMark.vertices[i]);
                //Vector3 vert = newLandMark.vertices[i];
                vert = Quaternion.Euler(0, 90, 0) * vert;
                BitConverter.GetBytes(vert.x).CopyTo(byteArray, x_posi);
                BitConverter.GetBytes(vert.z).CopyTo(byteArray, y_posi);
                BitConverter.GetBytes(vert.y).CopyTo(byteArray, z_posi);
            }

            message.data = byteArray;
            message.header.Update();

            Publish(message);
        }

    }
}
