using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ChangeCardTip : MonoBehaviour
    {
        public Text ContentText;

        public void ShowTip(int type)
        {
            gameObject.SetActive(true);
            switch (type)
            {
                case 0:
                    ContentText.text = "本局顺时针换牌";
                    break;
                case 1:
                    ContentText.text = "本局逆时针换牌";
                    break;
                case 2:
                    ContentText.text = "本局对家换牌";
                    break;
                default:
                    ContentText.text = "本局顺时针换牌";
                    break;
            }
            StartCoroutine(IeShowTipAnimation());
        }

        private IEnumerator IeShowTipAnimation()
        {
            yield return new WaitForSeconds(2);
            gameObject.SetActive(false);
        }
    }
}