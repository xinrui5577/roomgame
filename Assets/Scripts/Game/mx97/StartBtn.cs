using UnityEngine;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.mx97
{
    public class StartBtn : MonoBehaviour {

        private bool _mIsAutoStart = true;                           // 是否自动开始游戏

        UIToggle _mAutoStartToggle ;                           // CHECK BOX
        UIButton _mStartButton ;                               // 开始按钮

        TweenRotation _mRotation ;

        // Use this for initialization
       protected void Start () 
        {
            _mRotation = transform.FindChild("RotationAni").GetComponent<TweenRotation>();
            _mAutoStartToggle = transform.FindChild("Checkbox").GetComponent<UIToggle>();

            // 开始按钮监听
            _mStartButton = GetComponent<UIButton>();
            UIEventListener.Get(_mStartButton.gameObject).onClick = delegate
                {
                    _mStartButton.isEnabled = false;
                    _mRotation.gameObject.SetActive(true);
                    _mRotation.PlayForward();

                    if (_mAutoStartToggle.value)
                    {
                        _mIsAutoStart = true;
                    }
                    else
                    {
                        _mIsAutoStart = false;
                    }

                    SendStartRequest();

                    //GlobalMessage.OnShowMessage();
                };

        
            UIEventListener.Get(_mAutoStartToggle.gameObject).onClick = delegate
                {
                    if (_mAutoStartToggle.value == false)
                    {
                        _mIsAutoStart = false;
                    }
                };

        }

        // Update is called once per frame
       protected void Update()
        {
            // 切换开始按钮状态
           var gdata = App.GetGameData<Mx97GlobalData>();
           if (gdata.IsCardsForwardComplete && gdata.IsShowPrizeMaskComplete)
            {
                gdata.IsCardsForwardComplete = false;
                gdata.IsShowPrizeMaskComplete = false;

                if (_mIsAutoStart)
                    Invoke("SendStartRequest",1.5f);                
                else
                {
                    _mStartButton.isEnabled = true;
                    //mRotation.gameObject.SetActive(false);
                    _mRotation.enabled = false;
                
                }
            }
        }

        // 发送开始游戏请求
        private void SendStartRequest()
        {
            var gdata = App.GetGameData<Mx97GlobalData>();

            gdata.GetPlayer(gdata.SelfLocalSeat).Coin -= Mathf.Abs(gdata.Ante * 4 * 110 / 100);

            // 发送请求数据
            var data = SFSObject.NewInstance();
            data.PutInt(RequestKey.KeyType, 1);
            App.RServer.SendGameRequest(data);

            if (gdata.IsEnableAudio)
                GetComponent<AudioSource>().Play();
        }

    }
}
