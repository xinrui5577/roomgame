using System;
using UnityEngine;

namespace Assets.Scripts.Hall.View.RecordWindows
{
    public class RecordTabItem : MonoBehaviour
    {

        UIToggle _toggle;

        public Action<int, bool> OnSelect;

        private int _index;
        void Awake()
        {
            _toggle = GetComponentInChildren<UIToggle>();
            _toggle.group = 8;
            _toggle.onChange.Add(new EventDelegate(this, "OnToggleSelect"));
        }

        private void OnToggleSelect()
        {
            if (OnSelect != null)
            {
                OnSelect(_index, _toggle.value);
            }
        }

        public void SelectThis()
        {
            _toggle.value = true;
        }

        public void Init(Action<int, bool> callBack, int index)
        {
            if (callBack != null)
            {
                OnSelect = callBack;
            }
            _index = index;
        }
    }
}
