using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.Ttzkf
{
    public class AResM : MonoBehaviour
    {
        public UILabel NameLabel;
        public UILabel WinLabel;
        public UILabel GoldLabel;
        public List<GameObject> Cards;
        public UILabel NiuLabel;
        public Transform ZhuangObj;
        public UILabel ZhuangRate;

        private int _win;
        private int _isUpdataUserDateFlag;
        private ISFSObject _userData;
        protected void Awake()
        {
            _isUpdataUserDateFlag++;
        }

        protected void Start()
        {
            UpdateUserData();
        }

        public void SetData(ISFSObject userData)
        {
            _userData = userData;
            _isUpdataUserDateFlag++;
            UpdateUserData();
        }

        private void UpdateUserData()
        {
            if (_isUpdataUserDateFlag < 2) return;
            _isUpdataUserDateFlag = 1;
            _win = _userData.ContainsKey(InteractParameter.Gold) ? _userData.GetInt(InteractParameter.Gold) : 0;
            NameLabel.text = _userData.GetUtfString(InteractParameter.Nick);

            SetWinText(_win);
            var cards = _userData.GetIntArray(InteractParameter.Cards);
            var count = cards.Length < Cards.Count ? cards.Length : Cards.Count;
            var z = _userData.GetBool(InteractParameter.IsBanker);
            ZhuangObj.gameObject.SetActive(z);
            ZhuangRate.text = z ? _userData.GetUtfString(InteractParameter.ZhuangData) : "";
            for (var i = 0; i < count; i++)
            {
                Cards[i].GetComponent<UISprite>().spriteName = "A_" + cards[i];
            }
            var nd = _userData.GetSFSObject(InteractParameter.NiuData);
            NiuLabel.text = GetNiuName(nd);
        }

        public void SetWinText(int win)
        {
            var wistr = YxUtiles.ReduceNumber(win);
            WinLabel.text = win > 0 ? string.Format("+{0}", wistr) : wistr;
        }

        private static string GetNiuName(ISFSObject responseData)
        {
            var value = responseData.GetInt(InteractParameter.Value);
            var type = responseData.GetInt(InteractParameter.Type);
            string[] daxie = { CountNum.ZeroCount, CountNum.OneCount, CountNum.TwoCount, CountNum.ThreeCount, CountNum.FourCount, CountNum.FiveCount, CountNum.SixCount, CountNum.SevenCount, CountNum.EightCount, CountNum.NineCount, CountNum.ErBaGang, CountNum.DuiZi, CountNum.TianDui };
            var str = "";
            switch (type)
            {
                case 0:
                    str = daxie[0];
                    break;
                case 1:
                case 2:
                    str = daxie[value];
                    break;
                case 3:
                    str = daxie[10];
                    break;
                case 4:
                    str = daxie[11];
                    break;
                case 5:
                    str = daxie[12];
                    break;
            }
            return str;
        }
    }
}
