using System.Collections.Generic;
using DragonBones;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.car
{
    public class CarResultItemView : YxView
    {
        public UISprite WinCarIcon;
        public UISprite WinAnte;
        public UnityArmatureComponent PlayAni;
        public List<int> Antes = new List<int>();
        public CarResultUserItem MySelfResultUserItem;
        public CarResultUserItem BankResultUserItem;
        public List<CarResultUserItem> RankResultUserItems;
        public List<GameObject> ShowCars;

        private CarGameData _gadta
        {
            get { return App.GetGameData<CarGameData>(); }
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var resultData = Data as ResultData;
            SetView(resultData);
        }

        public void SetView(ResultData resultData)
        {
            PlayAni.animation.Play("start");
            WinCarIcon.spriteName = string.Format("vbcbm_win_{0}", resultData.Car);
            WinCarIcon.MakePixelPerfect();

            WinAnte.spriteName = string.Format(resultData.Win >= 0 ? "vbcbm_result_win_race_{0}" : "vbcbm_result_lost_race_{0}", Antes[resultData.Car]);
            WinAnte.MakePixelPerfect();

            for (int i = 0; i < ShowCars.Count; i++)
            {
                ShowCars[i].SetActive(false);
            }
            ShowCars[resultData.Car].SetActive(true);
            var index = 0;

            for (int i = 0; i < resultData.Playerlist.Count; i++)
            {
                CarResultUserItem resultUserItem;

                var userData = resultData.Playerlist[i];
                if (userData.Seat == _gadta.GetPlayerInfo(0).Seat)
                {
                    resultUserItem = MySelfResultUserItem;
                }
                else if (userData.Seat == _gadta.BankPlayer.Info.Seat)
                {
                    resultUserItem = BankResultUserItem;
                }
                else
                {
                    resultUserItem = RankResultUserItems[index];
                }
                resultUserItem.SetData(userData.UserName, userData.Win);
            }
        }
    }
}
