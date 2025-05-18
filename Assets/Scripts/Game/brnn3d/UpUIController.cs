using UnityEngine;
using YxFramwork.Common; 

namespace Assets.Scripts.Game.brnn3d
{
    public class UpUIController : MonoBehaviour
    {
        public BankersManager TheBankersManager;
        public CountDownUI TheCountDownUI;
        public GamesNumUI TheGamesNumUI;
        public StateUI TheStateUI;

        //返回大厅
        public void ReturnToHall()
        {
            App.QuitGameWithMsgBox();
        }
        //上庄点击
        public void ApplyZhuangClicked()
        {
            App.GetGameManager<Brnn3DGameManager>().TheApplyXiaZhuangMgr.ApplyZhuangSendMsg();
        }

        //下庄点击
        public void XiaZhuangClicked()
        {
            App.GetGameManager<Brnn3DGameManager>().TheApplyXiaZhuangMgr.XiaZhuangSendMsg();
        }

        public void BankListUIOn_OffClick()
        {
            if (TheBankersManager.IsZhanKai)
            {
                TheBankersManager.HideBankListUI();
            }
            else
            {
                TheBankersManager.ShowBankListUI();
            }
        }

    }
}

