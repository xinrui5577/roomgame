/*
 * 用于在NGUI下,下注的按钮进行初始化
 * 设置好贴图后,会将点击事件中的图片全部换做该贴图
 * 
 */


using UnityEngine;
using System.Collections;
using Assets.Scripts.Game.GangWu.Mgr;
using YxFramwork.Common;
using YxFramwork.Tool;
using System.Collections.Generic;
using Assets.Scripts.Game.GangWu.Main;


namespace Assets.Scripts.Game.GangWu
{
    [RequireComponent(typeof(UISprite),typeof(UIButton),typeof(BoxCollider))]
    public class AddBetBtn : MonoBehaviour
    {

        /// <summary>
        /// 添加筹码的倍数
        /// </summary>
        [SerializeField]
        private int _addTime;

        /// <summary>
        /// 筹码下注倍数
        /// </summary>
        public int AddValue
        {
            set { _addTime = value > 0 ? value : _addTime; }
            get { return _addTime; }
        }


        [SerializeField]
        private Vector3 _boxColliderSize = Vector3.zero;

        private bool _pressing;
    
        // Use this for initialization
        protected void Start()
        {
            SetBtn();
            InitBoxCollider();
            
            //显示当前局此按钮添加筹码的值
            GetComponentInChildren<UILabel>(true).text = YxUtiles.ReduceNumber(AddValue, 2,true);//App.GetGameData<GlobalData>().GetShowGold(AddValue,true));
        }

        protected void OnEnable()
        {
            //显示当前局此按钮添加筹码的值
            GetComponentInChildren<UILabel>(true).text = YxUtiles.ReduceNumber(AddValue, 2, true);//App.GetGameData<GlobalData>().GetShowGold(AddValue,true);
        }


        /// <summary>
        /// 初始化按钮
        /// </summary>
        /// <returns></returns>
        private void SetBtn()
        {

            UIButton btn = GetComponent<UIButton>();
  
            btn.duration = 0.05f;

            //长按连续加注
            UIEventTrigger eventTri = btn.gameObject.AddComponent<UIEventTrigger>();
            eventTri.onPress.Add(new EventDelegate(() =>
                {
                    _pressing = true;
                    StartCoroutine(OnPress());  //下注间隔逐渐缩短
                }));

            //松手停止加注
            eventTri.onRelease.Add(new EventDelegate(() =>
                {
                    _pressing = false;
                    StopAllCoroutines();
                }));
        }

        IEnumerator OnPress()
        {
            float timer = 0.5f;

            while (_pressing)
            {
                yield return new WaitForEndOfFrame();
                DoAdd();
                yield return new WaitForSeconds(timer);
                timer = timer > 0.1f ? timer - 0.1f : 0.1f;
            }
        }

        /// <summary>
        /// 添加筹码
        /// </summary>
        private void DoAdd()
        {
            SendToServer();
            LocalAdd();
        }
        /// <summary>
        /// 预加注通知服务器
        /// </summary>
        void SendToServer()
        {
            var data = new Dictionary<string, object>() { { "gold", AddValue } };
            App.GetRServer<GangWuGameServer>().SendRequest(GameRequestType.AdvanceBet, data);
        }

        /// <summary>
        /// 本地添加数值
        /// </summary>
        void LocalAdd()
        {
            var mgr = App.GetGameManager<GangWuGameManager>();
            SpeakMgr speakMgr = mgr.SpeakMgr;

            //如果点击加注按钮,第一次下注后,会连同跟注的筹码一齐下入
            speakMgr.AddBetSum += AddValue;
            speakMgr.BetRemenber.Push(AddValue);
            mgr.BetMgr.RefreshAddBetBtns();     //刷新按钮的按键情况
        }


        /// <summary>
        /// 设置BoxCollider的大小,优先其本身设置,再根据本方法填入数值大小.最后通过图片大小设置
        /// </summary>
        private void InitBoxCollider()
        {
            BoxCollider box = GetComponent<BoxCollider>();
           
            //如果两者有一个是0，则证明没有设置,自动设置成图片等大小
            if (!(box.size.x <= 0.01f || box.size.y <= 0.01f))
            {
            }
            else if (!(_boxColliderSize.x <= 0.01f || _boxColliderSize.y <= 0.01f))
            {
                box.size = _boxColliderSize;
            }
            else
            {
                Texture tex = GetComponent<UISprite>().mainTexture;
                box.size = new Vector3(tex.width, tex.height, 0);
            }
        }

        public void StopPress()
        {
            StopCoroutine(OnPress());
            _pressing = false;
        }
    }
}