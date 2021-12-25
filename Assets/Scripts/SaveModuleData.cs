
using System;
using UnityEngine;


public class SaveModuleData : MonoBehaviour
{
    //Serialize専用Class
    SaveData saveData;

    //保存する変数
    string prefabPath;
    Vector3 position;
    Quaternion rotation;
    string oscMessage;

    // Start is called before the first frame update
    void Start()
    {
    }

    public string Save()
    {
        //現在のデータを取得
        prefabPath = this.gameObject.GetComponent<Module>().prefabPath;
        position = this.gameObject.GetComponent<RectTransform>().position;
        rotation = this.gameObject.GetComponent<RectTransform>().rotation;
        oscMessage = this.gameObject.GetComponent<Module>().oscMessage;

        saveData = new SaveData(prefabPath, position, rotation, oscMessage);
        return JsonUtility.ToJson(saveData, prettyPrint:true);
    }

    public SaveData GetSaveData()
    {
        //現在のデータを取得
        prefabPath = this.gameObject.GetComponent<Module>().prefabPath;
        position = this.gameObject.GetComponent<RectTransform>().position;
        rotation = this.gameObject.GetComponent<RectTransform>().rotation;
        oscMessage = this.gameObject.GetComponent<Module>().oscMessage;

        saveData = new SaveData(prefabPath, position, rotation, oscMessage);
        Debug.Log(oscMessage);
        return saveData;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


//データをJsonに変換して保存するためのClass
[System.Serializable]
public class SaveData : object {

	[SerializeField]
	private string prefabPath;
    [SerializeField]
	private Vector3 position;
    [SerializeField]
	private Quaternion rotation;
    [SerializeField]
	private string oscMessage;

    //Constructor
    public SaveData(string path, Vector3 pos, Quaternion quat, string str) {
        prefabPath = path;
        position = pos;
        rotation = quat;
        oscMessage = str;
    }
 
    //各変数をSetする関数
	public void SetPrefabPath(string path) {
		prefabPath = path;
	}
    public void SetPosition(Vector3 pos) {
		position = pos;
	}
    public void SetRotation(Quaternion quat) {
		rotation = quat;
	}
    public void SetOSCMessage(string str) {
		oscMessage = str;
	}
    
    //各変数をGetする関数
	public string GetPrefabPath() 
    {
		return prefabPath;
	}
    public Vector3 GetPosition() 
    {
		return position;
	}
    public Quaternion GetRotation()
    {
        return rotation;
    }
    public string GetOSCMessage()
    {
        return oscMessage;
    }
 
}