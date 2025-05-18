using YxFramwork.Common.Utils;
using YxFramwork.Framework;
using Sfs2X.Entities.Data;
using YxFramwork.ConstDefine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public static class GameFrameworkExtension
    {
        /// <summary>
        /// table座位号 转换 client座位号
        /// 2人场 | tableChair: 0 2     --->  chair：0 1
        /// 3人场 | tableChair: 0 1 3   --->  chair：0 1 2 
        /// 4人场 | tableChair: 0 1 2 3 --->  chair：0 1 2 3
        /// </summary>
        /// <param name="gameChair">牌桌座位号</param>
        /// <returns></returns>
        public static int ExChairT2C(this int tableChair)
        {
            var playerCount = GameCenter.DataCenter.MaxPlayerCount;
            switch (playerCount)
            {
                case 2: if (tableChair == 2) return 1; break;
                case 3: if (tableChair == 3) return 2; break;
            }
            return tableChair;
        }

        /// <summary>
        /// client 转换 table
        /// 2人场 | chair：0 1     ---> tableChair: 0 2
        /// 3人场 | chair：0 1 2   ---> tableChair: 0 1 3
        /// 4人场 | chair：0 1 2 3 ---> tableChair: 0 1 2 3
        /// </summary>
        /// <param name="gameChair">客户端座位号</param>
        /// <returns></returns>
        public static int ExChairC2T(this int gameChair)
        {
            var playerCount = GameCenter.DataCenter.MaxPlayerCount;
            switch (playerCount)
            {
                case 2: if (gameChair == 1) return 2; break;
                case 3: if (gameChair == 2) return 3; break;
            }
            return gameChair;
        }

        /// <summary>
        /// server座位号 转换 table座位号
        /// </summary>
        /// <param name="seat">服务器座位号</param>
        /// <returns></returns>
        public static int ExChairS2T(this int seat)
        {
            return seat.ExSeatS2C().ExChairC2T();
        }

        /// <summary>
        /// server座位号 转换 client座位号
        /// </summary>
        public static int ExSeatS2C(this int seat)
        {
            if (GameCenter.Instance.GameType == GameType.Normal)
            {
                var gameData = App.GetGameData<YxGameData>();
                return gameData != null ? gameData.GetLocalSeat(seat) : 0;
            }
            else
            {
                return seat;
            }
        }

        /// <summary>
        /// server座位号 转换 client座位号
        /// </summary>
        public static int ExSeatS2C(this ISFSObject data)
        {
            return data != null ? data.TryGetInt(RequestKey.KeySeat).ExSeatS2C() : 0;
        }

        public static T GetPlayerInfoItem<T>(this YxGameData GameData, int chair) where T : YxBaseGamePlayer
        {
            var item = GameData.PlayerList[chair];
            if (null != item)
            {
                return item as T;
            }
            return null;
        }

        public static bool TryGetBool(this ISFSObject data, string key)
        {
            if (data.ContainsKey(key)) { return data.GetBool(key); }
            return false;
        }

        public static long TryGetLong(this ISFSObject data, string key)
        {
            if (data.ContainsKey(key)) { return data.GetLong(key); }
            return 0;
        }

        public static float TryGetFloat(this ISFSObject data, string key)
        {
            if (data.ContainsKey(key)) { return data.GetFloat(key); }
            return 0;
        }

        public static int TryGetInt(this ISFSObject data, string key)
        {
            if (data.ContainsKey(key)) { return data.GetInt(key); }
            return 0;
        }

        public static string TryGetString(this ISFSObject data, string key)
        {
            if (data.ContainsKey(key)) { return data.GetUtfString(key); }
            return "";
        }

        public static int[] TryGetIntArray(this ISFSObject data, string key)
        {
            if (data.ContainsKey(key)) { return data.GetIntArray(key); }
            return null;
        }

        public static ISFSArray TryGetSFSArray(this ISFSObject data, string key)
        {
            if (data.ContainsKey(key)) { return data.GetSFSArray(key); }
            return null;
        }

        public static ISFSObject TryGetSFSObject(this ISFSObject data, string key)
        {
            if (data.ContainsKey(key)) { return data.GetSFSObject(key); }
            return null;
        }
    }
}