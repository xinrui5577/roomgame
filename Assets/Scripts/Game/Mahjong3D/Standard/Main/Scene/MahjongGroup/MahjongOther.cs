using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongOther : MahjongGroup
    {
        public bool RotationMahjong;

        protected override Vector3 GetPos(MahjongVecter index)
        {
            var mahjongSize = DefaultUtils.MahjongSize;
            return new Vector3(mahjongSize.x * (index.x + 0.5f), -mahjongSize.y * (index.y + 0.5f), -mahjongSize.z * (index.z + 0.5f));
        }

        public override List<MahjongContainer> GetInMahjong(IList<int> value)
        {
            var list = new List<MahjongContainer>();
            for (int i = 0; i < value.Count; i++)
            {
                GetInMahjong(value[i]);
            }            
            return list;
        } 

        public override MahjongContainer GetInMahjong(int value)
        {            
            var item = mMahjongList.Find(m => m.Value == value);
            if (item != null)
            {
                item.Number++;
            }
            else
            {
                item = GameCenter.Scene.MahjongCtrl.PopMahjong(value);
                item.transform.ExSetParent(transform);
                mMahjongList.Add(item);
                item.Number = 1;
            }
            if (RotationMahjong)
            {
                item.transform.ExLocalRotation(new Vector3(0, 0, 180));
            }
            SetMahjongPos();
            return item;
        }
    }
}