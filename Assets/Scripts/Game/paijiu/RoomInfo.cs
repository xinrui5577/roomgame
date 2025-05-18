using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.paijiu.ImgPress.Main;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
#pragma warning disable 649

namespace Assets.Scripts.Game.paijiu
{
    public class RoomInfo : MonoBehaviour
    {

        /// <summary>
        /// 显示房间ID
        /// </summary>
        [SerializeField]
        private UILabel _roomIdLabel;

        /// <summary>
        /// 显示局数
        /// </summary>
        [SerializeField]
        private UILabel _roundLabel;

        /// <summary>
        /// 显示轮数
        /// </summary>
        [SerializeField]
        private UILabel _turnLabel;

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
        private int _roomId;


        /// <summary>
        /// 房间最大局数
        /// </summary>
        public int MaxRound { get { return _maxRound; } }

        /// <summary>
        /// 房间ID
        /// </summary>
        public int RoomID { get { return _roomId; } }

        public int CurrentRound { get { return _curRound; } }



        /// <summary>
        /// 展示房间信息
        /// </summary>
        /// <param name="gameInfo"></param>
        public void ShowRoomInfo(Sfs2X.Entities.Data.ISFSObject gameInfo)
        {

            if (gameInfo.ContainsKey("rid"))
            {
                _roomId = gameInfo.GetInt("rid");
                _roomIdLabel.text = string.Format("{0}", _roomId);
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
            if (gameInfo.ContainsKey("maxfpround"))
            {
                _maxTurn = gameInfo.GetInt("maxfpround");
                _curTurn = gameInfo.GetInt("fpround");
                RefreshTurnInfo();
            }

            string rule = string.Empty;
            //游戏规则
            if (gameInfo.ContainsKey("rule"))
            {
                rule = gameInfo.GetUtfString("rule");
                InitRuleInfo(rule);
            }

            gameObject.SetActive(true);

            InitInvitBtn(rule);     //初始化微信邀请按钮

            RefreshRoomInfo();
        }

        void InitRuleInfo(string ruleInfo)
        {
            if (_ruleInfo == null)
                return;
            _ruleInfo.text = ruleInfo;
            _ruleInfo.gameObject.SetActive(true);
        }

        public void OnGameStart()
        {
            _invitButton.gameObject.SetActive(false);
        }

        private void InitInvitBtn(string rule)
        {
            if (_curRound > 0)  //已经开始游戏
            {
                _invitButton.gameObject.SetActive(false);
                return;
            }

#if  UNITY_EDITOR||UNITY_ANDROID || UNITY_IPHONE
            _invitButton.gameObject.SetActive(true);
            //微信邀请
            _invitButton.onClick.Add(new EventDelegate(() =>
            {
                YxWindowManager.ShowWaitFor();
                Facade.Instance<WeChatApi>().InitWechat();

                UserController.Instance.GetShareInfo(info =>
                {
                    YxWindowManager.HideWaitFor();

                    info.ShareData["title"] = App.GetGameData<PaiJiuGameData>().GetPlayerInfo().NickM + "-" + info.ShareData["title"];
                    info.ShareData["content"] = "[牌九]房间号:[" + _roomId + "]," + rule;  //游戏信息
                    info.ShareData["content"] += "。速来玩吧! (仅供娱乐，禁止赌博)";

                    Facade.Instance<WeChatApi>().ShareContent(info);
                });
            }));
#else 
            _invitButton.gameObject.SetActive(false);
#endif
        }

        void RefreshRoomInfo()
        {
            RefreshRoundInfo();
            RefreshTurnInfo();
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
            if (_invitButton != null)
                _invitButton.gameObject.SetActive(false);
        }
    }
}
