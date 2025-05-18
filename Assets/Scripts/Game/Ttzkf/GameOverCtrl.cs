using System;
using System.Collections.Generic;
using System.Globalization;
using com.yxixia.utile.Utiles;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.Ttzkf
{
    public class GameOverCtrl : MonoBehaviour
    {
        public UILabel RoomNum;
        public UILabel CurrentTime;
        public UILabel CurrentRound;
        public UIGrid GameOverGrid;
        public GameOverItem GameOverItem;
        public GameObject ShowParent;
        public CompressImg Img;

        private int _maxGold;
        private int _maxUser;

        public void AnalysisData(ISFSObject requestData)
        {
            ShowParent.SetActive(true);
            RoomNum.text = App.GetGameData<TtzGameData>().RoomType.ToString(CultureInfo.InvariantCulture);
            var currentRound = requestData.ContainsKey("round") ? requestData.GetInt("round") : 0;
            var maxRound = requestData.ContainsKey("maxRound") ? requestData.GetInt("maxRound") : 0;
            CurrentRound.text = string.Format("{0}/{1}局", currentRound, maxRound);
            CurrentTime.text = DateTime.Now.ToString("yyyy-MM-dd  HH:mm");
            var users = requestData.ContainsKey("users") ? requestData.GetSFSArray("users") : null;
            if (users == null) return;
            for (var i = 0; i < users.Count; i++)
            {
                var user = users.GetSFSObject(i);
                var nick = user.ContainsKey("nick") ? user.GetUtfString("nick") : null;
                if (nick == null) continue;
                var gold = user.ContainsKey("gold") ? user.GetInt("gold") : 0;
                var id = user.ContainsKey("id") ? user.GetInt("id") : 0;
                var obj = YxWindowUtils.CreateItem(GameOverItem, GameOverGrid.transform);
                obj.GetComponent<GameOverItem>().InitData(nick, gold);
                if (_maxGold >= gold) continue;
                _maxGold = gold;
                _maxUser = i;
            }
            GameOverGrid.transform.GetChild(_maxUser).GetComponent<GameOverItem>().BigWinner.SetActive(true);
            GameOverGrid.Reposition();
            GameOverGrid.repositionNow = true;
        }
        public void OnClickShare()
        {
            Img.DoScreenShot(new Rect(0, 0, Screen.width, Screen.height), imageUrl =>
            {
                YxWindowManager.ShowWaitFor();
                Facade.Instance<WeChatApi>().InitWechat(App.Config.WxAppId);
                YxFramwork.Controller.UserController.Instance.GetShareInfo(info =>
                {
                    YxWindowManager.HideWaitFor();
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
                            {"gamekey",App.GameKey},
                            {"bundle_id",Application.bundleIdentifier},
                            {"share_plat",SharePlat.WxSenceTimeLine.ToString() },
                        };
                        Facade.Instance<TwManager>().SendAction("shareAwards", parm, null);
                    });

                });
            });
        }
    }
}
