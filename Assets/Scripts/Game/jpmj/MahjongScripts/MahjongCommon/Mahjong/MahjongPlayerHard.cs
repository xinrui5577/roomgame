using System.Collections.Generic;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public.Adpater;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong
{
    public class MahjongPlayerHard : MahjongHard
    {
        private bool _hasToken;
        public bool HasToken
        {
            get { return _hasToken; }
            set { _hasToken = value; }
        }
        public static int PlayerHardLayer = -1;
        void Awake()
        {
            PlayerHardLayer = gameObject.layer;
            OnAwake();
        }

        protected virtual void OnAwake()
        {

        }

        void Start()
        {
            if (null != GameAdpaterManager.Singleton)
            {
                transform.localScale = GameAdpaterManager.Singleton.GetConfig.HandMahjongSize;
            }
        }

        public MahjongItem ChooseMj;

        //是否允许赖子牌打出
        public bool AllowLaiziPut = false;

        protected bool _isChooseTing;
        public bool IsChooseTing
        {
            get { return _isChooseTing; }
        }

        protected override void AddMahjong(MahjongItem item)
        {
            base.AddMahjong(item);
            item.ChangeToHardLayer(true);
            item.SetMahjongScript();
            item.SetThowOutCall(OnOutPutCard);
            item.SetSelectCall(OnItemSelect);
        }

        protected virtual void OnOutPutCard(Transform transf)
        {
            //有补张时 不允许出牌
            if (App.GetRServer<NetWorkManager>().BuzhangState)
            {
                return;
            }
            //如果允许当前用户出牌
            if (HasToken)
            {
                ChooseMj = transf.GetComponent<MahjongItem>();

                if (ChooseMj.Lock) return;

                bool branch = false;

                if (!ChooseMj.IsSign)
                    branch = true;
                else
                    branch = ChooseMj.IsSign && AllowLaiziPut;

                if (branch)
                {
                    HasToken = false;
                    OutCard(ChooseMj);
                }
            }
        }

        protected virtual void OnItemSelect(GameObject target, bool isPress)
        {

        }

        protected virtual void OutCard(MahjongItem chooseMj)
        {
            int value = chooseMj.Value;
            YxDebug.Log("出牌 值 " + value);
            //通知网络 发送出牌消息
            EventDispatch.Dispatch((int)NetEventId.OnOutMahjong, new EventData(value));
            //如果打出的牌是抬起状态放下
            if (chooseMj.Roll != null && chooseMj.Roll.IsUp)
            {
                chooseMj.Roll.ResetPos();
            }
            //当用户出牌之后 需要隐藏操作菜单
            EventDispatch.Dispatch((int)UIEventId.OperationMenu, new EventData());
        }

        public override MahjongItem GetMahjongItemByValue(int value)
        {
            for (int i = MahjongList.Count - 1; i >= 0; i--)
            {
                if (MahjongList[i].Value == value)
                {
                    return MahjongList[i];
                }
            }

            string strValue = "";
            foreach (MahjongItem item in MahjongList)
            {
                strValue += "[" + item.ToString() + "] ";
            }

            YxDebug.Log("查找麻将 " + strValue + " 中值为 " + value);
            YxDebug.LogError("在玩家的手牌中 找不到 值为" + value + "的麻将");
            return null;
        }

        public override void GameResultRota(float time)
        {
            foreach (MahjongItem item in MahjongList)
            {
                item.ChangeToHardLayer(false);
                item.RemoveMahjongScript();
            }

            base.GameResultRota(time);
        }

        public override void SetLastCardPos(int value)
        {
            if (value == UtilDef.NullMj || MahjongList.Find(item => item.Value == value) == null)//当前不是抓牌 是吃碰杠后的
            {
                base.SetLastCardPos(value);
                return;
            }

            if (MahjongList.Count > 1)
            {
                MahjongItem findItem = MahjongList.Find((item) =>
                {
                    return item.Value == value;
                });
                if (findItem != null)
                {
                    MahjongList.Remove(findItem);
                }

                MahjongList.Add(findItem);

                SetMahjongPos();

                findItem.transform.localPosition = GetHardLastMjPos();
            }
        }

        public override MahjongItem ThrowOut(int value)
        {
            MahjongItem Mahjong = base.ThrowOut(value);
            if (Mahjong)
            {
                Mahjong.ChangeToHardLayer(false);
            }
            return Mahjong;
        }

        protected override Vector3 GetPos(MjIndex index)
        {
            Vector3 mahjongSize = MahjongManager.MagjongSize;
            Vector3 pos = base.GetPos(index);
            pos.x = pos.x - (RowCnt - MahjongList.Count) / 2 * mahjongSize.x / 2;

            return pos;
        }

        public override void OnSendOverSortMahjong(float time, float wait, int laizi)
        {
            foreach (MahjongItem item in MahjongList)
            {
                item.RotaTo(new Vector3(-90, 0, 0), time);
            }

            DelayTimer.StartTimer(wait, () =>
            {
                SetLaizi(laizi);

                foreach (MahjongItem item in MahjongList)
                {
                    item.RotaTo(new Vector3(0, 0, 0), time);
                }
            });
        }

        public virtual void SetChooseTingPai(int[] tings)
        {
            _isChooseTing = true;
            var tingList = new List<int>(tings);
            foreach (MahjongItem item in MahjongList)
            {
                if (!tingList.Contains(item.Value))
                {
                    item.Lock = true;
                    //如果打出的牌是抬起状态放下
                    if (item.Roll != null && item.Roll.IsUp)
                    {
                        item.Roll.ResetPos();
                    }
                    item.RemoveMahjongScript();
                }
                else
                {
                    item.SetMahjongScript();
                    item.SetThowOutCall(TingPaiClick);
                }
            }
        }

        protected virtual void TingPaiClick(Transform transf)
        {
            if (HasToken)
            {
                var Mj = transf.GetComponent<MahjongItem>();
                if (!Mj.IsSign && !Mj.Lock)
                {
                    HasToken = false;
                    TingPai(Mj);

                    foreach (MahjongItem item in MahjongList)
                    {
                        item.SetMahjongScript();
                        item.SetThowOutCall(OnOutPutCard);
                    }
                }
            }
        }

        protected virtual void TingPai(MahjongItem chooseMj)
        {
            int value = chooseMj.Value;
            YxDebug.Log("听牌 值 " + value);
            //通知网络 发送出牌消息
            EventDispatch.Dispatch((int)NetEventId.OnTingPai, new EventData(value));
            //如果打出的牌是抬起状态放下
            if (chooseMj.Roll != null && chooseMj.Roll.IsUp)
            {
                chooseMj.Roll.ResetPos();
            }
            //当用户出牌之后 需要隐藏操作菜单
            EventDispatch.Dispatch((int)UIEventId.OperationMenu, new EventData());
        }

        public virtual void OnCancelTing()
        {
            foreach (MahjongItem item in MahjongList)
            {
                item.Lock = false;

                item.SetMahjongScript();
                item.SetThowOutCall(OnOutPutCard);

                //如果打出的牌是抬起状态放下
                if (item.Roll != null && item.Roll.IsUp)
                {
                    item.Roll.ResetPos();
                }
            }

            _isChooseTing = false;
        }

        public override void SetTingPai()
        {
            _isTing = true;
            _isChooseTing = false;

            foreach (MahjongItem item in MahjongList)
            {
                item.Lock = true;

                //如果打出的牌是抬起状态放下
                if (item.Roll != null && item.Roll.IsUp)
                {
                    item.Roll.ResetPos();
                }

                item.RemoveMahjongScript();
            }
        }

        public override void SetTingPaiNeedOutCard()
        {
            var lastMj = GetLastMj();
            lastMj.Lock = false;
            lastMj.SetMahjongScript();
            lastMj.SetThowOutCall(OnOutPutCard);
        }

        protected override void SortMahjongForHand()
        {
            SortMahjong();
        }

        //托管
        public void Trusteeship(bool isOn)
        {
            //托管中
            if (isOn)
            {
                foreach (MahjongItem item in MahjongList)
                {
                    item.Lock = true;
                    //如果打出的牌是抬起状态放下
                    if (item.Roll != null)
                    {
                        item.Roll.ResetPos();
                        item.Roll.IsHoverEnable = false;
                    }                   
                    item.ShowGray();
                    item.ChangeWaitForXjfd(true);
                    item.RemoveMahjongScript();
                }
            }
            else
            {
                foreach (MahjongItem item in MahjongList)
                {
                    item.SetMahjongScript();
                    item.ChangeWaitForXjfd(false);                   
                    item.SetThowOutCall(OnOutPutCard);
                    item.ShowNormal();
                    item.Lock = false;
                    if (item.Roll != null)
                        item.Roll.IsHoverEnable = true;
                }
            }
        }

        //当抢杠胡
        public virtual void OnQiangganghu(int value)
        {
            MahjongItem findItem = MahjongList.Find((item) =>
            {
                return item.Value == value;
            });

            MahjongManager.Instance.Recycle(findItem);
            MahjongList.Remove(findItem);
            SortMahjongForHand();
            findItem.gameObject.SetActive(false);
        }

        public void SetCardsNegative()
        {
            foreach (MahjongItem item in GetMahjongList)
            {
                item.Lock = true;
                item.Roll.IsHoverEnable = false;
                item.ChangeWaitForXjfd(true);
                //如果打出的牌是抬起状态放下
                if (item.Roll != null) item.Roll.ResetPos();                
                item.SetThowOutCall((a) => { });               
            }
            SetMahjongPos();
        }

        public void SetCardsActive()
        {
            foreach (MahjongItem item in MahjongList)
            {
                item.Lock = false;
                item.Roll.IsHoverEnable = true;
                item.ChangeWaitForXjfd(false);

                if (item.Roll != null) item.Roll.ResetPos();
                item.SetSelectCall(OnItemSelect);
                item.SetThowOutCall(OnOutPutCard);
                item.ShowNormal();
            }
            SetMahjongPos();
        }

        public void MahjongsResetPos()
        {
            foreach (MahjongItem item in MahjongList)
            {
                var v3 = item.transform.localPosition;
                v3.y = 0.2f;
                item.transform.localPosition = v3;
            }
        }
    }
}

