using System;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.Game.GameCtrl;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Mahjong2D.Game.Item
{
    public class PhizItem : MonoBehaviour
    {
        int _clickNum;
        public static Action OnItemClick; 
        void Awake()
        {
            string name = GetComponent<UISprite>().spriteName;
            string[] arr;
            switch ((EnumGameKeys)Enum.Parse(typeof(EnumGameKeys), App.GameKey))
            {
                case EnumGameKeys.ykmj:
                    arr = name.Split('_');
                    break;
                default:
                    arr = name.Split('-');
                    break;
            }          
            _clickNum = int.Parse(arr[0]);
        }

        void OnClick()
        {
            switch ((EnumGameKeys)Enum.Parse(typeof(EnumGameKeys), App.GameKey))
            {
                case EnumGameKeys.ykmj:
                    ChatControl.Instance.SendUserPhizTalk(_clickNum + ConstantData.ExpPlush);
                    break;
                default:
                    ChatControl.Instance.SendUserPhizTalk(_clickNum);
                    break;
            }   
            OnItemClick();
        }
    }
}
