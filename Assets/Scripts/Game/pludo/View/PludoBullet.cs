using UnityEngine;

/*===================================================
 *文件名称:     PludoBullet.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2019-01-09
 *描述:        	子弹
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo.View
{
    public class PludoBullet : PludoFreshView 
    {
        #region UI Param
        [Tooltip("飞机子弹图片")]
        public UISprite BulletSprite;

        #endregion

        #region Data Param

        #endregion

        #region Local Data

        private PludoBulletData _curData;
        /// <summary>
        /// 移动完毕回调
        /// </summary>
        private EventDelegate _moveFinishDel;
        #endregion

        #region Life Cycle

        protected override void OnAwake()
        {
            base.OnAwake();
            _moveFinishDel=new EventDelegate(OnMoveFinish);
        }

        protected override void OnFreshViewWithData()
        {
            base.OnFreshViewWithData();
            _curData =Data as PludoBulletData;
            if (_curData!=null)
            {
                Reset();
                var tween=TweenPosition.Begin(gameObject, _curData.Time, _curData.TargetPos,true);
                if (!tween.onFinished.Contains(_moveFinishDel))
                {
                    tween.onFinished.Add(_moveFinishDel);
                }
            }
        }

        #endregion

        #region Function

        private void Reset()
        {
            transform.localPosition=Vector3.zero;
        }
        /// <summary>
        /// 移动完成处理
        /// </summary>
        private void OnMoveFinish()
        {
            Hide();
        }

        #endregion
    }

    /// <summary>
    /// 子弹数据
    /// </summary>
    public class PludoBulletData
    {
        /// <summary>
        /// 飞行时间
        /// </summary>
        public float Time;
        /// <summary>
        /// 子弹组Id
        /// </summary>
        public int GroupId;
        /// <summary>
        /// 目标方向
        /// </summary>
        public Vector3 TargetPos;
    }
}
