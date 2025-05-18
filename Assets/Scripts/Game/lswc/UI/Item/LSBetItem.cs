using Assets.Scripts.Game.lswc.Data;
using Assets.Scripts.Game.lswc.Windows;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.lswc.UI.Item
{
    /// <summary>
    /// 下注Item
    /// </summary>
    public class LSBetItem : MonoBehaviour
    {

        public int SelfIndex = 0;

        private Text _peiLv;

        private Text _betNumT;

        private GameObject OnSelect;

        private bool _isSlelect = false;

        private LSBetWindow parentWindow;

        private void Start()
        {
            Find();

            parentWindow.BetItems.Add(this);

            InitListener();
        }

        private void Find()
        {
            _peiLv = transform.FindChild("bg/PeiLv").GetComponent<Text>();

            _betNumT = transform.FindChild("bg/YaZhu").GetComponent<Text>();

            OnSelect = transform.FindChild("bg/onSelect").gameObject;

            parentWindow = GetComponentInParent<LSBetWindow>();
        }

        private void InitListener()
        {

            transform.GetChild(0).GetComponent<Button>().onClick.AddListener(OnAddBet);
        }

        public void OnAddBet()
        {
            bool success = App.GetGameData<LswcGameData>().AddBet(SelfIndex);

            App.GetGameManager<LswcGamemanager>().SystemControl.PlaySuccess(success);
            if (!success)
            {
                return;
            }
            SetSelectState();
            SetBetNumber();
        }



        public void InitItem()
        {

            _isSlelect = false;

            OnSelect.SetActive(false);

            RefreshItem();
        }

        public void RefreshItem()
        {
            SetPeiLV();

            SetBetNumber();

            SetSelectState();
        }

        private void SetPeiLV()
        {
            _peiLv.text = App.GetGameData<LswcGameData>().PeiLvs[SelfIndex].ToString();
        }

        private void SetBetNumber()
        {
            _betNumT.text = YxUtiles.GetShowNumberToString(App.GetGameData<LswcGameData>().Bets[SelfIndex]);
        }

        private void SetSelectState()
        {
            if (App.GetGameData<LswcGameData>().Bets[SelfIndex] > 0)
            {
                _isSlelect = true;
            }
            else
            {
                return;
            }
            if (_isSlelect && !OnSelect.activeInHierarchy)
            {
                OnSelect.SetActive(true);
            }
        }

    }
}
