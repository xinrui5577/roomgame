using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Common.UI
{
    public class CameraAni : MonoBehaviour
    {

        /// <summary>
        /// 动画的起始位置
        /// </summary>
        protected Vector3 BeginPos;
        /// <summary>
        /// 目标位置
        /// </summary>
        protected Vector3 EndPos;

        /// <summary>
        /// 起始方向
        /// </summary>
        protected Quaternion BeginQuat;

        /// <summary>
        /// 目标方向
        /// </summary>
        protected Quaternion ToQuat;

        /// <summary>
        /// 静态的指向自己的指针
        /// </summary>
        private static CameraAni _self = null;

        ///public float Distence = 10.0f;

        /// <summary>
        /// 静态的指向自己的指针
        /// 方便外部调用
        /// </summary>
        public static CameraAni Self {
            get {
                return _self;
            }
        }

        /// <summary>
        /// 初始化时候获取游戏对象上的组件
        /// </summary>
        protected void Start() {
            if (_self == null)
                _self = this;
            BeginPos = transform.localPosition;
            BeginQuat = transform.localRotation;
        }


        /// <summary>
        /// 摄像机移动到目标位置
        /// </summary>
        /// <param name="toPos">目标点位置</param>
        /// <param name="toQuat">目标点旋转</param>
        /// <param name="allTime"></param>
        /// <returns></returns>
        protected IEnumerator CameraParty(Vector3 toPos, Quaternion toQuat, float allTime) {
            float passTime = 0; //动画已用时间
            float binTime = Time.time;

            //SoundHelper.Self.PlaySound("camera", SoundType.uiSound);

            while (passTime < allTime) {
                passTime = Time.time - binTime;

                float val = passTime/allTime;

                transform.localPosition = Vector3.Lerp(transform.localPosition, toPos, val);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, toQuat, val);
                yield return 3;
            }
        }

        /// <summary>
        /// 摄像机移动到目标位置
        /// </summary>
        /// <param name="toPos">目标点位置</param>
        /// <param name="toQuat">目标点旋转</param>
        /// <param name="allTime">需要时间</param>
        /// <returns></returns>
        public IEnumerator CameraPartyNoSmooth(Vector3 toPos, Quaternion toQuat, float allTime) {
            float passTime = 0; //动画已用时间
            float binTime = Time.time;

            Vector3 tmpbegPos = transform.localPosition;
            Quaternion tmpbegQuat = transform.localRotation;
            while (passTime < allTime) {
                passTime = Time.time - binTime;

                float val = passTime/allTime;

                transform.localPosition = Vector3.Lerp(tmpbegPos, toPos, val);
                transform.localRotation = Quaternion.Slerp(tmpbegQuat, toQuat, val);
                yield return 3;
            }
        }
        /// <summary>
        /// 相机拉回到原来位置
        /// </summary>
        /// <param name="allTime">动画播放时间</param>
        public void PlayN2F(float allTime) {
            EndPos = BeginPos;
            ToQuat = BeginQuat;
            StartCoroutine(CameraParty(EndPos, ToQuat, allTime));
        }
    }
}
