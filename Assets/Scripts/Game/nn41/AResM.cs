using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.nn41
{
    public class AResM : MonoBehaviour
    {
        public UILabel NameLabel;
        public UILabel WinLabel;
        public UILabel GoldLabel;
        public List<GameObject> Cards;
        public UILabel NiuLabel;
        public UILabel NiuRate;
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
            var z = _userData.GetBool(InteractParameter.IsZ);
            ZhuangObj.gameObject.SetActive(z);
            ZhuangRate.text = z ? _userData.GetUtfString(InteractParameter.ZhuangData) : "";
            for (var i = 0; i < count; i++)
            {
                Cards[i].GetComponent<UISprite>().spriteName = "0x" + cards[i].ToString("X");
            }
            var nd = _userData.GetSFSObject(InteractParameter.NiuData);
            NiuRate.text = "x" + nd.GetInt(InteractParameter.Rate);

            if (z || nd.GetBool(InteractParameter.IsWin))
            {
                NiuRate.color = new Color32(255, 255, 255, 255);
            }
            else
            {
                NiuRate.color = new Color32(0, 240, 2, 255);
            }
            NiuLabel.text = GetNiuName(nd);
        }

        public void SetWinText(int win)
        {
            var wistr = YxUtiles.ReduceNumber(win);
            WinLabel.text = win > 0 ? string.Format("+{0}", wistr) : wistr;
        }

        private static string GetNiuName(ISFSObject responseData)
        {
            var niu = responseData.GetInt(InteractParameter.Niu);
            var type = responseData.GetInt(InteractParameter.Type);
            string[] daxie = { NiuNum.NiuZero, NiuNum.NiuOne, NiuNum.NiuTwo, NiuNum.NiuThree, NiuNum.NiuFour, NiuNum.NiuFive, NiuNum.NiuSix, NiuNum.NiuSeven, NiuNum.NiuEight, NiuNum.NiuNine, NiuNum.NiuNiu, NiuNum.FourHuaNiu, NiuNum.FiveHuaNiu, NiuNum.BombNiu, NiuNum.FiveXiaoNiu };
            niu = niu < type ? niu : type;
            var str = daxie[niu];
            return str;
        }
    }
}
