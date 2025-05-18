using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.slyz
{
    public class ChangeView : MonoBehaviour
    {
        public YxView View1;
        public YxView View2;
        /// <summary>
        /// 
        /// </summary>
        protected void Awake()
        {
            SetChangeView(false);
        }

        public void OnChangeView()
        { 
            SetChangeView(!View1.IsShow());
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetChangeView(bool activty)
        {
            if (activty)
            {
                View1.gameObject.SetActive(true);
                View1.Show();
                View2.gameObject.SetActive(false);
                View2.Hide();
            }
            else
            {
                View1.gameObject.SetActive(false);
                View1.Hide();
                View2.gameObject.SetActive(true);
                View2.Show();
            }
        }
    }
}
