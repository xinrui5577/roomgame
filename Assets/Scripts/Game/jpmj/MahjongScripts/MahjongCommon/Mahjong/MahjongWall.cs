using System.Collections.Generic;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong
{
    public class MahjongWall : MahjongGroup
    {
        public int StartIndex = 0;

        public void SetRowCnt(int cnt)
        {
            RowCnt = cnt;
            MahjongCnt = RowCnt*2;
        }

        public override MahjongItem GetInMahjong(int value)
        {
            if (value!=0)
            {
                MahjongCnt = value;
            }

            if(MahjongCnt<=0)return null;

            //在麻将管理中取麻将
            List<MahjongItem> list = MahjongManager.Instance.GetMahjongList(MahjongCnt);

            AddMahjong(list);

            SetMahjongPos();

            return null;
        }

        public override List<MahjongItem> GetInMahjong(int[] value)
        {
            YxDebug.LogError("函数中无函数体~~~~~！！！！");
            return null;
        }

        public MahjongItem PopMahjong()
        {
            MahjongItem item = MahjongList[StartIndex];
            MahjongList.RemoveAt(StartIndex);
            MahjongManager.Instance.Recycle(item);

            if (StartIndex >= MahjongList.Count)
            {
                StartIndex = 0;
                EventDispatch.Dispatch((int)GameEventId.WallMahjongFinish, new EventData());
            }               

            return item;
        }

        /// <summary>
        /// 倒序摸牌
        /// </summary>
        /// <returns></returns>
        public MahjongItem RevPopMahjong()
        {
            int index=0;
            var mayIndex = MahjongList.Count - 2;
            if (mayIndex >= 0 && MahjongList.Count%2 == 0)
            {
                index = mayIndex;
            }
            else
            {
                index = MahjongList.Count - 1;
            }
            MahjongItem item = MahjongList[index];
            MahjongList.RemoveAt(index);
            MahjongManager.Instance.Recycle(item);

            if (MahjongList.Count<=0)
                EventDispatch.Dispatch((int)GameEventId.RevWallMahjongFinish, new EventData());

            return item;
        }

        public void PopMahjong(int cnt)
        {
            for (int i = 0; i < cnt; i++)
            {
                MahjongItem item = MahjongList[StartIndex];
                MahjongList.RemoveAt(StartIndex);
                MahjongManager.Instance.Recycle(item);

                if (StartIndex >= MahjongList.Count)
                {
                    StartIndex = 0;
                    EventDispatch.Dispatch((int)GameEventId.WallMahjongFinish, new EventData(cnt - (i + 1)));
                    break;
                }
            }
        }

        public MahjongItem GetNowFristMj()
        {
            return MahjongList[StartIndex];
        }

        public MahjongItem GetNowEndMj()
        {
            return MahjongList[StartIndex-1];
        }

        protected override Vector3 GetPos(MjIndex index)
        {
            Vector3 mahjongSize = MahjongManager.MagjongSize;
            float dis = RowCnt*mahjongSize.x/2;

            if (index.x % 2 == 0)
                index.y = 1;
            else
                index.y = 0;

            return new Vector3(dis - mahjongSize.x * (index.x / 2 + 0.5f), mahjongSize.y * (0.5f), mahjongSize.z * (index.y + 0.5f));
        }


        protected override MjIndex GetNextIndex(MjIndex mjIndex)
        {
            MjIndex next = new MjIndex(mjIndex);
            next.x++;
            return next;
        }
        //用来处理 获取最后麻将时候 index 的偏移

        public override MahjongItem GetLastMj()
        {
            MahjongItem item = null;
            if (MahjongList.Count == 0)
            {
                return null;
            }
            else if (MahjongList.Count == 1)
            {
                item = MahjongList[0];
                MahjongList.RemoveAt(0);
                return item;
            }
            int index = MahjongList.Count - 2;
            if (MahjongList[index].transform.position.y > 0.4f)
            {
                item = MahjongList[index];
                MahjongList.RemoveAt(index);
                return item;
            }
            item = MahjongList[index + 1];
            MahjongList.RemoveAt(index + 1);
            return item;

        }
    }
}
