using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Module : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{
    OSCClientScript clientComponent;
    private RectTransform rectTransform;
    private Canvas canvas;
    private Canvas canvas_ui;
    
    //モジュールが共通して持つOSCMessage, OSCArguments, ModuleAspect, Ray(Raycaster用), worldSize
    public string oscMessage;
    public List<Vector2> oscArguments = new List<Vector2>();
    public Vector2 ModuleAspect;
    public List<Vector2> rays = new List<Vector2>();

    public float worldSize = 0.0f; 

    //モジュール編集用パネルのPrefab読み込み
    public GameObject editPanelPrefab;
    Boolean isEditing;

    private GameObject OSCMsgText;

    //ドラッグアンドドロップ制御用CanvasGroup
    private CanvasGroup canvasGroup;

    //生成元Prefabのパス
    public string prefabPath;

    // Start is called before the first frame update
    void Start()
    {
        //GameObject:OSCClientを取得
        clientComponent = GameObject.Find("OSCClient").GetComponent<OSCClientScript>();

        //Component:RectTransformを取得
        rectTransform = GetComponent<RectTransform>();

        //GameObject:Canvasを取得
        canvas = GameObject.Find("Canvas_Modules").GetComponent<Canvas>();
        canvas_ui = GameObject.Find("Canvas_UI").GetComponent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        //GameObject:OSCMsgを取得
        OSCMsgText = transform.Find("OSCMsg").gameObject;
        OSCMsgText.GetComponent<Text>().text = oscMessage;

        //moduleのサイズを取得
        SetSize();
        isEditing = false;

        if(clientComponent.GetisEditing() == true)
        {
            ShowOSCMsg(true);
        }
        else if(clientComponent.GetisEditing() == false)
        {
            ShowOSCMsg(false);
        }
    }

    public Boolean GetisEditing()
    {
         return isEditing;
    }

    public void SetSize()
    {
        rectTransform.localScale = new Vector3(
            clientComponent.modulePerPixel/180.0f * clientComponent.moduleScale,
            clientComponent.modulePerPixel/180.0f * clientComponent.moduleScale,
            1.0f
        );

        Vector3[] v = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(v);
        worldSize = Vector3.Distance(v[1], v[0]);
    }

    public void SetisEditing(Boolean state)
    {
        isEditing = state;
        OSCMsgText.GetComponent<Text>().text = oscMessage;
    }

    public void SetRotation(float value)
    {
        transform.Rotate(0,0,value);
        for (int i = 0; i < rays.Count; i++)
        {
            rays[i] = Quaternion.Euler( 0, 0, 90 )*rays[i];
        }
    }

    public void ShowOSCMsg(Boolean state){
        if(state)
        {
            OSCMsgText.SetActive(true);
        }else{
            OSCMsgText.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clientComponent.GetisEditing() == true && isEditing == false && eventData.dragging == false)
        {
            GameObject editPanelObject = (GameObject)Instantiate (editPanelPrefab);
            EditModulePanel editPanelComponent = editPanelObject.GetComponent<EditModulePanel>();
            editPanelComponent.moduleObject = this.gameObject;
            editPanelObject.transform.SetParent (canvas_ui.transform, false);
            isEditing = true;
        }
        else
        {
            return;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (clientComponent.GetisEditing() == false || isEditing == true)
        {
            return;
        }
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (clientComponent.GetisEditing() == false || isEditing == true)
        {
            return;
        }

        rectTransform.anchoredPosition += eventData.delta/canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (clientComponent.GetisEditing() == false || isEditing == true)
        {
            return;
        }

        rectTransform.anchoredPosition += eventData.delta/canvas.scaleFactor;
        
        int counter = 0;
        Vector3 newpos = Vector3.zero;
        Vector3 hitpoint = Vector3.zero;

        for (int i = 0; i < rays.Count; i++)
        {
            Ray2D ray = new Ray2D(new Vector2(transform.position.x, transform.position.y) + (rays[i] * clientComponent.modulePerUnit * clientComponent.moduleScale), transform.forward);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            Debug.DrawRay (ray.origin, transform.forward, Color.red, 3.0f, false);
            if (hit.collider != null)
            {
                if(hitpoint != hit.transform.position)
                {
                    counter++;
                }
                hitpoint = hit.transform.position;
                newpos = newpos + hit.transform.position;
            }
        }

        if (counter == rays.Count)
        {
            Debug.Log("true"+counter.ToString()+","+ rays.Count.ToString());
            transform.position = newpos / rays.Count;
        }
        else
        {
            Debug.Log("false"+counter.ToString()+","+ rays.Count.ToString());
        }
        canvasGroup.blocksRaycasts = true;
    }
    
    // Update is called once per frame
    void Update()
    {
    }

    private void OnDestroy()
    {
        int moduleIndex = clientComponent.ModulesList.FindIndex(o => o == this.gameObject);
        clientComponent.ModulesList.RemoveAt(moduleIndex);
    }
}
