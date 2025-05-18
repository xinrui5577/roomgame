using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong
{
    public class MahjongThrow : MahjongGroup
    {
        protected override Vector3 GetPos(MjIndex index)
        {
            Vector3 mahjongSize = MahjongManager.MagjongSize;

            return new Vector3(mahjongSize.x * (index.x + 0.5f), -mahjongSize.y * (index.y + 0.5f), -mahjongSize.z * (index.z+0.5f));
        }

        public MahjongItem PopMahjong()
        {
            MahjongItem item = MahjongList[MahjongList.Count - 1];
            MahjongList.RemoveAt(MahjongList.Count - 1);
            MahjongManager.Instance.Recycle(item);

            return item;
        }

        public MahjongItem GetLastMahjong()
        {
            if (MahjongList.Count == 0) return null;
            return MahjongList[MahjongList.Count - 1];
        }

        public void SignItemByValueGreen(int value)
        {
            for (int i = 0; i < MahjongList.Count; i++)
            {
                if (MahjongList[i].Value == value)
                {
                    MahjongList[i].ShowGreen();
                }
            }
        }

        public void ReplyItem()
        {
            for (int i = 0; i < MahjongList.Count; i++)
            {
                MahjongList[i].ShowNormal();
            }
        }
    }
}
