using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.Hall.View.RecordWindows
{
    public class DbsRecordDetialItem : YxView
    {
        public List<DbsPokerCard> Pokers;
        public UILabel ResultLabel;
        public UILabel UserIdLabel;
        public UILabel UserNameLabel;
        public UILabel UserGoldLabel;

        private void RefreshItem(Dictionary<string, object> dic)
        {
            var _data = new PokerData(dic);
            RefreshItem(_data);
        }

        private void RefreshItem(PokerData data)
        {
            UserIdLabel.TrySetComponentValue(data.UserId);
            UserNameLabel.TrySetComponentValue(data.UserName);
            UserGoldLabel.TrySetComponentValue(YxUtiles.GetShowNumber(data.UserScore).ToString());
            ResultLabel.TrySetComponentValue(data.CardsValue);
            if (Pokers != null && data.Cards != null && Pokers.Count >= data.Cards.Count)
            {
                for (int i = 0; i < data.Cards.Count; i++)
                {
                    Pokers[i].SetCardId(int.Parse(data.Cards[i].ToString()));
                    Pokers[i].SetCardFront();
                    Pokers[i].gameObject.SetActive(true);
                }
                for (int i = data.Cards.Count; i < Pokers.Count; i++)
                {
                    Pokers[i].gameObject.SetActive(false);
                }
            }
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (Data == null)
            {
                return;
            }
            if (Data is PokerData)
            {
                RefreshItem(Data as PokerData);
            }
            else if (Data is Dictionary<string, object>)
            {
                RefreshItem((Dictionary<string, object>)Data);
            }

        }

        //public void UpdateDetailData(Dictionary<string, object> dic)
        //{
        //    if (UserIdLabel != null)
        //    {
        //        UserIdLabel.text = dic.ContainsKey("id") ? dic["id"].ToString() : "";
        //    }
        //    if (UserGoldLabel != null)
        //    {
        //        UserGoldLabel.text = dic.ContainsKey("gold") ? dic["gold"].ToString() : "";
        //    }
        //    if (!dic.ContainsKey("data")) return;
        //    var temp = dic["data"];
        //    if (!(temp is Dictionary<string, object>)) return;
        //    var data = temp as Dictionary<string, object>;
        //    if (ResultLabel != null)
        //    {
        //        ResultLabel.text = data.ContainsKey("cardsValue") ? data["cardsValue"].ToString() : "";
        //    }
        //    var cards = data.ContainsKey("cards") ? data["cards"] : null;
        //    if (cards == null) return;
        //    var cardsList = cards as List<object>;
        //    if (Pokers != null && Pokers.Count >= cardsList.Count)
        //    {
        //        for (int i = 0; i < cardsList.Count; i++)
        //        {
        //            Pokers[i].SetCardId(int.Parse(cardsList[i].ToString()));
        //            Pokers[i].SetCardFront();
        //            Pokers[i].gameObject.SetActive(true);
        //        }
        //        for (int i = cardsList.Count; i < Pokers.Count; i++)
        //        {
        //            Pokers[i].gameObject.SetActive(false);
        //        }
        //    }
        //}
    }

    public class PokerData
    {
        public const string KeyId = "id";
        public const string KeyName = "name";
        public const string KeyScore = "gold";
        public const string KeyValue = "cardsValue";
        public const string KeyCards = "cards";
        public const string KeyData = "data";

        private string _userId;
        private string _userName;
        private long _userScore;
        private string _cardsValue;
        private Dictionary<string, object> _data;
        private List<object> _cards;


        public string UserId
        {
            get
            { return _userId; }
        }

        public string UserName
        {
            get { return _userName; }
        }

        public long UserScore
        {
            get { return _userScore; }
        }

        public string CardsValue
        {
            get { return _cardsValue; }
        }

        public List<object> Cards
        {
            get { return _cards; }
        }

        public PokerData(Dictionary<string, object> dic)
        {
            DealInfo(dic);
        }
        /// <summary>
        /// 茶馆头像信息兼容处理
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        public PokerData(string id, object data)
        {
            DealInfo(data as Dictionary<string, object>);
            _userId = id;
        }

        private void DealInfo(Dictionary<string, object> dic)
        {
            dic.TryGetValueWitheKey(out _userId, KeyId);
            dic.TryGetValueWitheKey(out _userName, KeyName);
            dic.TryGetValueWitheKey(out _userScore, KeyScore);
            dic.TryGetValueWitheKey(out _data, KeyData);
            if (_data == null) return;
            _data.TryGetValueWitheKey(out _cardsValue, KeyValue);
            _data.TryGetValueWitheKey(out _cards, KeyCards);
        }
    }
}