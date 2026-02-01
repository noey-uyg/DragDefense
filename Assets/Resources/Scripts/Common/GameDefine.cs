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
    // [Game] 100 ~
    GamePlayTime = 101,
    GameGoldGainPercent = 102,
    GameSpawnTime = 103,
    GameMonsterLevel = 104,
    GameGoldBonusChance = 105,

    // [Center] 200 ~
    CenterHP = 201,
    CenterDefense = 202,

    // [Circle] 300 ~
    CircleAtk = 301,
    CircleAtkDelay = 302,
    CircleRadius = 303,
    CircleCritical = 304,
    CircleCriticalDam = 305,
    CircleVampire = 306,

    // [Skill] 1000 ~
    SkillSlash = 1001,
    SkillDeathBlast = 1002,
    SkillOrbital = 1003,
    SkillLaser = 1004,
}

public enum EffectType
{
    CircleHit,
    CenterDam,
    MonsterDam,
    SlashEffect,
}

public enum BGMType
{
    Main,
    Game,
}

public enum SFXType
{
    UI_ButtonClick,

    Atk_Normal,
    Atk_Slash,
    Atk_Orbital,
    Atk_Blast,

    Evt_MonsterHit,
    Evt_MonsterDie,
    Evt_CenterHit,
}