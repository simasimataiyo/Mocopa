using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using OscJack;

public class Toggle : MonoBehaviour
{
    Module module;
    OSCClientScript clientComponent;
    OscClient client;
    Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        clientComponent =  GameObject.Find("OSCClient").GetComponent<OSCClientScript>();
        client = clientComponent.GetOSCClient();

        //ModuleコンポーネントのOSCMessageとArgumentsを設定
        slider = this.gameObject.GetComponent<Slider>();
        module = this.gameObject.GetComponent<Module>();
        module.rays.Add( new Vector2(0.0f, 0.0f) );    
    }

    public void OnValueChange()
    {
        if (clientComponent.GetisEditing() == true || module.GetisEditing() == true)
        {
            return;
        }
        else
        {
            client.Send(module.oscMessage, slider.value);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
