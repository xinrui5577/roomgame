using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Assets.Scripts.Game.fruit
{
    public class EventTriggerListener : EventTrigger
    {
        public delegate void VoidDelegate(GameObject go);

        public VoidDelegate OnClick;
        public VoidDelegate OnDown;
        public VoidDelegate OnEnter;
        public VoidDelegate OnExit;
        public VoidDelegate OnUp;
        public VoidDelegate onSelect;
        public VoidDelegate OnUpdateSelect;

        public static EventTriggerListener Get(GameObject go)
        {
            if (go == null)
            {
                return null;
            }
            var listener = go.GetComponent<EventTriggerListener>() ?? go.AddComponent<EventTriggerListener>();
            return listener;
        }
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (OnClick != null) OnClick(gameObject);
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (OnDown != null) OnDown(gameObject);

            m_OnLongpress.Invoke();

            isPointDown = true;

            lastInvokeTime = Time.time;
        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (OnEnter != null) OnEnter(gameObject);
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            if (OnExit != null) OnExit(gameObject);

            isPointDown = false;
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (OnUp != null) OnUp(gameObject);
            isPointDown = false;
        }
        public override void OnSelect(BaseEventData eventData)
        {
            if (onSelect != null) onSelect(gameObject);
        }
        public override void OnUpdateSelected(BaseEventData eventData)
        {
            if (OnUpdateSelect != null) OnUpdateSelect(gameObject);
        }

        //长按事件处理
        public float interval = .03f;

        [SerializeField]
        UnityEvent m_OnLongpress = new UnityEvent();


        private bool isPointDown = false;
        private float lastInvokeTime;
        private float longPressedUpdate;

        // Use this for initialization
        void Start()
        {
            longPressedUpdate = Time.time;
        }

        private bool isPressed;

        // Update is called once per frame
        void Update()
        {
            if (isPointDown)
            {
                if (Time.time - lastInvokeTime > interval)
                {
                    //Debug.Log("m_OnLongpress");
                    isPressed = true;

                    //触发点击;
                    m_OnLongpress.Invoke();
                    lastInvokeTime = Time.time;
                }
            }
            else
            {
                isPressed = false;
                longPressedUpdate = Time.time;
            }

            LongPressedFunc();
        }

        void LongPressedFunc()
        {
            if (isPressed && Time.time - longPressedUpdate > .3f && gameObject.GetComponent<Settings>() && gameObject.GetComponent<Settings>().admitLongPressed)
            {
                if (OnClick != null) OnClick(gameObject);
            }
        }
    }
}

