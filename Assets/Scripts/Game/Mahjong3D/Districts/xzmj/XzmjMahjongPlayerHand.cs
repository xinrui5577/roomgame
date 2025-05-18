using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class XzmjMahjongPlayerHand : MahjongPlayerHand, IGameReadyCycle
    {
        private bool mForbidCardsSort;//禁止手牌排序
        private int mPigColor;//花猪 
        private int mSelectSum = 0;//选牌数
        private int mCountWan = 0, mCountTiao = 0, mCountTong = 0;
        private List<MahjongContainer> mSelectMahList = new List<MahjongContainer>();

        public List<MahjongContainer> SelectMahList { get { return mSelectMahList; } }

        private int Color(int value)
        {
            return value & 0xf0;
        }

        private bool HasHuiPai
        {
            get
            {
                var list = MahjongList;
                for (int i = list.Count - 1; i > 0; i--)
                {
                    if ((list[i].Value >> 4) == (mPigColor >> 4))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public override void OnReset()
        {
            base.OnReset();
            mSelectSum = 0;
            mPigColor = -1;
            mSelectMahList.Clear();
        }

        /// <summary>
        /// 张数最少的花色牌
        /// </summary>
        public int LeastColor
        {
            get
            {
                CalcColorCount();
                if (mCountWan <= mCountTong && mCountWan <= mCountTiao) return 0x10;
                if (mCountTiao <= mCountWan && mCountTiao <= mCountTong) return 0x20;
                if (mCountTong <= mCountWan && mCountTong <= mCountTiao) return 0x30;
                return 0;
            }
        }

        /// <summary>
        /// 出牌事件
        /// </summary>     
        protected override void ThrowCardClickEvent(Transform mahjong)
        {
            if (GameCenter.Controller.ForbbidToken)
            {
                return;
            }
            base.ThrowCardClickEvent(mahjong);
        }

        /// <summary>
        /// 计算各个花色牌张数
        /// </summary>
        private void CalcColorCount()
        {
            mCountWan = 0;
            mCountTiao = 0;
            mCountTong = 0;
            var list = MahjongList;
            for (int i = 0; i < list.Count; i++)
            {
                int mColor = Color(list[i].Value);
                mCountWan += (mColor == 0x10 ? 1 : 0);
                mCountTiao += (mColor == 0x20 ? 1 : 0);
                mCountTong += (mColor == 0x30 ? 1 : 0);
            }
        }

        protected override void Start()
        {
            base.Start();
            GameCenter.RegisterCycle(this);
            mPutOutFunc = (item) => 
            {
                return !((item.Value >> 4) == (mPigColor >> 4)) && HasHuiPai;
            };

            AddActionToDic(HandcardStateTyps.Dingqueing, OnDingqueing);
            AddActionToDic(HandcardStateTyps.DingqueOver, ChangeHandMahGray);
            AddActionToDic(HandcardStateTyps.ExchangeCards, OnExchangeCards);
        }

        public void OnGameReadyCycle()
        {
            mForbidCardsSort = false;
        }

        public override void OnSendOverSortMahjong(float time, float wait)
        {
            if (mForbidCardsSort)
            {
                return;
            }
            base.OnSendOverSortMahjong(time, wait);
        }

        /// <summary>
        /// 开始换张
        /// </summary>
        private void OnExchangeCards(params object[] args)
        {
            mForbidCardsSort = true;
            mSelectSum = (int)args[0];
            //切换为选牌状态
            SwitchSelectCardsState();
            //设置提示牌
            AddRecommondCards2Switch();
            //置灰少于3张的花色牌
            MakeLessCardsGray();
            GameCenter.Scene.TableManager.StartTimer(GameCenter.DataCenter.Config.TimeHuancard);
        }

        public void OnDingqueing(params object[] args)
        {
            GameCenter.Scene.TableManager.StartTimer(GameCenter.DataCenter.Config.TimeDingque);
        }

        /// <summary>
        /// 选花色完成 把手牌花猪置灰
        /// </summary>      
        public void ChangeHandMahGray(params object[] args)
        {
            mPigColor = (int)args[0];
            for (int i = 0; i < MahjongList.Count; i++)
            {
                int iValue = MahjongList[i].Value;
                if ((mPigColor >> 4) == (iValue >> 4))
                {
                    MahjongList[i].ShowGray();
                }
            }
            //排序
            SortMahjong();
            SetMahjongPos();
        }

        /// <summary>
        /// 切换为选换牌状态
        /// </summary>
        private void SwitchSelectCardsState()
        {
            MahjongContainer item = null;
            MahjongContorl.ClearSelectCard();
            var list = MahjongList;
            for (int i = 0; i < list.Count; i++)
            {
                item = list[i];
                item.SetAllowOffsetStatus(false);
                item.SetThowOutCall(SwitchCardsClickEvent);
                item.Lock = false;
                //屏蔽牌的select事件
                item.SetSelectFlag(true);
                item.ResetPos();
            }
        }

        /// <summary>
        /// 扣牌前 把少于三张的花色 置灰
        /// </summary>
        public void MakeLessCardsGray()
        {
            CalcColorCount();
            var list = MahjongList;
            for (int i = 0; i < list.Count; i++)
            {
                int iColor = Color(list[i].Value);
                if ((mCountWan < 3 && 0x10 == iColor) || (mCountTong < 3 && 0x30 == iColor) || (mCountTiao < 3 && 0x20 == iColor))
                {
                    list[i].Lock = true;
                    list[i].RemoveMahjongScript();
                }
            }
        }

        /// <summary>
        /// 换牌点击事件
        /// </summary>
        /// <param name="transf"></param>
        private void SwitchCardsClickEvent(Transform transf)
        {
            var chooseMj = transf.GetComponent<MahjongContainer>();
            var selectColor = Color(chooseMj.Value);
            bool sameColorFlag = true;

            if (mSelectMahList.Count > 0)
            {
                var currColor = Color(mSelectMahList[0].Value);
                sameColorFlag = selectColor == currColor;
            }

            if (sameColorFlag)
            {
                if (mSelectMahList.Count <= 3)
                {
                    var bIsFound = false;
                    for (int i = 0; i < mSelectMahList.Count; i++)
                    {
                        if (chooseMj.Value == mSelectMahList[i].Value && chooseMj.MahjongIndex == mSelectMahList[i].MahjongIndex)
                        {
                            bIsFound = true;
                            mSelectMahList.RemoveAt(i);
                            chooseMj.Tweener.Down();
                            break;
                        }
                    }
                    if (!bIsFound && mSelectMahList.Count < 3)
                    {
                        mSelectMahList.Add(chooseMj);
                        chooseMj.Tweener.Up();
                    }
                }
            }
            else
            {
                //重置花色
                for (int i = 0; i < mSelectMahList.Count; i++)
                {
                    mSelectMahList[i].Tweener.Down();
                }
                mSelectMahList.Clear();

                //添加新花色
                mSelectMahList.Add(chooseMj);
                chooseMj.Tweener.Up();
            }
        }

        /// <summary>
        /// 换牌超时 服务端自动换牌
        /// </summary>     
        public void SwitchCardsTimeOut(int[] buckles)
        {
            // 先把选中的牌还原
            for (int i = 0; i < SelectMahList.Count; i++)
            {
                SelectMahList[i].Tweener.Down();
            }
            SelectMahList.Clear();
            for (int i = 0; i < buckles.Length; i++)
            {
                RemoveMahjong(buckles[i]);
            }
        }

        public void AddRecommondCards2Switch()
        {
            if (mSelectMahList.Count >= 3) return;
            CalcColorCount();
            // 找到推荐的扣牌
            int[] colorValue = { 0x10, 0x20, 0x30 };
            int[] colorCount = { mCountWan, mCountTiao, mCountTong };
            // 冒泡把牌最少的门放最前
            for (int i = 0; i < 3; i++)
            {
                for (int j = i + 1; j < 3; j++)
                {
                    if (colorCount[j] < colorCount[i])
                    {
                        int iTempValue = colorValue[i];
                        colorValue[i] = colorValue[j];
                        colorValue[j] = iTempValue;
                        int iTempCount = colorCount[i];
                        colorCount[i] = colorCount[j];
                        colorCount[j] = iTempCount;
                    }
                }
            }
            int switchColor = 0;
            // 牌数小于3的花色不做考虑
            for (int i = 0; i < 3; i++)
            {
                if (3 <= colorCount[i])
                {
                    switchColor = colorValue[i];
                    break;
                }
            }
            // 先把选中的牌还原        
            for (int i = 0; i < mSelectMahList.Count; i++)
            {
                SelectMahList[i].Tweener.Down();
            }
            mSelectMahList.Clear();
            var list = MahjongList;
            // 找到符合条件的牌 加入列表
            for (int i = 0; i < list.Count; i++)
            {
                var entity = list[i];
                if (switchColor == (entity.Value & 0xf0))
                {
                    entity.Tweener.Up();
                    mSelectMahList.Add(entity);
                }
                if (mSelectMahList.Count == mSelectSum)
                {
                    break;
                }
            }
        }

        //重连回来时已经确定换三张
        public void OnRejoinConfirmedSwitchCards(int[] array)
        {
            List<MahjongContainer> list = GameCenter.Scene.MahjongCtrl.PopMahjong(array);
            if (list != null)
            {
                mSelectMahList.AddRange(list);
            }
        }

        public override MahjongContainer GetInMahjong(int value)
        {
            var item = base.GetInMahjong(value);
            if ((item.Value >> 4) == (mPigColor >> 4))
            {
                item.ShowGray();
            }
            return item;
        }

        protected override void SortMahjong()
        {
            MahjongList.Sort((a, b) =>
            {
                if ((a.Value >> 4) == (mPigColor >> 4) && (b.Value >> 4) != (mPigColor >> 4)) return 1;
                if ((a.Value >> 4) != (mPigColor >> 4) && (b.Value >> 4) == (mPigColor >> 4)) return -1;
                if (a.Value < b.Value) return -1;
                if (a.Value > b.Value) return 1;
                if (a.Value == b.Value)
                {
                    if (a.MahjongIndex > b.MahjongIndex) return 1;
                    if (a.MahjongIndex < b.MahjongIndex) return -1;
                }
                return 0;
            });
        }

        //换张之后再重新排序手牌
        public void AgainSortHandCards()
        {
            var list = MahjongList;
            list.Sort((a, b) =>
            {
                //先判断是否是赖子
                if (a.Laizi && !b.Laizi)
                    return -1;
                if (!a.Laizi && b.Laizi)
                    return 1;
                if (a.Value < b.Value)
                    return -1;
                if (a.Value > b.Value)
                    return 1;
                if (a.Value == b.Value)
                {
                    if (a.MahjongIndex > b.MahjongIndex)
                        return -1;
                    if (a.MahjongIndex < b.MahjongIndex)
                        return 1;
                }
                return 0;
            });
            MahjongVecter NowIndex = new MahjongVecter();
            for (int i = 0; i < list.Count; i++)
            {
                Transform mahjongTf = list[i].transform;
                Vector3 pos = AgainGetPos(NowIndex, list.Count, RowCnt);
                mahjongTf.localPosition = pos;
                NowIndex = GetNextMjIndex(NowIndex, RowCnt, ColCnt);
            }
        }

        public Vector3 AgainGetPos(MahjongVecter index, int mahjongCnt, int RowCnt)
        {
            Vector3 mahjongSize = DefaultUtils.MahjongSize;
            float Dis = -mahjongCnt * mahjongSize.x / 2 - mahjongSize.x * 1.2f / 2;
            Vector3 pos = new Vector3(Dis + mahjongSize.x * (index.x + 0.5f), mahjongSize.y * 0.5f, mahjongSize.z * 0.5f);
            pos.x = pos.x - (RowCnt - MahjongList.Count) / 2 * mahjongSize.x / 2;
            return pos;
        }

        public MahjongVecter GetNextMjIndex(MahjongVecter index, int RowCnt, int ColCnt)
        {
            MahjongVecter next = new MahjongVecter(index);
            if (RowCnt != 0 && next.x >= RowCnt - 1)
            {
                next.x = 0;
                if (next.y++ >= ColCnt - 1)
                {
                    next.z++;
                    next.y = 0;
                }
            }
            else
            {
                next.x++;
            }
            return next;
        }

        protected override bool AniSortCheck(int lastCard, int getCard)
        {
            var flag = true;
            if ((getCard >> 4) == (mPigColor >> 4) && (lastCard >> 4) == (mPigColor >> 4))
            {
                flag = lastCard > getCard;
            }
            else if ((getCard >> 4) != (mPigColor >> 4) && (lastCard >> 4) != (mPigColor >> 4))
            {
                flag = lastCard > getCard;
            }
            else if ((getCard >> 4) == (mPigColor >> 4) && (lastCard >> 4) != (mPigColor >> 4))
            {
                flag = false;
            }
            return flag;
        }
    }
}