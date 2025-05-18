using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Enums;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class YxUguiLaodingWindow : YxWindow
    {

        public Image LoadingImg;
        public override YxEUIType UIType
        {
            get
            {
                return YxEUIType.Default;
            }
        }

        protected Coroutine Loop;

        void Start()
        {
            OnShow();
        }

        protected override void OnShow()
        {
            base.OnShow();
            if (gameObject.activeInHierarchy)
            {
                if (Loop == null)
                    Loop = StartCoroutine(LoopAnimation());
            }
        }

        protected override void OnHide()
        {
            base.OnHide();
            if (Loop!=null)
            {
                StopCoroutine(Loop);
            }
        }

        protected IEnumerator LoopAnimation()
        {
            while (true)
            {
                var rote = LoadingImg.rectTransform.localRotation;
                var vec3Rote = rote.eulerAngles;
                vec3Rote += new Vector3(0, 0, 12);
                LoadingImg.rectTransform.localRotation = Quaternion.Euler(vec3Rote);

                yield return 1;
            }
            
        }
    }
}
