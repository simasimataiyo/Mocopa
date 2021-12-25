// Joystick Module
// OSC.send(message, angle, dist, states)
// states 0:onDrag, 1:pointerEnter, 2:onBeginDrag, 3:onEndDrag, 4:pointerExit, 5,6:threshold point

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using OscJack;

public class Joystick : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Module module;
    OSCClientScript clientComponent;
    OscClient client;
    bool isHover = false;
    GameObject stick;

    float angle = 0.0f;
    float dist = 0.0f;
    float preDist = 0.0f;
    float thresholdDist = 0.2f;


    // Start is called before the first frame update
    void Start()
    {
        clientComponent =  GameObject.Find("OSCClient").GetComponent<OSCClientScript>();
        client = clientComponent.GetOSCClient();

        //ModuleコンポーネントのOSCMessageとArgumentsを設定
        module = this.gameObject.GetComponent<Module>();
        module.rays.Add( Quaternion.Euler( 0, 0, this.gameObject.transform.eulerAngles.z ) * new Vector2(0.5f, 0.5f) );
        module.rays.Add( Quaternion.Euler( 0, 0, this.gameObject.transform.eulerAngles.z ) * new Vector2(-0.5f, -0.5f) );

        // stickオブジェクトを取得
        stick = transform.Find("stick").gameObject;
    }

    void calcRotation(Vector3 pos, bool isMove)
    {
        // Vector3 cursorPosition = eventData.position;
        // cursorPosition.z = 1.0f;
        
        Vector2 position = Camera.main.ScreenToWorldPoint(pos);
        // モジュール中心とカーソル位置の角度（atan2）を求める
        float dx = position.x - transform.position.x;
        float dy = position.y - transform.position.y;
        float rad = Mathf.Atan2(dy, dx);
        angle = rad * Mathf.Rad2Deg;

        dist = Vector2.Distance(position, transform.position)/module.worldSize * 4.0f;

        if(dist < 1.0f && isMove){
            stick.transform.position = position;
        }
        else if(dist >= 1.0f && isMove)
        {
            dist = 1.0f;
            stick.transform.position = new Vector2( 
                transform.position.x + Mathf.Cos(rad) * module.worldSize/2.0f*0.5f,
                transform.position.y + Mathf.Sin(rad) * module.worldSize/2.0f*0.5f
            );
        }
    }

    public void SetThresholdPoint(float value)
    {
        thresholdDist = value;
    }

    public float GetThresholdPoint()
    {
        return thresholdDist;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("pointerenter");
        if (clientComponent.GetisEditing() == true)
        {
            return;
        }
        else
        {
            isHover = true;
            calcRotation(eventData.position, true);
            client.Send(module.oscMessage, angle, dist, 1.0f);
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("pointerexit");
        if (isHover == true)
        {
            isHover = false;
            calcRotation(eventData.position, false);
            client.Send(module.oscMessage, angle, dist, 4.0f);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        calcRotation(eventData.position,false);
        client.Send(module.oscMessage, angle, dist, 2.0f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(isHover == true){
            calcRotation(eventData.position,true);
            if(dist >= thresholdDist && preDist < thresholdDist)
            {
                client.Send(module.oscMessage, angle, dist, 5.0f);
                Debug.Log("threshold");
            }
            else if(dist <= thresholdDist && preDist > thresholdDist)
            {
                client.Send(module.oscMessage, angle, dist, 6.0f);
                Debug.Log("threshold");
            }
            else
            {
                client.Send(module.oscMessage, angle, dist, 0.0f);
            }
            preDist = dist;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("enddrag");
        if(isHover == true){
            calcRotation(eventData.position,false);
            client.Send(module.oscMessage, angle, dist, 3.0f);
            preDist = 0.0f;
        }
        stick.transform.position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
