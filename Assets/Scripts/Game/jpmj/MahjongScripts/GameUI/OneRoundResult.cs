using System;
using System.Collections.Generic;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.ImgPress;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class OneRoundResult : MonoBehaviour
    {
        public Text RoomID;
        public Text Round;
        public Text TimeData;
        public OnRoudResultItem[] Items;
        public Button TotalBtn;
        public CompressImg Img;

        public Image RoundImg;
        public Sprite JuImg;
        public Sprite QuanImg;

        public Image Result;
        public Sprite WinSpr;
        public Sprite LostSpr;
        public Sprite HeSpr;

        public Image Gril;
        public Sprite GrilWin;
        public Sprite GrilLost;
        public Transform BaoArea;
        public GameObject BtGoOn;

        public Button ContiuneButton;
        public Button CloseButton;

        protected DVoidNoParam _startCall;
        protected bool mIsGameOver = false;

        public virtual void SetShowInfo(TableData table, Texture[] defineArray)
        {
            gameObject.SetActive(true);

            if (RoomID != null) RoomID.text = table.RoomInfo.RoomID + "";
            if (RoundImg != null) RoundImg.sprite = table.RoomInfo.GameLoopType == EnGameLoopType.round ? JuImg : QuanImg;
            if (Round != null) Round.text = table.RoomInfo.CurrRound + "/" + table.RoomInfo.MaxRound;
            if (TimeData != null) TimeData.text = DateTime.Now.ToString("yyyy年MM月dd日   HH:mm:ss");
            foreach (OnRoudResultItem item in Items)
            {
                item.SetVisible = false;
                item.Chair = -1;
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
            int huCnt = 1;
            for (int i = 0; i < seatList.Count; i++)
            {
                int seat = seatList[i];
                Items[i].SetVisible = true;
                Items[i].Name = table.UserDatas[seat].name;
                Items[i].UserID = table.UserDatas[seat].id.ToString();
                Items[i].StrHuInfo = table.Result.HuName[seat];
                Items[i].IntHuScore = table.Result.HuGold[seat];
                Items[i].IntGangScore = table.Result.GangGlod[seat];
                Items[i].IntScore = table.Result.Gold[seat];
                Items[i].IsHu = false;
                Items[i].SetPiaoScore = table.Result.PiaoGlod[seat];
                Items[i].IsBanker = seat == table.BankerSeat;
                // 码分
                Items[i].SetNiaoScore = table.Result.NiaoGold[seat];
                //吃碰杠加入
                Items[i].SetCpgCard(table.UserCpg[seat]);
                Items[i].SetHardCard(table.UserHardCard[seat].ToArray(), table.Laizi);
                //设置胡牌信息
                if (huCnt++ <= table.Result.HuSeat.Count)
                {
                    Items[i].IsHu = true;
                    Items[i].SetHardCard(new[] { table.Result.HuCard }, table.Laizi);
                }

                Items[i].SortCardGroup();

                Items[i].HuTypeValue = table.Result.UserHuType[seat];

                int sex = table.UserDatas[seat].Sex;
                sex = sex >= 0 ? sex : 0;
                Items[i].SetHeadImg(table.UserDatas[seat].HeadImage, defineArray[sex % 2]);
            }

            if (UtilData.RoomType == EnRoomType.YuLe)
            {
                _startCall = () =>
                {
                    Hide();
                    EventDispatch.Dispatch((int)NetEventId.OnGameRestart, new EventData());
                };
            }

            bool isMeWin = table.Result.HuSeat.Contains(table.PlayerSeat);

            if (isMeWin)
            {
                if (Gril != null)
                {
                    Gril.sprite = GrilWin;
                    Facade.Instance<MusicManager>().Play("win");
                    Gril.SetNativeSize();
                }
                if (Result != null) Result.sprite = WinSpr;
            }
            else
            {
                if (Gril != null)
                {
                    Gril.sprite = GrilLost;
                    Facade.Instance<MusicManager>().Play("loss");
                    Gril.SetNativeSize();
                }
                if (Result != null) Result.sprite = LostSpr;
            }

            if (table.Result.HuType == MjRequestData.MJReqTypeLastCd) //流局
            {
                if (Result != null)
                {
                    Result.sprite = HeSpr;

                }
            }
        }

        public virtual void SetIsGameOver(bool isGameOver)
        {
            //设置按钮
            if (isGameOver)
            {
                mIsGameOver = true;
                _startCall = null;
            }
            else
            {
                _startCall = () =>
                {
                    Hide();
                    EventDispatch.Dispatch((int)NetEventId.OnGameRestart, new EventData());
                };

                if (TotalBtn != null)
                {
                    TotalBtn.gameObject.SetActive(false);
                    if (BtGoOn != null)
                    {
                        BtGoOn.SetActive(true);
                    }
                }
            }
        }

        public virtual void ShowTotalScoreBtn()
        {
            TotalBtn.gameObject.SetActive(true);
            if (BtGoOn != null)
            {
                BtGoOn.SetActive(false);
            }
            if (ContiuneButton != null)
            {
                ContiuneButton.gameObject.SetActive(false);
            }

            if (CloseButton != null)
            {
                CloseButton.onClick.RemoveAllListeners();
                CloseButton.onClick.AddListener(OnGradeBtnClick);
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);

            foreach (OnRoudResultItem item in Items)
            {
                item.Cleare();
            }
        }

        public void OnShareBtnClick()
        {
#if UNITY_ANDROID || UNITY_IPHONE
            //YxWindowManager.ShowWaitFor();

            Img.DoScreenShot(new Rect(0, 0, Screen.width, Screen.height), imageUrl =>
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    imageUrl = "file://" + imageUrl;
                }

                EventDispatch.Dispatch((int)ShareEventID.OnWeiChatShareGameResult, new EventData(imageUrl));
            });
#endif
        }

        public void OnStartBtnClick()
        {
            if (mIsGameOver)
            {
                App.QuitGame();
                mIsGameOver = false;
            }
            if (_startCall != null)
            {
                _startCall();
            }
        }

        public void OnGradeBtnClick()
        {
            Hide();
            EventDispatch.Dispatch((int)NetEventId.OnTotalResult, new EventData());
        }

        public void OnCloseClick()
        {
            if (UtilData.RoomType == EnRoomType.YuLe)
                App.QuitGame();
            else
                OnStartBtnClick();
        }

        /// <summary>
        /// 直接返回游戏
        /// </summary>
        public void OnReturnBack()
        {
            App.QuitGame();
        }

        /// <summary>
        /// 换房间
        /// </summary>
        public void OnContinue()
        {
            Hide();
            EventDispatch.Dispatch((int)NetEventId.OnContinue);
        }
    }
}
