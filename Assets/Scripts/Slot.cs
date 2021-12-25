using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour
{
    OSCClientScript clientComponent;

    // Start is called before the first frame update
    void Start()
    {
        //OSCClientを取得
        clientComponent = GameObject.Find("OSCClient").GetComponent<OSCClientScript>();

    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

}
