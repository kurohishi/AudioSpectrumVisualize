using UnityEngine;

public class SpectrumLineControl : MonoBehaviour
{
    [SerializeField] private int type = 0;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private AudioSource source = null;

    [SerializeField] private int visible = 128;
    [SerializeField] private float waveLength = 20.0f;
    [SerializeField] private float yLength = 10f;

    private float[] spectram = null;
    private Vector3[] points = null;
    private const int FFT_RESOLUTION = 128;

    private void Start()
    {
        Prepare();
    }
    public void Prepare()
    {
        spectram = new float[FFT_RESOLUTION];
        points = new Vector3[visible + 1];
    }
    public void Update()
    {
        if(type == 0) LineRender();
        else if (type == 1) CircleRender();
        else if (type == 2) ScalingCircleRender();
    }

    private void LineRender()
    {
        source.GetSpectrumData(spectram, 0, FFTWindow.Rectangular);

        var xStart = -waveLength / 2;
        var xStep = waveLength / spectram.Length;

        for (var i = 0; i < visible; i++)
        {
            var y = spectram[i] * yLength;
            var x = xStart + xStep * i;

            var p = new Vector3(x, y, 0) + transform.position;
            points[i] = p;
        }

        if (points == null) return;
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }

    [SerializeField] private float radius = 1.0f;

    [SerializeField] private int direction = 1;

    [SerializeField] private int circleRate = 1;
    private void CircleRender()
    {
        source.GetSpectrumData(spectram, 0, FFTWindow.Rectangular);

        var r = spectram[0] * yLength;
        var rad = Mathf.Deg2Rad * (0 * 360f / (visible * circleRate));
        var X = (radius + r * direction) *Mathf.Sin(rad);
        var Y = (radius + r * direction) *Mathf.Cos(rad);
        points[0] = new Vector3(X, Y, 0) + transform.position;

        for (var i = 1; i < visible; i++)
        {
            r = spectram[i] * yLength;
            rad = Mathf.Deg2Rad * (i * 360f / (visible * circleRate));
            X = (radius + r * direction) * Mathf.Sin(rad);
            Y = (radius + r * direction) * Mathf.Cos(rad);

            points[i] = new Vector3(X, Y, 0) + transform.position;
        }

        r = spectram[visible - 1] * yLength; //スペクトラムの配列はvisible-1までなので、visibleにするとエラーが出る
        rad = Mathf.Deg2Rad * (visible * 360f / (visible * circleRate));
        X = (radius + r * direction) * Mathf.Sin(rad);
        Y = (radius + r * direction) * Mathf.Cos(rad);

        points[visible] = new Vector3(X, Y, 0) + transform.position;

        if (points == null) return;
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }
    private float scale = 1.0f;
    private void ScalingCircleRender()
    {
        source.GetSpectrumData(spectram, 0, FFTWindow.Rectangular);

        scale = spectram[0];

        var r = spectram[0] * yLength;
        var rad = Mathf.Deg2Rad * (0 * 360f / (visible * circleRate));
        var X = (radius + r * direction + scale) * Mathf.Sin(rad);
        var Y = (radius + r * direction + scale) * Mathf.Cos(rad);

        points[0] = new Vector3(X, Y, 0) + transform.position;

        for (var i = 1; i < visible; i++)
        {
            r = spectram[i] * yLength;
            rad = Mathf.Deg2Rad * (i * 360f / (visible * circleRate));
            X = (radius + r * direction + scale) * Mathf.Sin(rad);
            Y = (radius + r * direction + scale) * Mathf.Cos(rad);

            points[i] = new Vector3(X, Y, 0) + transform.position;
        }

        r = spectram[visible - 1] * yLength;
        rad = Mathf.Deg2Rad * (visible * 360f / (visible * circleRate));
        X = (radius + r * direction + scale) * Mathf.Sin(rad);
        Y = (radius + r * direction + scale) * Mathf.Cos(rad);

        points[visible] = new Vector3(X, Y, 0) + transform.position;

        if (points == null) return;
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }
}
