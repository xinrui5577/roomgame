using System;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Game.hg
{
    public class HgTableView : YxView
    {
        public GameObject BoxObj;
        public GameObject MusicPlay;
        public GameObject MusicMute;

        public EventObject EventObj;

        public void OnReceive(EventData data)
        {
            switch (data.Name)
            {
                case "SetMusic":
                    SetMusic();
                    break;

            }
        }


        public void SetMusic()
        {
            var music = Facade.Instance<MusicManager>().MusicVolume;
            if (Math.Abs(music - 1) <= 0)
            {
                MusicPlay.SetActive(true);
            }
            else
            {
                MusicMute.SetActive(true);
            }
        }

        public void OnMusic()
        {
            MusicPlay.SetActive(!MusicPlay.activeSelf);
            MusicMute.SetActive(!MusicMute.activeSelf);
            Facade.Instance<MusicManager>().MusicVolume = 1;
            Facade.Instance<MusicManager>().EffectVolume = 1;
        }

        public void OnMute()
        {
            MusicPlay.SetActive(!MusicPlay.activeSelf);
            MusicMute.SetActive(!MusicMute.activeSelf);
            Facade.Instance<MusicManager>().MusicVolume = 0;
            Facade.Instance<MusicManager>().EffectVolume = 0;
        }



        public void ReturnHall()
        {
            if (App.GameData.GStatus == YxEGameStatus.Normal)
            {
                App.QuitGame();
            }
            else
            {
                YxMessageBox.Show("正在游戏中,请稍后退出");
            }

        }

        public void OnShowSetting()
        {
            YxWindowManager.OpenWindow("SettingWindow");
        }

        public void OnShowRule()
        {
            YxWindowManager.OpenWindow("RuleWindow");
        }

        public void OnShowRank()
        {
            YxWindowManager.OpenWindow("RankWindow");
        }

        public void OnRule()
        {
            BoxObj.SetActive(!BoxObj.activeSelf);
        }

        public void OnApplyBank(GameObject applyBtn,GameObject giveUpBtn)
        {
            var isApplyBank = App.GetGameData<HgGameData>().ApplyBank;
            Debug.LogError("isApplyBank"+ isApplyBank);
            if (isApplyBank)
            {
                applyBtn.SetActive(false);
                giveUpBtn.SetActive(true);
                EventObj.SendEvent("GameServerEvent", "ApplyBank", null);
            }
            else
            {
                YxMessageTip.Show("您当前不具备 申请庄家的资格");
            }
        }

        public void OnGiveUpBank(GameObject applyBtn, GameObject giveUpBtn)
        {
            applyBtn.SetActive(true);
            giveUpBtn.SetActive(false);
            EventObj.SendEvent("GameServerEvent", "GiveUpBank", null);
        }
    }
}
