using UnityEngine;
using System.Collections;
using YxFramwork.Common;

namespace Assets.Scripts.Game.jsys
{
    public class SharkManager : MonoBehaviour
    {
        public float Speed;
        private Transform _currTransform;
        public bool IsOpen = false;
        public bool CanMove = false;
        private Animation _animation;

        void Start()
        {
            _animation = GetComponentInChildren<Animation>();
            CanMove = true;
        }

        public void StartMove()
        {
            var args = new Hashtable();
            var num = Random.Range(0, 9);
            _currTransform = App.GetGameManager<JsysGameManager>().ModelMgr.PointsTransforms[num].transform;
            args.Add("position", _currTransform.position);

            //设置类型为线性，线性效果会好一些。
            args.Add("easeType", iTween.EaseType.linear);
            //设置寻路的速度
            args.Add("speed", Speed);
            //是否先从原始位置走到路径中第一个点的位置
            //args.Add("movetopath", true);
            //是否让模型始终面朝当面目标的方向，拐弯的地方会自动旋转模型
            //如果你发现你的模型在寻路的时候始终都是一个方向那么一定要打开这个
            args.Add("orienttopath", true);
            args.Add("oncomplete", "Rest");
            //让模型开始寻路	
            iTween.MoveTo(gameObject, args);
            CanMove = false;
        }
        void Update()
        {
            if (CanMove && !IsOpen)
            {
                StartMove();
            }
        }

        public IEnumerator Rest()
        {
            yield return new WaitForSeconds(1.5f);
            CanMove = true;
        }
    }
}
