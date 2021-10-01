using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioTester : MonoBehaviour
{
    enum FilterNo
    {
        Chorus = 0,
        Distortion,
        Echo,
        Reverv,
        Num
    }

    private const float LowHz       = 22000f;
    private const float HighHz      = 10f;
    private const float InitVolume  = 0.4f;
    private const float InitPitch   = 1f;


    [SerializeField] AudioSource seSource;
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource voiceSource;

    [SerializeField] Slider sldVolume;
    [SerializeField] Text   txtVolume;
    [SerializeField] Slider sldPitch;
    [SerializeField] Text   txtPitch;
    [SerializeField] Slider sldLoPass;
    [SerializeField] Text   txtLoPass;
    [SerializeField] Slider sldHiPass;
    [SerializeField] Text   txtHiPass;

    [SerializeField] Toggle tglChorus;
    [SerializeField] Toggle tglDistotion;
    [SerializeField] Toggle tglEcho;
    [SerializeField] Toggle tglReverv;
    [SerializeField] Toggle tglLoop;

    [SerializeField] Button btnSePlay;
    [SerializeField] Button btnBgmPlay;
    [SerializeField] Button btnVoicePlay;
    [SerializeField] Button btnAllStop;

    [SerializeField] AudioChorusFilter      fltChorus;
    [SerializeField] AudioDistortionFilter  fltDistortion;
    [SerializeField] AudioEchoFilter        fltEcho;
    [SerializeField] AudioReverbFilter      fltReverb;
    [SerializeField] AudioLowPassFilter     fltLoPass;
    [SerializeField] AudioHighPassFilter    fltHiPass;

    void Awake()
    {
        // フィルタ設定
        for (int i = 0; i < (int)FilterNo.Num; i++)
        {
            OnClickToggleFilter(i);
        }

        // サウンド設定
        setVolume(InitVolume);  // 音量設定
        setPitch(InitPitch);    // ピッチ設定
        setLoPass(1f);          // 低域通過
        setHiPass(0f);          // 高域通過

        OnClickToggleSeLoop();  // SE ループ
    }

    private void Start()
    {
        // Voice 再生・停止
        btnVoicePlay.onClick.AddListener(() =>
        {
            if (!voiceSource.isPlaying) voiceSource.Play();
            else voiceSource.Stop();
        });
        // SE 再生・停止
        btnSePlay.onClick.AddListener(() =>
        {
            if (!seSource.isPlaying) seSource.Play();
            else seSource.Stop();
        });
        // BGM 再生・停止
        btnBgmPlay.onClick.AddListener(() =>
        {
            if (!bgmSource.isPlaying) bgmSource.Play();
            else bgmSource.Stop();
        });
        // 全停止
        btnAllStop.onClick.AddListener(() =>
        {
            if (voiceSource.isPlaying) voiceSource.Stop();
            if (seSource.isPlaying) seSource.Stop();
            if (bgmSource.isPlaying) bgmSource.Stop();
        });

        // ボリューム
        sldVolume.onValueChanged.AddListener((value) =>
        {
            setVolume(value);
        });
        // ピッチ
        sldPitch.onValueChanged.AddListener((value) =>
        {
            setPitch(value);
        });
        // 低音通過
        sldLoPass.onValueChanged.AddListener((value) =>
        {
            setLoPass(value);
        });
        // 高音通過
        sldHiPass.onValueChanged.AddListener((value) =>
        {
            setHiPass(value);
        });
    }


    // 音量設定
    private void setVolume(float val)
    {
        seSource.volume  = val;
        bgmSource.volume = val;
        txtVolume.text = string.Format("{0:0.##}", val);
    }
    // ピッチ設定
    private void setPitch(float val)
    {
        seSource.pitch  = val;
        bgmSource.pitch = val;
        txtPitch.text = string.Format("{0:0.##}", val);
    }

    // 低域通過フィルター設定
    private void setLoPass(float val)
    {
        var loPassHz = ExpHz(val);
        fltLoPass.cutoffFrequency = loPassHz;
        txtLoPass.text = string.Format("{0:#}", loPassHz);
    }

    // 高域通過フィルター設定
    private void setHiPass(float val)
    {
        var hiPassHz = ExpHz(val);
        fltHiPass.cutoffFrequency = hiPassHz;
        txtHiPass.text = string.Format("{0:#}", hiPassHz);
    }

    // Filter 用トグル
    public void OnClickToggleFilter(int no)
    {
        // Debug.Log("Click Toggle : " + no);
        switch ((FilterNo)no)
        {
            case FilterNo.Chorus:
                fltChorus.enabled = tglChorus.isOn;
                break;
            case FilterNo.Distortion:
                fltDistortion.enabled = tglDistotion.isOn;
                break;
            case FilterNo.Echo:
                fltEcho.enabled = tglEcho.isOn;
                break;
            case FilterNo.Reverv:
                fltReverb.enabled = tglReverv.isOn;
                break;
        }
    }

    // SE ループ用トグル
    public void OnClickToggleSeLoop()
    {
        seSource.loop = tglLoop.isOn;
    }

    // 10hzで0、22000hzで1になるような対数変換を返す
    float LogHz(float hz)
    {
        return (Mathf.Log(hz) - Mathf.Log(HighHz)) / (Mathf.Log(LowHz) - Mathf.Log(HighHz));
    }

    // 上の逆変換
    // logHz = (log(hz) - log(10)) / (log(22000) - log(10))
    // logHz * (log(22000) - log(10)) = log(hz) - log(10)
    // log(hz) = (logHz * (log(22000) - log(10))) + log(10)
    // expして
    // hz = Exp((logHz * (log(22000) - log(10))) + log(10))
    //    = Exp(logHz * (log(22000) - log(10))) * 10
    float ExpHz(float logHz)
    {
        var t = Mathf.Log(LowHz) - Mathf.Log(HighHz);
        return Mathf.Exp(logHz * t) * 10f;
    }

}
