using System.Collections;
using Assets.Scripts.Game.fillpit.ImgPress.Main;
using Assets.Scripts.Game.fillpit.skin1;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Game.fillpit.skin2
{
    public class SelfPanelSk2 : PlayerPanelSk2
    {

        public UIButton ReadyBtn;

        public UIButton StartBtn;
        /// <summary>
        /// 不能开始游戏对话框
        /// </summary>
        public UILabel CantStartLabel = null;

        //public UILabel AllCardPointLabel;

        public GameObject ShowHidePokerBtn;


        protected override void OnStart()
        {
            base.OnStart();
            ReadyBtn.onClick.Add(new EventDelegate(() =>
            {
                App.GetRServer<FillpitGameServer>().ReadyGame();
            }));

            StartBtn.onClick.Add(new EventDelegate(() =>
            {
                YxMessageBox.Show(new YxMessageBoxData()
                {
                    Msg = "点击确定后,其他玩家将无法加入游戏,确定要开始游戏么?",
                    Listener = (box, btnName) =>
                    {
                        if (btnName == YxMessageBox.BtnLeft)
                        {
                            if (App.GetGameData<FillpitGameData>().Nmno || CheckCouldStart())
                            {
                                App.GetRServer<FillpitGameServer>().SendRequest(GameRequestType.StartGame, null);
                            }
                        }
                    },
                    IsTopShow = true,
                    BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle
                });
            }));
        }


        public bool CheckCouldStart()
        {
            var dic = App.GetGameData<FillpitGameData>().UserInfoDict;
            bool couldStart = true;
            int playerCount = 1;        //场内玩家的数量,因为有自己了 所以为1

            foreach (var keyVal in dic)
            {
                if (keyVal.Key == Info.Seat)
                    continue;

                if (!keyVal.Value.State)
                {
                    couldStart = false;
                    break;
                }
                playerCount++;
            }

            if (couldStart && playerCount > 1)
            {
                return true;
            }

            _alpha = 1;
            //如果文字没显示,则开启携程
            if (!CantStartLabel.gameObject.activeSelf)
            {
                CantStartLabel.gameObject.SetActive(true);
                StartCoroutine(HideObjWithAlpha());
            }

            if (!couldStart)
            {
                CantStartLabel.text = "有玩家不在准备状态,无法开始游戏!";
            }
            else if (playerCount < 2)
            {
                CantStartLabel.text = "单个玩家无法开始游戏!";
            }
            return false;
        }

        /// <summary>
        /// 不能开始游戏的a值
        /// </summary>
        private float _alpha;

        // ReSharper disable once FunctionRecursiveOnAllPaths
        IEnumerator HideObjWithAlpha()
        {
            while (_alpha > 0)
            {
                _alpha -= Time.deltaTime / 2;

                Color color = CantStartLabel.color;
                color.a = _alpha;
                CantStartLabel.color = color;
                yield return null;
            }
            CantStartLabel.gameObject.SetActive(false);
        }

        public void SetSeePokerBtnActive(bool active)
        {
            if (ShowHidePokerBtn == null) return;
            ShowHidePokerBtn.SetActive(active);
        }

        public override void OnPlayerReady()
        {
            base.OnPlayerReady();
            SetReadyBtnActive(false);
            var gdata = App.GetGameData<FillpitGameData>();
            SetStartBtnActive(gdata.ShowStartBtn);
        }

        public override void OnGameResult(ISFSObject resultItem)
        {
            base.OnGameResult(resultItem);
//            SetSeePokerBtnActive(false);
        }


        internal override void SetReadyBtnActive(bool active)
        {
            base.SetReadyBtnActive(active);
            if (ReadyBtn == null) return;
            ReadyBtn.gameObject.SetActive(active);
        }

        internal override void SetStartBtnActive(bool active)
        {
            base.SetStartBtnActive(active);
            if (StartBtn == null) return;
            StartBtn.gameObject.SetActive(active);
        }

        //private int _allCardsPoint = -1;

        //public UILabel DifAllLabel;

        //public GameObject AllMaxMark;

        public string AllPointFormat = "+{0}";

        protected int SelfAllCardPoint;
        public override void SetAllCardPoint(int point)
        {
            if (PlayerType == 3) return;    //弃牌玩家不予处理

            SelfAllCardPoint = point;

            if(_isShow)
            {
                OnPressShowPokerBtn();
            }
        }

    
       

        public void OnPressShowPokerBtn()
        {
//            if (!ReadyState && PlayerType == 3) return;    //没有参与游戏,不显示
            if (UserBetPoker.CantShowPoint) return;        //没有发牌,不显示
            CardPoint.SetOpenCardsPoint(SelfAllCardPoint);
            CardPoint.ShowDifPointLabel();
            UserBetPoker.ShowPokerValue();
        }

        public void OnReleaseShowPokerBtn()
        {
            CardPoint.SetOpenCardsPoint(OpenCardPoint);
            CardPoint.ShowDifPointLabel();
            UserBetPoker.HidePokerValue();
        }

        private bool _isShow;

        public void OnClickShowPokerBtn()
        {
            if (_isShow)
            {
                OnReleaseShowPokerBtn();
                _isShow = false;
            }
            else
            {
                OnPressShowPokerBtn();
                _isShow = true;
            }
        }

        public override void Reset()
        {
            base.Reset();
            _isShow = false;
            SelfAllCardPoint = -1;
            ReadyBtn.gameObject.SetActive(true);
            App.GetGameManager<FillpitGameManagerSk1>().WeiChatInvite.SetWeiChatBtnActive(true);
        }

        public override void PlayerWin()
        {
            base.PlayerWin();
            Facade.Instance<MusicManager>().Play("win");
        }

    }
}