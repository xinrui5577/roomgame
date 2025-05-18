using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Game.slyz.GameScene
{
    public class StartBtn : MonoBehaviour {

        private bool mIsAutoStart = true;                           // 是否自动开始游戏

        UIToggle mAutoStartToggle = null;                           // CHECK BOX
        UIButton mStartButton = null;                               // 开始按钮

        TweenRotation mRotation = null;

        // Use this for initialization
        void Start ()
        {
            mRotation = transform.FindChild("RotationAni").GetComponent<TweenRotation>();
            mAutoStartToggle = transform.FindChild("Checkbox").GetComponent<UIToggle>();
            mAutoStartToggle.value = mIsAutoStart;
            // 开始按钮监听
            mStartButton = GetComponent<UIButton>();
            UIEventListener.Get(mStartButton.gameObject).onClick = delegate(GameObject o)
                {
                    mStartButton.isEnabled = false;
                    mRotation.gameObject.SetActive(true);
                    mRotation.PlayForward();
                    StartRequest();

                    //GlobalMessage.OnShowMessage();
                };

        
            UIEventListener.Get(mAutoStartToggle.gameObject).onClick = delegate(GameObject o)
                {
                    mIsAutoStart = mAutoStartToggle.value;
                };

        }

        // Update is called once per frame
        void Update()
        {
            var gdata = App.GetGameData<SlyzGameData>();
            if (gdata == null) return;
            // 切换开始按钮状态
            if (gdata.IsCardsForwardComplete && gdata.IsShowPrizeMaskComplete)
            {                        
                gdata.IsCardsForwardComplete = false;
                gdata.IsShowPrizeMaskComplete = false;
                if (gdata.IsStopAutoStart)
                {
                    mAutoStartToggle.value = false;
                    mIsAutoStart = false;
                    gdata.IsStopAutoStart = false;
                }
                if (mIsAutoStart)
                    Invoke("StartRequest", 1.5f);                
                else
                {
                    mStartButton.isEnabled = true;
                    //mRotation.gameObject.SetActive(false);
                    mRotation.enabled = false;
                }
            }
        }

        // 发送开始游戏请求
        private void StartRequest()
        {
            var gdata = App.GetGameData<SlyzGameData>();
             
            // 初始化游戏数据
            if (App.GetGameData<SlyzGameData>().GMessage.OnGameInit == null) { return;}
            App.GetGameData<SlyzGameData>().GMessage.OnGameInit();

            long deductGold = Mathf.Abs(gdata.Ante * 4 * 110 / 100);
            var player = gdata.GetPlayer(gdata.SelfLocalSeat);
            if (player.Coin < deductGold)
            {
                YxMessageTip.Show("金币不足！");
                if (mIsAutoStart) mIsAutoStart = false;
                return;
            }

            player.Coin -= deductGold;

            if (App.GetGameData<SlyzGameData>().GMessage.OnShowTotalGlod != null)
            {
                App.GetGameData<SlyzGameData>().GMessage.OnShowTotalGlod();
            }

            // 发送请求数据
            App.GetRServer<SlyzGameServer>().SendStart();
            Facade.Instance<MusicManager>().Play("FlopCards");
        }

    }
}
