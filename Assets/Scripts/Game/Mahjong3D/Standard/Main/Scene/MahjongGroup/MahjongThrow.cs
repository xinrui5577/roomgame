using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongThrow : MahjongGroup
    {
        public override void OnInitalization()
        {
            var adp = GetComponent<MahjongThrowAdpOffset>();
            if (adp != null)
            {
                adp.OnInitalization();
            }
        }

        protected override Vector3 GetPos(MahjongVecter index)
        {
            Vector3 mahjongSize = DefaultUtils.MahjongSize;
            return new Vector3(mahjongSize.x * (index.x + 0.5f), -mahjongSize.y * (index.y + 0.5f), -mahjongSize.z * (index.z + 0.5f));
        }

        public MahjongContainer PopMahjong()
        {
            if (mMahjongList.Count == 0) return null;

            MahjongContainer item = mMahjongList[mMahjongList.Count - 1];
            GameCenter.Scene.MahjongCtrl.PushMahjongToPool(item);
            mMahjongList.Remove(item);
            return item;
        }

        public MahjongContainer PopMahjong(int value)
        {
            MahjongContainer item = mMahjongList[mMahjongList.Count - 1];
            //找到打出的牌
            if (value != item.Value)
            {
                MahjongContainer tempItem = null;
                for (int i = mMahjongList.Count - 1; i >= 0; i--)
                {
                    tempItem = mMahjongList[i];
                    if (value == tempItem.Value)
                    {
                        item = tempItem;
                    }
                }
            }
            GameCenter.Scene.MahjongCtrl.PushMahjongToPool(item);
            mMahjongList.Remove(item);
            return item;
        }

        public MahjongContainer GetLastMahjong()
        {
            if (mMahjongList.Count == 0) return null;
            return mMahjongList[mMahjongList.Count - 1];
        }

        public void SignItemByValueGreen(int value)
        {
            for (int i = 0; i < mMahjongList.Count; i++)
            {
                if (mMahjongList[i].Value == value)
                {
                    mMahjongList[i].ShowBlue();
                }
            }
        }

        public void ReplyItem()
        {
            for (int i = 0; i < mMahjongList.Count; i++)
            {
                mMahjongList[i].ShowNormal();
            }
        }

        public override void SetMahjongPos()
        {
            base.SetMahjongPos();
            if (Chair == 2 && GameCenter.DataCenter.Config.MahjongTowardsMe)
            {
                for (int i = 0; i < mMahjongList.Count; i++)
                {
                    var item = mMahjongList[i];
                    float z = item.transform.localEulerAngles.z;
                    item.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 180f));
                }
            }
        }
    }
}