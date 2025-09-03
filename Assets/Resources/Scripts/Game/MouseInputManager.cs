using UnityEngine;

public class MouseInputManager : Singleton<MouseInputManager>
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Transform _draggingObject;

    private float _dragSmoothness = 50f;
    private Vector3 _targetPosition;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        if (HandleTouchInput()) return;
        HandleMouseInput();
        SmoothFollow();
    }

    #region Input Handlers
    private bool HandleTouchInput()
    {
        if (Input.touchCount <= 0) return false;

        Touch touch = Input.GetTouch(0);
        Vector3 worldPos = ScreenToWorld(touch.position);

        switch (touch.phase)
        {
            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                TryDrag(worldPos);
                break;
        }

        return true;
    }

    private void HandleMouseInput()
    {
        Vector3 worldPos = ScreenToWorld(Input.mousePosition);

        if (Input.GetMouseButton(0))
        {
            TryDrag(worldPos);
        }
    }

    private Vector3 ScreenToWorld(Vector3 screenPos)
    {
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(screenPos);
        worldPos.z = 0;
        return worldPos;
    }

    private void TryDrag(Vector3 worldPos)
    {
        _targetPosition = worldPos;
    }

    private void SmoothFollow()
    {
        if(_draggingObject != null)
        {
            _draggingObject.position = Vector3.Lerp(_draggingObject.position, _targetPosition, Time.deltaTime * _dragSmoothness);
        }
    }
    #endregion
}
