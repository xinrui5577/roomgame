/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_SIGNALR

using System;

using UnityEngine;
using BestHTTP.SignalR;
using BestHTTP.Examples;

sealed class SimpleStreamingSample : MonoBehaviour
{
    readonly Uri URI = new Uri("https://besthttpsignalr.azurewebsites.net/streaming-connection");

    /// <summary>
    /// Reference to the SignalR Connection
    /// </summary>
    Connection signalRConnection;

    /// <summary>
    /// Helper GUI class to handle and display a string-list
    /// </summary>
    GUIMessageList messages = new GUIMessageList();

    #region Unity Events

    void Start()
    {
        // Create the SignalR connection
        signalRConnection = new Connection(URI);

        // set event handlers
        signalRConnection.OnNonHubMessage += signalRConnection_OnNonHubMessage;
        signalRConnection.OnStateChanged += signalRConnection_OnStateChanged;
        signalRConnection.OnError += signalRConnection_OnError;

        // Start connecting to the server
        signalRConnection.Open();
    }

    void OnDestroy()
    {
        // Close the connection when the sample is closed
        signalRConnection.Close();
    }

    void OnGUI()
    {
        GUIHelper.DrawArea(GUIHelper.ClientArea, true, () =>
        {
            GUILayout.Label("Messages");

            GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                messages.Draw(Screen.width - 20, 0);
            GUILayout.EndHorizontal();
        });
    }

    #endregion

    #region SignalR Events

    /// <summary>
    /// Handle Server-sent messages
    /// </summary>
    void signalRConnection_OnNonHubMessage(Connection connection, object data)
    {
        messages.Add("[Server Message] " + data.ToString());
    }

    /// <summary>
    /// Display state changes
    /// </summary>
    void signalRConnection_OnStateChanged(Connection connection, ConnectionStates oldState, ConnectionStates newState)
    {
        messages.Add(string.Format("[State Change] {0} => {1}", oldState, newState));
    }

    /// <summary>
    /// Display errors.
    /// </summary>
    void signalRConnection_OnError(Connection connection, string error)
    {
        messages.Add("[Error] " + error);
    }

    #endregion
}

#endif
