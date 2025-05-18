using System;
using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Game.car
{
    public class CarTableView : YxView
    {
        public EventObject EventObj;
        public UISlider MusicSlider;
        public UISlider SoundSlider;

        public YxNguiWindow PullDownWindow;

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
            var sound = Facade.Instance<MusicManager>().EffectVolume;
            MusicSlider.value = Math.Abs(music - 1) <= 0 ? 1 : 0;
            SoundSlider.value = Math.Abs(sound - 1) <= 0 ? 1 : 0;
        }

        public void OnMusic(UISlider slider)
        {
            Facade.Instance<MusicManager>().MusicVolume = slider.value > 0 ? 0 : 1;
        }

        public void OnSound(UISlider slider)
        {
            Facade.Instance<MusicManager>().EffectVolume = slider.value > 0 ? 0 : 1;
        }


        public void OnPullDown(UISprite obj)
        {
            if (obj.spriteName.Equals("vbcbm_btn_xiala2"))
            {
                obj.spriteName = "BTN-shezhi1";
                obj.GetComponent<UIButton>().normalSprite= "BTN-shezhi1";
                PullDownWindow.Show();
            }
            else
            {
                obj.spriteName = "vbcbm_btn_xiala2";
                obj.GetComponent<UIButton>().normalSprite = "vbcbm_btn_xiala2";
                PullDownWindow.Hide();
            }
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

        public void OnApplyBank(GameObject applyBtn,GameObject giveUpBtn)
        {
            var isApplyBank = App.GetGameData<CarGameData>().ApplyBank;
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
