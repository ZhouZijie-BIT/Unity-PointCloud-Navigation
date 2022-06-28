using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//����������������ص���
public static class HexMetrics
{


    public const float innerRadius = 0.5f;//�ڰ뾶

    public const float outerRadius = innerRadius * 1.1547f; //��뾶

    //public const float outerRadius = 10f; //��뾶
    //public const float innerRadius = outerRadius * 0.8660254f;//�ڰ뾶



    public const float solidFactor = 0.75f; //�ڲ���ɫ�ı���

    public const float blendFactor = 1f - solidFactor; //��Ե�����ɫ�ı���

    public const float elevationStep = 0.1f; //�߶�step

    public const int terracesPerSlope = 2; //ÿ��б����ƽ̨������

    public const int terraceSteps = terracesPerSlope * 2 + 1; // ƽ̨��step�������۱ߵ�����

    public const int chunkSizeX = 5, chunkSizeZ = 5; //��Ĵ�С

    //ˮƽ����ƽ̨����
    public const float horizontalTerraceStepSize = 1f / terraceSteps;

    //��ֱ������ƽ̨����
    public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1);

    static Vector3[] corners = { //��������λ��
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius)
    };


    public static Vector3 GetFirstCorner(HexDirection direction)
    { //���ݷ���ѡȡ����
        return corners[(int)direction];
    }

    public static Vector3 GetSecondCorner(HexDirection direction)
    {//���ض�Ӧ�����ϵ���һ������
        return corners[(int)direction + 1];
    }

    public static Vector3 GetFirstSolidCorner(HexDirection direction)
    { //���ض�Ӧ�����ϵĴ�ɫ����
        return corners[(int)direction] * solidFactor;
    }

    public static Vector3 GetSecondSolidCorner(HexDirection direction)
    {//���ض�Ӧ�����ϵ���һ����ɫ����
        return corners[(int)direction + 1] * solidFactor;
    }
    public static HexDirection Previous(this HexDirection direction)
    {//���� ����direction��ǰһ������
        return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
    }

    public static HexDirection Next(this HexDirection direction)
    {//���� ����direction�ĺ�һ������
        return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
    }
    public static Vector3 GetBridge(HexDirection direction)
    {
        return (corners[(int)direction] + corners[(int)direction + 1]) * blendFactor;
    }

    public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
    { //��б�±�ƽ̨�������Բ�ֵ
        float h = step * HexMetrics.horizontalTerraceStepSize;
        a.x += (b.x - a.x) * h;
        a.z += (b.z - a.z) * h;

        //(step+1)/2����Ϊ����ֻ��Ҫ������������yֵ
        float v = ((step + 1) / 2) * HexMetrics.verticalTerraceStepSize;
        a.y += (b.y - a.y) * v;
        return a;
    }

    public static Color TerraceLerp(Color a, Color b, int step)
    { //��ƽ̨��ɫ��ֵ
        float h = step * HexMetrics.horizontalTerraceStepSize;
        return Color.Lerp(a, b, h);
    }
    public static HexEdgeType GetEdgeType(float elevation1, float elevation2)
    { //���������߶��жϱߵ�����
        if (elevation1 == elevation2)
        {
            return HexEdgeType.Flat;
        }
        float delta = elevation2 - elevation1;
        //if (delta == 0.1f || delta == -0.1f)
        if (delta <= 0.1f && delta >= -0.1f)
        {
            return HexEdgeType.Slope;
        }
        return HexEdgeType.Cliff;
    }
}
