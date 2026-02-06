using System;
using UnityEngine;

public class ScreenManager : Singleton<ScreenManager>
{
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _transform;

    private int _curResIndex = -1;
    private bool _isFull = true;

    public bool IsFull {  get { return _isFull; } }
    public int CurResIndex { get { return _curResIndex; } }

    public void ScaleBackGround()
    {
        if (_sr == null) return;

        float worldScreenHeight = _camera.orthographicSize * 2.0f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        float spriteWidth = _sr.sprite.bounds.size.x;
        float spriteHeight = _sr.sprite.bounds.size.y;

        Vector3 scale = _transform.localScale;
        scale.x = worldScreenWidth / spriteWidth;
        scale.y = worldScreenHeight / spriteHeight;

        float maxScale = Mathf.Max(scale.x, scale.y);
        maxScale *= 1.5f;
        scale = new Vector3(maxScale, maxScale, 1f);

        _transform.localScale = scale;
    }

    public void SetResolution(int index)
    {
        if (_curResIndex == index) return;
        _curResIndex = index;

        switch (index)
        {
            case 0: Screen.SetResolution(2560, 1440, _isFull); break;
            case 1: Screen.SetResolution(1920, 1080, _isFull); break;
            case 2: Screen.SetResolution(1280, 720, _isFull); break;
            case 3: Screen.SetResolution(960, 540, _isFull); break;
        }

        CancelInvoke(nameof(ScaleBackGround));
        Invoke(nameof(ScaleBackGround), 0.1f);
    }

    public void SetScreenMode(bool full)
    {
        if (_isFull == full) return;

        _isFull = full;
        Screen.SetResolution(Screen.width, Screen.height, _isFull);

        CancelInvoke(nameof(ScaleBackGround));
        Invoke(nameof(ScaleBackGround), 0.1f);
    }
}
