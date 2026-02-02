using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _transform;

    private void Start()
    {
        ScaleBackGround();
    }

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
}
