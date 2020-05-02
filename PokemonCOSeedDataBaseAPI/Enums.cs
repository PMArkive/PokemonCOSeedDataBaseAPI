namespace PokemonCOSeedDataBaseAPI
{
    enum Nature
    {
        /// <summary> がんばりや </summary>
        Hardy,
        /// <summary> さみしがり </summary>
        Lonely,
        /// <summary> ゆうかん </summary>
        Brave,
        /// <summary> いじっぱり </summary>
        Adamant,
        /// <summary> やんちゃ </summary>
        Naughty,
        /// <summary> ずぶとい </summary>
        Bold,
        /// <summary> すなお </summary>
        Docile,
        /// <summary> のんき </summary>
        Relaxed,
        /// <summary> わんぱく </summary>
        Impish,
        /// <summary> のうてんき </summary>
        Lax,
        /// <summary> おくびょう </summary>
        Timid,
        /// <summary> せっかち </summary>
        Hasty,
        /// <summary> まじめ </summary>
        Serious,
        /// <summary> ようき </summary>
        Jolly,
        /// <summary> むじゃき </summary>
        Naive,
        /// <summary> ひかえめ </summary>
        Modest,
        /// <summary> おっとり </summary>
        Mild,
        /// <summary> れいせい </summary>
        Quiet,
        /// <summary> てれや </summary>
        Bashful,
        /// <summary> うっかりや </summary>
        Rash,
        /// <summary> おだやか </summary>
        Calm,
        /// <summary> おとなしい </summary>
        Gentle,
        /// <summary> なまいき </summary>
        Sassy,
        /// <summary> しんちょう </summary>
        Careful,
        /// <summary> きまぐれ </summary>
        Quirky,
        other
    }
    enum Gender { Genderless, Male, Female }
    enum GenderRatio : uint
    {
        MaleOnly = 0,
        M7F1 = 0x1F,
        M3F1 = 0x3F,
        M1F1 = 0x7F,
        M1F3 = 0xBF,
        FemaleOnly = 0x100,
        Genderless = 0x12C
    }

    public enum PlayerName : uint
    {
        LEO, YUTA, TATSUKI
    }
    public enum BattleTeam : uint
    {
        Blazikin,
        Entei,
        Swampert,
        Raikou,
        Meganium,
        Suicun,
        Metagross,
        Heracross
    }
}
