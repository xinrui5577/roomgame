using Assets.Scripts.Game.lswc.Data;
using Assets.Scripts.Game.lswc.UI.Item;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.lswc.Windows
{
    /// <summary>
    /// 下注面板
    /// </summary>
    public class LSBetWindow : MonoBehaviour
    {

        public UITweener Tween;

        public Text TotalGold;

        public Text TotalBet;

        public Image ChangeAnteBtnImage;

        public Text ChangeAnteText;

        [HideInInspector]
        public List<LSBetItem> BetItems;

        private bool _isShow=false;

        private void Awake()
        {
            BetItems = new List<LSBetItem>();
        }

        private void Start()
        {
           FindChild();
           Tween.SetOnFinished(OnHideFinished);
        }

        void FindChild()
        {
            Tween = transform.FindChild("LeftBottom").GetComponent<UITweener>();
            TotalGold = transform.FindChild("LeftBottom/TotalGold").GetComponent<Text>();
            ChangeAnteBtnImage= transform.FindChild("LeftBottom/ChangeAnteBtn").GetComponent<Image>();
            TotalBet = transform.FindChild("LeftBottom/TotalBet").GetComponent<Text>();
            ChangeAnteText = transform.FindChild("LeftBottom/ChangeAnteBtn/Text").GetComponent<Text>();
        }

        public void SetBetWindow()
        {
            foreach (var lsBetItem in BetItems)
            {
                lsBetItem.InitItem();
            }
            SetTotalBets();
            SetTotalGold();
        }

        public void SetTotalGold()
        {
            var gold = App.GetGameData<LswcGameData>().TotalGold - App.GetGameData<LswcGameData>().TotalBets;
            TotalGold.text = YxUtiles.GetShowNumberToString(gold);
        }

        public void SetTotalBets()
        {
            TotalBet.text = YxUtiles.GetShowNumberToString(App.GetGameData<LswcGameData>().TotalBets);
        }

        public void Show()
        {
            _isShow = true;
            gameObject.SetActive(true);
            Tween.PlayForward();
            ChangeAnte();
        }

        public void Hide()
        {
            _isShow = false;
            Tween.PlayReverse();
        }

        private void OnHideFinished()
        {
            if (!_isShow)
            {
                gameObject.SetActive(false);
            }   
        }

        public void RefreshItems()
        {
            foreach (var lsBetItem in BetItems)
            {
                lsBetItem.RefreshItem();
            }
        }

        public void ChangeAnte()
        {
            ChangeAnteText.text = YxUtiles.ReduceNumber(App.GetGameData<LswcGameData>().GetNowAnte());
            ChangeAnteBtnImage.overrideSprite = App.GetGameData<LswcGameData>().GetNowAnteSprite();
        }
    }
}
