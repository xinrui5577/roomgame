using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class PlayersPnl : MonoBehaviour
    {
        public UserInfo[] Players = new UserInfo[4];
        public PlayerOther PlayerOther;

        // Use this for initialization
        void Awake()
        {
            for (int i = 1; i < Players.Length; i++)
            {
                Players[i].gameObject.SetActive(false);
                Players[i].RepairPos(i);
            }
        }

        public void ResetInfo()
        {
            for (int i = 1; i < Players.Length; i++)
            {
                Players[i].gameObject.SetActive(false);                
            }
        }

        public void SetGold(int Chair, long Gold)
        {
            Players[Chair].SetGlod(Gold);
        }

        public void AddGold(int chair, int gold)
        {
            //防止没有人的玩家位置显示杠分数字
            if(gold==0)return;

            Players[chair].AddGlod(gold);

            PlayerOther.SetUserAddGold(chair, gold);
        }

        //用艺术字显示杠分
        public void AddGoldWithWordart(int chair, int gold)
        {
            //防止没有人的玩家位置显示杠分数字
            if (gold == 0) return;

            Players[chair].AddGlod(gold);

            PlayerOther.SetUsersAddGold(chair, gold);
        }

        public void SetUserInfo(int Chair, UserData data)
        {
            if (Chair < 0) return; 
            if (data.IsNull)
            {
                Players[Chair].IsExsit = !data.IsNull;
                return;
            }

            Players[Chair].IsExsit = !data.IsNull;
            Players[Chair].SetGlod(data.Glod);
            Players[Chair].IsOutLine = data.IsOutLine;
            Players[Chair].SetSeat(data.Seat);
            Players[Chair].Name = data.name;
            Players[Chair].ID = data.id.ToString();
            Players[Chair].gameObject.SetActive(true);
        }

        public void SetUserHead(int Chair,string url,Texture img)
        {
            AsyncImage.GetInstance().SetTextureWithAsyncImage(url,Players[Chair].HeadTxtr,img);
        }

        public void SetOutLine(int Chair, bool value)
        {
            Players[Chair].IsOutLine = value;
        }

        public void SetReady(int Chair, bool value)
        {
            PlayerOther.PlayReady(Chair, value);
        }

        public virtual void SetBanker(int chair)
        {
            for (int i = 0; i < Players.Length; i++)
            {
                var player = Players[i];
                if (player)
                {
                    if (i == chair)
                    {
                        player.IsBanker = true;
                    }
                    else
                    {
                        player.IsBanker = false;
                    }
                    player.SetDirectionFlag((i- chair+4)%4);
                }
             
            }
        }

        public void SetCurr(int chair)
        {
            Players[chair].IsCurr = true;
        }

        public void SetNotCurr(int chair)
        {
            Players[chair].IsCurr = false;
        }

        public void PlayEffect(int chair, EnCpgEffect effect)
        {
            PlayerOther.PlayEffect(chair, effect);
        }

        public void HidePlayersReady()
        {
            for (int i = 0; i < UtilDef.GamePlayerCnt; i++)
            {
                PlayerOther.PlayReady(i, false);
            }
        }

        public void OnUserTalk(int chair,EnChatType type,object Content)
        {
            PlayerOther.UserTalk(chair, type, Content);
        }

        public void Reset()
        {        
        }

        public void OnUserOut(int chair)
        {
            PlayerOther.SetEmpty(chair);
            Players[chair].IsExsit = false;
        }

        public void OnUserSpeak(int chair, bool isShow)
        {
            Players[chair].IsSpeak = isShow;
        }

        public void SetTing(int currChair, bool b)
        {
            Players[currChair].IsTing = b;
        }

        //点击头像显示GPS信息
        public void OnShowGPSClick()
        {
            EventDispatch.Dispatch((int)UIEventId.ShowGPSInfo, new EventData());
        }

    }
}
