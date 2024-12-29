using UnityEngine;

public class SequenceLineControl : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float waveLength = 20.0f;
    [SerializeField] private float yLength = 10f;

    [SerializeField] private AudioSource source = default;
    private float[] data = default;
    private int sampleStep = default;
    private Vector3[] samplingLinePoints = default;

    private float[] spectram = null;
    private const int FFT_RESOLUTION = 128;
    private void Start()
    {
        Prepare();
    }

    public void Prepare()
    {
        var clip = source.clip;
        data = new float[clip.channels * clip.samples];
        clip.GetData(data, 0);

        var fps = Mathf.Max(60f, 1f / Time.fixedDeltaTime);
        sampleStep = (int)(clip.frequency / fps);
        samplingLinePoints = new Vector3[sampleStep];

        spectram = new float[FFT_RESOLUTION];
    }

    private void Update()
    {
        if (source.isPlaying && source.timeSamples < data.Length)
        {
            var startIndex = source.timeSamples;
            var endIndex = source.timeSamples + sampleStep;

            Inflate(data, startIndex, endIndex, samplingLinePoints, waveLength, -waveLength / 2f, yLength);

            Render(samplingLinePoints);
        }
    }
    [SerializeField] private float radius;       // ‰~‚Ì”¼Œa
    [SerializeField] private float lineWidth;    // ‰~‚Ìü‚Ì‘¾‚³
    [SerializeField] private int circleRate = 1;
    private float scale = 1.0f;
    private void Render(Vector3[] points)
    {
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = sampleStep;
        lineRenderer.useWorldSpace = false; // transform.localScale ‚ð“K—p‚·‚é‚½‚ß

        source.GetSpectrumData(spectram, 0, FFTWindow.Rectangular);
        //scale = spectram[0];

        for (var i = 0; i < sampleStep; i++)
        {
            //ãæ‚¹‚·‚é”gŒ`
            var rad = Mathf.Deg2Rad * (i * 360f / (sampleStep* circleRate));
            var wave = Mathf.Abs(samplingLinePoints[i].y) * 0.1f;

            //ƒx[ƒX‚É‚È‚é‰~Œ`
            var x = Mathf.Sin(rad) * (radius + wave + scale);
            var y = Mathf.Cos(rad) * (radius + wave + scale);
            points[i] = new Vector3(x, y, 0);
        }


        lineRenderer.SetPositions(points);
    }
    public void Inflate(float[] target, int start, int end, Vector3[] result, float xLength, float xOffset, float yLength)
    {
        var samples = Mathf.Max(end - start, 1f);
        var xStep = xLength / samples;
        var j = 0;

        for (var i = start; i < end || j < sampleStep-1; i++, j++)
        {
            var x = xOffset + xStep * j;
            var y = i < target.Length ? target[i] * yLength : 0f;
            var p = new Vector3(x, y, 0) + transform.position;
            result[j] = p;
        }
    }
}
