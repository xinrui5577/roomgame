using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Common.WebView
{
    /// <summary>
    /// 内嵌网页显示范围辅助类（unity中使用）
    /// </summary>
    public class WebViewParam : MonoBehaviour
    {
        [Tooltip("显示范围，锚点定左下")]
        public UISprite ShowView;
        private UIRoot _root;
        public List<EventDelegate> OnWebMove=new List<EventDelegate>();
        private int _screenWidth;
        private Vector3 _postion;
        [Tooltip("视图是否需要移动")]
        public bool IsMoving=false;

        public void StopMoving()
        {
            IsMoving = false;
        }

        void Awake()
        {
            _postion = transform.position;
        }

        private UIRoot Root
        {
            get
            {
                if (_root==null)
                {
                    _root = FindObjectOfType<UIRoot>();
                }
                return _root;
            }
        }

        private float Scale
        {
            get { return (float)_screenWidth / Root.manualWidth; }
        }
        
        public UniWebViewEdgeInsets GetShowParam()
        {
            if (ShowView == null)
            {
                return UniWebViewEdgeInsets.Zero;
            }
            _screenWidth = UniWebViewHelper.screenWidth;
            UniWebViewEdgeInsets showParame = new UniWebViewEdgeInsets((int)ShowView.topAnchor.relative, (int)ShowView.leftAnchor.relative, (int)ShowView.bottomAnchor.relative, (int)ShowView.rightAnchor.relative);
            YxDebug.LogError(string.Format("Top:{0},Left:{1} ,Bottom:{2},Right:{3}", showParame.top, showParame.left, showParame.bottom, showParame.right));
            return showParame;   
        }

        void Update()
        {
            if (IsMoving)
            {
                if (!_postion.Equals(transform.position))
                {
                    ExcuteActions();
                    _postion = transform.position;
                }
            }  
        }

        void OnDrag(Vector2 vec)
        {
            Vector2 vec3 = transform.position;
            vec3+=vec;
            transform.position = vec3;
            _postion = vec3;
            ExcuteActions();
        }

        private void ExcuteActions()
        {
            StartCoroutine(OnWebMove.WaitExcuteCalls());
        }
    }
}
