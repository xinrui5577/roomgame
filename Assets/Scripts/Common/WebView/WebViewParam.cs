using System;
using System.Collections.Generic;
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
        public int Border = 0;
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
            _screenWidth = UniWebViewHelper.screenWidth;
            int _screenHeight = UniWebViewHelper.screenHeight;
            int _webViewScale = UniWebViewHelper.screenScale;
            var vec = UICamera.mainCamera.WorldToScreenPoint(transform.position);
            vec=new Vector2(vec.x/ _webViewScale, vec.y/ _webViewScale);
            var size = ShowView.localSize;
            var dealSize=new Vector2(size.x* Scale, size.y*Scale);
            var top = (int)(_screenHeight - (vec.y + dealSize.y));
            int left = (int)vec.x;
            int bottom = (int)vec.y;
            int right = (int)(_screenWidth - (vec.x + dealSize.x));
            UniWebViewEdgeInsets showParame = new UniWebViewEdgeInsets(top+ Border, left+ Border, bottom+ Border, right+ Border);       
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
            if (OnWebMove != null && OnWebMove.Count > 0)
            {
                foreach (var action in OnWebMove)
                {
                    action.Execute();
                }

            }
        }
    }
}
