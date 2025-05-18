using Assets.Scripts.Game.lswc.Data;
using Assets.Scripts.Game.lswc.Manager;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lswc.UI.Item
{
    public class LSHistoryItem : MonoBehaviour
    {

        private Image _animal;

        private Image _banker;

        private GameObject _shanDianParent;

        private Image _num1;

        private Image _num2;

        void Start () 
        {
	        Find();
            Init();
        }

        private void Init()
        {
            _num1.gameObject.SetActive(true);
            _num2.gameObject.SetActive(true);
        }

        private void Find()
        {
            _animal = transform.FindChild("animal").GetComponent<Image>();

            _banker = transform.FindChild("maker").GetComponent<Image>();

            _shanDianParent = transform.FindChild("shandian").gameObject;

            _num1 = _shanDianParent.transform.FindChild("num1").GetComponent<Image>();

            _num2 = _shanDianParent.transform.FindChild("num2").GetComponent<Image>();

        }

        public void InitItem(LSResult history)
        {
            var gdata = App.GetGameData<LswcGameData>();
            _animal.sprite = gdata.SetHistoryAnimal(history);
            _banker.sprite = gdata.SetHistoryBanker(history.Banker);
            _shanDianParent.SetActive(history.ResultType==LSRewardType.LIGHTING);
            SetMultiple(history.Multiple);
        }

        private void SetMultiple(int multiple)
        {
            _num1.gameObject.SetActive(false);
            _num2.gameObject.SetActive(false);
            if(multiple==0)
            {
                return;
            }
            _num2.gameObject.SetActive(true);
            var gameMgr = App.GetGameManager<LswcGamemanager>();
            if(multiple>=10)
            {
                _num1.gameObject.SetActive(true);
                _num1.sprite = gameMgr.ResourseManager.GetSprite((multiple / 10).ToString());
                _num2.sprite = gameMgr.ResourseManager.GetSprite((multiple % 10).ToString());
            }
            else
            {
                _num2.sprite = gameMgr.ResourseManager.GetSprite(multiple.ToString());
            }
        }
    }
}
