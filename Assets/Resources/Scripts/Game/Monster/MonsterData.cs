using UnityEngine;

[CreateAssetMenu(fileName ="MonsterData", menuName ="ScriptableObjects/MonsterData")]
public class MonsterData : ScriptableObject
{
    public int monsterID;
    public Sprite sprite;
    public int monsterLevel;

    public int rewardGold;
    public float baseHP;
    public float baseAtk;
    public float baseSpeed;

    public float baseScale;

    public bool isBoss;
}
