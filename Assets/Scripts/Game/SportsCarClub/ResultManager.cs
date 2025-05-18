using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using Sfs2X.Entities.Data;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Game.SportsCarClub
{
    /// <summary>
    /// 结算面板管理 单例
    /// </summary>
    public class ResultManager : MonoBehaviour
    {
        #region 单例

        private static ResultManager _instance;

        public static ResultManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ResultManager();
            }
            return _instance;
        }

        void Awake()
        {
            _instance = this;
        }

        #endregion

        /// <summary>
        /// 结算背景
        /// </summary>
        public GameObject BG;
        /// <summary>
        /// 自己的信息
        /// </summary>
        public GameObject Self;
        /// <summary>
        /// 店家的信息
        /// </summary>
        public GameObject Banker;
        /// <summary>
        /// 名次排行,只显示10人  -- 10+本家+店家
        /// </summary>
        public GameObject[] Top;

        [SerializeField]
        private CompressImg Img;

        /// <summary>
        /// 打开结算界面
        /// </summary>
        public void OpenResult(ISFSObject data = null)
        {
            var selfRd = GetData(Self);
            if (data != null)
            {
                var pwins = data.GetUtfStringArray("pwins");
                var index = 0;

                foreach (var pwin in pwins)
                {
                    var info = pwin.Split(',');

                    if (index < Top.Length && int.Parse(info[1]) != 0)
                    {
                        var topRd = GetData(Top[index]);
                        topRd.Top.text = (index + 1).ToString();
                        topRd.Name.text = info[2];
                        long coin;
                        long.TryParse(info[1], out coin);
                        topRd.Gold.text = "￥" + YxUtiles.GetShowNumberForm(coin);
                        index++;
                    }
                }

                for (var i = index; i < Top.Length; i++)
                {
                    var topRd = GetData(Top[i]);
                    SetDataNull(topRd);
                }
                selfRd.Top.text = "本家";
                selfRd.Gold.text = "￥" + YxUtiles.GetShowNumberForm(data.GetInt("win"));

                if (data.GetInt("win") > 0)
                { 
                    Facade.Instance<MusicManager>().Play("Win");
                    var gameMgr = App.GetGameManager<CschGameManager>();
                    //gameMgr.LeftLampCtrl.PlayLamp();
                    //gameMgr.RightLampCtrl.PlayLamp();
                }
                
                selfRd.Name.text = App.GameData.GetPlayer().Nick;
                var bankerRd = GetData(Banker);
                bankerRd.Name.text = App.GetGameManager<CschGameManager>().BankerMgr.Banker.Nick;
                bankerRd.Gold.text = "￥" + YxUtiles.GetShowNumberForm(data.GetInt("bankWin"));

                PlayerRecord.GetInstance().ShowRealResultRecord();
            }
            else
            {
                SetDataNull(selfRd);

                foreach (var t in Top)
                {
                    var topRd = GetData(t);
                    SetDataNull(topRd);
                }
            }


            
            BG.SetActive(true);
            var game = App.GetGameManager<CschGameManager>();
            var rbMgr = App.GetGameManager<CschGameManager>().RightBottomMgr;
            var upBetValue = rbMgr.UpBetValue;
            if (!game.Execute)
            {
                for (var i = 0; i < game.Regions.Length; i++)
                {
                    upBetValue[i] = 0;
                }
            }
            else
            {
                game.Execute = false;
            }

            RecordCtrl.GetInstance().AssignmentLastRound();

            for (var i = 0; i < game.Regions.Length; i++)
            {
                upBetValue[i] = 0;
            }

            CoinSpriteVisCtrl();
        }

        //控制金币图标显隐
        private void CoinSpriteVisCtrl()
        {
            //本家
            if (Self.GetComponentInChildren<UILabel>().text != "")
                Self.transform.Find("CoinSprite").gameObject.SetActive(true);
            else
                Self.transform.Find("CoinSprite").gameObject.SetActive(false);

            //庄家
            if (Banker.GetComponentInChildren<UILabel>().text != "")
                Banker.transform.Find("CoinSprite").gameObject.SetActive(true);
            else
                Banker.transform.Find("CoinSprite").gameObject.SetActive(false);

            //排名
            foreach (var item in Top)
            {
                if (item.GetComponentInChildren<UILabel>().text != "")
                    item.transform.Find("CoinSprite").gameObject.SetActive(true);
                else
                    item.transform.Find("CoinSprite").gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 关闭结算界面
        /// </summary>
        public void CloseResult()
        {
            BG.SetActive(false);
        }

        public ResultData GetData(GameObject gob)
        {
            return new ResultData
            {
                Top = gob.transform.FindChild("Top").GetComponent<UILabel>(),
                Name = gob.transform.FindChild("Name").GetComponent<UILabel>(),
                Gold = gob.transform.FindChild("Gold").GetComponent<UILabel>()
            };
        }

        public void SetDataNull(ResultData rdData, bool isBanker = false)
        {
            rdData.Gold.text = "";
            rdData.Name.text = "";
            if (!isBanker)
            {
                rdData.Top.text = "";
            }
        }

        public struct ResultData
        {
            public UILabel Top;
            public UILabel Name;
            public UILabel Gold;
        }

        /*public void OnClickShare()
        {
            var selfRd = GetData(Self);

            YxWindowManager.ShowWaitFor();

            var gtype = App.GetGameData<CschGameData>().GameInfo.GetInt("gtype");

            Facade.Instance<WeChatApi>().InitWechat();

            var dic = new Dictionary<string, object>
                            {
                            {"option",1},
                            {"room_id",gtype},
                            {"game_key",App.GameKey},
                            {"winCoin",selfRd.Gold.text},
                            {"bundle_id",Application.bundleIdentifier},
                            {"share_plat",SharePlat.WxSenceSession.ToString() },
                        };
            Facade.Instance<TwManager>().SendActionKey("shareGameResultRequest", dic, data =>
                {
                    if (data == null) return;
                    if (!(data is bool)) return;
                    var isContinue = (bool)data;
                    if (!isContinue) return;
                    UserController.Instance.GetShareInfo(info =>
                        {
                            YxWindowManager.HideWaitFor();
                            Img.DoScreenShot(new Rect(0, 0, Screen.width, Screen.height), imageUrl =>
                                {
                                    if (Application.platform == RuntimePlatform.Android)
                                    {
                                        imageUrl = "file://" + imageUrl;
                                    }
                                    info.ImageUrl = imageUrl;
                                    info.ShareType = ShareType.Image;
                                    Facade.Instance<WeChatApi>().ShareContent(info, str =>
                                        {
                                            var parm = new Dictionary<string, object>
                                                {
                                                    {"option",2},
                                                    {"game_key",App.GameKey},
                                                    {"bundle_id",Application.bundleIdentifier},
                                                    {"share_plat",SharePlat.WxSenceSession.ToString() },
                                                };
                                            Facade.Instance<TwManager>().SendActionKey("shareGameResultRequest", parm, dataInfo =>
                                                {
                                                    if (dataInfo == null) return;
                                                    if (!(dataInfo is Dictionary<string, object>)) return;
                                                    var dataDic = dataInfo as Dictionary<string, object>;
                                                    var awardInfo = dataDic.ContainsKey("awardInfo")
                                                                        ? dataDic["awardInfo"].ToString()
                                                                        : "";
                                                    var message = new YxMessageBoxData { Msg = awardInfo };
                                                    YxMessageBox.Show(message);
                                                });
                                        });
                                });
                        });
                });
        }*/

        /*public void OnClickShareFriend()
        {
            var selfRd = GetData(Self);
            YxWindowManager.ShowWaitFor();

            if (!Facade.Instance<WeChatApi>().CheckWechatValidity()) return;

            Facade.Instance<WeChatApi>().InitWechat();

            var gtype = App.GetGameData<CschGameData>().GameInfo.GetInt("gtype");


            var dic = new Dictionary<string, object>
                            {
                            {"option",1},
                            {"room_id",gtype},
                            {"game_key",App.GameKey},
                            {"winCoin",selfRd.Gold.text},
                            {"bundle_id",Application.bundleIdentifier},
                            {"share_plat",SharePlat.WxSenceTimeLine.ToString() },
                        };
            Facade.Instance<TwManager>().SendActionKey("shareGameResultRequest", dic, data =>
            {
                if (!(data is bool)) return;
                var isContinue = (bool)data;
                if (!isContinue) return;
                UserController.Instance.GetShareInfo(info =>
                {
                    YxWindowManager.HideWaitFor();
                    Img.DoScreenShot(new Rect(0, 0, Screen.width, Screen.height), imageUrl =>
                    {
                        if (Application.platform == RuntimePlatform.Android)
                        {
                            imageUrl = "file://" + imageUrl;
                        }
                        info.ImageUrl = imageUrl;
                        info.ShareType = ShareType.Image;
                        info.Plat = SharePlat.WxSenceTimeLine;

                        Facade.Instance<WeChatApi>().ShareContent(info, str =>
                        {
                            var parm = new Dictionary<string, object>
                            {
                            {"option",2},
                            {"game_key",App.GameKey},
                            {"bundle_id",Application.bundleIdentifier},
                            {"share_plat",SharePlat.WxSenceTimeLine.ToString() },
                        };
                            Facade.Instance<TwManager>().SendActionKey("shareGameResultRequest", parm, dataInfo =>
                                {
                                    var dataDic = dataInfo as Dictionary<string, object>;
                                    if (dataDic == null) return;
                                    
                                    var awardInfo = dataDic.ContainsKey("awardInfo")
                                                        ? dataDic["awardInfo"].ToString()
                                                        : "";
                                    var message = new YxMessageBoxData { Msg = awardInfo };
                                    YxMessageBox.Show(message);
                                });
                        });
                    });
                });
            });

        }*/
    }
}
