using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Assets.Scripts.Game.jpmj.MahjongScripts.GameUI;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;
using AsyncImage = Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool.AsyncImage;

namespace Assets.Scripts.Game.jpmj
{
    public class JpOneRoundResult : OneRoundResult
    {
        [SerializeField]
        protected OneRdExtraMessShow[] OnerdExtraMsgItems;

        [SerializeField]
        protected Sprite WinbgImg;
        [SerializeField]
        protected Sprite LosebgImg;
        [SerializeField]
        protected Sprite HebgImg;

        
        /// <summary>
        /// 玩家自己的胡分数
        /// </summary>
        [SerializeField]
        protected Text SelfPlayerHuScoreText;
        [SerializeField]
        protected Text SelfPlayerHuScoreText1;


        /// <summary>
        /// 分享胡的方式信息
        /// </summary>
        [SerializeField]
        protected Text ShareHuInfoText;


        /// <summary>
        /// 玩家自己的连庄个数
        /// </summary>
        [SerializeField]
        protected Text SelfPlayerlianzhuangZhuangText;
        [SerializeField]
        protected Text SelfPlayerlianzhuanInfoText;


        [SerializeField]
        protected GameObject ShareScoreUiGob;
        /// <summary>
        /// 如果胡了显示这个图片
        /// </summary>
        [SerializeField]
        protected GameObject HurawImgBg;
        /// <summary>
        /// 如果没胡显示花的图片
        /// </summary>
        [SerializeField]
        protected GameObject HuaShareRawImgBg;
        /// <summary>
        /// 未胡时的花数，
        /// </summary>
        [SerializeField]
        protected Text HuaNumtext;
        [SerializeField]
        protected Text HuaNumtext1;

        [SerializeField]
        protected GameObject ShareLianZhuangUiGob;

        [SerializeField]
        protected GameObject ShareFlowerUiGob;

        [SerializeField]
        protected GameObject Btns;

        /// <summary>
        /// 玩家自己的3种界面的分享图片
        /// </summary>
        [SerializeField]
        protected RawImage ShareHeadImgScore;

        /// <summary>
        /// 玩家自己的3种界面的分享图片
        /// </summary>
        [SerializeField]
        protected RawImage ShareHeadImgLianzhuang;


        /// <summary>
        /// 玩家自己的3种界面的分享图片
        /// </summary>
        [SerializeField]
        protected RawImage ShareHeadImgHua;

        /// <summary>
        /// 分数二维码显示
        /// </summary>
        [SerializeField]
        protected GameObject[] ErweiGobs;


        [SerializeField] protected GameObject BackToHallBtn;

        /// <summary>
        /// 牌信息
        /// </summary>
        [SerializeField] protected GameObject CardsInfoZone;

        [SerializeField] protected BigCdStyleTypePlay BigCdStyleTypePlay;

        [SerializeField] protected OnrdUiSizeCtrl OnerdUisizwCtrl;


        public void BackToHallClick()
        {
            App.QuitGame();
        }

