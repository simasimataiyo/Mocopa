using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ModuleAddButton : MonoBehaviour
{
    public GameObject prefab;
    GameObject module;
    GameObject modulesBoard;
    
    OSCClientScript clientComponent;

    // Start is called before the first frame update
    void Start()
    {
        //OSCClientを取得
        GameObject OSCClient = GameObject.Find("OSCClient");
        clientComponent = OSCClient.GetComponent<OSCClientScript>();

        //GameObject:ModuluesBoardを取得
        modulesBoard = GameObject.Find("ModulesBoard");
    }
    
    public void AddModule()
    {
        module = (GameObject)Instantiate (prefab);
        module.transform.SetParent (modulesBoard.transform, false); 
        clientComponent.ModulesList.Add(module);
        Debug.Log(clientComponent.ModulesList.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
