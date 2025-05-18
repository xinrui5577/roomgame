using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [UIPanelData(typeof(PanelHandup), UIPanelhierarchy.System)]
    public class PanelHandup : UIPanelBase, IUIPanelControl<HandupEventArgs>
    {
        public Text Time;
        public Text Message;
        public Text DismissUser;
        public MahjongUiButton AgreenBtn;
        public MahjongUiButton RefuseBtn;
        public Sprite[] StateSprites;
        public HandupPlayerItem[] PlayersItem;

        private Dictionary<int, DismissFeedBack> mStateCache;
        private bool mIsCountDown = false;
        private string mDismissUserMsg;
        private float mTimer = 0;

        private float mTimeTotal
        {
            get { return GameCenter.DataCenter.Config.TimeHandup; }
        }

        public override void OnContinueGameUpdate() { Close(); }

        public void OnRequestDismissClick()
        {
            OnRequestHandup((int)DismissFeedBack.ApplyFor);
        }

        public void OnApplyClick()
        {
            OnRequestHandup((int)DismissFeedBack.Agree);
        }

        public void OnRefuseClick()
        {
            OnRequestHandup((int)DismissFeedBack.Refuse);
        }

        public override void Close()
        {
            ResetData();
            SetHandMjTouch(true);
            ButtonsSetActive(true);
            GameCenter.DataCenter.DissolvedState = false;
            base.Close();
        }

        public override void Open()
        {
            base.Open();
            //正在解散房间
            GameCenter.DataCenter.DissolvedState = true;
            SetHandMjTouch(false);
            SetButtonAction(false);
            SetPlayerInfo();
        }

        public void Open(HandupEventArgs Args)
        {
            Open();
            SetButtonAction(true);
            SetDismissInfo(Args);
        }

        private void ResetPlayersItem()
        {
            for (int i = 0; i < PlayersItem.Length; i++)
            {
                PlayersItem[i].gameObject.SetActive(false);
            }
        }

        private void SetPlayerInfo()
        {
            ResetPlayersItem();
            MahjongUserInfo data = null;
            MahjongPlayersData playersData = GameCenter.DataCenter.Players;
            for (int i = 0; i < playersData.CurrPlayerCount; i++)
            {
                data = playersData[i];
                PlayersItem[data.Chair].gameObject.SetActive(true);
                PlayersItem[data.Chair].SetDismissSelect(StateSprites[0]);
                PlayersItem[data.Chair].SetDismissInfo(data.NickM, playersData.GetPlayerHead(data.Chair));
            }
        }

        public void SetHandupState(HandupEventArgs Args)
        {
            DismissFeedBack type = Args.HandupType;
            if (type == DismissFeedBack.Agree)
            {
                int chair = Args.Chair;
                if (mStateCache.ContainsKey(chair))
                {
                    mStateCache[chair] = type;
                    PlayersItem[chair].SetDismissSelect(GetSprite(Args.HandupType));
                    if (type == DismissFeedBack.Agree && chair == 0)
                    {
                        ButtonsSetActive(false);
                    }
                }
            }
            else
            {
                Close();
            }
        }

        private void ButtonsSetActive(bool isOn)
        {
            AgreenBtn.gameObject.SetActive(isOn);
            RefuseBtn.gameObject.SetActive(isOn);
        }

        private void Update()
        {
            if (mIsCountDown)
            {
                mTimer -= UnityEngine.Time.deltaTime;
                if (mTimer <= 0)
                {
                    ButtonsSetActive(false);
                    mIsCountDown = false;
                    mTimer = 0;
                    Close();
                }
                Time.text = ((int)mTimer).ToString();
            }
        }

        /// <summary>
        /// 设置投票界面数据
        /// </summary>  
        private void SetDismissInfo(HandupEventArgs Args)
        {
            var playersData = GameCenter.DataCenter.Players;
            var name = playersData[Args.Chair].NickM.Replace(" ", "");
            mDismissUserMsg = "玩家【" + name + "】申请解散房间，请等待其他玩家选择。";
            DismissUser.text = mDismissUserMsg;
            mStateCache = new Dictionary<int, DismissFeedBack>();
            mStateCache.Add(Args.Chair, DismissFeedBack.Agree);
            var count = GameCenter.DataCenter.MaxPlayerCount;
            for (int i = 0; i < count; i++)
            {
                if (!mStateCache.ContainsKey(i))
                {
                    mStateCache.Add(i, DismissFeedBack.None);
                }
                PlayersItem[i].SetDismissSelect(GetSprite(mStateCache[i]));
            }
            ButtonsSetActive(playersData[0].NickM != Args.UserName);
            mTimer = Args.Time == 0 ? mTimeTotal : Args.Time;
            mIsCountDown = true;
        }

        /// <summary>
        /// 设置投票界面数据
        /// </summary>  
        public void OnRejoinSetDismissInfo(RejoinHandupEventArgs Args)
        {
            Open();
            mStateCache = new Dictionary<int, DismissFeedBack>();
            var idArr = Args.PlayersId.Split(',');
            var db = GameCenter.DataCenter;
            for (int i = 0; i < db.MaxPlayerCount; i++)
            {
                var chair = i;
                var item = PlayersItem[chair];
                var playerInfo = db.Players[chair];
                mStateCache.Add(chair, DismissFeedBack.None);
                //以及投票的玩家信息
                for (int j = 0; j < idArr.Length; j++)
                {
                    var id = int.Parse(idArr[j]);
                    if (id == playerInfo.Id)
                    {
                        mStateCache[chair] = DismissFeedBack.Agree;
                        if (j == 0)
                        {
                            var name = playerInfo.NickM.Replace(" ", "");
                            DismissUser.text = "玩家【" + name + "】申请解散房间，请等待其他玩家选择。";
                        }
                    }
                }
                var type = mStateCache[chair];
                item.SetDismissSelect(GetSprite(type));
                if (i == 0)
                {
                    var flag = type == DismissFeedBack.None;
                    ButtonsSetActive(flag);
                    SetButtonAction(flag);
                }
            }
            mTimer = Args.Time == 0 ? mTimeTotal : Args.Time;
            Message.ExCompShow().text = "(超过{0}秒未做出选择，则默认同意!)".ExFormat(mTimeTotal);
            mIsCountDown = true;
        }

        private void OnRequestHandup(int type)
        {
            GameCenter.EventHandle.Dispatch<C2SDismissRoomArgs>((int)EventKeys.C2SDismissRoom, (param) =>
            {
                param.DismissType = type;
            });
        }

        private void SetButtonAction(bool isOn)
        {
            AgreenBtn.Event.RemoveAllListeners();
            RefuseBtn.Event.RemoveAllListeners();
            if (!isOn)
            {
                Message.ExCompHide();
                AgreenBtn.Event.AddListener(OnRequestDismissClick);
                RefuseBtn.Event.AddListener(Close);
            }
            if (isOn)
            {
                Message.ExCompShow().text = "(超过{0}秒未做出选择，则默认同意!)".ExFormat(mTimeTotal);
                AgreenBtn.Event.AddListener(OnApplyClick);
                RefuseBtn.Event.AddListener(OnRefuseClick);
            }
        }

        private Sprite GetSprite(DismissFeedBack type)
        {
            Sprite s = StateSprites[1];
            switch (type)
            {
                case DismissFeedBack.None:
                    s = StateSprites[1];
                    break;
                case DismissFeedBack.Agree:
                    s = StateSprites[0];
                    break;
                case DismissFeedBack.Refuse:
                    s = StateSprites[2];
                    break;
            }
            return s;
        }

        private void ResetData()
        {
            mTimer = 0;
            mIsCountDown = false;
        }
    }
}