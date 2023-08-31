using System;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(RosConnector))]
    public class PointCloudSubscriber : UnitySubscriber<MessageTypes.Sensor.PointCloud2>
    {
        private byte[] byteArray;
        private bool isMessageReceived = false;
        private bool isPointCloudTransformed = true;   
        private int size;

        private Vector3[] pcl;
        private Color[] pcl_color;

        public Color color = Color.white;

        int point_step;

        protected override void Start()
        {
            base.Start();
        }

        public void Update()
        {

            if (isMessageReceived)
            {
                PointCloudRendering();
                isMessageReceived = false;
            }


        }

        protected override void ReceiveMessage(PointCloud2 message)
        {

            //Debug.Log("MessageReceived");

            size = message.data.GetLength(0);

            byteArray = new byte[size];
            byteArray = message.data;

            point_step = (int)message.point_step;

            size = size / point_step;
            isMessageReceived = true;
            isPointCloudTransformed = false;
        }

        //点群の座標を変換
        void PointCloudRendering()
        {
            //Debug.Log("Pointcloud Rendered");
            pcl = new Vector3[size];
            pcl_color = new Color[size];

            int x_posi;
            int y_posi;
            int z_posi;

            float x;
            float y;
            float z;

            //この部分でbyte型をfloatに変換         
            for (int n = 0; n < size; n++)
            {
                x_posi = n * point_step + 0;
                y_posi = n * point_step + 4;
                z_posi = n * point_step + 8;

                x = BitConverter.ToSingle(byteArray, x_posi);
                y = BitConverter.ToSingle(byteArray, y_posi);
                z = BitConverter.ToSingle(byteArray, z_posi);

                Vector3 tempVector = new Vector3(x, z, y);
                tempVector = Quaternion.Euler(0, -90, 0) * tempVector;
                pcl[n] = tempVector;
                pcl_color[n] = new Color(color.r, color.g, color.b);
            }
        }

        public Vector3[] GetPCL()
        {
            return pcl;
        }

        public Color[] GetPCLColor()
        {
            return pcl_color;
        }

        public bool GetIsTransformed()
        {
            return isPointCloudTransformed;
        }

        public void SetIsTransformed(bool value)
        {
            isPointCloudTransformed = value;
        }
    }
}
