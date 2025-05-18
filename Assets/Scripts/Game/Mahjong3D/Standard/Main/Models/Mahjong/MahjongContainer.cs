using YxFramwork.ConstDefine;
using UnityEngine;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public enum MahjongColor
    {
        Normal,
        Golden,
        Gray,
        Blue,
    }

    public class MahjongContainer : MonoBehaviour
    {
        protected Quaternion mRotaToAcross = Quaternion.Euler(0, 0, -90);
        protected bool mIsAcross;

        /// <summary>
        /// 麻将牌值
        /// </summary>
        public MahjongCard MahjongCard;
        /// <summary>
        /// 打骰子之后，确定牌再桌上的顺序
        /// </summary>
        public int TableSortIndex = -1;
        /// <summary>
        /// 排序用 先根据牌值 然后 根据index
        /// </summary>
        public int MahjongIndex = -1;

        private MahjongSign mSign;
        private MahjongTween mTweener;

        public MahjongSign Sign
        {
            get
            {
                if (mSign == null) mSign = GetComponent<MahjongSign>();
                return mSign;
            }
        }
        public MahjongTween Tweener
        {
            get
            {
                if (mTweener == null) mTweener = GetComponent<MahjongTween>();
                return mTweener;
            }
        }

        public BoxCollider BoxCollider { get; protected set; }
        public MahjongContorl Contorl { get; protected set; }

        public override string ToString()
        {
            return "值:" + Value + "；编号：" + MahjongIndex + " 排序号：" + TableSortIndex;
        }

        public void OnReset()
        {
            MahjongIndex = -1;
            TableSortIndex = -1;
            SetAllowOffsetStatus(true);
            ChangeToHardLayer(false);
            RemoveMahjongScript();
            ShowNormal();
            mIsAcross = false;
            Lock = false;

            mLaizi = false;
            mTingCard = false;
            mOther = false;
            mNumber = 1;
            Sign.OnReset();

            if (Tweener != null) Tweener.StopAllTween();
        }

        public void RollUp()
        {
            if (IsTingCard)
            {
                GameCenter.Network.OnRequestC2S((sfs) =>
                {
                    sfs.PutInt(RequestKey.KeyType, NetworkProls.GetHuCards);
                    sfs.PutInt("card", Value);
                    if (GameCenter.DataCenter.Config.QueryTingInRate)
                    {
                        sfs.PutBool("rate", true);
                    }
                    return sfs;
                });
            }
            Tweener.Up(0.02f);
            GameCenter.Scene.MahjongGroups.OnFlagMahjong(Value);
        }

        public void RollDown()
        {
            if (Contorl != null) Contorl.RollDown();
        }

        public void ResetPos()
        {
            if (Contorl != null) Contorl.RollDown();
        }

        public int Value
        {
            get { return MahjongCard.Value; }
            set
            {
                MahjongCard.Value = value;
                Laizi = GameCenter.DataCenter.IsLaizi(value);
            }
        }

        public virtual void SetMahjongScript()
        {
            if (BoxCollider == null)
            {
                BoxCollider = gameObject.GetComponent<BoxCollider>();
                if (BoxCollider == null)
                {
                    BoxCollider = gameObject.AddComponent<BoxCollider>();
                }
                BoxCollider.size = DefaultUtils.MahjongSize;
            }
            if (Contorl == null)
            {
                Contorl = gameObject.GetComponent<MahjongContorl>();
                if (Contorl == null)
                {
                    Contorl = gameObject.AddComponent<MahjongContorl>();
                }
            }
        }

        public void RemoveMahjongScript()
        {
            if (Contorl != null)
            {
                DestroyImmediate(Contorl);
                Contorl = null;
            }
            if (BoxCollider != null)
            {
                DestroyImmediate(BoxCollider);
                BoxCollider = null;
            }
        }

        /// <summary>
        /// 赖子
        /// </summary>
        protected bool mLaizi;
        public bool Laizi
        {
            get { return mLaizi; }
            set
            {
                mLaizi = value;
                if (value)
                {
                    Sign.LaiziSign(value);
                }
            }
        }

        /// <summary>
        /// 听牌标记
        /// </summary>
        protected bool mTingCard;
        public bool IsTingCard
        {
            get { return mTingCard; }
            set
            {
                mTingCard = value;
                if (value)
                {
                    Sign.TingSign(value);
                }
                else
                {
                    var item = Sign.GetSign(MahSignType.Ting);
                    if (item != null)
                    {
                        item.SetState(false);
                    }
                }
            }
        }

        /// <summary>
        /// 麻将记牌标记
        /// </summary>
        protected int mNumber = 1;
        public int Number
        {
            get { return mNumber; }
            set
            {
                mNumber = value;
                Sign.SetNumberSign(mNumber);
            }
        }

        /// <summary>
        /// 其他标记
        /// </summary>
        protected bool mOther;
        public void SetOtherSign(Anchor anchor, bool state)
        {
            mOther = state;
            Sign.OtherSign(anchor, state);
        }

        public bool GetOther()
        {
            return mOther;
        }

        public void ShowGray()
        {
            SetMahjongColor(MahjongColor.Gray);
        }

        public void ShowNormal()
        {
            SetMahjongColor(MahjongColor.Normal);
        }

        public void ShowBlue()
        {
            SetMahjongColor(MahjongColor.Blue);
        }

        /// <summary>
        /// 设置麻将颜色
        /// </summary>
        public void SetMahjongColor(MahjongColor skin)
        {
            MahjongCard.SetMahjongColor(skin);
        }

        public bool IsAcross
        {
            get { return mIsAcross; }
            set
            {
                mIsAcross = value;
                if (value)
                {
                    transform.localRotation = mRotaToAcross;
                }
            }
        }

        public void SetMjRota(float x, float y, float z)
        {
            transform.localRotation = Quaternion.Euler(x, y, z);
        }

        protected bool _lock;
        public bool Lock
        {
            get { return _lock; }
            set
            {
                _lock = value;
                if (_lock)
                {
                    ShowGray();
                }
                else
                {
                    ShowNormal();
                }
            }
        }

        public void SetSelectFlag(bool status)
        {
            if (Contorl != null)
            {
                Contorl.mNoSelectFlag = status;
            }
        }

        public void ChangeToHardLayer(bool isHard)
        {
            if (isHard && GameCenter.Instance.GameType == GameType.Normal)
            {
                GameUtils.ChangeLayer(transform, 9);
            }
            else
            {
                GameUtils.ChangeLayer(transform, 0);
            }
        }

        public void SetAllowOffsetStatus(bool status)
        {
            if (Contorl != null)
            {
                Contorl.AllowOffsetStatus = status;
            }
        }

        public void SetThowOutCall(Action<Transform> throwOutCall)
        {
            if (Contorl != null)
            {
                Contorl.OnThrowOut = throwOutCall;
            }
        }
    }
}