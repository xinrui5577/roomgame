using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace Assets.Scripts.Game.fillpit
{
    public class SettingMenu : MonoBehaviour
    {

        /// <summary>
        /// 关闭按钮
        /// </summary>
        [SerializeField]
        private UIButton _closeBtn = null;

        /// <summary>
        /// 退出按钮
        /// </summary>
        [SerializeField]
        private UIButton _quitBtn = null;

        /// <summary>
        /// 音乐设置拉条
        /// </summary>
        [SerializeField]
        private UISlider _musicSlider = null;


        /// <summary>
        /// 音效设置拉条
        /// </summary>
        [SerializeField]
        private UISlider _soundSlider = null;

        /// <summary>
        /// 解散房间Normal状态图片名
        /// </summary>
        [SerializeField] private string _dismissRoomBtnSprNormalName = string.Empty;

        /// <summary>
        /// 解散房间Over状态图片名
        /// </summary>
        [SerializeField] private string _dismissRoomBtnSprOvername = string.Empty;

        /// <summary>
        /// 退出房间Normal状态图片名
        /// </summary>
        [SerializeField] private string _quitBtnSprNormalName = string.Empty;

        /// <summary>
        /// 退出房间Over状态图片名
        /// </summary>
        [SerializeField] private string _quitBtnSprOverName = string.Empty;

        

        // Use this for initialization
        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once UnusedMember.Local
        void Start()
        {
            //初始化按钮事件
            _closeBtn.onClick.Add(new EventDelegate(OnClickClose));
            _quitBtn.onClick.Add(new EventDelegate( App.GetGameManager<FillpitGameManager>().DismissRoomMgr.SetDismissRoomBtn));

            //初始化声音设置
            _soundSlider.value = Facade.Instance<MusicManager>().EffectVolume;
            _musicSlider.value = Facade.Instance<MusicManager>().MusicVolume;
        }

        private void OnClickClose()
        {
            gameObject.SetActive(false);
        }

        // ReSharper disable once UnusedMember.Local
        void OnEnable()
        {
            InitQuitBtn();
        }

        void InitQuitBtn()
        {
            var gdata = App.GetGameData<FillpitGameData>();
            bool couldDismiss = gdata.IsRoomGame && (gdata.SelfSeat == 0 || gdata.IsPlayed);
                                
            if (couldDismiss)
            {
                _quitBtn.normalSprite = _dismissRoomBtnSprNormalName;
                _quitBtn.disabledSprite = _dismissRoomBtnSprNormalName;
                _quitBtn.hoverSprite = _dismissRoomBtnSprOvername;
                _quitBtn.pressedSprite = _dismissRoomBtnSprOvername;
            }
            else
            {
                _quitBtn.normalSprite = _quitBtnSprNormalName;
                _quitBtn.disabledSprite = _quitBtnSprNormalName;
                _quitBtn.hoverSprite = _quitBtnSprOverName;
                _quitBtn.pressedSprite = _quitBtnSprOverName;
            }

            var spr = _quitBtn.GetComponent<UISprite>();
            if (spr != null)
                spr.MakePixelPerfect();
        }

        public void SetMusicValue()
        {
            Facade.Instance<MusicManager>().MusicVolume = _musicSlider.value;
        }

        public void SetSoundValue()
        {
            Facade.Instance<MusicManager>().EffectVolume = _soundSlider.value;

        }
    }
}