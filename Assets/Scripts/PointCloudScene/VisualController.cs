using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualController : MonoBehaviour
{

    public GameObject pointcloud;
    public float rotationSensitivityX = 3.5f;
    public float rotationSensitivityY = 3.5f;
    public float rotationSensitivityZ = 0.01f;
    public float translateSensitivityX = 3.5f;
    public float translateSensitivityY = 3.5f;
    public float zoomRate = 5.0f;
    public float MouseWheelSensitivity = 10f;
    public bool isZooming = false;
    public bool isRotating = false;

    //�����ӳ�
    int timerScalingDelay = 2; //�ӳټ�������ǰ2֡������

    //old touch position
    Vector2 oTouchPos1;
    Vector2 oTouchPos2;



    // Update is called once per frame
    void Update()
    {
        pointcloud.GetComponentInChildren<MeshFilter>().sharedMesh.UploadMeshData(true);
#if UNITY_STANDALONE_WIN
        
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
            PCScale();
        if (Input.GetMouseButton(0))
        {
            PCRotate();
        }
        if (Input.GetMouseButton(2))
        {
            PCTranslate();
        }
#endif

#if UNITY_ANDROID

        if (isZooming && Input.touchCount == 2)
        {
            MobileZoom();
        }
        if (!isZooming || Input.touchCount != 2)
        {
            timerScalingDelay = 2; //�����Ų����󣬼���������
        }
        
        if (Input.touchCount == 1&&isRotating)
        {//��z����ת
            MobileRotateZ();
        }

        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved&&!isRotating)
        {
            MobileRotate();

        }

        if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved && !isZooming)
        {
            MobileTranslate();
        }


#endif
    }
#if UNITY_STANDALONE_WIN
    void PCRotate()
    {
        float rotationX = Input.GetAxis("Mouse X") * rotationSensitivityX;
        float rotationY = Input.GetAxis("Mouse Y") * rotationSensitivityY;
        if (Input.GetKey(KeyCode.LeftAlt))
        {//������alt������z����ת             
            transform.Rotate(0, 0, rotationX);
        }
        else
            transform.Rotate(rotationY, rotationX, 0);
    }

    void PCScale()
    {

        Camera.main.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * MouseWheelSensitivity;

    }

    void PCTranslate()
    {
        float translateX = Input.GetAxis("Mouse X") * translateSensitivityX;
        float translateY = Input.GetAxis("Mouse Y") * translateSensitivityY;

        transform.Translate(-translateX, -translateY, 0);
        //transform.Translate(10, 0, 0);
        //Debug.Log(Input.GetAxis("Mouse Y"));
    }
#endif
#if UNITY_ANDROID
    void MobileRotate()
    {
        float rotationX = Input.GetTouch(0).deltaPosition.x * rotationSensitivityX*Time.deltaTime;
        float rotationY = Input.GetTouch(0).deltaPosition.y * rotationSensitivityY*Time.deltaTime;
        pointcloud.transform.Rotate(-rotationY, -rotationX, 0);
    }
    void MobileTranslate()
    {
        var deltaposition = Input.GetTouch(0).deltaPosition*Time.deltaTime;
        Camera.main.transform.Translate(-deltaposition.x * translateSensitivityX, -deltaposition.y * translateSensitivityY, 0);
        //Debug.Log(Input.GetTouch(0).deltaPosition.x);
    }

    public void MobileZoom()
    {//���зŴ���С����


        //new touch position
        Vector2 nTouchPos1 = Input.GetTouch(0).position;
        Vector2 nTouchPos2 = Input.GetTouch(1).position;
        //new and old distance
        float nDis = Vector2.Distance(nTouchPos1, nTouchPos2);
        float oDis = Vector2.Distance(oTouchPos1, oTouchPos2);
        if (timerScalingDelay < 0)
        {
            //Debug.Log("Zoom");
            if (nDis > oDis)
            {//�Ŵ�

                Camera.main.fieldOfView -= (nDis - oDis) * zoomRate * Time.deltaTime;
            }
            else
                Camera.main.fieldOfView += (oDis - nDis) * zoomRate * Time.deltaTime;
        }
        else
        {
            timerScalingDelay -= 1;
        }
        oTouchPos1 = nTouchPos1;
        oTouchPos2 = nTouchPos2;

    }

    void MobileRotateZ()
    {//��Z����ת

        //��ȡ�������ĵ���Ļ����λ��
        Vector3 pointcloudPos = Camera.main.WorldToScreenPoint(pointcloud.transform.parent.position);
        pointcloudPos.z = 0;

        //��ָ����ָ���Լ���ָ����
        Vector3 fingerPos = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y,0);
        Vector3 fingerDirection = new(Input.GetTouch(0).deltaPosition.x, Input.GetTouch(0).deltaPosition.y,0);
        Vector3 relativeDirection = fingerPos - pointcloudPos;
        Vector3 cross = Vector3.Cross(relativeDirection, fingerDirection);
        //�ж�˳ʱ�������ʱ��
        float rotateZ = -cross.z*rotationSensitivityZ*Time.deltaTime;
        Debug.Log($"({cross.z})");
        pointcloud.transform.Rotate(0, 0, rotateZ);
    }
#endif

}
