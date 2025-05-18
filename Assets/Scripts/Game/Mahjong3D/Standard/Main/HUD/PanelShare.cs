using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [UIPanelData(typeof(PanelShare), UIPanelhierarchy.System)]
    public class PanelShare : UIPanelBase
    {
        private CompressImg mCompressImg;

        public override void Open()
        {
            if (mCompressImg == null)
            {
                mCompressImg = new CompressImg();
            }
            mCompressImg.DoScreenShot(this, new Rect(0, 0, Screen.width, Screen.height), callback =>
            {
                //生成图片之后再打开分享界面            
                base.Open();
            });
        }

        /// <summary>
        /// 分享到微信
        /// </summary>
        public void OnshareToWechat()
        {
            GameUtils.WeChatShareGameResult(mCompressImg.SShotImgpath);
        }

        /// <summary>
        /// 分享到朋友圈
        /// </summary>
        public void OnshareToFriends()
        {

        }

        /// <summary>
        /// 分享到亲友圈
        /// </summary>
        public void OnshareToKinFriends()
        {

        }
    }
}