using System.Collections;
using Assets.Scripts.Game.ddz2.InheritCommon;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.View;

namespace Assets.Scripts.Game.ddz2.DDz2Common
{

    public class MessageBoxScroll : MonoBehaviour
    {

        /// <summary>
        /// 显示的内容宽度
        /// </summary>
        [SerializeField] protected float ViewContentWidth = 600;

        [SerializeField] protected TweenWidth TweenWidth;

        [SerializeField] protected UIPanel UiPanel;

        private IEnumerator StartShowUiContent()
        {
            var panelx = UiPanel.baseClipRegion.x;
            var panely = UiPanel.baseClipRegion.y;
            var panelw = UiPanel.baseClipRegion.w;

            while (true)
            {
                yield return new WaitForEndOfFrame();
                UiPanel.baseClipRegion = new Vector4(panelx, panely, TweenWidth.value, panelw);
                if (UiPanel.baseClipRegion.z > ViewContentWidth) yield break;
            }
        }

        protected void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(StartShowUiContent());
            TweenWidth.SetOnFinished(() =>
                {
                    UiPanel.baseClipRegion = new Vector4(UiPanel.baseClipRegion.x, UiPanel.baseClipRegion.y, 600, UiPanel.baseClipRegion.w);
                });
            TweenWidth.PlayForward();
        }

        protected void OnDisable()
        {
            TweenWidth.ResetToBeginning();
            UiPanel.baseClipRegion = new Vector4(UiPanel.baseClipRegion.x, UiPanel.baseClipRegion.y, 30, UiPanel.baseClipRegion.w);
        }

        /// <summary>
        /// 当点击关闭按钮
        /// </summary>
        public void OnCloseBtnClick()
        {
            gameObject.SetActive(false);
        }

        public void OpenMessageBox()
        {
            gameObject.SetActive(true);
        }

        public void HideMessageBox()
        {
            gameObject.SetActive(false);
        }

        public void OnChangeRoomBtnClick()
        {
            var gdata = App.GetGameData<DdzGameData>();
            if (gdata.IsGameStart || gdata.AllReady())
            {
                YxDebug.Log("正在游戏中,无法更换房间!");
                YxMessageBox.Show(new YxMessageBoxData
                {
                    Msg = "正在游戏中,无法更换房间!",
                    Delayed = 5,
                });
            }
            else
            {
                YxDebug.Log("正在更换房间....");
                App.GetRServer<DdzGameServer>().ChangeRoom();
            }
        }

        public void OnLeaveBtnClick()
        {
            YxDebug.LogError(" Try to Leave Room");
            var gdata = App.GetGameData<DdzGameData>();
            if (gdata.IsGameStart || gdata.AllReady())
            {
                YxDebug.LogError(" Is started , can't Leave Room");
                YxMessageBox.Show("游戏还在进行中，请结束游戏后再退出！！！");
            }
            else
            {
                YxDebug.LogError(" Leave Room , successfully!");
                App.QuitGameWithMsgBox();
            }
        }

    }
}
