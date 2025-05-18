using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class TitleMessageStyle1 : MonoBehaviour
    {
        public CanvasGroup CanvasGroup;
        public Text TitleContent;
        public Text TitleHead;

        /// <summary>
        /// 渐隐时间
        /// </summary>
        public float DissolvingTime = 1;
        /// <summary>
        /// 显示时间
        /// </summary>
        public float ShowTime = 2;
       
        private float mDissolvingTimer = 0;
        private float mShowTimer = 0;
        private bool mFlag = false;    

        public void ShowMessage(string content)
        {
            TitleContent.gameObject.SetActive(true);
            TitleContent.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            TitleContent.text = content;
            TitleContent.fontSize = 40;
            mFlag = true;
        }

        public void ShowMessage(string title, string content)
        {
            TitleContent.gameObject.SetActive(true);
            TitleContent.gameObject.SetActive(true);
            TitleContent.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 20, 0);          
            TitleContent.fontSize = 38;
            TitleContent.text = title;
            TitleHead.text = content;
            mFlag = true;
        }

        public void Hide()
        {
            TitleContent.gameObject.SetActive(false);
            TitleHead.gameObject.SetActive(false);
            gameObject.SetActive(false);
            CanvasGroup.alpha = 1;
        }

        private void Update()
        {
            if (mFlag)
            {
                mShowTimer += Time.deltaTime;
                if (mShowTimer >= ShowTime)
                {
                    mDissolvingTimer += Time.deltaTime;
                    if (DissolvingTime >= mDissolvingTimer)
                    {
                        float rate = mDissolvingTimer / DissolvingTime;
                        float value = Mathf.Lerp(1, 0, rate);
                        CanvasGroup.alpha = value;
                    }
                    else
                    {
                        mDissolvingTimer = 0;
                        mFlag = false;
                        mShowTimer = 0;
                        Hide();
                    }
                }
            }
        }
    }
}