using System.Collections;
using Assets.Scripts.Game.slyz.Entitys;
using Assets.Scripts.Game.slyz.Windows;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.slyz.GameScene
{
    public class LeftPanel : MonoBehaviour
    {

        public LineItemView[] LineItems;


        private bool _mIsCardsBackOk = true;

        public AwardPrizeWindow TheAwardPrizeWindow;
        // Use this for initialization
        protected void Start () {

            App.GetGameData<SlyzGameData>().GMessage.OnGameInit += OnGameInit;

            App.GetGameData<SlyzGameData>().GMessage.OnGameStart += OnGameStart;

        }

        private bool _mIsFirstEnter = true;
        private void OnGameInit()
        {
            foreach (var lineItemView in LineItems)
            {
                lineItemView.Init();
            } 

            // 点击开始按钮之后 开始把牌扣过去
            if (_mIsFirstEnter)
                _mIsFirstEnter = false;
            else
                MakeCardsBack();
        }

        private Coroutine _changeLinesCoroutine;
        private void OnGameStart()
        {
            if (_changeLinesCoroutine != null)
            {
                StopCoroutine(_changeLinesCoroutine);
            }
            _changeLinesCoroutine = StartCoroutine(ChangeLines());
        }

        private IEnumerator ChangeLines()
        {
            while (!_mIsCardsBackOk)
            {
                yield return null;
            }
            var lineWait = new WaitForSeconds(1f);
            var count = LineItems.Length-1;
            var gdata = App.GetGameData<SlyzGameData>();
            var cardValues = gdata.StartData.CardTeamList;
            var i = 0;
            for (; i < count; i++)
            {
                var item = LineItems[i];
                item.PlayRollbackCards(cardValues[i]);
                yield return lineWait;
            }
            var lastItem = LineItems[i];
            lastItem.RollbackFinishedAction = CompleteRollback;
            lastItem.PlayRollbackCards(cardValues[i]);
            yield return lineWait;
        }

        private void CompleteRollback(int index)
        {
            var gdata = App.GetGameData<SlyzGameData>();
            gdata.IsCardsForwardComplete = true;
            if (App.GetGameData<SlyzGameData>().GMessage.OnShowTotalGlod != null)
                App.GetGameData<SlyzGameData>().GMessage.OnShowTotalGlod();

            if (JudgeIsShowPrizeMask())
            {
                if (TheAwardPrizeWindow != null)
                {
                    // 判断是否该自动关闭中奖信息遮罩
                    gdata.ChangeStopAutoStart();
                    // 中奖信息显示                    
                    TheAwardPrizeWindow.Show();
                }

                gdata.IsShowPrizeMaskComplete = false;
            }
            else
            {
                gdata.IsShowPrizeMaskComplete = true;
            }
        }

        private bool JudgeIsShowPrizeMask()
        {
            var gdata = App.GetGameData<SlyzGameData>();
            if (gdata == null)
            {
                return false;
            }
            var len = (short)gdata.StartData.CardTeamList.Count;
            for (short i = 0; i < len; i++)
            {
                var team = gdata.StartData.CardTeamList[i];
                if (CardTeam.TYPE_HL <= team.type)
                {
                    return true;
                }
            }
            return false;
        }


        private void MakeCardsBack()
        {
            _mIsCardsBackOk = false;
            var count = LineItems.Length-1;
            LineItemView item = null;
            var i = 0;
            for (; i < count; i++)
            {
                item = LineItems[i];
                item.PlayCardBack();
            }
            item = LineItems[i];
            item.RollbackFinishedAction = delegate
            {
                _mIsCardsBackOk = true;
            };
            item.PlayCardBack();
        }
    }
}
