using UnityEngine;

namespace Assets.Scripts.Game.brnn
{
    public class Lister : MonoBehaviour
    {
        public GameObject SwallowObj;
        private bool _isOpen;

        public void OpenList()
        {
            if (_isOpen) return;
            _isOpen = true;
            transform.localPosition += new Vector3(-256, 0, 0);

            SwallowObj.SetActive(true);
        }

        public void CloseList()
        {
            if (!_isOpen) return;
            _isOpen = false;
            transform.localPosition += new Vector3(256, 0, 0);

            SwallowObj.SetActive(false);
        }
    }
}