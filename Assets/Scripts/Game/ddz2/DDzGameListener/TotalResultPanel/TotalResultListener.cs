using System;
using System.Collections.Generic;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.DDz2Common.ImgPress;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using Assets.Scripts.Game.ddz2.InheritCommon;
using YxFramwork.Common.Adapters;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.TotalResultPanel
{
    public class TotalResultListener : ServEvtListener
    {

        /// <summary>
        /// 总成绩ui父节点
        /// </summary>
        [SerializeField]
        protected GameObject UiGameObject;

        /// <summary>
        /// 玩家结算的数据结构gameobject
        /// </summary>
        [SerializeField]
        protected GameObject ItemsOrgGob;

        /// <summary>
        /// 截图对象
        /// </summary>
        [SerializeField]
        protected CompressImg Img;

        /// <summary>
        /// 每个玩家的信息item的grid
        /// </summary>
        [SerializeField]
        protected GameObject GridGob;

        /// <summary>
        /// 总结算发过来的信息缓存
        /// </summary>
        private ISFSObject _ttGameResultData;

        /// <summary>
        /// 游戏信息
        /// </summary>
        private ISFSObject _gameInfo;

        /// <summary>
        /// 规则说明
        /// </summary>
        [SerializeField]
        private UILabel _ruleInfo;

        /// <summary>
        /// 局数说明
        /// </summary>
        [SerializeField]
        private UILabel _roundInfo;

        /// <summary>
        /// 房间ID
        /// </summary>
        [SerializeField]
        private UILabel _roomId;

        /// <summary>
        /// 当前系统时间
        /// </summary>
        [SerializeField]
        private UILabel _nowTime;

        /// <summary>
        /// 房前房主创建的房间对应名称
        /// </summary>
        [SerializeField]
        private UILabel _roomerInfo;

        /// <summary>
        /// 大赢家头像
        /// </summary>
        [SerializeField]
        private YxBaseTextureAdapter _bigWinnerImg;


        protected override void OnAwake()
        {
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyGetGameInfo, OnGameInfo);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyOnRejoin, OnGameInfo);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyRoomGameOver, OnGameOverEvt);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyShowReadyBtn, ShowTtResultView);
        }

        private void ShowTtResultView(DdzbaseEventArgs obj)
        {
            if (_ttGameResultData == null)
                return;
            if (!App.GetGameData<DdzGameData>().IsRoomGame)
                return;
            ShowTtResultView();
        }

        /// <summary>
        /// 显示总结算界面
        /// </summary>
        void ShowTtResultView()
        {
            if (!App.GetGameData<DdzGameData>().IsRoomGame)
                return;

            UiGameObject.SetActive(true);
            SetData(_ttGameResultData);
            _ttGameResultData = null;
        }

        /// <summary>
        /// 当房间游戏结束时
        /// </summary>
        /// <param name="args"></param>
        private void OnGameOverEvt(DdzbaseEventArgs args)
        {
            _ttGameResultData = args.IsfObjData; //赋值总结算数据

            ShowTtResultView(); //解散房间的情况下,直接显示总结算界面
        }

        /// <summary>
        /// 游戏初始化部分信息需要缓存，用于结算显示
        /// </summary>
        /// <param name="args"></param>
        private void OnGameInfo(DdzbaseEventArgs args)
        {
            _gameInfo = args.IsfObjData;
        }

        public override void RefreshUiInfo()
        {

        }


        /// <summary>
        /// 设置总结算数据
        /// </summary>
        public void SetData(ISFSObject data)
        {
            ISFSArray userArray = data.GetSFSArray("users");

            DDzUtil.ClearPlayerGrid(GridGob);
            int bigWinnerSeat = -1;
            int bigScore = -1;

            for (int i = 0; i < userArray.Count; i++)
            {
                var gob = GridGob.AddChild(ItemsOrgGob);
                gob.SetActive(true);
                var userInfo = userArray.GetSFSObject(i);
                var resultItem = gob.GetComponent<ResultItem>();
                resultItem.SetUserInfo(userInfo);
                int tempScore = userInfo.GetInt("gold");
                if (tempScore > bigScore)
                {
                    bigScore = tempScore;
                    bigWinnerSeat = userInfo.GetInt("seat");
                }
            }

            GridGob.GetComponent<UIGrid>().repositionNow = true;

            if (_bigWinnerImg != null)
            {
                var gdata = App.GetGameData<DdzGameData>();
                var bigWinnerInfo = gdata.GetOnePlayerInfo(bigWinnerSeat, true);
                YxFramwork.Common.DataBundles.PortraitDb.SetPortrait(bigWinnerInfo.AvatarX, _bigWinnerImg,
                    bigWinnerInfo.SexI);
            }

            InfoAbout(data);
        }


        /// <summary>
        /// 大结算周边信息处理
        /// </summary>
        /// <param name="data"></param>
        private void InfoAbout(ISFSObject data)
        {
            if (_roundInfo)
            {
                var now = data.GetInt("round");
                var total = data.GetInt("maxRound");
                _roundInfo.text = string.Format("{0}/{1}", now, total);
            }
            if (_nowTime)
            {
                var time = data.GetLong("svt");
                DateTime nowTime = GetSvtTime(time);
                _nowTime.text = nowTime.ToString("yyyy-MM-dd hh:mm:ss");
            }
            if (_ruleInfo)
            {
                _ruleInfo.text = _gameInfo.GetUtfString("rule");
            }

            if (_roomerInfo)
            {
                _roomerInfo.text = _gameInfo.GetUtfString("roomName");
            }

            if (_roomId)
            {
                _roomId.text = _gameInfo.GetInt("rid").ToString();
            }
        }


        /// <summary>
        /// 时间转化
        /// </summary>
        /// <param name="svt"></param>
        /// <returns></returns>
        public DateTime GetSvtTime(long svt)
        {
            DateTime s = new DateTime(1970, 1, 1, 8, 0, 0);
            s = s.AddSeconds(svt);
            return s;
        }

        /// <summary>
        /// 点击返回大厅按钮
        /// </summary>
        public void OnClickBackHall()
        {
            //App.GetGameData<DdzGameData>().ClearParticalGob();
            UiGameObject.SetActive(false);
            App.QuitGame();
        }

        private string _screenshot;

        /// <summary>
        /// 点击分享战绩按钮
        /// </summary>
        public void OnCLickShare()
        {
            _screenshot = App.UI.CaptureScreenshot();
            Invoke("CaptureScreenshot", 1f);
        }

        void CaptureScreenshot()
        {
            Facade.Instance<WeChatApi>().InitWechat(App.Config.WxAppId);
            var dic = new Dictionary<string, object>();
            dic["type"] = 0;
            dic["game_key_c"] = App.GameKey;
            dic["image"] = _screenshot;
            dic["shareType"] = 1;
            dic["sharePlat"] = 0;
            UserController.Instance.GetShareInfo(dic, info => Facade.Instance<WeChatApi>().ShareContent(info, str =>
            {
                var dict = new Dictionary<string, object>();
                dict["option"] = 2;
                dict["sharePlat"] = SharePlat.WxSenceSession.ToString();
                Facade.Instance<TwManager>().SendAction("shareGameResultRequest", dict, null);
            }));
        }

    }
}
