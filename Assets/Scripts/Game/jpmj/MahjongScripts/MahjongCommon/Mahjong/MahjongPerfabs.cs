using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong
{
    public class MahjongPerfabs : MonoBehaviour
    {
        public List<GameObject> MahjongList;

        public Material MahjongMaterialNomal;
        public Material MahjongMaterialGreen;
        public Material MahjongMaterialGay;
        public GameObject TingIcon;  

        public List<GameObject> GetGameObjList(List<int> cards)
        {
            List<GameObject> ret = new List<GameObject>();
            for (int i = 0; i < cards.Count; i++)
            {
                GameObject findObj = MahjongList.Find(a =>
                {
                    return a.name.Equals(cards[i].ToString());
                });

                if (findObj == null)
                {
                    YxDebug.LogError("在麻将预设中找不到对应的牌" + cards[i]);
                }
                else
                {
                    ret.Add(findObj);
                }
            }

            return ret;
        }
    }
}
