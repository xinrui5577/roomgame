using System;
using System.Collections;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.nn41
{
    public class NnGameManager : YxGameManager
    {
        public HupWindow HupWindow;
        public GameOverCtrl GameOverCtrl;
        public RoomInfoCtrl RoomInfoCtrl;
        public AntePanel AnteGameObject;
        public MenuCtrl MenuCtrl;
        public TurnCard TurnCard;
        public RobBank RobBank;
        public SettingCtrl SettingCtrl;
        public Ecffect Ecffect;
        public Result Result;
        public Clock Clock;
        public Tip Tip;

        public Transform CoinItem;
        public GameObject Porker;
        public GameObject[] Target;

        public GameObject SetDownBtn;
        public GameObject ReadyBtn;
        public GameObject BeginBtn;
        public UIGrid PaiStateGrid;
        public GameObject BlackBg;
        public GameObject WeiChatInvite;

        private bool _isHaveMe;
        private float _cdTime;
        private bool _isCuoPai;
        private string[] _bankAntes;
        private int _bankSeat;//服务器座位号
        private ISFSArray _tuserData;
        private int _giveCardsIndex;

        //牌区集合
        private readonly List<GameObject> _giveCardsTarget = new List<GameObject>();
        //位置集合
        private readonly List<int> _serverSearInts = new List<int>();


        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            Clear();

            YxWindowManager.HideWaitFor();
            Tip.Close();

            var record = gameInfo.ContainsKey(InteractParameter.Record) ? gameInfo.GetSFSArray(InteractParameter.Record) : null;
            if (record != null)
            {
                Result.AddResult(record);
            }

            DealHupInfo(gameInfo);
        }

        public override void BeginNewGame(ISFSObject sfsObject)
        {
            base.BeginNewGame(sfsObject);
            var gdata = App.GetGameData<NnGameData>();
            Facade.Instance<MusicManager>().Play(GameSounds.GameStart);
            //刷新房间局数的显示
            gdata.CurrentRound++;
            RoomInfoCtrl.GameNumChange(gdata.CurrentRound);

            App.GameData.GStatus = YxEGameStatus.PlayAndConfine;
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
        }

        public override void GameStatus(int status, ISFSObject info)
        {
            switch (App.GameKey)
            {
                case "nn41"://名牌抢庄
                case "nntp":
                    OnJoinGame(status, info);
                    break;
                default:
                    OnNnJoinGame(status, info);
                    break;
            }
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            var gserver = App.GetRServer<NnGameServer>();
            if (gserver.HasGetGameInfo == false) return;

            switch ((GameInteract)type)
            {
                case GameInteract.SendCard:
                    YxDebug.Log("---------------------发牌张牌:---- " + type);
                    OnGiveCards(response);
                    break;
                case GameInteract.RobBanker:
                    YxDebug.Log("---------------------抢夺庄家:---- " + type);
                    OnBeginBanker(response);
                    break;
                case GameInteract.SureRobDouble:
                    YxDebug.Log("---------------------定抢庄注:---- " + type);
                    UserBank(response);
                    break;
                case GameInteract.StartBet:
                    YxDebug.Log("---------------------庄定闲注:---- " + type);
                    OnAnteStart(response);
                    break;
                case GameInteract.BetState:
                    YxDebug.Log("---------------------定闲家注:---- " + type);
                    int seat = response.GetInt(InteractParameter.Seat);
                    int gold = response.GetInt(InteractParameter.Gold);
                    OnUserAnte(seat, gold);
                    break;
                case GameInteract.SendLastCard:
                    YxDebug.Log("---------------------发牌最后牌:---- " + type);

                    OnGive41Card(response);
                    break;
                case GameInteract.ShowHandCard:
                    YxDebug.Log("---------------------亮出手牌:---- " + type);
                    OnUserLiang(response);
                    // 保存到一个数组中，并排序
                    break;
                case GameInteract.ResultState:
                    YxDebug.Log("---------------------结束显分:---- " + type);
                    App.GameData.GStatus = YxEGameStatus.Over;
                    OnGameResult(response);
                    break;
                case GameInteract.GiveReward:
                    YxDebug.Log("---------------------打赏:---- " + type);
                    break;
                case GameInteract.ContrlStartBtn:
                    OnGameBegin(response);
                    break;
                default:
                    YxDebug.Log("---------------------没有:---- " + type);
                    break;
            }
        }

        public override void BeginReady()
        {
            base.BeginReady();
            var gdata = App.GetGameData<NnGameData>();
            if (gdata.CurrentRound == gdata.MaxRound)
            {
                IsNeedReday(false);
            }
            else
            {
                IsNeedReday(true, 5);
            }
        }

        public override void OnChangeRoomClick()
        {
            if (App.GameData.GStatus == YxEGameStatus.PlayAndConfine)
            {
                YxMessageBox.Show("玩完这局再换吧！", 3);
                return;
            }
            Facade.Instance<MusicManager>().Play(GameSounds.ChangeRoom);
            Clear();
            Tip.T("正在更换房间，请稍后");
        }

        public override void UserIdle(int localSeat, ISFSObject responseData)
        {
        }

        public override void UserOnLine(int localSeat, ISFSObject responseData)
        {
//            if (localSeat == 0) App.GetRServer<NnGameServer>().RequestGameInfo();// App.ReBackLogin();
        }

        private void DealHupInfo(ISFSObject data)
        {
            var gdata = App.GetGameData<NnGameData>();
            string hupInfo;
            if (data.ContainsKey(InteractParameter.Hup))
            {
                hupInfo = data.GetUtfString(InteractParameter.Hup);
            }
            else
            {
                return;
            }
            //接收重连解散信息
            if (!string.IsNullOrEmpty(hupInfo))
            {
                long svt = data.ContainsKey(InteractParameter.Svt) ? data.GetLong(InteractParameter.Svt) : 0;
                long hupStart = data.ContainsKey(InteractParameter.HupStart) ? data.GetLong(InteractParameter.HupStart) : 0;
                var time = (int)(gdata.HupTime - (svt - hupStart));
                time = time < 0 ? 0 : time;
                string[] ids = hupInfo.Split(',');
                for (int i = 0; i < ids.Length; i++)
                {
                    foreach (var userInfo in gdata.UserInfoDict)
                    {
                        var info = (NnUserInfo)userInfo.Value;
                        var id = ids[i];
                        if (id.Equals(info.UserId))
                        {
                            //2发起投票 3同意 -1拒绝
                            ISFSObject hupData = new SFSObject();
                            hupData.PutUtfString(InteractParameter.UserName, info.NickM);
                            hupData.PutInt(RequestKey.KeyType, i == 0 ? 2 : 3);
                            hupData.PutInt(InteractParameter.CdTime, time);
                            hupData.PutInt(RequestKey.KeyId, int.Parse(id));
                            App.GetRServer<NnGameServer>().OnHandsUp(hupData);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 点击返回大厅按钮
        /// </summary>
        public void OnClickBackHall()
        {
            App.QuitGame();
        }

        /// <summary>
        /// 是否显示看牌和搓牌的图片
        /// </summary>
        /// <param name="isShow"></param>
        public void PaiOperate(bool isShow)
        {
            if (App.GameKey == "nn41"||App.GameKey== "nntp")
            {
                if (_isCuoPai)
                    if (PaiStateGrid.transform.GetChild(1) != null)
                    {
                        PaiStateGrid.transform.GetChild(1).gameObject.SetActive(isShow);
                    }
                    else if (PaiStateGrid.transform.GetChild(1) != null)
                    {
                        PaiStateGrid.transform.GetChild(1).gameObject.SetActive(false);
                    }
                if (PaiStateGrid.transform.GetChild(2) != null)
                {
                    PaiStateGrid.transform.GetChild(2).gameObject.SetActive(isShow);
                }
            }
            else
            {
                if (_isCuoPai)
                    if (PaiStateGrid.transform.GetChild(0) != null)
                    {
                        PaiStateGrid.transform.GetChild(0).gameObject.SetActive(isShow);

                    }
                    else if (PaiStateGrid.transform.GetChild(0) != null)
                    {
                        PaiStateGrid.transform.GetChild(0).gameObject.SetActive(false);
                    }

                if (PaiStateGrid.transform.GetChild(2) != null)
                {
                    PaiStateGrid.transform.GetChild(2).gameObject.SetActive(isShow);
                }
            }

            PaiStateGrid.repositionNow = true;
        }


        public void NewliangPai(GameObject obj)
        {
            var gdata = App.GetGameData<NnGameData>();
            gdata.Kind = int.Parse(obj.name);
            gdata.GetPlayer<NnPlayerSelf>().OnsetResult(gdata.Kind);

            if (int.Parse(obj.name) == 0)
            {
                BlackBg.SetActive(true);
            }
            PaiOperate(false);
        }

        public void ReLiang()
        {
            var gdata = App.GetGameData<NnGameData>();
            if (!gdata.IsEndCardFlop)
            {
                PaiOperate(false);
            }
            App.GetRServer<NnGameServer>().SendLiang();
        }

        /// <summary>
        /// 玩家开牌
        /// </summary>
        /// <param name="responseData"></param>
        public void OnUserLiang(ISFSObject responseData)
        {
            var gdata = App.GetGameData<NnGameData>();
            var seat = responseData.GetInt(RequestKey.KeySeat);

            gdata.GetPlayer<NnPlayer>(seat, true).SetResult(responseData);
        }

        /// <summary>
        /// 玩家下注的
        /// </summary>
        /// <param name="seat"></param>
        /// <param name="gold"></param>
        public void OnUserAnte(int seat, int gold)
        {
            var gdata = App.GetGameData<NnGameData>();

            if (seat == gdata.SelfSeat)
            {
                AnteGameObject.Hide();
                App.GameData.GStatus = YxEGameStatus.PlayAndConfine;
            }
            gdata.GetPlayer<NnPlayer>(seat, true).SetUserAnte(gold);
        }
        /// <summary>
        /// 游戏结算
        /// </summary>
        /// <param name="responseData"></param>
        public void OnGameResult(ISFSObject responseData)
        {
            var gdata = App.GetGameData<NnGameData>();
            AnteGameObject.Hide();

            RobBank.RobParentCtrl();

            Tip.Close();
            gdata.Kind = 3;
            PaiOperate(false);

            Clock.SetCountDown(0, ShowCards);
            _tuserData = responseData.GetSFSArray("users");
            Tip.T("正在等待结果");
            CancelInvoke("ShowCards");
            Invoke("Fly2Bank", 1f);
        }

        public void OnGameBegin(ISFSObject responseData)
        {
            var seat = responseData.GetInt("seat");
            if (App.GameData.SelfSeat == seat)
            {
                BeginBtnInvalid(true, true);
            }
        }

        /// <summary>
        /// 结算之后的庄家赢了的金币的飞行动画
        /// </summary>
        protected void Fly2Bank()
        {
            var gdata = App.GetGameData<NnGameData>();
            CoinFly.AnimoEnable = true;

            var find = false;

            var gbank = gdata.GetPlayer<NnPlayer>(_bankSeat, true).CoinLabel.transform.position;

            foreach (ISFSObject userData in _tuserData)
            {
                if (userData.GetBool("isBank")) continue;

                var win = userData.GetInt("gold");
                if (win >= 0)
                {
                    gdata.GetPlayer<NnPlayer>(userData.GetInt(RequestKey.KeySeat), true).WinCoin = win;
                    continue;
                }

                var gf = gdata.GetPlayer<NnPlayer>(userData.GetInt(RequestKey.KeySeat), true).CoinLabel.transform.position;
                var gFly = gameObject.AddComponent<CoinFly>();

                gFly.ParenTransform = GameObject.Find("UI Root/Camera/Game").transform;
                gFly.Item = CoinItem;
                gFly.Num = 20;
                gFly.DetaTime = 0.08f;
                gFly.MoveTime = 0.2f;
                gFly.FromTo(gf, gbank, true);
                find = true;
            }
            if (find)
            {
                Invoke("Fly2Win", 0.8f);
                Facade.Instance<MusicManager>().Play(GameSounds.Coin);
            }
            else
            {
                Fly2Win();
            }
        }

        /// <summary>
        /// 除了庄家之外的玩家赢了 结算之后的界面的金币的飞行动画
        /// </summary>
        private void Fly2Win()
        {
            var gdata = App.GetGameData<NnGameData>();

            var find = false;
            var gbank = gdata.GetPlayer<NnPlayer>(_bankSeat, true).CoinLabel.transform.position;

            foreach (ISFSObject userData in _tuserData)
            {
                if (userData.GetBool("isBank")) continue;

                var win = userData.GetInt("gold");
                if (win <= 0) continue;
                var gt = gdata.GetPlayer<NnPlayer>(userData.GetInt(RequestKey.KeySeat), true).CoinLabel.transform.position;
                var gFly = gameObject.AddComponent<CoinFly>();
                gFly.ParenTransform = GameObject.Find("UI Root/Camera/Game").transform;
                gFly.Item = CoinItem;
                gFly.Num = 20;
                gFly.DetaTime = 0.08f;
                gFly.MoveTime = 0.4f;
                gFly.FromTo(gbank, gt, true);
                find = true;
            }
            if (find)
            {
                Facade.Instance<MusicManager>().Play(GameSounds.Coin);
                Invoke("DownTimeResult", 0.6f);
            }
            else
            {
                DownTimeResult();
            }
        }

        private void DownTimeResult()
        {
            var gdata = App.GetGameData<NnGameData>();
            Result.SetResult(_tuserData);
            foreach (ISFSObject userData in _tuserData)
            {
                var serverSeat = userData.GetInt(InteractParameter.Seat);

                gdata.GetPlayer<NnPlayer>(serverSeat, true).Coin = userData.GetLong(InteractParameter.Ttgold);

                gdata.GetPlayer<NnPlayer>(serverSeat, true).ShowResult(userData);
            }
            Invoke("ReGame", 2f);
        }

        protected void ReGame()
        {
            var gdata = App.GetGameData<NnGameData>();
            foreach (var userController in gdata.PlayerList)
            {
                userController.GetComponent<NnPlayer>().ReSetGame();
            }
        }

        /// <summary>
        /// 发最后一张牌
        /// </summary>
        /// <param name="responseData"></param>
        public void OnGive41Card(ISFSObject responseData)
        {
            var gdata = App.GetGameData<NnGameData>();

            AnteGameObject.Hide();

            _userSfsArray = responseData.GetSFSArray(RequestKey.KeyCards);
            var newCards = responseData.GetIntArray(InteractParameter.NewCards);

            const int cardNum = 4;
            var count = _userSfsArray.Count;

            for (var i = 0; i < count; i++)
            {
                var userSfsObject = _userSfsArray.GetSFSObject(i);
                var seat = userSfsObject.GetInt(RequestKey.KeySeat);

                if (seat == gdata.SelfSeat)
                {
                    gdata.NewCard = newCards[i];
                    if (_giveCardsTarget.Count != 0&&gdata.IsEndCardFlop)
                    {
                        PaiOperate(true);
                    }
                    else
                    {
                        PaiStateGrid.repositionNow = true;
                        PaiStateGrid.transform.GetChild(2).gameObject.SetActive(true);
                    }

                }
                gdata.GetPlayer<NnPlayer>(seat, true).UserData = userSfsObject;

                if (userSfsObject.ContainsKey(RequestKey.KeyCards))
                {
                    gdata.GetPlayer<NnPlayer>(seat, true).Cards.Add(newCards[i]);

                    //                    YxDebug.LogError(string.Format("最后一张牌的值{0}", "0x" + newCards[i].ToString("X")));

                    _giveCardsTarget.Add(gdata.GetPlayer<NnPlayer>(seat, true).Target);

                    _serverSearInts.Add(seat);
                }
                var temp = Instantiate(Porker);
                temp.name = "Porker";

                var v = Vector3.zero;
                temp.transform.parent = gdata.GetPlayer<NnPlayer>(seat, true).Target.transform;

                if (seat == gdata.SelfSeat)
                {
                    temp.transform.localScale = new Vector3(1.4f, 1.3f, 0f);
                    v.x = v.x + (cardNum * 114);
                }
                else
                {
                    v.x = v.x + (cardNum * 32);
                    temp.transform.localScale = Vector3.one;
                }
                temp.GetComponent<UISprite>().depth = 3 + _giveCardsIndex + cardNum;
                temp.GetComponent<SpringPosition>().target = v;
                temp.GetComponent<SpringPosition>().enabled = true;

                temp.SetActive(true);
                gdata.GetPlayer<NnPlayer>(seat, true).Porkers.Add(temp);
            }

            Facade.Instance<MusicManager>().Play(GameSounds.Card);
            if (!gdata.IsEndCardFlop)
            {
                Invoke("ReLiang", 5);
            }
            Tip.T("等待玩家翻牌");
            _giveCardsIndex = 0;
        }

        /// <summary>
        /// 服务器发牌.
        /// </summary>
        /// <param name="responseData"></param>
        public void OnGiveCards(ISFSObject responseData)
        {
            _giveCardsTarget.Clear();
            _serverSearInts.Clear();

            _userSfsArray = responseData.GetSFSArray(InteractParameter.Cards);
            _cdTime = Time.time + responseData.GetInt(InteractParameter.Cd);

            GiveCards();
        }

        /// <summary>
        /// 设置发牌数据
        /// </summary>
        private void GiveCards()
        {
            var gdata = App.GetGameData<NnGameData>();
            foreach (var controller in gdata.PlayerList)
            {
                controller.GetComponent<NnPlayer>().ReSetGame();
            }

            _giveCardsIndex = 0;

            for (int i = 0; i < _userSfsArray.Count; i++)
            {
                var userSfsObject = _userSfsArray.GetSFSObject(i);
                SetUserCard(userSfsObject);
            }

            while (_giveCardsIndex < _userSfsArray.Count)
            {
                GiveCardsA();
            }

            if (App.GameKey == "nn41"||App.GameKey== "nntp")
            {
                RobBankStart(); //结束发牌.开始抢庄
            }
            else
            {
                if (_isHaveMe)
                {
                    PaiOperate(true); //显示搓牌和看牌
                }
            }
        }

        /// <summary>
        /// 设置玩家牌
        /// </summary>
        /// <param name="userSfsObject"></param>
        private void SetUserCard(ISFSObject userSfsObject)
        {
            var gdata = App.GetGameData<NnGameData>();
            var seat = userSfsObject.GetInt(RequestKey.KeySeat);

            var user = gdata.GetPlayer<NnPlayer>(seat, true);
            user.UserData = userSfsObject;

            var hasCards = userSfsObject.ContainsKey(RequestKey.KeyCards);

            if (seat == gdata.SelfSeat)
            {
                if (hasCards)
                {
                    App.GameData.GStatus = YxEGameStatus.PlayAndConfine;
                }
            }

            if (!hasCards) return;

            var cards = userSfsObject.GetIntArray(RequestKey.KeyCards);
            if (seat == gdata.SelfSeat)
            {
                _isHaveMe = true;
                gdata.SelfCards = new List<int>(cards);
            }

            foreach (var c in cards)
            {
                user.Cards.Add(c);
            }
            _giveCardsTarget.Add(user.Target);
            _serverSearInts.Add(seat);
        }

        private ISFSArray _userSfsArray;

        public void ShowCards()
        {
            var gdata = App.GetGameData<NnGameData>();
            for (var i = 0; i < _userSfsArray.Count; i++)
            {
                var cardSfsObject = _userSfsArray.GetSFSObject(i);

                gdata.GetPlayer<NnPlayer>(cardSfsObject.GetInt(InteractParameter.Seat), true).SetResult(cardSfsObject);
            }
        }

        public void Ante(GameObject go)
        {
            var gserver = App.GetRServer<NnGameServer>();
            var gdata = App.GetGameData<NnGameData>();

            if (!gdata.IsKaiFang)
            {
                if (Int32.Parse(go.name) < App.GameData.GetPlayerInfo().CoinA)
                {
                    gserver.Ante(Int32.Parse(go.name));
                }
            }
            else
            {
                gserver.Ante(Int32.Parse(go.name));
            }

            AnteGameObject.Hide();
        }

        /// <summary>
        /// 发牌动作
        /// </summary>
        private void GiveCardsA()
        {
            if (_serverSearInts.Count == 0) return;
            var gdata = App.GetGameData<NnGameData>();
            var seat = _serverSearInts[_giveCardsIndex];
            var userCtrl = gdata.GetPlayer<NnPlayer>(seat, true);
            var count = userCtrl.Cards.Count;
            //            YxDebug.Log("玩家【" + seat + "】 cards中有： " + count + "张牌");
            count = count < 6 ? count : 5;
            Facade.Instance<MusicManager>().Play(GameSounds.Card);
            int i = 0;
            for (; i < count; i++)
            {
                var temp = Instantiate(Porker);
                var v = Vector3.zero;
                temp.name = "Porker" + i;
                temp.transform.parent = _giveCardsTarget[_giveCardsIndex].transform;
                var tempSprite = temp.GetComponent<UISprite>();
                if (seat == gdata.SelfSeat)
                {
                    switch (App.GameKey)
                    {
                        case "nn41":
                        case "nntp":
                            tempSprite.spriteName = "0x" + userCtrl.Cards[i].ToString("X");
                            tempSprite.MakePixelPerfect();
                            temp.transform.localScale = new Vector3(1.4f, 1.3f, 0f);
                            v.x = v.x + (i * 114);
                            break;
                        default:
                            temp.transform.localScale = new Vector3(1.4f, 1.3f, 0f);
                            v.x = v.x + (i * 114);
                            break;
                    }
                }
                else
                {
                    temp.transform.localScale = Vector3.one;
                    v.x = v.x + (i * 32);
                }
                tempSprite.depth = 3 + _giveCardsIndex + i;
                var tempSposition = temp.GetComponent<SpringPosition>();
                tempSposition.target = v;
                tempSposition.enabled = true;

                temp.SetActive(true);
                userCtrl.Porkers.Add(temp);
                //                YxDebug.Log(string.Format("牌的个数{0}和排的名字{1}", i, "0x" + userCtrl.Cards[i].ToString("X")));
            }
            //            YxDebug.Log("实际给玩家发 " + i + "张牌");
            _giveCardsIndex++;
        }
        /// <summary>
        /// 开始抢庄
        /// </summary>
        private void RobBankStart()
        {
            var gdata = App.GetGameData<NnGameData>();

            RobBank.RobParentCtrl(true);
            if (gdata.BankLimit != 0)
            {
                RobBank.CtrlRobBtnShow(gdata.GetPlayerInfo().CoinA < gdata.BankLimit);
            }
            Tip.T("开始抢庄", _cdTime);
        }

        public void RobBankSelect(GameObject go)
        {
            int rate = int.Parse(go.name);
            RobBank.RobParentCtrl();
            App.GetRServer<NnGameServer>().RobBank(rate);
        }

        public void OnBeginBanker(ISFSObject data)
        {
            var cd = data.GetInt(InteractParameter.Cd);
            _cdTime = Time.time + cd;

            ReSetStatus();
            RobBankStart();
        }
        /// <summary>
        /// 玩家的抢庄值
        /// </summary>
        /// <param name="data"></param>
        public void UserBank(ISFSObject data)
        {
            var gdata = App.GetGameData<NnGameData>();
            int rate = data.GetInt(InteractParameter.Rate);
            int seat = data.GetInt(RequestKey.KeySeat);

            gdata.GetPlayer<NnPlayer>(seat, true).SetBankAnte(rate);
        }

        public void ReSetBanker(int seat)
        {
            _bankSeat = seat;
            var gdata = App.GetGameData<NnGameData>();
            foreach (var user in gdata.PlayerList)
            {
                var player = (NnPlayer)user;
                player.SetBanker(false);
            }
            gdata.GetPlayer<NnPlayer>(_bankSeat, true).SetBanker(true);

            //            YxDebug.Log("ReSetBanker庄家：" +_bankSeat);
        }

        /// <summary>
        /// 开始下注
        /// </summary>
        /// <param name="responseData"></param>
        public void OnAnteStart(ISFSObject responseData)
        {
            var gdata = App.GetGameData<NnGameData>();

            RobBank.RobParentCtrl();

            if (responseData.ContainsKey(InteractParameter.Bkp))
            {
                _bankSeat = responseData.GetInt(InteractParameter.Bkp);
                YxDebug.Log("庄家坐位：" + _bankSeat);
            }

            var cd = responseData.GetInt(InteractParameter.Cd);
            _cdTime = Time.time + cd;
            var antes = responseData.GetUtfString(InteractParameter.Antes);
            _bankAntes = antes.Split(',');

            foreach (var user in gdata.PlayerList)
            {
                var player = (NnPlayer)user;
                player.SetBanker(false);
            }

            StartCoroutine(responseData.ContainsKey(InteractParameter.Banks) ? ShowRandomBankAni(responseData) : BankShake());
        }

        /// <summary>
        /// 显示庄家的边框的闪动
        /// </summary>
        /// <param name="responseData"></param>
        /// <returns></returns>
        private IEnumerator ShowRandomBankAni(ISFSObject responseData)
        {
            yield return 1;
            var gdata = App.GetGameData<NnGameData>();
            var banks = responseData.GetIntArray(InteractParameter.Banks);
            var wt = new WaitForSeconds(0.1f);
            var t = 5;
            var seat = -1;
            while (t > 0)
            {
                t--;
                if (seat > -1)
                {
                    gdata.GetPlayer<NnPlayer>(seat).SetBankKuang(false);
                }
                seat = gdata.GetLocalSeat(banks[t % banks.Length]);
                gdata.GetPlayer<NnPlayer>(seat).SetBankKuang(true);
                yield return wt;
            }
            foreach (var bank in banks)
            {
                gdata.GetPlayer<NnPlayer>(bank, true).HideBankKuang();
            }
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(BankShake());
        }

        private IEnumerator BankShake()
        {
            var gdata = App.GetGameData<NnGameData>();
            for (var i = 0; i < 5; i++)
            {
                gdata.GetPlayer<NnPlayer>(_bankSeat, true).SetBankKuang(i % 2 == 0);

                //                gdata.PlayerList[_bankSeat].GetComponent<NnPlayer>().SetBankKuang(i % 2 == 0);
                yield return new WaitForSeconds(0.01f);
            }
            OnBeginAnte();
        }

        /// <summary>
        /// 开始下注的时候显示下注的值
        /// </summary>
        public void OnBeginAnte()
        {
            var gdata = App.GetGameData<NnGameData>();

            gdata.GetPlayer<NnPlayer>(_bankSeat, true).SetBanker(true);
            ReSetStatus();
            if (_bankSeat != gdata.SelfSeat)
            {
                if (App.GameData.GStatus == YxEGameStatus.PlayAndConfine)
                {
                    AnteGameObject.SetNewAnte(_bankAntes);
                    Tip.T("请下注", _cdTime);
                }
                else
                {
                    Tip.T("下注中");
                }
                Facade.Instance<MusicManager>().Play(GameSounds.PlayerBet);
            }
            else
            {
                Facade.Instance<MusicManager>().Play(GameSounds.GetBanker);
                Tip.T("正在等待其他玩家下注", _cdTime);
            }
        }

        public void ReadyGame()
        {
            App.GetRServer<NnGameServer>().ReadyGame();
//            IsNeedReday(false);
        }

        public void OnUserReady(int serverSeat)
        {
            var gdata = App.GetGameData<NnGameData>();

            if (serverSeat == gdata.SelfSeat)
            {
                IsNeedReday(false);
            }
            gdata.GetPlayer<NnPlayer>(serverSeat, true).SetReady();
            YxDebug.Log("" + gdata.GetLocalSeat(serverSeat) + "：已经准备");
        }

        public void Clear()
        {
            var gdata = App.GetGameData<NnGameData>();

            YxDebug.Log("======================== 清空房间 =============================");

            CoinFly.AnimoEnable = false;
            RobBank.RobParentCtrl();
            AnteGameObject.Hide();
            Tip.Close();
            Clock.Stop();
            CancelInvoke("Fly2Bank");
            CancelInvoke("DownTimeResult");
            CancelInvoke("ReGame");
            CancelInvoke("GiveCardsA");
            foreach (var user in gdata.PlayerList)
            {
                user.GetComponent<NnPlayer>().ReSetGame();
                user.GetComponent<NnPlayer>().RemoveUser();
            }
            IsNeedReday(false);
            AnteGameObject.Hide();
        }

        public void ReSetStatus()
        {
            var gdata = App.GetGameData<NnGameData>();

            foreach (var user in gdata.PlayerList)
            {
                user.GetComponent<NnPlayer>().ReSetStatus();
            }
        }


        /// <summary>
        /// 返回大厅
        /// </summary>
        public void OnReturnHall()
        {
            Facade.Instance<MusicManager>().Play(GameSounds.ChangeRoom);
            var gdata = App.GetGameData<NnGameData>();

            if (App.GameData.GStatus == YxEGameStatus.PlayAndConfine)
            {
                YxMessageBox.Show("玩完这局再退吧！", 3, "MessageInfo");
                if (SettingCtrl)
                {
                    SettingCtrl.OnClickCloseBtn();
                }

                return;
            }
            YxMessageBox.Show("确定要离开游戏吗？", "", (box, btnName) =>
                {
                    if (btnName == YxMessageBox.BtnLeft)
                    {
                        Clear();
                        App.QuitGame();

                    }
                }, false, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
        }

        /// <summary>
        /// 退出房间 
        /// </summary>
        public void UOut()
        {

            if (App.GameData.GStatus == YxEGameStatus.PlayAndConfine)
            {
                YxMessageBox.Show("正在游戏中稍后再退", 3);
                if (SettingCtrl)
                {
                    SettingCtrl.OnClickCloseBtn();
                }
            }
            else
            {
                YxMessageBox.Show("您确定要换桌吗？亲", "", (box, btnName) =>
                    {
                        if (btnName == YxMessageBox.BtnLeft)
                        {
                            Clear();
                            App.GetRServer<NnGameServer>().ChangeRoom();
                            if (SettingCtrl)
                            {
                                SettingCtrl.OnClickCloseBtn();
                            }


                        }

                    }, false, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
            }

        }

        /// <summary>
        /// 是否需要准备
        /// </summary>
        /// <param name="need"></param>
        /// <param name="countDown">倒计时</param>
        public void IsNeedReday(bool need, float countDown = 0)
        {
            var gdata = App.GetGameData<NnGameData>();

            if (gdata.CurrentRound == 0)
            {
                if (gdata.IsKaiFang)
                {
                    SetDownBtn.SetActive(need);
                }
                else
                {
                    ReadyBtn.SetActive(need);
                }
            }
            else
            {
                ReadyBtn.SetActive(need);
            }
            if (need)
            {
                if (countDown > 0) Tip.T("请准备", Time.time + countDown);
            }
            else
            {
                Tip.Close();
            }
        }

        /// <summary>
        /// 开始游戏按钮的显示与隐藏
        /// </summary>
        /// <param name="isShow"></param>
        /// <param name="isCanClick"></param>
        public void BeginBtnInvalid(bool isShow, bool isCanClick)
        {
            BeginBtn.SetActive(isShow);
            BeginBtn.GetComponent<BoxCollider>().enabled = isCanClick;
            if (isCanClick)
            {
                BeginBtn.GetComponent<UIButton>().enabled = false;
                BeginBtn.GetComponent<UIButton>().enabled = true;
            }
        }

        /// <summary>
        /// 加入游戏
        /// </summary>
        /// <param name="state"></param>
        /// <param name="requestData"></param>
        public void OnJoinGame(int state, ISFSObject requestData)
        {
            YxDebug.Log("===================   进入房间   ========================");
            RobBank.SetRobValue(requestData.GetIntArray(InteractParameter.RobRate));
            _isCuoPai = requestData.GetBool(InteractParameter.IsCuoPai);
            TableShow(requestData);
            switch ((GameNn41States)state)
            {
                case GameNn41States.Init:
                    YxDebug.Log("【进入房间state：" + state + " -----> 玩家自己创建房间");
                    InitEmptyRoom();
                    break;
                case GameNn41States.Ready:
                    YxDebug.Log("【进入房间state：" + state + " -----> 准备");
                    InitReadying();
                    break;
                case GameNn41States.SendFourCard:
                    YxDebug.Log("【进入房间state：" + state + " -----> 4张牌");
                    InitDealing(requestData);
                    break;
                case GameNn41States.SureBanker:
                    YxDebug.Log("【进入房间state：" + state + " -----> 确认庄家");
                    InitGrabingBanker(requestData);
                    break;
                case GameNn41States.Beting:
                    YxDebug.Log("【进入房间state：" + state + " -----> 下注中");
                    InitAnteing(requestData);
                    break;
                case GameNn41States.LookCard:
                    YxDebug.Log("【进入房间state：" + state + " -----> 开牌中");
                    InitShowingPoker(requestData);
                    break;
                case GameNn41States.Result:
                    YxDebug.Log("【进入房间state：" + state + " ----->  计算结果");
                    InitResulting(requestData);
                    break;
                default:
                    YxDebug.Log("【进入房间state：" + state + " -----> 没有执行脚本");
                    break;
            }

        }

        /// <summary>
        /// 初始化0：玩家进入空房间时状态
        /// </summary>
        private void InitEmptyRoom()
        {
            var gdata = App.GetGameData<NnGameData>();

            _serverSearInts.Clear();
            _giveCardsTarget.Clear();
            if (gdata.GetPlayerInfo<NnUserInfo>().State)
            {
                IsNeedReday(false);
            }
            else
            {
                IsNeedReday(true, 5);
            }

        }

        /// <summary>
        /// 初始化1：所有玩家准备中状态
        /// </summary>
        private void InitReadying()
        {
            _serverSearInts.Clear();
            _giveCardsTarget.Clear();
            IsNeedReday(true, 5);
        }

        /// <summary>
        /// 初始化2：发牌中状态
        /// </summary>
        private void InitDealing(ISFSObject requestData)
        {
            var users = requestData.GetSFSArray(RequestKey.KeyUserList);
            var me = requestData.GetSFSObject(RequestKey.KeyUser);
            _userSfsArray = users;

            _serverSearInts.Clear();
            _giveCardsTarget.Clear();

            _giveCardsIndex = 0;

            SetUserCard(me);

            GiveCardsA();
            var count = _userSfsArray.Count;
            for (var i = 0; i < count; i++)
            {
                var userSfsObject = _userSfsArray.GetSFSObject(i);

                SetUserCard(userSfsObject);

                GiveCardsA();
            }

            IsNeedReday(false);
        }

        /// <summary>
        /// 初始化3：抢庄中状态
        /// </summary>
        private void InitGrabingBanker(ISFSObject requestData)
        {
            RobBank.RobParentCtrl(true);
            var users = requestData.GetSFSArray(RequestKey.KeyUserList);
            var me = requestData.GetSFSObject(RequestKey.KeyUser);
            _userSfsArray = users;

            _serverSearInts.Clear();
            _giveCardsTarget.Clear();

            if (App.GameKey == "nn41"||App.GameKey== "nntp")
            {
                _giveCardsIndex = 0;
                SetUserCard(me);
                GiveCardsA();
            }
            var count = _userSfsArray.Count;
            for (var i = 0; i < count; i++)
            {
                var userSfsObject = _userSfsArray.GetSFSObject(i);

                if (App.GameKey == "nn41"||App.GameKey== "nntp")
                {
                    SetUserCard(userSfsObject);
                    GiveCardsA();
                }
            }
            IsNeedReday(false);

            if (!requestData.ContainsKey(InteractParameter.Bkp)) return;
            var bankerSeat = requestData.GetInt(InteractParameter.Bkp);
            ReSetBanker(bankerSeat); //设置庄家位置 
        }

        /// <summary>
        /// 初始化4：下注中状态
        /// </summary>
        private void InitAnteing(ISFSObject requestData)
        {
            var users = requestData.GetSFSArray(RequestKey.KeyUserList);
            var me = requestData.GetSFSObject(RequestKey.KeyUser);

            var bankerSeat = requestData.GetInt(InteractParameter.Bkp);

            _userSfsArray = users;
            _serverSearInts.Clear();
            _giveCardsTarget.Clear();

            if (App.GameKey == "nn41"||App.GameKey== "nntp")
            {
                _giveCardsIndex = 0;
                SetUserCard(me);
                GiveCardsA();
            }
            var count = _userSfsArray.Count;
            for (var i = 0; i < count; i++)
            {
                var userSfsObject = _userSfsArray.GetSFSObject(i);

                if (App.GameKey == "nn41"||App.GameKey== "nntp")
                {
                    SetUserCard(userSfsObject);
                    GiveCardsA();
                }
            }
            IsNeedReday(false);
            ReSetBanker(bankerSeat); //设置庄家位置 
        }

        /// <summary>
        /// 初始化5：开牌中状态
        /// </summary>
        private void InitShowingPoker(ISFSObject requestData)
        {
            var users = requestData.GetSFSArray(RequestKey.KeyUserList);
            var me = requestData.GetSFSObject(RequestKey.KeyUser);

            var bankerSeat = requestData.GetInt(InteractParameter.Bkp);

            _userSfsArray = users;
            _serverSearInts.Clear();
            _giveCardsTarget.Clear();

            _giveCardsIndex = 0;
            SetUserCard(me);
            GiveCardsA();

            var count = _userSfsArray.Count;
            for (var i = 0; i < count; i++)
            {
                var userSfsObject = _userSfsArray.GetSFSObject(i);

                SetUserCard(userSfsObject);
                GiveCardsA();
                OnUserLiang(userSfsObject);
            }
            IsNeedReday(false);
            ReSetBanker(bankerSeat); //设置庄家位置  

            if (App.GameData.GStatus != YxEGameStatus.PlayAndConfine) return;

            OnUserLiang(me);
            ReLiang();
        }

        /// <summary>
        /// 初始化8：开牌中状态
        /// </summary>
        private void InitResulting(ISFSObject requestData)
        {
            InitShowingPoker(requestData);
        }

        public void BeginGame()
        {
            App.GetRServer<NnGameServer>().BeginGame();
            MenuCtrl.NormalShow();
            WeiChatInvite.gameObject.SetActive(false);
            BeginBtn.SetActive(false);
        }

        /// <summary>
        /// 加入游戏
        /// </summary>
        /// <param name="status"></param>
        /// <param name="requestData"></param>
        public void OnNnJoinGame(int status, ISFSObject requestData)
        {
            RobBank.SetRobValue(requestData.GetIntArray(InteractParameter.RobRate));
            _isCuoPai = requestData.GetBool(InteractParameter.IsCuoPai);
            YxDebug.Log("===================   进入房间   ========================");
            TableShow(requestData);
            var state = (GameNnStates)requestData.GetInt(InteractParameter.State);
            switch (state)
            {
                case GameNnStates.Init: //玩家自己创建房间  
                    YxDebug.Log("【进入房间state：" + state + " -----> 玩家自己创建房间");
                    InitEmptyRoom();
                    break;
                case GameNnStates.Waiting: //准备
                    YxDebug.Log("【进入房间state：" + state + " -----> 准备");
                    InitReadying();
                    break;
                case GameNnStates.RobBank: //抢庄
                    YxDebug.Log("【进入房间state：" + state + " -----> 抢庄");
                    InitGrabingBanker(requestData);
                    break;
                case GameNnStates.Ante: //下注中
                    YxDebug.Log("【进入房间state：" + state + " -----> 下注中");
                    InitAnteing(requestData);
                    break;
                case GameNnStates.ViewCard: //开牌中
                    YxDebug.Log("【进入房间state：" + state + " -----> 开牌中");
                    InitShowingPoker(requestData);
                    break;
                case GameNnStates.GameOver: //游戏结束
                    YxDebug.Log("【进入房间state：" + state + " ----->  计算结果");
                    InitResulting(requestData);
                    break;
                default:
                    YxDebug.Log("【进入房间state：" + state + " -----> 没有执行脚本");
                    break;
            }
        }

        /// <summary>
        /// 桌面显示处理
        /// </summary>
        /// <param name="requestData"></param>
        public void TableShow(ISFSObject requestData)
        {
            var gdata = App.GetGameData<NnGameData>();

            var bankType = "";
            var infoList = new string[] { };
            var banktypeStr = requestData.ContainsKey(InteractParameter.BanktypeStr)
                                  ? requestData.GetUtfString(InteractParameter.BanktypeStr)
                                  : "";

            if (banktypeStr != "") bankType = banktypeStr.Split(':')[1];

            gdata.Rule = requestData.ContainsKey(InteractParameter.Rule)
                             ? requestData.GetUtfString(InteractParameter.Rule)
                             : "";

            if (gdata.Rule != "") infoList = gdata.Rule.Split(';');

            RoomInfoCtrl.InitRoomInfo(gdata.RoomType, infoList, bankType);

            if (gdata.CurrentRound == 0)
            {
                if (gdata.IsKaiFang)
                {
                    if (SettingCtrl)
                    {
                        SettingCtrl.HideBtn();
                    }
                    if (gdata.SelfSeat == 0)
                    {
                        if (BeginBtn != null)
                        {
                            BeginBtnInvalid(true, false);
                        }
                        WeiChatInvite.gameObject.SetActive(true);
                        MenuCtrl.CreatRoomShow();
                    }
                    else
                    {
                        MenuCtrl.CreatRoomOtherShow();
                    }
                }
                else
                {
                    MenuCtrl.SpecialShow();
                }
            }
            else
            {

                if (gdata.IsKaiFang)
                {
                    MenuCtrl.NormalShow();
                }
                else
                {
                    MenuCtrl.SpecialShow();
                }
            }
        }

    }
}
