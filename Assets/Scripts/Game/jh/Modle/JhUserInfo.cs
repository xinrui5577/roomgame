using Assets.Scripts.Game.jh.network;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.jh.Modle
{
    public class JhUserInfo : YxBaseGameUserInfo
    {

        public bool Iskong = true;
        public string Name = "";
        public bool IsReady;
        public long Cash = 0;
        public int BetGold;

        public string Call = "";

        public int Award;
        public int Chip;


        public int WinTimes;
        public int LostTimes;
        public int TotalTimes;


        public bool IsLook;
        public bool IsGiveUp;//gameinfo 的时候为true 发牌的时候变为false
        public bool IsFail;

        public int[] Cards;


        public int WinGold;
        public int AddGold;
        public bool IsWinner;
        public bool IsGzyz;
        public bool IsShowCards;
        public int AddCnt;
        public override void Parse(ISFSObject userData)
        {
            base.Parse(userData);
            BetGold = userData.ContainsKey(JhRequestConstKey.KeyBetGold) ? userData.GetInt(JhRequestConstKey.KeyBetGold) : 0;
            CoinA -= BetGold;
            WinTimes = userData.GetInt(JhRequestConstKey.KeyWin);
            LostTimes = userData.GetInt(JhRequestConstKey.KeyLost);
            TotalTimes = userData.GetInt(JhRequestConstKey.KeyTotal);
            Iskong = false;
            Cards = userData.ContainsKey(RequestKey.KeyCards)
                ? userData.GetIntArray(RequestKey.KeyCards)
                : null;
            Award = userData.ContainsKey(JhRequestConstKey.KeyAward) ? userData.GetInt(JhRequestConstKey.KeyAward) : 0;

            Chip = App.GetGameData<JhGameTable>().IsGameStart && userData.ContainsKey(RequestKey.KeyGold) ? Mathf.Abs(userData.GetInt(RequestKey.KeyGold)) : 0;

            IsGiveUp = userData.ContainsKey(JhRequestConstKey.KeyIsGiveUp) && userData.GetBool(JhRequestConstKey.KeyIsGiveUp);
            IsLook = userData.ContainsKey(JhRequestConstKey.KeyIsLook) && userData.GetBool(JhRequestConstKey.KeyIsLook);
            IsFail = userData.ContainsKey(JhRequestConstKey.KeyIsFail) && userData.GetBool(JhRequestConstKey.KeyIsFail);
            IsPlayingGame = userData.ContainsKey(JhRequestConstKey.KeyPlaying) && userData.GetBool(JhRequestConstKey.KeyPlaying);
            IsShowCards = userData.ContainsKey("showcard") && userData.GetBool("showcard");
            IsReady = userData.ContainsKey("ready") && userData.GetBool("ready");
            IsGzyz = userData.ContainsKey("gzyz") && userData.GetBool("gzyz");
         }


        public ISFSObject GetSfsObject(RoomStatus status)
        {
            ISFSObject obj = SFSObject.NewInstance();
            if (status <= RoomStatus.CanStart)
            {
                obj.PutBool("IsReady", IsReady);
            }
            else
            {
                obj.PutBool("IsLook", IsLook);
                obj.PutBool("IsGiveUp", IsGiveUp);
                obj.PutBool("IsFail", IsFail);
                obj.PutBool("IsPlaying",IsPlayingGame);
                obj.PutBool("IsGzyz", IsGzyz);
                if ((IsLook || IsShowCards )&& Cards != null)
                {
                    obj.PutIntArray("Cards", Cards);
                }
            }

            return obj;
        }


        public void SetResult(ISFSObject userData)
        {
            AddGold = userData.GetInt("gold");
            CoinA = userData.GetLong("ttgold");
            if (userData.ContainsKey("win"))
            {
                IsWinner = true;
                WinGold = userData.GetInt("win");
            }
        }

        public ISFSObject GetResultSfsObject()
        {
            ISFSObject obj = SFSObject.NewInstance();
            
            if (!IsWinner)
            {
                if (IsFail)
                {
                    obj.PutInt("ResultType", 2);
                }
                else if (IsGiveUp)
                {
                    obj.PutInt("ResultType", 1);
                }
                else
                {
                    obj.PutInt("ResultType", 3);
                }
            }
            else
            {
                obj.PutInt("ResultType", 0);
                obj.PutInt("WinGold", WinGold);
            }
            if(Cards!=null){
                obj.PutIntArray("Cards", Cards);
            }
            obj.PutInt("AddGold", AddGold);
            obj.PutUtfString("Name", NickM);

            return obj;

        }

        public bool IsJoinGame;

        public bool IsPlaying()
        {
            return !IsFail && !IsGiveUp && IsPlayingGame;// && IsJoinGame&& IsLeaved == false
        }

        public bool ShowCards()
        {
            return (IsFail||IsGiveUp)&&!IsShowCards && IsPlayingGame;
        }


        public bool IsExist()
        {
            return !Iskong;// !IsLeaved && 
        }

        public void ResetUserStatus()
        {
            IsLook = false;
            IsFail = false;
            IsGiveUp = false;
            IsPlayingGame = false;
            IsWinner = false;
            IsReady = false;
            IsGzyz = false;
            AddCnt = 0;
        }
    }
}
