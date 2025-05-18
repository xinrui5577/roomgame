using System;
using System.IO;
using Assets.Scripts.Common.Adapters;
using com.yxixia.utile.Utiles;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Common.Interfaces;
using YxFramwork.Enums;
using YxFramwork.Manager;
using YxFramwork.View;

// ReSharper disable All

namespace Assets.Scripts.Common.Interface
{
    public class UIImpl : IUI
    {
        public Vector2 ScreenSize;

        public UIImpl(Vector2 screenSize)
        {
            ScreenSize = screenSize;
        }

         
        public Transform CreateWindowContainer(YxEUIType uiType,int layer,Transform parentTs)
        {
            switch (uiType)
            {
                case YxEUIType.Default:
                case YxEUIType.Nguid:
                    return CreateNguiWindowContainer(layer,parentTs);
                case YxEUIType.Ugui:
                    return CreateUguiWindowContainer(layer, parentTs);
            }
            return null;
        }


        /// <summary>
        /// 窗口使用的专用窗口
        /// </summary>
        /// <returns></returns>
        public Camera GetWindowCamera()
        {
            var ts = YxWindowManager.Instance.GetWindowContainer(YxEUIType.Default);
            return ts == null ? null : ts.GetComponentInChildren<Camera>();
        }

        public YxEUIType GetDefualtType()
        {
            return YxEUIType.Nguid;
        }

        public string CaptureScreenshot()
        {
            var time = DateTime.Now.ToString("yyyyMMddHHmmss");
            // ReSharper disable once UseStringInterpolation
            var pngName = string.Format("Screenshot{0}.png", time);
            const string folder = "Screenshots";
//            var path = Application.isMobilePlatform ? Application.persistentDataPath.CombinePath(folder) : Application.dataPath.CombinePath("..").CombinePath(folder);
            var captureFile = folder.CombinePath(pngName);
            //Debug.Log(captureFile);
            Application.CaptureScreenshot(captureFile);
            var path = App.GetGameDataPath(folder);
            //            path = Path.GetFullPath(path);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            // ReSharper disable once UseStringInterpolation
            var pngPath = path.CombinePath(pngName);
            //Debug.Log(pngPath);
            // ReSharper disable once UseStringInterpolation
            return Application.isMobilePlatform ? pngPath : Path.GetFullPath(pngPath);
        }

        /// <summary>
        /// 创建ngui窗口Root
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        private Transform CreateNguiWindowContainer(int layer, Transform parentTs)
        {
            var ui2d = NGUITools.CreateUI(false, layer);
            var ts = ui2d.transform;
            GameObjectUtile.ResetTransformInfo(ts, parentTs);
            var uiroot = ui2d.GetComponent<UIRoot>();
            uiroot.scalingStyle = UIRoot.Scaling.ConstrainedOnMobiles;
            uiroot.manualWidth = (int)ScreenSize.x;
            uiroot.manualHeight = (int)ScreenSize.y;
            uiroot.fitWidth = true;
            uiroot.fitHeight = true;
            var uiC = ui2d.GetComponentInChildren<Camera>();
            uiC.clearFlags = CameraClearFlags.Depth;
            uiC.depth = 100;
            uiC.gameObject.AddComponent<YxBaseCameraAssist>();
            return ts;
        }

        /// <summary>
        /// 创建ugui窗口Root
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        private Transform CreateUguiWindowContainer(int layer, Transform parentTs)
        { 
            var go = new GameObject { layer = layer };
            GameObjectUtile.ResetTransformInfo(go.transform, parentTs);
            var ts = go.transform;
            var canvas = go.AddComponent<Canvas>();
            canvas.enabled = true;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = GetWindowCamera();
            canvas.planeDistance = 0;
            var canvasScaler = go.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = ScreenSize;
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            return go.transform; 
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="uiType"></param>
        /// <returns></returns>
        public YxWindowCoverInfo GetWinCoverStyle(YxEUIType uiType, int layer)
        {
            switch (uiType)
            {
                case YxEUIType.Nguid:
                    return CreateNguiCoverStyle(layer);
                case YxEUIType.Ugui:
                    return CreateUguiCoverStyle(layer);
            }
            return null;
        }

        private YxWindowCoverInfo CreateUguiCoverStyle(int layer)
        {
            var coverInfo = new YxWindowCoverInfo();
            var go = new GameObject("UguiCover") { layer = layer };
            var ts = go.transform;
            var p = YxWindowManager.Instance.GetWindowContainer(YxEUIType.Ugui);
            GameObjectUtile.ResetTransformInfo(ts, p);
            var canvas = go.AddComponent<Canvas>();
            canvas.enabled = true;
            canvas.overrideSorting = true;
            var raycaster = go.AddComponent<GraphicRaycaster>();
            //            var collider = go.AddComponent<BoxCollider>(); 
            var rectTs = go.GetComponent<RectTransform>();
            var uguiPanelAdapter = go.AddComponent<UguiPanelAdapter>();
            uguiPanelAdapter.OverrideSorting = true;
            uguiPanelAdapter.IsMainPanel = true;
            var textureGo = new GameObject("Background");
            textureGo.layer = layer;
            GameObjectUtile.ResetTransformInfo(textureGo.transform, rectTs);
            var background = textureGo.AddComponent<RawImage>();
            var color = Color.black;
            color.a = 0.5f;
            background.color = color;
            coverInfo.Panel = uguiPanelAdapter;
            var textureAdapter = textureGo.AddComponent<UguiRawImageAdapter>();
            textureAdapter.SetAnchor(go,0,0,0,0);
            coverInfo.Background = textureAdapter;
            return coverInfo;
        }

        private YxWindowCoverInfo CreateNguiCoverStyle(int layer)
        {
            var coverInfo = new YxWindowCoverInfo();
            var go = new GameObject("NguiCover") { layer = layer };
            var ts = go.transform;
            var p = YxWindowManager.Instance.GetWindowContainer(YxEUIType.Nguid);
            GameObjectUtile.ResetTransformInfo(ts, p);
            go.AddComponent<UIPanel>();
            var textureGo = new GameObject("Background");
            textureGo.layer = layer;
            GameObjectUtile.ResetTransformInfo(textureGo.transform, ts);
            var box = textureGo.AddComponent<BoxCollider>();
            var size = box.size;
            size.z = 2;
            box.size = size;
            var uitexture = textureGo.AddComponent<UITexture>();
            uitexture.SetAnchor(go);
            uitexture.autoResizeBoxCollider = true;
            uitexture.ResizeCollider();
            coverInfo.Panel = go.AddComponent<NguiPanelAdapter>();
            coverInfo.Background = textureGo.AddComponent<NguiTextureAdapter>();
            return coverInfo;
        }
    }
}
