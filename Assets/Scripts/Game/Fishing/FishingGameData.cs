using System;
using Assets.Scripts.Game.Fishing.entitys;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.Fishing
{
    public class FishingGameData : YxGameData
    {
        public int BaseUpperScore = 1;
        public bool NeedUpperScore = true;
        public Rect FishPonRect;
        public int GunType;

        public int MaxGunBet { get; private set; }
        public int MinGunBet { get; private set; }
        public float FireSpeed { get; private set; }
        private int[] _gunBets = { 1, 2, 5, 10, 50, 100 };
        public int[] GunBets
        {
            get { return _gunBets; }
        }

        protected override void InitGameData(ISFSObject gameInfo)
        {
            base.InitGameData(gameInfo);
            
            InitBet(gameInfo);
        }


        private void InitBet(ISFSObject gameInfo)
        {
            var maxGunScore = gameInfo.ContainsKey("bmax") ? gameInfo.GetInt("bmax") : 1000;
            var basicGunScore = gameInfo.ContainsKey("ante") ? gameInfo.GetInt("ante") : 1;
            var blts = gameInfo.ContainsKey("blts") ? gameInfo.GetIntArray("blts") : _gunBets;
            FireSpeed = gameInfo.ContainsKey("blsp") ? gameInfo.GetFloat("blsp") : 0.32f;
            var count = blts.Length;

            _gunBets = new int[count];
            var i = 0;
            for (; i < count; i++)
            {
                var score = blts[i] * basicGunScore;
                if (score > maxGunScore) break;
                _gunBets[i] = score;
            }
            Array.Resize(ref _gunBets, i);
            if (i < 1) return;
            MinGunBet = _gunBets[0];
            MaxGunBet = _gunBets[_gunBets.Length - 1];
        }


        protected override YxBaseGameUserInfo OnInitUser(ISFSObject userData)
        {
            var info = new FishingUserInfo();
            info.Parse(userData);
            return info;
        } 

        public override void InitCfg(ISFSObject cargs2)
        {
            base.InitCfg(cargs2);
            var baseUp = "";
            if (cargs2.Parse("-baseUp", ref baseUp))//上分倍数
            {
                int upScore;
                if (int.TryParse(baseUp, out upScore))
                {
                    BaseUpperScore = upScore;
                }
            }
            var needUpStr = "";
            if (cargs2.Parse("-needUpCoin", ref needUpStr))
            {
                bool needUpCoin;
                if (bool.TryParse(needUpStr, out needUpCoin))
                {
                    NeedUpperScore = needUpCoin;
                }
            }
            var gunTypeStr = "";
            if (cargs2.Parse("-gunType", ref gunTypeStr))
            {
                int gunType;
                if (int.TryParse(gunTypeStr, out gunType))
                {
                    GunType = gunType;
                }
            }
        }
    }
}
