using System.Collections;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;
using YxFramwork.Manager;

namespace Assets.Scripts.Common.components
{
    /// <inheritdoc />
    /// <summary>
    /// 背景系统
    /// </summary>
    public class YxBackGround : YxView
    {
        [Tooltip("背景要显示的大小，默认取自身的Adapter")]
        public YxBaseWidgetAdapter Widget;
        [SerializeField, Tooltip("显示类型")]
        protected YxBackgroundType BgType;
        private YxBaseWidgetAdapter BackGround;
        /// <summary>
        /// 游戏列表背景
        /// </summary>
        [SerializeField, Tooltip("背景名称")]
        protected string BackGroundName = "DefBackGroun";
        [SerializeField, Tooltip("背景所在源")]
        protected string BackGroundSource = "BackGround";

        protected override void OnAwake()
        {
            base.OnAwake();
            CheckIsStart = true;
            NeedFreshWithShow = false;
            if (Widget == null)
            {
                Widget = GetComponent<YxBaseWidgetAdapter>();
            }
            if (BackGround != null)
            {
                Data = BackGroundName;
            }
        }

        public void ChangeBackGround(string bgName)
        {
            BackGroundName = bgName;
            UpdateView();
        }

        protected override void OnFreshView()
        {
            if (!string.IsNullOrEmpty(BackGroundName) && !BackGroundName.Equals(Data))
            {
                Data = BackGroundName;
                if (BackGround != null)
                {
                    Destroy(BackGround.gameObject);
                }
                var bgPrefab = ResourceManager.LoadAsset(App.GameKey, string.Format("{0}/{1}", BackGroundSource, BackGroundName), BackGroundName);
                if (bgPrefab == null) { return; }
                var go = GameObjectUtile.Instantiate(bgPrefab, transform);
                BackGround = go.GetComponent<YxBaseWidgetAdapter>();
            }
            if (BackGround == null) { return;}
            if (BgType == YxBackgroundType.Enwrap)
            {
                StartCheckEnwrap(BackGround);
            }
            else
            {
                BackGround.SetAnchor(Widget.gameObject, 0, 0, 0, 0);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (BackGround == null) { return;}
            if (BgType == YxBackgroundType.Enwrap)
            {
                StartCheckEnwrap(BackGround);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (_checkEnwrapCoroutine != null)
            {
                StopCoroutine(_checkEnwrapCoroutine);
            }
            _isChecked = false;
        }

        private bool _isChecked = false;
        private Coroutine _checkEnwrapCoroutine;
        protected void StartCheckEnwrap(YxBaseWidgetAdapter bgWidget)
        {
            if (_isChecked) return;
            _isChecked = true;
            _checkEnwrapCoroutine = StartCoroutine(CheckEnwrap(bgWidget));
        }

        private int _oldW;
        private int _oldH;
        protected IEnumerator CheckEnwrap(YxBaseWidgetAdapter bgWidget)
        {
            var wait = new WaitForEndOfFrame(); 
            while (gameObject != null && gameObject.activeSelf)
            {
                if (_oldW != Width || _oldH != Height)
                {
                    _oldW = Width;
                    _oldH = Height;
                    ChangeEnwrap(bgWidget);
                }
                yield return wait;
            }
            _isChecked = false;
        }

        /// <summary>
        /// 包裹式
        /// </summary>
        protected void ChangeEnwrap(YxBaseWidgetAdapter bgWidget)
        {
            var scW = Widget.Width;
            var scH = Widget.Height;
            var bgW = bgWidget.Width;
            var bgH = bgWidget.Height;
            var scRate = (float) scW / scH;//屏幕比率
            var bgRate = (float)bgW / bgH;//图片比率
            if (scRate >= bgRate)//屏幕比率大于图片比率
            {
                bgWidget.Width = scW;
                bgWidget.Height = (int)(scW / bgRate);
            }
            else//图片比率大于屏幕比率
            {
                bgWidget.Width = (int)(bgRate * scH);
                bgWidget.Height = scH;
            }
        }

        protected enum YxBackgroundType
        {
            /// <summary>
            /// 自由，自定义
            /// </summary>
            Free,
            /// <summary>
            /// 背景包裹着屏幕
            /// </summary>
            Enwrap
        }

        public override int Width
        {
            get { return Widget.Width; }
            set { Widget.Width = value; }
        }
        public override int Height
        {
            get { return Widget.Height; }
            set { Widget.Height = value; }
        }
    }
}
