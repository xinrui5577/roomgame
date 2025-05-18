using System;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Game.Item
{
    public class PiaoItem : MonoBehaviour
    {
        public int GangNumBer;

        public static Action<int> OnItemSelect;

        private UIButton _button;

        void Awake()
        {
            _button = GetComponent<UIButton>();
            _button.onClick.Add(new EventDelegate(this, "OnClickItem")) ;
        }

        public void OnClickItem()
        {
            OnItemSelect(GangNumBer);
        }
    }
}
