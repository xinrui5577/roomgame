/*
http://www.cgsoso.com/forum-211-1.html

CG鎼滄悳 Unity3d 姣忔棩Unity3d鎻掍欢鍏嶈垂鏇存柊 鏇存湁VIP璧勬簮锛?

CGSOSO 涓绘墦娓告垙寮鍙戯紝褰辫?璁捐?绛塁G璧勬簮绱犳潗銆佁

鎻掍欢濡傝嫢鍟嗙敤锛岃?鍔″繀瀹樼綉璐?拱锛?

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using BestHTTP;

namespace BestHTTP.Examples
{
    public sealed class AssetBundleSample : MonoBehaviour
    {
        /// <summary>
        /// The url of the resource to download
        /// </summary>
#if UNITY_WEBGL && !UNITY_EDITOR
    const string URL = "https://besthttp.azurewebsites.net/Content/AssetBundle_v5.html";
#else
        const string URL = "https://besthttp.azurewebsites.net/Content/AssetBundle.html";
#endif

        #region Private Fields

        /// <summary>
        /// Debug status text
        /// </summary>
        string status = "Waiting for user interaction";

        /// <summary>
        /// The downloaded and cached AssetBundle
        /// </summary>
        AssetBundle cachedBundle;

        /// <summary>
        /// The loaded texture from the AssetBundle
        /// </summary>
        Texture2D texture;

        /// <summary>
        /// A flag that indicates that we are processing the request/bundle to hide the "Start Download" button.
        /// </summary>
        bool downloading;

        #endregion

        #region Unity Events

        void OnGUI()
        {
            GUIHelper.DrawArea(GUIHelper.ClientArea, true, () =>
                {
                    GUILayout.Label("Status: " + status);

                // Draw the texture from the downloaded bundle
                if (texture != null)
                        GUILayout.Box(texture, GUILayout.MaxHeight(256));

                    if (!downloading && GUILayout.Button("Start Download"))
                    {
                        UnloadBundle();

                        StartCoroutine(DownloadAssetBundle());
                    }
                });
        }

        void OnDestroy()
        {
            UnloadBundle();
        }

        #endregion

        #region Private Helper Functions

        IEnumerator DownloadAssetBundle()
        {
            downloading = true;

            // Create and send our request
            var request = new HTTPRequest(new Uri(URL)).Send();

            status = "Download started";

            // Wait while it's finishes and add some fancy dots to display something while the user waits for it.
            // A simple "yield return StartCoroutine(request);" would do the job too.
            while (request.State < HTTPRequestStates.Finished)
            {
                yield return new WaitForSeconds(0.1f);

                status += ".";
            }

            // Check the outcome of our request.
            switch (request.State)
            {
                // The request finished without any problem.
                case HTTPRequestStates.Finished:

                    if (request.Response.IsSuccess)
                    {
#if !BESTHTTP_DISABLE_CACHING && (!UNITY_WEBGL || UNITY_EDITOR)
                        status = string.Format("AssetBundle downloaded! Loaded from local cache: {0}", request.Response.IsFromCache.ToString());
#else
                    status = "AssetBundle downloaded!";
#endif

                        // Start creating the downloaded asset bundle
                        AssetBundleCreateRequest async =
#if UNITY_5_3
                            AssetBundle.LoadFromMemoryAsync(request.Response.Data);
#else
                            AssetBundle.LoadFromMemoryAsync(request.Response.Data);
#endif

                        // wait for it
                        yield return async;

                        // And process the bundle
                        yield return StartCoroutine(ProcessAssetBundle(async.assetBundle));
                    }
                    else
                    {
                        status = string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}",
                                                        request.Response.StatusCode,
                                                        request.Response.Message,
                                                        request.Response.DataAsText);
                        Debug.LogWarning(status);
                    }

                    break;

                // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
                case HTTPRequestStates.Error:
                    status = "Request Finished with Error! " + (request.Exception != null ? (request.Exception.Message + "\n" + request.Exception.StackTrace) : "No Exception");
                    Debug.LogError(status);
                    break;

                // The request aborted, initiated by the user.
                case HTTPRequestStates.Aborted:
                    status = "Request Aborted!";
                    Debug.LogWarning(status);
                    break;

                // Ceonnecting to the server is timed out.
                case HTTPRequestStates.ConnectionTimedOut:
                    status = "Connection Timed Out!";
                    Debug.LogError(status);
                    break;

                // The request didn't finished in the given time.
                case HTTPRequestStates.TimedOut:
                    status = "Processing the request Timed Out!";
                    Debug.LogError(status);
                    break;
            }

            downloading = false;
        }

        /// <summary>
        /// In this function we can do whatever we want with the freshly downloaded bundle.
        /// In this example we will cache it for later use, and we will load a texture from it.
        /// </summary>
        IEnumerator ProcessAssetBundle(AssetBundle bundle)
        {
            if (bundle == null)
                yield break;

            // Save the bundle for future use
            cachedBundle = bundle;

            // Start loading the asset from the bundle
            var asyncAsset =
#if UNITY_5
            cachedBundle.LoadAssetAsync("9443182_orig", typeof(Texture2D));
#else
        
            cachedBundle.LoadAsync("9443182_orig", typeof(Texture2D));
#endif

            // wait til load
            yield return asyncAsset;

            // get the texture
            texture = asyncAsset.asset as Texture2D;
        }

        void UnloadBundle()
        {
            if (cachedBundle != null)
            {
                cachedBundle.Unload(true);
                cachedBundle = null;
            }
        }

        #endregion
    }
}
