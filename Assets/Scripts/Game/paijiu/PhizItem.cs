//using Assets.Scripts.Game.paijiu.ImgPress.Main;
//using UnityEngine;
//using YxFramwork.Common;
//
//
//namespace Assets.Scripts.Game.paijiu
//{
//    public class PhizItem : MonoBehaviour
//    {
//
//
//        // ReSharper disable once InconsistentNaming
//        private int clickNum;
//
//        public static System.Action OnItemClick = null;
//
//
//        // Use this for initialization
//        // ReSharper disable once ArrangeTypeMemberModifiers
//        // ReSharper disable once UnusedMember.Local
//        void Start()
//        {
//            string s = GetComponent<UISprite>().spriteName;
//            string[] arr = s.Split('-');
//            clickNum = int.Parse(arr[0]);
//        }
//
//        // Update is called once per frame
//        // ReSharper disable once ArrangeTypeMemberModifiers
//        // ReSharper disable once UnusedMember.Local
//        void OnClick()
//        {
////            App.GetRServer<PaiJiuGameServer>().SendUserPhiz(clickNum);
//            //ChatManager.GetInstance().SendUserPhizTalk(clickNum);
//            OnItemClick();
//        }
//    }
//}