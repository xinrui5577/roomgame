using System;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.Game.GameCtrl;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Mahjong2D.Game.Item
{
    public class NormalTalkItem : MonoBehaviour
    {
        public static Action OnItemClick;
        int clickNum;
        void Awake()
        {
            clickNum = int.Parse(name);
        }

        void OnClick()
        {
            switch ((EnumGameKeys)Enum.Parse(typeof(EnumGameKeys),App.GameKey))
            { 
               case EnumGameKeys.ykmj:
                    App.GetRServer<Mahjong2DGameServer>().SendYkUserTalk(clickNum + ConstantData.SortTalkPlush);
                    break;
                default:
                    App.GetRServer<Mahjong2DGameServer>().SendUserTalk(ConstantData.TalkConmmonTag + name);
                    break;      
            }            
            OnItemClick();
        }
    }
}
