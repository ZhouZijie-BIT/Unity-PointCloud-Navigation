using UnityEngine;

public class HexGridChunk : MonoBehaviour
{
	//С���ӵļ��� chunk
	HexCell[] cells; 

	HexMesh hexMesh;
	Canvas gridCanvas;

	void Awake()
	{
		gridCanvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();

		cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
		//ShowUI(false); //Ĭ�ϲ���ʾ����UI
	}

	

	public void AddCell(int index, HexCell cell)
	{ //��chunk�����cell
		cells[index] = cell;
		//cell.transform.SetParent(transform, false);
		cell.chunk = this;
		cell.uiRect.SetParent(gridCanvas.transform, false);
	}
	public void Refresh()
	{ //�������ǻ�
		enabled = true;
	}
	void LateUpdate()
	{ //��Update()��������֮��ִ��
		hexMesh.Triangulate(cells);
		enabled = false;
	}

	public void ShowUI(bool visible)
	{
		gridCanvas.gameObject.SetActive(visible);
	}
}
