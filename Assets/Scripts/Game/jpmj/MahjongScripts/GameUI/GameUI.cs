using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.jpmj.MahjongScripts.GameUI.GPS;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using System.Collections;
using Assets.Scripts.Game.jpmj.MahjongScripts.GameUI.QueryHu;
using Assets.Scripts.Game.jpmj.MahjongScripts.Helper;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public.Adpater;
using Sfs2X.Entities.Data;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class GameUI : MonoBehaviour
    {
        public PlayersPnl PlayersPnl;
        public OpreateMenu OpreateMenu;
        //public BiaoQIngControl BiaoQing;
        protected virtual OpreateMenu GetOpreateMenu()
        {
            if (OpreateMenu == null)
            {
                GameObject go = InstantiateAsset("OpreateMenuPnl");
                OpreateMenu = go.GetComponent<OpreateMenu>();
                ResetGameObjPos(go);
            }
            return OpreateMenu;
        }
        public OneRoundResult OneRoundResult;
        protected virtual OneRoundResult GetOneRoundResult()
        {
            if (OneRoundResult == null)
            {
                GameObject go = InstantiateAsset("OneRoundResult");
                OneRoundResult = go.GetComponent<OneRoundResult>();
                ResetGameObjPos(go, OtherGameUi);
            }
            return OneRoundResult;
        }
        public TtResultRecord TotalResult;
        public GameInfoPnl GameInfoPnl;

        protected virtual GameInfoPnl GetGameInfoPnl()
        {
            if (GameInfoPnl == null)
            {
                GameObject go = InstantiateAsset("GameInfoPnl");
                GameInfoPnl = go.GetComponent<GameInfoPnl>();
                ResetGameObjPos(go);
            }
            return GameInfoPnl;
        }

        public ChooseCgPnl ChooseCgPnl;

        protected virtual ChooseCgPnl GetChooseCgPnl()
        {
            if (ChooseCgPnl == null)
            {
                GameObject go = InstantiateAsset("ChooseCgPnl");
                ChooseCgPnl = go.GetComponent<ChooseCgPnl>();
                ResetGameObjPos(go);
            }
            return ChooseCgPnl;
        }
        public GameSettingPnl SettingPnl;
        protected virtual GameSettingPnl GetGameSettingPnl()
        {
            if (SettingPnl == null)
            {
                GameObject go = InstantiateAsset("SettingPnl");
                SettingPnl = go.GetComponent<GameSettingPnl>();
                ResetGameObjPos(go);
            }
            return SettingPnl;
        }

        public HandUpPnl HandUpPnl;
        protected virtual HandUpPnl GetHandUp()
        {
            if (HandUpPnl == null)
            {
                GameObject go = InstantiateAsset("HandUpPnl");
                HandUpPnl = go.GetComponent<HandUpPnl>();
                ResetGameObjPos(go);
            }
            return HandUpPnl;
        }

        public UserDetail UserDetail;
        public ChatPnl ChatPnl;
        protected virtual ChatPnl GetChatPnl()
        {
            if (ChatPnl == null)
            {
                GameObject go = InstantiateAsset("ChatPnl");
                ChatPnl = go.GetComponent<ChatPnl>();
                ResetGameObjPos(go);
            }
            return ChatPnl;
        }

        public TalkPnl TalkPnl;
        public GameBtnPnl GameBtnPnl;
        public JiaPiaoPnl JiaPiaoPnl;
        public QueryHuPnl TingInfoPnl;
        public GpsInfosCtrl GpsInfosCtrl;

        public GameObject HuanBao;

        public Transform OtherGameUi;

        public Texture DefHeadMan;
        public Texture DefHeadWoman;

        protected Texture[] _headTextureArray = new Texture[UtilDef.GamePlayerCnt];
        // Use this for initialization
        protected int _currChair;
        protected int _oldChair;

        protected void Awake()
        {
            RegistEvent();
            Init();
        }

        protected virtual void Init() { }

        protected virtual void RegistEvent()
        {
            EventDispatch.Instance.RegisteEvent((int)UIEventId.GameStatus, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.UserJionIn, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.UserOutLine, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.UserOutRoom, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.UserReady, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.UserGlodChange, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.OperationMenu, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.ChangeCurrUser, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.Banker, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.MahjongCnt, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.Result, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.CgChoose, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.Cpg, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.Hu, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.FanPai, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.GameStart, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.HandUp, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.CurrRound, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.UserTalk, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.OnGetSoundKey, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.DownLoadVoice, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.IsGameOver, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.GameOverShowOneRoundBtn, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.ChooseTing, OnRecvEvent);

            EventDispatch.Instance.RegisteEvent((int)UIEventId.ShowSettintPnl, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.ShowChatPnl, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.ShowTalkPnl, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.ShowUserDetail, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.ShowTotalResult, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.ShowUserSpeak, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.StopUserSpeak, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.RoomInfo, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.ShowChooseXJFD, OnRecvEvent);

            EventDispatch.Instance.RegisteEvent((int)UIEventId.ShowHuanBaoEffect, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.ShowJiaPiaoBtn, OnJiaPiaoDispose);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.ShowJiaPiaoEffect, OnJiaPiaoDispose);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.HideJiaPiaoEffect, OnJiaPiaoDispose);

            EventDispatch.Instance.RegisteEvent((int)GameEventId.Ting, OnTingPai);

            EventDispatch.Instance.RegisteEvent((int)UIEventId.ShowQueryHulistPnl, OnShowHulist);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.HideQueryHulistPnl, OnHideHulist);

            EventDispatch.Instance.RegisteEvent((int)UIEventId.ShowThrowEffectOnYoujin, ShowThrowEffectOnYoujin);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.ShowFinalRoundWarning, OnFinalRound);

            //GPS相关
            EventDispatch.Instance.RegisteEvent((int)UIEventId.ReceiveGPSInfo, OnReceiveGpsInfo);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.GetIpAndCountry, GetIpAndCountry);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.ShowGPSInfo, OnShowGPSInfo);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.UsersGlodChangeWithWordart, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)UIEventId.OnContinueGame, OnRecvEvent);
        }

        protected virtual void Start()
        {
            if (null != GameAdpaterManager.Singleton)
            {
                transform.FindChild("Camera").GetComponent<Camera>().fieldOfView = GameAdpaterManager.Singleton.GetConfig.UICamera_FieldOfView;
            }
            OtherGameUi = GameObject.Find("GameUiOther").transform;
        }


        //delegate 回调
        //----------------------------------------
        protected virtual void OnRecvEvent(int eventId, EventData evn)
        {
            switch ((UIEventId)eventId)
            {
                case UIEventId.GameStatus:
                    OnGameStatus(evn);
                    break;
                case UIEventId.UserJionIn:
                    OnUserJionIn(evn);
                    break;
                case UIEventId.UserOutLine:
                    OnUserOutLine(evn);
                    break;
                case UIEventId.UserOutRoom:
                    OnUserOutRoom(evn);
                    break;
                case UIEventId.UserReady:
                    OnUserReady(evn);
                    break;
                case UIEventId.UserGlodChange:
                    OnUserGlodChange(evn);
                    break;
                case UIEventId.OperationMenu:
                    OnOperationMenu(evn);
                    break;
                case UIEventId.ChangeCurrUser:
                    ChangeCurrUser(evn);
                    break;
                case UIEventId.Banker:
                    SetBanker(evn);
                    break;
                case UIEventId.Result:
                    OnResult(evn);
                    break;
                case UIEventId.MahjongCnt:
                    OnMahjongCnt(evn);
                    break;
                case UIEventId.CgChoose:
                    OnCgChoose(evn);
                    break;
                case UIEventId.Cpg:
                    OnCpg(evn);
                    break;
                case UIEventId.Hu:
                    OnHu(evn);
                    break;
                case UIEventId.FanPai:
                    OnFanPai(evn);
                    break;
                case UIEventId.GameStart:
                    OnGameStart();
                    break;
                case UIEventId.ShowSettintPnl:
                    GetGameSettingPnl().Show();
                    break;
                case UIEventId.HandUp:
                    OnHandUp(evn);
                    break;
                case UIEventId.ShowUserDetail:
                    OnUserDetail(evn);
                    break;
                case UIEventId.ShowTotalResult:
                    OnShowTotalResult(evn);
                    break;
                case UIEventId.CurrRound:
                    OnCurrRound(evn);
                    break;
                case UIEventId.ShowChatPnl:
                    OnShowChatPnl();
                    break;
                case UIEventId.UserTalk:
                    OnUserTalk(evn);
                    break;
                case UIEventId.ShowUserSpeak:
                    OnShowUserSpeak(evn);
                    break;
                case UIEventId.StopUserSpeak:
                    OnStopUserSpeak(evn);
                    break;
                case UIEventId.OnGetSoundKey:
                    OnGetSoundKey(evn);
                    break;
                case UIEventId.DownLoadVoice:
                    OnDownLoadVoice(evn);
                    break;
                case UIEventId.RoomInfo:
                    OnRoomInfo(evn);
                    break;
                case UIEventId.IsGameOver:
                    OnIsGameOver(evn);
                    break;
                case UIEventId.GameOverShowOneRoundBtn:
                    OnShowOneRoundBtn(evn);
                    break;
                case UIEventId.ChooseTing:
                    OnChooseTing(evn);
                    break;
                case UIEventId.ShowChooseXJFD:
                    ShowChooseXJFD(evn);
                    break;
                case UIEventId.ShowHuanBaoEffect:
                    ShowHuanBaoEffect(evn);
                    break;
                case UIEventId.UsersGlodChangeWithWordart:
                    OnUserGlodChangeWithWordart(evn);
                    break;
                case UIEventId.OnContinueGame:
                    OnContinueGame(evn);
                    break;
                default:
                    OnGameUIEvent((UIEventId)eventId, evn);
                    break;
            }
        }

        protected virtual void OnContinueGame(EventData evn)
        {
            TotalResult.gameObject.SetActive(false);
            PlayersPnl.ResetInfo();
            PlayersPnl.PlayerOther.ResetReadyState();
        }

        private void ShowHuanBaoEffect(EventData evn)
        {
            HuanBao.SetActive(true);
            HuanBao.GetComponent<ParticleSystem>().Play();
        }

        public void ResetGameObjPos(GameObject go, Transform parent = null)
        {
            if (parent == null)
                go.transform.parent = transform;
            else
                go.transform.parent = parent;
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            RectTransform rect = go.transform.GetComponent<RectTransform>();
            rect.offsetMax = Vector2.zero;
            rect.offsetMin = Vector2.zero;
        }

        protected virtual void ShowChooseXJFD(EventData evn)
        {

        }

        protected virtual void OnGameUIEvent(UIEventId id, EventData evn)
        {
            YxDebug.Log("game ui 没有处理的事件 " + id);
        }

        protected virtual void OnGameStatus(EventData evn)
        {
            EnGameStatus status = (EnGameStatus)evn.data1;
            TableData data = (TableData)evn.data2;
            //根据游戏不同的状态来更新UI
            switch (status)
            {
                case EnGameStatus.GameFree:
                    OnGameFree(data);
                    break;
                case EnGameStatus.GameReady:
                    break;
                case EnGameStatus.GameSendCard:
                    OnGameSendCard(data);
                    GameBtnPnl.SetSharBtnActive(false);
                    break;
                case EnGameStatus.GamePlay:
                    OnGamePlay(data);
                    GameBtnPnl.SetSharBtnActive(false);
                    PlayersPnl.HidePlayersReady();
                    break;
                case EnGameStatus.GameEnd:
                    GameBtnPnl.SetSharBtnActive(false);
                    break;
            }
        }

        protected virtual void OnUserJionIn(EventData evn)
        {
            int Chair = (int)evn.data1;
            UserData data = (UserData)evn.data2;
            PlayersPnl.SetUserInfo(Chair, data);
            //设置头像
            if (data.Sex == 0)
                PlayersPnl.SetUserHead(Chair, data.HeadImage, DefHeadWoman);
            else
                PlayersPnl.SetUserHead(Chair, data.HeadImage, DefHeadMan);
        }

        //短线
        protected virtual void OnUserOutLine(EventData evn)
        {
            int Chair = (int)evn.data1;
            bool value = evn.data2 == null || (bool)evn.data2;
            PlayersPnl.SetOutLine(Chair, value);
        }
        //准备
        protected virtual void OnUserReady(EventData evn)
        {
            int Chair = (int)evn.data1;
            bool value = evn.data2 == null || (bool)evn.data2;
            PlayersPnl.SetReady(Chair, value);
        }

        protected virtual void OnUserGlodChange(EventData evn)
        {
            int[] glod = (int[])evn.data1;
            for (int i = 0; i < glod.Length; i++)
            {
                //PlayerOther.SetUserAddGold(i, glod[i]);
                PlayersPnl.AddGold(i, glod[i]);
            }
        }





        protected GameObject InstantiateAsset(string name)
        {
            GameObject asset = ResourceManager.LoadAsset(name, name);
            GameObject go = null;

            if (null != asset)
            {
                go = Instantiate(asset);
                go.name = name;
            }
            return go;
        }

        //使用艺术字加动画表现加减分
        protected virtual void OnUserGlodChangeWithWordart(EventData evn)
        {
            int[] glod = (int[])evn.data1;
            for (int i = 0; i < glod.Length; i++)
            {
                PlayersPnl.AddGoldWithWordart(i, glod[i]);
            }
        }

        protected virtual void OnOperationMenu(EventData evn)
        {
            if (evn.data1 == null)
            {
                GetOpreateMenu().Reset();
                GetChooseCgPnl().Reset();
            }
            else
            {
                SwitchCombination switchC = (SwitchCombination)evn.data1;
                GetOpreateMenu().ShowMenu(switchC);
            }
        }

        protected virtual void ChangeCurrUser(EventData evn)
        {
            _currChair = (int)evn.data1;
            _oldChair = (int)evn.data2;

            PlayersPnl.SetCurr(_currChair);

            if (_oldChair != _currChair)
            {
                PlayersPnl.SetNotCurr(_oldChair);
            }
        }

        protected virtual void SetBanker(EventData evn)
        {
            var banker = (int)evn.data1;
            PlayersPnl.SetBanker(banker);
        }

        protected virtual void OnResult(EventData evn)
        {
            var table = (TableData)evn.data1;
            GetOneRoundResult().SetShowInfo(table, new[] { DefHeadWoman, DefHeadMan });
        }

        protected virtual void OnMahjongCnt(EventData evn)
        {
            var mahjongCnt = (int)evn.data1;
            GetGameInfoPnl().SetMahjongCnt(mahjongCnt);
        }

        protected virtual void OnCgChoose(EventData evn)
        {
            var list = (List<int[]>)evn.data1;
            var call = (DVoidInt)evn.data2;
            var outPutCard = (int)evn.data3;
            GetChooseCgPnl().SetChooseCg(list, call, outPutCard);
            GetChooseCgPnl().CancelCall = () => GetOpreateMenu().ShowMenuBg();
            //隐藏操作菜单
            GetOpreateMenu().HideMenu();
        }

        protected virtual void OnCpg(EventData evn)
        {
            var data = (CpgData)evn.data1;
            //显示吃碰杠特效
            EnCpgEffect effect = EnCpgEffect.none;
            if (data.Type == EnGroupType.Chi)
                effect = EnCpgEffect.chi;
            else if (data.Type == EnGroupType.Peng)
                effect = EnCpgEffect.peng;
            else if (data.Type >= EnGroupType.WZhuaGang)
                effect = EnCpgEffect.gang;

            PlayersPnl.PlayEffect(_currChair, effect);
        }

        protected virtual void OnHu(EventData evn)
        {
            GameResult data = (GameResult)evn.data1;
            //显示吃碰杠特效
            EnCpgEffect effect = EnCpgEffect.none;
            if (data.HuType == MjRequestData.MJRequestTypeHu)
                effect = EnCpgEffect.hu;
            else if (data.HuType == MjRequestData.MJReqTypeZiMo)
                effect = EnCpgEffect.zimo;

            //游金胡特效
            switch (data.QzmjHuType)
            {
                case QzmjHuType.youjin: effect = EnCpgEffect.youjin; break;
                case QzmjHuType.shuangyou: effect = EnCpgEffect.shuangyou; break;
                case QzmjHuType.sanyou: effect = EnCpgEffect.sanyou; break;
                case QzmjHuType.sanjindao: effect = EnCpgEffect.sanjindao; break;
            }

            foreach (int i in data.HuSeat)
            {
                var chair = UtilFunc.GetChairId(i);
                PlayersPnl.PlayEffect(chair, effect);
            }

            GetOpreateMenu().Reset();
            if (TrusteeshipHelper.Instance != null)
            {
                //取消托管
                TrusteeshipHelper.Instance.OnDisableTrusteeshipClick();
            }
        }

        protected virtual void OnFanPai(EventData evn)
        {
            int fanpai = (int)evn.data1;

            if (fanpai != UtilDef.NullMj)
            {
                GetGameInfoPnl().SetFanPai(fanpai);
            }
        }

        protected virtual void OnGameStart()
        {
            GetGameInfoPnl().SetStartGame();
        }

        protected virtual void OnHandUp(EventData evn)
        {
            EnDismissFeedBack feedBack = (EnDismissFeedBack)evn.data1;
            string userName = (string)evn.data2;
            if (feedBack == EnDismissFeedBack.ApplyFor)
            {
                string[] allUserName = (string[])evn.data3;
                if (evn.data4 != null)
                    GetHandUp().DismissApplyeFor(userName, allUserName, (int)evn.data4);
                else
                    GetHandUp().DismissApplyeFor(userName, allUserName);
            }
            else
            {
                GetHandUp().DismissFeedBack(userName, feedBack);
            }
        }
        protected virtual void OnUserDetail(EventData evn)
        {
            UserData data = (UserData)evn.data1;
            UserDetail.Show();
            UserDetail.SetShow(data);
            //设置头像
            if (data.Sex == 0)
                UserDetail.SetHead(data.HeadImage, DefHeadWoman);
            else
                UserDetail.SetHead(data.HeadImage, DefHeadMan);
        }

        protected virtual void OnShowTotalResult(EventData evn)
        {
            TableData data = (TableData)evn.data1;
            if (TotalResult == null)
            {
                float timer = Time.realtimeSinceStartup;
                GameObject go = InstantiateAsset("TtResultRecordPnl");
                TotalResult = go.GetComponent<TtResultRecord>();
                go.transform.parent = transform;
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;
                RectTransform rect = go.transform.GetComponent<RectTransform>();
                rect.offsetMax = Vector2.zero;
                rect.offsetMin = Vector2.zero;
            }
            TotalResult.Show(data, new[] { DefHeadWoman, DefHeadMan });
            if (data.CurrRound != data.TotalResult.MaxRound)
            {
                GetHandUp().Reset();
            }
        }

        protected virtual void OnCurrRound(EventData evn)
        {
            string roundInfo = (string)evn.data1;
            GetGameInfoPnl().SetRound(roundInfo);
        }

        protected virtual void OnShowChatPnl()
        {
            GetChatPnl().Show();
        }

        protected virtual void OnUserTalk(EventData evn)
        {
            int chair = (int)evn.data1;
            EnChatType type = (EnChatType)evn.data2;
            if (type == EnChatType.ani)
            {
                return;
            }
            PlayersPnl.OnUserTalk(chair, type, evn.data3);
        }
        protected virtual void OnUserOutRoom(EventData evn)
        {
            int chair = (int)evn.data1;
            PlayersPnl.OnUserOut(chair);
        }

        protected virtual void OnGetSoundKey(EventData evn)
        {
            string soundKey = (string)evn.data1;
            TalkPnl.OnGetSoundKey(soundKey);
        }

        protected virtual void OnDownLoadVoice(EventData evn)
        {
            string url = (string)evn.data1;
            int chair = (int)evn.data2;
            int len = (int)evn.data3;

            TalkPnl.DownLoadVoice(url, chair, len);
        }


        protected virtual void OnStopUserSpeak(EventData evn)
        {
            int chair = (int)evn.data1;
            PlayersPnl.OnUserSpeak(chair, false);
        }

        protected virtual void OnShowUserSpeak(EventData evn)
        {
            int chair = (int)evn.data1;
            PlayersPnl.OnUserSpeak(chair, true);
        }


        protected virtual void OnRoomInfo(EventData evn)
        {
            var info = (RoomInfo)evn.data1;
            //设置 圈
            GetGameInfoPnl().SetGameInfo(info);
            GameBtnPnl.SetRoomInfo(info);
            GetGameSettingPnl().SetRoomInfo(info);
            //设置投票的时间
            GetHandUp().TimeTotal = info.TouPiaoTimeOut;

            if (info.CurrRound > 1)
            {
                GameBtnPnl.SetSharBtnActive(false);
            }


        }

        protected virtual void OnIsGameOver(EventData evn)
        {
            //判断是否是游戏结束了
            bool isGameOver = (bool)evn.data1;
            GetOneRoundResult().SetIsGameOver(isGameOver);
        }

        protected virtual void OnShowOneRoundBtn(EventData evn)
        {
            GetOneRoundResult().ShowTotalScoreBtn();
        }

        protected virtual void OnChooseTing(EventData evn)
        {
            GetChooseCgPnl().SetChooseTing();
            GetChooseCgPnl().CancelCall = () =>
            {
                GetOpreateMenu().ShowMenuBg();
                EventDispatch.Dispatch((int)GameEventId.ChooseTingCancel);
            };
        }

        protected virtual void OnTingPai(int eventid, EventData data)
        {
            PlayersPnl.SetTing(_currChair, true);
            PlayersPnl.PlayEffect(_currChair, EnCpgEffect.tingpai);
        }

        //----------------------------------------
        protected virtual void OnGameFree(TableData data)
        {
            for (int i = 0; i < UtilData.CurrGamePalyerCnt; i++)
            {
                int chair = data.GetChairId(i);

                PlayersPnl.SetUserInfo(chair, data.UserDatas[i]);
                //设置头像
                if (data.UserDatas[i].Sex == 0)
                    PlayersPnl.SetUserHead(chair, data.UserDatas[i].HeadImage, DefHeadWoman);
                else
                    PlayersPnl.SetUserHead(chair, data.UserDatas[i].HeadImage, DefHeadMan);

                //如果不是短线重练 设置准备
                if (!data.IsReconect)
                    PlayersPnl.SetReady(chair, data.UserDatas[i].IsReady);

                //设置无当前用户
                PlayersPnl.SetNotCurr(i);
                //重新设置听牌
                PlayersPnl.SetTing(chair, data.IsTing[i]);
            }

            if (data.BankerSeat != UtilDef.NullSeat)
            {
                int bankerChair = data.GetChairId(data.BankerSeat);
                PlayersPnl.SetBanker(bankerChair);
            }

            GetGameInfoPnl().SetMahjongCnt(data.LeaveMahjongCnt);
            GetGameInfoPnl().HideFanPai();

            //根据用户性别设置用户短语
            int playerSex = data.UserDatas[data.PlayerSeat].Sex;
            playerSex = playerSex >= 0 ? playerSex : 0;
            GetChatPnl().SetSortTalk(playerSex);

            if (HuanBao != null)
            {
                HuanBao.SetActive(false);
            }
        }

        protected virtual void OnGamePlay(TableData data)
        {
            GetOneRoundResult().Hide();
            if (data.IsReconect)
            {
                OnGameFree(data);
                //设置赖子 判断是否有赖子
                if (data.RoomInfo.IsExsitLaizi)
                {
                    GetGameInfoPnl().SetFanPaiWithOutAnimation(data.Fanpai);
                }

/*                if (data.ReconectStatus == 0)
                {
                    if (data.CurrChair == 0 && !data.PlayerOpreateMenu.IsForbiddenAll())
                    {
                        GetOpreateMenu().ShowMenu(data.PlayerOpreateMenu);
                    }
                }
                else if (data.ReconectStatus == 1)
                {
                    if (!data.PlayerOpreateMenu.IsForbiddenAll())
                    {
                        GetOpreateMenu().ShowMenu(data.PlayerOpreateMenu);
                    }
                }*/

                if (!data.PlayerOpreateMenu.IsForbiddenAll())
                {
                    GetOpreateMenu().ShowMenu(data.PlayerOpreateMenu);
                }
            }
        }

        protected virtual void OnGameSendCard(TableData data)
        {
            //隐藏准备
            PlayersPnl.HidePlayersReady();
        }

        protected virtual void OnJiaPiaoDispose(int eventId, EventData evn)
        {
            if (null == JiaPiaoPnl) return;

            switch ((UIEventId)eventId)
            {
                case UIEventId.ShowJiaPiaoBtn:
                    JiaPiaoPnl.BtnsGroup.gameObject.SetActive(true);
                    //隐藏微信邀请
                    GameBtnPnl.SetSharBtnActive(false);
                    //隐藏准备按钮
                    PlayersPnl.HidePlayersReady();
                    break;
                case UIEventId.HideJiaPiaoEffect:
                    JiaPiaoPnl.HideAllImg();
                    break;
                case UIEventId.ShowJiaPiaoEffect:
                    OnShowJiaPiaoEffect(evn);
                    break;
            }
        }

        protected virtual void OnShowJiaPiaoEffect(EventData data)
        {
            int chair = (int)data.data1;
            int value = (int)data.data2;

            if (chair != -1)
            {
                JiaPiaoPnl.BtnsGroup.gameObject.SetActive(false);
                if (value == -1) value = 0;
                JiaPiaoPnl.SetImageByPlace(chair, value);
            }
        }

        public void OnShowHulist(int evtId, EventData evn)
        {
            if (null != TingInfoPnl)
            {
                TingInfoPnl.OnShowHulist(evn);
            }
        }

        public virtual void OnHideHulist(int evtId, EventData evn)
        {
            if (null != TingInfoPnl)
            {
                TingInfoPnl.HidePnl();
            }
        }

        public void ShowThrowEffectOnYoujin(int eventId, EventData evn)
        {
            if ((UIEventId)eventId == UIEventId.ShowThrowEffectOnYoujin)
            {
                int seat = (int)evn.data1;
                int type = (int)evn.data2;

                //显示吃碰杠特效
                EnCpgEffect effect = EnCpgEffect.none;

                //游金胡特效
                switch (type)
                {
                    case 2: effect = EnCpgEffect.shuangyou; break;
                    case 3: effect = EnCpgEffect.sanyou; break;
                }

                var chair = UtilFunc.GetChairId(seat);
                PlayersPnl.PlayEffect(chair, effect);

            }
        }

        protected virtual void OnFinalRound(int eventId, EventData data)
        {
            StartCoroutine("ShowFinalRoundWarning");
        }

        protected IEnumerator ShowFinalRoundWarning()
        {
            GameObject obj = transform.FindChild("Warning").gameObject;
            obj.SetActive(true);
            yield return new WaitForSeconds(2f);
            obj.gameObject.SetActive(false);
        }

        //接受GPS信息
        protected virtual void OnReceiveGpsInfo(int eventId, EventData data)
        {
            if (GpsInfosCtrl != null)
            {
                int chair = (int)data.data1;
                string Ip = (string)data.data2;

                ISFSObject info = (ISFSObject)data.data3;
                if (info == null) return;
                GpsInfosCtrl.SetGpsUserData(info, Ip, chair);
            }
        }

        //接受GPS信息
        protected virtual void GetIpAndCountry(int eventId, EventData data)
        {
            if (GpsInfosCtrl != null)
            {
                int chair = (int)data.data1;
                string Ip = (string)data.data2;
                string country = (string)data.data3;
                GpsInfosCtrl.SetIpAndCountry(Ip, country, chair);
            }
        }

        //显示GPS信息
        protected virtual void OnShowGPSInfo(int eventId, EventData data)
        {
            if (GpsInfosCtrl != null)
            {
                if (GpsInfosCtrl.IsShow)
                    GpsInfosCtrl.Hide();
                else
                {
                    GpsInfosCtrl.Show();
                    for (int i = 0; i < PlayersPnl.Players.Length; i++)
                    {
                        if (!PlayersPnl.Players[i].gameObject.activeInHierarchy)
                        {
                            GpsInfosCtrl.Users[i].IsShowHead = false;
                            continue;
                        }

                        int nextSeat = (i + 1) % UtilDef.GamePlayerCnt;

                        if (i == UtilDef.GamePlayerCnt - 1)
                        {
                            nextSeat = (i + 2) % UtilDef.GamePlayerCnt;
                        }

                        GpsInfosCtrl.Users[i].IsShowHead = true;
                        GpsInfosCtrl.Users[i].Init(i, nextSeat);
                        GpsInfosCtrl.Users[i].ShowAddressInfo();
                    }
                    GpsInfosCtrl.ShowLine();
                }
            }
        }

    }
}