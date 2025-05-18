using System.Collections.Generic;
using YxFramwork.Framework.Core;
using YxFramwork.Common.Utils;
using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;
using YxFramwork.Manager;
using YxFramwork.Common;
using YxFramwork.Tool;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    /// <summary>
    /// 框架功能接口类   
    /// </summary>
    public partial class MahjongUtility
    {
        public static string GameKey { get { return App.GameKey; } }

        public static void ReturnToHall() { App.QuitGame(); }

        public static int GetChair(ISFSObject data)
        {
            if (data != null)
            {
                int seat = data.TryGetInt(RequestKey.KeySeat);
                return GetChair(seat);
            }
            return 0;
        }

        public static int GetChair(int seat)
        {
            var gameData = App.GetGameData<YxGameData>();
            if (!gameData.ExIsNullOjbect())
            {
                return gameData.GetLocalSeat(seat);
            }
            return 0;
        }

        public static int GetChairOnCpg(int seat, int chair)
        {
            int fromInt = 0;
            fromInt = GetChair(seat);
            fromInt = (fromInt - chair + GameCenter.DataCenter.MaxPlayerCount) % GameCenter.DataCenter.MaxPlayerCount;
            switch (GameCenter.DataCenter.MaxPlayerCount)
            {
                case 2:
                    if (seat == 1 || seat == 0) fromInt = 2;
                    break;
                case 3:
                    fromInt = fromInt == 2 && chair == 0 ? 3 : fromInt;
                    fromInt = chair == 3 || chair == 1 ? fromInt + 1 : fromInt;
                    break;
            }
            return fromInt;
        }

        public static YxGameData GetYxGameData()
        {
            return App.GetGameData<YxGameData>();
        }

        public static AudioClip PlaySound(string name, string source)
        {
            return Facade.Instance<MusicManager>().Play(name, source);
        }

        public static AudioClip PlayEnvironmentSound(string sound)
        {
            return Facade.Instance<MusicManager>().Play(sound);
        }

        public static void SetMusicVolume(float value)
        {
            Facade.Instance<MusicManager>().MusicVolume = value;
        }

        public static float GetMusicVolume()
        {
            return Facade.Instance<MusicManager>().MusicVolume;
        }

        public static void SetEffectVolume(float value)
        {
            Facade.Instance<MusicManager>().EffectVolume = value;
        }

        public static float GetEffectVolume()
        {
            return Facade.Instance<MusicManager>().EffectVolume;
        }

        public static long GetShowNumber(long value)
        {
            return (long)YxUtiles.GetShowNumber(value);
        }

        public static float GetShowNumberFloat(long value)
        {
            return (float)YxUtiles.GetShowNumber(value);
        }

        public static void QuitGame()
        {
            App.QuitGame();
        }

        public static void SendAction(string mainCode,
            Dictionary<string, object> parm,
            TwCallBack callBack,
            bool hasMsgBox = true,
            TwCallBack errMsgEvent = null,
            bool hasWait = true,
            string cacheKey = null)
        {
            Facade.Instance<TwManager>().SendAction(mainCode, parm, callBack, hasMsgBox, errMsgEvent, hasWait, cacheKey);
        }
    }
}