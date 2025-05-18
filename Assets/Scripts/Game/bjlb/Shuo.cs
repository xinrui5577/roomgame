using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.bjlb
{
    public class Shuo : MonoBehaviour
    {


        public SpringPosition Sp;

        public void Open()
        {
            Sp.target = new Vector3(468,-43,0);
            Sp.enabled = true;
            Invoke("Close",5);
        }
        public void Close()
        {
            CancelInvoke("Close");
            Sp.target = new Vector3(554, -43, 0);
            Sp.enabled = true;
        }

        public void Play(GameObject go)
        {
            Close();
            var sName = go.GetComponent<UISprite>().spriteName;
            Facade.Instance<MusicManager>().Play(sName);
        }
    }
}
