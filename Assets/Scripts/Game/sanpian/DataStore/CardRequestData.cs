using Sfs2X.Entities.Data;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.sanpian.DataStore
{
    public class CardRequestData 
    {
        public int type;
        public int[] cards;
        public int seat;
        public int cd;
        public int cardsv;
        public int carda;
        public int card4;
        public int lastv;
        public int asp;

        public static CardRequestData ParseData(ISFSObject sfsObject)
        {
            CardRequestData data = new CardRequestData
            {
                type = sfsObject.GetInt(RequestKey.KeyType),
                cards = sfsObject.GetIntArray(RequestKey.KeyCards),
                seat = sfsObject.GetInt(RequestKey.KeySeat),
                cd = sfsObject.GetInt("cd"),
                cardsv=sfsObject.GetInt("cardsv"),
                carda =sfsObject.ContainsKey("carda")? sfsObject.GetInt("carda"):-1,
                card4 = sfsObject.ContainsKey("card4") ? sfsObject.GetInt("card4") : -1,
                lastv = sfsObject.GetInt("lastv"),
                asp=sfsObject.GetInt("asp")
            };

            return data;
        }
    }
}
