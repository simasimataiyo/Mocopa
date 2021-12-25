using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using OscJack;

public class OSCClientScript : MonoBehaviour
{

    OscClient client;
    bool isEditing = false;

    //編集モードのためのUI
    public GameObject EditButtons;
    public GameObject ModulesPanel;
    public GameObject GridSettingsPanel;
    public GameObject SlotGrid;

    //配置されているモジュールのリスト
    public List<GameObject> ModulesList = new List<GameObject>();
    Wrapper wrapper = new Wrapper();
    public GameObject modulesBoard;
    
    //2cmをピクセル数に変換した数
    public int modulePerPixel;

    // モジュールをワールド座標系に変換
    public float modulePerUnit;

    //modulePerPixelを1としたときのモジュールの拡大率
    public float moduleScale;

    // Start is called before the first frame update
    void Start()
    {
        EditButtons.SetActive(false);
        SlotGrid.SetActive(false);

        //modulePerPixelを計算して初期値にする
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            modulePerPixel = 224;
        }else if(Application.platform == RuntimePlatform.Android)
        {
            double mpp = GetDPI() * (2/2.54) * (double)720/Screen.width;
            modulePerPixel = (int)mpp;
        }

        modulePerUnit = (float)modulePerPixel / (1280/10) * (1280/(float)Screen.height);
        moduleScale = PlayerPrefs.GetFloat("ModuleScale", 1.0f);

        Debug.Log( modulePerPixel +","+ modulePerUnit+","+moduleScale);

        //サーバに接続
        try{
            string address = PlayerPrefs.GetString("ServerIPAddress", "");
            int port = PlayerPrefs.GetInt("ServerPortNumber", 0);
            client = new OscClient(address, port);
        }
        catch(Exception e)
        {
            Debug.Log("接続エラー");
            // シーン切り替え
            SceneManager.LoadScene("StartUpScene");
        }

        //ロード処理
        LoadModules();
        
    }

    float GetDPI()
    {
        AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
    
        AndroidJavaObject metrics = new AndroidJavaObject("android.util.DisplayMetrics");
        activity.Call<AndroidJavaObject>("getWindowManager").Call<AndroidJavaObject>("getDefaultDisplay").Call("getMetrics", metrics);
    
        return (metrics.Get<float>("xdpi") + metrics.Get<float>("ydpi")) * 0.5f;
    }

    //編集モードの切り替え
    public void SetisEdit()
    {
        isEditing = !isEditing;
        Debug.Log(isEditing);

        if(isEditing == true)
        {
            EditButtons.SetActive(true);
            SlotGrid.SetActive(true);
            for (int i = 0; i < ModulesList.Count; i++)
            {
                ModulesList[i].GetComponent<Module>().ShowOSCMsg(true);
            }
        }
        else
        {
            //既に他ModuleのEditPanelが存在していたらApply()して消去する
            GameObject otherEditPanel = GameObject.Find("EditModulePanel");
            if (otherEditPanel != null)
            {
                EditModulePanel otherEditPanelComponent = otherEditPanel.GetComponent<EditModulePanel>();
                otherEditPanelComponent.Apply();
            }

            SaveModules();

            for (int i = 0; i < ModulesList.Count; i++)
            {
                ModulesList[i].GetComponent<Module>().ShowOSCMsg(false);
            }
            SlotGrid.SetActive(false);
            ModulesPanel.SetActive(false);
            GridSettingsPanel.SetActive(false);
            EditButtons.SetActive(false);
        }
    }

    //OSCClientを返す
    public OscClient GetOSCClient()
    {
        return client;
    }

    //isEditingを返す
    public Boolean GetisEditing()
    {
        return isEditing;
    }

    //モジュール情報のセーブとロード
    //Save
    void SaveModules()
    {
        if (wrapper._saveModulesList.Count > 0)
        {
            wrapper._saveModulesList.Clear();
        }
        
        for (int i = 0; i < ModulesList.Count; i++)
        {
            wrapper._saveModulesList.Add( ModulesList[i].GetComponent<SaveModuleData>().GetSaveData() );
        }

        string json = JsonUtility.ToJson(wrapper, prettyPrint:true);
        File.WriteAllText(GetSaveFilePath(), json);
    }

    //Load
    void LoadModules()
    {
        if (GetJson() != null)
        {
            wrapper = JsonUtility.FromJson<Wrapper> ( GetJson() );
            //GameObject:ModuluesBoardを取得
            for (int i = 0; i < wrapper._saveModulesList.Count; i++)
            {
                GameObject prefab = (GameObject)Resources.Load ( wrapper._saveModulesList[i].GetPrefabPath() );
                GameObject module = Instantiate (prefab) as GameObject;
                Debug.Log(module);
                module.transform.SetParent (modulesBoard.transform, false);
                module.GetComponent<RectTransform>().position = wrapper._saveModulesList[i].GetPosition();
                module.GetComponent<RectTransform>().rotation = wrapper._saveModulesList[i].GetRotation();
                module.GetComponent<Module>().oscMessage = wrapper._saveModulesList[i].GetOSCMessage();
                ModulesList.Add(module);
            }
            Debug.Log(wrapper._saveModulesList.Count);
        }
        
    }

    private static string GetSaveFilePath()
    {
        string filePath = "ControllerData";
        //確認しやすいようにエディタではAssetsと同じ階層に保存
        //それ以外ではApplication.persistentDataPath以下に保存するように。
        #if UNITY_EDITOR
                filePath += ".json";
        #else
                filePath = Application.persistentDataPath + "/" + filePath;
        #endif
        return filePath;
    }

    private static string GetJson()
    {
        string json = null;
        //Jsonを保存している場所のパスを取得
        string filePath = GetSaveFilePath();

        //Jsonが存在するか調べてから取得し変換する
        if (File.Exists(filePath))
        {
            json = File.ReadAllText(filePath);
        }

        return json;
    }

    //backキーを押すとスタート画面に戻る
    void OnEscapeEnter()
    {
        if (client != null)
        {
            client.Dispose();
        }
        SceneManager.LoadScene("StartUpScene");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscapeEnter();
        }
    }

    private void OnDestroy() {
        if (client != null)
        {
            client.Dispose();
        }
    }
}

[System.Serializable]
class Wrapper
{
    [SerializeField]
    public List<SaveData> _saveModulesList = new List<SaveData>();
}