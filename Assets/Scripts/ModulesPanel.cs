using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModulesPanel : MonoBehaviour
{
    public GameObject GridSettingsPanel;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void SetIsActive()
    {
        if(this.gameObject.activeInHierarchy == false)
        {
            //既に他ModuleのEditPanelが存在していたらApply()して消去する
            GameObject otherEditPanel = GameObject.Find("EditModulePanel");
            if (otherEditPanel != null)
            {
                EditModulePanel otherEditPanelComponent = otherEditPanel.GetComponent<EditModulePanel>();
                otherEditPanelComponent.Apply();
            }
            GridSettingsPanel.SetActive(false);
            
            this.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
