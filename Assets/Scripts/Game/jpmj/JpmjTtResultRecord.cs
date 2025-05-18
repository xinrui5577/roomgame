using System;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.jpmj.MahjongScripts.GameUI;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.jpmj
{
    public class JpmjTtResultRecord : TtResultRecord
    {
        [SerializeField]
        protected PlayerExtraCombatInfo[] PlayerExtrInfos;
        private readonly Color _rgbGreenColor=new Color(161.0f / 255.0f, 255.0f / 255.0f, 50.0f / 255.0f);
        private readonly Color _rgbRedColor = new Color(228.0f / 255.0f, 19.0f / 255.0f, 19.0f/ 255.0f);
        /// <summary>
        /// key是userid,第二个是iplayer位置
        /// </summary>
        private readonly Dictionary<long, int> _useridI = new Dictionary<long, int>();

        public override void Show(TableData tableData, Texture[] defineArray)
        {
            _useridI.Clear();
            gameObject.SetActive(true);
            if (RoomID != null) RoomID.text = tableData.RoomInfo.RoomID + "";
            if (Round != null) Round.sprite = tableData.RoomInfo.GameLoopType == EnGameLoopType.round ? JuImg : QuanImg;
            if (RoundCnt != null) RoundCnt.text = tableData.RoomInfo.CurrRound + "/" + tableData.RoomInfo.MaxRound;

            foreach (UiTTrtPlayerInfo uiTTrtPlayerInfo in Players)
            {
                uiTTrtPlayerInfo.SetVisible = false;
            }

            var data = tableData.TotalResult;
            for (int i = 0; i < UtilData.CurrGamePalyerCnt; i++)
            {
                Players[i].Clear();
                Players[i].SetVisible = true;
                Players[i].UserName = data.Name[i];
                Players[i].UserId = data.Id[i].ToString(CultureInfo.InvariantCulture);
                _useridI[data.Id[i]] = i;
                var ttscore = data.Glod[i];
                if (ttscore < 0)
                {
                    PlayerExtrInfos[i].TotalScores.color =_rgbGreenColor;
                    PlayerExtrInfos[i].TotalScores.text = YxUtiles.GetShowNumber(ttscore).ToString(CultureInfo.InvariantCulture);
                }

                else
                {
                    PlayerExtrInfos[i].TotalScores.color = _rgbRedColor;
                    PlayerExtrInfos[i].TotalScores.text = "+" + YxUtiles.GetShowNumber(ttscore).ToString(CultureInfo.InvariantCulture);
                }
                Players[i].IsFangZhu = i == tableData.OwnerSeat;
                Players[i].IsBestPao = i == data.BeatPaoSeat;
                Players[i].IsBigWinner = i == data.BigWinnerSeat;
                int sex = tableData.UserDatas[i].Sex;
                sex = sex >= 0 ? sex : 0;
                Players[i].SetHeadImg(tableData.UserDatas[i].HeadImage, defineArray[sex % 2]);


                var scoreTexts = PlayerExtrInfos[i].Texts;
                foreach (var scoreText in scoreTexts)
                {
                    scoreText.text = "";
                }
            }
            CombatGainsController.Instance.GetGameDetail(App.GameKey, tableData.RoomInfo.RoomIdOnly.ToString(CultureInfo.InvariantCulture), UpdateViewData);
            //根据owerid判断房主
            var jpTableData = tableData as JpTableData;
            if (jpTableData == null) return;
            int owerId = jpTableData.OwerId;
            var ttdata = tableData.TotalResult;
            for (int i = 0; i < UtilData.CurrGamePalyerCnt; i++)
            {
                Players[i].IsFangZhu = ttdata.Id[i] == owerId;
            }

            if (RoomID != null) RoomID.text = "房间：" + RoomID.text + "  " + jpTableData.Rule;
        }

        private void UpdateViewData(object msg)
        {
            try
            {
                if (msg == null) return;
                var dict = msg as Dictionary<string, object>;
                if (dict == null) return;
                if (dict.ContainsKey("detail"))
                {
                    var list = dict["detail"] as List<object>;
                    if (list != null)
                    {

                        var dirIInfo = new Dictionary<int, List<long>>();

                        var count = list.Count;
                        for (var i = 0; i < count; i++)
                        {
                            var obj = list[i];
                            if (obj == null) continue;
                            if (!(obj is Dictionary<string, object>)) continue;
                            var cgdDict = obj as Dictionary<string, object>;

                            var infoh = cgdDict["info_h"] as List<object>;

                            if (infoh == null) return;
                            foreach (var info in infoh)
                            {
                                var playerInfo = info as Dictionary<string, object>;
                                if (playerInfo != null)
                                {
                                    var id = (long)playerInfo["id"];
                                    if (!dirIInfo.ContainsKey(_useridI[id]))
                                        dirIInfo[_useridI[id]] = new List<long>();
                                    dirIInfo[_useridI[id]].Add((long)playerInfo["gold"]);
                                }
                            }

                        }
                        var uiCount = PlayerExtrInfos.Length;
                        var dataCount = 0;
                        foreach (var i in dirIInfo.Keys)
                        {
                            //给成绩赋值
                            dataCount++;
                            if (dataCount > uiCount)
                            {
                                break;
                            }
                            var scoreTexts = PlayerExtrInfos[i].Texts;
                            var scoreTxtLen = scoreTexts.Length;
                            int k = 0;
                            foreach (var scoreText in dirIInfo[i])
                            {
                                if (scoreText < 0)
                                {
                                    scoreTexts[k].color = _rgbGreenColor;
                                    scoreTexts[k].text = YxUtiles.GetShowNumber(scoreText).ToString(CultureInfo.InvariantCulture);
                                }
                                else
                                {
                                    scoreTexts[k].color = _rgbRedColor;
                                    scoreTexts[k].text = "+" + YxUtiles.GetShowNumber(scoreText).ToString(CultureInfo.InvariantCulture);
                                }

                                k++;
                                if (k >= scoreTxtLen) break;
                            }
                        }




                    }
                }
            }
            catch (Exception)
            {
                Debug.LogError("总结算的回放信息问题：----------");
                Debug.LogError("msg:" + msg);
                var dict = msg as Dictionary<string, object>;
                if (dict != null)
                {
                    if (dict.ContainsKey("detail"))
                    {
                        var list = dict["detail"] as List<object>;
                        if (list != null)
                        {
                            var count = list.Count;
                            for (var i = 0; i < count; i++)
                            {
                                var obj = list[i];
                                if (obj == null) continue;
                                if (!(obj is Dictionary<string, object>)) continue;
                                var cgdDict = obj as Dictionary<string, object>;

                                var infoh = cgdDict["info_h"] as List<object>;

                                if (infoh == null) return;
                                foreach (var info in infoh)
                                {
                                    var playerInfo = info as Dictionary<string, object>;
                                    if (playerInfo != null)
                                    {
                                        var id = (long)playerInfo["id"];
                                        Debug.LogError("玩家id：" + id);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Debug.LogError("detail 不是list");
                        }
                    }
                    else
                    {
                        Debug.LogError("dict不含有detail的key");
                    }
                }
                else
                {
                   Debug.LogError("回放数据不是字典格式");
                }
            }


        }


        /// <summary>
        /// 单局结算分享好友
        /// </summary>
        public void OnJustShareFriend()
        {
#if UNITY_ANDROID || UNITY_IPHONE
            Img.DoScreenShot(new Rect(0, 0, Screen.width, Screen.height), imageUrl =>
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    imageUrl = "file://" + imageUrl;
                }
                OnWeiChatShareGameResult(new EventData(imageUrl));
            });
#endif
        }

        /// <summary>
        /// 分享好友
        /// </summary>
        /// <param name="evn"></param>
        public void OnWeiChatShareGameResult(EventData evn)
        {
            var imageUrl = (string)evn.data1;

            Facade.Instance<WeChatApi>().InitWechat(AppInfo.WxAppId);

            UserController.Instance.GetShareInfo(info =>
            {
                info.ImageUrl = imageUrl;
                info.ShareType = ShareType.Image;
                Facade.Instance<WeChatApi>().ShareContent(info);
            });

        }
    }
}
