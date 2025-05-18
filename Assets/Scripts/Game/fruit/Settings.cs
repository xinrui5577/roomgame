using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game.fruit
{
    public class Settings : MonoBehaviour
    {
        public GameObject pref_settings;
        public Transform trans;
        public bool admitLongPressed = false;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnClickCreatePrefab()
        {
            Instantiate(pref_settings, trans);
        }

    }

}
