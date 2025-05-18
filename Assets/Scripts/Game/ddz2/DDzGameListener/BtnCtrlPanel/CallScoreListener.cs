using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using com.yxixia.utile.YxDebug;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.BtnCtrlPanel
{
    public class CallScoreListener : ServEvtListener
    {
        /// <summary>
        /// 叫分按钮的父层级
        /// </summary>
        [SerializeField]
        protected GameObject CallBtnsParent;

        /// <summary>
        /// 不叫按钮
        /// </summary>
        [SerializeField]
        protected GameObject NoCallBtn;

        /// <summary>
        /// 叫分按钮,从1分到高分
        /// </summary>
        [SerializeField]
        protected GameObject[] CallScoreBtns;

        /// <summary>
        /// 叫地主,抢地主按钮父层级
        /// </summary>
        [SerializeField]
        protected GameObject CallLandParent;

        /// <summary>
        /// 叫地主按钮
        /// </summary>
        [SerializeField]
        protected UIButton CallLandBtn;

        [SerializeField]
        protected GameObject NoCallLandBtn;

        /// <summary>
        /// 叫地主按钮图片名称
        /// </summary>
        [SerializeField]
        protected string CallLandBtnSprName;

        /// <summary>
        /// 抢地主按钮图片名称
        /// </summary>
        [SerializeField]
        protected string RobLandBtnSprName;

        /// <summary>
        /// 不叫分按钮的名字
        /// </summary>
        [SerializeField]
        protected string NoCallBtnBtnName;


        /// <summary>
        /// 是否处于托管状态
        /// </summary>
        private bool _autoState;

        /// <summary>
        /// 是否是抢庄模式
        /// 抢庄类型,0正常叫分,1抢地主带踢,2抢地主都不抢黄庄,3叫分都不叫黄庄
        /// 这里0，3为叫份模式,字段为false ; 1，2为抢庄模式,字段为true
        /// </summary>
        private bool _robModel;



        protected override void OnAwake()
        {
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeGrab, OnTypeGrab);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeGrabSpeaker, OnTypeGrabSpeaker);

            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyOnRejoin, OnGetRejoionData);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyGetGameInfo, OnGetGameInfo);
            Facade.EventCenter.AddEventListeners<string, bool>(GlobalConstKey.KeySelfAuto, OnSelfAuto);

            Facade.EventCenter.AddEventListeners<string, bool>(GlobalConstKey.CheckLuckyResult, luckyCardResult);

            InitBtns();

        }

        private void InitBtns()
        {
            UIEventListener.Get(NoCallBtn).onClick = OnCallScoreClick;
            foreach (var item in CallScoreBtns)
            {
                UIEventListener.Get(item).onClick = OnCallScoreClick;
            }
            UIEventListener.Get(CallLandBtn.gameObject).onClick = OnLandBtnClick;
            UIEventListener.Get(NoCallLandBtn).onClick = OnLandBtnClick;
        }


        /// <summary>
        /// 托管状态设置
        /// </summary>
        /// <param name="state"></param>
        private void OnSelfAuto(bool state)
        {
            _autoState = state;
        }

        /// <summary>
        /// 叫分按钮点击
        /// </summary>
        /// <param name="gob"></param>
        private void OnCallScoreClick(GameObject gob)
        {
            int calScore = 0;
            if (gob.name.Equals(NoCallBtnBtnName))
            {
                calScore = 0;
            }
            else
            {
                int len = CallScoreBtns.Length;
                for (int i = 0; i < len; i++)
                {
                    if (ReferenceEquals(gob, CallScoreBtns[i]))
                    {
                        calScore = i + 1;
                        break;
                    }
                }
            }

            //向服务器发送叫分信息
            App.GetRServer<DdzGameServer>().CallGameScore(App.GameData.SelfSeat, calScore);
        }

        private void OnLandBtnClick(GameObject gob)
        {
            int calScore = 0;
            if (gob.name.Equals(NoCallBtnBtnName))
            {
                calScore = 0;
                //向服务器发送叫分信息
                App.GetRServer<DdzGameServer>().CallGameScore(App.GameData.SelfSeat, calScore);
                return;
            }

            if (ServDataTemp == null) return;

            if (ServDataTemp.ContainsKey(NewRequestKey.KeyScore))
            {
                calScore = ServDataTemp.GetInt(NewRequestKey.KeyScore);
            }

            if (ServDataTemp.ContainsKey(NewRequestKey.KeyMinScore))
            {
                var minscore = ServDataTemp.GetInt(NewRequestKey.KeyMinScore);

                if (minscore > calScore) calScore = minscore;
            }
            //向服务器发送叫分信息
            App.GetRServer<DdzGameServer>().CallGameScore(App.GameData.SelfSeat, calScore + 1);
        }

        /// <summary>
        /// 存储与界面显示相关的服务器消息缓存
        /// </summary>
        protected ISFSObject ServDataTemp;

        /// <summary>
        /// 幸运手牌开关 控制： 1分 2分
        /// </summary>
        public bool LuckyCardsSwitch { get; set; }

        /// <summary>
        /// 幸运手牌开关 控制： 1分 2分 不叫
        /// </summary>
        public bool SuperLuckyCardsSwitch { get; set; }

        /// <summary>
        /// 当进入游戏获取游戏参数
        /// </summary>
        /// <param name="args"></param>
        private void OnGetGameInfo(DdzbaseEventArgs args)
        {
            var sfsData = args.IsfObjData;
            var cargs = sfsData.GetSFSObject("cargs2");
            if (cargs.ContainsKey("-qt"))
            {
                var qt = cargs.GetUtfString("-qt");
                _robModel = qt.Equals("1") || qt.Equals("2");
            }

            if (cargs.ContainsKey("-luckyCards"))
            {
                var str = cargs.GetUtfString("-luckyCards");
                LuckyCardsSwitch = int.Parse(str) > 0;
            }
            if (cargs.ContainsKey("-superLuckyCards"))
            {
                var str = cargs.GetUtfString("-superLuckyCards");
                SuperLuckyCardsSwitch = int.Parse(str) > 0;
            }
        }

        /// <summary>
        /// 重连
        /// </summary>
        /// <param name="args"></param>
        private void OnGetRejoionData(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            if (data.ContainsKey(NewRequestKey.KeyGameStatus)
                && data.GetInt(NewRequestKey.KeyGameStatus) == GlobalConstKey.StatusChoseBanker)
            {
                if ((data.ContainsKey(NewRequestKey.KeyMinScore) || data.ContainsKey(NewRequestKey.KeyScore))
                    && data.ContainsKey(NewRequestKey.KeyCurrp)
                    && data.ContainsKey(NewRequestKey.KeyQt))
                {
                    //当前谁发言
                    int curCallSeat = data.GetInt(NewRequestKey.KeyCurrp);
                    if (App.GetGameData<DdzGameData>().SelfSeat == curCallSeat)
                    {
                        ServDataTemp = data;
                        RefreshUiInfo();
                    }
                }
                return;
            }

            HideAllBtn();
        }

        /// <summary>
        /// 当轮到某人要准备要叫分时
        /// </summary>
        protected void OnTypeGrabSpeaker(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            if (!DDzUtil.IsServDataContainAllKey(
                new[]
                    {
                        RequestKey.KeySeat, NewRequestKey.KeyTttype
                    }, data))
            {
                return;
            }

            if (App.GetGameData<DdzGameData>().SelfSeat != data.GetInt(RequestKey.KeySeat))
            {
                HideAllBtn();
                return;
            }

            if (ServDataTemp == null) ServDataTemp = new SFSObject();

            ServDataTemp = data;
            //把与"qt"相同引用值的 "ttype" 的值赋值过来
            ServDataTemp.PutInt(NewRequestKey.KeyQt, data.GetInt(NewRequestKey.KeyTttype));
            RefreshUiInfo();
        }


        /// <summary>
        /// 当有人叫了分了
        /// </summary>
        /// <param name="args"></param>
        protected void OnTypeGrab(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            //如果是玩家自己叫了分了则隐藏叫分面板
            if (data.ContainsKey(RequestKey.KeySeat) &&
                data.GetInt(RequestKey.KeySeat) == App.GameData.SelfSeat)
            {
                HideAllBtn();
            }
        }

        public override void RefreshUiInfo()
        {
            if (_autoState)
            {
                //托管状态,向服务器发送不叫
                App.GetRServer<DdzGameServer>().CallGameScore(App.GameData.SelfSeat, 0);
                return;
            }

            if (ServDataTemp == null) return;

            //显示叫分按钮或者显示抢地主按钮
            SetActive(CallLandParent, _robModel);
            SetActive(CallBtnsParent, !_robModel);

            SetCallScoreWithFlowUi();
        }

        /// <summary>
        /// 叫分带流分时的ui设置
        /// </summary>
        private void SetCallScoreWithFlowUi()
        {
            //如果服务器没有返回score和minscore,则出现错误,隐藏分数选择界面
            if (!ServDataTemp.ContainsKey(NewRequestKey.KeyMinScore) &&
                !ServDataTemp.ContainsKey(NewRequestKey.KeyScore))
            {
                HideAllBtn();
                YxDebug.LogError("轮到自己叫分时服务器没有发minscore或score的类型");
                return;
            }

            //获取叫分分数
            var score = 0;
            if (ServDataTemp.ContainsKey(NewRequestKey.KeyScore))
            {
                score = ServDataTemp.GetInt(NewRequestKey.KeyScore);
            }

            if (ServDataTemp.ContainsKey(NewRequestKey.KeyMinScore))
            {
                var minscore = ServDataTemp.GetInt(NewRequestKey.KeyMinScore);

                if (minscore > score) score = minscore;
            }

            //设置按钮状态
            if (_robModel)
            {
                SetBtnSprite(CallLandBtn, score > 0 ? RobLandBtnSprName : CallLandBtnSprName);  //叫地主按钮的图片
            }
            else
            {
                //设置叫分按钮状态
                int len = CallScoreBtns.Length;
                for (int i = 0; i < len; i++)
                {
                    SetBtnState(CallScoreBtns[i], i > score - 1);
                }
            }

            //如果玩家手牌中有 2王，4个2， 玩家直接叫3分（最高分） 
            if (LuckyCardsSwitch || SuperLuckyCardsSwitch)
            {
                //重置按钮状态
                SetBtnState(NoCallBtn, true);
                Facade.EventCenter.DispatchEvent<string, bool>(GlobalConstKey.CheckLuckyCards, false);
            }
        }

        /// <summary>
        /// 隐藏所有按钮
        /// </summary>
        void HideAllBtn()
        {
            SetActive(CallBtnsParent, false);
            SetActive(CallLandParent, false);
        }


        void SetActive(GameObject go, bool active)
        {
            if (go == null) return;
            go.SetActive(active);
        }

        /// <summary>
        /// 设置按钮的图片
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="sprName"></param>
        void SetBtnSprite(UIButton btn, string sprName)
        {
            btn.normalSprite = string.Format("{0}_up", sprName);
            btn.pressedSprite = string.Format("{0}_over", sprName);
            btn.hoverSprite = string.Format("{0}_over", sprName);
        }

        /*        /// <summary>
                /// 重置按钮状态
                /// </summary>
                void ResetBtnState()
                {
                    var btngobs = new[] {NoCallBtn, Call1Btn, Call2Btn, Call3Btn};
                    foreach (var btngob in btngobs)
                    {
                        btngob.SetActive(true);
                        btngob.GetComponent<UISprite>().color = new Color(1, 1, 1);
                        btngob.GetComponent<BoxCollider>().enabled = true;
                    }
                }*/


        /// <summary>
        /// 设置按钮的状态
        /// </summary>
        /// <param name="gob"></param>
        /// <param name="couldClick"></param>
        void SetBtnState(GameObject gob, bool couldClick)
        {
            var btn = gob.GetComponent<UIButton>();
            if (btn == null) return;
            btn.GetComponent<BoxCollider>().enabled = couldClick;
            btn.state = couldClick ? UIButtonColor.State.Normal : UIButtonColor.State.Disabled;
        }

        private void luckyCardResult(bool isHas)
        {
            if (isHas)
            {
                int len = CallScoreBtns.Length;
                for (int i = 0; i < len; i++)
                {
                    var flag = i == (len - 1);
                    SetBtnState(CallScoreBtns[i], flag);
                }
                SetBtnState(NoCallBtn, !SuperLuckyCardsSwitch);
            }
        }
    }
}
