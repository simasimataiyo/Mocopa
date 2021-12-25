// ButtonSwitch Module
// OSC.send(message, states)
// states 0:pointerExit, 1:pointerEnter

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using OscJack;

public class ButtonSwitch : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Module module;
    OSCClientScript clientComponent;
    OscClient client;
    GameObject btn;
    Image image;
    Color btn_color;
    // Start is called before the first frame update
    void Start()
    {
        clientComponent =  GameObject.Find("OSCClient").GetComponent<OSCClientScript>();
        client = clientComponent.GetOSCClient();

        //ModuleコンポーネントのOSCMessageとArgumentsを設定
        module = this.gameObject.GetComponent<Module>();
        module.rays.Add( new Vector2(0,0) );

        //btnオブジェクトを取得
        btn = transform.Find("btn").gameObject;
        image = btn.GetComponent<Image>();
        btn_color = image.color;
    }

    //OSCMessageを送る用
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (clientComponent.GetisEditing() == true)
        {
            return;
        }
        else
        {
            float h,s,v;
            Color.RGBToHSV(btn_color, out h, out s, out v);
            image.color = Color.HSVToRGB(h,s,v-0.15f);
            client.Send(module.oscMessage, 1.0f);
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (clientComponent.GetisEditing() == true)
        {
            return;
        }
        else
        {
            image.color = btn_color;
            client.Send(module.oscMessage, 0.0f);
        }
        
    }

    // Update is called once per frame
    void Update()
    {       
    }
}
