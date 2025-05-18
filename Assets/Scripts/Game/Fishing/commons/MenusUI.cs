using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Fishing.commons
{
    // ReSharper disable once InconsistentNaming
    public class MenusUI : MonoBehaviour
    {
        /// <summary>
        /// 双倍
        /// </summary>
        /// <param name="toggle"></param>
        public void OnChangeBarrelTypeClick(Toggle toggle)
        {
            var gMgr = App.GetGameManager<FishingGameManager>();
            if (gMgr == null) return;
            var operationManager = gMgr.OperationManager;
            operationManager.OnChangeBarrelTypeClick(toggle);
        }

        /// <summary>
        /// 锁定
        /// </summary>
        /// <param name="toggle"></param>
        public void OnChangeLockClick(Toggle toggle)
        {
            var gMgr = App.GetGameManager<FishingGameManager>();
            if (gMgr == null) return;
            var operationManager = gMgr.OperationManager;
            operationManager.OnChangeLockClick(toggle);
        }


        /// <summary>
        /// 退出
        /// </summary>
        public void OnQuitGame()
        {
            App.QuitGameWithMsgBox();
        }
    }
}
