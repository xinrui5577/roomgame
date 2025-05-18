using System.Collections.Generic;
using Assets.Scripts.Game.Fishing.entitys;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.Fishing.windows
{
    public class ModelDisplayView : YxView
    {
        private BoxCollider _boxCollider;
        [SerializeField]
        private Camera _camera;
        public GraphicRaycaster UiRaycaster;

        public Transform Dot;


        protected override void OnAwake()
        {
            base.OnAwake();
            _boxCollider = GetComponent<BoxCollider>();
        }

        protected override void OnStart()
        {
            base.OnStart();
            _camera = App.UI.GetWindowCamera();
        }

        private bool IsMouseDown;
        private float _startMouseX;
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (_camera == null) return;
                var mousePosition = Input.mousePosition;
                var ray = _camera.ScreenPointToRay(mousePosition);
                var eventData = new PointerEventData(EventSystem.current)
                {
                    pressPosition = mousePosition,
                    position = mousePosition
                };
                var list = new List<RaycastResult>();
                UiRaycaster.Raycast(eventData, list);
                if (list.Count > 0 && list[0].gameObject == gameObject)
                {
                    _startMouseX = mousePosition.x;
                    IsMouseDown = true;
                }
            }else if (Input.GetMouseButtonUp(0))
            {
                IsMouseDown = false;
            }
            if (IsMouseDown)
            {
                var angles = Dot.localEulerAngles;
                var newX = Input.mousePosition.x;
                angles.y -= newX - _startMouseX;
                _startMouseX = newX;
                Dot.localEulerAngles = angles;
            }
        }
    }
}
