using UnityEngine;
using Assets.Scripts.Game.fillpit.ImgPress.Main;
using YxFramwork.Common;


namespace Assets.Scripts.Game.fillpit
{
    public class PhizItem : MonoBehaviour
    {
        int _clickNum;

        public static System.Action OnItemClick = null;


        // Use this for initialization
        protected void Start()
        {
            string spriteName = GetComponent<UISprite>().spriteName;
            string[] arr = spriteName.Split('-');
            _clickNum = int.Parse(arr[0]);
        }

        // Update is called once per frame
        public void OnClick()
        {
            App.GetRServer<FillpitGameServer>().SendUserTalk(_clickNum);
            //ChatManager.GetInstance().SendUserPhizTalk(clickNum);
            if(OnItemClick != null) OnItemClick();
        }
    }
}