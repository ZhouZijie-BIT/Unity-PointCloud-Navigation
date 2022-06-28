using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class HexMapEditor : MonoBehaviour
{
    public Color[] colors; //�ڱ༭����ѡ����ɫ

    public HexGrid hexGrid; 

    private Color activeColor;

    HexDirection dragDirection; //drag�ķ���


    int brushSize;
    int activeElevation; //ѡ��ʱ�ĸ߶�
    bool applyColor; //ѡ��ʱ�Ƿ���ɫ
    bool applyElevation = true;
    bool isEditing = false;
 
    void Awake()
    {
        SelectColor(0);
    }

    void Update()
    {
        //if (Input.GetButtonDown("Fire1") && !EventSystem.current.IsPointerOverGameObject())
        //if (Input.GetMouseButtonDown(0) && isEditing)
        if (Input.GetMouseButton(0) && isEditing&&Input.touchCount!=2 && !EventSystem.current.IsPointerOverGameObject()) 
        {
            //if (!EventSystem.current.IsPointerOverGameObject())
            //{
            //    HandleInput();
            //}
            //else
            //{
            //    GameObject _button = EventSystem.current.currentSelectedGameObject;
            //    Debug.Log(_button);
            //}
            HandleInput();

        }
        
    }


    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            HexCell currentCell = hexGrid.GetCell(hit.point);           
            EditCells(currentCell);
            
        }
    }
    void EditCells(HexCell center)
    { //ͬʱ�༭���cell���Դ����centerΪ����
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;

        //rΪ����ʱ�İ뾶��������Χ��cell
        for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + brushSize; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
        for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
        {
            for (int x = centerX - brushSize; x <= centerX + r; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }

    }
    void EditCell(HexCell cell)
    { //�����cell�ı༭����
        if (cell!=null)
        {
            if (applyColor)
            {
                cell.Color = activeColor;
            }
            if (applyElevation)
            {
                cell.Elevation = activeElevation;
            }
            
        }

    }

    public void SelectColor(int index)
    {
        applyColor = index >= 0;
        if (applyColor)
        {
            activeColor = colors[index];
        }
    }
    public void SetElevation(Slider slider)
    { //�������sliderֵת������
        activeElevation = (int)slider.value;
        //Debug.Log(activeElevation);
    }
    //public void SetApplyElevation(Toggle toggle)
    //{
    //    applyElevation = toggle.isOn;
    //}
    public void SetBrushSize(Slider slider)
    {
        brushSize = (int)slider.value;
    }
    public void SetisEditing(Toggle toggle)
    {
        isEditing = toggle.isOn;
    }

    public void ShowUI(Toggle toggle)
    { //�Ƿ���ʾ����UI
        hexGrid.ShowUI(toggle.isOn);
    }    

}
