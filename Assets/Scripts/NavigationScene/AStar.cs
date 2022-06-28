using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AStar : MonoBehaviour
{

    public HexGrid gridMap;
    static List<HexCell> openSet = new List<HexCell>();
    static List<HexCell> closeSet = new List<HexCell>();
    HexCell startCell;
    HexCell targetCell;
    DrawPath drawPath;
    public Color displayColor = Color.red;
    static List<HexCell> path = new List<HexCell>(); //����·��
    static List<HexCell> exploredPath = new List<HexCell>(); //����̽������cell
    public Toggle mapEditor_toggle;
    public Toggle pathPlan_toggle;
    public HexMapCamera hexMapCamera;
    int clickCount; //��ȡ����������������յ�
    bool isEditing = false;
    bool isPathPlanning = true; //�Ƿ����·���滮

    public bool IsEditing
    {
        get
        {
            return isEditing;
        }
        set
        {
            isEditing = value;
            mapEditor_toggle.isOn = isEditing;
            if (isEditing == true)
            {
                pathPlan_toggle.isOn = false;
                hexMapCamera.IsRotating = false;
            }
                
        }
    }

    public bool IsPathPlanning
    {
        get
        {
            return isPathPlanning;
        }
        set
        {
            isPathPlanning = value;
            pathPlan_toggle.isOn = isPathPlanning; //��isPathPlanning�ı�֮��ͬʱ�ı��Ӧ��toggle
            //��pathplan�͹ر�mapeditor��rotate
            if (isPathPlanning == true)
            {
                mapEditor_toggle.isOn = false;
                hexMapCamera.IsRotating = false;
            }
                
        }
               
    }

    void Start()
    {
        drawPath = GetComponentInChildren<DrawPath>();
    }
    void Update()
    {
        //if(Input.GetMouseButtonDown(0) && !IsEditing && Input.touchCount <= 1 && IsPathPlanning)
        //{
        //    Debug.Log("sdfsd");
        //}
        if (Input.GetMouseButtonDown(0) && !IsEditing && Input.touchCount <= 1&&IsPathPlanning && !EventSystem.current.IsPointerOverGameObject())
        {
            //Debug.Log("touch_count:" + Input.touchCount);
            HandleInput();
        }
        if (IsEditing || !IsPathPlanning)
        {
            ErasePath();
            startCell = targetCell = null;
        }       
    }
    void FindPath(HexCell startCell, HexCell targetCell)
    {

        //Debug.Log("finding path...");
        openSet.Clear();
        closeSet.Clear();
        exploredPath.Clear();
        openSet.Add(startCell);
        startCell.gCost = 0;
        exploredPath.Add(startCell);
        int c = 0;
        while (openSet.Count > 0 && c <= HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ * gridMap.chunkCountX * gridMap.chunkCountZ)
        {
            HexCell currentCell = openSet[openSet.Count - 1];
            for (int i = openSet.Count - 1; i >= 0; i--)
            {
                if (openSet[i].fCost < currentCell.fCost ||(openSet[i].fCost == currentCell.fCost&&openSet[i].hCost < currentCell.hCost))
                {
                    currentCell = openSet[i];
                }
            }
            //PrintCor(currentCell);
            //Debug.Log($"cost: g:{currentCell.gCost},h{currentCell.hCost}");
            openSet.Remove(currentCell);
            closeSet.Add(currentCell);
            if (currentCell == targetCell)
            {
                Debug.Log("�����յ�");
                RetracePath(targetCell);
                return;
            }

            //��ȡ��ǰ����ھӵ�
            HexCell[] neighbors = currentCell.getNeighbors;
            if (neighbors == null)
            {
                Debug.Log("neighbor������");
            }
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (neighbors[i] == null)
                    continue;

                //Ϊ�ϰ�������Ѿ���closeSet��
                if (neighbors[i].Elevation != 0 || closeSet.Contains(neighbors[i]))
                {
                    continue;
                }

                int newgCost = currentCell.gCost + 1;
                int newhCost = GetManhattanDistance(neighbors[i], targetCell);
                //int newhCost = GetEulerDistance(neighbors[i], targetCell);


                if (openSet.Contains(neighbors[i])) //�ھ�cell�Ѿ���openSet������
                {
                    if (neighbors[i].fCost > newgCost + newhCost)
                    {
                        neighbors[i].gCost = newgCost;
                        neighbors[i].parentCell = currentCell;
                    }
                }
                else
                {
                    neighbors[i].hCost = newhCost;
                    neighbors[i].gCost = newgCost;
                    neighbors[i].parentCell = currentCell;
                    openSet.Add(neighbors[i]);
                    exploredPath.Add(neighbors[i]);
                }
            }
        }
        Debug.Log("û���ҵ�ͨ·");
        return;
    }

    int GetManhattanDistance(HexCell cell1, HexCell cell2)
    { // ��ȡ�����ε�ͼ�µ������پ���
        float x = Mathf.Abs(cell1.coordinates.X - cell2.coordinates.X);
        float z = Mathf.Abs(cell1.coordinates.Z - cell2.coordinates.Z);
        if (cell1.coordinates.X < cell2.coordinates.X && cell1.coordinates.Z > cell2.coordinates.Z)
            return (int)(x + z) - 1;
        else if (cell2.coordinates.X < cell1.coordinates.X && cell2.coordinates.Z > cell1.coordinates.Z)
            return (int)(x + z) - 1;
        else
            return (int)(x + z);
    }

    int GetEulerDistance(HexCell cell1, HexCell cell2)
    { //��ȡŷ������
        float x1 = cell1.postion_.x;
        float z1 = cell1.postion_.z;
        float x2 = cell2.postion_.x;
        float z2 = cell2.postion_.z;
        return (int)(Mathf.Sqrt((x1 - x2) * (x1 - x2) + (z1 - z2) * (z1 - z2)));
    }

    void RetracePath(HexCell targetCell)
    { //����·��
        //Debug.Log("·�����£�");
        //List<HexCell> path = new List<HexCell>();
        path.Clear();
        HexCell cell = targetCell;
        while (cell != null)
        {
            path.Add(cell);
            //Debug.Log($"({cell.coordinates.X},{cell.coordinates.Z})");
            cell = cell.parentCell;
        }
        DisplayPath();
    }

    void HandleInput()
    {

        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            if (clickCount == 0)
            {//�������
                Debug.Log($"pos:{hit.point}");
                startCell = gridMap.GetCell(hit.point);
                drawPath.StartPoint = hit.point;
                if(startCell.Elevation == 0)
                {
                    clickCount += 1;
                    startCell.Color = Color.red;
                }
                
            }
            else
            {
                targetCell = gridMap.GetCell(hit.point);
                drawPath.EndPoint = hit.point;
                drawPath.targetCell = targetCell;
                if (targetCell.Elevation != 0)
                    return;
                clickCount = 0;
                // Debug.Log("go find path");
                ErasePath();
                if (startCell != null && targetCell != null)
                {
                    //Debug.Log($"({startCell.coordinates.X},{startCell.coordinates.Z}),({targetCell.coordinates.X},{targetCell.coordinates.Z})");
                    FindPath(startCell, targetCell);
                }
                else
                {
                    Debug.LogError("�����յ�Ϊ�գ�");
                }


                startCell = null;
                targetCell = null;
            }

        }
    }

    void PrintCor(HexCell cell)
    {
        Debug.Log($"({cell.coordinates.X},{cell.coordinates.Z})");
    }

    /// <summary>
    /// ���õ���·����ʾ����
    /// </summary>
    void DisplayPath()
    {//���õ���·����ʾ����
        foreach (HexCell cell in exploredPath)
        {
            cell.Color = Color.green;
        }

        foreach (HexCell cell in path)
        { //�ߵ���cell�ı���ɫ
            cell.Color = displayColor;
        }
        drawPath.AddPositions();

    }

    /// <summary>
    /// ����һ�ε�·������
    /// </summary>
    void ErasePath()
    {
        
        if (exploredPath == null) return;
        foreach (HexCell cell in exploredPath)
        {
            cell.Color = Color.white;
            cell.gCost = int.MaxValue;
            cell.hCost = 0;
            cell.parentCell = null;
        }
        path.Clear();
        exploredPath.Clear();
        drawPath.isEnabled = false;
    }
    public void SetisEditing(Toggle toggle)
    {
        IsEditing = toggle.isOn;
        
    }
    public void SetisPathPlanning(Toggle toggle)
    {
        IsPathPlanning = toggle.isOn;
        if(isPathPlanning == false)
        {
            if(startCell!=null)
            {
                startCell.Color = Color.white;
            }
        }
        
    }
}
