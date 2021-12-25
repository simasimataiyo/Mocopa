using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClientSetup : MonoBehaviour
{
    public InputField InputIPAddress;
    public InputField InputPortNumber;

    // Start is called before the first frame update
    void Start()
    {
        //addressとport numberを読み込む
        string address = PlayerPrefs.GetString("ServerIPAddress");
        if (address != null)
        {
            InputIPAddress.text = address;
        }

        int port = PlayerPrefs.GetInt("ServerPortNumber");
        InputPortNumber.text = port.ToString();
    }

    // IPAddressとポート番号をセット
    public void setOSCSettings()
    {
        string address = InputIPAddress.text;
        int port = int.Parse(InputPortNumber.text);
        PlayerPrefs.SetString("ServerIPAddress", address);
        PlayerPrefs.SetInt("ServerPortNumber", port);
        PlayerPrefs.Save();
    }

    public void loadControllerScene()
    {
        setOSCSettings();
        // シーン切り替え
        SceneManager.LoadScene("ControllerScene");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
