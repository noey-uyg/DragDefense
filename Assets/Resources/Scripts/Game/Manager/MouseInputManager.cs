using UnityEngine;

public class MouseInputManager : Singleton<MouseInputManager>
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Transform _draggingObject;

    private float _dragSmoothness = 50f;
    private Vector3 _targetPosition;
    private Vector3 _lastMoustPosition;
    private Transform _cameraTransform;

    protected override void OnAwake()
    {
        if (_mainCamera == null) _mainCamera = Camera.main;
        _cameraTransform = _mainCamera.transform;

        if (_draggingObject != null) _targetPosition = _draggingObject.position;
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing) return;
        if (HandleTouchInput()) return;
        HandleMouse();
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

    private void HandleMouse()
    {
        Vector3 currentMoustPos = Input.mousePosition;
        if(currentMoustPos != _lastMoustPosition)
        {
            _targetPosition = ScreenToWorld(currentMoustPos);
            _lastMoustPosition = currentMoustPos;
        }
    }

    private Vector3 ScreenToWorld(Vector3 screenPos)
    {
        screenPos.z = -_cameraTransform.position.z;
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
        if (_draggingObject == null) return;

        if(Vector3.SqrMagnitude(_draggingObject.position - _targetPosition) < 0.0001f)
        {
            _draggingObject.position = _targetPosition;
            return;
        }

        _draggingObject.position = Vector3.Lerp(
            _draggingObject.position,
            _targetPosition,
            Time.deltaTime * _dragSmoothness);
    }
    #endregion
}
