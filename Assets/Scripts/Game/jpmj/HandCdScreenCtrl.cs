using System;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj
{
    /// <summary>
    /// 根据屏幕比例调整手牌比例
    /// </summary>
    public class HandCdScreenCtrl : MonoBehaviour
    {

        [SerializeField] protected float HdcdScale = 1f;
        [SerializeField]
        protected float RatiolValue = 1.77777f;
        // Use this for initialization
        [Tooltip("最大倍数")]
        public float MaxScale= 1.15f;

        public Vector3 HandCardScaleVec { get; private set;}
        

        void Start ()
        {
            var ratiol = (float)Screen.width / Screen.height;
            var curScreenRatial = HdcdScale * (ratiol / RatiolValue);
            curScreenRatial = Math.Min(MaxScale, curScreenRatial);
            HandCardScaleVec=new Vector3(curScreenRatial, curScreenRatial, curScreenRatial);
            transform.localScale = HandCardScaleVec;
        }
	
    }
}
