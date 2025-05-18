using System;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using Assets.Scripts.Tea.House;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.Photo
{
    /// <summary>
    /// 亲友圈相册功能
    /// </summary>
    public class SocialPhotoView : BaseSocialSelectWrapListView
    {
        [Tooltip("当前玩家事件")]
        public List<EventDelegate> SelfAction = new List<EventDelegate>();
        [Tooltip("显示最大数量")]
        public int MaxShow = 3;
        [Tooltip("图片Url集合")]
        private List<string> _imageUrls = new List<string>();
        /// <summary>
        /// 文件名称格式
        /// </summary>
        private string _fileNameFormat = "亲友圈相册截图{0}.jpg";
        public bool IsSelfView { get; private set; }
        /// <summary>
        /// 是否需要显示上传提示
        /// </summary>
        public bool NeedShowUploadNotice { get; private set; }

        public string ViewImId { private set; get; }

        protected override void AddListeners()
        {
            AddEventListener<Dictionary<string, object>>(InitAction, OnInitReceive);
            AddEventListener<Dictionary<string, object>>(PartAction, OnChangePhotoData);
        }

        protected override void RemoveListeners()
        {
            RemoveEventListener<Dictionary<string, object>>(InitAction, OnInitReceive);
            RemoveEventListener<Dictionary<string, object>>(PartAction, OnChangePhotoData);
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var dic = Data as Dictionary<string, object>;
            if (dic != null)
            {
                string imId;
                dic.TryGetValueWitheKey(out imId, SocialTools.KeyImId);
                ViewImId = imId;
                IsSelfView = imId.Equals(CurrentImId);
                Manager.SendRequest(InitAction, dic);
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(SelfAction.WaitExcuteCalls());
                }
            }
        }

        protected override void OnInitDataValid()
        {
            List<string> photoData;
            InitGetData.TryGetStringListWithKey(out photoData, SocialTools.KeyPhotoData);
            int upLoadNum;
            InitGetData.TryGetValueWitheKey(out upLoadNum, SocialTools.KeyPhotoUploadNum);
            NeedShowUploadNotice = upLoadNum == 0;
            var count = Math.Min(MaxShow, photoData.Count);
            PageIds.Clear();
            _imageUrls.Clear();
            var newData = new Dictionary<string, Dictionary<string, object>>();
            if (photoData.Count!= 0)
            {
                for (int i = 0; i < count; i++)
                {
                    var phptoData = photoData[i];
                    if (!string.IsNullOrEmpty(phptoData))
                    {
                        var key = i.ToString();
                        _imageUrls.Add(phptoData);
                        PageIds.Add(key);
                        newData[key] = new Dictionary<string, object>()
                        {
                            { SocialTools.KeyId,key}, { SocialTools.KeyData,phptoData}
                        };
                    }
                }
                if (IsSelfView)
                {
                    var urlCount = _imageUrls.Count;
                    if (urlCount>0&& urlCount<MaxShow)
                    {
                        var key = urlCount.ToString();
                        PageIds.Add(key);
                        newData[key] = new Dictionary<string, object>()
                        {
                            { SocialTools.KeyId,key}, { SocialTools.KeyData,string.Empty}
                        };
                    }
                }
            }
            FreshWrapList(newData);
        }

        private void OnChangePhotoData(Dictionary<string, object> photoData)
        {
            if (Data is Dictionary<string, object>)
            {
                Manager.SendRequest(InitAction, Data as Dictionary<string, object>);
            }
        }
        /// <summary>
        /// 添加新的图片
        /// </summary>
        /// <param name="viewId"></param>
        public void AddNewImage(string viewId)
        {
#if UNITY_EDITOR
            string url = "Assets/Skins/DefaultSkins/skin_0021/loading/LOGO 1.png";
            url = Path.GetFullPath(url);
            TalkCenter.LocalImageUpLoad(delegate (string uploadUrl)
                {
                    CheckIndex(viewId, delegate { _imageUrls.Add(uploadUrl); });
                }, url);
            return;
#endif
            TalkCenter.SelectImageUpLoad(delegate (string uploadUrl)
            {
                CheckIndex(viewId, delegate { _imageUrls.Add(uploadUrl);});
            });
        }

        /// <summary>
        /// 检测索引
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="finishAction"></param>
        private bool CheckIndex(string viewId, Action finishAction)
        {
            if (_imageUrls.Count <= MaxShow)
            {
                var index = PageIds.FindIndex(item => item == viewId);
                if (index > -1|| string.IsNullOrEmpty(viewId))
                {
                    if (finishAction != null)
                    {
                        finishAction();
                    }
                    ChangePhotoImage();
                    return true;
                }
            }
            return false;
        }

        private void ChangePhotoImage()
        {
            SendSocialMessage(PartAction, new Dictionary<string, object>()
            {
                {SocialTools.KeyPhotoData, _imageUrls}
            });
        }

        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="viewId"></param>
        public void DeleteImage(string viewId)
        {
            CheckIndex(viewId, delegate
            {
                var url = GetItemImageUrl(viewId);
                if (!string.IsNullOrEmpty(url))
                {
                    if (_imageUrls.Contains(url))
                    {
                        _imageUrls.Remove(url);
                    }
                }
            });
        }

        /// <summary>
        /// 获取item url
        /// </summary>
        /// <param name="viewId"></param>
        /// <returns></returns>
        private string GetItemImageUrl(string viewId)
        {
            if (IdsDataDic.ContainsKey(viewId))
            {
                var dic = IdsDataDic[viewId];
                if (dic != null)
                {
                    string imageUrl;
                    dic.TryGetValueWitheKey(out imageUrl, SocialTools.KeyData);
                    return imageUrl;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 分享图片到微信
        /// </summary>
        public void ShareImageToWeiChat(string viewId)
        {
            var imageUrl = GetItemImageUrl(viewId);
            if (!string.IsNullOrEmpty(imageUrl))
            {
                YxTools.ShareImageWithGameKey(imageUrl, SharePlat.WxSenceSession);
            }
        }

        /// <summary>
        /// 下载图片到本地
        /// </summary>
        public void DownLoadImageToLocal(string viewId)
        {
            var imageUrl = GetItemImageUrl(viewId);
            if (!string.IsNullOrEmpty(imageUrl))
            {
                Facade.Instance<DownLoadTool>().DownAndSave(imageUrl, string.Format(_fileNameFormat, DateTime.Now.Ticks), delegate { }, delegate (string loaclPath)
                 {
                     NativeGallery.SaveImageToGallery(loaclPath,SocialTools.KeyPhotoAlbumName, string.Format(_fileNameFormat, DateTime.Now.Ticks), delegate
                         {
                             YxMessageBox.Show(SocialTools.KeyNoticePhotoDownload);
                         });
                 });

            }
        }
        /// <summary>
        /// 设置布局
        /// </summary>
        /// <param name="layout"></param>
        /// <param name="showParent"></param>
        public void ShowLayout(TeaTableLayout layout,Transform showParent)
        {
            List<Vector3> setPos;
            if (IsSelfView)
            {
                setPos=layout.GetPosByNum(MaxShow);
            }
            else
            {
                setPos = layout.GetPosByNum(_imageUrls.Count);
            }
            if (setPos.Count>0)
            {
                showParent.localPosition = setPos[0];
            }
        }

       
    }
}
