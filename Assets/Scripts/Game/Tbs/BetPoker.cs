using System.Globalization;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Tbs
{
    /// <summary>
    /// 下注区域的扑克管理
    /// </summary>
    public class BetPoker : MonoBehaviour
    {

        /// <summary>
        /// 左侧的扑克位置
        /// </summary>
        public GameObject LeftPokerPos;
        /// <summary>
        /// 右侧的扑克位置
        /// </summary>
        public GameObject RightPokerPos;
        /// <summary>
        /// 第一张扑克
        /// </summary>
        [HideInInspector]
        public GameObject LeftPoker;
        /// <summary>
        /// 第二张扑克
        /// </summary>
        [HideInInspector]
        public GameObject RightPoker;
        /// <summary>
        /// 左牌值
        /// </summary>
        [HideInInspector]
        public int LeftCardV;
        /// <summary>
        /// 右牌值
        /// </summary>
        [HideInInspector]
        public int RightCardV;
        /// <summary>
        /// 锚点位置
        /// </summary>
        public GameObject PoivtPos;
        /// <summary>
        /// 
        /// </summary>
        public GameObject HandFather;
        /// <summary>
        /// 
        /// </summary>
        public UISprite RightHand;
        /// <summary>
        /// 开牌索引
        /// </summary>
        private int _handIndex;
        /// <summary>
        /// 自己是否处于翻牌阶段
        /// </summary>
        private bool _isToc = true;


        /// <summary>
        /// 设置牌的值
        /// </summary>
        /// <param name="leftCardV"></param>
        /// <param name="rightCardV"></param>
        public void SetCardV(int leftCardV, int rightCardV)
        {
            LeftCardV = leftCardV;
            RightCardV = rightCardV;
        }

       protected void Start()
        {
            _changeFEvent = new EventDelegate(Onfinish);
            _changeOEvent = new EventDelegate(Onfinish1);
        }


        /*************翻转牌**************/
        private EventDelegate _changeFEvent;
        private EventDelegate _changeOEvent;

        private void Onfinish()
        {
            var rightTweenR = RightPoker.GetComponent<TweenRotation>();

            App.GetGameManager<TbsGameManager>().PokerMgr.ShowPokerV(RightPoker, "0x" + RightCardV.ToString("X"));

            rightTweenR.RemoveOnFinished(_changeFEvent);

            rightTweenR.from = new Vector3(0, -90, 0);
            rightTweenR.to = new Vector3(0, 0, 0);
            rightTweenR.AddOnFinished(_changeOEvent);
            rightTweenR.ResetToBeginning();
            rightTweenR.PlayForward();
        }
        private void Onfinish1()
        {
            var rightTweenR = RightPoker.GetComponent<TweenRotation>();
            rightTweenR.RemoveOnFinished(_changeOEvent);
        }

        /// <summary>
        /// 翻牌
        /// </summary>
        public void TurnOverCard()
        {
            if (LeftPoker == null || RightPoker == null)
            {
                return;
            }

            if (!_isToc)
            {
                //显示点数
                PlayDealDian();
                return;
            }

            //if (handIndex > 0)
            //{
            _handIndex = 0;
            if (HandFather != null)
            {
                HandFather.SetActive(false);
            }

            var gmanager = App.GetGameManager<TbsGameManager>();
            gmanager.PokerMgr.CloseAllTween(LeftPoker);
            gmanager.PokerMgr.CloseAllTween(RightPoker);

            gmanager.PokerMgr.ShowPokerV(LeftPoker, "0x" + LeftCardV.ToString("X"));
            gmanager.PokerMgr.ShowPokerV(RightPoker, "0x" + 0);

            RightPoker.transform.parent = transform;

            LeftPoker.transform.localPosition = LeftPokerPos.transform.localPosition;
            RightPoker.transform.localPosition = RightPokerPos.transform.localPosition;

            RightPoker.transform.localRotation = Quaternion.Euler(Vector3.zero);
            PoivtPos.transform.localRotation = Quaternion.Euler(Vector3.zero);

            var rightTweenR = RightPoker.GetComponent<TweenRotation>();

            rightTweenR.from = new Vector3(0, 0, 0);
            rightTweenR.to = new Vector3(0, -90, 0);
            rightTweenR.AddOnFinished(_changeFEvent);
            rightTweenR.ResetToBeginning();
            rightTweenR.PlayForward();

            PlayDealDian();

            _isToc = false;
        }

        /// <summary>
        /// 自己翻牌
        /// </summary>
        public void ContinueTurnOverCard()
        {

            //float baseOffset = 15f;
            var gmanager = App.GetGameManager<TbsGameManager>();
            if (_handIndex <= 0)
            {
               
                RightPoker.transform.localRotation = Quaternion.Euler(Vector3.zero);
                gmanager.PokerMgr.CloseAllTween(LeftPoker);
                gmanager.PokerMgr.CloseAllTween(RightPoker);

                RightPoker.transform.localPosition = new Vector3(0f, RightPokerPos.transform.localPosition.y);
                LeftPoker.transform.localPosition = new Vector3(0f, LeftPokerPos.transform.localPosition.y);

                gmanager.PokerMgr.ShowPokerV(LeftPoker, "0x" + RightCardV.ToString("X"));
                gmanager.PokerMgr.ShowPokerV(RightPoker, "0x" + LeftCardV.ToString("X"));

                HandFather.SetActive(true);
                RightHand.spriteName = "righthand_" + _handIndex;
                RightHand.MakePixelPerfect();

                PoivtPos.transform.localPosition = new Vector3((LeftPoker.transform.FindChild("Sprite").GetComponent<UISprite>().width >> 1) * RightPoker.transform.localScale.x,
                    (-(LeftPoker.transform.FindChild("Sprite").GetComponent<UISprite>().height >> 1) * RightPoker.transform.localScale.y) + RightPokerPos.transform.localPosition.y);

                RightPoker.transform.parent = PoivtPos.transform;

                PoivtPos.transform.localRotation = Quaternion.Euler(0f,0f,16f);
                _handIndex++;

                return;
            }
            if (_handIndex < 3)
            {
                RightHand.spriteName = "righthand_" + _handIndex;
                RightHand.MakePixelPerfect();
                RightPoker.transform.parent = PoivtPos.transform;

                PoivtPos.GetComponent<TweenRotation>().@from = PoivtPos.transform.localEulerAngles;
                PoivtPos.GetComponent<TweenRotation>().to = new Vector3(0f, 0f, 16f + _handIndex * 7f);
            }
            else
            {
                _handIndex = 0;
                HandFather.SetActive(false);

                gmanager.PokerMgr.ShowPokerV(LeftPoker, "0x" + RightCardV.ToString("X"));
                gmanager.PokerMgr.ShowPokerV(RightPoker, "0x" + LeftCardV.ToString("X"));

                RightPoker.transform.parent = transform;

                LeftPoker.transform.localPosition = LeftPokerPos.transform.localPosition;
                RightPoker.transform.localPosition = RightPokerPos.transform.localPosition;

                RightPoker.transform.localRotation = Quaternion.Euler(Vector3.zero);

                _isToc = false;
                PoivtPos.transform.localRotation = Quaternion.Euler(Vector3.zero);
                PoivtPos.GetComponent<TweenRotation>().@from = Vector3.zero;
                PoivtPos.GetComponent<TweenRotation>().to = Vector3.zero;
                return;
            }

            PoivtPos.GetComponent<TweenRotation>().ResetToBeginning();
            PoivtPos.GetComponent<TweenRotation>().PlayForward();

            _handIndex++;

        }

        /// <summary>
        /// 普通翻牌点数显示动画
        /// </summary>
        public TweenScale CommonDeal;
        /// <summary>
        /// 豹子翻牌点数显示动画
        /// </summary>
        public TweenPosition BaoziDeal;
        /// <summary>
        /// 点数显示
        /// </summary>
        public UISprite Dian;
        /// <summary>
        /// 闭十显示
        /// </summary>
        public UISprite Bishi;
        /// <summary>
        /// 豹子显示
        /// </summary>
        public UISprite Baozi;
        /// <summary>
        /// 当前牌值类型
        /// </summary>
        public CardVType CurVType;
        /// <summary>
        /// 当前牌的值
        /// </summary>
        public int CurCardV;
        /// <summary>
        /// 播放点数动画
        /// </summary>
        public void PlayDealDian()
        {
            CardVType cvType;
            int cardv;
            CardColorType cct;
            GetCardType(out cvType, out cardv, out cct);

            Dian.gameObject.SetActive(false);
            Bishi.gameObject.SetActive(false);
            Baozi.gameObject.SetActive(false);

            CurVType = cvType;
            CurCardV = cardv;

            switch (cvType)
            {
                case CardVType.Dian:
                    Dian.gameObject.SetActive(true);
                    Dian.spriteName = cardv.ToString(CultureInfo.InvariantCulture);
                    Dian.MakePixelPerfect();
                    CommonDeal.ResetToBeginning();
                    CommonDeal.PlayForward();
                    Facade.Instance<MusicManager>().Play(cct.ToString() + "s" + cardv.ToString("x"));
                    break;
                case CardVType.Bishi:
                    Bishi.gameObject.SetActive(true);
                    CommonDeal.ResetToBeginning();
                    CommonDeal.PlayForward();
                    Facade.Instance<MusicManager>().Play(CardVType.Bishi.ToString().ToLower());
                    break;
                case CardVType.Baozi:
                    Baozi.gameObject.SetActive(true);
                    Baozi.spriteName = cardv.ToString("x");
                    Baozi.MakePixelPerfect();
                    BaoziDeal.ResetToBeginning();
                    BaoziDeal.PlayForward();
                    Facade.Instance<MusicManager>().Play(cct.ToString() + "p" + cardv.ToString("x"));
                    break;
                default:
                    YxDebug.LogError("不存在的牌型!");
                    break;
            }
        }
        /// <summary>
        /// 关闭对象
        /// </summary>
        public void CloseGameObject(GameObject gob)
        {
            gob.SetActive(false);
        }
        /// <summary>
        /// 关闭显示点数
        /// </summary>
        public void CloseDealDian()
        {
            Dian.gameObject.SetActive(false);
            Bishi.gameObject.SetActive(false);
            Baozi.gameObject.SetActive(false);
        }
        /// <summary>
        /// 牌型颜色
        /// </summary>
        private readonly int[] _color = new[] { 0, 1, 1, 2, 2 };

        /// <summary>
        /// 获取牌的类型和值
        /// </summary>
        public void GetCardType(out CardVType ct, out int cv, out CardColorType cc)
        {
            int x = LeftCardV & 0xf;
            int y = RightCardV & 0xf;

            int xc = int.Parse(LeftCardV.ToString("X")[0].ToString(CultureInfo.InvariantCulture));
            int yc = int.Parse(RightCardV.ToString("X")[0].ToString(CultureInfo.InvariantCulture));

            cc = _color[xc] == _color[yc] ? (CardColorType)_color[yc] : CardColorType.Z;

            if (x == y)
            {
                ct = CardVType.Baozi;
                cv = x;
                return;
            }

            x = GetCardV(x);
            y = GetCardV(y);

            int v = (x + y) % 10;

            if (v == 0)
            {
                ct = CardVType.Bishi;
                cv = v;
            }
            else
            {
                ct = CardVType.Dian;
                cv = v;
            }
        }

        /// <summary>
        /// 获取单牌的值
        /// </summary>
        /// <returns></returns>
        private int GetCardV(int card)
        {
            if (card == 14)
                card = 1;

            if (card >= 10)
                card %= 10;

            return card;
        }

        /// <summary>
        /// 点击下注区
        /// </summary>
        void OnClick()
        {
            if (IsOpenPoker())
            {
                Facade.Instance<MusicManager>().Play("change");
                //翻自己的牌
                ContinueTurnOverCard();
            }
        }
        /// <summary>
        /// 点击翻牌
        /// </summary>
        public void ClickOpen()
        {
            if (!App.GetGameManager<TbsGameManager>().BankerBtnMgr.IsAuto)
            {
                return;
            }

            if (IsOpenPoker())
            {
                Facade.Instance<MusicManager>().Play("change");
                //翻自己的牌
                ContinueTurnOverCard();
            }
        }
        /// <summary>
        /// 是否走过发牌回调
        /// </summary>
        private bool _isDealFinish;

        /// <summary>
        /// 当发牌完成
        /// </summary>
        public void OnDealFinish()
        {
            var gmanager = App.GetGameManager<TbsGameManager>();
            if (!App.GetGameData<TbsGameData>().IsDeal)//没发牌
                return;

            if (LeftPoker.GetComponent<TweenTransform>().enabled || RightPoker.GetComponent<TweenTransform>().enabled)//牌没发完
            {
                return;
            }

            if (_isDealFinish || !_isToc)
            {
                return;
            }

            _isDealFinish = true;

            RightPoker.transform.localPosition = new Vector3(0f, RightPoker.transform.localPosition.y);
            LeftPoker.transform.localPosition = new Vector3(0f, LeftPoker.transform.localPosition.y);

            gmanager.PokerMgr.ShowPokerV(LeftPoker, "0x" + RightCardV.ToString("X"));
            gmanager.PokerMgr.ShowPokerV(RightPoker, "0x" + LeftCardV.ToString("X"));

            const float xOffset = 18f;

            RightPoker.GetComponent<TweenPosition>().from = RightPoker.transform.localPosition;
            RightPoker.GetComponent<TweenPosition>().to = RightPoker.transform.localPosition + new Vector3(-xOffset * RightPoker.transform.localScale.x, -xOffset/2.5f);

            RightPoker.GetComponent<TweenPosition>().ResetToBeginning();
            RightPoker.GetComponent<TweenPosition>().PlayForward();

            RightPoker.GetComponent<TweenRotation>().from = Vector3.zero;
            RightPoker.GetComponent<TweenRotation>().to = new Vector3(0f, 0f, xOffset * 0.9f);

            RightPoker.GetComponent<TweenRotation>().ResetToBeginning();
            RightPoker.GetComponent<TweenRotation>().PlayForward();

        }

        /// <summary>
        /// 是否可以翻牌
        /// </summary>
        /// <returns></returns>
        public bool IsOpenPoker()
        {
            if (!App.GetGameData<TbsGameData>().IsDeal)//没发牌
                return false;

            if (LeftPoker.GetComponent<TweenTransform>().enabled || RightPoker.GetComponent<TweenTransform>().enabled)//牌没发完
            {
                return false;
            }

            if (!_isToc)//翻过牌了
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// 扑克切换到筹码的下方
        /// </summary>
        public void PokerToBetUnder()
        {
            if (LeftPoker == null || RightPoker == null)
            {
                return;
            }

            App.GetGameManager<TbsGameManager>().PokerMgr.SetPokerDepth(LeftPoker, 25);
            App.GetGameManager<TbsGameManager>().PokerMgr.SetPokerDepth(RightPoker, 26);

        }

        public void Reset()
        {
            _isDealFinish = false;
            _isToc = true;
            CloseDealDian();
        }
    }

    /// <summary>
    /// 牌值类型
    /// </summary>
    public enum CardVType
    {
        Bishi,
        Dian,
        Baozi,
    }
    /// <summary>
    /// 牌颜色类型
    /// </summary>
    public enum CardColorType
    {
        Z,
        R,
        B,
    }
}