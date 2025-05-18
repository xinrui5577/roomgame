using System.Collections;
using UnityEngine;
using YxFramwork.Common; 

namespace Assets.Scripts.Game.brnn3d
{
    public class CameraMove_3D : MonoBehaviour
    {
        public CameraPathBezierAnimator[] pathAnimator;

        private CameraPathBezierAnimator playAni;

        //主要包含的就是结算阶段的相关UI
        public void Move()
        {
            StartCoroutine(Wait());
        }
        IEnumerator Wait()
        {
            yield return new WaitForSeconds(8f);
            var gameMgr = App.GetGameManager<Brnn3DGameManager>();
            gameMgr.TheDownUICtrl.DownUILeftUIOn_OffClicked(false);//显示历史纪录的面板
            gameMgr.ThePaiMode.History();//游戏历史记录的显示
            gameMgr.TheZhongJiangMode.HideZhongJiangEffect();//隐藏中奖特效
            var midUiCtrl = gameMgr.TheMidUICtrl;
            midUiCtrl.TheNiuNumberUI.HideNiuNumberUI();//隐藏牛数界面
            gameMgr.ThePaiMode.DeletPaiList(); //清空牌的列表

            CameraMoveByPath(1);//3D摄像机移动照大厅
            var settleMentUI  = midUiCtrl.TheSettleMentUI;
            settleMentUI.SetSettleMentUI();//游戏结束计算面板的显示

            App.GameData.GetPlayer().UpdateView();

            settleMentUI.HideSettleMentUI();//游戏结算面板的隐藏
            gameMgr.TheBetModeMgr.TheBetMode.DeletCoinList();  //删除下注金币
            gameMgr.TheUpUICtrl.TheBankersManager.TheBankerListUI.DeleteBankerListUI();//更新前先删除
            gameMgr.TheUpUICtrl.TheBankersManager.SetBankerInfoUIData();//设置庄家的信息
        }

        //相机移动的路径
        public void CameraMoveByPath(int pathID)
        {
            if (playAni != null)
                playAni.Stop();
            playAni = pathAnimator[pathID];
            if (playAni != null)
            {
                playAni.Play();
            }
        }
    }
}

