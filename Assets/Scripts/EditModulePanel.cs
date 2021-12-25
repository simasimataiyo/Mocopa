using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditModulePanel : MonoBehaviour
{
    //moduleObjectにはmodule側からこのパネルを呼び出したときに代入される
    public GameObject moduleObject;

    public InputField InputOSCMessage;
    public InputField InputThresholdPoint;
    OSCClientScript clientComponent;
    Module module;
    

    // Start is called before the first frame update
    void Start()
    {
        //既に他ModuleのEditPanelが存在していたらApply()して消去する
        GameObject otherEditPanel = GameObject.Find("EditModulePanel");
        if (otherEditPanel != null)
        {
            EditModulePanel otherEditPanelComponent = otherEditPanel.GetComponent<EditModulePanel>();
            otherEditPanelComponent.Apply();
        }

        this.gameObject.name = "EditModulePanel";
        clientComponent = GameObject.Find("OSCClient").GetComponent<OSCClientScript>();
        module = moduleObject.GetComponent<Module>();
        InputOSCMessage.text = module.oscMessage;

        if(InputThresholdPoint != null)
        {
            InputThresholdPoint.text = moduleObject.GetComponent<Joystick>().GetThresholdPoint().ToString("F2");
        }
    }

    public void Rotate(float value)
    {
        module.SetRotation(value);
    }

    public void SetThresholdPoint()
    {
        Joystick joystick = moduleObject.GetComponent<Joystick>();
        joystick.SetThresholdPoint( float.Parse(InputThresholdPoint.text) );        
    }

    public void Apply()
    {
        module.oscMessage = InputOSCMessage.text;
        module.SetisEditing(false);
        Destroy(this.gameObject);
    }

    public void DeleteModule()
    {
        Destroy(moduleObject);
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
