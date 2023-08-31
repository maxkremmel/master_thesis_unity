using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBaseLink : MonoBehaviour
{
    public GameObject base_link = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (base_link == null)
        {
            base_link = GameObject.Find("/odom/base_link");
            if (base_link != null)
            {
                this.transform.SetParent(base_link.transform);
            }
        }
    }
}
