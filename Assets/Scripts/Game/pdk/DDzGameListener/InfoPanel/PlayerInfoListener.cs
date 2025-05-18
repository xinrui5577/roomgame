using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Game.pdk.DDz2Common;
using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using Assets.Scripts.Game.pdk.PokerRule;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.pdk.DDzGameListener.InfoPanel
{
    public class PlayerInfoListener : ServEvtListener
    {
        public const string SpkBuJiao = "bujiao";
        public const string Spk1Fen = "1fen";
        public const string Spk2Fen = "2fen";
        public const string Spk3Fen = "3fen";
        public const string SpkBuChu = "buchu 1";

        public const string SpkJiabei = "jiabei";
        public const string SpkBuJiabei = "bujiabei";
        public const string SpkDoubleIcon = "DoubleIcon";

        public NumScoreAnimCtrl NumScoreAnimCtrlGob;

        protected override void OnAwake()
        {
            PdkGameManager.AddOnGameInfoEvt(SetUserInfo);
            PdkGameManager.AddOnGetRejoinDataEvt(OnRejoinGame);
            Ddz2RemoteServer.AddOnUserReadyEvt(OnUserReady);
            Ddz2RemoteServer.AddOnUserJoinRoomEvt(OnUserJoinRoom);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeGrab, OnTypeGrab);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeGrabSpeaker, OnTypeGrabSpeaker);

            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeAllocate, OnAlloCateCds);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeFirstOut, TypeFirstOut);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeGameOver, OnTypeGameOver);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeDoubleOver, OnDoubleOver);

            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeBombScore, OnTypeBombScore);
            //Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeOutCard, OnTypeOutCard);
        }
        protected virtual void SetUserInfo(object sender, DdzbaseEventArgs args)
        {

        }
        private void OnRejoinGame(object sender, DdzbaseEventArgs args)
        {
            //  if (UserDataTemp==null) return;

            SetUserInfo(sender, args);

            /*            var data = args.IsfObjData;

                        //检查是否显示地主图标
                        if ( UserDataTemp.GetInt(RequestKey.KeySeat) == data.GetInt(NewRequestKey.KeyLandLord))
                        {
                            DizhuSp.SetActive(true);
                            return;
                        }

                        CheckShowJiabeiIcon(data);*/

            var data = args.IsfObjData;
            if (data.ContainsKey(NewRequestKey.KeyState) && data.GetInt(NewRequestKey.KeyState) == GlobalConstKey.StatusHasStart) GameReadySp.gameObject.SetActive(false);
        }
        protected virtual void OnUserReady(object sender, DdzbaseEventArgs args)
        {
            ShowJiaBeiSp.SetActive(false);
            DizhuSp.SetActive(false);
            SetSpeakSpState(false);
            var data = args.IsfObjData;
            if (UserDataTemp != null && data.GetInt(RequestKey.KeySeat) == UserDataTemp.GetInt(RequestKey.KeySeat)) GameReadySp.gameObject.SetActive(true);
        }
        protected virtual void OnUserJoinRoom(object sender, DdzbaseEventArgs args)
        {

        }
        /// <summary>
        /// 当有人叫分抢地主时
        /// </summary>
        protected void OnTypeGrab(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            if (!DDzUtil.IsServDataContainAllKey(
                new[]
                    {
                        RequestKey.KeySeat, RequestKey.KeyScore
                    }, data))
            {
                Debug.LogError("有人叫分时，信息key不全");
                return;
            }

            var seat = data.GetInt(RequestKey.KeySeat);
            var score = data.GetInt(RequestKey.KeyScore);
            if (UserDataTemp != null && UserDataTemp.GetInt(RequestKey.KeySeat) == seat)
            {
                //叫的分值
                switch (score)
                {
                    case 0:
                        {
                            SetSpeakSpState(true, SpkBuJiao);
                            break;
                        }
                    case 1:
                        {
                            SetSpeakSpState(true, Spk1Fen);
                            break;
                        }
                    case 2:
                        {
                            SetSpeakSpState(true, Spk2Fen);
                            break;
                        }
                    case 3:
                        {
                            SetSpeakSpState(true, Spk3Fen);
                            break;
                        }
                    default:
                        {
                            Debug.LogError("叫了一个超出预估的分值，所以不显示");
                            SetSpeakSpState(false);
                            break;
                        }
                }

            }

        }
        /// <summary>
        /// 开始叫分时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnTypeGrabSpeaker(object sender, DdzbaseEventArgs args)
        {
            //进入叫分阶段后，隐藏掉准备状态
            GameReadySp.gameObject.SetActive(false);
        }
        /// <summary>
        /// 当给这个玩家发牌时
        /// </summary>
        protected virtual void OnAlloCateCds(object sender, DdzbaseEventArgs args)
        {
            //进入发牌阶段，隐藏掉准备状态
            GameReadySp.gameObject.SetActive(false);

            var data = args.IsfObjData;
            //此处不用判断是不是给自己发牌，因为服务器实际只给 游戏玩家自己发牌，其他玩家不发，但是每个玩家的牌数发的是一样的。所以可以赋值，擦
            if (UserDataTemp == null) return;
            var cardsLen = data.GetIntArray(GlobalConstKey.C_Cards).Length;
            UserDataTemp.PutInt(NewRequestKey.KeyCardNum, cardsLen);
        }
        /// <summary>
        /// 当收到服务TypeFirstOut器相应
        /// </summary>
        private void TypeFirstOut(object sender, DdzbaseEventArgs args)
        {
            OnGetDipai(sender, args);

            OnCheckSelectDouble(args.IsfObjData);
        }
        /// <summary>
        /// 当游戏结算时清空GameReadySp 和  ShowSpeakSp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void OnTypeGameOver(object sender, DdzbaseEventArgs args)
        {
            if (GameReadySp != null) GameReadySp.gameObject.SetActive(false);
            SetSpeakSpState(false);
            if (UserDataTemp != null && UserDataTemp.ContainsKey(NewRequestKey.KeyCardNum)) UserDataTemp.PutInt(NewRequestKey.KeyCardNum, 0);
        }
        /// <summary>
        /// 当收到加倍已经结束的信息
        /// </summary>
        protected virtual void OnDoubleOver(object sender, DdzbaseEventArgs args)
        {
            if (UserDataTemp == null) return;

            var data = args.IsfObjData;

            var rates = data.GetIntArray("jiabei");
            var len = rates.Length;
            var thisUserSeat = UserDataTemp.GetInt(RequestKey.KeySeat);

            for (int i = 0; i < len; i++)
            {
                if (thisUserSeat != i) continue;
                if (rates[i] > 1)
                {
                    SetSpeakSpState(true, SpkJiabei);
                    ShowJiaBeiSp.gameObject.SetActive(true);
                }
                else
                {
                    SetSpeakSpState(true, SpkBuJiabei);
                }

                break;
            }

            StopCoroutine("HideJiabeiSp");
            StartCoroutine("HideJiabeiSp");
        }
        private void OnTypeBombScore(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            if (UserDataTemp != null &&
                UserDataTemp.ContainsKey(RequestKey.KeySeat) &&
                data.ContainsKey(RequestKey.KeySeat))
            {
                if (UserDataTemp.GetInt(RequestKey.KeySeat) == data.GetInt(RequestKey.KeySeat))
                {
                    if (data.ContainsKey(NewRequestKey.KeyWin))
                    {
                        PlayShowScore(data.GetInt(NewRequestKey.KeyWin));
                    }
                }
                else
                {
                    if (data.ContainsKey(NewRequestKey.KeyLost))
                    {
                        PlayShowScore(data.GetInt(NewRequestKey.KeyLost));
                    }
                }

            }
        }

        /// <summary>
        /// 播放加减分
        /// </summary>
        /// <param name="score"></param>
        private void PlayShowScore(int score)
        {
            try
            {
                var orgScore = int.Parse(ScoreLabel.text);
                ScoreLabel.text = (orgScore + score).ToString(CultureInfo.InvariantCulture);
                if (NumScoreAnimCtrlGob != null) NumScoreAnimCtrlGob.ShowScoreNum(score);
            }
            catch (Exception e)
            {
                Debug.LogError("当前scorelabel没有:" + e.Message);
            }
        }


        void Start()
        {
            App.GetGameData<GlobalData>().OnUserScoreChanged = OnUserScoreChanged;
        }
        /// <summary>
        /// 当玩家分数改变时
        /// </summary>
        /// <param name="seat"></param>
        /// <param name="scoreGold"></param>
        private void OnUserScoreChanged(int seat, int scoreGold)
        {
            if (UserDataTemp == null) return;

            if (seat == UserDataTemp.GetInt(RequestKey.KeySeat))
            {
                var curGold = UserDataTemp.GetLong(NewRequestKey.KeyTtGold) + scoreGold;
                UserDataTemp.PutLong(NewRequestKey.KeyTtGold, curGold);
                ScoreLabel.text = curGold.ToString(CultureInfo.InvariantCulture);
            }
        }
        //用户信息缓存
        protected ISFSObject UserDataTemp;

        //静态信息-----------------------start
        [SerializeField]
        protected UITexture HeadTexture;
        [SerializeField]
        protected UILabel IdLabel;
        [SerializeField]
        protected UILabel NameLabel;
        [SerializeField]
        protected UILabel ScoreLabel;
        //------------------------------end


        //动态信息-----------------------------start
        /// <summary>
        /// 玩家准备了
        /// </summary>
        [SerializeField]
        protected UISprite GameReadySp;
        /// <summary>
        /// 显示玩家行动时要说的话
        /// </summary>
        [SerializeField]
        protected UISprite ShowSpeakSp;

        protected void SetSpeakSpState(bool isActive, string spriteName)
        {
            if (ShowSpeakSp == null) return;

            StopCoroutine("HideSpeakeSp");
            ShowSpeakSp.gameObject.SetActive(isActive);
            ShowSpeakSp.spriteName = spriteName;
            ShowSpeakSp.MakePixelPerfect();
            if (spriteName == SpkBuChu) StartCoroutine(HideSpeakeSp());
        }

        protected void SetSpeakSpState(bool isActive)
        {
            if (ShowSpeakSp == null) return;

            StopCoroutine("HideSpeakeSp");
            ShowSpeakSp.gameObject.SetActive(isActive);
            ShowSpeakSp.MakePixelPerfect();
        }

        private IEnumerator HideSpeakeSp()
        {
            yield return new WaitForSeconds(2f);
            ShowSpeakSp.gameObject.SetActive(false);
        }

        /// <summary>
        /// 显示玩家是否加倍
        /// </summary>
        [SerializeField]
        protected GameObject ShowJiaBeiSp;

        /// <summary>
        /// 地主身份sprite的 GameObject
        /// </summary>
        [SerializeField]
        protected GameObject DizhuSp;
        //--------------------------------end

        /// <summary>
        /// 根据缓存的信息刷新用户信息ui
        /// </summary>
        public override void RefreshUiInfo()
        {
            if (UserDataTemp != null)
            {
                if (UserDataTemp.ContainsKey(RequestKey.KeyName))
                    NameLabel.text = UserDataTemp.GetUtfString(RequestKey.KeyName);

                if (UserDataTemp.ContainsKey(RequestKey.KeyId))
                    IdLabel.text = UserDataTemp.GetInt(RequestKey.KeyId).ToString(CultureInfo.InvariantCulture);


                short sex = 0;
                if (UserDataTemp.ContainsKey(NewRequestKey.KeySex)) sex = UserDataTemp.GetShort(NewRequestKey.KeySex);

                if (UserDataTemp.ContainsKey(NewRequestKey.KeyAvatar))
                {
                    DDzUtil.LoadRealHeadIcon(UserDataTemp.GetUtfString(NewRequestKey.KeyAvatar), sex, HeadTexture);
                }
                else
                {
                    DDzUtil.LoadDefaultHeadIcon(sex, HeadTexture);
                }

                if (UserDataTemp.ContainsKey(NewRequestKey.KeyTtGold))
                    ScoreLabel.text =
                        UserDataTemp.GetLong(NewRequestKey.KeyTtGold).ToString(CultureInfo.InvariantCulture);

                if (!App.GetGameData<GlobalData>().IsStartGame
                    && UserDataTemp.ContainsKey(RequestKey.KeyState))
                    GameReadySp.gameObject.SetActive(UserDataTemp.GetBool(RequestKey.KeyState));
            }

        }

        /// <summary>
        /// 获得这个玩家的手牌数量,获取不到时返回-1；
        /// </summary>
        public int ThisPlayerHdCdsNum
        {
            get
            {
                if (UserDataTemp == null || !UserDataTemp.ContainsKey(NewRequestKey.KeyCardNum)) return -1;
                return UserDataTemp.GetInt(NewRequestKey.KeyCardNum);
            }
        }

        /// <summary>
        /// 检查是否显示加倍icon
        /// </summary>
        /// <param name="data">重连时获得的消息数据</param>
        private void CheckShowJiabeiIcon(ISFSObject data)
        {
            if (UserDataTemp == null) return;

            var thisUserSeat = UserDataTemp.GetInt(RequestKey.KeySeat);

            //先查是不是自己
            if (data.ContainsKey(RequestKey.KeyUser))
            {
                var user = data.GetSFSObject(RequestKey.KeyUser);
                if (user.GetInt(RequestKey.KeySeat) == thisUserSeat && user.GetInt(NewRequestKey.KeyRate) > 1)
                {
                    ShowJiaBeiSp.SetActive(true);
                    return;
                }
            }

            //检查显示加倍图标
            if (data.ContainsKey(RequestKey.KeyUserList))
            {
                var users = data.GetSFSArray(RequestKey.KeyUserList);
                var len = users.Count;
                for (int i = 0; i < len; i++)
                {
                    var user = users.GetSFSObject(i);
                    if (user.GetInt(RequestKey.KeySeat) != thisUserSeat) continue;
                    if (user.GetInt(NewRequestKey.KeyRate) > 1)
                    {
                        ShowJiaBeiSp.SetActive(true);
                    }
                }
            }
        }

        /// <summary>
        /// 检查是否在选择加倍
        /// </summary>
        /// <param name="data"></param>
        public virtual void OnCheckSelectDouble(ISFSObject data)
        {

        }

        /// <summary>
        /// 从服务器获得底牌信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public virtual void OnGetDipai(object sender, DdzbaseEventArgs args)
        {
            if (UserDataTemp == null) return;

            var data = args.IsfObjData;
            //判断是给这位玩家发底牌么
            if (data.GetInt(RequestKey.KeySeat) != UserDataTemp.GetInt(RequestKey.KeySeat)) return;
            var ttcdsNum = UserDataTemp.GetInt(NewRequestKey.KeyCardNum) + data.GetIntArray(RequestKey.KeyCards).Length;
            UserDataTemp.PutInt(NewRequestKey.KeyCardNum, ttcdsNum);
            DizhuSp.SetActive(true);
            SetSpeakSpState(false);
        }

        /// <summary>
        /// 隐藏加倍说话sp
        /// </summary>
        /// <returns></returns>
        private IEnumerator HideJiabeiSp()
        {
            yield return new WaitForSeconds(3f);
            if (ShowSpeakSp.spriteName.Equals(SpkJiabei) || ShowSpeakSp.spriteName.Equals(SpkBuJiabei))
                ShowSpeakSp.gameObject.SetActive(false);
        }

        //--------以下子类用到的公共方法-------------------------------------------------------------------------
        /// <summary>
        /// 获得玩家最大人人数
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected int GetPlayerMaxNum(ISFSObject data)
        {
            if (!data.ContainsKey(NewRequestKey.KeyPlayerNum)) throw new Exception("此isfobj data  不能获得玩家最大人数");
            return data.GetInt(NewRequestKey.KeyPlayerNum);
        }

        /// <summary>
        /// 获得其他玩家的数据信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected Dictionary<int, ISFSObject> GetOtherUsesDic(ISFSObject data)
        {
            if (!data.ContainsKey(RequestKey.KeyUserList)) throw new Exception("此isfobj data  不能获得玩家其他玩家的数据集合");
            //其他玩家数据集合
            var otherUsers = data.GetSFSArray(RequestKey.KeyUserList);
            //座位号对应ISFSObject
            var dataDic = new Dictionary<int, ISFSObject>();
            foreach (var user in otherUsers)
            {
                var isfobj = user as ISFSObject;
                if (isfobj != null) dataDic[isfobj.GetInt(RequestKey.KeySeat)] = isfobj;
            }
            return dataDic;
        }

        /*        /// <summary>
                /// 设置默认头像
                /// </summary>
                protected void LoadDefaultHeadIcon(short sex)
                {
                    string assetName = sex == 0 ? "headtexture0" : "headtexture1";
                    var textureGob = ResourceManager.LoadAsset(assetName, assetName);
                    HeadTexture.mainTexture = textureGob.GetComponent<UITexture>().mainTexture;
                }*/

        /*        /// <summary>
                /// 加载真实头像
                /// </summary>
                /// <param name="headImgUrl">头像地址</param>
                /// <param name="sex">按性别加载默认头像</param>
                protected void LoadRealHeadIcon(string headImgUrl,short sex)
                {
                    //加载真实头像
                    Facade.Instance<AsyncImage>()
                          .GetAsyncImage(headImgUrl, tex =>
                          {
                              if (tex != null)
                              {
                                  HeadTexture.mainTexture = tex;
                              }
                              else
                              {
                                  DDzUtil.LoadDefaultHeadIcon(sex, HeadTexture);
                              }
                          });
                }*/

        /// <summary>
        /// 获得此客户端玩家自己的data
        /// </summary>
        /// <returns></returns>
        protected ISFSObject GetHostUserData(ISFSObject data)
        {
            if (data.ContainsKey(RequestKey.KeyUser))
            {
                return data.GetSFSObject(RequestKey.KeyUser);
            }

            throw new Exception("得到了空的服务器信息");
        }
    }
}
