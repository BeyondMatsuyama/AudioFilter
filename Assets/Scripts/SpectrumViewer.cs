using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectrumViewer : MonoBehaviour
{
    private const int SampleCount = 8192;
    private const int BinCount = 512;
    private const float BarWidth = 3f;

    // private const int graphCenterFrequency = 1000;
    private const int graphMinFrequency = 20;
    private const int graphMaxFrequency = 20000;

    [SerializeField] private Transform parent;
    [SerializeField] private GameObject objBar;
    private List<RectTransform> spectrumBars = new List<RectTransform>();
    private float[] spectrumData = new float[SampleCount];
    private float[] bins = new float[BinCount];

    // Start is called before the first frame update
    void Start()
    {
        // 初期化
        createSpectrumBars();
    }

    // Update is called once per frame
    void Update()
    {
        // 描画
        draw();
    }

    // スペクトラムアナライザ的な何か
    private void createSpectrumBars()
    {
        Vector3 pos = Vector3.zero;
        for (int i = 0; i < BinCount; i++)
        {
            var obj = GameObject.Instantiate(objBar);
            Transform t = obj.transform;
            t.SetParent(parent);
            t.localScale = Vector3.one;
            t.name = "sp_bar_" + i;

            t.localPosition = pos;
            pos.x += BarWidth;

            spectrumBars.Add(t as RectTransform);
        }
    }

    private void draw()
    {
        AudioListener.GetSpectrumData(spectrumData, 0, FFTWindow.Hanning);
        var outputF = AudioSettings.outputSampleRate;
        // 全binを初期化
        for (int i=0; i<bins.Length; i++)
        {
            bins[i] = 0f;
        }

        var logMaxF = Mathf.Log(graphMaxFrequency); // 上のbinの周波数のlog
        var logMinF = Mathf.Log(graphMinFrequency);
        var logRange = logMaxF - logMinF;
        if (logRange <= 0f)
        {
            logRange = 8f;
        }
        // まず周波数分類
        for (int i=0; i<spectrumData.Length; i++)
        {
            var f = outputF * 0.5f * (float)i / (float)spectrumData.Length;
            if (f == 0f)
            {
                f = float.Epsilon;
            }
            // 対数を取ってどのビンに入るか確定
            float binValue = (float)bins.Length * (Mathf.Log(f) - logMinF) / logRange;
            int binIndex = Mathf.FloorToInt(binValue);
            if ((binIndex >= 0) && (binIndex < bins.Length))
            {
                // そのビンにデータを加算
                bins[binIndex] += spectrumData[i];
            }
        }
        // バーに反映
        for(int i=0; i<BinCount; i++)
        {
            float v = bins[i];
            float y = Mathf.Min(v * 8000f, 420f);
            Vector2 sd = new Vector2(BarWidth, y);
            spectrumBars[i].sizeDelta = sd;
        }
    }

}
