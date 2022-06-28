using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell 
{

    public HexCoordinates coordinates ; //����������
    Color color; //��ɫ
    //int elevation = int.MinValue;//�߶�
    float elevation = float.MinValue;
    public RectTransform uiRect; //��cell��Ӧ��UI��ǩ

    public HexGridChunk chunk; //��cell���ڵ�chunk
    public Vector3 postion_ = new(0, 0, 0);

    int pointCount = 0;

    public int Elevation
    {
        get
        {
            return (int)(elevation*10);
        }
        set
        { //���ø߶�,valueΪ1-6����
            if (Mathf.Approximately(elevation, value / 10f))
            {
                return;
            }
            elevation = value/10f;
            Vector3 position = postion_;
            position.y = value * HexMetrics.elevationStep;
            postion_ = position;

            //����UI��ǩ�߶�
            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = value * -HexMetrics.elevationStep;
            uiRect.localPosition = uiPosition;
            
            Refresh();
        }
    }



    public Color Color
    {
        get
        {
            return color;
        }
        set
        {
            if (color == value)
            {
                return;
            }
            color = value;
            Refresh();
        }
    }

    //A*�㷨�õ�������ֵ
    public int hCost;
    public int gCost;
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
    public HexCell parentCell;


    //[SerializeField]
    HexCell[] neighbors = new HexCell[6];
    public HexCell[] getNeighbors
    {
        get
        {
            return neighbors;
        }
    }

    public int PointCount { get => pointCount; set => pointCount = value; }

    public HexCell GetNeighbor(HexDirection direction)
    {//��ȡ�ھ�
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {//�����ھ�
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }
    public HexEdgeType GetEdgeType(HexDirection direction)
    {//���ݷ���õ���÷����ھӵıߵ�����
        return HexMetrics.GetEdgeType(
            elevation, neighbors[(int)direction].elevation
        );
    }
    public HexEdgeType GetEdgeType(HexCell otherCell)
    { //ȷ��������cell��othercell֮��ıߵ�����
        return HexMetrics.GetEdgeType(
            elevation, otherCell.elevation
        );
    }
    void Refresh()
    { //����cellҪˢ��ʱ������ˢ��������chunk�����ھ����ڵ�chunk
        if (chunk)
        {
            chunk.Refresh();
            for (int i = 0; i < neighbors.Length; i++)
            {
                HexCell neighbor = neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk)
                {
                    neighbor.chunk.Refresh();
                }
            }
        }
    }

    void RefreshSelfOnly()
    { //����ˢ�¸�cell������chunk
        chunk.Refresh();
    }
   
}
