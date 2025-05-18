using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon
{
    public class ConfigData : MonoBehaviour {
        ////麻将的个数
        public int MaxHardCnt = 13;

        public int OutCardTime = 10;

        public bool IsCpgHeng = true;
        //发牌时间
        public float SendCardInterval = 0.13f;
        public float SendCardUpTime = 0.2f;
        public float SendCardUpWait = 0.15f;
        //抓牌时间
        public float GetInCardRoteTime = 0.15f;
        public float GetInCardWaitTime = 0.15f;
    }
}
