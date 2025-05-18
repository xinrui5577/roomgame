using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Common.Utils;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;

/*===================================================
 *文件名称:     HandUpWindow.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-12-24
 *描述:        	解散投票
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo.View
{
    public class HandUpWindow:PludoFreshWindow
    {
        #region UI Param
        [Tooltip("显示Cd时间")]
        public UILabel Noctie;
        [Tooltip("显示Cd时间")]
        public UILabel CdTime;
        [Tooltip("投票预设")]
        public HandUpItemView HandPrefab;
        [Tooltip("投票父级")]
        public Transform HandContainer;
        #endregion
        #region Data Param
        [Tooltip("操作按钮显示事件")]
        public List<EventDelegate> OperationBtnShwoAction=new List<EventDelegate>();


        [Tooltip("间隔时间")]
        public float DeltaTime = 1f;

        [Tooltip("解散关闭时间")]
        public float ColseTime = 5f;

        [Tooltip("解散关闭时间")]
        public string  SenderFormat = "玩家[{0}]发起解散，等待其它玩家操作!";
        [Tooltip("解散关闭时间")]
        public string DisagreeFormat = "玩家[{0}]拒绝解散，即将关闭面板!";
        public bool BtnShow { private set; get; }

        public bool CloseShow { private set; get; }

        #endregion

        #region Local Data


        private HandUpData _curData;
        /// <summary>
        /// 视图字典
        /// </summary>
        private Dictionary<string, HandUpItemView> _viewDic=new Dictionary<string, HandUpItemView>();
        #endregion

        #region Life Cycle

        protected override void OnAwake()
        {
            base.OnAwake();
            Facade.EventCenter.AddEventListeners<LoaclRequest, HandUpItemData>(LoaclRequest.HandUpLocalMessage, OnHandItemInfo);
        }

        public override void OnDestroy()
        {
            Reset();
            Facade.EventCenter.RemoveEventListener<LoaclRequest, HandUpItemData>(LoaclRequest.HandUpLocalMessage, OnHandItemInfo);
            base.OnDestroy();
        }

        protected override void OnFreshViewWithData()
        {
            base.OnFreshViewWithData();
            _curData = Data as HandUpData;
            if (_curData != null)
            {
                Reset();
                var dic = _curData.HandUpDic;
                var index = 0;
                foreach (var item in dic)
                {
                    var view =HandContainer.GetChildView(index, HandPrefab).GetComponent<HandUpItemView>();
                    view.UpdateView(item.Value);
                    _viewDic.Add(item.Key,view);
                    index++;
                }
                ShowCd(_curData.HaveTime, delegate
                {
                    CdTime.TrySetComponentValue(ConstantData.IntValue.ToString());
                });
                Noctie.TrySetComponentValue(string.Format(SenderFormat, _curData.SenderInfo.NickM));
                FreshHandInfo();
            }
        }

        private void FreshHandInfo(bool oneDisAgree=false)
        {
            if (_curData!=null)
            {
                BtnShow = _curData.HandUpDic[_curData.CurUserId].Status == HandUpStatus.Wait;
                if (oneDisAgree)
                {
                    BtnShow = false;
                }
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(OperationBtnShwoAction.WaitExcuteCalls());
                }
            }
        }

        private Coroutine _cdCor;
        private void ShowCd(float time,AsyncCallback finish = null)
        {
            if (_cdCor!=null)
            {
                StopCoroutine(_cdCor);
            }
            _cdCor=StartCoroutine(ShowCdIenum(time,finish));
        }

        IEnumerator ShowCdIenum(float time, AsyncCallback finish=null)
        {
            while (time>0)
            {
                CdTime.TrySetComponentValue(time.ToString(CultureInfo.InvariantCulture));
                yield return new WaitForSeconds(DeltaTime);
                time -= DeltaTime;
            }
            if (finish!=null)
            {
                finish(null);
            }
        }

        #endregion

        #region Function
        private void OnHandItemInfo(HandUpItemData itemData)
        {
            if (_curData!=null)
            {
                var changeView = _viewDic[itemData.UserId];
                if (changeView)
                {
                    changeView.OnStateChange(itemData.Status);
                    if (itemData.Status== HandUpStatus.DisAgree)
                    {
                        Noctie.TrySetComponentValue(string.Format(DisagreeFormat, itemData.Info.NickM));
                        ShowCd(ColseTime, delegate
                        {
                            CdTime.TrySetComponentValue(ConstantData.IntValue.ToString());
                            Close();
                        });
                    }
                }
                FreshHandInfo();
            }
        }

        /// <summary>
        /// 点击同意
        /// </summary>
        public void OnClickAgree()
        {
            int id;
            int.TryParse(_curData.CurUserId,out id);
            HandUp(HandUpStatus.Agree,id);
        }
        /// <summary>
        /// 点击不同意
        /// </summary>
        public void OnClickDisAgree()
        {
            int id;
            int.TryParse(_curData.CurUserId, out id);
            HandUp(HandUpStatus.DisAgree,id);
        }

        /// <summary>
        /// 房主解散请求（直接解散无需投票）
        /// </summary>
        public static void OwnerHandup()
        {
            Facade.EventCenter.DispatchEvent(LoaclRequest.HandUpByOwnerRequset,ConstantData.IntDefValue);
        }

        /// <summary>
        /// 房主解散（使用条件：牌局为正式开始之前，仅房主使用）
        /// </summary>
        public static void StartHandUp(int userId)
        {
            HandUp(HandUpStatus.Start,userId);
        }

        /// <summary>
        /// 正常局内解散（游戏开始后通用解散）
        /// </summary>
        /// <param name="state">解散状态</param>
        /// <param name="userId">玩家ID</param>
        public static void HandUp(HandUpStatus state, int userId)
        {
            ISFSObject data = new SFSObject();
            data.PutUtfString(ConstantData.KeyCommond, ConstantData.KeyCommondHandsUp);
            data.PutInt(RequestKey.KeyType, (int)state);
            data.PutInt(RequestKey.KeyId, userId);
            Facade.EventCenter.DispatchEvent(LoaclRequest.HandUpRequest, data);
        }

        private void Reset()
        {
            if (_cdCor != null)
            {
                StopCoroutine(_cdCor);
            }
            CloseShow = false;
            BtnShow = false;
            _viewDic.Clear();
        }

        #endregion
    }

    /// <summary>
    /// 投票数据
    /// </summary>
    public class HandUpData
    {
        public long HaveTime { private set; get; }

        public string SenderId { private set; get; }

        public string CurUserId { private set; get; }

        public PludoPlayerInfo CurInfo
        {
            get
            {
                var handItem = HandUpDic[CurUserId];
                if (handItem != null)
                {
                    return handItem.Info;
                }
                else
                {
                    Debug.LogError("未找到当前投票玩家！");
                    return new PludoPlayerInfo();
                }
            }
        }

        public PludoPlayerInfo SenderInfo
        {
            get
            {
                var handItem = HandUpDic[SenderId];
                if (handItem != null)
                {
                    return handItem.Info;
                }
                else
                {
                    Debug.LogError("未找到当前投票玩家！");
                    return new PludoPlayerInfo();
                }
            }
        }

        public int SenderSeat
        {
            get { return HandUpDic[SenderId].Info.Seat; }
        }

        public int CurUserSeat
        {
            get { return HandUpDic[CurUserId].Info.Seat; }
        }
        /// <summary>
        /// 投票字典，主键是玩家ID
        /// </summary>
        public Dictionary<string, HandUpItemData> HandUpDic=new Dictionary<string, HandUpItemData>();

        public void SetPathData(long haveTime, string curUserId, string senderUserId)
        {
            HaveTime = haveTime;
            CurUserId = curUserId;
            SenderId = senderUserId;
        }

        public bool AllUserAgree()
        {
            bool agreeState = true;
            foreach (var item in HandUpDic)
            {
                if (item.Value.Status==HandUpStatus.DisAgree||item.Value.Status == HandUpStatus.Wait)
                {
                    agreeState = false;
                    break;
                }
            }
            return agreeState;
        }
    }

    public class HandUpItemData
    {
        /// <summary>
        /// 玩家ID
        /// </summary>
        public string UserId
        {
            get
            {
                if (Info!=null)
                {
                    return Info.UserId;
                }
                return ConstantData.IntValue.ToString();
            }
        }
        /// <summary>
        /// 玩家状态
        /// </summary>
        private int _handStatus;

        /// <summary>
        /// 玩家基本信息
        /// </summary>
        public PludoPlayerInfo Info;


        public HandUpStatus Status
        {
            get { return (HandUpStatus)_handStatus; }
        }


        public HandUpItemData(PludoPlayerInfo info)
        {
            Info = info;
        }

        /// <summary>
        /// 设置投票状态
        /// </summary>
        /// <param name="status"></param>
        public void SetHandState(int status)
        {
            _handStatus = status;
        }

    }
}
