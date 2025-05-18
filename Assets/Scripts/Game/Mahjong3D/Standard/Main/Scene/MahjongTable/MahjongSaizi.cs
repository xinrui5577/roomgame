using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [System.Serializable]
    public class SaiziAnimationData
    {
        public Vector3 StartPos;
        public Vector3 MaxOffset;
    }

    public class MahjongSaizi : MahjongTablePart
    {
        public Transform Saizi1;
        public Transform Saizi2;
        public Transform Saizi3;
        public Transform Glass;
        public SaiziAnimationData[] SaiziDatas;

        /// <summary>
        /// 2个骰子动画
        /// </summary>
        private IEnumerator SaiziTwoAnimation(byte val1, byte val2, float AllTime, System.Action action)
        {
            //动画已用时间
            float passTime = 0;
            float midTime = AllTime * 0.7f;
            //骰子的旋转速度 
            float rSpeed = 3000;
            //第一个塞子
            Vector3 ptionSaizi1 = SaiziDatas[0].StartPos;//骰子的初始化位置
            Vector3 lptoinSaizi1 = SaiziDatas[0].MaxOffset; //骰子的最大偏移坐标
            Quaternion quatSaizi1 = GetSaiziQuaternion(val1); //根据骰子的点数来计算出骰子的旋转量                                                              
            //第二个塞子
            Vector3 ptionSaizi2 = SaiziDatas[1].StartPos;
            Vector3 lptoinSaizi2 = SaiziDatas[1].MaxOffset;
            Quaternion quatSaizi2 = GetSaiziQuaternion(val2);
            while (passTime < AllTime)
            {
                float tmpval = passTime / AllTime;
                transform.Rotate(Vector3.up * Time.deltaTime * Mathf.Lerp(rSpeed, 0, tmpval));
                if (passTime < midTime)
                {
                    tmpval = passTime / midTime;
                    //1
                    Saizi1.localPosition = Vector3.Lerp(ptionSaizi1, lptoinSaizi1, tmpval);
                    Saizi1.rotation = Random.rotation;
                    //2
                    Saizi2.localPosition = Vector3.Lerp(ptionSaizi2, lptoinSaizi2, tmpval);                   
                    Saizi2.rotation = Random.rotation;
                }
                else
                {
                    tmpval = (passTime - midTime) / (AllTime - midTime);
                    //1
                    Saizi1.localPosition = Vector3.Lerp(lptoinSaizi1, ptionSaizi1, tmpval);                  
                    Saizi1.rotation = Quaternion.Slerp(Saizi1.rotation, quatSaizi1, tmpval);
                    //2
                    Saizi2.localPosition = Vector3.Lerp(lptoinSaizi2, ptionSaizi2, tmpval);
                    Saizi2.rotation = Quaternion.Slerp(Saizi2.rotation, quatSaizi2, tmpval);
                }
                yield return 3;
                passTime += Time.deltaTime;
            }
            Saizi1.rotation = quatSaizi1;
            Saizi2.rotation = quatSaizi2;
            yield return new WaitForSeconds(1f);
            if (null != action) action();
            HideSaizi();
        }

        /// <summary>
        /// 1个骰子动画
        /// </summary>
        private IEnumerator SaiziOneAnimation(byte val1, float AllTime, System.Action action)
        {
            //动画已用时间
            float passTime = 0;
            float midTime = AllTime * 0.7f;
            //骰子的旋转速度 
            float rSpeed = 3000;
            //骰子的初始化位置                                 
            Vector3 ptionSaizi3 = SaiziDatas[2].StartPos;
            //骰子的最大偏移坐标                   
            Vector3 lptoinSaizi3 = SaiziDatas[2].MaxOffset;
            //根据骰子的点数来计算出骰子的旋转量                   
            Quaternion quatSaizi3 = GetSaiziQuaternion(val1);
            while (passTime < AllTime)
            {
                float tmpval = passTime / AllTime;
                transform.Rotate(Vector3.up * Time.deltaTime * Mathf.Lerp(rSpeed, 0, tmpval));
                if (passTime < midTime)
                {
                    tmpval = passTime / midTime;
                    Saizi3.localPosition = Vector3.Lerp(ptionSaizi3, lptoinSaizi3, tmpval);
                    Saizi3.rotation = Random.rotation;
                }
                else
                {
                    tmpval = (passTime - midTime) / (AllTime - midTime);
                    Saizi3.localPosition = Vector3.Lerp(lptoinSaizi3, ptionSaizi3, tmpval);
                    Saizi3.rotation = Quaternion.Slerp(Saizi3.rotation, quatSaizi3, tmpval);
                }
                yield return 2;
                passTime += Time.deltaTime;
            }
            Saizi3.rotation = quatSaizi3;
            yield return new WaitForSeconds(0.6f);
            if (null != action) action();
            HideSaizi();
        }

        /// <summary>
        /// 根据传进来的点数，来确定骰子的旋转值
        /// </summary>
        private Quaternion GetSaiziQuaternion(int val)
        {
            Quaternion ret;
            Quaternion tmp;
            switch (val)
            {
                case 1:
                    ret = Quaternion.identity * Quaternion.AngleAxis(90, Vector3.right);
                    tmp = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward);
                    break;
                case 2:
                    ret = Quaternion.identity * Quaternion.AngleAxis(180, Vector3.right);
                    tmp = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
                    break;
                case 3:
                    ret = Quaternion.identity * Quaternion.AngleAxis(-90, Vector3.forward);
                    tmp = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.left);
                    break;
                case 4:
                    ret = Quaternion.identity * Quaternion.AngleAxis(90, Vector3.forward);
                    tmp = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.left);
                    break;
                case 5:
                    tmp = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
                    ret = Quaternion.identity;
                    break;
                default:
                    ret = Quaternion.identity * Quaternion.AngleAxis(-90, Vector3.right);
                    tmp = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward);
                    break;
            }
            return ret * tmp;
        }

        public void PlaySaiziAnimation(byte val1, byte val2, System.Action action = null)
        {
            Glass.gameObject.SetActive(false);
            Saizi1.gameObject.SetActive(true);
            Saizi2.gameObject.SetActive(true);
            MahjongUtility.PlayEnvironmentSound("saizi");
            StartCoroutine(SaiziTwoAnimation(val1, val2, 1, action));
        }

        public void PlaySaiziAnimation(byte val1, System.Action action = null)
        {
            Glass.gameObject.SetActive(false);
            Saizi3.gameObject.SetActive(true);
            MahjongUtility.PlayEnvironmentSound("saizi");
            StartCoroutine(SaiziOneAnimation(val1, 0.7f, action));
        }

        public void HideSaizi()
        {
            Saizi1.gameObject.SetActive(false);
            Saizi2.gameObject.SetActive(false);
            Saizi3.gameObject.SetActive(false);
            Glass.gameObject.SetActive(true);
        }

        public override void OnReset()
        {
            HideSaizi();
        }
    }
}