using System;
using UnityEngine;

namespace Assets.Scripts.Game.lyzz2d.Game.Item.PiaoItem
{
    public class PiaoItem : MonoBehaviour
    {
        public static Action<int> OnItemSelect;

        private UIButton _button;
        public int GangNumBer;

        private void Awake()
        {
            _button = GetComponent<UIButton>();
            _button.onClick.Add(new EventDelegate(this, "OnClickItem"));
        }

        public void OnClickItem()
        {
            OnItemSelect(GangNumBer);
        }
    }
}