        /// <summary>
        /// 设置玩家自己的分享头像
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headimg"></param>
        /// <param name="define"></param>
        private void SetShareHeadImg(string url,RawImage headimg,Texture define)
        {
            AsyncImage.GetInstance().SetTextureWithAsyncImage(url, headimg, define);
        }
        public override void SetShowInfo(TableData table, Texture[] defineArray)
        {
            ShareScoreUiGob.SetActive(false);
            ShareLianZhuangUiGob.SetActive(false);
            ShareFlowerUiGob.SetActive(false);
            Btns.SetActive(false);
            foreach (var ewgob in ErweiGobs)
            {
                ewgob.SetActive(false);
            }
            BackToHallBtn.SetActive(table.RoomInfo.RoomType == EnRoomType.YuLe);

            gameObject.SetActive(true);

            MahjongManager.Instance.Reset();

            if (RoomID != null) RoomID.text = table.RoomInfo.RoomID + "";
            if (RoundImg != null) RoundImg.sprite = table.RoomInfo.GameLoopType == EnGameLoopType.round ? JuImg : QuanImg;
            if (Round != null) Round.text = table.RoomInfo.CurrRound + "/" + table.RoomInfo.MaxRound;

            foreach (OnRoudResultItem item in Items)
            {
                item.SetVisible = false;
            }

            var seatList = new List<int>();
            for (int i = 0; i < UtilData.CurrGamePalyerCnt; i++)
            {
                seatList.Add(i);
            }
            //根据胡牌玩家排序
            seatList.Sort((a, b) =>
            {
                bool isAHu = table.Result.HuSeat.Contains(a);
                bool isBHu = table.Result.HuSeat.Contains(b);
                if (isAHu && !isBHu) return -1;

                if (!isAHu && isBHu) return 1;

                if (isAHu && isBHu)
                {
                    int aScore = table.Result.HuGold[a];
                    int bScore = table.Result.HuGold[b];

                    if (aScore < bScore) return -1;
                    if (aScore > bScore) return 1;
                }

                return 0;
            });

            UtilFunc.OutPutList(seatList, "游戏结束 玩家胡牌排序后");
            if (BaoArea != null)
            {
                int bao = table.Result.bao;

                if (bao > 0)
                {
                    GameObject baoItem = D2MahjongMng.Instance.GetMj(bao, EnD2MjType.Up, bao);
                    baoItem.transform.parent = BaoArea;
                    baoItem.transform.localPosition = Vector3.zero;
                    baoItem.transform.localScale = Vector3.one;
                }

            }
            //是否赢
            bool isMeWin = table.Result.HuSeat.Contains(table.PlayerSeat);
            //是否流局
            bool isLiuJu = table.Result.HuType == MjRequestData.MJReqTypeLastCd;
  
            int huCnt = 1;
            for (int i = 0; i < seatList.Count; i++)
            {
                int seat = seatList[i];
                Items[i].SetVisible = true;
                Items[i].Name = table.UserDatas[seat].name;


                var hunameInfo = table.Result.HuName[seat];


                Items[i].StrHuInfo = hunameInfo;
                Items[i].IntHuScore = table.Result.HuGold[seat];
                Items[i].IntGangScore = table.Result.GangGlod[seat];
                Items[i].IntScore = table.Result.Gold[seat];
                Items[i].IsHu = false;
                Items[i].IsBanker = seat == table.BankerSeat;
                // 码分
                Items[i].SetNiaoScore = table.Result.NiaoGold[seat];
                //吃碰杠加入
                Items[i].SetCpgCardJp(table.UserCpg[seat], table.Laizi);
                Items[i].SetHardCard(table.UserHardCard[seat].ToArray(), table.Laizi);

                bool ishu = false;
                //设置胡牌信息
                if (huCnt++ <= table.Result.HuSeat.Count)
                {
                    ishu = true;
                    Items[i].IsHu = true;
                    Items[i].SetHardCard(new[] { table.Result.HuCard }, table.Laizi);
                }

                Items[i].SortCardGroup();

                Items[i].HuTypeValue = table.Result.UserHuType[seat];

                Items[i].SetHeadImg(table.UserDatas[seat].HeadImage, defineArray[table.UserDatas[seat].Sex % 2]);

                if (OnerdExtraMsgItems[i] != null)
                {
                    var bgimg = OnerdExtraMsgItems[i].GetComponent<Image>();
                    if (bgimg != null)
                    {
                        if (!isLiuJu) bgimg.sprite = isMeWin ? WinbgImg : LosebgImg;
                        else
                            bgimg.sprite = HebgImg;
                    }

                    var buzhangList = new int[] {};
                    var buzhangLen = table.Result.BuZhangList.Count;
                    if (buzhangLen >= seat && buzhangLen > 0) buzhangList = table.Result.BuZhangList[seat];
                    OnerdExtraMsgItems[i].SetExtraInfo(table.Result.Huaname[seat],
                                                       table.Result.Taicnt[seat], table.Result.Huacnt[seat], CheckLaiziGroup(table.UserHardCard[seat].ToArray(), table.Result.HuCard, 
                                                       ishu, table.Laizi), buzhangList, table.Laizi,
                                                       table.Result.IsBaoPai[seat], table.Result.LianZhuangInfo[seat], table.Fanpai);
                }


                #region  添加分享信息
                var shareGoldScore = table.Result.ShareShowGold[seat];
                if (seat == table.PlayerSeat)
                {
                    var jptabledata = table as JpTableData;

                    if (jptabledata != null)
                    {
                        //胡了，且分数足够分享
                        var ishuAndShareScore = ishu && shareGoldScore >= jptabledata.ShareHuScoreValue;
                        //没胡，但花数足够分享
                        var nohuAndShareHua = !ishu && table.Result.Huacnt[seat]>= jptabledata.ShareHuHuaNumValue;
                        if (ishuAndShareScore || nohuAndShareHua)
                        {
                            ShareScoreUiGob.SetActive(true); 

                            //分数信息
                            if (ishuAndShareScore)
                            {
                                SelfPlayerHuScoreText.text = YxUtiles.GetShowNumber(shareGoldScore).ToString(CultureInfo.InvariantCulture)+"分";
                                SelfPlayerHuScoreText1.text = "总积分:" + YxUtiles.GetShowNumber(shareGoldScore).ToString(CultureInfo.InvariantCulture) + "分";
                                HurawImgBg.SetActive(true);
                                HuaShareRawImgBg.SetActive(false);
                            }
                            else if (nohuAndShareHua)
                            {

                                HuaNumtext.text = "花:" + table.Result.Huacnt[seat];
                                HuaNumtext1.text = "花:" + table.Result.Huacnt[seat];
                                HurawImgBg.SetActive(false);
                                HuaShareRawImgBg.SetActive(true);
                            }


                            //头像信息
                            SetShareHeadImg(table.UserDatas[seat].HeadImage, ShareHeadImgScore,
                                            defineArray[table.UserDatas[seat].Sex % 2]);

                            //按钮
                            Btns.SetActive(true);

                            


                            var cdinfoOnrditem = CardsInfoZone.GetComponent<OnRoudResultItem>();
                            cdinfoOnrditem.Cleare();
                            cdinfoOnrditem.SetCpgCardJp(table.UserCpg[seat], table.Laizi);
                            cdinfoOnrditem.SetHardCard(table.UserHardCard[seat].ToArray(), table.Laizi);
                            if (ishu)
                            {
                                cdinfoOnrditem.SetHardCard(new[] { table.Result.HuCard }, table.Laizi);
                            }
                            cdinfoOnrditem.SortCardGroup();

                            cdinfoOnrditem.StrHuInfo = hunameInfo;

                            var onrdextramessage = CardsInfoZone.GetComponent<OneRdExtraMessShow>();
                            onrdextramessage.SetExtraInfo(table.Result.Huaname[seat],
                                                       table.Result.Taicnt[seat], table.Result.Huacnt[seat], CheckLaiziGroup(table.UserHardCard[seat].ToArray(), table.Result.HuCard,
                                                       ishu, table.Laizi), table.Result.BuZhangList[seat], table.Laizi,
                                                       table.Result.IsBaoPai[seat], table.Result.LianZhuangInfo[seat],table.Fanpai);
                            onrdextramessage.SetEstraCds();
                        }
                        else if (table.Result.LianZhuangNumInfo[seat] >= jptabledata.LianzhuangNum)
                            {
                                //连庄数信息
                                SelfPlayerlianzhuangZhuangText.text = table.Result.LianZhuangNumInfo[seat].ToString(CultureInfo.InvariantCulture);
                                //连庄信息    
                                SelfPlayerlianzhuanInfoText.text =  table.Result.LianZhuangInfo[seat];    

                                ShareLianZhuangUiGob.SetActive(true);
   
                                SetShareHeadImg(table.UserDatas[seat].HeadImage, ShareHeadImgLianzhuang,
                                defineArray[table.UserDatas[seat].Sex % 2]);

                                Btns.SetActive(true);
                            }
                    }
                }
                #endregion
            }

            if (UtilData.RoomType == EnRoomType.YuLe)
            {
                _startCall = () =>
                {
                    Hide();
                    EventDispatch.Dispatch((int)NetEventId.OnGameRestart, new EventData());
                };
            }

            if (isMeWin)
            {
                if (Gril != null) Gril.sprite = GrilWin;
                if (Result != null) Result.sprite = WinSpr;
            }
            else
            {
                if (Gril != null) Gril.sprite = GrilLost;
                if (Result != null) Result.sprite = LostSpr;
            }

            if (isLiuJu) //流局
            {
                if (Result != null) Result.sprite = HeSpr;
            }

            if (_laterUnfoldCor!=null)
            {
                StopCoroutine(_laterUnfoldCor);
            }
            _laterUnfoldCor=StartCoroutine(LaterUnfoldingInfo());
        }

