using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.brnn3d
{
    public class DownUIController : MonoBehaviour
    {
        public DownUILeftBg TheDownUILeftBg;
        public DownUILeftUIOn_Off TheDownUILeftUIOnOff;
        public LuziInfoUI TheLuziInfoUI;
        public ResetChip CoinBtnUI;
 
        public void ResetChip()
        {
            CoinBtnUI.InitChipBtns(App.GameData.AnteRate);
        }

        //路子信息左点击
        public void LuziLeftBtn()
        {
            var gameMgr = App.GetGameManager<Brnn3DGameManager>();
            gameMgr.ThePaiMode.SetLuziInfoUIData(-1);
        }

        //路子信息右点击
        public void LuziRightBtn()
        {
            var gameMgr = App.GetGameManager<Brnn3DGameManager>();
            gameMgr.ThePaiMode.SetLuziInfoUIData(1);
        }

        //显示历史纪录的面板
        public void DownUILeftUIOn_OffClicked(bool isKai)
        {
            App.GetGameData<Brnn3dGameData>().IsKai = isKai;
            DownUILeftUIOn_OffClicked();
        }

        public void DownUILeftUIOn_OffClicked()
        {
            if (App.GetGameData<Brnn3dGameData>().IsKai)
            {
                TheDownUILeftBg.HideDownUILeftBg();
                TheDownUILeftUIOnOff.ShowKaiBtn();
                App.GetGameData<Brnn3dGameData>().IsKai = false;
            }
            else
            {
                TheDownUILeftBg.ShowDownUILeftBg();
                TheDownUILeftUIOnOff.ShowGuanBtn();
                App.GetGameData<Brnn3dGameData>().IsKai = true;
            }
        }
    }


}

