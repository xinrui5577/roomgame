using Assets.Scripts.Game.pdk.InheritCommon;
using UnityEngine;
using YxFramwork.View;

namespace Assets.Scripts.Game.pdk.DDz2Common
{
    public class MsgChatUiCtrl : MonoBehaviour
    {

        /// <summary>
        /// 父节点，用于隐藏整个聊天界面
        /// </summary>
        [SerializeField] 
        protected GameObject UiGob;
        
        /// <summary>
        /// 常用聊天信息原型
        /// </summary>
        [SerializeField]
        protected GameObject MsgLabelOrg;

        [SerializeField] 
        protected GameObject MsgGrid;

        [SerializeField]
        protected GameObject FaceGrid;
        /// <summary>
        /// 发送聊天按钮
        /// </summary>
        [SerializeField]
        protected GameObject SendTalkBtn;
        /// <summary>
        /// 发送俩天的label
        /// </summary>
        [SerializeField] 
        protected UILabel TapTalklabel;

        /// <summary>
        /// 常用语
        /// </summary>
        public readonly string[] TalkStrs =
            {
                "这位朋友，出牌有点慢呀",
                "哇,原来您才是真正的绝世高手哦",
                "呦,是跟高手过招呀",
                "哎,一把好牌给你废武功了",
                "小样,碰到高手了吧",
                "网络太差了，总是掉线呀",
                "啊,不好意思刚刚接了个电话",
                "不好意思我有事儿了,我们解散吧",
                "啥情况呀,这牌竟然被打输了",
                "天灵灵,地灵灵,来把好牌行不行!",
                "苍天呀,大地呀,我又出错牌呀!",
                "不要走,陪你到天亮!"
            };

        // Use this for initialization
        void Start ()
        {
            InitTalk();
            InitFace();
        }

        /// <summary>
        /// 初始化talk
        /// </summary>
        void InitTalk()
        {
            //常用聊天初始化
            var len = TalkStrs.Length;
            for (var i = 0; i < len; i++)
            {
                var gob = NGUITools.AddChild(MsgGrid, MsgLabelOrg);
                gob.SetActive(true);
                gob.GetComponent<UILabel>().text = TalkStrs[i];
                UIEventListener.Get(gob).onClick = OnClickTalk;
            }
            MsgGrid.GetComponent<UIGrid>().repositionNow = true;

        }

        /// <summary>
        /// 初始化face
        /// </summary>
        void InitFace()
        {
            //初始化face
            var faceTransForm = FaceGrid.transform;
            var faceCunt = faceTransForm.childCount;
            for (var i = 0; i < faceCunt; i++)
            {
                UIEventListener.Get(faceTransForm.GetChild(i).gameObject).onClick = OnClickExp;
            }
        }

        /// <summary>
        /// 当点击常用语聊天
        /// </summary>
        public void OnClickTalk(GameObject obj)
        {
            var label = obj.GetComponent<UILabel>();
            GlobalData.ServInstance.UserTalk(label.text);
            CloseChatUi();
        }

        /// <summary>
        /// 当点击发送聊天按钮
        /// </summary>
        public void OnClickSendTalk()
        {
            if (!string.IsNullOrEmpty(TapTalklabel.text))
            {
                GlobalData.ServInstance.UserTalk(TapTalklabel.text);
                CloseChatUi();
            }
            else
            {
                YxMessageBox.Show("不能发送空消息");
            }
         
        }

        /// <summary>
        /// 当点击表情
        /// </summary>
        /// <param name="obj"></param>
        public void OnClickExp(GameObject obj)
        {
            var expId = int.Parse(obj.GetComponent<UISprite>().spriteName);
            GlobalData.ServInstance.UserTalk(expId);
            CloseChatUi();
        }



        /// <summary>
        /// 关闭聊天界面
        /// </summary>
        public void CloseChatUi()
        {
            UiGob.SetActive(false);
        }

        //打开聊天界面
        public void OpenChatUi()
        {
            UiGob.SetActive(true);
        }

        [SerializeField] protected string AdviceStr = "不能再输入过长内容了,建议用语音";

        private YxMessageBox _yxmegBox;
        public void OnInPutChange(UIInput inputobj)
        {
   

            if (inputobj.value.Length >= inputobj.characterLimit)
            {
                if (_yxmegBox == null)
                {
                    _yxmegBox = YxMessageBox.Show(AdviceStr, "", (box, btnName) =>
                        {
                            if (btnName == YxMessageBox.BtnMiddle)
                            {
                                _yxmegBox = null;
                            }
                        });
                }

            }
        }

    }
}
