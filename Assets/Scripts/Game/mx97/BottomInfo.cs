using UnityEngine;

namespace Assets.Scripts.Game.mx97
{
    public class BottomInfo : MonoBehaviour
    {
        private UILabel _mLeftLabel;
    
        // Use this for initialization
       protected void Start ()
        {
            _mLeftLabel = transform.FindChild("LeftLabel").GetComponent<UILabel>();

            _mLeftLabel.gameObject.SetActive(false);
#if (UNITY_WINRT || UNITY_EDITOR)
            _mLeftLabel.gameObject.SetActive(true);
#endif
        }
    }
}