        private Coroutine _laterUnfoldCor;
        private IEnumerator LaterUnfoldingInfo()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.1f);
            if (OnerdUisizwCtrl!=null) OnerdUisizwCtrl.UnfoldingInfo();
        }

        /// <summary>
        /// 返回手牌中的赖子数组
        /// </summary>
        /// <param name="hdcds"></param>
        /// <param name="hucd">胡的那张牌</param>
        /// <param name="ishu">是否是胡牌</param>
        /// <param name="laizi">赖子</param>
        /// <returns></returns>
        private int[] CheckLaiziGroup(IEnumerable<int> hdcds,int hucd ,bool ishu,int laizi)
        {
            var laiziCut = hdcds.Count(hdcd => hdcd == laizi);
            if (ishu && hucd == laizi) laiziCut++;//如果自己胡判断胡的那张牌是不是赖子
            if (laiziCut < 1) return null;

            var laiziGroup = new int[laiziCut];
            for (int i = 0; i < laiziCut; i++)
                laiziGroup[i] = laizi;
            return laiziGroup;
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
                    OnWeiChatShareGameResult(new EventData(imageUrl, SharePlat.WxSenceSession));
                });
            #endif
        }

        /// <summary>
        /// 分享图片后显示分享按钮
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShowShareBtnsLater()
        {
            yield return new WaitForSeconds(1f);
            Btns.SetActive(true);
            foreach (var ewgob in ErweiGobs)
            {
                ewgob.SetActive(false);
            }
            CardsInfoZone.transform.localPosition = new Vector3(0,-153,0);
        }


        public void HideShareUi()
        {
            ShareScoreUiGob.SetActive(false);
            ShareLianZhuangUiGob.SetActive(false);
            ShareFlowerUiGob.SetActive(false);
            Btns.SetActive(false);
        }

        #region 分享
        /// <summary>
        /// 分享截图到微信会话
        /// </summary>
        public void OnshareTtResultScreen()
        {
            ShareImage(SharePlat.WxSenceSession);
        }
        /// <summary>
        /// 分享截图到朋友圈
        /// </summary>
        public void OnSharefriends()
        {
            ShareImage(SharePlat.WxSenceTimeLine);
        }

        private Coroutine _shareImageLater;
        /// <summary>
        /// 分享图片到对应平台
        /// </summary>
        /// <param name="plat"></param>
        private void ShareImage(SharePlat plat)
        {
            Btns.SetActive(false);
            foreach (var ewgob in ErweiGobs)
            {
                ewgob.SetActive(true);
            }
            CardsInfoZone.transform.localPosition = new Vector3(0, -200, 0);
            Img.DoScreenShot(new Rect(0, 0, Screen.width, Screen.height), imageUrl =>
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    imageUrl = "file://" + imageUrl;
                }
                if (_shareImageLater != null)
                {
                    StopCoroutine(_shareImageLater);
                }
                _shareImageLater = StartCoroutine(ShowShareBtnsLater());
                OnWeiChatShareGameResult(new EventData(imageUrl, plat));
            });
        }
        /// <summary>
        /// 分享
        /// </summary>
        /// <param name="evn"></param>
        public void OnWeiChatShareGameResult(EventData evn)
        {
            var imageUrl = (string)evn.data1;
            var sharePlat = (SharePlat)evn.data2;

            var weiXin = Facade.Instance<WeChatApi>();
            if (weiXin&&weiXin.InitWechat()&& weiXin.CheckWechatValidity())
            {
                UserController.Instance.GetShareInfo((info) =>
                {
                    info.ImageUrl = imageUrl;
                    info.ShareType = ShareType.Image;
                    Facade.Instance<WeChatApi>().ShareContent(info);
                }, ShareType.Image, sharePlat, imageUrl, App.GameKey);
            }
        }
        #endregion

    }
}
