using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class HexGrid : MonoBehaviour
{
    public int chunkCountX = 30, chunkCountZ = 10; //�������
    int cellCountX;
    int cellCountZ;

    public HexCell cellPrefab; //cellԤ�Ƽ�

    public TextMeshProUGUI cellLabelPrefab; //cell��ǩԤ�Ƽ�
    public HexGridChunk chunkPrefab; //���Ԥ�Ƽ�

    public Color defaultColor = Color.white;
    public Color touchedColor = Color.magenta;

    HexCell[] cells; //���е�cells

    HexGridChunk[] chunks;



    public HexCell[] Cells
    {
        get
        {
            return cells;
        }
    }

    void Awake()
    {

        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;
        CreateChunks(); //������
        CreateCells(); //����cells

    }

    void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        for (int z = 0, i = 0; z < chunkCountZ; z++)
        {
            for (int x = 0; x < chunkCountX; x++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab, transform);
                
                chunk.transform.localPosition = new Vector3(0f, 0f, 0f);
            }
        }
    }
    
    void CreateCells()
    {
        cells = new HexCell[cellCountZ * cellCountX];

        for (int z = 0, i = 0; z < cellCountZ; z++)
        {
            for (int x = 0; x < cellCountX; x++)
            {
                CreateCell(x, z, i++);
            }
        }
#if UNITY_EDITOR
        TimeCount.t2 = System.DateTime.Now;
        if (TimeCount.GetSubSeconds(TimeCount.t1, TimeCount.t2) < 3.0)
        {
            Debug.Log($"t1={TimeCount.t1.ToString("yyyyMMddHH:mm:ss fff")}");
            Debug.Log($"t2={TimeCount.t2.ToString("yyyyMMddHH:mm:ss fff")}");
            Debug.Log($"����ʱ�䣺{TimeCount.GetSubSeconds(TimeCount.t1, TimeCount.t2)}");
        }
            
#endif
    }

    /// <summary>
    /// ����position���ض�Ӧcell
    /// </summary>
    public HexCell GetCell(Vector3 position)
    {//����λ��position��Ӧ��cell

        position = transform.InverseTransformPoint(position);//��������ϵ���ֲ�����ϵ

        HexCoordinates coordinates = HexCoordinates.FromPosition(position);//�õ�����������ϵ
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;//�õ�cell����
        //Debug.Log($"{coordinates.X},{coordinates.Y},{coordinates.Z})" );
        //Debug.Log("index: "+index);
        if (index < cellCountZ * cellCountX && index>=0)
            return cells[index];
        else return null;
    }
    public int GetCellIndex(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);//��������ϵ���ֲ�����ϵ

        HexCoordinates coordinates = HexCoordinates.FromPosition(position);//�õ�����������ϵ
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;//�õ�cell����
        //Debug.Log($"{coordinates.X},{coordinates.Y},{coordinates.Z})" );
        //Debug.Log("index: "+index);
        if (index < cellCountZ * cellCountX && index >= 0)
            return index;
        else return -1;
    }

    /// <summary>
    /// ͨ��cell�õ�������index
    /// </summary>
    public int GetCellIndex(HexCell cell)
    {
        int index = cell.coordinates.X + cell.coordinates.Z * cellCountX + cell.coordinates.Z / 2;
        return index; 
    }
    public HexCell GetCell(int index)
    {
        return cells[index];
    }
    public HexCell GetCell(HexCoordinates coordinates)
    { //�������ȡcell
        int z = coordinates.Z;
        if (z < 0 || z >= cellCountZ)
        { 
            return null;
        }
        int x = coordinates.X + z / 2;
        if (x < 0 || x >= cellCountX)
        {
            return null;
        }
        return cells[x + z * cellCountX];
    }

    void CreateCell(int x, int z, int i)
    { //����cell

        //�����cell�ľֲ�����(x,y,z)
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        //position.x = (x + z * 0.5f ) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        
        HexCell cell = cells[i] = new HexCell();
        cell.postion_ = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z); //�õ�cell������������
        cell.Color = defaultColor;
        cell.gCost = int.MaxValue;
        //if (x == 0 && z == 2)
        //{
            
        //}
        //Debug.Log($"test:{cells[0].postion_}");
        //HexCell cell1 = new HexCell();

        //cell.gameObject.SetActive(false);
        if (x > 0)
        { //���x > 0 �����������ھ�Ϊǰһ��cell
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }
        if (z > 0)
        {
            if ((z & 1) == 0)
            {//���z>0������ż���������ö����ھ�
                cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
                }
            }
            else
            { //z>0����Ϊ������
                cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                if (x < cellCountX - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
                }
            }
        }

        //������ʾ
        TextMeshProUGUI label = Instantiate<TextMeshProUGUI>(cellLabelPrefab);
        label.rectTransform.anchoredPosition =
            new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();
        cell.uiRect = label.rectTransform; //��UI������cell��
        cell.Elevation = 0;
        AddCellToChunk(x, z, cell);
    }

    void AddCellToChunk(int x, int z, HexCell cell)
    { //����cell��ӵ���Ӧ��chunk����
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];
        //��cell��chunk�еľֲ�����
        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
    }

    public void ShowUI(bool visible)
    { //�Ƿ���ʾ����UI
        for (int i = 0; i < chunks.Length; i++)
        {
            chunks[i].ShowUI(visible);
        }
    }


}
