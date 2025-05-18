using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.sanpian.DataStore
{
    public class DataParse :MonoBehaviour
    {
        public static DataParse instance;

        void Awake()
        {
            instance = this;
        }

        public  UserInfo GetUserInfo(ISFSObject userData)
        {
            UserInfo userInfo = new UserInfo
            {
                Name = userData.GetUtfString(RequestKey.KeyName),//名字
                Seat = userData.GetInt(RequestKey.KeySeat),//座位号
                Id = userData.GetInt(RequestKey.KeyId),//id
                IsReady = userData.GetBool(RequestKey.KeyState),//状态
                Gold = userData.GetLong(RequestKey.KeyTotalGold),//金币总数
                pianNum = userData.GetInt("pianNum"),//三片
                PianScore = userData.GetInt(Constants.KeyBetGold),//三片分数
                Country = userData.GetUtfString(RequestKey.KeyCountry),
                HeadImage = userData.GetUtfString("avatar"),
                cardLen = userData.GetInt("cardLen"),
                Sex = userData.GetShort("sex"),
                Ip = userData.GetUtfString("ip"),
                IsOnline = userData.GetBool("network"),
                teamId = userData.GetInt("teamId"),
                cards = userData.ContainsKey("cards")?userData.GetIntArray("cards"):null,
                mateCards = userData.ContainsKey("mateCards") ? userData.GetIntArray("mateCards") : null
            };
            userInfo.SetGpsData(userData);

            return userInfo;
        }
    }
}
