using Assets.Scripts.Common.Utils;
using UnityEngine;

// ReSharper disable FieldCanBeMadeReadOnly.Local
#pragma warning disable 649
namespace Assets.Scripts.Game.duifen
{
    public class RoomInfo : MonoBehaviour {

        /// <summary>
        /// 显示房间ID
        /// </summary>
        [SerializeField]
        private UILabel _roomIdLabel = null;

        /// <summary>
        /// 显示局数
        /// </summary>
        [SerializeField]
        private UILabel _roundLabel = null;

        /// <summary>
        /// 显示轮数
        /// </summary>
        [SerializeField]
        private UILabel _turnLabel = null;

        /// <summary>
        /// 微信邀请按钮
        /// </summary>
        [SerializeField]
        private UIButton _invitButton;

        /// <summary>
        /// 游戏规则
        /// </summary>
        [SerializeField]
        private UILabel _ruleInfo;


        /// <summary>
        /// 最大轮数
        /// </summary>
        private int _maxTurn;

        /// <summary>
        /// 当前轮数
        /// </summary>
        private int _curTurn;

        /// <summary>
        /// 本房间最大局数
        /// </summary>
        private int _maxRound;

        /// <summary>
        /// 本房间当前局数
        /// </summary>
        private int _curRound;

        /// <summary>
        /// 本房间游戏房间号
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private int _roomID;

   

        /// <summary>
        /// 房间最大局数
        /// </summary>
        public int MaxRound { get { return _maxRound; } }

        /// <summary>
        /// 房间ID
        /// </summary>
        public int RoomID { get { return _roomID; } }

        public int CurrentRound { get { return _curRound; } }

        

        /// <summary>
        /// 展示房间信息
        /// </summary>
        /// <param name="gameInfo"></param>
        public void ShowRoomInfo(Sfs2X.Entities.Data.ISFSObject gameInfo)
        {

            if (gameInfo.ContainsKey("rid"))
            {
                _roomID = gameInfo.GetInt("rid");
                _roomIdLabel.text = _roomID.ToString();
                _roomIdLabel.gameObject.SetActive(true);
            }

            //设置局数信息
            if (gameInfo.ContainsKey("maxRound"))
            {
                _maxRound = gameInfo.GetInt("maxRound");
                _curRound = gameInfo.GetInt("round");
                RefreshRoundInfo();
            }

            //设置轮数信息
            if(gameInfo.ContainsKey("maxfpround"))
            {
                _maxTurn = gameInfo.GetInt("maxfpround");
                _curTurn = gameInfo.GetInt("fpround");
                RefreshTurnInfo();
            }

            //游戏规则
            if (gameInfo.ContainsKey("rule"))
            {
                InitRuleInfo(gameInfo.GetUtfString("rule"));
            }

            gameObject.SetActive(true);

            RefreshRoomInfo();

            //InitInvitBtn();     //初始化微信邀请按钮
        }

        void InitRuleInfo(string ruleInfo)
        {
            _ruleInfo.text = ruleInfo;
            _ruleInfo.gameObject.SetActive(true);
        }


        public void InitInvitBtn()
        {
            if (_curRound > 0)  //已经开始游戏
            {
                _invitButton.gameObject.SetActive(false);
                return;
            }

#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE
            _invitButton.gameObject.SetActive(true);
            //微信邀请
            _invitButton.onClick.Add(new EventDelegate(OnClickChatInvitBtn));
#else
                _invitButton.gameObject.SetActive(false);
#endif
        }

        void RefreshRoomInfo()
        {
            RefreshRoundInfo();
            RefreshTurnInfo();
        }

        public void OnClickChatInvitBtn()
        {
            YxTools.ShareFriend(_roomIdLabel.text, _ruleInfo.text);
        }

        /// <summary>
        /// 更新当前局数
        /// </summary>
        public void SetCurRound(int round)
        {
            _curRound = round;
            RefreshRoundInfo();
        }

        public void RefreshRoundInfo()
        {
            _roundLabel.text = string.Format("{0} / {1}", _curRound, _maxRound);
        }

        public void SetCurTurn(int turn)
        {
            _curTurn = turn;
            RefreshTurnInfo();
        }

        public void RefreshTurnInfo()
        {
            _turnLabel.text = string.Format("{0} / {1}", _curTurn, _maxTurn);
        }


        internal void HideInvitBtn()
        {
            if(_invitButton != null)
                _invitButton.gameObject.SetActive(false);
        }
    }
}
