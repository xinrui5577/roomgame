using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class SetScrollState : MonoBehaviour
    {
        public Toggle tog;
        public GameObject target;

        public void ChangeSelectState(bool selected)
        {

            if (tog.isOn)
            {
                target.SetActive(true);
            }
            else
            {
                target.SetActive(false);
            }
        }


        // Use this for initialization
        void Start()
        {
            tog.GetComponent<Toggle>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}