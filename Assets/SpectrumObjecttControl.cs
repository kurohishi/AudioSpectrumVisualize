using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectrumObjecttControl : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject obj;

    [SerializeField] private int type = 0;
    [SerializeField] private int visible = 128;
    [SerializeField] private float angle = 0.0f;
    [SerializeField] private float scale = 10.0f;
    [SerializeField] private float baseHeight = 1.0f;

    private const int FFT_RESOLUTION = 128;
    private float[] spectram;
    private GameObject objectCenter;
    private GameObject[] objectLeft;
    private GameObject[] objectRight;
    private GameObject objectEnd;

    private Vector3 localScale;
    void Start()
    {
        Prepare();

        if(type == 0) LinerPosition();
        else if (type == 1) CirclePosition();
        else if (type == 2) SideCirclePosition();

        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
    }
    private void Prepare()
    {
        spectram = new float[FFT_RESOLUTION];
        objectLeft = new GameObject[spectram.Length];
        objectRight = new GameObject[spectram.Length];
        if (type == 2)
        {
            objectLeftBox = new GameObject[spectram.Length];
            objectRightBox = new GameObject[spectram.Length];
        }

        localScale = obj.transform.localScale;
    }
    private void LinerPosition()
    {
        obj.transform.position = transform.position;
        objectCenter = Instantiate(obj);
        objectCenter.name = "objectCenter";
        objectCenter.SetActive(true);
        objectCenter.transform.SetParent(transform);

        for (int i = 1; i < visible; i++)
        {
            obj.transform.position = transform.position + 1 * new Vector3(localScale.x + .1f, 0, 0) * i;
            objectLeft[i] = Instantiate(obj);
            objectLeft[i].name = "objectLeft" + i;
            objectLeft[i].SetActive(true);
            objectLeft[i].transform.SetParent(transform);

            obj.transform.position = transform.position - 1 * new Vector3(localScale.x + .1f, 0, 0) * i;
            objectRight[i] = Instantiate(obj);
            objectRight[i].name = "objectRight" + i;
            objectRight[i].SetActive(true);
            objectRight[i].transform.SetParent(transform);
        }
    }
    [SerializeField] private int circleRate = 2;
    [SerializeField] private float radius = 10f;
    private void CirclePosition()
    {
        var rad = Mathf.Deg2Rad * (0 * 360f / (visible * circleRate));
        obj.transform.position = transform.position + new Vector3(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius, 0);
        objectCenter = Instantiate(obj);
        objectCenter.name = "objectCenter";
        objectCenter.transform.SetParent(transform);
        objectCenter.transform.rotation = Quaternion.Euler(0, 0, 0);
        objectCenter.SetActive(true);

        //CreateCircleCube(0, ref objectCenter, 1);

        for (int i = 1; i < visible; i++)
        {
            rad = Mathf.Deg2Rad * (i * 360f / (visible * circleRate));

            obj.transform.position = transform.position + new Vector3(Mathf.Sin(-rad) * radius, Mathf.Cos(-rad) * radius, 0);
            objectLeft[i] = Instantiate(obj);
            objectLeft[i].name = "objectLeft" + i;
            objectLeft[i].transform.SetParent(transform);
            objectLeft[i].transform.rotation = Quaternion.Euler(0, 0, i * 360f / (visible * circleRate));
            objectLeft[i].SetActive(true);

            //CreateCircleCube(i, ref objectLeft[i], -1);

            obj.transform.position = transform.position + new Vector3(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius, 0);
            objectRight[i] = Instantiate(obj);
            objectRight[i].name = "objectRight" + i;
            objectRight[i].transform.SetParent(transform);
            objectRight[i].transform.rotation = Quaternion.Euler(0, 0, -i * 360f / (visible * circleRate));
            objectRight[i].SetActive(true);

            //CreateCircleCube(i, ref objectRight[i], 1);
        }

        rad = Mathf.Deg2Rad * (visible * 360f / (visible * circleRate));
        obj.transform.position = transform.position + new Vector3(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius, 0);
        objectEnd = Instantiate(obj);
        objectEnd.name = "objectEnd";
        objectEnd.transform.SetParent(transform);
        objectEnd.transform.rotation = Quaternion.Euler(0, 0, -visible * 360f / (visible * circleRate));
        objectEnd.SetActive(true);

        //CreateCircleCube(visible, ref objectEnd, 1);
    }
    private GameObject objectCenterBox;
    private GameObject[] objectLeftBox;
    private GameObject[] objectRightBox;
    private GameObject objectEndBox;
    [SerializeField] private float correction = 1.0f; //スプライトの場合は5.0fにする
    private void SideCirclePosition()
    {
        var rad = Mathf.Deg2Rad * (0 * 360f / (visible * circleRate));
        obj.transform.position = transform.position + new Vector3(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius, 0);

        objectCenterBox = new GameObject("CenterBox");
        objectCenterBox.transform.position = obj.transform.position;
        objectCenterBox.transform.SetParent(transform);

        objectCenter = Instantiate(obj);
        objectCenter.name = "objectCenter";
        objectCenter.SetActive(true);
        objectCenter.transform.SetParent(objectCenterBox.transform);
        objectCenter.transform.localPosition = Vector3.zero + new Vector3(0, objectCenter.transform.localScale.y / 2, 0);
        objectCenter.transform.rotation = Quaternion.Euler(0, 0, 0);

        //CreateSideCircleCube(0,ref objectCenterBox,ref objectCenter,1);

        for (int i = 1; i < visible; i++)
        {
            rad = Mathf.Deg2Rad * (i * 360f / (visible * circleRate));
            objectLeftBox[i] = new GameObject("LeftBox" + i);
            obj.transform.position = transform.position + new Vector3(Mathf.Sin(-rad) * radius, Mathf.Cos(-rad) * radius, 0);
            objectLeftBox[i].transform.position = obj.transform.position;
            objectLeftBox[i].transform.SetParent(transform);
            objectLeftBox[i].transform.localRotation = Quaternion.Euler(0, 0, i * 360f / (visible * circleRate));

            objectLeft[i] = Instantiate(obj);
            objectLeft[i].name = "objectLeft" + i;
            objectLeft[i].SetActive(true);
            objectLeft[i].transform.SetParent(objectLeftBox[i].transform);
            objectLeft[i].transform.localPosition = Vector3.zero + new Vector3(0, objectLeft[i].transform.localScale.y / 2, 0);
            objectLeft[i].transform.localRotation = Quaternion.identity;

            //CreateSideCircleCube(i, ref objectLeftBox[i],ref objectLeft[i],-1);

            objectRightBox[i] = new GameObject("RightBox" + i);
            obj.transform.position = transform.position + new Vector3(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius, 0);
            objectRightBox[i].transform.position = obj.transform.position;
            objectRightBox[i].transform.SetParent(transform);
            objectRightBox[i].transform.localRotation = Quaternion.Euler(0, 0, -i * 360f / (visible * circleRate));

            objectRight[i] = Instantiate(obj);
            objectRight[i].name = "objectRight" + i;
            objectRight[i].SetActive(true);
            objectRight[i].transform.SetParent(objectRightBox[i].transform);
            objectRight[i].transform.localPosition = Vector3.zero + new Vector3(0, objectRight[i].transform.localScale.y / 2, 0);
            objectRight[i].transform.localRotation = Quaternion.identity;

            //CreateSideCircleCube(i,ref objectRightBox[i],ref objectRight[i],1);
        }

        rad = Mathf.Deg2Rad * (visible * 360f / (visible * circleRate));
        objectEndBox = new GameObject("EndBox");
        obj.transform.position = transform.position + new Vector3(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius, 0);
        objectEndBox.transform.position = obj.transform.position;
        objectEndBox.transform.SetParent(transform);
        objectEndBox.transform.localRotation = Quaternion.Euler(0, 0, -visible * 360f / (visible * circleRate));

        objectEnd = Instantiate(obj);
        objectEnd.name = "objectEnd";
        objectEnd.SetActive(true);
        objectEnd.transform.SetParent(objectEndBox.transform);
        objectEnd.transform.localPosition = Vector3.zero + new Vector3(0, objectEnd.transform.localScale.y / 2, 0);
        objectEnd.transform.localRotation = Quaternion.identity;

        //CreateSideCircleCube(visible,ref objectEndBox,ref objectEnd,1);
    }

    private void CreateLineCube(int num, ref GameObject obj, int direction) 
    {
        this.obj.transform.position = transform.position + direction * new Vector3(localScale.x + .1f, 0, 0) * num;
        obj = Instantiate(this.obj);
        obj.SetActive(true);
        obj.transform.SetParent(transform);
    }
    private void CreateCircleCube(int num, ref GameObject obj, int direction) 
    {
        var rad = Mathf.Deg2Rad * (num * 360f / (visible * circleRate));

        this.obj.transform.position = transform.position + new Vector3(Mathf.Sin(direction * rad) * radius, Mathf.Cos(direction * rad) * radius, 0);
        obj = Instantiate(this.obj);
        obj.SetActive(true);
        obj.transform.SetParent(transform);
        obj.transform.rotation = Quaternion.Euler(0, 0, -direction * num * 360f / (visible * circleRate));
    }
    private void CreateSideCircleCube(int num, ref GameObject box, ref GameObject obj, int direction) 
    {
        var rad = Mathf.Deg2Rad * (num * 360f / (visible * circleRate));

        box = new GameObject("Box");
        this.obj.transform.position = transform.position + new Vector3(Mathf.Sin(direction * rad) * radius, Mathf.Cos(direction * rad) * radius, 0);
        box.transform.position = this.obj.transform.position;
        box.transform.SetParent(transform);
        box.transform.localRotation = Quaternion.Euler(0, 0, -direction * num * 360f / (visible * circleRate));

        obj = Instantiate(this.obj);
        obj.SetActive(true);
        obj.transform.SetParent(box.transform);
        obj.transform.localPosition = Vector3.zero + new Vector3(0, obj.transform.localScale.y / 2, 0);
        obj.transform.localRotation = Quaternion.identity;
    }

    void Update()
    {
        ScaleChange();
    }
    private void ScaleChange()
    {
        var direction = (int)(scale / Mathf.Abs(scale));
        var height = direction * baseHeight;

        audioSource.GetSpectrumData(spectram, 0, FFTWindow.Rectangular);
        objectCenter.transform.localScale = new Vector3(localScale.x, direction * (spectram[0] * scale + height), localScale.z);
        if (type == 2) objectCenter.transform.localPosition = new Vector3(0, direction * objectCenter.transform.localScale.y / 2 * correction, 0);

        for (int i = 1; i < visible; i++)
        {
            var current = (spectram[i] * scale + height);
            objectLeft[i].transform.localScale = new Vector3(localScale.x, direction * current, localScale.z);
            objectRight[i].transform.localScale = new Vector3(localScale.x, direction * current, localScale.z);

            if (type == 2)
            {
                objectLeft[i].transform.localPosition = new Vector3(0, direction * objectLeft[i].transform.localScale.y / 2 * correction, 0);
                objectRight[i].transform.localPosition = new Vector3(0, direction * objectRight[i].transform.localScale.y / 2 * correction, 0);
            }
        }

        if (type == 1) objectEnd.transform.localScale = new Vector3(localScale.x, direction * (spectram[visible - 1] * scale + height), localScale.z);
        if (type == 2)
        {
            objectEnd.transform.localScale = new Vector3(localScale.x, direction * (spectram[visible - 1] * scale + height), localScale.z);
            objectEnd.transform.localPosition = new Vector3(0, direction * objectEnd.transform.localScale.y / 2 * correction, 0);
        }
    }
}
