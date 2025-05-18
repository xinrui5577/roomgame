using System.Collections;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.Mahjong2D.Game.Component.UserInfo;
using Assets.Scripts.Game.Mahjong2D.Game.Data;
using Assets.Scripts.Game.Mahjong2D.Game.Item;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.DataBundles;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.Mahjong2D.Common.MahJongCommon
{
    public class UserInfoPanel : MonoBehaviour
    {
        /// <summary>
        /// 玩家当前的钱数
        /// </summary>
        [SerializeField]
        private UILabel _goldNumber;
        public UILabel UserNameLabel;
        public GameObject Speaker;                  // 扬声器
        public YxBaseTextureAdapter UserIcon;       // 用户头像
        public UILabel DesLabel;                    //玩家GPS信息面板
        public GameObject GpsLine;                  //GPS的线
        public UILabel DistanceLabel;               //距离描述     
        /// <summary>
        /// 玩家ID
        /// </summary>
        public UILabel UserID;
        [SerializeField]
        private TalkBubble _talkBubble;
        [SerializeField]
        private GameObject _auto;
        [SerializeField]
        private GameObject _Outline;
        [SerializeField]
        private GameObject _addGang;
        [SerializeField]
        private GameObject _zhuang;
        [SerializeField] protected GameObject _ting;
        [Tooltip("金币背景,无玩家信息时隐藏")]
        public GameObject GoldNumBg;
        [Tooltip("房主标识")]
        public GameObject RoomOenerFlag;
        [Tooltip("0座位号显示标记（圈头标记，默认座位号0）")]
        public GameObject SeatZeroFlag;

        /// <summary>
        /// 面板信息
        /// </summary>
        private UserData _userInfo;
        [Tooltip("准备状态移动动画")]
        public TweenPosition TwPos;
        [Tooltip("准备状态缩放动画")]
        public TweenScale TwScale;
        [Tooltip("无数据隐藏")]
        public bool NoDataHide = true;
        [Tooltip("空头像")]
        public string EmptyAvatar = "http://empty";
        public UserData UserInfo
        {
            get
            {
                return _userInfo;
            }
            set
            {
                _userInfo = value;
                if (_userInfo != null)
                {
                    _userInfo.IsStayInRoom = true;
                }
                MRefresh();
            }
        }

        void Awake()
        {
            if (TwPos==null)
            {
                TwPos = GetComponent<TweenPosition>();
            }
            if (TwScale == null)
            {
                TwScale = GetComponent<TweenScale>();
            }
        }

        public void Refresh(bool isGameing)
        {
            SetOutLineState(UserInfo.IsOnLine);
            SetGang(UserInfo.Piao);
            FreshReadyTween(isGameing);
        }

        private void FreshReadyTween(bool isGameing)
        {
            if (TwScale)
            {
                TwScale.Play(isGameing);
            }
            if (TwPos)
            {
                TwPos.Play(isGameing);
            }
        }

        public void SetGold(long num)
        {
            _goldNumber.TrySetComponentValue(YxUtiles.GetShowNumber(num).ToString());
        }
        /// <summary>
        /// 增加钱数的变化
        /// </summary>
        /// <param name="num"></param>
        public void AddGold(int num)
        {
            if (_userInfo != null)
            {
                _userInfo.Gold += num;
                SetGold(_userInfo.Gold);
            }
        }

        // 显示用户头像
        public void ShowUserIcon(string url)
        {
            if (UserIcon)
            {
                PortraitDb.SetPortrait(url, UserIcon, UserInfo.Sex);
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
        /// 刷新头像相关信息
        /// </summary>
        private void MRefresh()
        {
            bool userInfoNull = _userInfo == null;
            if (NoDataHide)
            {
                gameObject.TrySetComponentValue(!userInfoNull);
            }
            GoldNumBg.TrySetComponentValue(!userInfoNull);
            if (userInfoNull)
            {
                GoldNumBg.TrySetComponentValue(false);
                RoomOenerFlag.TrySetComponentValue(false);
                SeatZeroFlag.TrySetComponentValue(false);
                UserNameLabel.TrySetComponentValue(string.Empty);
                _goldNumber.TrySetComponentValue(string.Empty);
                UserID.TrySetComponentValue(string.Empty);
                _Outline.TrySetComponentValue(false);
                if (UserIcon)
                {
                    PortraitDb.SetPortrait(EmptyAvatar,UserIcon,0);
                }
            }
            else
            {
                UserNameLabel.TrySetComponentValue(UserInfo.name);
                UserID.TrySetComponentValue(string.Format("ID:{0}", UserInfo.id));
                SetGold((int)UserInfo.Gold);
                if (UserIcon)
                {
                    PortraitDb.SetPortrait(UserInfo.HeadImage, UserIcon,UserInfo.Sex);
                }
                RoomOenerFlag.TrySetComponentValue(UserInfo.IsOwner);
                SeatZeroFlag.TrySetComponentValue(UserInfo.Seat==0);
            }
        }
        private float _voice;
        private Coroutine _voiceCor;
        /// <summary>
        /// 播放语音聊天
        /// </summary>
        public void PlayVoiceChat(AudioClip chatClip, float len)
        {
            Facade.Instance<MusicManager>().SetMusicPause(true);
            Facade.Instance<MusicManager>().Play(chatClip);
            Speaker.TrySetComponentValue(true);
            if (gameObject.activeInHierarchy)
            {
                if (_voiceCor!=null)
                {
                    StopCoroutine(_voiceCor);
                }
                _voiceCor= StartCoroutine(CloseSpeaker(len));
            }
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
        /// 关闭小喇叭
        /// </summary>
        IEnumerator  CloseSpeaker(float time)
        {
            yield return new WaitForSeconds(time);
            Speaker.TrySetComponentValue(false);
            Facade.Instance<MusicManager>().SetMusicPause(false);
        }

        public void SetOutLineState(bool state, bool showTag = true)
        {
            if (state)
            {
                UserNameLabel.color = Color.white;
                if (UserIcon)
                {
                    UserIcon.Color = Color.white;
                }
                _goldNumber.color = Color.white;
                if (UserID)
                {
                    UserID.color = Color.white;
                }
            }
            else
            {
                UserNameLabel.color = Color.gray;
                if (UserIcon)
                {
                    UserIcon.Color = Color.gray;
                }
                _goldNumber.color = Color.gray;
                if (UserID)
                {
                    UserID.color = Color.gray;
                }
            }
            _Outline.SetActive(showTag && !state);
        }

        public void SetZhuang(bool state)
        {
            if (_zhuang != null)
            {
                _zhuang.SetActive(state);
            }
        }

        public virtual void SetTing(bool state)
        {
            if (_ting != null)
            {
                _ting.SetActive(state);
            }
        }

        public void SetGang(int gangNum)
        {
            _addGang.TrySetComponentValue(gangNum > 0);
            switch (gangNum)
            {
                case 1:
                case 99:
                    if (_addGang)
                    {
                        _addGang.GetComponent<UISprite>().spriteName = string.Format("AddGang{0}", gangNum);
                    }
                    break;
            }
        }

        /// <summary>
        /// 显示地址信息
        /// </summary>
        public void ShowAddressInfo()
        {
            DesLabel.gameObject.SetActive(true);
            DesLabel.TrySetComponentValue(string.Format("ID:{2}\nIP:{0}\n所在地:{1}", _userInfo.ip, UserInfo.IsHasGpsInfo ? UserInfo.Country : "未提供位置信息\n请开启位置服务,并给予应用相应权限", UserInfo.id)) ;
        }

        public void ShowAutoState(bool isauto)
        {
            _auto.TrySetComponentValue(isauto);
        }

        public void DetailInfo()
        {
            if (UserInfo == null)
                return;
            UserInfoDetail.Instance.ShowInfo
              (
              UserInfo.name,
              string.Format("ID:{0}", UserInfo.id),
              string.Format("IP:{0}", UserInfo.ip),
              UserIcon.GetTexture()
              );
        }

        public void ShowVisible(bool visible)
        {
            gameObject.TrySetComponentValue(visible);
        }

    }
}