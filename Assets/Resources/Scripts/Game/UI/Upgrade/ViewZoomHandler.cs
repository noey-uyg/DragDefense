using UnityEngine;
using UnityEngine.EventSystems;

public class ViewZoomHandler : MonoBehaviour, IScrollHandler
{
    [SerializeField] private RectTransform _content;
    [SerializeField] private float _zoomSpeed = 0.1f;
    [SerializeField] private float _minZoom = 0.5f;
    [SerializeField] private float _maxZoom = 1.5f;

    private Vector3 _originalPosition;

    private void Start()
    {
        _originalPosition = _content.position;
    }

    public void OnScroll(PointerEventData eventData)
    {
        Vector3 mouseWorldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(_content, eventData.position, eventData.pressEventCamera, out mouseWorldPos);

        float zoomDelta = eventData.scrollDelta.y * _zoomSpeed;
        Vector3 nextScale = _content.localScale + Vector3.one * zoomDelta;

        nextScale.x = Mathf.Clamp(nextScale.x, _minZoom, _maxZoom);
        nextScale.y = Mathf.Clamp(nextScale.y, _minZoom, _maxZoom);
        nextScale.z = 1f;

        _content.localScale = nextScale;

        Vector3 mouseWorldPosAfterZoom;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(_content, eventData.position, eventData.pressEventCamera, out mouseWorldPosAfterZoom);

        Vector3 delta = mouseWorldPos - mouseWorldPosAfterZoom;
        _content.position -= delta;
    }

    public void OnResetView()
    {
        SoundManager.Instance.PlaySFX(SFXType.UI_ButtonClick);
        _content.position = _originalPosition;
    }
}
