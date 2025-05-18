using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class CameraAni : MonoBehaviour
    {
        public static void ClearSelfStatic()
        {
            _self = null;
        }

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
        public static CameraAni Self
        {
            get
            {
                return _self;
            }
        }

        /// <summary>
        /// 初始化时候获取游戏对象上的组件
        /// </summary>
        protected void Start()
        {
            if (_self == null) _self = this;
            BeginPos = transform.position;
            BeginQuat = transform.rotation;
        }

        /// <summary>
        /// 摄像机移动到目标位置
        /// </summary>
        /// <param name="toPos">目标点位置</param>
        /// <param name="toQuat">目标点旋转</param>
        /// <param name="allTime"></param>
        /// <returns></returns>
        protected IEnumerator CameraParty(Vector3 toPos, Quaternion toQuat, float allTime)
        {
            float passTime = 0; //动画已用时间
            float binTime = Time.time;
            while (passTime < allTime)
            {
                passTime = Time.time - binTime;
                float val = passTime / allTime;
                transform.position = Vector3.Lerp(transform.position, toPos, val);
                transform.rotation = Quaternion.Slerp(transform.rotation, toQuat, val);
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
        public IEnumerator CameraPartyNoSmooth(Vector3 toPos, Quaternion toQuat, float allTime)
        {
            float passTime = 0; //动画已用时间
            float binTime = Time.time;
            Vector3 tmpbegPos = transform.localPosition;
            Quaternion tmpbegQuat = transform.localRotation;
            while (passTime < allTime)
            {
                passTime = Time.time - binTime;
                float val = passTime / allTime;
                transform.localPosition = Vector3.Lerp(tmpbegPos, toPos, val);
                transform.localRotation = Quaternion.Slerp(tmpbegQuat, toQuat, val);
                yield return 3;
            }
        }


        /// <summary>
        /// 相机拉近动画
        /// </summary>
        /// <param name="targeTransform">赖子的坐标</param>
        /// <param name="totalTime">动画播放时间</param>
        public void PlayFar2Near(Transform targeTransform, float totalTime)
        {
            EndPos = targeTransform.position + (BeginPos - targeTransform.position).normalized * 5.0f;
            ToQuat = Quaternion.LookRotation(targeTransform.position - BeginPos, Vector3.up);
            StartCoroutine(CameraParty(EndPos, ToQuat, totalTime));
        }

        /// <summary>
        /// 相机放大牌
        /// </summary>
        /// <param name="targeTransform">赖子的坐标</param>
        /// <param name="totalTime">动画播放时间</param>
        public void PlayFar2FangDa(Transform targeTransform, float totalTime)
        {
            EndPos = targeTransform.position + (BeginPos - targeTransform.position).normalized * 5.0f - Vector3.forward - Vector3.right * 0.5f;
            ToQuat = Quaternion.LookRotation(targeTransform.position - BeginPos, Vector3.up);
            StartCoroutine(CameraParty(EndPos, Quaternion.Euler(transform.eulerAngles), totalTime));
        }

        /// <summary>
        /// 相机拉近动画
        /// </summary>
        /// <param name="laizi">赖子的坐标</param>
        /// <param name="toalTime">动画播放时间</param>
        /// <param name="pindex"></param>
        public void PlayFar2NearnoSmooth(Transform laizi, float toalTime, byte pindex)
        {
            float[] fDis = { 12f, 13f, 11f, 13f };
            EndPos = laizi.position + (BeginPos - laizi.position).normalized * fDis[pindex];
            ToQuat = Quaternion.LookRotation(laizi.position - BeginPos, Vector3.up);
            StartCoroutine(CameraParty(EndPos, ToQuat, toalTime));
        }

        /// <summary>
        /// 相机拉回到原来位置
        /// </summary>
        /// <param name="allTime">动画播放时间</param>
        public void PlayN2F(float allTime)
        {
            EndPos = BeginPos;
            ToQuat = BeginQuat;
            StartCoroutine(CameraParty(EndPos, ToQuat, allTime));
        }

        public void PlayFar2Near2Back(Transform targeTransform, float totalTime, float waitTime)
        {
            EndPos = targeTransform.position + (BeginPos - targeTransform.position).normalized * 5.0f;
            ToQuat = Quaternion.LookRotation(targeTransform.position - BeginPos, Vector3.up);
            StartCoroutine(EnPlayFar2Near2Back(EndPos, ToQuat, totalTime, waitTime));
        }

        public IEnumerator EnPlayFar2Near2Back(Vector3 toPos, Quaternion toQuat, float allTime, float waitTime)
        {
            yield return CameraParty(EndPos, ToQuat, allTime);
            yield return new WaitForSeconds(waitTime);
            yield return CameraParty(BeginPos, BeginQuat, allTime);
        }
    }
}