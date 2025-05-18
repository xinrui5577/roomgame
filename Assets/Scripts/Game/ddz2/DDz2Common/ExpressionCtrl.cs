using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.ddz2.DDz2Common
{
    /// <summary>
    /// 聊天信息表达
    /// </summary>
    public class ExpressionCtrl : MonoBehaviour
    {
        /// <summary>
        /// 显示文字
        /// </summary>
        [SerializeField]
        protected UILabel Text;

        /// <summary>
        /// 表情父节点
        /// </summary>
        public GameObject ExpsParent;
        /// <summary>
        /// 显示表情
        /// </summary>
        protected GameObject[] Exps;

        /// <summary>
        /// 快捷聊天语音
        /// </summary>
        [SerializeField]
        protected AudioSource ChatAudioSource;


        void Awake()
        {
            InitFaces();
        }

        /// <summary>
        /// 初始化表情
        /// </summary>
        private void InitFaces()
        {
            if (ExpsParent == null) return;
            var childCnt = ExpsParent.transform.childCount;
            Exps = new GameObject[childCnt];
            for (int i = 0; i < childCnt; i++)
            {
                Exps[i] = ExpsParent.transform.GetChild(i).gameObject;
            }
            HideAllFaces();
        }

        /// <summary>
        /// 显示文字
        /// </summary>
        /// <param name="text"></param>
        /// <param name="delay">延时</param>
        /// <param name="audioClip"></param>
        public void ShowText(string text, int delay = 3,AudioClip audioClip=null)
        {
            Text.text = text;
            Text.gameObject.SetActive(true);
            //Exp.gameObject.SetActive(false);
            HideAllFaces();
            gameObject.SetActive(true);

            StopAllCoroutines();
            StartCoroutine(CloseDelay(delay));

            //播放语音
            ChatAudioSource.PlayOneShot(audioClip);
        }
        /// <summary>
        /// 显示表情
        /// </summary>
        /// <param name="exp"></param>
        public void ShowExp(int exp)
        {
            if (Exps.Length > exp && exp >= 0)
            {
                Exps[exp].SetActive(true);
                Exps[exp].GetComponent<UISpriteAnimation>().ResetToBeginning();
                Exps[exp].GetComponent<UISpriteAnimation>().Play();


                //Exp.spriteName = exp.ToString();
                Text.gameObject.SetActive(false);
                //Exp.gameObject.SetActive(true);
                gameObject.SetActive(true);
                StopAllCoroutines();

                var delay = Exps[exp].GetComponent<UISprite>().atlas.spriteList.Count;
                StartCoroutine(CloseDelay(delay));
            }

        }
        /// <summary>
        /// 延时关闭
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public IEnumerator CloseDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Text.gameObject.SetActive(false);
            //Exp.gameObject.SetActive(false);
            HideAllFaces();
        }

        //隐藏所有表情
        private void HideAllFaces()
        {
            if(Exps==null) return;

            foreach (var face in Exps)
            {
                face.SetActive(false);
            }
        }
    }
}
