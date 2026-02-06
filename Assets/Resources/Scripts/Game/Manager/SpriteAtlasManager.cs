using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteAtlasManager : Singleton<SpriteAtlasManager>
{
    [SerializeField] private List<SpriteAtlas> _atlases = new List<SpriteAtlas>();
    
    private Dictionary<string, SpriteAtlas> _atlasCache = new Dictionary<string, SpriteAtlas>();

    protected override void OnAwake()
    {
        base.OnAwake();

        foreach(var v in _atlases)
        {
            if(v != null && !_atlasCache.ContainsKey(v.name))
            {
                _atlasCache.Add(v.name, v);
            }
        }
    }

    public Sprite GetSprite(string atlasName, string spriteName)
    {
        if(_atlasCache.TryGetValue(atlasName, out var atlas))
        {
            Sprite sprite = atlas.GetSprite(spriteName);

            if (sprite == null)
            {
                Debug.LogWarning($"{atlasName} 에서 {spriteName}를 찾을 수 없음");
            }

            return sprite;
        }

        Debug.LogWarning($"{atlasName}이 등록되지 않음");
        return null;
    }
}
