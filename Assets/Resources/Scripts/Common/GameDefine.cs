public enum GameState 
{
    None,
    Lobby,
    Playing,
    Upgrade,
    Paused,
    GameOver,
}

public enum UpgradeType
{
    GamePlayTime = 101,
    GameGoldGainPercent = 102,
    GameSpawnTime = 103,
    CenterHP = 201,
    CenterDefense = 202,
    CircleAtk = 301,
    CircleAtkDelay = 302,
    CircleRadius = 303,
    CircleCiritical = 304,
    SkillChainLightning = 1001,
    SkillDeathBlast = 1002,
    SkillOrbital = 1003,
    SkillLaser = 1004,
}

public enum EffectType
{
    CircleHit,
    CenterDam,
    MonsterDam,
}