using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameTable
{
    public class Saizi : MonoBehaviour {
        /// <summary>
        /// 骰子 1
        /// </summary>
        Transform Saizi1;
        /// <summary>
        /// 骰子 2
        /// </summary>
        Transform Saizi2;

        //private GameObject _saiziCamera;
        ///// <summary>
        ///// 静态的指向自己的指针
        ///// </summary>
        //static Saizi self = null;

        ///// <summary>
        ///// 静态的指向自己的指针
        ///// 方便外部调用
        ///// </summary>
        //public static Saizi Self {
        //    get {
        //        return self;
        //    }
        //}

        /// <summary>
        /// 初始化时候获取游戏对象上的组件
        /// </summary>
        void Start() {
            //if (self == null)
            //    self = this;

            Saizi1 = transform.FindChild("touzi1");
            Saizi2 = transform.FindChild("touzi2");
            //_saiziCamera = GameObject.Find("GameTable/desktop/SaiziCamera");
            //Play(3, 5, 3);
        }


        /// <summary>
        /// 骰子动画
        /// </summary>
        /// <param name="val1">骰子1的点数 </param>
        /// <param name="val2">骰子2的点数</param>
        /// <param name="AllTime">动画播放时间</param>
        /// <returns></returns>
        IEnumerator SaiziParty(byte val1, byte val2, float AllTime) {
            float passTime = 0;                                     //动画已用时间
            float midTime = AllTime * 0.7f;
            float rSpeed = 3000;                                    //骰子的旋转速度 

            Vector3 ptionSaizi1 = new Vector3(0, 0, -0.09f);       //骰子的初始化位置
            Vector3 ptionSaizi2 = new Vector3(0, 0, 0.09f);

            Vector3 lptoinSaizi1 = new Vector3(0, 0, -0.15f);      //骰子的最大偏移坐标
            Vector3 lptoinSaizi2 = new Vector3(0, 0, 0.15f);

            Quaternion quatSaizi1 = GetSaiziQuaternion(val1);       //根据骰子的点数来计算出骰子的旋转量
            Quaternion quatSaizi2 = GetSaiziQuaternion(val2);

            while (passTime < AllTime) {
                float tmpval = passTime / AllTime;

                transform.Rotate(Vector3.up * Time.deltaTime * Mathf.Lerp(rSpeed, 0, tmpval));

                if (passTime < midTime) {
                    tmpval = passTime / midTime;
                    Saizi1.localPosition = Vector3.Lerp(ptionSaizi1, lptoinSaizi1, tmpval);
                    Saizi2.localPosition = Vector3.Lerp(ptionSaizi2, lptoinSaizi2, tmpval);
                    Saizi1.rotation = Random.rotation;
                    Saizi2.rotation = Random.rotation;

                } else {
                    tmpval = (passTime - midTime) / (AllTime - midTime);
                    Saizi1.localPosition = Vector3.Lerp(lptoinSaizi1, ptionSaizi1, tmpval);
                    Saizi2.localPosition = Vector3.Lerp(lptoinSaizi2, ptionSaizi2, tmpval);
                    Saizi1.rotation = Quaternion.Slerp(Saizi1.rotation, quatSaizi1, tmpval);
                    Saizi2.rotation = Quaternion.Slerp(Saizi2.rotation, quatSaizi2, tmpval);
                }
                yield return 3;
                passTime += Time.deltaTime;
            }
        }

        /// <summary>
        /// 根据传进来的点数，来确定骰子的旋转值
        /// </summary>
        /// <param name="val">骰子的点数</param>
        /// <returns></returns>
        Quaternion GetSaiziQuaternion(int val) {
            Quaternion ret;
            Quaternion tmp;
            switch (val) {
                case 1:
                    ret = Quaternion.identity * Quaternion.AngleAxis(-90, Vector3.right);
                    tmp = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward);
                    break;
                case 2:
                    ret = Quaternion.identity * Quaternion.AngleAxis(180, Vector3.right);
                    tmp = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
                    break;
                case 3:
                    ret = Quaternion.identity * Quaternion.AngleAxis(90, Vector3.forward);
                    tmp = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.left);
                    break;
                case 4:
                    ret = Quaternion.identity * Quaternion.AngleAxis(-90, Vector3.forward);
                    tmp = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.left);
                    break;
                case 6:
                    ret = Quaternion.identity * Quaternion.AngleAxis(90, Vector3.right);
                    tmp = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward);
                    break;
                default:
                    tmp = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
                    ret = Quaternion.identity;
                    break;
            }

            return ret * tmp;
        }

        /// <summary>
        /// 播放骰子动画
        /// </summary>
        /// <param name="val1">骰子1的点数 </param>
        /// <param name="val2">骰子2的点数</param>
        /// <param name="AllTime">动画播放时间</param>
        private Coroutine PlayCoro;
        public void Play(byte val1, byte val2, float AllTime) {
            PlayCoro = StartCoroutine(SaiziParty(val1, val2, AllTime));
        }

        public void StopPlaye(byte val1, byte val2)
        {
            if (PlayCoro != null) StopCoroutine(PlayCoro);
            ReConncet(val1, val2);
        }
        /// <summary>
        /// 断线重连骰子的显示
        /// </summary>
        /// <param name="val1">骰子1的点数</param>
        /// <param name="val2">骰子2的点数</param>
        public void ReConncet(byte val1, byte val2) {
            Quaternion quatSaizi1 = GetSaiziQuaternion(val1);       //根据骰子的点数来计算出骰子的旋转量
            Quaternion quatSaizi2 = GetSaiziQuaternion(val2);

            Saizi1.rotation = quatSaizi1;
            Saizi2.rotation = quatSaizi2;
        }

        public void HideSaizi()
        {
            gameObject.SetActive(false);
        }

        public void ShowSaizi()
        {
            gameObject.SetActive(true);
        }

        public void ShowDingshenSaizi()
        {
            gameObject.SetActive(true);
            Saizi1 = transform.FindChild("touzi1");
            Saizi2 = transform.FindChild("touzi2");
        }
    }
}
