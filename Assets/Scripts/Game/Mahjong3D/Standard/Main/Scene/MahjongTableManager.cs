using System.Collections.Generic;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongTableManager : SceneManagerBase
    {
        public List<MahjongTablePart> MahjongTableParts;

        public override void OnInitalization()
        {
            for (int i = 0; i < MahjongTableParts.Count; i++)
            {
                MahjongTableParts[i].OnInitalization();
            }
        }

        /// <summary>
        /// 获取麻将桌部件
        /// </summary>
        /// <typeparam name="Part">部件类型</typeparam>
        /// <param name="Index">索引</param>
        /// <returns></returns>
        public Part GetParts<Part>(TablePartsType type) where Part : MahjongTablePart
        {
            var part = MahjongTableParts.Find((p) => p.PartType == type);
            if (part != null)
            {
                return part as Part;
            }
            return null;
        }

        public override void OnReset()
        {
            for (int i = 0; i < MahjongTableParts.Count; i++)
            {
                MahjongTableParts[i].OnReset();
            }
        }

        #region 东南西北方向
        /// <summary>
        /// 切换东南西北方向
        /// </summary>
        /// <param name="seat">服务器座位号</param>  
        public void SwitchDirection(int seat)
        {
            int newChair = seat;
            switch (GameCenter.DataCenter.MaxPlayerCount)
            {
                case 2: if (newChair == 1) newChair = 2; break;
                case 3:
                    {
                        newChair = MahjongUtility.GetChair(newChair);
                        if (newChair == 2)
                        {
                            newChair = 3;
                        }
                    }
                    break;
            }
            GetParts<MahjongTableDnxb>(TablePartsType.DnxbDirection).SwitchDirection((DnxbDirection)newChair);
        }
        #endregion

        #region 箭头     
        public void ShowOutcardFlag(MahjongContainer item)
        {
            if (!item.ExIsNullOjbect())
            {
                GetParts<MahjongOutCardFlag>(TablePartsType.OutCardFlag).Show(item);
            }
        }

        public void HideOutcardFlag()
        {
            GetParts<MahjongOutCardFlag>(TablePartsType.OutCardFlag).Hide();
        }
        #endregion

        #region 骰子
        /// <summary>
        /// 二个骰子
        /// </summary> 
        public void PlaySaiziAnimation(byte val1, byte val2, Action action = null)
        {
            GetParts<MahjongSaizi>(TablePartsType.Saizi).PlaySaiziAnimation(val1, val2, action);
            HideMahjongCounter();
            HideTimer();
        }

        /// <summary>
        /// 一个骰子
        /// </summary>  
        public void PlaySaiziAnimation(byte val1, Action action = null)
        {
            GetParts<MahjongSaizi>(TablePartsType.Saizi).PlaySaiziAnimation(val1, action);
            HideTimer();
        }

        public void HideSaizi()
        {
            GetParts<MahjongSaizi>(TablePartsType.Saizi).HideSaizi();
        }
        #endregion

        #region 翻牌
        /// <summary>
        /// 设置显示赖子
        /// </summary>
        public MahjongContainer SetShowMahjong(int card)
        {
            return GetParts<MahjongDisplayCard>(TablePartsType.DisplayCard).SetMahjong(card, GameCenter.DataCenter.Game.LaiziCard);
        }

        /// <summary>
        /// 设置显示宝牌
        /// </summary> 
        public MahjongContainer SetShowBao(int bao, bool isShow = true)
        {
            return GetParts<MahjongDisplayCard>(TablePartsType.DisplayCard).SetBaoMahjong(bao, isShow);
        }
        #endregion

        #region 时钟
        public void StartTimer(int time, Action callBack = null)
        {
            var timer = GetParts<MahjongTimerCtrl>(TablePartsType.Timer);
            if (timer != null)
            {
                timer.StartTimer(time, callBack);
            }
        }

        public void HideTimer()
        {
            var timer = GetParts<MahjongTimerCtrl>(TablePartsType.Timer);
            if (timer != null)
            {
                timer.OnReset();
            }           
        }

        public void StopTimer()
        {
            GetParts<MahjongTimerCtrl>(TablePartsType.Timer).StopTimer();
        }
        #endregion

        #region 桌子
        public void SwitchTableSkin()
        {
            GetParts<MahjongTable>(TablePartsType.Table).SwitchTableSkin();
        }
        #endregion

        #region 计数器
        public void MahjongCounter(int number)
        {
            var counter = GetParts<MahjongCounter>(TablePartsType.MahjongCounter);
            if (counter != null)
            {
                counter.SetMahjongCounter(number);
            }
        }

        public void HideMahjongCounter()
        {
            var counter = GetParts<MahjongCounter>(TablePartsType.MahjongCounter);
            if (counter != null)
            {
                counter.OnReset();
            }
        }
        #endregion
    }
}