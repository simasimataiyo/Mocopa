// Slider Module
// OSC.send(message, value)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using OscJack;

public class SliderModule : MonoBehaviour
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
        module.rays.Add( Quaternion.Euler( 0, 0, this.gameObject.transform.eulerAngles.z ) * new Vector2(0.5f, 0) );
        module.rays.Add( Quaternion.Euler( 0, 0, this.gameObject.transform.eulerAngles.z ) * new Vector2(-0.5f, 0) );
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
