using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSettingsPanel : MonoBehaviour
{
    public GameObject ModulesPanel;
    public GameObject grid;
    public GameObject slot;
    public InputField inputModuleSize;
    public InputField inputColumnCount;
    public InputField inputRowCount;
    public InputField inputPosX;
    public InputField inputPosY;
    
    OSCClientScript clientComponent;
    GridLayoutGroup gridLayoutGroup;
    SlotGrid slotGrid;

    // Start is called before the first frame update
    void Start()
    {
        //OSCClientを取得
        clientComponent = GameObject.Find("OSCClient").GetComponent<OSCClientScript>();

        this.gameObject.SetActive(false);
        gridLayoutGroup = grid.GetComponent<GridLayoutGroup>();
        slotGrid = grid.GetComponent<SlotGrid>();

        inputModuleSize.text = PlayerPrefs.GetFloat("ModuleScale", 1.0f).ToString();
        inputColumnCount.text = PlayerPrefs.GetInt("ColumnCount", 3).ToString();
        inputRowCount.text = PlayerPrefs.GetInt("RowCount", 6).ToString();
        inputPosX.text = PlayerPrefs.GetFloat("PosX", 0.0f).ToString();
        inputPosY.text = PlayerPrefs.GetFloat("PosY", 0.0f).ToString();
    }

    public void SetModuleSize(float value)
    {
        if (clientComponent.moduleScale + value <= 0)
        {
            return;
        }

        clientComponent.moduleScale += value;
        PlayerPrefs.SetFloat("ModuleScale", clientComponent.moduleScale);
        slotGrid.cellSize = new Vector2(clientComponent.modulePerPixel*clientComponent.moduleScale, clientComponent.modulePerPixel*clientComponent.moduleScale);
        gridLayoutGroup.cellSize = slotGrid.cellSize;
        inputModuleSize.text = clientComponent.moduleScale.ToString("F2");
    }

    public void SetModuleSizeFromInputField()
    {
        if (float.Parse(inputModuleSize.text) <= 0)
        {
            return;
        }

        clientComponent.moduleScale = float.Parse(inputModuleSize.text);
        PlayerPrefs.SetFloat("ModuleScale", clientComponent.moduleScale);
        slotGrid.cellSize = new Vector2(clientComponent.modulePerPixel*clientComponent.moduleScale, clientComponent.modulePerPixel*clientComponent.moduleScale);

        gridLayoutGroup.cellSize = slotGrid.cellSize;
        slotGrid.SetColliderSize();

        inputModuleSize.text = clientComponent.moduleScale.ToString("F2");
    }

    //以下スロット数変化あり
    public void SetColumnCount(int value)
    {
        if (slotGrid.columnCount + value <= 0)
        {
            return;
        }
        slotGrid.columnCount += value;
        PlayerPrefs.SetInt("ColumnCount", slotGrid.columnCount);
        gridLayoutGroup.constraintCount = slotGrid.columnCount;
        inputColumnCount.text = slotGrid.columnCount.ToString();
        ChangeSlot(slotGrid.columnCount*slotGrid.rowCount);
    }

    public void SetColumnCountFromInputField()
    {
        if (int.Parse(inputColumnCount.text) <= 0)
        {
            return;
        }
        slotGrid.columnCount = int.Parse(inputColumnCount.text);
        PlayerPrefs.SetInt("ColumnCount", slotGrid.columnCount);
        gridLayoutGroup.constraintCount = slotGrid.columnCount;
        inputColumnCount.text = slotGrid.columnCount.ToString();
        ChangeSlot(slotGrid.columnCount*slotGrid.rowCount);
    }

    public void SetRowCount(int value)
    {
        if (slotGrid.rowCount + value <= 0)
        {
            return;
        }
        slotGrid.rowCount += value;
        PlayerPrefs.SetInt("RowCount", slotGrid.rowCount);
        inputRowCount.text = slotGrid.rowCount.ToString();
        ChangeSlot(slotGrid.columnCount*slotGrid.rowCount);
    }

    public void SetRowCountFromInputField()
    {
        if (int.Parse(inputRowCount.text) <= 0)
        {
            return;
        }
        slotGrid.rowCount = int.Parse(inputRowCount.text);
        PlayerPrefs.SetInt("RowCount", slotGrid.rowCount);
        inputRowCount.text = slotGrid.rowCount.ToString();
        ChangeSlot(slotGrid.columnCount*slotGrid.rowCount);
    }

    void ChangeSlot(int nsc)
    {
        int psc = slotGrid.slotList.Count;
        if (nsc > psc)
        {
            for (int i = 0; i < nsc - psc; i++)
            {
                GameObject s = (GameObject)Instantiate (slot);
                s.transform.SetParent (grid.transform, false);
                s.GetComponent<BoxCollider2D>().size = slotGrid.cellSize;
                slotGrid.slotList.Add(s);
            }
        }

        else if (nsc < psc)
        {
            for (int i = 0; i < psc - nsc; i++)
            {
                GameObject s = slotGrid.slotList[slotGrid.slotList.Count-1];
                Destroy(s);
                slotGrid.slotList.RemoveAt(slotGrid.slotList.Count-1);
            }
        }
    }

    public void SetPositionX(float value)
    {
        Vector2 gridpos = grid.GetComponent<RectTransform>().anchoredPosition;
        gridpos.x += value;
        grid.GetComponent<RectTransform>().anchoredPosition = gridpos;
        PlayerPrefs.SetFloat("PosX", gridpos.x);
        inputPosX.text = gridpos.x.ToString("F2");
    }

    public void SetPositionXFromInputField()
    {
        Vector3 gridpos = grid.GetComponent<RectTransform>().anchoredPosition;
        gridpos.x = float.Parse(inputPosX.text);
        grid.GetComponent<RectTransform>().anchoredPosition = gridpos;
        PlayerPrefs.SetFloat("PosX", gridpos.x);
        inputPosX.text = gridpos.x.ToString("F2");
    }
    public void SetPositionY(float value)
    {
        Vector2 gridpos = grid.GetComponent<RectTransform>().anchoredPosition;
        gridpos.y += value;
        grid.GetComponent<RectTransform>().anchoredPosition = gridpos;
        PlayerPrefs.SetFloat("PosY", gridpos.y);
        inputPosY.text = gridpos.y.ToString("F2");
    }

    public void SetPositionYFromInputField()
    {
        Vector3 gridpos = grid.GetComponent<RectTransform>().anchoredPosition;
        gridpos.y = float.Parse(inputPosY.text);
        grid.GetComponent<RectTransform>().anchoredPosition = gridpos;
        PlayerPrefs.SetFloat("PosY", gridpos.y);
        inputPosY.text = gridpos.y.ToString("F2");
    }

    public void SetIsActive()
    {
        if(this.gameObject.activeInHierarchy == false)
        {
            ModulesPanel.SetActive(false);
            this.gameObject.SetActive(true);
        }
        else
        {
            for (int i = 0; i < clientComponent.ModulesList.Count; i++)
            {
                clientComponent.ModulesList[i].GetComponent<Module>().SetSize();
            }
            PlayerPrefs.Save();
            this.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
