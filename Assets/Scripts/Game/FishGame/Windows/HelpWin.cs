using Assets.Scripts.Game.FishGame.Utils;
using YxFramwork.Enums;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.FishGame.Windows
{
    public class HelpWin : YxWindow
    {
        private static HelpWin _instance;

        // Update is called once per frame
        void Update () {
//            if (GlobalData.GameState != YxGameState.Run) return;
//#if UNITY_EDITOR
//            if (Input.GetMouseButtonDown(0)) Close(); 
//#elif UNITY_ANDROID
//            if (Input.touchCount > 0) Close();
//        #else
//            if (Input.GetMouseButtonDown(0)) Close(); 
//        #endif
        }

        public override YxEUIType UIType
        {
            get { return YxEUIType.Default; }
        }
    }
}
