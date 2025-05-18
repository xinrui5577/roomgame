/** 
 *文件名称:     OutPile.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-20 
 *描述:         打牌牌堆，继承于麻将堆，只是多了一张打出牌的额外处理
 *历史记录: 
*/

using System;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.Game.Data;
using Assets.Scripts.Game.Mahjong2D.Game.GameCtrl;
using Assets.Scripts.Game.Mahjong2D.Game.Item;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Mahjong2D.Common.MahJongCommon
{
    public class OutPile : MahjongPile
    {
        private ThrowOutCard _throw;
        private MahjongItem outCard;
        private MahjongItem _controlItem;
        /// <summary>
        /// 打出牌的x轴倍数
        /// </summary>
        public float OutCardScaleX=1.5f;
        /// <summary>
        /// 打出牌的y轴倍数
        /// </summary>
        public float OutCardScaleY=1.5f;
        /// <summary>
        /// 是否直接落下
        /// </summary>
        private bool _direct;
        /// <summary>
        /// 飞到显示位置是否播放声音
        /// </summary>
        public bool FlyShowVoice = true;
        /// <summary>
        /// 落到位置是否播放声音
        /// </summary>
        public bool DownShowVoice = false;
        [Tooltip("二人场OutPos")]
        public Vector3 TwoPeoplePos = Vector3.zero;
        [Tooltip("打出牌直接变为放倒状态")]
        public bool DirectLie = false;

        private Vector3 _defVec;

        protected override void Awake()
        {
            base.Awake();
            _defVec = transform.localPosition;
        }

        public ThrowOutCard OutCard
        {
            set { _throw = value; }
            get
            {
                if (_throw == null)
                {
                    _throw = GetComponentInChildren<ThrowOutCard>();
                }
                return _throw;
            }
        }
        /// <summary>
        /// 直接打出一张牌
        /// </summary>
        public void ThrowOutCard()
        {
            Transform lastTrans = Layout.GetLastItem();
            if (lastTrans!=null)
            {
                MahjongItem lastItem = lastTrans.GetComponent<MahjongItem>();
                ThrowOutCard(lastItem, 0);
            }
        }

        private AsyncCallback _showFinishCall;
        public void ThrowOutCard(MahjongItem item,float time, AsyncCallback showFinishCall = null)
        {
            if (showFinishCall!=null)
            {
                _showFinishCall = showFinishCall;
            }
            _throw.ThrowCard = item;
            item.SelfData.Action= DirectLie ? EnumMahJongAction.Lie: EnumMahJongAction.StandWith;
            item.SelfData.Direction = EnumMahJongDirection.Vertical;
            item.SelfData.ShowDirection = EnumShowDirection.Self;
            GameTools.AddChild(_throw.transform, _throw.ThrowCard.transform,1,1,false);
            GameTools.RefreshTrans(item.transform);
            _pos =TweenPosition.Begin(item.gameObject, time, _throw.transform.position);
            _scale=TweenScale.Begin(item.gameObject, time, new Vector3(OutCardScaleX, OutCardScaleY, 0));
            App.GetGameManager<Mahjong2DGameManager>().OnSomeOneNeedCard = OnThrowOutShow;
            Invoke("OnThrowOutShow", time);
        }
        /// <summary>
        /// 飞到地方啊
        /// </summary>
        private void OnThrowOutShow()
        {
            CancelInvoke("OnThrowOutShow");
            if (_throw.ThrowCard!=null)
            {
                _throw.ThrowCard.SelfData.Action = EnumMahJongAction.Lie;
                if (_pos != null)
                {
                    Destroy(_pos);
                }
                if (_scale != null)
                {
                    Destroy(_scale);
                }
                if (FlyShowVoice)
                {
                    Facade.Instance<MusicManager>().Play(ConstantData.VoiceZhuaPai);
                }
                _throw.ThrowCard.transform.localScale = new Vector3(OutCardScaleX, OutCardScaleY, 0);
                _throw.ThrowCard.transform.localPosition = Vector3.zero;
                App.GetGameManager<Mahjong2DGameManager>().OnSomeOneNeedCard=null;
                if (_showFinishCall!=null)
                {
                    _showFinishCall(null);
                    _showFinishCall = null;
                }
            }
        }
        /// <summary>
        /// 飞起动作
        /// </summary>
        private TweenPosition _pos;
        /// <summary>
        /// 飞起缩放
        /// </summary>
        private TweenScale _scale;
        /// <summary>
        /// 下落动作
        /// </summary>
        private TweenPosition _downPos;
        /// <summary>
        /// 下落倍数
        /// </summary>
        private TweenScale _downScale;

        /// <summary>
        /// 等待指定时间后落下，打出的牌落下
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="direct">是否直接落下</param>
        public void ThrowOutCardFinished(float time, bool direct = false)
        {
            if (_throw.ThrowCard != null)
            {
                _direct = direct;
                if (direct)
                {
                    OnStayFinish();
                }
                else
                {
                    StayForAWhile(time);
                }
            }
        }
        /// <summary>
        /// 等待一段时间，牌移动到对应位置后的停滞操作
        /// </summary>
        /// <param name="waitTime"></param>
        private void StayForAWhile(float waitTime)
        {
            Invoke("OnStayFinish", waitTime);
        }

        /// <summary>
        /// 等待完成
        /// </summary>
        private void OnStayFinish()
        {
            if (_direct)
            {
                if(IsInvoking("OnStayFinish"))
                {
                    CancelInvoke("OnStayFinish");
                    MoveCardToPile();
                }
            }
            else
            {
                CancelInvoke("OnStayFinish");
                MoveCardToPile();
            }
               
        }
        /// <summary>
        /// 移动一张牌到牌堆里面
        /// </summary>
        public void MoveCardToPile()
        {
            if (_throw.ThrowCard != null)
            {
                if (_pos != null)
                {
                    Destroy(_pos);
                }
                if (_scale != null)
                {
                    Destroy(_scale);
                }
                MahjongItem item = _throw.ThrowCard;
                _throw.ThrowCard = null;
                AddCardToPiles(item, App.GetGameData<Mahjong2DGameData>().CardDownTime);
            }
        }
        /// <summary>
        /// 添加一张牌到牌堆
        /// </summary>
        /// <param name="item"></param>
        /// <param name="downTime"></param>
        public void AddCardToPiles(MahjongItem item,float downTime)
        {
            if (item != null)
            {
                _controlItem = item;
                if (_direct)
                {
                    GameTools.AddChild(Layout.transform, _controlItem.transform, OutCardScaleX, OutCardScaleY, false, false);
                    OnMoveDownFinished();
                }
                else
                {
                    Vector3 nextPos = GetNextCardPosition();
                    GameTools.AddChild(Layout.transform, _controlItem.transform, OutCardScaleX, OutCardScaleY, false, false);
                    _downPos = TweenPosition.Begin(_controlItem.gameObject, downTime, nextPos);
                    TweenScale.Begin(item.gameObject, downTime, new Vector3(ItemScaleX, ItemScaleY));
                    Invoke("OnMoveDownFinished", downTime);
                }
            }
            else
            {
                Debug.LogError("打出的那张牌竟然空了，额........................");
            }

        }
        /// <summary>
        /// 落到地方
        /// </summary>
        public void OnMoveDownFinished()
        {
            CancelInvoke("OnMoveDownFinished"); 
            if (_downScale!=null)
            {
                Destroy(_downScale);
            }
            if (_downPos!=null)
            {
                Destroy(_downPos);
            }
            if (_throw.ThrowCard!= null)
            {
                _throw.ThrowCard = null;
            }
            if (_controlItem)
            {
                ParseItemToThis(_controlItem);
                if (DownShowVoice && !_direct)
                {
                    Facade.Instance<MusicManager>().Play(ConstantData.VoiceZhuaPai);
                }
                GameTools.RefreshTrans(_controlItem.transform);
                Layout.ResetPositionNow = true;
                _direct = false;
            }  
        }

        public override void ResetPile()
        {
            base.ResetPile();
            if (OutCard.ThrowCard!=null)
            {
                OutCard.ThrowCard = null;
            }
            while (OutCard.transform.childCount > 0)
            {
               DestroyImmediate(OutCard.transform.GetChild(0).gameObject);
            }
            _direct = false;
        }

        public void SetPilePos()
        {
            transform.localPosition = TwoPeoplePos;
        }
    }
}
