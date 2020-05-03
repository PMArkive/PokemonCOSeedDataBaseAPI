using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace PokemonCOSeedDataBaseAPI
{
    public abstract class SeedSearcher
    {
        protected readonly string PATH;
        public const int SpecifiedNumberOfKey = 7;
        internal SeedSearcher(string path) { PATH = path; }

        /// <summary>
        /// 7回分の連続したとにかくバトル(シングル, 最強)の生成結果から, 現在のseed候補を検索します. 引数の長さが7以外の場合は例外を投げます.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public abstract IEnumerable<uint> Search((PlayerName playerNameIndex, BattleTeam teamIndex)[] keys);

        public static SeedSearcher CreateFullDBSearcher(string path)
        {
            for (int i = 0; i < 24 * 24; i++)
            {
                if (!File.Exists(path + $"/{i}.bin"))
                    throw new Exception($"File could not be found : {path}\\{i}.bin");
            }
            return new FullDBSearcher(path + "/");
        }
        public static SeedSearcher CreateLightDBSearcher(string path)
        {
            for (int i = 0; i < 24 * 24; i++)
            {
                if (!File.Exists(path + $"/{i}.bin"))
                    throw new Exception($"File could not be found : {path}\\{i}.bin");
            }
            return new LightDBSearcher(path + "/");
        }
    }

    class FullDBSearcher : SeedSearcher
    {
        internal FullDBSearcher(string path) : base(path) { }

        public override IEnumerable<uint> Search((PlayerName playerNameIndex, BattleTeam teamIndex)[] keys)
        {
            if (keys.Length != SpecifiedNumberOfKey) throw new Exception($"Number of search keys must be {SpecifiedNumberOfKey}.");

            var codedKeys = keys.Select(_ => (uint)_.playerNameIndex * 8 + (uint)_.teamIndex).ToArray();

            uint fileKey = codedKeys[5] + codedKeys[6] * 24;
            uint seedKey = codedKeys[4] + codedKeys[3] * 24 + codedKeys[2] * 24 * 24 + codedKeys[1] * 24 * 24 * 24 + codedKeys[0] * 24 * 24 * 24 * 24;

            var seedList = new List<uint>();
            string fileName = PATH + $"{fileKey}.bin";
            try
            {
                var fileinfo = new FileInfo(fileName);
                long filesize = fileinfo.Length / sizeof(uint);
                long tempIndex = seedKey * filesize / 0x798001;
                using (var fstream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                using (var binaryReader = new BinaryReader(fstream))
                {
                    // 前方への探索
                    int index = (int)Math.Max(0, tempIndex - 1); // tempIndexの位置の探索は後方へ探索するときに通る.
                    fstream.Seek(index * 4, SeekOrigin.Begin);
                    while (true)
                    {
                        var seed = binaryReader.ReadUInt32();
                        var key = GenerateSeedKey(ref seed);

                        if (key < seedKey) break;
                        if (key == seedKey) seedList.Add(AdvanceWithGenerateCode(seed));

                        if (--index < 0) break;
                        fstream.Seek(index * 4, SeekOrigin.Begin);
                    }

                    // 後方への探索
                    fstream.Seek(tempIndex * 4, SeekOrigin.Begin);
                    while (true)
                    {
                        var seed = binaryReader.ReadUInt32();
                        if (fstream.Position > fstream.Length) break;
                        if (fstream.Position < 0) break;

                        var key = GenerateSeedKey(ref seed);

                        if (key > seedKey) break;
                        if (key == seedKey) seedList.Add(AdvanceWithGenerateCode(seed));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load file.{Environment.NewLine}{ex.Message}");
            }
            return seedList.Distinct();
        }

        private static uint GenerateSeedKey(ref uint seed)
        {
            uint[] codes = new uint[5];
            for (int k = 0; k < 5; k++)
            {
                uint EnemyTeamIndex = seed.GetRand(8);
                uint PlayerTeamIndex;
                do { PlayerTeamIndex = seed.GetRand(8); } while (EnemyTeamIndex == PlayerTeamIndex);

                uint EnemyTSV = seed.GetRand() ^ seed.GetRand();
                var EnemyParty = BattleTeamUltimate.UltimateTeams[EnemyTeamIndex];
                for (int i = 0; i < 6; i++)
                {
                    seed.Advance5();
                    while (true)
                    {
                        uint HID = seed.GetRand();
                        uint LID = seed.GetRand();
                        uint PID = (HID << 16) | LID;
                        if (GetGender(PID, EnemyParty[i].GenderRatio) != EnemyParty[i].fixedGender) continue;
                        if (PID % 25 != (uint)EnemyParty[i].fixedNature) continue;
                        if ((LID ^ HID ^ EnemyTSV) < 8) continue;

                        break;
                    }
                }

                uint PlayerNameIndex = seed.GetRand(3);

                uint PlayerTSV = seed.GetRand() ^ seed.GetRand();
                var PlayerParty = BattleTeamUltimate.UltimateTeams[PlayerTeamIndex];
                for (int i = 0; i < 6; i++)
                {
                    seed.Advance5();
                    while (true)
                    {
                        uint HID = seed.GetRand();
                        uint LID = seed.GetRand();
                        uint PID = (HID << 16) | LID;
                        if (GetGender(PID, PlayerParty[i].GenderRatio) != PlayerParty[i].fixedGender) continue;
                        if (PID % 25 != (uint)PlayerParty[i].fixedNature) continue;
                        if ((LID ^ HID ^ PlayerTSV) < 8) continue;

                        break;
                    }
                }

                codes[k] = PlayerNameIndex * 8 + PlayerTeamIndex;
            }

            return codes[4] + codes[3] * 24 + codes[2] * 24 * 24 + codes[1] * 24 * 24 * 24 + codes[0] * 24 * 24 * 24 * 24;
        }

        private static uint AdvanceWithGenerateCode(uint seed)
        {
            for (int k = 0; k < 2; k++)
            {
                uint EnemyTeamIndex = seed.GetRand(8);
                uint PlayerTeamIndex;
                do { PlayerTeamIndex = seed.GetRand(8); } while (EnemyTeamIndex == PlayerTeamIndex);

                uint EnemyTSV = seed.GetRand() ^ seed.GetRand();
                var EnemyParty = BattleTeamUltimate.UltimateTeams[EnemyTeamIndex];
                for (int i = 0; i < 6; i++)
                {
                    seed.Advance5();
                    while (true)
                    {
                        uint HID = seed.GetRand();
                        uint LID = seed.GetRand();
                        uint PID = (HID << 16) | LID;
                        if (GetGender(PID, EnemyParty[i].GenderRatio) != EnemyParty[i].fixedGender) continue;
                        if (PID % 25 != (uint)EnemyParty[i].fixedNature) continue;
                        if ((LID ^ HID ^ EnemyTSV) < 8) continue;

                        break;
                    }
                }

                seed.Advance();

                uint PlayerTSV = seed.GetRand() ^ seed.GetRand();
                var PlayerParty = BattleTeamUltimate.UltimateTeams[PlayerTeamIndex];
                for (int i = 0; i < 6; i++)
                {
                    seed.Advance5();
                    while (true)
                    {
                        uint HID = seed.GetRand();
                        uint LID = seed.GetRand();
                        uint PID = (HID << 16) | LID;
                        if (GetGender(PID, PlayerParty[i].GenderRatio) != PlayerParty[i].fixedGender) continue;
                        if (PID % 25 != (uint)PlayerParty[i].fixedNature) continue;
                        if ((LID ^ HID ^ PlayerTSV) < 8) continue;

                        break;
                    }
                }
            }

            return seed;
        }

        private static Gender GetGender(uint PID, GenderRatio genderRatio)
        {
            if (genderRatio == GenderRatio.Genderless) return Gender.Genderless;
            return (PID & 0xFF) < (uint)genderRatio ? Gender.Female : Gender.Male;
        }
    }
    class LightDBSearcher : SeedSearcher
    {
        internal LightDBSearcher(string path) : base(path) { }
        public new const int SpecifiedNumberOfKey = 8;
        public override IEnumerable<uint> Search((PlayerName playerNameIndex, BattleTeam teamIndex)[] keys)
        {
            if (keys.Length != SpecifiedNumberOfKey) throw new Exception($"Number of search keys must be {SpecifiedNumberOfKey}.");

            var codedKeys = keys.Select(_ => (uint)_.playerNameIndex * 8 + (uint)_.teamIndex).ToArray();

            uint fileKey = codedKeys[1] + codedKeys[2] * 24;
            uint seedKey = codedKeys[3] + codedKeys[4] * 24 + codedKeys[5] * 24 * 24 + codedKeys[6] * 24 * 24 * 24 + codedKeys[7] * 24 * 24 * 24 * 24;

            string fileName = PATH + $"{fileKey}.bin";

            var seedList = new List<uint>();
            try
            {
                var fileinfo = new FileInfo(fileName);
                var fileLength = fileinfo.Length;
                var filesize = (int)(fileLength / (2 * sizeof(uint)));
                using (var fstream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                using (var binaryReader = new BinaryReader(fstream))
                {
                    int left = -1, right = filesize;
                    while (right - left > 1)
                    {
                        var mid = (left + right) / 2;
                        fstream.Seek(mid * 8, SeekOrigin.Begin);

                        var key = binaryReader.ReadUInt32();
                        if (key >= seedKey) right = mid; else left = mid;
                    }
                    if (right == left) yield break;

                    fstream.Seek(right * 8, SeekOrigin.Begin);
                    while(fstream.Position < fileLength)
                    {
                        var key = binaryReader.ReadUInt32();
                        if (key != seedKey) break;
                        seedList.Add(binaryReader.ReadUInt32());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load file.{Environment.NewLine}{ex.Message}");
            }

            foreach (var seed in seedList) yield return seed;
        }
    }
}