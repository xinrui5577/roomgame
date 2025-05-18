using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongSwitchNode : MahjongGroup
    {
        // 将麻将添加到旋转节点中
        public void AddMahToSwitch(IList<int> cards)
        {
            MahjongContainer item;
            var ctrl = GameCenter.Scene.MahjongCtrl;
            for (int i = 0; i < cards.Count; i++)
            {
                item = ctrl.PopMahjong(cards[i]);
                AddMahjongToList(item);
                Quaternion v3Rotation = item.transform.localRotation;
                v3Rotation.y = 180;
                item.transform.localRotation = v3Rotation;
            }
            SetMahjongPos();
        }
    }
}