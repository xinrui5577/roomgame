using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine; 
using YxFramwork.Common.Utils;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.fruit
{
    public class FruitGameData : YxGameData
    { 
        //水果种类和押注数量
        public IDictionary<FruitType, int> YaFruitDic = new Dictionary<FruitType, int>();
        /// <summary>
        /// casino押注金币
        /// </summary>
        public int Gold { get; set; }

        /// <summary>
        /// 比大小押注选择项
        /// </summary>
        public int Dx { get; set; }

        private FruitGameManager _gameMgr;
        public FruitGameManager GameMgr
        {
            get
            {
                if (_gameMgr != null) return _gameMgr;
                var gob = GameObject.Find("ScriptContainer");
                _gameMgr = gob.AddComponent<FruitGameManager>();
                return _gameMgr;
            }
            set
            {
                if (_gameMgr != null) return;
                _gameMgr = value; 
            }
        }

        protected override void InitGameData(ISFSObject gameInfo)
        {
            if (gameInfo.ContainsKey(RequestKey.KeyAnte))
            {
                Ante = gameInfo.GetInt(RequestKey.KeyAnte);
            }
        }


        public static int[] GetYazhuFruitArray(Dictionary<FruitType, int> dict)
        {
            var fruitAnteAry = new int[8];
            foreach (var fruitType in dict.Keys)
            {
                var index = (int) fruitType;
                fruitAnteAry[index] = dict[fruitType];
            }
            return fruitAnteAry;
        }
    }
}
