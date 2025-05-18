using UnityEngine;
using YxFramwork.Common; 

namespace Assets.Scripts.Game.brnn3d
{
    public class ApplyXiaZhuangMgr : MonoBehaviour
    {
        public ApplyXiaZhuangUI TheApplyXiaZhuangUI;
        //玩家上庄
        public void ApplyZhuangSendMsg()
        {
            var gdata = App.GetGameData<Brnn3dGameData>();
            var rserver = App.GetRServer<Brnn3dGameServer>();
            if (!rserver.HasGetGameInfo) return;
            if (CheckIsZhuang()) return;
            if (gdata.GetPlayerInfo().CoinA < gdata.Bkmingold)
            {
                App.GetGameManager<Brnn3DGameManager>().TheNoteUI.Note(string.Format(gdata.ShangZhuangMoneyLos, gdata.Bkmingold));
            }
            else
            {
                rserver.ApplyBanker();
            }
        }

        //玩家下庄
        public void XiaZhuangSendMsg()
        {
            if (CheckIsZhuang())
            {
                var gdata = App.GetGameData<Brnn3dGameData>();
                App.GetRServer<Brnn3dGameServer>().ApplyQuit();
                var gameMgr = App.GetGameManager<Brnn3DGameManager>();
                if (gameMgr.TheUpUICtrl.TheBankersManager.BankerIsSelf())
                {
                    gameMgr.TheNoteUI.Note(gdata.NextXiaZuang);
                }
            }
        }

        //设置申请上下装按钮的状态
        public void SetApplayXiaZhuangUIData()
        {
            SetApplyXiaZhuangUIDataEx(!CheckIsZhuang());
        }

        //判断是否是庄
        bool CheckIsZhuang()
        {
            var gdata = App.GetGameData<Brnn3dGameData>();
            if (gdata == null)
            {
                return false;
            }
            var gameMgr = App.GetGameManager<Brnn3DGameManager>();
            if (gameMgr.TheUpUICtrl.TheBankersManager.BankerIsSelf())
            {
                return true;
            }
           
            var bankList = gdata.BankList;
            if (bankList == null)
            {
                return false;
            }
            var count = bankList.Count;
            var seat = gdata.SelfSeat;
            for (var i = 0; i < count; i++)
            {
                if (bankList.GetSFSObject(i).GetInt("seat") == seat)
                {
                    return true;
                }
            }
            return false;
        }

        //设置上下庄的UI
        void SetApplyXiaZhuangUIDataEx(bool isApply)
        {
            //MusicManager.Instance.Play("UpAndDownRanker");
            if (isApply)
            {
                TheApplyXiaZhuangUI.ShowApplyZhuang();
            }
            else
            {
                TheApplyXiaZhuangUI.ShowXiaZhuang();
            }
        }
    }
}

