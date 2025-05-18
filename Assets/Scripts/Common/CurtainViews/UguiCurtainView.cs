using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common.Utils;
using YxFramwork.Manager;
using YxFramwork.View;
using com.yxixia.utile.YxDebug;
using YxFramwork.Enums;

namespace Assets.Scripts.Common.CurtainViews
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class UguiCurtainView : CurtainView
    {
        public Slider ProSlider;
        public Text Label;
        public CanvasGroup Background;
        public Text TipLabel;
        public float HideSpeed = 0.05f;
        public override void FreshCamera()
        {
            var curCamera = Util.GetMainCamera("Untagged");
            if (curCamera == null) return;
            var curCanvas = GetComponent<Canvas>(); 
            curCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            curCanvas.worldCamera = YxWindowManager.GetCamera();
            curCanvas.sortingOrder = (int)YxWindowManager.YxWinLayer.LoadingLayer;
            curCanvas.planeDistance = curCamera.nearClipPlane+1;
            YxDebug.Log(string.Format("loading设置在:{0}", curCanvas.planeDistance));
        }
         
        protected override void UpdateInfo(float progress, string msg)
        {
            if (ProSlider!=null) ProSlider.value = progress;
            if (Label!= null) Label.text = msg;
        }
         
        public override IEnumerator CurtainCall()
        {
            if (Background == null) yield break;
            if (ProSlider!=null) ProSlider.gameObject.SetActive(false);
            var alpha = Background.alpha;
            while (alpha>0f)
            {
                alpha -= HideSpeed;
                Background.alpha = alpha;
                yield return new WaitForFixedUpdate();
            }
        }

        protected override void FreshTip(string tipMsg)
        {
            TipLabel.text = tipMsg;
        }

        public override IEnumerator ReadyCurtainCall()
        {
            yield break;
        }
    }
}
