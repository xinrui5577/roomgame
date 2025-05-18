using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.lswc.Core;
using UnityEngine;

namespace Assets.Scripts.Game.lswc.Scene
{
    /// <summary>
    /// 控制烟花是否播放
    /// </summary>
    public class LSFireWorksControl:InstanceControl
    {
        public bool BeginState=false;

        private readonly List<GameObject> _fireWorks=new List<GameObject>();
 
        void Start () 
        {
            for (int i = 0; i <transform.childCount; i++)
            {
                var obj = transform.GetChild(i).gameObject;
                obj.SetActive(BeginState);
                _fireWorks.Add(obj);
            }   
        }

        public void Show()
        {
            StartCoroutine(PlayParticalSys());
        }

        private IEnumerator PlayParticalSys()
        {
            foreach (var fireWork in _fireWorks)
            {
                fireWork.SetActive(true);
                yield return new WaitForEndOfFrame();
            }
        }

        public void Hide()
        {
            foreach (var fireWork in _fireWorks)
            {
                fireWork.SetActive(false);
            }
        }

        public override void OnExit()
        {
           
        }
    }
}
