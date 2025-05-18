using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.sanpian.item
{
    public class Clock : MonoBehaviour
    {
        private int m_cd;

        public UILabel ClockNum;

        public int  cd 
        {
            set
            {
                m_cd = value;
                StartCoroutine(StartClolk());
            }
        }

        IEnumerator  StartClolk()
        {
            while (true)
            {
                ClockNum.text = m_cd+"";
                m_cd--;
                yield return new WaitForSeconds(1f);
                if (m_cd<=0)
                {
                    gameObject.SetActive(false);
                    break;
                }
            }
        }
    }
}
