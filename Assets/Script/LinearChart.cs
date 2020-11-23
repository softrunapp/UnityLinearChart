using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LinearChart : MonoBehaviour
{
    [SerializeField]
    private List<Vector3> rectPositions;
    public Sprite pointImage;
    public Transform content;
    private List<GameObject> points;
    private List<GameObject> lines;
    public bool drawAtStart = true;
    public Vector2 pointSize = new Vector2(10, 10);
    public float lineWidth = 2;
    public Color32 lineColor = new Color32(255, 0, 0, 255);
    [SerializeField]
    private bool hidePoint = false, hideLine = false;
    public bool hidePoints { get { return hidePoint; } set { hidePoint = value; DrawChart(); } }
    public bool hideLines { get { return hideLine; } set { hideLine = value; DrawChart(); } }
    // Start is called before the first frame update
    void Start()
    {
        if (drawAtStart)
            DrawChart();
    }

    public void SetPositions(List<Vector3> positions)
    {
        this.rectPositions = positions;
        DrawChart();
    }

    private void DrawChart()
    {
        if (rectPositions == null)
            return;

        ClearChart();

        points = new List<GameObject>();
        lines = new List<GameObject>();

        for (int i = 0; i < rectPositions.Count; i++)
        {
            points.Add(InstantiatePoint(rectPositions[i], i));
        }

        for (int i = 0; i < points.Count - 1; i++)
        {
            DrawLine(points[i], points[i + 1], i);
        }

        foreach (GameObject item in points)
        {
            item.transform.SetAsLastSibling();
        }
    }

    private void ClearChart()
    {
        if (points == null)
        {
            return;
        }
        foreach (var item in points)
        {
            Destroy(item.gameObject);
        }

        if (lines == null)
        {
            return;
        }
        foreach (var item in lines)
        {
            Destroy(item.gameObject);
        }
    }

    private void DrawLine(GameObject startPoint, GameObject endPoint, int i)
    {
        Vector3 dir = startPoint.transform.position - endPoint.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.Euler(0f, 0f, angle);

        GameObject line = new GameObject("line " + i);
        line.AddComponent<Image>();
        var lineObject = Instantiate(line, GetCenterOfTwoObject(startPoint, endPoint), q, content);
        lineObject.GetComponent<RectTransform>().sizeDelta = GetDeltaSize(startPoint, endPoint);
        lineObject.GetComponent<Image>().color = lineColor;
        lineObject.GetComponent<Image>().enabled = !hideLine;
        lines.Add(lineObject);
    }

    private Vector3 GetDeltaSize(GameObject startPoint, GameObject endPoint)
    {
        return new Vector2(Vector2.Distance(startPoint.GetComponent<RectTransform>().anchoredPosition, endPoint.GetComponent<RectTransform>().anchoredPosition), lineWidth);
    }

    private Vector3 GetCenterOfTwoObject(GameObject startPoint, GameObject endPoint)
    {
        return new Vector3((startPoint.transform.position.x + endPoint.transform.position.x) / 2,
             (startPoint.transform.position.y + endPoint.transform.position.y) / 2, 0);
    }

    private GameObject InstantiatePoint(Vector3 vector3, int i)
    {
        if (content == null)
        {
            throw new NullReferenceException("Content in null!");
        }
        GameObject point = Instantiate(new GameObject("Point " + i), content);
        point.AddComponent<RectTransform>().sizeDelta = pointSize;
        point.GetComponent<RectTransform>().anchoredPosition = vector3;
        point.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
        point.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        point.AddComponent<Image>().sprite = pointImage;
        point.GetComponent<Image>().enabled = !hidePoint;
        return point;
    }
}
