using System;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.pdk.DDz2Common;
using Assets.Scripts.Game.pdk.DDz2Common.ImgPress;
using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.pdk.DDzGameListener.TotalResultPanel
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

        [SerializeField]
        protected UILabel PdkNumlabel;

        /// <summary>
        /// 房间id
        /// </summary>
        [SerializeField]
        protected UILabel Roomidlabel;

        [SerializeField]
        protected UILabel Timedatelabel;

        /// <summary>
        /// 总结算发过来的信息缓存
        /// </summary>
        private ISFSObject _ttGameResultData;

        public static TotalResultListener Instance { private set; get; }

        /// <summary>
        /// gameinfo
        /// </summary>
        private ISFSObject _gameInfoTemp;

        protected override void OnAwake()
        {
            Instance = this;

            PdkGameManager.AddOnGameInfoEvt(OnGameInfo);
            PdkGameManager.AddOnGetRejoinDataEvt(OnRejoinGame);
            Ddz2RemoteServer.AddOnGameOverEvt(OnGameOverEvt);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeAllocate, OnAlloCateCds);

        }

        private int _maxRound = 0;
        private int _curRound = 0;
        private void OnAlloCateCds(object sender, DdzbaseEventArgs args)
        {
            _curRound++;
        }

        private void OnRejoinGame(object sender, DdzbaseEventArgs args)
        {
            _gameInfoTemp = args.IsfObjData;
            CheckPdkNumAndRoundInfo(_gameInfoTemp);
        }

        private void OnGameInfo(object sender, DdzbaseEventArgs args)
        {
            _gameInfoTemp = args.IsfObjData;
            CheckPdkNumAndRoundInfo(_gameInfoTemp);
        }

        /// <summary>
        /// 检查跑得快的张数
        /// </summary>
        /// <param name="data"></param>
        private void CheckPdkNumAndRoundInfo(ISFSObject data)
        {

            if (data.ContainsKey(NewRequestKey.KeyMaxRound))
                _maxRound = _gameInfoTemp.GetInt(NewRequestKey.KeyMaxRound);

            if (data.ContainsKey(NewRequestKey.KeyCurRound))
                _curRound = _gameInfoTemp.GetInt(NewRequestKey.KeyCurRound);

            if (data.ContainsKey(NewRequestKey.KeyClientArgs2))
            {
                var roomData = data.GetSFSObject(NewRequestKey.KeyClientArgs2);
                if (roomData.ContainsKey(NewRequestKey.KeyModel))
                {
                    PdkNumlabel.text = roomData.GetUtfString(NewRequestKey.KeyModel).Equals("2") ? "跑得快15张" : "跑得快16张";
                }
            }
        }

        /// <summary>
        /// 场景销毁后，重置静态变量
        /// </summary>
        private void OnDestroy()
        {
            Instance = null;
        }

        private void OnGameOverEvt(object sender, DdzbaseEventArgs args)
        {
            _ttGameResultData = args.IsfObjData;
        }

        public override void RefreshUiInfo()
        {
            if (_ttGameResultData == null) return;

            UiGameObject.SetActive(true);
            SetData(_ttGameResultData);

            _ttGameResultData = null;
        }

        /// <summary>
        /// 标记是否已经结束所有牌局
        /// </summary>
        public bool IsEndAllRound
        {
            get { return _ttGameResultData != null; }
        }


        /// <summary>
        /// 设置总结算数据
        /// </summary>
        public void SetData(ISFSObject data)
        {

            string time = DateTime.Now.Year + "." + DateTime.Now.Month + "." + DateTime.Now.Day +
              "  " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
            Timedatelabel.text = time;

            SetidAndRoundInfo();


            ISFSArray userArray = data.GetSFSArray("users");

            DDzUtil.ClearPlayerGrid(GridGob);

            int maxScore = 0;
            ResultItem useritem = null;
            for (int i = 0; i < userArray.Count; i++)
            {
                var gob = NGUITools.AddChild(GridGob, ItemsOrgGob);
                gob.SetActive(true);
                var resultItem = gob.GetComponent<ResultItem>();
                var isfData = userArray.GetSFSObject(i);
                resultItem.SetUserInfo(isfData);

                var curScore = isfData.GetInt("gold");
                if (curScore != 0 && maxScore <= curScore)
                {
                    maxScore = curScore;
                    useritem = resultItem;
                }
            }

            //确定大赢家图标
            if (useritem != null) useritem.BigWinner.SetActive(true);

            GridGob.GetComponent<UIGrid>().repositionNow = true;
        }


        /// <summary>
        /// 设置id和局数信息
        /// </summary>
        private void SetidAndRoundInfo()
        {
            if (_gameInfoTemp == null) return;
            string roomidstr = "";
            string roundstr = "";
            if (_gameInfoTemp.ContainsKey(NewRequestKey.KeyRoomId))
            {
                roomidstr = "房间号:" + _gameInfoTemp.GetInt(NewRequestKey.KeyRoomId);
            }
            roundstr = "  局数:" + _curRound.ToString(CultureInfo.InvariantCulture) + "/" + _maxRound.ToString(CultureInfo.InvariantCulture);
            Roomidlabel.text = roomidstr + roundstr;
        }

        /// <summary>
        /// 点击返回大厅按钮
        /// </summary>
        public void OnClickBackHall()
        {
            //App.GetGameData<GlobalData>().ClearParticalGob();
            UiGameObject.SetActive(false);
            //App.QuitGameWithMsgBox();
            App.QuitGame();//退回大厅
        }
        private string _screenshot;
        /// <summary>
        /// 点击分享战绩按钮
        /// </summary>
        public void OnCLickShare()
        {
            _screenshot = App.UI.CaptureScreenshot();
            Invoke("CaptureScreenshot", 1f);
            //老版分享
            //Facade.Instance<WeChatApi>().InitWechat(AppInfo.WxAppId);

            //UserController.Instance.GetShareInfo(delegate (ShareInfo info)
            //{
            //    YxWindowManager.HideWaitFor();
            //    Img.DoScreenShot(new Rect(0, 0, Screen.width, Screen.height), imageUrl =>
            //    {
            //        YxDebug.Log("Url == " + imageUrl);
            //        if (Application.platform == RuntimePlatform.Android)
            //        {
            //            imageUrl = "file://" + imageUrl;
            //        }
            //        info.ImageUrl = imageUrl;
            //        info.ShareType = ShareType.Image;
            //        Facade.Instance<WeChatApi>().ShareContent(info, str =>
            //        {
            //            //成功后给奇哥发消息
            //            var parm = new Dictionary<string, object>
            //                            {
            //                                {"option", 2},
            //                                {"bundle_id", Application.bundleIdentifier},
            //                                {"share_plat", SharePlat.WxSenceTimeLine.ToString()},
            //                            };
            //            Facade.Instance<TwManager>().SendActionKey("shareAwards", parm, null);
            //        });
            //    });
            //});
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
                //Facade.Instance<TwManager>().SendActionKey("shareGameResultRequest", dict, null);
            }));
        }
    }
}
