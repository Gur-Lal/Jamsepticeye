using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public sealed class CustomAnimator : MonoBehaviour
{
    [Serializable]
    public sealed class Clip
    {
        public string name = "idle";

        //drag many sprites at once into this array in the inspector
        public Sprite[] frames;

        public WrapMode wrapMode = WrapMode.Loop;

        //0 = no loop (play once)
        //n > 0 = loop n times (replays)
        //n < 0 = loop forever
        public int loopCount = -1;
    }

    public enum WrapMode
    {
        Once,
        Loop,
        PingPong,
    }

    [Header("renderer")]
    [SerializeField] SpriteRenderer spriteRenderer;

    [Header("config")]
    [SerializeField] float fps = 12f; //fixed framerate for pixel art
    [SerializeField] Sprite defaultSprite;

    [Header("clips")]
    [SerializeField] Clip[] clips;

    [Header("autoplay")]
    [SerializeField] bool playOnStart = false;
    [SerializeField] string startClipName = "idle";

    public event Action<string> OnClipFinished;

    Clip _clip;
    int _frameIndex;
    int _dir = 1; //1 forward, -1 backward
    float _accum;
    int _loopsRemaining; //-1 infinite, 0 done, >0 remaining replays
    bool _playing;

    float FrameDt => (fps <= 0f) ? (1f / 12f) : (1f / fps);

    void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Awake()
    {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        ApplyDefaultSpriteIfNeeded();
    }

    void Start()
    {
        if (playOnStart && !string.IsNullOrWhiteSpace(startClipName))
            Play(startClipName);
    }

    void Update()
    {
        if (!_playing || _clip == null) return;

        var frames = _clip.frames;
        if (frames == null || frames.Length == 0) { Stop(); return; }

        _accum += Time.deltaTime;
        var dt = FrameDt;
        while (_accum >= dt)
        {
            _accum -= dt;
            Step(frames.Length);
            ApplyFrame(frames);
            if (!_playing) break;
        }
    }

    public void Play(string clipName, bool restartIfSame = true)
    {
        var c = FindClip(clipName);
        if (c == null) return;
        Play(c, restartIfSame);
    }

    public void Play(Clip clip, bool restartIfSame = true)
    {
        if (clip == null) return;

        if (!restartIfSame && _playing && ReferenceEquals(_clip, clip))
            return;

        _clip = clip;
        _accum = 0f;
        _dir = 1;
        _playing = true;

        _loopsRemaining = NormalizeLoopCount(_clip.wrapMode, _clip.loopCount);

        _frameIndex = 0;
        ApplyFrame(_clip.frames);
    }

    public void Stop(bool showDefaultSprite = true)
    {
        _playing = false;
        _clip = null;
        _accum = 0f;
        _frameIndex = 0;
        _dir = 1;

        if (showDefaultSprite)
            ApplyDefaultSpriteIfNeeded();
    }

    public void Pause() => _playing = false;
    public void Resume()
    {
        if (_clip != null && _clip.frames != null && _clip.frames.Length > 0)
            _playing = true;
    }

    public bool IsPlaying => _playing;
    public string CurrentClipName => _clip != null ? _clip.name : null;

    public void SetFps(float newFps) => fps = newFps;

    Clip FindClip(string clipName)
    {
        if (clips == null) return null;
        for (int i = 0; i < clips.Length; i++)
        {
            var c = clips[i];
            if (c != null && string.Equals(c.name, clipName, StringComparison.Ordinal))
                return c;
        }
        return null;
    }

    void ApplyDefaultSpriteIfNeeded()
    {
        if (!spriteRenderer) return;
        if (defaultSprite != null)
            spriteRenderer.sprite = defaultSprite;
        else if (spriteRenderer.sprite == null && clips != null && clips.Length > 0 && clips[0]?.frames != null && clips[0].frames.Length > 0)
            spriteRenderer.sprite = clips[0].frames[0];
    }

    void ApplyFrame(Sprite[] frames)
    {
        if (!spriteRenderer) return;
        int len = frames.Length;
        if (len == 0) return;

        _frameIndex = Mathf.Clamp(_frameIndex, 0, len - 1);
        var s = frames[_frameIndex];
        if (s != null) spriteRenderer.sprite = s;
    }

    void Step(int len)
    {
        switch (_clip.wrapMode)
        {
            case WrapMode.Once:
                StepOnce(len);
                break;

            case WrapMode.Loop:
                StepLoop(len);
                break;

            case WrapMode.PingPong:
                StepPingPong(len);
                break;
        }
    }

    void StepOnce(int len)
    {
        if (len <= 1)
        {
            FinishClip();
            return;
        }

        _frameIndex++;
        if (_frameIndex >= len)
        {
            _frameIndex = len - 1;
            FinishClip();
        }
    }

    void StepLoop(int len)
    {
        if (len <= 1)
        {
            HandleLoopBoundary();
            return;
        }

        _frameIndex++;
        if (_frameIndex >= len)
        {
            _frameIndex = 0;
            HandleLoopBoundary();
        }
    }

    void StepPingPong(int len)
    {
        if (len <= 1)
        {
            HandleLoopBoundary();
            return;
        }

        _frameIndex += _dir;

        if (_frameIndex >= len)
        {
            _dir = -1;
            _frameIndex = len - 2; //bounce
            HandleLoopBoundary();
        }
        else if (_frameIndex < 0)
        {
            _dir = 1;
            _frameIndex = 1; //bounce
            HandleLoopBoundary();
        }
    }

    void HandleLoopBoundary()
    {
        if (_loopsRemaining < 0) return; //infinite

        if (_loopsRemaining == 0)
        {
            FinishClip();
            return;
        }

        _loopsRemaining--;
        if (_loopsRemaining == 0 && _clip.wrapMode != WrapMode.Loop && _clip.wrapMode != WrapMode.PingPong)
            FinishClip();
    }

    void FinishClip()
    {
        var finishedName = _clip != null ? _clip.name : null;
        Stop(showDefaultSprite: false);
        OnClipFinished?.Invoke(finishedName);
    }

    static int NormalizeLoopCount(WrapMode mode, int loopCount)
    {
        //for Once: always play once regardless of loopCount
        if (mode == WrapMode.Once) return 0;

        if (loopCount < 0) return -1;
        if (loopCount == 0) return 1; //no loop --> one pass (boundary finishes immediately)
        return loopCount; //n replays
    }

    public void SetDefaultSprite(Sprite newDefaultSprite)
    {
        defaultSprite = newDefaultSprite;
        ApplyDefaultSpriteIfNeeded();
    }
}
