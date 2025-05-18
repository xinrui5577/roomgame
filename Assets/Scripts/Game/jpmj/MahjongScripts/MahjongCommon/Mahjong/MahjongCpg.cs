using System.Collections.Generic;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using Assets.Scripts.Game.jpmj;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong
{
    public class MahjongCpg : MonoBehaviour
    {
        public int ArrowCrossIndex
        {
            get { return _arrowCrossIndex; }
        }
        private int _arrowCrossIndex = UtilDef.NullArrowIndex;

        public CpgData Data;
        public int AcrossIndex;
        public List<MahjongItem> MahjongList;

        protected float _offsetX;
        public float OffsetX
        {
            get { return _offsetX; }
        }
        //创建之后的初始化
        public virtual void Init(CpgData data, int index, int arrowindex = UtilDef.NullArrowIndex)
        {
            _arrowCrossIndex = arrowindex;

            Data = data;
            AcrossIndex = index;
            //加入麻将
            AddMahjong();
            //校验横着的位子
            if(GameConfig.IsCpgHeng&&index == UtilDef.DefInt)GetAcrossIndex();
            //SortMahjong();

            //检查指示箭头和赖子杠头标志
            CheckSignAndArrow();
            SortMahjong();
        }

        /// <summary>
        /// 检查指示箭头和财神标志
        /// </summary>
        private void CheckSignAndArrow()
        {
            if (MahjongList == null || MahjongList.Count < 1) return;
            var mj = MahjongList[0];
            if (mj.Value == JpMahjongPlayerHard.CaishenValue)
            {
                MahjongList[MahjongList.Count - 1].IsSign = true;
                mj.HideArrow();
                return;
            }

            //如果不让都显示箭头标记或者标记已经显示，则不再设置显示
            if (!GameConfig.IsShowArrow) return;
            if (_arrowCrossIndex == -1)
            {
                if (Data.AcrossIndex == 1)
                    _arrowCrossIndex = 2;
                else if (Data.AcrossIndex == 2)
                    _arrowCrossIndex = 1;
                else if (Data.AcrossIndex == 3)
                    _arrowCrossIndex = 0;
                else
                    return;
            }
            mj.SetArrow(_arrowCrossIndex);
        }

        //加入麻将到当前
        protected virtual void AddMahjong()
        {
            //根据吃碰杠数据 从麻将管理中 取得麻将
            MahjongList = MahjongManager.Instance.GetMahjongList(Data.AllCards().ToArray());

            foreach (MahjongItem item in MahjongList)
            {
                item.transform.parent = transform;
                item.Reset();

                if (item.Value == Data.Laizi)
                {
                    item.IsSign = true;
                }
            }
        }
        //排序麻将别且设置位子
        protected virtual void SortMahjong()
        {
            Vector3 MahjongSize = MahjongManager.MagjongSize;
            for (int i = 0; i < MahjongList.Count; i++)
            {
                var mj = MahjongList[i];
                //设置横过来的麻将
                if (AcrossIndex != UtilDef.DefInt && i == AcrossIndex)
                {
                    mj.IsAcross = true;
                    //横过来时宽度是麻将的长度
                    _offsetX += MahjongSize.y*0.5f;
                    mj.transform.localPosition = new Vector3(_offsetX, -(MahjongSize.y - MahjongSize.x * (0.5f)), -MahjongSize.z * 0.5f);
                    _offsetX += MahjongSize.y*0.5f;
                    continue;
                }

                _offsetX += MahjongSize.x*0.5f;
                mj.transform.localPosition = new Vector3(_offsetX, -MahjongSize.y * (0.5f), -MahjongSize.z * (0.5f));
                _offsetX += MahjongSize.x*0.5f;
            }
        }
        //随机获得横着的牌的index 如不需要横着的 重写这个函数
        protected virtual void GetAcrossIndex()
        {
            if (Data.AcrossIndex == 1)
                AcrossIndex = 2;
            else if (Data.AcrossIndex == 2)
                AcrossIndex = 1;
            else if (Data.AcrossIndex == 3)
                AcrossIndex = 0;
            else
                AcrossIndex = UtilDef.DefInt;
        }
        //删除
        public virtual void Delete()
        {
            foreach (MahjongItem item in MahjongList)
            {
                MahjongManager.Instance.Recycle(item);
            }

            MahjongList.Clear();

            Destroy(gameObject);
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

    public class MahjongCpgZhuaGang : MahjongCpg
    {
        //排序麻将别且设置位子
        protected override void SortMahjong()
        {
            Vector3 MahjongSize = MahjongManager.MagjongSize;
            Vector3 acrossMjPos = new Vector3();
            for (int i = 0; i < MahjongList.Count-1; i++)
            {
                var mj = MahjongList[i];
                //设置横过来的麻将
                if (AcrossIndex != UtilDef.DefInt && i == AcrossIndex)
                {
                    mj.IsAcross = true;
                    //横过来时宽度是麻将的长度
                    _offsetX += MahjongSize.y*0.5f;
                    acrossMjPos = new Vector3(_offsetX, -(MahjongSize.y - MahjongSize.x * (0.5f)), -MahjongSize.z * 0.5f);
                    mj.transform.localPosition = acrossMjPos;
                    _offsetX += MahjongSize.y*0.5f;
                    continue;
                }

                _offsetX += MahjongSize.x*0.5f;
                mj.transform.localPosition = new Vector3(_offsetX, -MahjongSize.y * (0.5f), -MahjongSize.z * (0.5f));
                _offsetX += MahjongSize.x*0.5f;
            }
            if (GameConfig.GameKey == "jpmj")
            {

                var len = MahjongList.Count;

                if (!GameConfig.IsShowArrow || UtilData.CurrGamePalyerCnt < 3)
                {
                    var ssecondMj = MahjongList[1];
                    MahjongList[len-1].transform.localPosition = ssecondMj.transform.localPosition +
                                                                                 new Vector3(0, 0, -MahjongSize.z);
                }
                else
                {
                    for (var i = 0; i < len; i++)
                    {
                        if (MahjongList[i].IsArrowActive)
                        {
                            //代方向箭头的牌放到中间上方，最后一张牌补位这张牌
                            MahjongList[len - 1].transform.localPosition =
                                MahjongList[i].transform.localPosition;
                            var ssecondMj = MahjongList[1];
                            MahjongList[i].transform.localPosition = ssecondMj.transform.localPosition +
                                                                                         new Vector3(0, 0, -MahjongSize.z);
                            break;
                        }

                    }
                }



                //MahjongList[MahjongList.Count - 1].SetArrow(MahjongList[0].ArrowAcrossIndex);

            }
            else
            {
                //设置最后一个麻将的位子 在上一个横排的上面
                MahjongList[MahjongList.Count - 1].IsAcross = true;
                MahjongList[MahjongList.Count - 1].transform.localPosition = acrossMjPos + new Vector3(0, MahjongSize.x, 0);
            }
        }
    }

    public class MahjongCpgAngang : MahjongCpgSelfGang
    {
        public override void Init(CpgData data, int index, int arrowindex = UtilDef.NullArrowIndex)
        {
            data.FromSeat = data.Seat;        ///暗杠特殊处理（隐藏角标显示处理）
            base.Init(data, index, arrowindex);
        }
        //加入麻将到当前
        protected override void AddMahjong()
        {
            //根据吃碰杠数据 从麻将管理中 取得麻将
            MahjongList = MahjongManager.Instance.GetMahjongList(Data.AllCards().ToArray());
            foreach (MahjongItem item in MahjongList)
            {
                item.transform.parent = transform;
                item.Reset();
                item.SetMjRota(0, 180, 0);
            }

        }

        protected override void SortMahjong()
        {
            Vector3 MahjongSize = MahjongManager.MagjongSize;
            var lenth = MahjongList.Count;
            for (int i = 0; i < MahjongList.Count - 1; i++)
            {
                //var mj = MahjongList[lenth-i-1];
                var mj = MahjongList[i];
                _offsetX += MahjongSize.x * 0.5f;
                mj.transform.localPosition = new Vector3(_offsetX, -MahjongSize.y * (0.5f), -MahjongSize.z * (0.5f));
                _offsetX += MahjongSize.x * 0.5f;
            }
            //设置最后一张麻将 在第二个麻将的上面
            //var ssecondMj = MahjongList[2];
            var ssecondMj = MahjongList[1];
            MahjongList[MahjongList.Count - 1].transform.localPosition = ssecondMj.transform.localPosition +new Vector3(0, 0, -MahjongSize.z);
            if (GameConfig.GameKey != "jpmj")
            {
                if (Data.Chair == 0)
                {
                    MahjongList[MahjongList.Count - 1].SetMjRota(0, 0, 0);
                }
            }//精品麻将，如果有暗杠 要所有玩家看他都是一张牌朝上其他朝下的形式
            else
            {
                //MahjongList[0].SetMjRota(0, 0, 0);
                MahjongList[MahjongList.Count - 1].SetMjRota(0, 0, 0);
            }
        }
    }

    
}