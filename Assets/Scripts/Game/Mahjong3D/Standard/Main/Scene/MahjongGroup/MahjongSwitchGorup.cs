using System;
using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongSwitchGorup : MonoBehaviour
    {
        public ObjContainer SwitchNodeContainer;
        public Transform Group1;
        public Transform Group2;
        public float Time = 1;

        public MahjongSwitchNode[] SwitchNodes { get; private set; }
        public Action OnCallback { get; set; }

        public MahjongSwitchNode this[int index]
        {
            get { return SwitchNodes[index]; }
        }

        public void Oninit(int count)
        {
            SwitchNodes = SwitchNodeContainer.GetComponent<MahjongSwitchNode>(count);           
        }

        public void OnResetSwitchNodes()
        {
            for (int i = 0; i < SwitchNodes.Length; i++)
            {
                SwitchNodes[i].OnReset();
            }
        }

        public void StartRotation(int iType)
        {
            switch (GameCenter.DataCenter.MaxPlayerCount)
            {
                case 3: RotationAnimationBy3(iType); break;
                case 4: NormalRotationAnimation(iType); break;
            }
        }

        //private IEnumerator RotoTo(GameObject go, Vector3 to, float time, Action callback = null)
        //{
        //    float val = 0;
        //    float bTime = UnityEngine.Time.time;
        //    Quaternion fqua = transform.localRotation;
        //    Quaternion tquat = Quaternion.Euler(to);
        //    while (val < time)
        //    {
        //        val = UnityEngine.Time.time - bTime;
        //        float smoothval = val / time;
        //        transform.localRotation = Quaternion.Lerp(fqua, tquat, smoothval);
        //        yield return 2;
        //    }
        //    if (callback != null) callback();
        //}

        /// <summary>
        /// 换张动画
        ///  0 顺时针
        ///  1 逆时针
        ///  2 对家
        /// </summary>
        private void NormalRotationAnimation(int type)
        {
            Action action = () => { transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0)); };
            //switch (type)
            //{
            //    case 0:
            //        StartCoroutine(RotoTo(gameObject, new Vector3(0, 90, 0), 1.2f, action));
            //        break;
            //    case 1:
            //        StartCoroutine(RotoTo(gameObject, new Vector3(0, -90, 0), 1.2f, action));
            //        break;
            //}
            switch (type)
            {
                case 0:
                    transform.DOLocalRotate(new Vector3(0, 90, 0), 1.2f).SetEase(Ease.InOutQuad).OnComplete(() =>
                    {
                        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    });
                    break;
                case 1:
                    transform.DOLocalRotate(new Vector3(0, -90, 0), 1.2f).SetEase(Ease.InOutQuad).OnComplete(() =>
                    {
                        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    });
                    break;
            }
        }

        /// <summary>
        /// 3人换张动画
        /// 三人血战换牌的时候，g1 转90， g2转180 
        ///  0 顺时针
        ///  1 逆时针   
        /// </summary>
        private void RotationAnimationBy3(int type)
        {
            Vector3 v1 = Vector3.zero;
            Vector3 v2 = Vector3.zero;
            switch (type)
            {
                case 0:
                    v1 = new Vector3(0, 90, 0);
                    v2 = new Vector3(0, 180, 0);
                    break;
                case 1:
                    v1 = new Vector3(0, -90, 0);
                    v2 = new Vector3(0, -180, 0);
                    break;
            }

            //StartCoroutine(RotoTo(Group1.gameObject, v1, 1.2f,
            //    () => { Group1.localRotation = Quaternion.Euler(new Vector3(0, 0, 0)); }));
            //StartCoroutine(RotoTo(Group2.gameObject, v2, 1.2f,
            //    () => { Group2.localRotation = Quaternion.Euler(new Vector3(0, 0, 0)); }));

            Group1.DOLocalRotate(v1, 1.2f).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                Group1.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            });
            Group2.DOLocalRotate(v2, 1.2f).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                Group2.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            });
        }
    }
}