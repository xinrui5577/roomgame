using UnityEngine;
using System.Collections.Generic;
using YxFramwork.Common;

namespace Assets.Scripts.Game.brnn3d
{
    public class PaiModeMgr : MonoBehaviour
    {
        private int[] _pais;

        private int _paiIndex;

        private readonly Dictionary<int, int> _beefPoint = new Dictionary<int, int>();

        protected void Awake()
        {
            _pais = new int[5];
        }
        //设置发牌数据
        public void SetPaiModeDataEx()
        {
            int tmp = App.GetGameData<Brnn3dGameData>().SendCardPosition;
            _paiIndex = 0;
            var gameMgr = App.GetGameManager<Brnn3DGameManager>();
            var paiMode = gameMgr.ThePaiMode;
            for (var i = 0; i < 5; i++)
            {
                for (var j = 1; j < 6; j++)
                {
                    paiMode.InstancePai(i, tmp/*, (tmp + i)%5*/, _paiIndex);
                    tmp += 1;
                    if (tmp > 4)
                    {
                        tmp = 0;
                    }
                    _paiIndex += 1;
                }
            }
        }
        void GetBeefPoint(int area, int paiP)
        {
            if (_beefPoint.ContainsKey(area))
            {
                if (paiP > 10) paiP = 10;
                _beefPoint[area] += paiP;
            }
            else
            {
                _beefPoint.Add(area, paiP);
            }
        }
    }
}

