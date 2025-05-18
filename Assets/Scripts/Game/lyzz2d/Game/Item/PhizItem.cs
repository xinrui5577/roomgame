using System;
using Assets.Scripts.Game.lyzz2d.Game.Component;
using UnityEngine;

namespace Assets.Scripts.Game.lyzz2d.Game.Item
{
    public class PhizItem : MonoBehaviour
    {
        public static Action OnItemClick;
        private int clickNum;

        private void Awake()
        {
            var name = GetComponent<UISprite>().spriteName;
            var arr = name.Split('-');
            clickNum = int.Parse(arr[0]);
        }

        private void OnClick()
        {
            ChatControl.Instance.SendUserPhizTalk(clickNum);
            OnItemClick();
        }
    }
}