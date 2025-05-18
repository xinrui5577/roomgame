using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.sanpian.CommonCode;
using Assets.Scripts.Game.sanpian.DataStore;
using Assets.Scripts.Game.sanpian.Gps;
using Assets.Scripts.Game.sanpian.item;
using Assets.Scripts.Game.sanpian.Tool;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Game.sanpian.server
{
    public class SanPianGameManager : YxGameManager
    {
        [HideInInspector]
        public MyPlayerCtrl RealPlayer;  //本机玩家
        [HideInInspector]
        public PlayerCtrl RightPlayer;
        [HideInInspector]
        public PlayerCtrl OppositePlayer;
        [HideInInspector]
        public PlayerCtrl LeftPlayer;
        [HideInInspector]
        public PlayerCtrl[] PlayerArr;

        //        public TalkManager _TalkManager;

        public Clock Clock;//倒计时

        public BigResult GameOver;

        public DismissRoomMgr DismissRoomMgr;

        public UIButtonCtrl UIButtonCtrl;

        public GameObject[] PlayerScript;

        public UIPlayerInfo[] UIPlayerInfos;

        public Transform StartArea;  //开始放牌的地方

        public CardItem cardItem; //牌模型
        [HideInInspector]
        public int TableScore = 0; //本轮的分数

        public List<CardItem> CardsList = new List<CardItem>();//所有牌模
        [HideInInspector]
        public int[] lastCards;// 最后出的牌
        [HideInInspector]
        public int lastV = 0;//最后出的牌值

        public UILabel lastValue;

        public ResultCtrl ResultPanel;

        public int leaveOrder = 0;

        public List<CardItem> XiaoHuiCardList = new List<CardItem>();//待销毁的LIST

        public MoveUserHead MoveUser;

        public bool ChatVioceToggle
        {
            set
            {
                App.GetGameData<SanPianGameData>().isChatVoiceOn = value;
            }
            get { return App.GetGameData<SanPianGameData>().isChatVoiceOn; }
        }

        public SanPianGameData Data
        {
            get
            {
                return App.GetGameData<SanPianGameData>();
            }
        }


        protected override void OnAwake()
        {
            base.OnAwake();
            InitUser();
        }

        private void InitUser()
        {
            RealPlayer = PlayerScript[0].GetComponent<MyPlayerCtrl>();
            RealPlayer.position = 0;
            RealPlayer.UIInfo = UIPlayerInfos[0]; //本机玩家
            RightPlayer = PlayerScript[1].GetComponent<PlayerCtrl>();
            RightPlayer.position = 1;
            RightPlayer.UIInfo = UIPlayerInfos[1];
            OppositePlayer = PlayerScript[2].GetComponent<PlayerCtrl>();
            OppositePlayer.position = 2;
            OppositePlayer.UIInfo = UIPlayerInfos[2];
            LeftPlayer = PlayerScript[3].GetComponent<PlayerCtrl>();
            LeftPlayer.position = 3;
            LeftPlayer.UIInfo = UIPlayerInfos[3];

        }

        public void MyUserJoin(UserInfo info)
        {
            int seat = info.Seat;
            int maxNum = App.GetGameData<SanPianGameData>().MaxPeopleNum;
            YxDebug.Log("本玩家进入游戏，座位号为" + seat);
            RealPlayer.userInfo = info;
            RealPlayer.UpdateUIInfo();
            PlayerArr[seat] = RealPlayer;
            seat = (seat + 1) % maxNum;
            PlayerArr[seat] = RightPlayer;
            seat = (seat + 1) % maxNum;
            PlayerArr[seat] = OppositePlayer;
            seat = (seat + 1) % maxNum;
            PlayerArr[seat] = LeftPlayer;
            RealPlayer.IsOnline = true;
        }


        public void OtherUserJoin(UserInfo info)
        {
            int seat = info.Seat;
            if (PlayerArr[seat] == null)
            {
                return;
            }
            PlayerArr[seat].UIInfo.IsOnLineSprite.SetActive(false);
            PlayerArr[seat].userInfo = info;
            PlayerArr[seat].UpdateUIInfo();
            PlayerArr[seat].IsOnline = true;
        }

        public void RoJoin(ISFSObject param)
        {
            YxDebug.Log("重新加入游戏");
            UIButtonCtrl.RoJoin(param);
            //本家重连拿牌
            var user = param.GetSFSObject(RequestKey.KeyUser);
            UserInfo info = DataParse.instance.GetUserInfo(user);
            bool MySnow = false;
            bool SnowState = false;
            if (param.ContainsKey("snow"))
            {
                MySnow = user.ContainsKey("snow");
                SnowState = true;
            }
            RealPlayer.TeamNum = user.GetInt("teamId");
            RealPlayer.IsOnline = true;
            if (info.pianNum != 0)
            {
                RealPlayer.UIInfo.AddFlowerScore(info.pianNum);
            }
            if (info.cards != null && info.cards.Length > 0)
            {
                RealPlayer.GetCardsFromArr(info.cards, true);
            }
            if (user.ContainsKey("mateCards"))
            {
                RealPlayer.GetCardsFromArr(info.mateCards, false);
            }
            //其他重连拿牌
            ISFSArray users = param.GetSFSArray(RequestKey.KeyUserList);
            foreach (ISFSObject u in users)
            {
                info = DataParse.instance.GetUserInfo(u);
                PlayerArr[info.Seat].CreateCards(info.cardLen);
                PlayerArr[info.Seat].TeamNum = u.GetInt("teamId");
                PlayerArr[info.Seat].UIInfo.AddFlowerScore(info.pianNum);
            }
            DismissRoomMgr.ShowDismissOnRejion(param);
            //谁的回合
            ISFSObject SpeakerData = param.GetSFSObject("speaker");
            int SpeakerSeat;
            if (SpeakerData == null)
            {
                SpeakerSeat = 0;
            }
            else
            {
                SpeakerSeat = SpeakerData.GetInt("curSpeaker");
            }

            if (SpeakerData.ContainsKey("lastOutSeat"))
            {
                lastCards = SpeakerData.GetIntArray("lastOutCards");
                lastV = SpeakerData.GetInt("lastCardsV");
                int LastOutSeat = SpeakerData.GetInt("lastOutSeat");
                //这是算出最后出牌和说话的中间的座位号，显示不出
                int temp = SpeakerSeat - LastOutSeat;
                temp = temp < 0 ? temp + App.GetGameData<SanPianGameData>().MaxPeopleNum : temp;
                if (temp > 1 && temp < 4)
                {
                    for (int i = 1; i < temp; i++)
                    {
                        PlayerArr[(LastOutSeat + i) % App.GetGameData<SanPianGameData>().MaxPeopleNum].UIInfo.PassFont.SetActive(true);
                    }
                }
                List<int> OutList = new List<int>(lastCards);
                SortCardsTool.SortCards(OutList);
                for (int i = 0; i < OutList.Count; i++)
                {
                    CardItem item = Instantiate(cardItem);
                    PlayerArr[LastOutSeat].OutCardItemList.Add(item);
                    item.transform.SetParent(PlayerArr[LastOutSeat].UIInfo.OutCardsArea);
                    item.Value = OutList[i];
                    item.transform.localScale = Vector3.one * 0.5f;
                    item.SetCardDepth(i);
                }
                PlayerArr[LastOutSeat].UIInfo.OutCardsArea.GetComponent<UIGrid>().Reposition();
            }
            else
            {
                lastV = 0;
            }
            if (SpeakerSeat == RealPlayer.userInfo.Seat && !MySnow)
            {
                UIButtonCtrl.ShowOpBt();
            }
            else
            {
                PlayerArr[SpeakerSeat].UIInfo.OutCardsIcon.SetActive(true);
            }

            //分组
            for (int i = 0; i < App.GetGameData<SanPianGameData>().MaxPeopleNum; i++)
            {
                if (PlayerArr[i].userInfo.Seat == RealPlayer.userInfo.Seat)
                {
                    continue;
                }
                if (RealPlayer.TeamNum == PlayerArr[i].TeamNum)
                {
                    PlayerArr[i].UIInfo.FriendIconAni();
                    RealPlayer.MyFriendIndex = i;
                    break;
                }
            }
            //雪判定
            if (SnowState)
            {
                UIButtonCtrl.XuePaiZhong.SetActive(true);
                if (MySnow)
                {
                    UIButtonCtrl.XueObj.SetActive(true);
                }
            }
            //明幺
            MingYao(param);
            //取分
            UpdateScore(param);
            if (param.ContainsKey("curScore"))
            {
                TableScore = param.GetInt("curScore");
                UIButtonCtrl.AddScore(TableScore);
            }

            //取头游
            if (param.ContainsKey("leaveOrder"))
            {
                int[] leaveOrderArr = param.GetIntArray("leaveOrder");
                for (int i = 0; i < leaveOrderArr.Length; i++)
                {
                    PlayerArr[leaveOrderArr[i]].UIInfo.TouYou.spriteName = (i + 1) + "you";
                    PlayerArr[leaveOrderArr[i]].UIInfo.TouYou.gameObject.SetActive(true);
                    leaveOrder++;
                }
            }
        }

        public void OnServerResponse(ISFSObject param)
        {
            CardRequestData reqData = CardRequestData.ParseData(param);

            switch (reqData.type)
            {
                case (int)LandRequestData.GameRequestType.TypeAllocate:
                    YxDebug.Log("开始发牌");
                    UIButtonCtrl.DisableBigRoomId();
                    StartCoroutine(Allocate(reqData));
                    UIButtonCtrl.ChangeJieSanSprite();
                    UIButtonCtrl.WeiChatBt.SetActive(false);
                    App.GetGameData<SanPianGameData>().GameStart = true;
                    App.GetGameData<SanPianGameData>().RoundNum++;
                    break;
                case (int)LandRequestData.GameRequestType.TypeAlly:
                    YxDebug.Log("分组玩家");
                    MakeTeam(param);
                    break;
                case (int)LandRequestData.GameRequestType.TypeSpeaker:
                    YxDebug.Log("玩家" + reqData.seat + "回合");
                    WhoSpeak(reqData);
                    break;
                case (int)LandRequestData.GameRequestType.TypeChuPai:
                    YxDebug.Log("玩家" + reqData.seat + "出牌" + StringTool.CardsToString(reqData.cards));
                    BeforeOutCards(reqData);
                    OutCards(reqData);
                    break;
                case (int)LandRequestData.GameRequestType.TypeBuChu:
                    YxDebug.Log("玩家" + reqData.seat + "不出");
                    PlayerPass(reqData.seat);
                    break;
                case (int)LandRequestData.GameRequestType.UpdateScore:
                    YxDebug.Log("更新积分");
                    UpdateScore(param);
                    break;
                case (int)LandRequestData.GameRequestType.AutoCard:
                    YxDebug.Log("自动打牌");
                    BeforeOutCards(reqData);
                    AutoCard(reqData);
                    break;
                case (int)LandRequestData.GameRequestType.MingYao:
                    YxDebug.Log("亮幺");
                    MingYao(param);
                    break;
                case (int)LandRequestData.GameRequestType.TypeChooseSnow:
                    YxDebug.Log("是否雪");
                    XuePai(reqData.seat);
                    break;
                case (int)LandRequestData.GameRequestType.MateCards:
                    YxDebug.Log("队友手牌");
                    int[] cards = param.GetIntArray(RequestKey.KeyCards);
                    RealPlayer.GetCardsFromArr(cards, false);
                    break;
                case (int)LandRequestData.GameRequestType.TypeResult:
                    YxDebug.Log("结算");
                    StartCoroutine(ShowResult(param));
                    break;
                case (int)LandRequestData.GameRequestType.Snow:
                    bool xue = param.GetBool("snow");
                    ShowSnowResult(xue);
                    YxDebug.Log("雪的结果");
                    break;
                case (int)LandRequestData.GameRequestType.ClearPoker:
                    YxDebug.Log("清除出牌区域");
                    ClearOutCards(param.GetInt(RequestKey.KeySeat));
                    break;
                case (int)LandRequestData.GameRequestType.UpdateCards:
                    YxDebug.Log("强制同步手牌");
                    break;
                case (int)LandRequestData.GameRequestType.ChangeSeat:
                    ChangeSeat(param);
                    YxDebug.Log("换座");
                    break;
                case (int)LandRequestData.GameRequestType.PianScore:
                    OnPianScore(param);
                    break;
                default:
                    YxDebug.LogError("unknow type");
                    break;
            }
        }

        private Action ChangeSeatCallBack;

        private void ChangeSeat(ISFSObject sfsObject)
        {
            LaterSend = true;
            ChangeSeatCallBack = FinishChangeSeat;
            HideAllReadyIcon();
            List<int> turnIndex = new List<int>();
            int maxNum = App.GetGameData<SanPianGameData>().MaxPeopleNum;
            int[] arr = sfsObject.GetIntArray("changeArr");
            int mySeat = RealPlayer.userInfo.Seat;
            PlayerCtrl[] newArr = new PlayerCtrl[PlayerArr.Length];
            newArr[arr[mySeat]] = RealPlayer;
            newArr[arr[mySeat]].position = 0;
            for (int i = 1; i < maxNum; i++)
            {
                int index = 0;
                for (int j = 0; j < maxNum; j++)
                {
                    if ((arr[mySeat] + i) % maxNum == arr[j])
                    {
                        index = j;
                        break;
                    }
                }
                newArr[(arr[mySeat] + i) % maxNum] = PlayerArr[index];
                turnIndex.Add(PlayerArr[index].position);
                turnIndex.Add(i);
                newArr[(arr[mySeat] + i) % maxNum].UIInfo = UIPlayerInfos[i];
                newArr[(arr[mySeat] + i) % maxNum].position = i;
            }

            MoveUser.MoveHead(turnIndex, ChangeSeatCallBack);
            PlayerArr = newArr;
            for (int i = 0; i < PlayerArr.Length; i++)
            {
                PlayerArr[i].userInfo.Seat = i;
            }
        }

        /// <summary>
        /// 片分数
        /// </summary>
        /// <param name="data"></param>
        private void OnPianScore(ISFSObject data)
        {
            var score = data.GetIntArray(Constants.KeyBetScores);
            for (int i = 0; i < score.Length; i++)
            {
                var pianScore = score[i];
                YxDebug.LogError(pianScore);
                PlayerArr[i].UIInfo.AddPianScore(pianScore);
                PlayerArr[i].userInfo.PianScore += pianScore;
                PlayerArr[i].UpdateUIInfo();
            }
        }

        public void FinishChangeSeat()
        {
            for (int i = 0; i < PlayerArr.Length; i++)
            {
                PlayerArr[i].UpdateUIInfo();
            }
            LaterSend = false;
        }

        private void ClearOutCards(int seat)
        {
            List<CardItem> Items = PlayerArr[seat].OutCardItemList;
            int len = Items.Count;
            if (len > 0)
            {
                for (int i = 0; i < len; i++)
                {
                    Destroy(Items[i].gameObject);
                }
                PlayerArr[seat].OutCardItemList.Clear();
            }
        }

        IEnumerator ShowResult(ISFSObject param)
        {
            yield return new WaitForSeconds(2f);
            ResultPanel.gameObject.SetActive(true);
            ResultPanel.ShowResult(param);
            ISFSArray users = param.GetSFSArray("result");
            int len = App.GetGameData<SanPianGameData>().MaxPeopleNum;
            for (int i = 0; i < len; i++)
            {
                ISFSObject user = users.GetSFSObject(i);
                int ttgold = (int)user.GetLong(RequestKey.KeyTotalGold);
                PlayerArr[i].userInfo.PianScore = 0;
                PlayerArr[i].userInfo.Gold = ttgold;
                PlayerArr[i].UpdateUIInfo();
            }
        }

        private void ShowSnowResult(bool xue)
        {
            UIButtonCtrl.XuePaiZhong.SetActive(false);
            UIButtonCtrl.XueObj.SetActive(false);
            UIButtonCtrl.ShowXue(xue);
        }

        private void XuePai(int seat)
        {
            UIButtonCtrl.XuePaiZhong.SetActive(true);
            if (seat == RealPlayer.userInfo.Seat)
            {
                UIButtonCtrl.XueObj.SetActive(true);
            }
        }

        private void MingYao(ISFSObject param)
        {
            ISFSArray yao = param.GetSFSArray("yao");
            for (int i = 0; i < yao.Count; i++)
            {
                if (i == RealPlayer.userInfo.Seat)
                {
                    continue;
                }
                ISFSObject data = yao.GetSFSObject(i);
                int carda = data.GetInt("carda");
                int card4 = data.GetInt("card4");
                if (carda == 0)
                {
                    PlayerArr[i].UIInfo.HideYao();
                }
                else
                {
                    PlayerArr[i].UIInfo.ShowYao(card4, carda);
                }
            }
        }

        private void AutoCard(CardRequestData reqData)
        {
            PlayerArr[reqData.seat].OutCards(reqData.cards);
            if (reqData.seat == RealPlayer.userInfo.Seat)
            {
                UIButtonCtrl.OpBt.SetActive(false);
            }
            if (PlayerArr[reqData.seat].CardItemList.Count == 0)
            {

            }

        }

        //出牌之前处理数据
        void BeforeOutCards(CardRequestData reqData)
        {
            int[] cards = reqData.cards;
            PlayerArr[reqData.seat].CardsMusicPlay(cards);
       
            lastCards = cards;
            int len = cards.Length;
            for (int i = 0; i < len; i++)
            {
                var value = cards[i] % 16;
                if (value == 5)
                {
                    TableScore += 5;
                }
                else if (value == 10)
                {
                    TableScore += 10;
                }
                else if (value == 13)
                {
                    TableScore += 10;
                }
            }
          
            UIButtonCtrl.AddScore(TableScore);
            if (reqData.cardsv > 500000)
            {
                int FlowerScore = reqData.cards.Length / 3;
                PlayerArr[reqData.seat].UIInfo.AddFlowerScore(FlowerScore);
            }
        }

        private void UpdateScore(ISFSObject sfsObject)
        {
            TableScore = 0;
            UIButtonCtrl.ScoreToZero();
            string scoreText = sfsObject.GetUtfString("score");
            string[] stringTemp = scoreText.Split(',');
            if (stringTemp.Length != 4)
            {
                return;
            }
            int[] tempNum = { int.Parse(stringTemp[0]), int.Parse(stringTemp[1]), int.Parse(stringTemp[2]), int.Parse(stringTemp[3]) };
            for (int i = 0; i < tempNum.Length; i++)
            {
                PlayerArr[i].UIInfo.AddGameScore(tempNum[i]);
            }
        }

        private void MakeTeam(ISFSObject sfsObject)
        {
            string sixSeat = sfsObject.GetUtfString("sixSeat");
            string[] stringTemp = sixSeat.Split(',');
            int[] tempNum = new int[] { int.Parse(stringTemp[0]), int.Parse(stringTemp[1]) };
            bool constMate = sfsObject.GetBool("constMate");
            if (constMate)
            {
                int max = App.GetGameData<SanPianGameData>().MaxPeopleNum;
                int myseat = RealPlayer.userInfo.Seat;
                if (myseat == 0 || myseat == 2)
                {
                    RealPlayer.TeamNum = 0;
                }
                else
                {
                    RealPlayer.TeamNum = 1;
                }
                int index = (myseat + 2) % max;
                RealPlayer.MyFriendIndex = index;
                PlayerArr[index].UIInfo.FriendIconAni();
                PlayerArr[index].myFriend = true;
            }
            else
            {
                PlayerArr[tempNum[0]].ShowSix(0);
                PlayerArr[tempNum[1]].ShowSix(2);
                int index;
                if (stringTemp[0] != stringTemp[1])
                {
                    if (RealPlayer.userInfo.Seat == tempNum[0])
                    {
                        RealPlayer.TeamNum = 0;
                        index = tempNum[1];
                    }
                    else if (RealPlayer.userInfo.Seat == tempNum[1])
                    {
                        RealPlayer.TeamNum = 0;
                        index = tempNum[0];
                    }
                    else
                    {
                        RealPlayer.TeamNum = 1;
                        index = 6 - tempNum[0] - tempNum[1] - RealPlayer.userInfo.Seat;
                    }
                }
                else
                {
                    int max = App.GetGameData<SanPianGameData>().MaxPeopleNum;
                    int myseat = RealPlayer.userInfo.Seat;
                    if (myseat == tempNum[0] || myseat == (tempNum[0] + 2) % max)
                    {
                        RealPlayer.TeamNum = 0;
                    }
                    else
                    {
                        RealPlayer.TeamNum = 1;
                    }
                    index = (myseat + 2) % max;
                }
                RealPlayer.MyFriendIndex = index;
                PlayerArr[index].UIInfo.FriendIconAni();
                PlayerArr[index].myFriend = true;
            }
        }

        private void PlayerPass(int seat)
        {
            PlayerArr[seat].UIInfo.PassFont.SetActive(true);
            PlayerArr[seat].PlayEffect("pass", Random.Range(0, 2));
            if (seat == RealPlayer.userInfo.Seat)
            {
                UIButtonCtrl.OpBt.SetActive(false);
            }
        }

        private void WhoSpeak(CardRequestData reqData)
        {
            lastV = reqData.lastv;
            lastValue.text = lastV + "";
            List<CardItem> Items = PlayerArr[reqData.seat].OutCardItemList;
            int len = Items.Count;
            if (len > 0)
            {
                for (int i = 0; i < len; i++)
                {
                    Destroy(Items[i].gameObject);
                }
                PlayerArr[reqData.seat].OutCardItemList.Clear();
            }
            PlayerArr[reqData.seat].UIInfo.PassFont.SetActive(false);
            for (int i = 0; i < App.GetGameData<SanPianGameData>().MaxPeopleNum; i++)
            {
                if (i != RealPlayer.userInfo.Seat)
                {
                    PlayerArr[i].UIInfo.OutCardsIcon.SetActive(false);
                }
            }
            if (reqData.seat == RealPlayer.userInfo.Seat)
            {
                UIButtonCtrl.ShowOpBt();
            }
            else
            {
                PlayerArr[reqData.seat].UIInfo.OutCardsIcon.SetActive(true);
            }
        }

        private void OutCards(CardRequestData reqData)
        {
            if (reqData.seat == RealPlayer.userInfo.Seat)
            {
                UIButtonCtrl.OpBt.SetActive(false);
            }
            else
            {
                PlayerArr[reqData.seat].UIInfo.OutCardsIcon.SetActive(false);
            }
            PlayerArr[reqData.seat].OutCards(reqData.cards);
            if (RealPlayer.IsOut && RealPlayer.MyFriendIndex == reqData.seat)
            {
                RealPlayer.FriendOutCard(reqData.cards);
            }
            if (PlayerArr[reqData.seat].IsPlayerOut() && !PlayerArr[reqData.seat].UIInfo.TouYou.gameObject.activeSelf)
            {

                PlayerArr[reqData.seat].UIInfo.TouYou.spriteName = (leaveOrder++ + 1) + "you";
                PlayerArr[reqData.seat].UIInfo.TouYou.gameObject.SetActive(true);

            }
        }

        public void CreateAllCards()
        {
            return;
            for (int i = 0; i < 108; i++)
            {
                CardItem m_card = (CardItem)Instantiate(cardItem, new Vector3(0.002f * i - 0.1f, 0, 0), Quaternion.identity);
                m_card.transform.SetParent(StartArea);
                m_card.transform.localScale = Vector3.one * 0.6f;
                m_card.SetBackKey();
                CardsList.Add(m_card);
                XiaoHuiCardList.Add(m_card);
            }
        }


        void HideAllReadyIcon()
        {
            for (int i = 0; i < App.GetGameData<SanPianGameData>().MaxPeopleNum; i++)
            {
                PlayerArr[i].UIInfo.ReadyIcon.SetActive(false);
            }
        }

        IEnumerator Allocate(CardRequestData reqData)
        {
            LaterSend = true;
            int Plen = PlayerArr.Length;
            int Clen = reqData.cards.Length;
            for (int i = 0; i < 41; i++)
            {
                Facade.Instance<MusicManager>().Play("k-sends");
                yield return new WaitForSeconds(0.0005f);
                for (int j = 0; j < Plen; j++)
                {
                    if (i == Clen - 1 && !(reqData.asp == j || j == (reqData.asp + 1) % Clen))
                    {
                        continue;
                    }
                    CardItem m_card = (CardItem)Instantiate(cardItem, new Vector3(0.002f * i - 0.1f, 0, 0), Quaternion.identity);
                    m_card.transform.SetParent(StartArea);
                    m_card.transform.localScale = Vector3.one * 0.6f;
                    m_card.SetBackKey();
                    PlayerArr[j].GetCardsOnStart(m_card);
                    XiaoHuiCardList.Add(m_card);

                    //CardsList.RemoveAt(CardsList.Count - 1);
                }

            }
            yield return new WaitForSeconds(1.2f);
            for (int i = 0; i < Plen; i++)
            {
                PlayerArr[i].SetCardValue(reqData.cards);
            }
            yield return new WaitForSeconds(2);
            LaterSend = false;
        }

        public void Reset()
        {
            int len = XiaoHuiCardList.Count;
            for (int i = 0; i < len; i++)
            {
                if (XiaoHuiCardList[i] != null)
                {
                    Destroy(XiaoHuiCardList[i].gameObject);
                }
            }
            CreateAllCards();
            foreach (PlayerCtrl player in PlayerArr)
            {
                player.UIInfo.Reset();
                player.Reset();
            }
            UIButtonCtrl.Reset();
            TableScore = 0;
            UIButtonCtrl.ClickReady();
            UIButtonCtrl.XuePaiZhong.SetActive(false);
            leaveOrder = 0;
        }

        public void OnGameOver(ISFSObject param)
        {
            GameOver.ShowBigResult(param);
        }



        #region GPS 相关

        public void CheckGpsInfo(ISFSObject data)
        {
            int userID = data.GetInt("uid");
            for (int i = 0, max = App.GetGameData<SanPianGameData>().MaxPeopleNum; i < max; i++)
            {
                if (PlayerArr[i].userInfo != null && PlayerArr[i].userInfo.Id.Equals(userID))
                {
                    PlayerArr[i].userInfo.SetGpsData(data);
                }
            }
        }

        #endregion

        public GpsInfosCtrl GpsCtrl;
        private int numberCount;
        /// <summary>
        /// GPS信息
        /// </summary>
        public void OnClickUserHead()
        {
            numberCount = 0;
            if (!GpsCtrl.gameObject.activeSelf)
            {
                GpsCtrl.Show();
                int num = 0;
                List<GpsUser> users = new List<GpsUser>();
                for (int i = RealPlayer.userInfo.Seat, max = PlayerArr.Length;
                    numberCount < max;
                    i = (i + 1) % max, numberCount++)
                {
                    if (i != RealPlayer.userInfo.Seat && PlayerArr[i].IsOnline && PlayerArr[i].userInfo.IsHasGpsInfo)
                    {
                        num++;
                        var userinfo = PlayerArr[i].userInfo;
                        var UIInfo = PlayerArr[i].UIInfo;
                        if (userinfo == null)
                        {
                            continue;
                        }
                        int nextSeat = (i + 1) % max;
                        if (nextSeat.Equals(RealPlayer.userInfo.Seat))
                        {
                            nextSeat = (i + 2) % max;
                        }
                        GpsUser user = new GpsUser(userinfo.IsHasGpsInfo, userinfo.GpsX, userinfo.GpsY, UIInfo.GpsLabel,
                            UIInfo.GpsLine, i, nextSeat);
                        users.Add(user);
                    }
                }
                GpsCtrl.ShowDistince(users.ToArray());
            }
            //    numberCount = 0;
            //    for (int i = RealPlayer.UserSeat, max = Players.Length; numberCount < max; i = (i + 1) % max, numberCount++)
            //    {
            //        if (GpsCtrl.gameObject.activeSelf)
            //        {
            //            var userinfo = Players[i].UserInfo;
            //            if (userinfo == null)
            //            {
            //                continue;
            //            }
            //            Players[i].UserInfoPanel.ShowAddressInfo();
            //        }
            //        else
            //        {
            //            Players[i].UserInfoPanel.DesLabel.gameObject.SetActive(false);
            //        }
            //    }
            //}
            for (int i = 0; i < App.GetGameData<SanPianGameData>().MaxPeopleNum; i++)
            {
                if (PlayerArr[i].IsOnline)
                {
                    UserInfo info = PlayerArr[i].userInfo;
                    PlayerArr[i].UIInfo.ShowAddressInfo(info);
                }
            }
        }

        public void CancelUserGps()
        {
            GpsCtrl.Hide();
            for (int i = RealPlayer.userInfo.Seat, max = App.GetGameData<SanPianGameData>().MaxPeopleNum; numberCount < max; i = (i + 1) % max, numberCount++)
            {
                var desLabel = PlayerArr[i].UIInfo.GpsLabel;
                var line = PlayerArr[i].UIInfo.GpsLine;
                if (desLabel)
                {
                    desLabel.gameObject.SetActive(false);
                }
                if (line)
                {
                    line.SetActive(false);
                }
            }
            for (int i = 0; i < App.GetGameData<SanPianGameData>().MaxPeopleNum; i++)
            {
                PlayerArr[i].UIInfo.AddressLabel.gameObject.SetActive(false);
            }
        }

        //void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.Space))
        //    {
        //        UIButtonCtrl.instance.ChangeJieSanSprite(true);
        //    }

        //}

        IEnumerator FaPaiSound()
        {
            for (int i = 0; i < 50; i++)
            {
                Facade.Instance<MusicManager>().Play("sends");

                yield return new WaitForSeconds(0.05f);
            }
        }

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            CurtainManager.CloseCurtain();

            PlayerArr = new PlayerCtrl[App.GetGameData<SanPianGameData>().MaxPeopleNum];
            CardsList = new List<CardItem>();

            App.GetGameData<SanPianGameData>().IsGameInfo = true;
            //游戏重置
            //初始化用户信息
            var user = gameInfo.GetSFSObject(RequestKey.KeyUser);
            App.GetGameData<SanPianGameData>().IsRoomGame = gameInfo.ContainsKey("rid");
            App.GetGameData<SanPianGameData>().OwnerId =
                gameInfo.ContainsKey("ownerId") ? gameInfo.GetInt("ownerId") : 0;
            if (App.GetGameData<SanPianGameData>().IsRoomGame)
            {
                App.GetGameData<SanPianGameData>().RoomID = gameInfo.GetInt("rid");
                //必须先获取最大局数
                App.GetGameData<SanPianGameData>().MaxRound = gameInfo.GetInt("maxRound");
                App.GetGameData<SanPianGameData>().RoundNum = gameInfo.GetInt("round");
                if (App.GetGameData<SanPianGameData>().RoundNum > 0)
                {
                    UIButtonCtrl.WeiChatBt.SetActive(false);
                    App.GetGameData<SanPianGameData>().GameStart = true;
                }
                else
                {
                    if (user.GetInt("id") != App.GetGameData<SanPianGameData>().OwnerId)
                    {
                        UIButtonCtrl.ChangeJieSanSprite(true);
                    }
                }
            }
     
            UserInfo info = DataParse.instance.GetUserInfo(user);
            MyUserJoin(info);
            ISFSArray users = gameInfo.GetSFSArray(RequestKey.KeyUserList);
            foreach (ISFSObject u in users)
            {
                info = DataParse.instance.GetUserInfo(u);
                OtherUserJoin(info);
            }
            if (gameInfo.GetBool("playing"))
            {
                RoJoin(gameInfo);
            }
            else
            {
                CreateAllCards();
            }
            //初始化房间信息
            //隐藏等待
            YxWindowManager.HideWaitFor();

            if (App.GetGameData<SanPianGameData>().IsRoomGame)
            {
                //ReadyGame();
            }
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
        }

        //消息容器
        Queue<ISFSObject> ResponseQueue = new Queue<ISFSObject>();


        //是否开启消息延迟
        public bool _laterSend = false;

        public bool LaterSend
        {
            get { return _laterSend; }
            set
            {
                _laterSend = value;
                if (!value)
                {
                    while (ResponseQueue.Count > 0 && !LaterSend)
                    {
                        if (!App.GetRServer<SanPianGameServer>().HasGetGameInfo) return;
                        OnServerResponse(ResponseQueue.Dequeue());
                    }

                }

            }
        }

        public override void GameStatus(int status, ISFSObject info)
        {
           
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            if (LaterSend)
            {
                ResponseQueue.Enqueue(response);
            }
            else
            {
                OnServerResponse(response);
            }
        }

        public override void UserOut(int localSeat, ISFSObject responseData)
        {
            base.UserOut(localSeat, responseData);
            PlayerArr[responseData.GetInt("seat")].UIInfo.gameObject.SetActive(false);
        }

        public override void UserIdle(int localSeat, ISFSObject responseData)
        {
            base.UserIdle(localSeat, responseData);
            PlayerArr[responseData.GetInt("seat")].UIInfo.IsOnLineSprite.SetActive(true);
            PlayerArr[responseData.GetInt("seat")].IsOnline = false;
        }

        public override void UserOnLine(int localSeat, ISFSObject responseData)
        {
            base.UserOnLine(localSeat, responseData);
            PlayerArr[responseData.GetInt("seat")].UIInfo.IsOnLineSprite.SetActive(false);
            PlayerArr[responseData.GetInt("seat")].IsOnline = true;

        }

        public void OnAuto()
        {

        }

        //void Update()
        //{
        //    if (Input.GetKeyUp(KeyCode.A))
        //    {
        //        foreach (var player in PlayerArr)
        //        {
        //            player.UIInfo.AddPianScore(1);
        //        }
        //    }
        //    if (Input.GetKeyUp(KeyCode.S))
        //    {
        //        RealPlayer.OldCheckThree();
        //    }

        //}
    }
}
