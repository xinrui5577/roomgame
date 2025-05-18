using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongCpg : MonoBehaviour
    {
        public CpgData Data;
        public int AcrossIndex;
        public List<MahjongContainer> MahjongList;

        protected float _offsetX = 0;
        public float OffsetX { get { return _offsetX; } }

        //创建之后的初始化
        public virtual void Init(CpgData data, int index)
        {
            if (null == data) return;
            Data = data;
            AcrossIndex = index;
            //加入麻将
            AddMahjong();
            //校验横着的位子
            if (index == DefaultUtils.DefInt) GetAcrossIndex();
            SortMahjong();
        }

        //加入麻将到当前
        protected virtual void AddMahjong()
        {
            MahjongContainer item;
            //根据吃碰杠数据 从麻将管理中 取得麻将
            MahjongList = GameCenter.Scene.MahjongCtrl.PopMahjong(Data.AllCards().ToArray());
            for (int i = 0; i < MahjongList.Count; i++)
            {
                item = MahjongList[i];
                item.transform.ExSetParent(transform);
                if (item.Value == Data.Laizi)
                {
                    item.Laizi = true;
                }
            }
        }

        //排序麻将别且设置位子
        protected virtual void SortMahjong()
        {
            Vector3 MahjongSize = DefaultUtils.MahjongSize;
            var tempList = MahjongList.ToArray();
            for (int i = 0; i < tempList.Length; i++)
            {
                if (tempList[i].Value == Data.Card && Data.Type == EnGroupType.Chi)
                {
                    var temp = tempList[AcrossIndex];
                    tempList[AcrossIndex] = tempList[i];
                    tempList[i] = temp;
                }
            }
            for (int i = 0; i < tempList.Length; i++)
            {
                var mj = tempList[i];
                //设置横过来的麻将
                if (AcrossIndex != DefaultUtils.DefInt && i == AcrossIndex)
                {
                    mj.IsAcross = true;
                    //横过来时宽度是麻将的长度
                    _offsetX += MahjongSize.y * 0.5f;
                    mj.transform.localPosition = new Vector3(_offsetX, -(MahjongSize.y - MahjongSize.x * (0.5f)), -MahjongSize.z * 0.5f);
                    _offsetX += MahjongSize.y * 0.5f;
                    continue;
                }
                _offsetX += MahjongSize.x * 0.5f;
                mj.transform.localPosition = new Vector3(_offsetX, -MahjongSize.y * (0.5f), -MahjongSize.z * (0.5f));
                _offsetX += MahjongSize.x * 0.5f;
                if (Data.Chair == 2 && GameCenter.DataCenter.Config.MahjongTowardsMe)
                {
                    DuiMianRotate(mj);
                }
            }
        }

        //随机获得横着的牌的index 如不需要横着的 重写这个函数BHY^
        protected virtual void GetAcrossIndex()
        {
            if (Data.AcrossIndex == 1)
                AcrossIndex = 2;
            else if (Data.AcrossIndex == 2)
                AcrossIndex = 1;
            else if (Data.AcrossIndex == 3)
                AcrossIndex = 0;
            else
                AcrossIndex = DefaultUtils.DefInt;
        }

        //删除
        public virtual void OnReset()
        {
            for (int i = 0; i < MahjongList.Count; i++)
            {
                GameCenter.Scene.MahjongCtrl.PushMahjongToPool(MahjongList[i]);
            }
            MahjongList.Clear();
            Destroy(gameObject);
        }

        protected void DuiMianRotate(MahjongContainer item)
        {
            var x = item.transform.localEulerAngles.x;
            var y = item.transform.localEulerAngles.y;
            var z = item.transform.localEulerAngles.z;
            item.transform.localRotation = Quaternion.Euler(new Vector3(x, y, 180f));
        }
    }

    public class MahjongCpgChi : MahjongCpg
    {
        //吃牌时横拍的位子是根据吃的牌来的
        protected override void GetAcrossIndex()
        {
            for (int i = 0; i < MahjongList.Count; i++)
            {
                if (MahjongList[i].Value == Data.Card)
                {
                    AcrossIndex = i;
                    break;
                }
            }
        }
    }

    public class MahjongCpgSelfGang : MahjongCpg
    {
        //玩家手中牌的杠不需要横牌
        protected override void GetAcrossIndex()
        {
        }
    }

    public class MahjongCpgXFGang : MahjongCpg
    {
        //玩家手中牌的杠不需要横牌
        protected override void SortMahjong()
        {
            Vector3 MahjongSize = DefaultUtils.MahjongSize;
            for (int i = 0; i < MahjongList.Count; i++)
            {
                var mj = MahjongList[i];
                _offsetX += MahjongSize.x * 0.5f;
                mj.transform.localPosition = new Vector3(_offsetX, -MahjongSize.y * (0.5f), -MahjongSize.z * (0.5f));
                _offsetX += MahjongSize.x * 0.5f;
            }
        }
    }

    public class MahjongCpgZhuaGang : MahjongCpg
    {
        //排序麻将别且设置位子
        protected override void SortMahjong()
        {
            Vector3 MahjongSize = DefaultUtils.MahjongSize;
            Vector3 acrossMjPos = new Vector3();
            for (int i = 0; i < MahjongList.Count - 1; i++)
            {
                var mj = MahjongList[i];
                //设置横过来的麻将
                if (AcrossIndex != DefaultUtils.DefInt && i == AcrossIndex)
                {
                    mj.IsAcross = true;
                    //横过来时宽度是麻将的长度
                    _offsetX += MahjongSize.y * 0.5f;
                    acrossMjPos = new Vector3(_offsetX, -(MahjongSize.y - MahjongSize.x * (0.5f)), -MahjongSize.z * 0.5f);
                    mj.transform.localPosition = acrossMjPos;
                    _offsetX += MahjongSize.y * 0.5f;
                    continue;
                }
                _offsetX += MahjongSize.x * 0.5f;
                mj.transform.localPosition = new Vector3(_offsetX, -MahjongSize.y * (0.5f), -MahjongSize.z * (0.5f));
                _offsetX += MahjongSize.x * 0.5f;
                if (Data.Chair == 2 && GameCenter.DataCenter.Config.MahjongTowardsMe)
                {
                    DuiMianRotate(mj);
                }
            }
            //设置最后一个麻将的位子 在上一个横排的上面
            MahjongList[MahjongList.Count - 1].IsAcross = true;
            MahjongList[MahjongList.Count - 1].transform.localPosition = acrossMjPos + new Vector3(0, MahjongSize.x, 0);
        }
    }

    public class MahjongCpgAnJuegang : MahjongCpgSelfGang
    {
        //加入麻将到当前
        protected override void AddMahjong()
        {
            MahjongContainer item;
            //根据吃碰杠数据 从麻将管理中 取得麻将
            MahjongList = GameCenter.Scene.MahjongCtrl.PopMahjong(Data.AllCards());
            for (int i = 0; i < MahjongList.Count; i++)
            {
                item = MahjongList[i];
                item.transform.ExSetParent(transform);
                //item.SetMjRota(0, 180, 0);
            }
        }

        protected override void SortMahjong()
        {
            Vector3 MahjongSize = DefaultUtils.MahjongSize;
            for (int i = 0; i < MahjongList.Count; i++)
            {
                var mj = MahjongList[i];
                _offsetX += MahjongSize.x * 0.5f;
                mj.transform.localPosition = new Vector3(_offsetX, -MahjongSize.y * (0.5f), -MahjongSize.z * (0.5f));
                _offsetX += MahjongSize.x * 0.5f;
                if (Data.Chair == 2 && GameCenter.DataCenter.Config.MahjongTowardsMe)
                {
                    DuiMianRotate(mj);
                }
                //mj.SetMjRota(0, 180, 0);
            }
        }
    }

    public class MahjongCpgAngang : MahjongCpgSelfGang
    {
        //加入麻将到当前
        protected override void AddMahjong()
        {
            MahjongContainer item;
            //根据吃碰杠数据 从麻将管理中 取得麻将
            MahjongList = GameCenter.Scene.MahjongCtrl.PopMahjong(Data.AllCards().ToArray());
            for (int i = 0; i < MahjongList.Count; i++)
            {
                item = MahjongList[i];
                item.transform.ExSetParent(transform);
                item.SetMjRota(0, 180, 0);
            }
        }

        protected override void SortMahjong()
        {
            Vector3 MahjongSize = DefaultUtils.MahjongSize;
            for (int i = 0; i < MahjongList.Count; i++)
            {
                var mj = MahjongList[i];
                _offsetX += MahjongSize.x * 0.5f;
                mj.transform.localPosition = new Vector3(_offsetX, -MahjongSize.y * (0.5f), -MahjongSize.z * (0.5f));
                _offsetX += MahjongSize.x * 0.5f;
                if (Data.Chair == 2 && GameCenter.DataCenter.Config.MahjongTowardsMe)
                {
                    DuiMianRotate(mj);
                }
                mj.SetMjRota(0, 180, 0);
            }
            if (GameCenter.DataCenter.Config.ShowAnGang || Data.Chair == 0)
            {
                MahjongList[MahjongList.Count - 1].SetMjRota(0, 0, 0);
            }
        }
    }

    public class MahjongCpgXjfdGang : MahjongCpgSelfGang
    {
        protected override void AddMahjong()
        {
            MahjongList = new List<MahjongContainer>();
            int value = 0;
            MahjongContainer item;
            var list = Data.AllCards();
            Dictionary<int, MahjongContainer> dic = new Dictionary<int, MahjongContainer>();
            for (int i = 0; i < list.Count; i++)
            {
                value = list[i];
                if (dic.ContainsKey(value))
                {
                    dic[value].Number++;
                }
                else
                {
                    item = GameCenter.Scene.MahjongCtrl.PopMahjong(value);
                    dic.Add(value, item);
                    dic[value].Number = 1;
                    MahjongList.Add(item);
                    item.transform.ExSetParent(transform);
                    if (item.Value == Data.Laizi)
                    {
                        item.Laizi = true;
                    }
                }
            }
        }

        public void AddXjfd(MahjongContainer item)
        {
            _offsetX = 0;
            bool isHave = false;
            for (int i = 0; i < MahjongList.Count; i++)
            {
                if (MahjongList[i].Value == item.Value)
                {
                    MahjongList[i].Number++;
                    isHave = true;
                }
            }
            if (!isHave)
            {
                var obj = GameCenter.Scene.MahjongCtrl.PopMahjong(item.Value);
                MahjongList.Add(obj);
                obj.Number = 1;
                obj.transform.ExSetParent(transform);
            }
            Data.GetCardDatas.Add(item.Value);
            Data.GetAllCardDatas.Add(item.Value);
            SortMahjong();
        }

        protected override void SortMahjong()
        {
            Vector3 mahjongSize = DefaultUtils.MahjongSize;
            for (int i = 0; i < MahjongList.Count; i++)
            {
                var mahjong = MahjongList[i];
                _offsetX += mahjongSize.x * 0.5f;
                mahjong.transform.localPosition = new Vector3(_offsetX, -mahjongSize.y * (0.5f), -mahjongSize.z * (0.5f));
                _offsetX += mahjongSize.x * 0.5f;
            }
        }
    }
}