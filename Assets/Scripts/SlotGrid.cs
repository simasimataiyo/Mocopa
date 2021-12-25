using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotGrid : MonoBehaviour
{
    public GameObject slot;
    public Vector2 cellSize;
    public int columnCount;
    public int rowCount;
    public Vector2 pos;

    OSCClientScript clientComponent;
    GridLayoutGroup gridLayoutGroup;
    
    //配置されているスロットのリスト
    public List<GameObject> slotList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        clientComponent =  GameObject.Find("OSCClient").GetComponent<OSCClientScript>();
        gridLayoutGroup = GetComponent<GridLayoutGroup>();

        cellSize = new Vector2(clientComponent.modulePerPixel*clientComponent.moduleScale, clientComponent.modulePerPixel*clientComponent.moduleScale);
        columnCount = PlayerPrefs.GetInt("ColumnCount", 3);
        rowCount = PlayerPrefs.GetInt("RowCount", 6);
        pos.x = PlayerPrefs.GetFloat("PosX", 0.0f);
        pos.y = PlayerPrefs.GetFloat("PosY", 0.0f);

        gridLayoutGroup.cellSize = cellSize;
        gridLayoutGroup.constraintCount = columnCount;

        this.gameObject.GetComponent<RectTransform>().anchoredPosition = pos;

        for (int i = 0; i < columnCount*rowCount; i++)
        {
            GameObject s = (GameObject)Instantiate (slot);
            s.transform.SetParent (this.gameObject.transform, false);
            s.GetComponent<BoxCollider2D>().size = cellSize;
            slotList.Add(s);
        }
    }

    public void SetColliderSize()
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            slotList[i].GetComponent<BoxCollider2D>().size = cellSize;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
