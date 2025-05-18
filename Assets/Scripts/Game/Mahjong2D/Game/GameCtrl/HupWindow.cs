using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Mahjong2D.Game.GameCtrl
{
    public class HupWindow : MonoSingleton<HupWindow>
    {
        /// <summary>
        /// 内容显示
        /// </summary>
        public UILabel Content;
        /// <summary>mahjong
        /// 投票显示标签
        /// </summary>
        public UILabel TitleLabel;
        /// <summary>
        /// 倒计时显示
        /// </summary>
        public UILabel CountDown;
        /// <summary>
        /// 卷轴动画
        /// </summary>
        public TweenWidth ScrollAnim;
        /// <summary>
        /// 右侧按键
        /// </summary>
        public GameObject RightBtn;
        /// <summary>
        /// 左侧按键
        /// </summary>
        public GameObject LeftBtn;
        /// <summary>
        /// 中间按键
        /// </summary>
        public GameObject MiddleBtn;
        /// <summary>
        /// 显示的控制开关
        /// </summary>
        [SerializeField]
        private GameObject _showParent;
        [Tooltip("倒计时文本格式")]
        public string CountDownTimeFormat= "倒计时{0}秒";
        //----------------------------最大程度兼容-----------------------------
        [Tooltip("拒绝Title文本格式")]
        public string FormatDeffuseTitle = "[581e1e]{0}拒绝解散游戏,投票结束![-]";

        [Tooltip("当前玩家申请解散Title文本格式")]
        public string FormatSelfApplyTitle = "[581e1e]您正在申请解散游戏，请等待其他玩家操作！[-]";

        [Tooltip("其它玩家申请解散Title文本格式")]
        public string FormatotherApplyTitle = "[581e1e]玩家{0}申请解散游戏,您同意解散游戏么?[-]";

        [Tooltip("玩家投票内容格式")]
        public string FormatPlayerContent = "[581e1e]玩家{0}{1}{2}[-]";

        [Tooltip("玩家同意格式")]
        public string FormatPlayerSureContent = "[159026]同意[-]";

        [Tooltip("玩家拒绝格式")]
        public string FormatPlayerDeffuseContent = "[c22525]拒绝[-]";

        [Tooltip("玩家正在选择格式")]
        public string FormatPlayerChooseContent = "正在选择";

        private string _lineCellFlag = "\n";

        public void OnCLickLeftBtn()
        {
            App.GetRServer<Mahjong2DGameServer>().StartHandsUp(3);
        }

        public void OnClickRightBtn()
        {
            App.GetRServer<Mahjong2DGameServer>().StartHandsUp(-1);
        }

        public override void Awake()
        {
            base.Awake();
            Content.overflowMethod = UILabel.Overflow.ResizeFreely;
        }

        /// <summary>
        /// 当前玩家ID
        /// </summary>
        private int localID;

        public void OnClickMiddleBtn()
        {
            Close();
        }
        private List<HupData> hups;
        /// <summary>
        /// 投票结束标识
        /// </summary>
        bool hupEnd;

        /// <summary>
        /// 投票倒计时协程
        /// </summary>
        private Coroutine _handUpCoroutine;

        /// <summary>
        /// 显示投票结果内容
        /// </summary>
        public void ShowHandUp(HupData data, int countDown)
        {
            App.GetRServer<Mahjong2DGameServer>().IsInHandsUp = true;
            switch (data.Operation)
            {
                case -1:
                    hupEnd = true;
                    break;
                case 2:
                    _showParent.SetActive(true);
                    localID = App.GetGameManager<Mahjong2DGameManager>().SelfPlayer.UserInfo.id;
                    if(!App.GameKey.Equals(EnumGameKeys.shmj.ToString()))
                    {
                        if (ScrollAnim!=null)
                        {
                            ScrollAnim.PlayForward();
                        }
                    }
                    hups = new List<HupData>();
                    MiddleBtn.SetActive(false);
                    CurTime = countDown;
                    if (_handUpCoroutine != null)
                    {
                        StopCoroutine(_handUpCoroutine);
                    }
                    CountDown.TrySetComponentValue(CurTime.ToString());
                    _handUpCoroutine =StartCoroutine(CuntDownTime());
                    _isStopCountDown = false;
                    hupEnd = false;
                    break;
                case 3:
                    break;
            }
            hups.Add(data);
            DealHups();
        }

        private void DealHups()
        {     
            TitleLabel.TrySetComponentValue(string.Empty);
            Content.TrySetComponentValue(string.Empty);
            if (hups[0].ID.Equals(localID))
            {
                TitleLabel.TrySetComponentValue(FormatSelfApplyTitle);
            }
            else
            {
                TitleLabel.TrySetComponentValue(string.Format(FormatotherApplyTitle, hups[0].Name));
            }
            bool haveHup = false;
            var content = string.Empty;
            for (int i = 0, max = App.GetGameManager<Mahjong2DGameManager>().Players.Length; i < max; i++)
            {
                UserInfo info = App.GetGameManager<Mahjong2DGameManager>().Players[i].UserInfo;
                if (info != null)
                {
                    int hupIndex = hups.FindIndex(item => item.ID.Equals(info.id));
                    if (hupIndex==0)
                    {
                        continue;
                    }
                    var playerName = info.name;
                    if (playerName.Length>7)
                    {
                        playerName = playerName.Substring(0, 4).PadRight(7,'.');
                    }
                    playerName = playerName.PadRight(7, ' ');
                    var singelContent= string.Format(FormatPlayerContent, playerName, hupIndex > 0 ? hups[hupIndex].Operation == -1 ? FormatPlayerDeffuseContent : FormatPlayerSureContent : FormatPlayerChooseContent, _lineCellFlag);
                    content += singelContent;
                }
            }
            var index = content.LastIndexOf(_lineCellFlag, StringComparison.Ordinal);
            if (index>-1)
            {
                content = content.Substring(0, index);
            }
            Content.TrySetComponentValue(content);
            haveHup = hups.Exists(item => item.ID.Equals(localID));
            RightBtn.SetActive(!haveHup);
            LeftBtn.SetActive(!haveHup);

            //End
            if (hupEnd)
            {
                HandupEnd();
            }
        }

        /// <summary>
        /// 投票结束
        /// </summary>
        public void HandupEnd()
        {
            RightBtn.SetActive(false);
            LeftBtn.SetActive(false);
            MiddleBtn.SetActive(true);
            _isStopCountDown = true;
            if (_handUpCoroutine!=null)
            {
                StopCoroutine(_handUpCoroutine);
            }
            HupData lastData = hups.Last();
            TitleLabel.TrySetComponentValue(string.Format(FormatDeffuseTitle,lastData.ID == localID ? "您" : string.Format("玩家{0}", lastData.Name)));
            CountDown.TrySetComponentValue("0");
            hups.Clear();
            App.GetRServer<Mahjong2DGameServer>().IsInHandsUp = false;
        }

        private EventDelegate _del;
        /// <summary>
        /// 关闭卷轴并删除对象
        /// </summary>
        public void Close()
        {
            if (ScrollAnim != null)
            {
                _del = new EventDelegate(DirectClose);
                ScrollAnim.AddOnFinished(_del);
                ScrollAnim.PlayReverse();
            }
            else
            {
                DirectClose();
            }
         }

        /// <summary>
        /// 关闭响应
        /// </summary>
        public Action OnWindowClose;
        /// <summary>
        /// 直接关闭
        /// </summary>
        public void DirectClose()
        {
            if (ScrollAnim!=null)
            {
                ScrollAnim.RemoveOnFinished(_del);
            }
            if (OnWindowClose!=null)
            {
                OnWindowClose();
            }
            _showParent.SetActive(false);
        }
        [HideInInspector]
        /// <summary>
        /// 倒计时当前时间
        /// </summary>
        public int CurTime;
        /// <summary>
        /// 是否暂停倒计时
        /// </summary>
        private bool _isStopCountDown;
        /// <summary>
        /// 倒计时协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator CuntDownTime()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                if (_isStopCountDown)
                {
                    CountDown.TrySetComponentValue("0");
                    _isStopCountDown = false;
                    yield break;
                }
                CurTime--;
                if (CurTime < 0)
                {
                    CurTime = 0;
                    CountDown.text = string.Format(CountDownTimeFormat,CurTime.ToString(CultureInfo.InvariantCulture));
                    RightBtn.SetActive(false);
                    LeftBtn.SetActive(false);
                    MiddleBtn.SetActive(true);
                    yield break;
                }
                CountDown.text = string.Format(CountDownTimeFormat,CurTime.ToString(CultureInfo.InvariantCulture));
            }
        }

    }
}
