using System.IO;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View
{
    public class HallStart : MonoBehaviour
    { 
        // Use this for initialization
        public void Start ()
        {
            Facade.Instance<MusicManager>().Init();
            HallMainController.Instance.LaunchHall(App.HasLogin);
        }
    }
}
