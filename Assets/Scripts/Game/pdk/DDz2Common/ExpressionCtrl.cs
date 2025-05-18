using System.Collections;
using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.pdk.DDz2Common
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
/*        /// <summary>
        /// 显示表情
        /// </summary>
        [SerializeField]
        protected UISprite Exp;*/

        [SerializeField]
        protected GameObject[] ExpAnim;

        /// <summary>
        /// 快捷聊天语音
        /// </summary>
        [SerializeField]
        protected AudioSource ChatAudioSource;

        void Awake()
        {
            Ddz2RemoteServer.AddOnVoiceChatEvt(OnUserSpeak);
        }

        private void OnUserSpeak(object sender, DdzbaseEventArgs args)
        {
            if (!App.GetGameData<GlobalData>().IsPlayVoiceChat) return;

            var param = args.IsfObjData;
            int len = param.ContainsKey("len") ? param.GetInt("len") : 1;

            StartCoroutine(MuteAudioTemply(len));
        }

        private IEnumerator MuteAudioTemply(int len)
        {
            ChatAudioSource.mute = true;
            yield return new WaitForSeconds(len);
            ChatAudioSource.mute = false;
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
            HidAllAnimExp();

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
        /// <param name="delay"></param>
        public void ShowExp(int exp, int delay = 3)
        {
            HidAllAnimExp();
            Text.gameObject.SetActive(false);
            gameObject.SetActive(true);

            if (ExpAnim==null || ExpAnim.Length <= exp) return;

            var anim = ExpAnim[exp].GetComponent<UISpriteAnimation>();
            if(anim==null) return;

            ExpAnim[exp].SetActive(true);
            anim.ResetToBeginning();
            anim.Play();

            StopAllCoroutines();
            StartCoroutine(CloseDelay(delay));
        }
        /// <summary>
        /// 延时关闭
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public IEnumerator CloseDelay(int delay)
        {
            yield return new WaitForSeconds(delay);
            Text.gameObject.SetActive(false);
            HidAllAnimExp();
        }


        private void HidAllAnimExp()
        {
            foreach (var anim in ExpAnim)
            {
                anim.SetActive(false);
            }
        }
    }
}
