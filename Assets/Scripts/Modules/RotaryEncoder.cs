// RotaryEncoder Module
// OSC.send(message, angle, deltaAngle, states)
// states 0:onDrag, 1:pointerEnter, 2:onBeginDrag, 3:onEndDrag, 4:pointerExit

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using OscJack;

public class RotaryEncoder : MonoBehaviour, IPointerEnterHandler,  IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerExitHandler
{
    Module module;
    OSCClientScript clientComponent;
    OscClient client;
    bool isHover = false;
    GameObject hand;

    float angle = 0;
    float preAngle = 0;
    float deltaAngle = 0;

    // Start is called before the first frame update
    void Start()
    {
        clientComponent =  GameObject.Find("OSCClient").GetComponent<OSCClientScript>();
        client = clientComponent.GetOSCClient();

        //ModuleコンポーネントのOSCMessageとArgumentsを設定
        module = this.gameObject.GetComponent<Module>();
        module.rays.Add( Quaternion.Euler( 0, 0, this.gameObject.transform.eulerAngles.z ) * new Vector2(0.5f, 0.5f) );
        module.rays.Add( Quaternion.Euler( 0, 0, this.gameObject.transform.eulerAngles.z ) * new Vector2(-0.5f, -0.5f) );

        // handオブジェクトを取得
        hand = transform.Find("hand").gameObject;
    }

    void calcRotation(PointerEventData eventData){
        Vector3 cursorPosition = eventData.position;
        cursorPosition.z = 1.0f;
        Vector2 cursorWorldPosition = Camera.main.ScreenToWorldPoint(cursorPosition);

        // モジュール中心とカーソル位置の角度（atan2）を求める
        float dx = cursorWorldPosition.x - transform.position.x;
        float dy = cursorWorldPosition.y - transform.position.y;
        float rad = Mathf.Atan2(dy, dx);
        angle = rad * Mathf.Rad2Deg;

        hand.transform.rotation = Quaternion.Euler( 0, 0, angle );
        deltaAngle = angle - preAngle;
        preAngle = angle;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (clientComponent.GetisEditing() == true)
        {
            return;
        }
        else
        {
            isHover = true;
            calcRotation(eventData);
            client.Send(module.oscMessage, angle, 0.0f, 1.0f);
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
            calcRotation(eventData);
            client.Send(module.oscMessage, angle, 0.0f, 4.0f);
            isHover = false;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(isHover == true){
            calcRotation(eventData);
            client.Send(module.oscMessage, angle, 0.0f, 2.0f);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(isHover == true){
            calcRotation(eventData);
            client.Send(module.oscMessage, angle, deltaAngle, 0.0f);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(isHover == true){
            calcRotation(eventData);
            client.Send(module.oscMessage, angle, deltaAngle, 3.0f);
            preAngle = 0.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
