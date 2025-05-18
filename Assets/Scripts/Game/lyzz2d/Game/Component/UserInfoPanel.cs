using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.lyzz2d.Game.GameCtrl;
using Assets.Scripts.Game.lyzz2d.Game.Talk;
using Sfs2X.Entities;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using AsyncImage = Assets.Scripts.Game.lyzz2d.Utils.AsyncImage;

namespace Assets.Scripts.Game.lyzz2d.Game.Component
{
    public class UserInfoPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _addGang;

        [SerializeField] private GameObject _auto;

        [SerializeField] private GameObject _Outline;

        [SerializeField] private TalkBubble _talkBubble;

        [SerializeField] private GameObject _ting;

        /// <summary>
        ///     面板信息
        /// </summary>
        private UserData _userInfo;

        private float _voice;

        [SerializeField] private GameObject _zhuang;

        public Color AddGoldColor;
        public UILabel DesLabel; //玩家GPS信息面板
        public UILabel DistanceLabel; //距离描述     
        public UILabel Gold;
        public GameObject GpsLine; //GPS的线
        public Color MinusGoldColor;
        public GameObject Speaker; // 扬声器
        public UITexture UserIcon; // 用户头像

        /// <summary>
        ///     玩家ID
        /// </summary>
        public UILabel UserID;

        public UILabel UserNameLabel;

        public UserData UserInfo
        {
            get { return _userInfo; }
            set
            {
                _userInfo = value;
                if (_userInfo != null)
                {
                    gameObject.SetActive(true);
                    _userInfo.IsStayInRoom = true;
                    MRefresh();
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }

        public GameObject UserHead
        {
            get { return UserIcon.transform.parent.gameObject; }
        }

        private void Awake()
        {
            Reset();
        }

        public void Refresh()
        {
            SetOutLineState(UserInfo.IsOnLine);
            SetGang(UserInfo.Piao);
            UserHead.SetActive(true);
        }

        public void Reset()
        {
            if (_userInfo == null)
            {
                UserNameLabel.text = "";
                Gold.text = "";
                UserID.text = "";
            }
            gameObject.SetActive(_userInfo != null && _userInfo.IsStayInRoom);
        }

        public void SetGold(int num)
        {
            if (_userInfo != null)
            {
                _userInfo.Gold = num;
            }
            YxTools.TrySetComponentValue(Gold, YxUtiles.GetShowNumber(num).ToString());
        }

        // 显示用户头像
        public void ShowUserIcon(string url)
        {
            AsyncImage.Instance.SetAsyncImage(url, UserIcon, _userInfo.Sex);
        }

        public void UserLeave(User user)
        {
            if (_userInfo == null) return;
            if (user.Id == _userInfo.id)
            {
                UserLeave();
            }
        }

        public void UserLeave()
        {
            if (App.GetGameManager<Lyzz2DGameManager>().IsGameing())
            {
                if (UserInfo != null)
                {
                    UserInfo.IsStayInRoom = false;
                }
                return;
            }
            if (UserInfo != null)
            {
                UserInfo.IsStayInRoom = false;
                MRefresh();
            }
            if (Application.isPlaying)
            {
                if (gameObject != null)
                {
                    gameObject.SetActive(false);
                }
            }
        }

        public void GameReady()
        {
            if (UserInfo != null)
            {
                UserInfo.isReady = true;
                MRefresh();
            }
        }

        /// <summary>
        ///     刷新头像相关信息
        /// </summary>
        private void MRefresh()
        {
            UserNameLabel.text = UserInfo.name;
            UserID.text = string.Format("ID:{0}", UserInfo.id);
            SetGold((int) UserInfo.Gold);
            ShowUserIcon(UserInfo.HeadImage);
        }

        /// <summary>
        ///     播放语音聊天
        /// </summary>
        public void PlayVoiceChat(AudioClip chatClip, float len)
        {
            _voice = Facade.Instance<MusicManager>().MusicVolume;
            Facade.Instance<MusicManager>().MusicVolume = 0;
            Facade.Instance<MusicManager>().Play(chatClip);
            Speaker.SetActive(true);
            Invoke("CloseSpeaker", len);
        }

        public void ShowTalkContent(string text)
        {
            _talkBubble.ShowLabel(text);
        }

        public void ShowPhizContent(int index)
        {
            _talkBubble.ShowPhiz(index);
        }

        /// <summary>
        ///     关闭小喇叭
        /// </summary>
        private void CloseSpeaker()
        {
            Speaker.SetActive(false);
            Facade.Instance<MusicManager>().MusicVolume = _voice;
        }

        public void SetOutLineState(bool state, bool showTag = true)
        {
            if (state)
            {
                UserNameLabel.color = Color.white;
                UserIcon.color = Color.white;
                Gold.GetComponent<UILabel>().color = Color.white;
                UserID.color = Color.white;
            }
            else
            {
                UserNameLabel.color = Color.gray;
                UserIcon.color = Color.gray;
                Gold.GetComponent<UILabel>().color = Color.gray;
                UserID.color = Color.gray;
            }
            _Outline.SetActive(showTag && !state);
        }

        public void SetZhuang(bool state)
        {
            _zhuang.SetActive(state);
        }

        public void SetGang(int gangNum)
        {
            _addGang.SetActive(gangNum > 0);
            switch (gangNum)
            {
                case 1:
                case 99:
                    _addGang.GetComponent<UISprite>().spriteName = string.Format("AddGang{0}", gangNum);
                    break;
            }
        }

        /// <summary>
        ///     显示地址信息
        /// </summary>
        public void ShowAddressInfo()
        {
            DesLabel.gameObject.SetActive(true);
            DesLabel.text = string.Format("ID:{2}\nIP:{0}\n所在地:{1}", _userInfo.ip,
                UserInfo.IsHasGpsInfo ? UserInfo.Country : "未提供位置信息\n请开启位置服务,并给予应用相应权限", UserInfo.id);
        }

        public void SetTing(bool state)
        {
            if (_ting != null)
            {
                _ting.SetActive(state);
            }
        }
    }
}