using UnityEngine;
using DG.Tweening;
using Sfs2X.Entities.Data;
using YxFramwork.Common;

namespace Assets.Scripts.Game.brnn3d
{
    public class BankersManager : MonoBehaviour
    {
        public BankerListUI TheBankerListUI;
        public Brnn3DPlayer Banker;
        public Transform BgTf;
        public Transform ListUITf;
        public bool IsZhanKai = true;

        public bool HasSystemBanker = false;
        //显示庄家列表的UI
        public void ShowBankListUI()
        {
            if (IsZhanKai)
                return;
            var tw = BgTf.DOLocalMoveY(-152, 0.4f);
            tw.OnComplete(delegate
                {
                    if (!ListUITf.gameObject.activeSelf)
                        ListUITf.gameObject.SetActive(true);
                    IsZhanKai = true;
                });
        }

        //隐藏庄家列表的UI
        public void HideBankListUI()
        {
            if (!IsZhanKai)
                return;
            if (ListUITf.gameObject.activeSelf)
                ListUITf.gameObject.SetActive(false);
            var tw = BgTf.DOLocalMoveY(-70, 0.4f);
            tw.OnComplete(delegate
            {
                IsZhanKai = false;
            });
        }
        private int _record = -1;

        public void SetBankerInfoUIData()
        {
            var gdata = App.GetGameData<Brnn3dGameData>();
            if (_record == -1)
            {
                _record = gdata.B;
            }

            if (_record != gdata.B)
            {
                gdata.Frame++;
                gdata.Bundle = 0;
                Banker.WinTotalCoin = 0;
            }

            _record = gdata.B;

            if (gdata.BankList == null || gdata.BankList.Size() < 1)
            {
                Banker.Info = GetSysBanker();
                return;
            }
            var old = Banker.GetInfo<Brnn3DUserInfo>();
            Banker.Info = null;
            var bankList = gdata.BankList;
            var hasBanker = false;
            foreach (ISFSObject banber in bankList)
            {
                var user = new Brnn3DUserInfo();
                user.Parse(banber);
                if (user.Seat == gdata.B)
                {
                    Banker.Info = user;
                    if (old!=null && old.Seat == gdata.B)
                    {
                        Banker.WinTotalCoin = old.WinTotalCoin;
                    }
                    hasBanker = true;
                }
                else
                {
                    var bankerListBg = App.GetGameManager<Brnn3DGameManager>().TheUpUICtrl.TheBankersManager;
                    bankerListBg.TheBankerListUI.SetBankerListUI(user.NickM, user.CoinA);
                }
            }
            if (!hasBanker)
            {
                Banker.Info = GetSysBanker();
            }
        }

        public Brnn3DUserInfo GetSysBanker()
        {
            if (!HasSystemBanker) { return null;}
            var banker = new Brnn3DUserInfo
            {
                NickM = "系统庄"
            };
            return banker;
        }

        /// <summary>
        /// 自己是不是banker
        /// </summary>
        /// <returns></returns>
        public bool BankerIsSelf()
        {
            var bankerInfo = Banker.Info;
            return bankerInfo !=null && App.GameData.SelfSeat == bankerInfo.Seat;
        }
    }
}

