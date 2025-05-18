using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Game.LXGameScripts.lx39
{
    public class ButtonChange : MonoBehaviour
    {

        private GameObject _start;
        private GameObject _stop;
        private GameObject _startHui;
        private GameObject _stopHui;
        private List<GameObject> _allBtn = new List<GameObject>();

        protected virtual void Awake()
        {
            _start = transform.FindChild("Start").gameObject;
            _allBtn.Add(_start);
            _stop = transform.FindChild("Stop").gameObject;
            _allBtn.Add(_stop);
            _startHui = transform.FindChild("Start_hui").gameObject;
            _allBtn.Add(_startHui);
            _stopHui = transform.FindChild("Stop_hui").gameObject;
            _allBtn.Add(_stopHui);
        }
        
        private int i = 0;
        public virtual void OnStartClick()
        {
            EventDispatch.Dispatch((int)EventID.GameEventId.OnStartClick);
            ChooseShowIcon(1);
        }

        public virtual void OnStopClick()
        {
            EventDispatch.Dispatch((int)EventID.GameEventId.OnStopClick);
            ChooseShowIcon(2);
        }

        public void ChooseShowIcon(int type)
        {
            if (type >= _allBtn.Count || type < 0) return;
            for (int i = 0; i < _allBtn.Count; i++)
            {
                if (i == type) _allBtn[type].SetActive(true);
                else _allBtn[i].SetActive(false);
            }
        }
    }
}

