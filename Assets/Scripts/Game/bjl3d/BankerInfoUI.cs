using Sfs2X.Entities.Data;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using com.yxixia.utile.YxDebug;
using YxFramwork.Common.Model;
using YxFramwork.Enums;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.bjl3d
{
    public class BankerInfoUI : MonoBehaviour// G 11.15
    {
        private Image _rankerHeadPortraitImg;
        private Text _rankerNameText;
        private Text _rankerGameCurrencyText;
        private Text _rankerAchievementText;
        private Text _gameNumber;


        /// <summary>
        /// 获取UI操作控件
        /// </summary>
        protected void Awake()
        {
            Transform tf = transform.FindChild("RankerHeadPortrait_Img");
            if (tf == null)
                YxDebug.LogError("No Such Object");//没有该物体    
            if (tf != null) _rankerHeadPortraitImg = tf.GetComponent<Image>();
            if (_rankerHeadPortraitImg == null)
                YxDebug.LogError("No Such  Component");//没有该组件

            tf = transform.FindChild("RankerName_Text");
            if (tf == null)
                YxDebug.LogError("No Such Object");//没有该物体    
            if (tf != null) _rankerNameText = tf.GetComponent<Text>();
            if (_rankerNameText == null)
                YxDebug.LogError("No Such  Component");//没有该组件

            tf = transform.FindChild("RankerGameCurrency_Text");
            if (tf == null)
                YxDebug.LogError("No Such Object");//没有该物体    
            if (tf != null) _rankerGameCurrencyText = tf.GetComponent<Text>();
            if (_rankerGameCurrencyText == null)
                YxDebug.LogError("No Such  Component");//没有该组件


            tf = transform.FindChild("RankerAchievement_Text");
            if (tf == null)
                YxDebug.LogError("No Such Object");//没有该物体    
            if (tf != null) _rankerAchievementText = tf.GetComponent<Text>();
            if (_rankerAchievementText == null)
                YxDebug.LogError("No Such  Component");//没有该组件


            tf = transform.FindChild("RankerGameNumber_Text");
            if (tf == null)
                YxDebug.LogError("No Such Object");//没有该物体    
            if (tf != null) _gameNumber = tf.GetComponent<Text>();
            if (_gameNumber == null)
                YxDebug.LogError("No Such  Component");//没有该组件
        }

        private long _resultGold;


        private bool _isChangezhuang = true;
        private int _record = -1;
        /// <summary>
        /// 游戏运行了多少局
        /// </summary>
        public void GameInnings()
        {
            var gameCfg = App.GetGameData<Bjl3DGameData>().GameConfig;
            gameCfg.GameNum++;
            if (_isChangezhuang)
            {
                gameCfg.GameNum = 1;
                _gameNumber.text = gameCfg.GameNum + "";
            }
            else
            {
                _gameNumber.text = gameCfg.GameNum + "";
            }

        }
        /// <summary>
        ///  显示等待上庄的玩家信息
        /// </summary>
        public void ShowUserInfoUI()
        {
            var gdata = App.GetGameData<Bjl3DGameData>();
            if (_record == -1)
            {
                _record = gdata.B;
            }

            if (_record != gdata.B)
            {
                _isChangezhuang = true;
            }
            else
            {
                _isChangezhuang = false;
            }
            if (gdata.BankList == null || gdata.BankList.Size() == 0)
            {
                gdata.CurrentBanker.Seat = gdata.B;
                _rankerNameText.text = "系统庄";
                _rankerAchievementText.text = /*_resultGold +*/ "";
                _rankerGameCurrencyText.text = "∞";
                return;
            }
            var waitForRankerListUI = App.GetGameManager<Bjl3DGameManager>().TheWaitForRankerListUI;

            foreach (ISFSObject banker in gdata.BankList)
            {
                var user = new YxBaseUserInfo();
                user.Parse(banker);
                if (banker.ContainsKey("username"))
                {
                    user.NickM = banker.GetUtfString("username");
                }

                gdata.CurrentBanker.Seat = gdata.B;
                if (user.Seat == gdata.B)
                {
                    App.GameData.GStatus = YxEGameStatus.PlayAndConfine;
                    gdata.CurrentBanker = user;
                    _rankerNameText.text = user.NickM;
                    _rankerAchievementText.text = YxUtiles.GetShowNumberToString(gdata.ResultBnakerTotal);
                    _rankerGameCurrencyText.text = YxUtiles.GetShowNumberToString(user.CoinA);
                }
                else
                {
                    waitForRankerListUI.ShowRankerListUI(user.NickM, user.CoinA);
                }
            }
        }
    }
}