/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using System;

namespace BestHTTP.Examples
{
    /// <summary>
    /// The html code for all the samples. These are used only in the WebPlayer demo.
    /// </summary>
    public static class CodeBlocks
    {
        #region TextureDownloadSample
        public static string TextureDownloadSample = @"<pre style=""font-family:Consolas;font-size:13;color:black;background:silver;""><span style=""color:blue;"">using</span>&nbsp;System;
<span style=""color:blue;"">using</span>&nbsp;System.Collections.Generic;

<span style=""color:blue;"">using</span>&nbsp;UnityEngine;
<span style=""color:blue;"">using</span>&nbsp;BestHTTP;

<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">sealed</span>&nbsp;<span style=""color:blue;"">class</span>&nbsp;<span style=""color:#2b91af;"">TextureDownloadSample</span>&nbsp;:&nbsp;<span style=""color:#2b91af;"">MonoBehaviour</span>
{
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;The&nbsp;URL&nbsp;of&nbsp;the&nbsp;server&nbsp;that&nbsp;will&nbsp;serve&nbsp;the&nbsp;image&nbsp;resources</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">const</span>&nbsp;<span style=""color:blue;"">string</span>&nbsp;BaseURL&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;http://besthttp.azurewebsites.net/Content/&quot;</span>;

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Private&nbsp;Fields

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;The&nbsp;downloadable&nbsp;images</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>[]&nbsp;Images&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:blue;"">string</span>[9]&nbsp;{&nbsp;<span style=""color:#a31515;"">&quot;One.png&quot;</span>,&nbsp;<span style=""color:#a31515;"">&quot;Two.png&quot;</span>,&nbsp;<span style=""color:#a31515;"">&quot;Three.png&quot;</span>,&nbsp;<span style=""color:#a31515;"">&quot;Four.png&quot;</span>,&nbsp;<span style=""color:#a31515;"">&quot;Five.png&quot;</span>,&nbsp;<span style=""color:#a31515;"">&quot;Six.png&quot;</span>,&nbsp;<span style=""color:#a31515;"">&quot;Seven.png&quot;</span>,&nbsp;<span style=""color:#a31515;"">&quot;Eight.png&quot;</span>,&nbsp;<span style=""color:#a31515;"">&quot;Nine.png&quot;</span>&nbsp;};

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;The&nbsp;downloaded&nbsp;images&nbsp;will&nbsp;be&nbsp;stored&nbsp;as&nbsp;textures&nbsp;in&nbsp;this&nbsp;array</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Texture2D</span>[]&nbsp;Textures&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">Texture2D</span>[9];

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;True&nbsp;if&nbsp;all&nbsp;images&nbsp;are&nbsp;loaded&nbsp;from&nbsp;the&nbsp;local&nbsp;cache&nbsp;instead&nbsp;of&nbsp;the&nbsp;server</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">bool</span>&nbsp;allDownloadedFromLocalCache;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;How&nbsp;many&nbsp;sent&nbsp;requests&nbsp;are&nbsp;finished</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">int</span>&nbsp;finishedCount;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;GUI&nbsp;scroll&nbsp;position</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Vector2</span>&nbsp;scrollPos;

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Unity&nbsp;Events

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;Awake()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Set&nbsp;a&nbsp;well&nbsp;observable&nbsp;value</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;This&nbsp;is&nbsp;how&nbsp;many&nbsp;concurrent&nbsp;requests&nbsp;can&nbsp;be&nbsp;made&nbsp;to&nbsp;a&nbsp;server</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">HTTPManager</span>.MaxConnectionPerServer&nbsp;=&nbsp;1;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Create&nbsp;placeholder&nbsp;textures</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">for</span>&nbsp;(<span style=""color:blue;"">int</span>&nbsp;i&nbsp;=&nbsp;0;&nbsp;i&nbsp;&lt;&nbsp;Images.Length;&nbsp;++i)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Textures[i]&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">Texture2D</span>(100,&nbsp;150);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnDestroy()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Set&nbsp;back&nbsp;to&nbsp;its&nbsp;defualt&nbsp;value.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">HTTPManager</span>.MaxConnectionPerServer&nbsp;=&nbsp;4;
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnGUI()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUIHelper</span>.DrawArea(<span style=""color:#2b91af;"">GUIHelper</span>.ClientArea,&nbsp;<span style=""color:blue;"">true</span>,&nbsp;()&nbsp;=&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;scrollPos&nbsp;=&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginScrollView(scrollPos);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Draw&nbsp;out&nbsp;the&nbsp;textures</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.SelectionGrid(0,&nbsp;Textures,&nbsp;3);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(finishedCount&nbsp;==&nbsp;Images.Length&nbsp;&amp;&amp;&nbsp;allDownloadedFromLocalCache)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUIHelper</span>.DrawCenteredText(<span style=""color:#a31515;"">&quot;All&nbsp;images&nbsp;loaded&nbsp;from&nbsp;the&nbsp;local&nbsp;cache!&quot;</span>);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.FlexibleSpace();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Max&nbsp;Connection/Server:&nbsp;&quot;</span>,&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Width(150));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#2b91af;"">HTTPManager</span>.MaxConnectionPerServer.ToString(),&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Width(20));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">HTTPManager</span>.MaxConnectionPerServer&nbsp;=&nbsp;(<span style=""color:blue;"">byte</span>)<span style=""color:#2b91af;"">GUILayout</span>.HorizontalSlider(<span style=""color:#2b91af;"">HTTPManager</span>.MaxConnectionPerServer,&nbsp;1,&nbsp;10);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">GUILayout</span>.Button(<span style=""color:#a31515;"">&quot;Start&nbsp;Download&quot;</span>))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DownloadImages();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndScrollView();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;});
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Private&nbsp;Helper&nbsp;Functions

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;DownloadImages()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Set&nbsp;these&nbsp;metadatas&nbsp;to&nbsp;its&nbsp;initial&nbsp;values</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;allDownloadedFromLocalCache&nbsp;=&nbsp;<span style=""color:blue;"">true</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;finishedCount&nbsp;=&nbsp;0;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">for</span>&nbsp;(<span style=""color:blue;"">int</span>&nbsp;i&nbsp;=&nbsp;0;&nbsp;i&nbsp;&lt;&nbsp;Images.Length;&nbsp;++i)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Set&nbsp;a&nbsp;blank&nbsp;placeholder&nbsp;texture,&nbsp;overriding&nbsp;previously&nbsp;downloaded&nbsp;texture</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Textures[i]&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">Texture2D</span>(100,&nbsp;150);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Construct&nbsp;the&nbsp;request</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">var</span>&nbsp;request&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">HTTPRequest</span>(<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">Uri</span>(BaseURL&nbsp;+&nbsp;Images[i]),&nbsp;ImageDownloaded);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Set&nbsp;the&nbsp;Tag&nbsp;property,&nbsp;we&nbsp;can&nbsp;use&nbsp;it&nbsp;as&nbsp;a&nbsp;general&nbsp;storage&nbsp;bound&nbsp;to&nbsp;the&nbsp;request</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request.Tag&nbsp;=&nbsp;Textures[i];

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Send&nbsp;out&nbsp;the&nbsp;request</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request.Send();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Callback&nbsp;function&nbsp;of&nbsp;the&nbsp;image&nbsp;download&nbsp;http&nbsp;requests</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;ImageDownloaded(<span style=""color:#2b91af;"">HTTPRequest</span>&nbsp;req,&nbsp;<span style=""color:#2b91af;"">HTTPResponse</span>&nbsp;resp)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Increase&nbsp;the&nbsp;finished&nbsp;count&nbsp;regardless&nbsp;of&nbsp;the&nbsp;state&nbsp;of&nbsp;our&nbsp;request</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;finishedCount++;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">switch</span>&nbsp;(req.State)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;The&nbsp;request&nbsp;finished&nbsp;without&nbsp;any&nbsp;problem.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#2b91af;"">HTTPRequestStates</span>.Finished:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(resp.IsSuccess)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Get&nbsp;the&nbsp;Texture&nbsp;from&nbsp;the&nbsp;Tag&nbsp;property</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Texture2D</span>&nbsp;tex&nbsp;=&nbsp;req.Tag&nbsp;<span style=""color:blue;"">as</span>&nbsp;<span style=""color:#2b91af;"">Texture2D</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Load&nbsp;the&nbsp;texture</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;tex.LoadImage(resp.Data);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Update&nbsp;the&nbsp;cache-info&nbsp;variable</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;allDownloadedFromLocalCache&nbsp;=&nbsp;allDownloadedFromLocalCache&nbsp;&amp;&amp;&nbsp;resp.IsFromCache;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">else</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Debug</span>.LogWarning(<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;Request&nbsp;finished&nbsp;Successfully,&nbsp;but&nbsp;the&nbsp;server&nbsp;sent&nbsp;an&nbsp;error.&nbsp;Status&nbsp;Code:&nbsp;{0}-{1}&nbsp;Message:&nbsp;{2}&quot;</span>,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;resp.StatusCode,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;resp.Message,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;resp.DataAsText));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">break</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;The&nbsp;request&nbsp;finished&nbsp;with&nbsp;an&nbsp;unexpected&nbsp;error.&nbsp;The&nbsp;request&#39;s&nbsp;Exception&nbsp;property&nbsp;may&nbsp;contain&nbsp;more&nbsp;info&nbsp;about&nbsp;the&nbsp;error.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#2b91af;"">HTTPRequestStates</span>.Error:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Debug</span>.LogError(<span style=""color:#a31515;"">&quot;Request&nbsp;Finished&nbsp;with&nbsp;Error!&nbsp;&quot;</span>&nbsp;+&nbsp;(req.Exception&nbsp;!=&nbsp;<span style=""color:blue;"">null</span>&nbsp;?&nbsp;(req.Exception.Message&nbsp;+&nbsp;<span style=""color:#a31515;"">&quot;\n&quot;</span>&nbsp;+&nbsp;req.Exception.StackTrace)&nbsp;:&nbsp;<span style=""color:#a31515;"">&quot;No&nbsp;Exception&quot;</span>));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">break</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;The&nbsp;request&nbsp;aborted,&nbsp;initiated&nbsp;by&nbsp;the&nbsp;user.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#2b91af;"">HTTPRequestStates</span>.Aborted:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Debug</span>.LogWarning(<span style=""color:#a31515;"">&quot;Request&nbsp;Aborted!&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">break</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Ceonnecting&nbsp;to&nbsp;the&nbsp;server&nbsp;is&nbsp;timed&nbsp;out.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#2b91af;"">HTTPRequestStates</span>.ConnectionTimedOut:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Debug</span>.LogError(<span style=""color:#a31515;"">&quot;Connection&nbsp;Timed&nbsp;Out!&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">break</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;The&nbsp;request&nbsp;didn&#39;t&nbsp;finished&nbsp;in&nbsp;the&nbsp;given&nbsp;time.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#2b91af;"">HTTPRequestStates</span>.TimedOut:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Debug</span>.LogError(<span style=""color:#a31515;"">&quot;Processing&nbsp;the&nbsp;request&nbsp;Timed&nbsp;Out!&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">break</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>
}</pre>";
        #endregion

        #region WebSocketSample
        public static string WebSocketSample = @"<pre style=""font-family:Consolas;font-size:13;color:black;background:silver;""><span style=""color:blue;"">using</span>&nbsp;System;
<span style=""color:blue;"">using</span>&nbsp;UnityEngine;
<span style=""color:blue;"">using</span>&nbsp;BestHTTP;
<span style=""color:blue;"">using</span>&nbsp;BestHTTP.WebSocket;

<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">class</span>&nbsp;<span style=""color:#2b91af;"">WebSocketSample</span>&nbsp;:&nbsp;<span style=""color:#2b91af;"">MonoBehaviour</span>
{
<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Private&nbsp;Fields

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;The&nbsp;WebSocket&nbsp;address&nbsp;to&nbsp;connect</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;address&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;ws://echo.websocket.org&quot;</span>;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Default&nbsp;text&nbsp;to&nbsp;send</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;msgToSend&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;Hello&nbsp;World!&quot;</span>;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Debug&nbsp;text&nbsp;to&nbsp;draw&nbsp;on&nbsp;the&nbsp;gui</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;Text&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Saved&nbsp;WebSocket&nbsp;instance</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">WebSocket</span>&nbsp;webSocket;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;GUI&nbsp;scroll&nbsp;position</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Vector2</span>&nbsp;scrollPos;

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Unity&nbsp;Events

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnDestroy()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(webSocket&nbsp;!=&nbsp;<span style=""color:blue;"">null</span>)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;webSocket.Close();
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnGUI()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUIHelper</span>.DrawArea(<span style=""color:#2b91af;"">GUIHelper</span>.ClientArea,&nbsp;<span style=""color:blue;"">true</span>,&nbsp;()&nbsp;=&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;scrollPos&nbsp;=&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginScrollView(scrollPos);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(Text);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndScrollView();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(5);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.FlexibleSpace();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;address&nbsp;=&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.TextField(address);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(webSocket&nbsp;==&nbsp;<span style=""color:blue;"">null</span>&nbsp;&amp;&amp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Button(<span style=""color:#a31515;"">&quot;Open&nbsp;Web&nbsp;Socket&quot;</span>))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Create&nbsp;the&nbsp;WebSocket&nbsp;instance</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;webSocket&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">WebSocket</span>(<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">Uri</span>(address));

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">HTTPManager</span>.Proxy&nbsp;!=&nbsp;<span style=""color:blue;"">null</span>)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;webSocket.InternalRequest.Proxy&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">HTTPProxy</span>(<span style=""color:#2b91af;"">HTTPManager</span>.Proxy.Address,&nbsp;<span style=""color:#2b91af;"">HTTPManager</span>.Proxy.Credentials,&nbsp;<span style=""color:blue;"">false</span>);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Subscribe&nbsp;to&nbsp;the&nbsp;WS&nbsp;events</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;webSocket.OnOpen&nbsp;+=&nbsp;OnOpen;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;webSocket.OnMessage&nbsp;+=&nbsp;OnMessageReceived;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;webSocket.OnClosed&nbsp;+=&nbsp;OnClosed;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;webSocket.OnError&nbsp;+=&nbsp;OnError;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Start&nbsp;connecting&nbsp;to&nbsp;the&nbsp;server</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;webSocket.Open();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Text&nbsp;+=&nbsp;<span style=""color:#a31515;"">&quot;Opening&nbsp;Web&nbsp;Socket...\n&quot;</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(webSocket&nbsp;!=&nbsp;<span style=""color:blue;"">null</span>&nbsp;&amp;&amp;&nbsp;webSocket.IsOpen)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(10);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;msgToSend&nbsp;=&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.TextField(msgToSend);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">GUILayout</span>.Button(<span style=""color:#a31515;"">&quot;Send&quot;</span>,&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.MaxWidth(70)))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Text&nbsp;+=&nbsp;<span style=""color:#a31515;"">&quot;Sending&nbsp;message...\n&quot;</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Send&nbsp;message&nbsp;to&nbsp;the&nbsp;server</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;webSocket.Send(msgToSend);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(10);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">GUILayout</span>.Button(<span style=""color:#a31515;"">&quot;Close&quot;</span>))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Close&nbsp;the&nbsp;connection</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;webSocket.Close(1000,&nbsp;<span style=""color:#a31515;"">&quot;Bye!&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;});
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;WebSocket&nbsp;Event&nbsp;Handlers

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Called&nbsp;when&nbsp;the&nbsp;web&nbsp;socket&nbsp;is&nbsp;open,&nbsp;and&nbsp;we&nbsp;are&nbsp;ready&nbsp;to&nbsp;send&nbsp;and&nbsp;receive&nbsp;data</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnOpen(<span style=""color:#2b91af;"">WebSocket</span>&nbsp;ws)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Text&nbsp;+=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;-WebSocket&nbsp;Open!\n&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Called&nbsp;when&nbsp;we&nbsp;received&nbsp;a&nbsp;text&nbsp;message&nbsp;from&nbsp;the&nbsp;server</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnMessageReceived(<span style=""color:#2b91af;"">WebSocket</span>&nbsp;ws,&nbsp;<span style=""color:blue;"">string</span>&nbsp;message)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Text&nbsp;+=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;-Message&nbsp;received:&nbsp;{0}\n&quot;</span>,&nbsp;message);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Called&nbsp;when&nbsp;the&nbsp;web&nbsp;socket&nbsp;closed</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnClosed(<span style=""color:#2b91af;"">WebSocket</span>&nbsp;ws,&nbsp;<span style=""color:#2b91af;"">UInt16</span>&nbsp;code,&nbsp;<span style=""color:blue;"">string</span>&nbsp;message)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Text&nbsp;+=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;-WebSocket&nbsp;closed!&nbsp;Code:&nbsp;{0}&nbsp;Message:&nbsp;{1}\n&quot;</span>,&nbsp;code,&nbsp;message);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;webSocket&nbsp;=&nbsp;<span style=""color:blue;"">null</span>;
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Called&nbsp;when&nbsp;an&nbsp;error&nbsp;occured&nbsp;on&nbsp;client&nbsp;side</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnError(<span style=""color:#2b91af;"">WebSocket</span>&nbsp;ws,&nbsp;<span style=""color:#2b91af;"">Exception</span>&nbsp;ex)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;errorMsg&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(ws.InternalRequest.Response&nbsp;!=&nbsp;<span style=""color:blue;"">null</span>)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;errorMsg&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;Status&nbsp;Code&nbsp;from&nbsp;Server:&nbsp;{0}&nbsp;and&nbsp;Message:&nbsp;{1}&quot;</span>,&nbsp;ws.InternalRequest.Response.StatusCode,&nbsp;ws.InternalRequest.Response.Message);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Text&nbsp;+=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;-An&nbsp;error&nbsp;occured:&nbsp;{0}\n&quot;</span>,&nbsp;(ex&nbsp;!=&nbsp;<span style=""color:blue;"">null</span>&nbsp;?&nbsp;ex.Message&nbsp;:&nbsp;<span style=""color:#a31515;"">&quot;Unknown&nbsp;Error&nbsp;&quot;</span>&nbsp;+&nbsp;errorMsg));

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;webSocket&nbsp;=&nbsp;<span style=""color:blue;"">null</span>;
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>
}</pre>";
        #endregion

        #region AssetBundleSample
        public static string AssetBundleSample = @"<pre style=""font-family:Consolas;font-size:13;color:black;background:silver;""><span style=""color:blue;"">using</span>&nbsp;System;
<span style=""color:blue;"">using</span>&nbsp;System.Collections;
<span style=""color:blue;"">using</span>&nbsp;System.Collections.Generic;

<span style=""color:blue;"">using</span>&nbsp;UnityEngine;
<span style=""color:blue;"">using</span>&nbsp;BestHTTP;

<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">sealed</span>&nbsp;<span style=""color:blue;"">class</span>&nbsp;<span style=""color:#2b91af;"">AssetBundleSample</span>&nbsp;:&nbsp;<span style=""color:#2b91af;"">MonoBehaviour</span>
{
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;The&nbsp;url&nbsp;of&nbsp;the&nbsp;resource&nbsp;to&nbsp;download</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">const</span>&nbsp;<span style=""color:blue;"">string</span>&nbsp;URL&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;http://besthttp.azurewebsites.net/Content/AssetBundle.html&quot;</span>;

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Private&nbsp;Fields

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Debug&nbsp;status&nbsp;text</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;status&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;Waiting&nbsp;for&nbsp;user&nbsp;interaction&quot;</span>;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;The&nbsp;downloaded&nbsp;and&nbsp;cached&nbsp;AssetBundle</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">AssetBundle</span>&nbsp;cachedBundle;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;The&nbsp;loaded&nbsp;texture&nbsp;from&nbsp;the&nbsp;AssetBundle</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Texture2D</span>&nbsp;texture;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;A&nbsp;flag&nbsp;that&nbsp;indicates&nbsp;that&nbsp;we&nbsp;are&nbsp;processing&nbsp;the&nbsp;request/bundle&nbsp;to&nbsp;hide&nbsp;the&nbsp;&quot;Start&nbsp;Download&quot;&nbsp;button.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">bool</span>&nbsp;downloading;

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Unity&nbsp;Events

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnGUI()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUIHelper</span>.DrawArea(<span style=""color:#2b91af;"">GUIHelper</span>.ClientArea,&nbsp;<span style=""color:blue;"">true</span>,&nbsp;()&nbsp;=&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Status:&nbsp;&quot;</span>&nbsp;+&nbsp;status);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Draw&nbsp;the&nbsp;texture&nbsp;from&nbsp;the&nbsp;downloaded&nbsp;bundle</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(texture&nbsp;!=&nbsp;<span style=""color:blue;"">null</span>)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Box(texture,&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.MaxHeight(256));

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(!downloading&nbsp;&amp;&amp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Button(<span style=""color:#a31515;"">&quot;Start&nbsp;Download&quot;</span>))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnloadBundle();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;StartCoroutine(DownloadAssetBundle());
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;});
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnDestroy()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnloadBundle();
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Private&nbsp;Helper&nbsp;Functions

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">IEnumerator</span>&nbsp;DownloadAssetBundle()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;downloading&nbsp;=&nbsp;<span style=""color:blue;"">true</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Create&nbsp;and&nbsp;send&nbsp;our&nbsp;request</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">var</span>&nbsp;request&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">HTTPRequest</span>(<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">Uri</span>(URL)).Send();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;status&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;Download&nbsp;started&quot;</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Wait&nbsp;while&nbsp;it&#39;s&nbsp;finishes&nbsp;and&nbsp;add&nbsp;some&nbsp;fancy&nbsp;dots&nbsp;to&nbsp;display&nbsp;something&nbsp;while&nbsp;the&nbsp;user&nbsp;waits&nbsp;for&nbsp;it.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;A&nbsp;simple&nbsp;&quot;yield&nbsp;return&nbsp;StartCoroutine(request);&quot;&nbsp;would&nbsp;do&nbsp;the&nbsp;job&nbsp;too.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">while</span>(request.State&nbsp;&lt;&nbsp;<span style=""color:#2b91af;"">HTTPRequestStates</span>.Finished)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">yield</span>&nbsp;<span style=""color:blue;"">return</span>&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">WaitForSeconds</span>(0.1f);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;status&nbsp;+=&nbsp;<span style=""color:#a31515;"">&quot;.&quot;</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Check&nbsp;the&nbsp;outcome&nbsp;of&nbsp;our&nbsp;request.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">switch</span>&nbsp;(request.State)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;The&nbsp;request&nbsp;finished&nbsp;without&nbsp;any&nbsp;problem.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#2b91af;"">HTTPRequestStates</span>.Finished:

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(request.Response.IsSuccess)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;status&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;AssetBundle&nbsp;downloaded!&nbsp;Loaded&nbsp;from&nbsp;local&nbsp;cache:&nbsp;{0}&quot;</span>,&nbsp;request.Response.IsFromCache.ToString());

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Start&nbsp;creating&nbsp;the&nbsp;downloaded&nbsp;asset&nbsp;bundle</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">AssetBundleCreateRequest</span>&nbsp;async&nbsp;=&nbsp;<span style=""color:#2b91af;"">AssetBundle</span>.CreateFromMemory(request.Response.Data);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;wait&nbsp;for&nbsp;it</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">yield</span>&nbsp;<span style=""color:blue;"">return</span>&nbsp;async;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;And&nbsp;process&nbsp;the&nbsp;bundle</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">yield</span>&nbsp;<span style=""color:blue;"">return</span>&nbsp;StartCoroutine(ProcessAssetBundle(async.assetBundle));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">else</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;status&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;Request&nbsp;finished&nbsp;Successfully,&nbsp;but&nbsp;the&nbsp;server&nbsp;sent&nbsp;an&nbsp;error.&nbsp;Status&nbsp;Code:&nbsp;{0}-{1}&nbsp;Message:&nbsp;{2}&quot;</span>,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request.Response.StatusCode,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request.Response.Message,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request.Response.DataAsText);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Debug</span>.LogWarning(status);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">break</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;The&nbsp;request&nbsp;finished&nbsp;with&nbsp;an&nbsp;unexpected&nbsp;error.&nbsp;The&nbsp;request&#39;s&nbsp;Exception&nbsp;property&nbsp;may&nbsp;contain&nbsp;more&nbsp;info&nbsp;about&nbsp;the&nbsp;error.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#2b91af;"">HTTPRequestStates</span>.Error:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;status&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;Request&nbsp;Finished&nbsp;with&nbsp;Error!&nbsp;&quot;</span>&nbsp;+&nbsp;(request.Exception&nbsp;!=&nbsp;<span style=""color:blue;"">null</span>&nbsp;?&nbsp;(request.Exception.Message&nbsp;+&nbsp;<span style=""color:#a31515;"">&quot;\n&quot;</span>&nbsp;+&nbsp;request.Exception.StackTrace)&nbsp;:&nbsp;<span style=""color:#a31515;"">&quot;No&nbsp;Exception&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Debug</span>.LogError(status);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">break</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;The&nbsp;request&nbsp;aborted,&nbsp;initiated&nbsp;by&nbsp;the&nbsp;user.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#2b91af;"">HTTPRequestStates</span>.Aborted:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;status&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;Request&nbsp;Aborted!&quot;</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Debug</span>.LogWarning(status);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">break</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Ceonnecting&nbsp;to&nbsp;the&nbsp;server&nbsp;is&nbsp;timed&nbsp;out.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#2b91af;"">HTTPRequestStates</span>.ConnectionTimedOut:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;status&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;Connection&nbsp;Timed&nbsp;Out!&quot;</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Debug</span>.LogError(status);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">break</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;The&nbsp;request&nbsp;didn&#39;t&nbsp;finished&nbsp;in&nbsp;the&nbsp;given&nbsp;time.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#2b91af;"">HTTPRequestStates</span>.TimedOut:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;status&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;Processing&nbsp;the&nbsp;request&nbsp;Timed&nbsp;Out!&quot;</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Debug</span>.LogError(status);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">break</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;downloading&nbsp;=&nbsp;<span style=""color:blue;"">false</span>;
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;In&nbsp;this&nbsp;function&nbsp;we&nbsp;can&nbsp;do&nbsp;whatever&nbsp;we&nbsp;want&nbsp;with&nbsp;the&nbsp;freshly&nbsp;downloaded&nbsp;bundle.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;In&nbsp;this&nbsp;example&nbsp;we&nbsp;will&nbsp;cache&nbsp;it&nbsp;for&nbsp;later&nbsp;use,&nbsp;and&nbsp;we&nbsp;will&nbsp;load&nbsp;a&nbsp;texture&nbsp;from&nbsp;it.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">IEnumerator</span>&nbsp;ProcessAssetBundle(<span style=""color:#2b91af;"">AssetBundle</span>&nbsp;bundle)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(bundle&nbsp;==&nbsp;<span style=""color:blue;"">null</span>)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">yield</span>&nbsp;<span style=""color:blue;"">break</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Save&nbsp;the&nbsp;bundle&nbsp;for&nbsp;future&nbsp;use</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;cachedBundle&nbsp;=&nbsp;bundle;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Start&nbsp;loading&nbsp;the&nbsp;asset&nbsp;from&nbsp;the&nbsp;bundle</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">var</span>&nbsp;asyncAsset&nbsp;=&nbsp;cachedBundle.LoadAsync(<span style=""color:#a31515;"">&quot;9443182_orig&quot;</span>,&nbsp;<span style=""color:blue;"">typeof</span>(<span style=""color:#2b91af;"">Texture2D</span>));

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;wait&nbsp;til&nbsp;load</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">yield</span>&nbsp;<span style=""color:blue;"">return</span>&nbsp;asyncAsset;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;get&nbsp;the&nbsp;texture</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;texture&nbsp;=&nbsp;asyncAsset.asset&nbsp;<span style=""color:blue;"">as</span>&nbsp;<span style=""color:#2b91af;"">Texture2D</span>;
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;UnloadBundle()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(cachedBundle&nbsp;!=&nbsp;<span style=""color:blue;"">null</span>)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;cachedBundle.Unload(<span style=""color:blue;"">true</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;cachedBundle&nbsp;=&nbsp;<span style=""color:blue;"">null</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>
}</pre>";
        #endregion

        #region LargeFileDownloadSample
        public static string LargeFileDownloadSample = @"<pre style=""font-family:Consolas;font-size:13;color:black;background:silver;""><span style=""color:blue;"">using</span>&nbsp;System;
<span style=""color:blue;"">using</span>&nbsp;System.Collections.Generic;

<span style=""color:blue;"">using</span>&nbsp;UnityEngine;
<span style=""color:blue;"">using</span>&nbsp;BestHTTP;

<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">sealed</span>&nbsp;<span style=""color:blue;"">class</span>&nbsp;<span style=""color:#2b91af;"">LargeFileDownloadSample</span>&nbsp;:&nbsp;<span style=""color:#2b91af;"">MonoBehaviour</span>
{
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;The&nbsp;url&nbsp;of&nbsp;the&nbsp;resource&nbsp;to&nbsp;download</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">const</span>&nbsp;<span style=""color:blue;"">string</span>&nbsp;URL&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;http://ipv4.download.thinkbroadband.com/100MB.zip&quot;</span>;

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Private&nbsp;Fields

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Cached&nbsp;request&nbsp;to&nbsp;be&nbsp;able&nbsp;to&nbsp;abort&nbsp;it</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">HTTPRequest</span>&nbsp;request;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Debug&nbsp;status&nbsp;of&nbsp;the&nbsp;request</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;status&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Download(processing)&nbsp;progress.&nbsp;Its&nbsp;range&nbsp;is&nbsp;between&nbsp;[0..1]</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">float</span>&nbsp;progress;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;The&nbsp;fragment&nbsp;size&nbsp;that&nbsp;we&nbsp;will&nbsp;set&nbsp;to&nbsp;the&nbsp;request</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">int</span>&nbsp;fragmentSize&nbsp;=&nbsp;<span style=""color:#2b91af;"">HTTPResponse</span>.MinBufferSize;

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Unity&nbsp;Events

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;Awake()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;If&nbsp;we&nbsp;have&nbsp;a&nbsp;non-finished&nbsp;download,&nbsp;set&nbsp;the&nbsp;progress&nbsp;to&nbsp;the&nbsp;value&nbsp;where&nbsp;we&nbsp;left&nbsp;it</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">PlayerPrefs</span>.HasKey(<span style=""color:#a31515;"">&quot;DownloadLength&quot;</span>))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;progress&nbsp;=&nbsp;<span style=""color:#2b91af;"">PlayerPrefs</span>.GetInt(<span style=""color:#a31515;"">&quot;DownloadProgress&quot;</span>)&nbsp;/&nbsp;(<span style=""color:blue;"">float</span>)<span style=""color:#2b91af;"">PlayerPrefs</span>.GetInt(<span style=""color:#a31515;"">&quot;DownloadLength&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnDestroy()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Stop&nbsp;the&nbsp;download&nbsp;if&nbsp;we&nbsp;are&nbsp;leaving&nbsp;this&nbsp;example</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(request&nbsp;!=&nbsp;<span style=""color:blue;"">null</span>&nbsp;&amp;&amp;&nbsp;request.State&nbsp;&lt;&nbsp;<span style=""color:#2b91af;"">HTTPRequestStates</span>.Finished)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request.OnProgress&nbsp;=&nbsp;<span style=""color:blue;"">null</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request.Callback&nbsp;=&nbsp;<span style=""color:blue;"">null</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request.Abort();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnGUI()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUIHelper</span>.DrawArea(<span style=""color:#2b91af;"">GUIHelper</span>.ClientArea,&nbsp;<span style=""color:blue;"">true</span>,&nbsp;()&nbsp;=&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Draw&nbsp;the&nbsp;current&nbsp;status</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Request&nbsp;status:&nbsp;&quot;</span>&nbsp;+&nbsp;status);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(5);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Draw&nbsp;the&nbsp;current&nbsp;progress</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;Progress:&nbsp;{0:P2}&nbsp;of&nbsp;{1:N0}Mb&quot;</span>,&nbsp;progress,&nbsp;<span style=""color:#2b91af;"">PlayerPrefs</span>.GetInt(<span style=""color:#a31515;"">&quot;DownloadLength&quot;</span>)&nbsp;/&nbsp;1048576&nbsp;<span style=""color:green;"">/*1&nbsp;Mb*/</span>));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.HorizontalSlider(progress,&nbsp;0,&nbsp;1);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(50);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(request&nbsp;==&nbsp;<span style=""color:blue;"">null</span>)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Draw&nbsp;a&nbsp;slider&nbsp;to&nbsp;be&nbsp;able&nbsp;to&nbsp;change&nbsp;the&nbsp;fragment&nbsp;size</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;Desired&nbsp;Fragment&nbsp;Size:&nbsp;{0:N}&nbsp;KBytes&quot;</span>,&nbsp;fragmentSize&nbsp;/&nbsp;1024f));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;fragmentSize&nbsp;=&nbsp;(<span style=""color:blue;"">int</span>)<span style=""color:#2b91af;"">GUILayout</span>.HorizontalSlider(fragmentSize,&nbsp;<span style=""color:#2b91af;"">HTTPResponse</span>.MinBufferSize,&nbsp;10&nbsp;*&nbsp;1024&nbsp;*&nbsp;1024);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(5);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;buttonStr&nbsp;=&nbsp;<span style=""color:#2b91af;"">PlayerPrefs</span>.HasKey(<span style=""color:#a31515;"">&quot;DownloadProgress&quot;</span>)&nbsp;?&nbsp;<span style=""color:#a31515;"">&quot;Continue&nbsp;Download&quot;</span>&nbsp;:&nbsp;<span style=""color:#a31515;"">&quot;Start&nbsp;Download&quot;</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">GUILayout</span>.Button(buttonStr))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;StreamLargeFileTest();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">else</span>&nbsp;<span style=""color:blue;"">if</span>&nbsp;(request.State&nbsp;==&nbsp;<span style=""color:#2b91af;"">HTTPRequestStates</span>.Processing&nbsp;&amp;&amp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Button(<span style=""color:#a31515;"">&quot;Abort&nbsp;Download&quot;</span>))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Simulate&nbsp;a&nbsp;connection&nbsp;lost</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request.Abort();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;});
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Private&nbsp;Helper&nbsp;Functions

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Calling&nbsp;this&nbsp;function&nbsp;again&nbsp;when&nbsp;the&nbsp;&quot;DownloadProgress&quot;&nbsp;key&nbsp;in&nbsp;the&nbsp;PlayerPrefs&nbsp;present&nbsp;will&nbsp;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//	continue&nbsp;the&nbsp;download</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;StreamLargeFileTest()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">HTTPRequest</span>(<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">Uri</span>(URL),&nbsp;(req,&nbsp;resp)&nbsp;=&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">switch</span>&nbsp;(req.State)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;The&nbsp;request&nbsp;is&nbsp;currently&nbsp;processed.&nbsp;With&nbsp;UseStreaming&nbsp;==&nbsp;true,&nbsp;we&nbsp;can&nbsp;get&nbsp;the&nbsp;streamed&nbsp;fragments&nbsp;here</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#2b91af;"">HTTPRequestStates</span>.Processing:

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Set&nbsp;the&nbsp;DownloadLength,&nbsp;so&nbsp;we&nbsp;can&nbsp;display&nbsp;the&nbsp;progress</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(!<span style=""color:#2b91af;"">PlayerPrefs</span>.HasKey(<span style=""color:#a31515;"">&quot;DownloadLength&quot;</span>))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;value&nbsp;=&nbsp;resp.GetFirstHeaderValue(<span style=""color:#a31515;"">&quot;content-length&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(!<span style=""color:blue;"">string</span>.IsNullOrEmpty(value))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">PlayerPrefs</span>.SetInt(<span style=""color:#a31515;"">&quot;DownloadLength&quot;</span>,&nbsp;<span style=""color:blue;"">int</span>.Parse(value));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Get&nbsp;the&nbsp;fragments,&nbsp;and&nbsp;save&nbsp;them</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ProcessFragments(resp.GetStreamedFragments());

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;status&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;Processing&quot;</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">break</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;The&nbsp;request&nbsp;finished&nbsp;without&nbsp;any&nbsp;problem.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#2b91af;"">HTTPRequestStates</span>.Finished:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(resp.IsSuccess)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Save&nbsp;any&nbsp;remaining&nbsp;fragments</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ProcessFragments(resp.GetStreamedFragments());

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Completly&nbsp;finished</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(resp.IsStreamingFinished)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;status&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;Streaming&nbsp;finished!&quot;</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;We&nbsp;are&nbsp;done,&nbsp;delete&nbsp;the&nbsp;progress&nbsp;key</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">PlayerPrefs</span>.DeleteKey(<span style=""color:#a31515;"">&quot;DownloadProgress&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">PlayerPrefs</span>.Save();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request&nbsp;=&nbsp;<span style=""color:blue;"">null</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">else</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;status&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;Processing&quot;</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">else</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;status&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;Request&nbsp;finished&nbsp;Successfully,&nbsp;but&nbsp;the&nbsp;server&nbsp;sent&nbsp;an&nbsp;error.&nbsp;Status&nbsp;Code:&nbsp;{0}-{1}&nbsp;Message:&nbsp;{2}&quot;</span>,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;resp.StatusCode,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;resp.Message,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;resp.DataAsText);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Debug</span>.LogWarning(status);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request&nbsp;=&nbsp;<span style=""color:blue;"">null</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">break</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;The&nbsp;request&nbsp;finished&nbsp;with&nbsp;an&nbsp;unexpected&nbsp;error.&nbsp;The&nbsp;request&#39;s&nbsp;Exception&nbsp;property&nbsp;may&nbsp;contain&nbsp;more&nbsp;info&nbsp;about&nbsp;the&nbsp;error.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#2b91af;"">HTTPRequestStates</span>.Error:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;status&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;Request&nbsp;Finished&nbsp;with&nbsp;Error!&nbsp;&quot;</span>&nbsp;+&nbsp;(req.Exception&nbsp;!=&nbsp;<span style=""color:blue;"">null</span>&nbsp;?&nbsp;(req.Exception.Message&nbsp;+&nbsp;<span style=""color:#a31515;"">&quot;\n&quot;</span>&nbsp;+&nbsp;req.Exception.StackTrace)&nbsp;:&nbsp;<span style=""color:#a31515;"">&quot;No&nbsp;Exception&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Debug</span>.LogError(status);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request&nbsp;=&nbsp;<span style=""color:blue;"">null</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">break</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;The&nbsp;request&nbsp;aborted,&nbsp;initiated&nbsp;by&nbsp;the&nbsp;user.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#2b91af;"">HTTPRequestStates</span>.Aborted:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;status&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;Request&nbsp;Aborted!&quot;</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Debug</span>.LogWarning(status);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request&nbsp;=&nbsp;<span style=""color:blue;"">null</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">break</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Ceonnecting&nbsp;to&nbsp;the&nbsp;server&nbsp;is&nbsp;timed&nbsp;out.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#2b91af;"">HTTPRequestStates</span>.ConnectionTimedOut:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;status&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;Connection&nbsp;Timed&nbsp;Out!&quot;</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Debug</span>.LogError(status);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request&nbsp;=&nbsp;<span style=""color:blue;"">null</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">break</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;The&nbsp;request&nbsp;didn&#39;t&nbsp;finished&nbsp;in&nbsp;the&nbsp;given&nbsp;time.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#2b91af;"">HTTPRequestStates</span>.TimedOut:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;status&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;Processing&nbsp;the&nbsp;request&nbsp;Timed&nbsp;Out!&quot;</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Debug</span>.LogError(status);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request&nbsp;=&nbsp;<span style=""color:blue;"">null</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">break</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;});

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Are&nbsp;there&nbsp;any&nbsp;progress,&nbsp;that&nbsp;we&nbsp;can&nbsp;continue?</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">PlayerPrefs</span>.HasKey(<span style=""color:#a31515;"">&quot;DownloadProgress&quot;</span>))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Set&nbsp;the&nbsp;range&nbsp;header</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request.SetRangeHeader(<span style=""color:#2b91af;"">PlayerPrefs</span>.GetInt(<span style=""color:#a31515;"">&quot;DownloadProgress&quot;</span>));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">else</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;This&nbsp;is&nbsp;a&nbsp;new&nbsp;request</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">PlayerPrefs</span>.SetInt(<span style=""color:#a31515;"">&quot;DownloadProgress&quot;</span>,&nbsp;0);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;If&nbsp;we&nbsp;are&nbsp;writing&nbsp;our&nbsp;own&nbsp;file&nbsp;set&nbsp;it&nbsp;true(disable),&nbsp;so&nbsp;don&#39;t&nbsp;duplicate&nbsp;it&nbsp;on&nbsp;the&nbsp;filesystem</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request.DisableCache&nbsp;=&nbsp;<span style=""color:blue;"">true</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;We&nbsp;want&nbsp;to&nbsp;access&nbsp;the&nbsp;downloaded&nbsp;bytes&nbsp;while&nbsp;we&nbsp;are&nbsp;still&nbsp;downloading</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request.UseStreaming&nbsp;=&nbsp;<span style=""color:blue;"">true</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Set&nbsp;a&nbsp;reasonable&nbsp;high&nbsp;fragment&nbsp;size.&nbsp;Here&nbsp;it&nbsp;is&nbsp;5&nbsp;megabytes.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request.StreamFragmentSize&nbsp;=&nbsp;fragmentSize;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Start&nbsp;Processing&nbsp;the&nbsp;request</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;request.Send();
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;In&nbsp;this&nbsp;function&nbsp;we&nbsp;can&nbsp;do&nbsp;whatever&nbsp;we&nbsp;want&nbsp;with&nbsp;the&nbsp;downloaded&nbsp;bytes.&nbsp;In&nbsp;this&nbsp;sample&nbsp;we&nbsp;will&nbsp;do&nbsp;nothing,&nbsp;just&nbsp;set&nbsp;the&nbsp;metadata&nbsp;to&nbsp;display&nbsp;progress.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;ProcessFragments(<span style=""color:#2b91af;"">List</span>&lt;<span style=""color:blue;"">byte</span>[]&gt;&nbsp;fragments)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(fragments&nbsp;!=&nbsp;<span style=""color:blue;"">null</span>&nbsp;&amp;&amp;&nbsp;fragments.Count&nbsp;&gt;&nbsp;0)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">/*string&nbsp;dir&nbsp;=&nbsp;&quot;TODO!&quot;;</span>
<span style=""color:green;"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;string&nbsp;filename&nbsp;=&nbsp;&quot;TODO!&quot;;</span>

<span style=""color:green;"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;using&nbsp;(System.IO.FileStream&nbsp;fs&nbsp;=&nbsp;new&nbsp;System.IO.FileStream(System.IO.Path.Combine(dir,&nbsp;filename),&nbsp;System.IO.FileMode.Append))</span>
<span style=""color:green;"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;for&nbsp;(int&nbsp;i&nbsp;=&nbsp;0;&nbsp;i&nbsp;&lt;&nbsp;fragments.Count;&nbsp;++i)</span>
<span style=""color:green;"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;fs.Write(fragments[i],&nbsp;0,&nbsp;fragments[i].Length);*/</span>

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">for</span>&nbsp;(<span style=""color:blue;"">int</span>&nbsp;i&nbsp;=&nbsp;0;&nbsp;i&nbsp;&lt;&nbsp;fragments.Count;&nbsp;++i)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Save&nbsp;how&nbsp;many&nbsp;bytes&nbsp;we&nbsp;wrote&nbsp;successfully</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">int</span>&nbsp;downloaded&nbsp;=&nbsp;<span style=""color:#2b91af;"">PlayerPrefs</span>.GetInt(<span style=""color:#a31515;"">&quot;DownloadProgress&quot;</span>)&nbsp;+&nbsp;fragments[i].Length;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">PlayerPrefs</span>.SetInt(<span style=""color:#a31515;"">&quot;DownloadProgress&quot;</span>,&nbsp;downloaded);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">PlayerPrefs</span>.Save();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Set&nbsp;the&nbsp;progress&nbsp;to&nbsp;the&nbsp;actually&nbsp;processed&nbsp;bytes</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;progress&nbsp;=&nbsp;<span style=""color:#2b91af;"">PlayerPrefs</span>.GetInt(<span style=""color:#a31515;"">&quot;DownloadProgress&quot;</span>)&nbsp;/&nbsp;(<span style=""color:blue;"">float</span>)<span style=""color:#2b91af;"">PlayerPrefs</span>.GetInt(<span style=""color:#a31515;"">&quot;DownloadLength&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>
}</pre>";
        #endregion

        #region SocketIOChatSample
        public static string SocketIOChatSample = @"<pre style=""font-family:Consolas;font-size:13;color:black;background:silver;""><span style=""color:blue;"">using</span>&nbsp;System;
<span style=""color:blue;"">using</span>&nbsp;System.Collections.Generic;

<span style=""color:blue;"">using</span>&nbsp;UnityEngine;
<span style=""color:blue;"">using</span>&nbsp;BestHTTP;
<span style=""color:blue;"">using</span>&nbsp;BestHTTP.SocketIO;
<span style=""color:blue;"">using</span>&nbsp;BestHTTP.JSON;
<span style=""color:blue;"">using</span>&nbsp;BestHTTP.SocketIO.Events;

<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">sealed</span>&nbsp;<span style=""color:blue;"">class</span>&nbsp;<span style=""color:#2b91af;"">SocketIOChatSample</span>&nbsp;:&nbsp;<span style=""color:#2b91af;"">MonoBehaviour</span>
{
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">readonly</span>&nbsp;<span style=""color:#2b91af;"">TimeSpan</span>&nbsp;TYPING_TIMER_LENGTH&nbsp;=&nbsp;<span style=""color:#2b91af;"">TimeSpan</span>.FromMilliseconds(700);

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">enum</span>&nbsp;<span style=""color:#2b91af;"">ChatStates</span>
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Login,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Chat
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Fields

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;The&nbsp;Socket.IO&nbsp;manager&nbsp;instance.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:#2b91af;"">SocketManager</span>&nbsp;Manager;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Current&nbsp;state&nbsp;of&nbsp;the&nbsp;chat&nbsp;demo.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:#2b91af;"">ChatStates</span>&nbsp;State;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;The&nbsp;selected&nbsp;nickname</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">string</span>&nbsp;userName&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Currently&nbsp;typing&nbsp;message</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">string</span>&nbsp;message&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Sent&nbsp;and&nbsp;received&nbsp;messages.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">string</span>&nbsp;chatLog&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Position&nbsp;of&nbsp;the&nbsp;scroller</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:#2b91af;"">Vector2</span>&nbsp;scrollPos;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;True&nbsp;if&nbsp;the&nbsp;user&nbsp;is&nbsp;currently&nbsp;typing</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">bool</span>&nbsp;typing;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;When&nbsp;the&nbsp;message&nbsp;changed.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:#2b91af;"">DateTime</span>&nbsp;lastTypingTime&nbsp;=&nbsp;<span style=""color:#2b91af;"">DateTime</span>.MinValue;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Users&nbsp;that&nbsp;typing.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:#2b91af;"">List</span>&lt;<span style=""color:blue;"">string</span>&gt;&nbsp;typingUsers&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">List</span>&lt;<span style=""color:blue;"">string</span>&gt;();

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Unity&nbsp;Events
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;Start()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;The&nbsp;current&nbsp;state&nbsp;is&nbsp;Login</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;State&nbsp;=&nbsp;<span style=""color:#2b91af;"">ChatStates</span>.Login;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Change&nbsp;an&nbsp;option&nbsp;to&nbsp;show&nbsp;how&nbsp;it&nbsp;should&nbsp;be&nbsp;done</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">SocketOptions</span>&nbsp;options&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">SocketOptions</span>();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;options.AutoConnect&nbsp;=&nbsp;<span style=""color:blue;"">false</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Create&nbsp;the&nbsp;Socket.IO&nbsp;manager</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Manager&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">SocketManager</span>(<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">Uri</span>(<span style=""color:#a31515;"">&quot;http://chat.socket.io/socket.io/&quot;</span>),&nbsp;options);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Set&nbsp;up&nbsp;custom&nbsp;chat&nbsp;events</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Manager.Socket.On(<span style=""color:#a31515;"">&quot;login&quot;</span>,&nbsp;OnLogin);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Manager.Socket.On(<span style=""color:#a31515;"">&quot;new&nbsp;message&quot;</span>,&nbsp;OnNewMessage);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Manager.Socket.On(<span style=""color:#a31515;"">&quot;user&nbsp;joined&quot;</span>,&nbsp;OnUserJoined);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Manager.Socket.On(<span style=""color:#a31515;"">&quot;user&nbsp;left&quot;</span>,&nbsp;OnUserLeft);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Manager.Socket.On(<span style=""color:#a31515;"">&quot;typing&quot;</span>,&nbsp;OnTyping);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Manager.Socket.On(<span style=""color:#a31515;"">&quot;stop&nbsp;typing&quot;</span>,&nbsp;OnStopTyping);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;The&nbsp;argument&nbsp;will&nbsp;be&nbsp;an&nbsp;Error&nbsp;object.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Manager.Socket.On(<span style=""color:#2b91af;"">SocketIOEventTypes</span>.Error,&nbsp;(socket,&nbsp;packet,&nbsp;args)&nbsp;=&gt;&nbsp;<span style=""color:#2b91af;"">Debug</span>.LogError(<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;Error:&nbsp;{0}&quot;</span>,&nbsp;args[0].ToString())));

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;We&nbsp;set&nbsp;SocketOptions&#39;&nbsp;AutoConnect&nbsp;to&nbsp;false,&nbsp;so&nbsp;we&nbsp;have&nbsp;to&nbsp;call&nbsp;it&nbsp;manually.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Manager.Open();
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnDestroy()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Leaving&nbsp;this&nbsp;sample,&nbsp;close&nbsp;the&nbsp;socket</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Manager.Close();
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;Update()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Go&nbsp;back&nbsp;to&nbsp;the&nbsp;demo&nbsp;selector</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">Input</span>.GetKeyDown(<span style=""color:#2b91af;"">KeyCode</span>.Escape))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">SampleSelector</span>.SelectedSample.DestroyUnityObject();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Stop&nbsp;typing&nbsp;if&nbsp;some&nbsp;time&nbsp;passed&nbsp;without&nbsp;typing</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(typing)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">var</span>&nbsp;typingTimer&nbsp;=&nbsp;<span style=""color:#2b91af;"">DateTime</span>.UtcNow;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">var</span>&nbsp;timeDiff&nbsp;=&nbsp;typingTimer&nbsp;-&nbsp;lastTypingTime;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(timeDiff&nbsp;&gt;=&nbsp;TYPING_TIMER_LENGTH)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Manager.Socket.Emit(<span style=""color:#a31515;"">&quot;stop&nbsp;typing&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;typing&nbsp;=&nbsp;<span style=""color:blue;"">false</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnGUI()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">switch</span>(State)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#2b91af;"">ChatStates</span>.Login:&nbsp;DrawLoginScreen();&nbsp;<span style=""color:blue;"">break</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#2b91af;"">ChatStates</span>.Chat:&nbsp;DrawChatScreen();&nbsp;<span style=""color:blue;"">break</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Chat&nbsp;Logic

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Called&nbsp;from&nbsp;an&nbsp;OnGUI&nbsp;event&nbsp;to&nbsp;draw&nbsp;the&nbsp;Login&nbsp;Screen.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;DrawLoginScreen()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUIHelper</span>.DrawArea(<span style=""color:#2b91af;"">GUIHelper</span>.ClientArea,&nbsp;<span style=""color:blue;"">true</span>,&nbsp;()&nbsp;=&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginVertical();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.FlexibleSpace();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUIHelper</span>.DrawCenteredText(<span style=""color:#a31515;"">&quot;What&#39;s&nbsp;your&nbsp;nickname?&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;userName&nbsp;=&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.TextField(userName);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">GUILayout</span>.Button(<span style=""color:#a31515;"">&quot;Join&quot;</span>))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SetUserName();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.FlexibleSpace();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndVertical();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;});
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Called&nbsp;from&nbsp;an&nbsp;OnGUI&nbsp;event&nbsp;to&nbsp;draw&nbsp;the&nbsp;Chat&nbsp;Screen.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;DrawChatScreen()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUIHelper</span>.DrawArea(<span style=""color:#2b91af;"">GUIHelper</span>.ClientArea,&nbsp;<span style=""color:blue;"">true</span>,&nbsp;()&nbsp;=&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginVertical();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;scrollPos&nbsp;=&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginScrollView(scrollPos);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(chatLog,&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.ExpandWidth(<span style=""color:blue;"">true</span>),&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.ExpandHeight(<span style=""color:blue;"">true</span>));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndScrollView();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;typing&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(typingUsers.Count&nbsp;&gt;&nbsp;0)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;typing&nbsp;+=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;{0}&quot;</span>,&nbsp;typingUsers[0]);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">for</span>&nbsp;(<span style=""color:blue;"">int</span>&nbsp;i&nbsp;=&nbsp;1;&nbsp;i&nbsp;&lt;&nbsp;typingUsers.Count;&nbsp;++i)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;typing&nbsp;+=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;,&nbsp;{0}&quot;</span>,&nbsp;typingUsers[i]);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(typingUsers.Count&nbsp;==&nbsp;1)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;typing&nbsp;+=&nbsp;<span style=""color:#a31515;"">&quot;&nbsp;is&nbsp;typing!&quot;</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">else</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;typing&nbsp;+=&nbsp;<span style=""color:#a31515;"">&quot;&nbsp;are&nbsp;typing!&quot;</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(typing);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Type&nbsp;here:&quot;</span>);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;message&nbsp;=&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.TextField(message);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">GUILayout</span>.Button(<span style=""color:#a31515;"">&quot;Send&quot;</span>,&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.MaxWidth(100)))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SendMessage();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">GUI</span>.changed)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UpdateTyping();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndVertical();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;});
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;SetUserName()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:blue;"">string</span>.IsNullOrEmpty(userName))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">return</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;State&nbsp;=&nbsp;<span style=""color:#2b91af;"">ChatStates</span>.Chat;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Manager.Socket.Emit(<span style=""color:#a31515;"">&quot;add&nbsp;user&quot;</span>,&nbsp;userName);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;SendMessage()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:blue;"">string</span>.IsNullOrEmpty(message))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">return</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Manager.Socket.Emit(<span style=""color:#a31515;"">&quot;new&nbsp;message&quot;</span>,&nbsp;message);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;chatLog&nbsp;+=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;{0}:&nbsp;{1}\n&quot;</span>,&nbsp;userName,&nbsp;message);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;message&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;UpdateTyping()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(!typing)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;typing&nbsp;=&nbsp;<span style=""color:blue;"">true</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Manager.Socket.Emit(<span style=""color:#a31515;"">&quot;typing&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;lastTypingTime&nbsp;=&nbsp;<span style=""color:#2b91af;"">DateTime</span>.UtcNow;
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;addParticipantsMessage(<span style=""color:#2b91af;"">Dictionary</span>&lt;<span style=""color:blue;"">string</span>,&nbsp;<span style=""color:blue;"">object</span>&gt;&nbsp;data)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">int</span>&nbsp;numUsers&nbsp;=&nbsp;<span style=""color:#2b91af;"">Convert</span>.ToInt32(data[<span style=""color:#a31515;"">&quot;numUsers&quot;</span>]);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(numUsers&nbsp;==&nbsp;1)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;chatLog&nbsp;+=&nbsp;<span style=""color:#a31515;"">&quot;there&#39;s&nbsp;1&nbsp;participant\n&quot;</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">else</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;chatLog&nbsp;+=&nbsp;<span style=""color:#a31515;"">&quot;there&nbsp;are&nbsp;&quot;</span>&nbsp;+&nbsp;numUsers&nbsp;+&nbsp;<span style=""color:#a31515;"">&quot;&nbsp;participants\n&quot;</span>;
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;addChatMessage(<span style=""color:#2b91af;"">Dictionary</span>&lt;<span style=""color:blue;"">string</span>,&nbsp;<span style=""color:blue;"">object</span>&gt;&nbsp;data)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">var</span>&nbsp;username&nbsp;=&nbsp;data[<span style=""color:#a31515;"">&quot;username&quot;</span>]&nbsp;<span style=""color:blue;"">as</span>&nbsp;<span style=""color:blue;"">string</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">var</span>&nbsp;msg&nbsp;=&nbsp;data[<span style=""color:#a31515;"">&quot;message&quot;</span>]&nbsp;<span style=""color:blue;"">as</span>&nbsp;<span style=""color:blue;"">string</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;chatLog&nbsp;+=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;{0}:&nbsp;{1}\n&quot;</span>,&nbsp;username,&nbsp;msg);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;AddChatTyping(<span style=""color:#2b91af;"">Dictionary</span>&lt;<span style=""color:blue;"">string</span>,&nbsp;<span style=""color:blue;"">object</span>&gt;&nbsp;data)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">var</span>&nbsp;username&nbsp;=&nbsp;data[<span style=""color:#a31515;"">&quot;username&quot;</span>]&nbsp;<span style=""color:blue;"">as</span>&nbsp;<span style=""color:blue;"">string</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;typingUsers.Add(username);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;RemoveChatTyping(<span style=""color:#2b91af;"">Dictionary</span>&lt;<span style=""color:blue;"">string</span>,&nbsp;<span style=""color:blue;"">object</span>&gt;&nbsp;data)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">var</span>&nbsp;username&nbsp;=&nbsp;data[<span style=""color:#a31515;"">&quot;username&quot;</span>]&nbsp;<span style=""color:blue;"">as</span>&nbsp;<span style=""color:blue;"">string</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">int</span>&nbsp;idx&nbsp;=&nbsp;typingUsers.FindIndex((name)&nbsp;=&gt;&nbsp;name.Equals(username));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(idx&nbsp;!=&nbsp;-1)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;typingUsers.RemoveAt(idx);
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Custom&nbsp;SocketIO&nbsp;Events

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnLogin(<span style=""color:#2b91af;"">Socket</span>&nbsp;socket,&nbsp;<span style=""color:#2b91af;"">Packet</span>&nbsp;packet,&nbsp;<span style=""color:blue;"">params</span>&nbsp;<span style=""color:blue;"">object</span>[]&nbsp;args)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;chatLog&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;Welcome&nbsp;to&nbsp;Socket.IO&nbsp;Chat&nbsp;—&nbsp;\n&quot;</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;addParticipantsMessage(args[0]&nbsp;<span style=""color:blue;"">as</span>&nbsp;<span style=""color:#2b91af;"">Dictionary</span>&lt;<span style=""color:blue;"">string</span>,&nbsp;<span style=""color:blue;"">object</span>&gt;);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnNewMessage(<span style=""color:#2b91af;"">Socket</span>&nbsp;socket,&nbsp;<span style=""color:#2b91af;"">Packet</span>&nbsp;packet,&nbsp;<span style=""color:blue;"">params</span>&nbsp;<span style=""color:blue;"">object</span>[]&nbsp;args)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;addChatMessage(args[0]&nbsp;<span style=""color:blue;"">as</span>&nbsp;<span style=""color:#2b91af;"">Dictionary</span>&lt;<span style=""color:blue;"">string</span>,&nbsp;<span style=""color:blue;"">object</span>&gt;);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnUserJoined(<span style=""color:#2b91af;"">Socket</span>&nbsp;socket,&nbsp;<span style=""color:#2b91af;"">Packet</span>&nbsp;packet,&nbsp;<span style=""color:blue;"">params</span>&nbsp;<span style=""color:blue;"">object</span>[]&nbsp;args)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">var</span>&nbsp;data&nbsp;=&nbsp;args[0]&nbsp;<span style=""color:blue;"">as</span>&nbsp;<span style=""color:#2b91af;"">Dictionary</span>&lt;<span style=""color:blue;"">string</span>,&nbsp;<span style=""color:blue;"">object</span>&gt;;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">var</span>&nbsp;username&nbsp;=&nbsp;data[<span style=""color:#a31515;"">&quot;username&quot;</span>]&nbsp;<span style=""color:blue;"">as</span>&nbsp;<span style=""color:blue;"">string</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;chatLog&nbsp;+=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;{0}&nbsp;joined\n&quot;</span>,&nbsp;username);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;addParticipantsMessage(data);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnUserLeft(<span style=""color:#2b91af;"">Socket</span>&nbsp;socket,&nbsp;<span style=""color:#2b91af;"">Packet</span>&nbsp;packet,&nbsp;<span style=""color:blue;"">params</span>&nbsp;<span style=""color:blue;"">object</span>[]&nbsp;args)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">var</span>&nbsp;data&nbsp;=&nbsp;args[0]&nbsp;<span style=""color:blue;"">as</span>&nbsp;<span style=""color:#2b91af;"">Dictionary</span>&lt;<span style=""color:blue;"">string</span>,&nbsp;<span style=""color:blue;"">object</span>&gt;;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">var</span>&nbsp;username&nbsp;=&nbsp;data[<span style=""color:#a31515;"">&quot;username&quot;</span>]&nbsp;<span style=""color:blue;"">as</span>&nbsp;<span style=""color:blue;"">string</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;chatLog&nbsp;+=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;{0}&nbsp;left\n&quot;</span>,&nbsp;username);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;addParticipantsMessage(data);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnTyping(<span style=""color:#2b91af;"">Socket</span>&nbsp;socket,&nbsp;<span style=""color:#2b91af;"">Packet</span>&nbsp;packet,&nbsp;<span style=""color:blue;"">params</span>&nbsp;<span style=""color:blue;"">object</span>[]&nbsp;args)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;AddChatTyping(args[0]&nbsp;<span style=""color:blue;"">as</span>&nbsp;<span style=""color:#2b91af;"">Dictionary</span>&lt;<span style=""color:blue;"">string</span>,&nbsp;<span style=""color:blue;"">object</span>&gt;);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnStopTyping(<span style=""color:#2b91af;"">Socket</span>&nbsp;socket,&nbsp;<span style=""color:#2b91af;"">Packet</span>&nbsp;packet,&nbsp;<span style=""color:blue;"">params</span>&nbsp;<span style=""color:blue;"">object</span>[]&nbsp;args)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;RemoveChatTyping(args[0]&nbsp;<span style=""color:blue;"">as</span>&nbsp;<span style=""color:#2b91af;"">Dictionary</span>&lt;<span style=""color:blue;"">string</span>,&nbsp;<span style=""color:blue;"">object</span>&gt;);
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>
}</pre>";
        #endregion

        #region SocketIOWePlaySample
        public static string SocketIOWePlaySample = @"<pre style=""font-family:Consolas;font-size:13;color:black;background:silver;""><span style=""color:blue;"">using</span>&nbsp;System;
<span style=""color:blue;"">using</span>&nbsp;System.Collections.Generic;

<span style=""color:blue;"">using</span>&nbsp;UnityEngine;
<span style=""color:blue;"">using</span>&nbsp;BestHTTP.SocketIO;
<span style=""color:blue;"">using</span>&nbsp;BestHTTP.SocketIO.Events;

<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">sealed</span>&nbsp;<span style=""color:blue;"">class</span>&nbsp;<span style=""color:#2b91af;"">SocketIOWePlaySample</span>&nbsp;:&nbsp;<span style=""color:#2b91af;"">MonoBehaviour</span>
{
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Possible&nbsp;states&nbsp;of&nbsp;the&nbsp;game.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">enum</span>&nbsp;<span style=""color:#2b91af;"">States</span>
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Connecting,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WaitForNick,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Joined
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Controls&nbsp;that&nbsp;the&nbsp;server&nbsp;understands&nbsp;as&nbsp;a&nbsp;parameter&nbsp;in&nbsp;the&nbsp;move&nbsp;event.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">string</span>[]&nbsp;controls&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:blue;"">string</span>[]&nbsp;{&nbsp;<span style=""color:#a31515;"">&quot;left&quot;</span>,&nbsp;<span style=""color:#a31515;"">&quot;right&quot;</span>,&nbsp;<span style=""color:#a31515;"">&quot;a&quot;</span>,&nbsp;<span style=""color:#a31515;"">&quot;b&quot;</span>,&nbsp;<span style=""color:#a31515;"">&quot;up&quot;</span>,&nbsp;<span style=""color:#a31515;"">&quot;down&quot;</span>,&nbsp;<span style=""color:#a31515;"">&quot;select&quot;</span>,&nbsp;<span style=""color:#a31515;"">&quot;start&quot;</span>&nbsp;};

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Ratio&nbsp;of&nbsp;the&nbsp;drawn&nbsp;GUI&nbsp;texture&nbsp;from&nbsp;the&nbsp;screen</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">const</span>&nbsp;<span style=""color:blue;"">float</span>&nbsp;ratio&nbsp;=&nbsp;1.5f;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;How&nbsp;many&nbsp;messages&nbsp;to&nbsp;keep.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">int</span>&nbsp;MaxMessages&nbsp;=&nbsp;50;
&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Current&nbsp;state&nbsp;of&nbsp;the&nbsp;game.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:#2b91af;"">States</span>&nbsp;State;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;The&nbsp;root(&quot;/&quot;)&nbsp;Socket&nbsp;instance.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:#2b91af;"">Socket</span>&nbsp;Socket;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;The&nbsp;user-selected&nbsp;nickname.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">string</span>&nbsp;Nick&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;The&nbsp;message&nbsp;that&nbsp;the&nbsp;user&nbsp;want&nbsp;to&nbsp;send&nbsp;to&nbsp;the&nbsp;chat.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">string</span>&nbsp;messageToSend&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;How&nbsp;many&nbsp;user&nbsp;connected&nbsp;to&nbsp;the&nbsp;server.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">int</span>&nbsp;connections;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Local&nbsp;and&nbsp;server&nbsp;sent&nbsp;messages.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:#2b91af;"">List</span>&lt;<span style=""color:blue;"">string</span>&gt;&nbsp;messages&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">List</span>&lt;<span style=""color:blue;"">string</span>&gt;();
&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;The&nbsp;chat&nbsp;scroll&nbsp;position.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:#2b91af;"">Vector2</span>&nbsp;scrollPos;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;The&nbsp;decoded&nbsp;texture&nbsp;from&nbsp;the&nbsp;server&nbsp;sent&nbsp;binary&nbsp;data</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:#2b91af;"">Texture2D</span>&nbsp;FrameTexture;

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Unity&nbsp;Events

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;Start()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Change&nbsp;an&nbsp;option&nbsp;to&nbsp;show&nbsp;how&nbsp;it&nbsp;should&nbsp;be&nbsp;done</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">SocketOptions</span>&nbsp;options&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">SocketOptions</span>();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;options.AutoConnect&nbsp;=&nbsp;<span style=""color:blue;"">false</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Create&nbsp;the&nbsp;SocketManager&nbsp;instance</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">var</span>&nbsp;manager&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">SocketManager</span>(<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">Uri</span>(<span style=""color:#a31515;"">&quot;http://io.weplay.io/socket.io/&quot;</span>),&nbsp;options);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Keep&nbsp;a&nbsp;reference&nbsp;to&nbsp;the&nbsp;root&nbsp;namespace</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Socket&nbsp;=&nbsp;manager.Socket;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Set&nbsp;up&nbsp;our&nbsp;event&nbsp;handlers.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Socket.On(<span style=""color:#2b91af;"">SocketIOEventTypes</span>.Connect,&nbsp;OnConnected);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Socket.On(<span style=""color:#a31515;"">&quot;joined&quot;</span>,&nbsp;OnJoined);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Socket.On(<span style=""color:#a31515;"">&quot;connections&quot;</span>,&nbsp;OnConnections);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Socket.On(<span style=""color:#a31515;"">&quot;join&quot;</span>,&nbsp;OnJoin);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Socket.On(<span style=""color:#a31515;"">&quot;move&quot;</span>,&nbsp;OnMove);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Socket.On(<span style=""color:#a31515;"">&quot;message&quot;</span>,&nbsp;OnMessage);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Socket.On(<span style=""color:#a31515;"">&quot;reload&quot;</span>,&nbsp;OnReload);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Don&#39;t&nbsp;waste&nbsp;cpu&nbsp;cycles&nbsp;on&nbsp;decoding&nbsp;the&nbsp;payload,&nbsp;we&nbsp;are&nbsp;expecting&nbsp;only&nbsp;binary&nbsp;data&nbsp;with&nbsp;this&nbsp;event,</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;&nbsp;and&nbsp;we&nbsp;can&nbsp;access&nbsp;it&nbsp;through&nbsp;the&nbsp;packet&#39;s&nbsp;Attachments&nbsp;property.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Socket.On(<span style=""color:#a31515;"">&quot;frame&quot;</span>,&nbsp;OnFrame,&nbsp;<span style=""color:green;"">/*autoDecodePayload:*/</span>&nbsp;<span style=""color:blue;"">false</span>);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Add&nbsp;error&nbsp;handler,&nbsp;so&nbsp;we&nbsp;can&nbsp;display&nbsp;it</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Socket.On(<span style=""color:#2b91af;"">SocketIOEventTypes</span>.Error,&nbsp;OnError);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;We&nbsp;set&nbsp;SocketOptions&#39;&nbsp;AutoConnect&nbsp;to&nbsp;false,&nbsp;so&nbsp;we&nbsp;have&nbsp;to&nbsp;call&nbsp;it&nbsp;manually.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;manager.Open();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;We&nbsp;are&nbsp;connecting&nbsp;to&nbsp;the&nbsp;server.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;State&nbsp;=&nbsp;<span style=""color:#2b91af;"">States</span>.Connecting;
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnDestroy()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Leaving&nbsp;this&nbsp;sample,&nbsp;close&nbsp;the&nbsp;socket</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Socket.Manager.Close();
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;Update()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Go&nbsp;back&nbsp;to&nbsp;the&nbsp;demo&nbsp;selector</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">Input</span>.GetKeyDown(<span style=""color:#2b91af;"">KeyCode</span>.Escape))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">SampleSelector</span>.SelectedSample.DestroyUnityObject();
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnGUI()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">switch</span>(State)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#2b91af;"">States</span>.Connecting:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUIHelper</span>.DrawArea(<span style=""color:#2b91af;"">GUIHelper</span>.ClientArea,&nbsp;<span style=""color:blue;"">true</span>,&nbsp;()&nbsp;=&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginVertical();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.FlexibleSpace();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUIHelper</span>.DrawCenteredText(<span style=""color:#a31515;"">&quot;Connecting&nbsp;to&nbsp;the&nbsp;server...&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.FlexibleSpace();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndVertical();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;});
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">break</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#2b91af;"">States</span>.WaitForNick:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUIHelper</span>.DrawArea(<span style=""color:#2b91af;"">GUIHelper</span>.ClientArea,&nbsp;<span style=""color:blue;"">true</span>,&nbsp;()&nbsp;=&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DrawLoginScreen();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;});
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">break</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#2b91af;"">States</span>.Joined:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUIHelper</span>.DrawArea(<span style=""color:#2b91af;"">GUIHelper</span>.ClientArea,&nbsp;<span style=""color:blue;"">true</span>,&nbsp;()&nbsp;=&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Draw&nbsp;Texture</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(FrameTexture&nbsp;!=&nbsp;<span style=""color:blue;"">null</span>)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Box(FrameTexture);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DrawControls();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DrawChat();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;});
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">break</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Helper&nbsp;Functions

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Called&nbsp;from&nbsp;an&nbsp;OnGUI&nbsp;event&nbsp;to&nbsp;draw&nbsp;the&nbsp;Login&nbsp;Screen.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;DrawLoginScreen()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginVertical();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.FlexibleSpace();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUIHelper</span>.DrawCenteredText(<span style=""color:#a31515;"">&quot;What&#39;s&nbsp;your&nbsp;nickname?&quot;</span>);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Nick&nbsp;=&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.TextField(Nick);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">GUILayout</span>.Button(<span style=""color:#a31515;"">&quot;Join&quot;</span>))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Join();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.FlexibleSpace();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndVertical();
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;DrawControls()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Controls:&quot;</span>);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">for</span>&nbsp;(<span style=""color:blue;"">int</span>&nbsp;i&nbsp;=&nbsp;0;&nbsp;i&nbsp;&lt;&nbsp;controls.Length;&nbsp;++i)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">GUILayout</span>.Button(controls[i]))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Socket.Emit(<span style=""color:#a31515;"">&quot;move&quot;</span>,&nbsp;controls[i]);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;&nbsp;Connections:&nbsp;&quot;</span>&nbsp;+&nbsp;connections);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;DrawChat(<span style=""color:blue;"">bool</span>&nbsp;withInput&nbsp;=&nbsp;<span style=""color:blue;"">true</span>)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginVertical();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Draw&nbsp;the&nbsp;messages</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;scrollPos&nbsp;=&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginScrollView(scrollPos,&nbsp;<span style=""color:blue;"">false</span>,&nbsp;<span style=""color:blue;"">false</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">for</span>&nbsp;(<span style=""color:blue;"">int</span>&nbsp;i&nbsp;=&nbsp;0;&nbsp;i&nbsp;&lt;&nbsp;messages.Count;&nbsp;++i)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(messages[i],&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.MinWidth(<span style=""color:#2b91af;"">Screen</span>.width));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndScrollView();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(withInput)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Your&nbsp;message:&nbsp;&quot;</span>);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;messageToSend&nbsp;=&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.TextField(messageToSend);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">GUILayout</span>.Button(<span style=""color:#a31515;"">&quot;Send&quot;</span>,&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.MaxWidth(100)))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SendMessage();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndVertical();
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Add&nbsp;a&nbsp;message&nbsp;to&nbsp;the&nbsp;message&nbsp;log</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;param&nbsp;name=</span><span style=""color:gray;"">&quot;msg&quot;</span><span style=""color:gray;"">&gt;&lt;/param&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;AddMessage(<span style=""color:blue;"">string</span>&nbsp;msg)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;messages.Insert(0,&nbsp;msg);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(messages.Count&nbsp;&gt;&nbsp;MaxMessages)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;messages.RemoveRange(MaxMessages,&nbsp;messages.Count&nbsp;-&nbsp;MaxMessages);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Send&nbsp;a&nbsp;chat&nbsp;message.&nbsp;The&nbsp;message&nbsp;must&nbsp;be&nbsp;in&nbsp;the&nbsp;messageToSend&nbsp;field.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;SendMessage()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:blue;"">string</span>.IsNullOrEmpty(messageToSend))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">return</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Socket.Emit(<span style=""color:#a31515;"">&quot;message&quot;</span>,&nbsp;messageToSend);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;AddMessage(<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;{0}:&nbsp;{1}&quot;</span>,&nbsp;Nick,&nbsp;messageToSend));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;messageToSend&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Join&nbsp;to&nbsp;the&nbsp;game&nbsp;with&nbsp;the&nbsp;nickname&nbsp;stored&nbsp;in&nbsp;the&nbsp;Nick&nbsp;field.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;Join()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">PlayerPrefs</span>.SetString(<span style=""color:#a31515;"">&quot;Nick&quot;</span>,&nbsp;Nick);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Socket.Emit(<span style=""color:#a31515;"">&quot;join&quot;</span>,&nbsp;Nick);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Reload&nbsp;the&nbsp;game.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;Reload()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;FrameTexture&nbsp;=&nbsp;<span style=""color:blue;"">null</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(Socket&nbsp;!=&nbsp;<span style=""color:blue;"">null</span>)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Socket.Manager.Close();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Socket&nbsp;=&nbsp;<span style=""color:blue;"">null</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Start();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;SocketIO&nbsp;Events

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Socket&nbsp;connected&nbsp;event.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnConnected(<span style=""color:#2b91af;"">Socket</span>&nbsp;socket,&nbsp;<span style=""color:#2b91af;"">Packet</span>&nbsp;packet,&nbsp;<span style=""color:blue;"">params</span>&nbsp;<span style=""color:blue;"">object</span>[]&nbsp;args)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">PlayerPrefs</span>.HasKey(<span style=""color:#a31515;"">&quot;Nick&quot;</span>))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Nick&nbsp;=&nbsp;<span style=""color:#2b91af;"">PlayerPrefs</span>.GetString(<span style=""color:#a31515;"">&quot;Nick&quot;</span>,&nbsp;<span style=""color:#a31515;"">&quot;NickName&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Join();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">else</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;State&nbsp;=&nbsp;<span style=""color:#2b91af;"">States</span>.WaitForNick;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;AddMessage(<span style=""color:#a31515;"">&quot;connected&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Local&nbsp;player&nbsp;joined&nbsp;after&nbsp;sending&nbsp;a&nbsp;&#39;join&#39;&nbsp;event</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnJoined(<span style=""color:#2b91af;"">Socket</span>&nbsp;socket,&nbsp;<span style=""color:#2b91af;"">Packet</span>&nbsp;packet,&nbsp;<span style=""color:blue;"">params</span>&nbsp;<span style=""color:blue;"">object</span>[]&nbsp;args)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;State&nbsp;=&nbsp;<span style=""color:#2b91af;"">States</span>.Joined;
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Server&nbsp;sent&nbsp;us&nbsp;a&nbsp;&#39;reload&#39;&nbsp;event.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnReload(<span style=""color:#2b91af;"">Socket</span>&nbsp;socket,&nbsp;<span style=""color:#2b91af;"">Packet</span>&nbsp;packet,&nbsp;<span style=""color:blue;"">params</span>&nbsp;<span style=""color:blue;"">object</span>[]&nbsp;args)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Reload();
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Someone&nbsp;wrote&nbsp;a&nbsp;message&nbsp;to&nbsp;the&nbsp;chat.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnMessage(<span style=""color:#2b91af;"">Socket</span>&nbsp;socket,&nbsp;<span style=""color:#2b91af;"">Packet</span>&nbsp;packet,&nbsp;<span style=""color:blue;"">params</span>&nbsp;<span style=""color:blue;"">object</span>[]&nbsp;args)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(args.Length&nbsp;==&nbsp;1)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;AddMessage(args[0]&nbsp;<span style=""color:blue;"">as</span>&nbsp;<span style=""color:blue;"">string</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">else</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;AddMessage(<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;{0}:&nbsp;{1}&quot;</span>,&nbsp;args[1],&nbsp;args[0]));
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Someone&nbsp;(including&nbsp;us)&nbsp;pressed&nbsp;a&nbsp;button.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnMove(<span style=""color:#2b91af;"">Socket</span>&nbsp;socket,&nbsp;<span style=""color:#2b91af;"">Packet</span>&nbsp;packet,&nbsp;<span style=""color:blue;"">params</span>&nbsp;<span style=""color:blue;"">object</span>[]&nbsp;args)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;AddMessage(<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;{0}&nbsp;pressed&nbsp;{1}&quot;</span>,&nbsp;args[1],&nbsp;args[0]));
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Someone&nbsp;joined&nbsp;to&nbsp;the&nbsp;game</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnJoin(<span style=""color:#2b91af;"">Socket</span>&nbsp;socket,&nbsp;<span style=""color:#2b91af;"">Packet</span>&nbsp;packet,&nbsp;<span style=""color:blue;"">params</span>&nbsp;<span style=""color:blue;"">object</span>[]&nbsp;args)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;loc&nbsp;=&nbsp;args.Length&nbsp;&gt;&nbsp;1&nbsp;?&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;({0})&quot;</span>,&nbsp;args[1])&nbsp;:&nbsp;<span style=""color:blue;"">string</span>.Empty;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;AddMessage(<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;{0}&nbsp;joined&nbsp;{1}&quot;</span>,&nbsp;args[0],&nbsp;loc));
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;How&nbsp;many&nbsp;players&nbsp;are&nbsp;connected&nbsp;to&nbsp;the&nbsp;game.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnConnections(<span style=""color:#2b91af;"">Socket</span>&nbsp;socket,&nbsp;<span style=""color:#2b91af;"">Packet</span>&nbsp;packet,&nbsp;<span style=""color:blue;"">params</span>&nbsp;<span style=""color:blue;"">object</span>[]&nbsp;args)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;connections&nbsp;=&nbsp;<span style=""color:#2b91af;"">Convert</span>.ToInt32(args[0]);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;The&nbsp;server&nbsp;sent&nbsp;us&nbsp;a&nbsp;new&nbsp;picture&nbsp;to&nbsp;draw&nbsp;the&nbsp;game.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnFrame(<span style=""color:#2b91af;"">Socket</span>&nbsp;socket,&nbsp;<span style=""color:#2b91af;"">Packet</span>&nbsp;packet,&nbsp;<span style=""color:blue;"">params</span>&nbsp;<span style=""color:blue;"">object</span>[]&nbsp;args)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(State&nbsp;!=&nbsp;<span style=""color:#2b91af;"">States</span>.Joined)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">return</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(FrameTexture&nbsp;==&nbsp;<span style=""color:blue;"">null</span>)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;FrameTexture&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">Texture2D</span>(0,&nbsp;0,&nbsp;<span style=""color:#2b91af;"">TextureFormat</span>.RGBA32,&nbsp;<span style=""color:blue;"">false</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;FrameTexture.filterMode&nbsp;=&nbsp;<span style=""color:#2b91af;"">FilterMode</span>.Point;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Binary&nbsp;data&nbsp;usage&nbsp;case&nbsp;1&nbsp;-&nbsp;using&nbsp;directly&nbsp;the&nbsp;Attachments&nbsp;property:</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">byte</span>[]&nbsp;data&nbsp;=&nbsp;packet.Attachments[0];

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Binary&nbsp;data&nbsp;usage&nbsp;case&nbsp;2&nbsp;-&nbsp;using&nbsp;the&nbsp;packet&#39;s&nbsp;ReconstructAttachmentAsIndex()&nbsp;function</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">/*packet.ReconstructAttachmentAsIndex();</span>
<span style=""color:green;"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;args&nbsp;=&nbsp;packet.Decode(socket.Manager.Encoder);</span>
<span style=""color:green;"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;data&nbsp;=&nbsp;packet.Attachments[Convert.ToInt32(args[0])];*/</span>

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Binary&nbsp;data&nbsp;usage&nbsp;case&nbsp;3&nbsp;-&nbsp;using&nbsp;the&nbsp;packet&#39;s&nbsp;ReconstructAttachmentAsBase64()&nbsp;function</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">/*packet.ReconstructAttachmentAsBase64();</span>
<span style=""color:green;"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;args&nbsp;=&nbsp;packet.Decode(socket.Manager.Encoder);</span>
<span style=""color:green;"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;data&nbsp;=&nbsp;Convert.FromBase64String(args[0]&nbsp;as&nbsp;string);*/</span>

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Load&nbsp;the&nbsp;server&nbsp;sent&nbsp;picture</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;FrameTexture.LoadImage(data);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Called&nbsp;on&nbsp;local&nbsp;or&nbsp;remote&nbsp;error.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnError(<span style=""color:#2b91af;"">Socket</span>&nbsp;socket,&nbsp;<span style=""color:#2b91af;"">Packet</span>&nbsp;packet,&nbsp;<span style=""color:blue;"">params</span>&nbsp;<span style=""color:blue;"">object</span>[]&nbsp;args)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;AddMessage(<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;--ERROR&nbsp;-&nbsp;{0}&quot;</span>,&nbsp;args[0].ToString()));
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>
}</pre>";
        #endregion

        #region SignalR SimpleStreamingSample
        public static string SignalR_SimpleStreamingSample = @"<pre style=""font-family:Consolas;font-size:13;color:black;background:silver;""><span style=""color:blue;"">using</span>&nbsp;System;

<span style=""color:blue;"">using</span>&nbsp;UnityEngine;
<span style=""color:blue;"">using</span>&nbsp;BestHTTP.SignalR;

<span style=""color:blue;"">sealed</span>&nbsp;<span style=""color:blue;"">class</span>&nbsp;<span style=""color:#2b91af;"">SimpleStreamingSample</span>&nbsp;:&nbsp;<span style=""color:#2b91af;"">MonoBehaviour</span>
{
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">readonly</span>&nbsp;<span style=""color:#2b91af;"">Uri</span>&nbsp;URI&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">Uri</span>(<span style=""color:#a31515;"">&quot;http://besthttpsignalr.azurewebsites.net/streaming-connection&quot;</span>);

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Reference&nbsp;to&nbsp;the&nbsp;SignalR&nbsp;Connection</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Connection</span>&nbsp;signalRConnection;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Helper&nbsp;GUI&nbsp;class&nbsp;to&nbsp;handle&nbsp;and&nbsp;display&nbsp;a&nbsp;string-list</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUIMessageList</span>&nbsp;messages&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">GUIMessageList</span>();

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Unity&nbsp;Events

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;Start()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Create&nbsp;the&nbsp;SignalR&nbsp;connection</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">Connection</span>(URI);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;set&nbsp;event&nbsp;handlers</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.OnNonHubMessage&nbsp;+=&nbsp;signalRConnection_OnNonHubMessage;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.OnStateChanged&nbsp;+=&nbsp;signalRConnection_OnStateChanged;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.OnError&nbsp;+=&nbsp;signalRConnection_OnError;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Start&nbsp;connecting&nbsp;to&nbsp;the&nbsp;server</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.Open();
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnDestroy()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Close&nbsp;the&nbsp;connection&nbsp;when&nbsp;the&nbsp;sample&nbsp;is&nbsp;closed</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.Close();
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnGUI()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUIHelper</span>.DrawArea(<span style=""color:#2b91af;"">GUIHelper</span>.ClientArea,&nbsp;<span style=""color:blue;"">true</span>,&nbsp;()&nbsp;=&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Messages&quot;</span>);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(20);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;messages.Draw(<span style=""color:#2b91af;"">Screen</span>.width&nbsp;-&nbsp;20,&nbsp;0);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;});
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;SignalR&nbsp;Events

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Handle&nbsp;Server-sent&nbsp;messages</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;signalRConnection_OnNonHubMessage(<span style=""color:#2b91af;"">Connection</span>&nbsp;connection,&nbsp;<span style=""color:blue;"">object</span>&nbsp;data)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;messages.Add(<span style=""color:#a31515;"">&quot;[Server&nbsp;Message]&nbsp;&quot;</span>&nbsp;+&nbsp;data.ToString());
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Display&nbsp;state&nbsp;changes</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;signalRConnection_OnStateChanged(<span style=""color:#2b91af;"">Connection</span>&nbsp;connection,&nbsp;<span style=""color:#2b91af;"">ConnectionStates</span>&nbsp;oldState,&nbsp;<span style=""color:#2b91af;"">ConnectionStates</span>&nbsp;newState)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;messages.Add(<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;[State&nbsp;Change]&nbsp;{0}&nbsp;=&gt;&nbsp;{1}&quot;</span>,&nbsp;oldState,&nbsp;newState));
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Display&nbsp;errors.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;signalRConnection_OnError(<span style=""color:#2b91af;"">Connection</span>&nbsp;connection,&nbsp;<span style=""color:blue;"">string</span>&nbsp;error)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;messages.Add(<span style=""color:#a31515;"">&quot;[Error]&nbsp;&quot;</span>&nbsp;+&nbsp;error);
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>
}</pre>";
        #endregion

        #region SignalR ConnectionAPISample
        public static string SignalR_ConnectionAPISample = @"<pre style=""font-family:Consolas;font-size:13;color:black;background:silver;""><span style=""color:blue;"">using</span>&nbsp;System;
<span style=""color:blue;"">using</span>&nbsp;System.Collections.Generic;

<span style=""color:blue;"">using</span>&nbsp;UnityEngine;
<span style=""color:blue;"">using</span>&nbsp;BestHTTP.SignalR;
<span style=""color:blue;"">using</span>&nbsp;BestHTTP.Cookies;

<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">sealed</span>&nbsp;<span style=""color:blue;"">class</span>&nbsp;<span style=""color:#2b91af;"">ConnectionAPISample</span>&nbsp;:&nbsp;<span style=""color:#2b91af;"">MonoBehaviour</span>
{
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">readonly</span>&nbsp;<span style=""color:#2b91af;"">Uri</span>&nbsp;URI&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">Uri</span>(<span style=""color:#a31515;"">&quot;http://besthttpsignalr.azurewebsites.net/raw-connection/&quot;</span>);

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Possible&nbsp;message&nbsp;types&nbsp;that&nbsp;the&nbsp;client&nbsp;can&nbsp;send&nbsp;to&nbsp;the&nbsp;server</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">enum</span>&nbsp;<span style=""color:#2b91af;"">MessageTypes</span>
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Send,&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;0</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Broadcast,&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;1</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Join,&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;2</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;PrivateMessage,&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;3</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;AddToGroup,&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;4</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;RemoveFromGroup,&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;5</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SendToGroup,&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;6</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;BroadcastExceptMe,&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;7</span>
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Private&nbsp;Fields

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Reference&nbsp;to&nbsp;the&nbsp;SignalR&nbsp;Connection</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Connection</span>&nbsp;signalRConnection;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Input&nbsp;strings</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;ToEveryBodyText&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;ToMeText&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;PrivateMessageText&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;PrivateMessageUserOrGroupName&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUIMessageList</span>&nbsp;messages&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">GUIMessageList</span>();

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Unity&nbsp;Events

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;Start()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Set&nbsp;a&nbsp;&quot;user&quot;&nbsp;cookie&nbsp;if&nbsp;we&nbsp;previously&nbsp;used&nbsp;the&nbsp;&#39;Enter&nbsp;Name&#39;&nbsp;button.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;The&nbsp;server&nbsp;will&nbsp;set&nbsp;this&nbsp;username&nbsp;to&nbsp;the&nbsp;new&nbsp;connection.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">PlayerPrefs</span>.HasKey(<span style=""color:#a31515;"">&quot;userName&quot;</span>))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">CookieJar</span>.Set(URI,&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">Cookie</span>(<span style=""color:#a31515;"">&quot;user&quot;</span>,&nbsp;<span style=""color:#2b91af;"">PlayerPrefs</span>.GetString(<span style=""color:#a31515;"">&quot;userName&quot;</span>)));

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">Connection</span>(URI);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;to&nbsp;serialize&nbsp;the&nbsp;Message&nbsp;class,&nbsp;set&nbsp;a&nbsp;more&nbsp;advanced&nbsp;json&nbsp;encoder</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.JsonEncoder&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;BestHTTP.SignalR.JsonEncoders.<span style=""color:#2b91af;"">LitJsonEncoder</span>();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;set&nbsp;up&nbsp;event&nbsp;handlers</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.OnStateChanged&nbsp;+=&nbsp;signalRConnection_OnStateChanged;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.OnNonHubMessage&nbsp;+=&nbsp;signalRConnection_OnGeneralMessage;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Start&nbsp;to&nbsp;connect&nbsp;to&nbsp;the&nbsp;server.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.Open();
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Draw&nbsp;the&nbsp;gui.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Get&nbsp;input&nbsp;strings.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Handle&nbsp;function&nbsp;calls.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnGUI()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUIHelper</span>.DrawArea(<span style=""color:#2b91af;"">GUIHelper</span>.ClientArea,&nbsp;<span style=""color:blue;"">true</span>,&nbsp;()&nbsp;=&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginVertical();

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;To&nbsp;Everybody
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;To&nbsp;Everybody&quot;</span>);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ToEveryBodyText&nbsp;=&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.TextField(ToEveryBodyText,&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.MinWidth(100));

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">GUILayout</span>.Button(<span style=""color:#a31515;"">&quot;Broadcast&quot;</span>))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Broadcast(ToEveryBodyText);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">GUILayout</span>.Button(<span style=""color:#a31515;"">&quot;Broadcast&nbsp;(All&nbsp;Except&nbsp;Me)&quot;</span>))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;BroadcastExceptMe(ToEveryBodyText);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">GUILayout</span>.Button(<span style=""color:#a31515;"">&quot;Enter&nbsp;Name&quot;</span>))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;EnterName(ToEveryBodyText);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">GUILayout</span>.Button(<span style=""color:#a31515;"">&quot;Join&nbsp;Group&quot;</span>))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;JoinGroup(ToEveryBodyText);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">GUILayout</span>.Button(<span style=""color:#a31515;"">&quot;Leave&nbsp;Group&quot;</span>))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;LeaveGroup(ToEveryBodyText);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();
<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;To&nbsp;Me
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;To&nbsp;Me&quot;</span>);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ToMeText&nbsp;=&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.TextField(ToMeText,&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.MinWidth(100));

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">GUILayout</span>.Button(<span style=""color:#a31515;"">&quot;Send&nbsp;to&nbsp;me&quot;</span>))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SendToMe(ToMeText);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();
<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Private&nbsp;Message
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Private&nbsp;Message&quot;</span>);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Message:&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;PrivateMessageText&nbsp;=&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.TextField(PrivateMessageText,&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.MinWidth(100));

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;User&nbsp;or&nbsp;Group&nbsp;name:&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;PrivateMessageUserOrGroupName&nbsp;=&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.TextField(PrivateMessageUserOrGroupName,&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.MinWidth(100));

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">GUILayout</span>.Button(<span style=""color:#a31515;"">&quot;Send&nbsp;to&nbsp;user&quot;</span>))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SendToUser(PrivateMessageUserOrGroupName,&nbsp;PrivateMessageText);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">GUILayout</span>.Button(<span style=""color:#a31515;"">&quot;Send&nbsp;to&nbsp;group&quot;</span>))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SendToGroup(PrivateMessageUserOrGroupName,&nbsp;PrivateMessageText);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();
<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(20);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(signalRConnection.State&nbsp;==&nbsp;<span style=""color:#2b91af;"">ConnectionStates</span>.Closed)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">GUILayout</span>.Button(<span style=""color:#a31515;"">&quot;Start&nbsp;Connection&quot;</span>))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.Open();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">else</span>&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">GUILayout</span>.Button(<span style=""color:#a31515;"">&quot;Stop&nbsp;Connection&quot;</span>))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.Close();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(20);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Draw&nbsp;the&nbsp;messages</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Messages&quot;</span>);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(20);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;messages.Draw(<span style=""color:#2b91af;"">Screen</span>.width&nbsp;-&nbsp;20,&nbsp;0);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndVertical();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;});
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnDestroy()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Close&nbsp;the&nbsp;connection&nbsp;when&nbsp;the&nbsp;sample&nbsp;is&nbsp;closed</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.Close();
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;SignalR&nbsp;Events

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Handle&nbsp;non-hub&nbsp;messages</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;signalRConnection_OnGeneralMessage(<span style=""color:#2b91af;"">Connection</span>&nbsp;manager,&nbsp;<span style=""color:blue;"">object</span>&nbsp;data)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;For&nbsp;now,&nbsp;just&nbsp;create&nbsp;a&nbsp;Json&nbsp;string&nbsp;from&nbsp;the&nbsp;sent&nbsp;data&nbsp;again</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;reencoded&nbsp;=&nbsp;BestHTTP.JSON.<span style=""color:#2b91af;"">Json</span>.Encode(data);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;and&nbsp;display&nbsp;it</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;messages.Add(<span style=""color:#a31515;"">&quot;[Server&nbsp;Message]&nbsp;&quot;</span>&nbsp;+&nbsp;reencoded);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;signalRConnection_OnStateChanged(<span style=""color:#2b91af;"">Connection</span>&nbsp;manager,&nbsp;<span style=""color:#2b91af;"">ConnectionStates</span>&nbsp;oldState,&nbsp;<span style=""color:#2b91af;"">ConnectionStates</span>&nbsp;newState)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;display&nbsp;state&nbsp;changes</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;messages.Add(<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;[State&nbsp;Change]&nbsp;{0}&nbsp;=&gt;&nbsp;{1}&quot;</span>,&nbsp;oldState.ToString(),&nbsp;newState.ToString()));
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;To&nbsp;EveryBody&nbsp;Functions

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Broadcast&nbsp;a&nbsp;message&nbsp;to&nbsp;all&nbsp;connected&nbsp;clients</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;Broadcast(<span style=""color:blue;"">string</span>&nbsp;text)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.Send(<span style=""color:blue;"">new</span>&nbsp;{&nbsp;Type&nbsp;=&nbsp;<span style=""color:#2b91af;"">MessageTypes</span>.Broadcast,&nbsp;Value&nbsp;=&nbsp;text&nbsp;});
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Broadcast&nbsp;a&nbsp;message&nbsp;to&nbsp;all&nbsp;connected&nbsp;clients,&nbsp;except&nbsp;this&nbsp;client</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;BroadcastExceptMe(<span style=""color:blue;"">string</span>&nbsp;text)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.Send(<span style=""color:blue;"">new</span>&nbsp;{&nbsp;Type&nbsp;=&nbsp;<span style=""color:#2b91af;"">MessageTypes</span>.BroadcastExceptMe,&nbsp;Value&nbsp;=&nbsp;text&nbsp;});
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Set&nbsp;a&nbsp;name&nbsp;for&nbsp;this&nbsp;connection.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;EnterName(<span style=""color:blue;"">string</span>&nbsp;name)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.Send(<span style=""color:blue;"">new</span>&nbsp;{&nbsp;Type&nbsp;=&nbsp;<span style=""color:#2b91af;"">MessageTypes</span>.Join,&nbsp;Value&nbsp;=&nbsp;name&nbsp;});
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Join&nbsp;to&nbsp;a&nbsp;group</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;JoinGroup(<span style=""color:blue;"">string</span>&nbsp;groupName)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.Send(<span style=""color:blue;"">new</span>&nbsp;{&nbsp;Type&nbsp;=&nbsp;<span style=""color:#2b91af;"">MessageTypes</span>.AddToGroup,&nbsp;Value&nbsp;=&nbsp;groupName&nbsp;});
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Leave&nbsp;a&nbsp;group</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;LeaveGroup(<span style=""color:blue;"">string</span>&nbsp;groupName)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.Send(<span style=""color:blue;"">new</span>&nbsp;{&nbsp;Type&nbsp;=&nbsp;<span style=""color:#2b91af;"">MessageTypes</span>.RemoveFromGroup,&nbsp;Value&nbsp;=&nbsp;groupName&nbsp;});
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;To&nbsp;Me&nbsp;Functions

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Send&nbsp;a&nbsp;message&nbsp;to&nbsp;the&nbsp;very&nbsp;same&nbsp;client&nbsp;through&nbsp;the&nbsp;server</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;SendToMe(<span style=""color:blue;"">string</span>&nbsp;text)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.Send(<span style=""color:blue;"">new</span>&nbsp;{&nbsp;Type&nbsp;=&nbsp;<span style=""color:#2b91af;"">MessageTypes</span>.Send,&nbsp;Value&nbsp;=&nbsp;text&nbsp;});
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Private&nbsp;Message&nbsp;Functions

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Send&nbsp;a&nbsp;private&nbsp;message&nbsp;to&nbsp;a&nbsp;user</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;SendToUser(<span style=""color:blue;"">string</span>&nbsp;userOrGroupName,&nbsp;<span style=""color:blue;"">string</span>&nbsp;text)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.Send(<span style=""color:blue;"">new</span>&nbsp;{&nbsp;Type&nbsp;=&nbsp;<span style=""color:#2b91af;"">MessageTypes</span>.PrivateMessage,&nbsp;Value&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;{0}|{1}&quot;</span>,&nbsp;userOrGroupName,&nbsp;text)&nbsp;});
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Send&nbsp;a&nbsp;message&nbsp;to&nbsp;a&nbsp;group</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;SendToGroup(<span style=""color:blue;"">string</span>&nbsp;userOrGroupName,&nbsp;<span style=""color:blue;"">string</span>&nbsp;text)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.Send(<span style=""color:blue;"">new</span>&nbsp;{&nbsp;Type&nbsp;=&nbsp;<span style=""color:#2b91af;"">MessageTypes</span>.SendToGroup,&nbsp;Value&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;{0}|{1}&quot;</span>,&nbsp;userOrGroupName,&nbsp;text)&nbsp;});
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>
}</pre>";
        #endregion

        #region SignalR ConnectionStatusSample
        public static string SignalR_ConnectionStatusSample = @"<pre style=""font-family:Consolas;font-size:13;color:black;background:silver;""><span style=""color:blue;"">using</span>&nbsp;System;
<span style=""color:blue;"">using</span>&nbsp;System.Collections.Generic;

<span style=""color:blue;"">using</span>&nbsp;UnityEngine;
<span style=""color:blue;"">using</span>&nbsp;BestHTTP.SignalR;
<span style=""color:blue;"">using</span>&nbsp;BestHTTP.SignalR.Hubs;

<span style=""color:blue;"">sealed</span>&nbsp;<span style=""color:blue;"">class</span>&nbsp;<span style=""color:#2b91af;"">ConnectionStatusSample</span>&nbsp;:&nbsp;<span style=""color:#2b91af;"">MonoBehaviour</span>
{
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">readonly</span>&nbsp;<span style=""color:#2b91af;"">Uri</span>&nbsp;URI&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">Uri</span>(<span style=""color:#a31515;"">&quot;http://besthttpsignalr.azurewebsites.net/signalr&quot;</span>);

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Reference&nbsp;to&nbsp;the&nbsp;SignalR&nbsp;Connection</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Connection</span>&nbsp;signalRConnection;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUIMessageList</span>&nbsp;messages&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">GUIMessageList</span>();

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Unity&nbsp;Events

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;Start()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Connect&nbsp;to&nbsp;the&nbsp;StatusHub&nbsp;hub</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">Connection</span>(URI,&nbsp;<span style=""color:#a31515;"">&quot;StatusHub&quot;</span>);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;General&nbsp;events</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.OnNonHubMessage&nbsp;+=&nbsp;signalRConnection_OnNonHubMessage;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.OnError&nbsp;+=&nbsp;signalRConnection_OnError;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.OnStateChanged&nbsp;+=&nbsp;signalRConnection_OnStateChanged;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Set&nbsp;up&nbsp;a&nbsp;callback&nbsp;for&nbsp;Hub&nbsp;events</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection[<span style=""color:#a31515;"">&quot;StatusHub&quot;</span>].OnMethodCall&nbsp;+=&nbsp;statusHub_OnMethodCall;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Connect&nbsp;to&nbsp;the&nbsp;server</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.Open();
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnDestroy()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Close&nbsp;the&nbsp;connection&nbsp;when&nbsp;we&nbsp;are&nbsp;closing&nbsp;the&nbsp;sample</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.Close();
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnGUI()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUIHelper</span>.DrawArea(<span style=""color:#2b91af;"">GUIHelper</span>.ClientArea,&nbsp;<span style=""color:blue;"">true</span>,&nbsp;()&nbsp;=&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">GUILayout</span>.Button(<span style=""color:#a31515;"">&quot;START&quot;</span>)&nbsp;&amp;&amp;&nbsp;signalRConnection.State&nbsp;!=&nbsp;<span style=""color:#2b91af;"">ConnectionStates</span>.Connected)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.Open();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">GUILayout</span>.Button(<span style=""color:#a31515;"">&quot;STOP&quot;</span>)&nbsp;&amp;&amp;&nbsp;signalRConnection.State&nbsp;==&nbsp;<span style=""color:#2b91af;"">ConnectionStates</span>.Connected)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.Close();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;messages.Clear();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">GUILayout</span>.Button(<span style=""color:#a31515;"">&quot;PING&quot;</span>)&nbsp;&amp;&amp;&nbsp;signalRConnection.State&nbsp;==&nbsp;<span style=""color:#2b91af;"">ConnectionStates</span>.Connected)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Call&nbsp;a&nbsp;Hub-method&nbsp;on&nbsp;the&nbsp;server.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection[<span style=""color:#a31515;"">&quot;StatusHub&quot;</span>].Call(<span style=""color:#a31515;"">&quot;Ping&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(20);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Connection&nbsp;Status&nbsp;Messages&quot;</span>);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(20);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;messages.Draw(<span style=""color:#2b91af;"">Screen</span>.width&nbsp;-&nbsp;20,&nbsp;0);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;});
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;SignalR&nbsp;Events

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Called&nbsp;on&nbsp;server-sent&nbsp;non-hub&nbsp;messages.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;signalRConnection_OnNonHubMessage(<span style=""color:#2b91af;"">Connection</span>&nbsp;manager,&nbsp;<span style=""color:blue;"">object</span>&nbsp;data)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;messages.Add(<span style=""color:#a31515;"">&quot;[Server&nbsp;Message]&nbsp;&quot;</span>&nbsp;+&nbsp;data.ToString());
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Called&nbsp;when&nbsp;the&nbsp;SignalR&nbsp;Connection&#39;s&nbsp;state&nbsp;changes.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;signalRConnection_OnStateChanged(<span style=""color:#2b91af;"">Connection</span>&nbsp;manager,&nbsp;<span style=""color:#2b91af;"">ConnectionStates</span>&nbsp;oldState,&nbsp;<span style=""color:#2b91af;"">ConnectionStates</span>&nbsp;newState)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;messages.Add(<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;[State&nbsp;Change]&nbsp;{0}&nbsp;=&gt;&nbsp;{1}&quot;</span>,&nbsp;oldState,&nbsp;newState));
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Called&nbsp;when&nbsp;an&nbsp;error&nbsp;occures.&nbsp;The&nbsp;plugin&nbsp;may&nbsp;close&nbsp;the&nbsp;connection&nbsp;after&nbsp;this&nbsp;event.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;signalRConnection_OnError(<span style=""color:#2b91af;"">Connection</span>&nbsp;manager,&nbsp;<span style=""color:blue;"">string</span>&nbsp;error)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;messages.Add(<span style=""color:#a31515;"">&quot;[Error]&nbsp;&quot;</span>&nbsp;+&nbsp;error);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Called&nbsp;when&nbsp;the&nbsp;&quot;StatusHub&quot;&nbsp;hub&nbsp;wants&nbsp;to&nbsp;call&nbsp;a&nbsp;method&nbsp;on&nbsp;this&nbsp;client.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;statusHub_OnMethodCall(<span style=""color:#2b91af;"">Hub</span>&nbsp;hub,&nbsp;<span style=""color:blue;"">string</span>&nbsp;method,&nbsp;<span style=""color:blue;"">params</span>&nbsp;<span style=""color:blue;"">object</span>[]&nbsp;args)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;id&nbsp;=&nbsp;args.Length&nbsp;&gt;&nbsp;0&nbsp;?&nbsp;args[0]&nbsp;<span style=""color:blue;"">as</span>&nbsp;<span style=""color:blue;"">string</span>&nbsp;:&nbsp;<span style=""color:blue;"">string</span>.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;when&nbsp;=&nbsp;args.Length&nbsp;&gt;&nbsp;1&nbsp;?&nbsp;args[1].ToString()&nbsp;:&nbsp;<span style=""color:blue;"">string</span>.Empty;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">switch</span>&nbsp;(method)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#a31515;"">&quot;joined&quot;</span>:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;messages.Add(<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;[{0}]&nbsp;{1}&nbsp;joined&nbsp;at&nbsp;{2}&quot;</span>,&nbsp;hub.Name,&nbsp;id,&nbsp;when));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">break</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#a31515;"">&quot;rejoined&quot;</span>:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;messages.Add(<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;[{0}]&nbsp;{1}&nbsp;reconnected&nbsp;at&nbsp;{2}&quot;</span>,&nbsp;hub.Name,&nbsp;id,&nbsp;when));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">break</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#a31515;"">&quot;leave&quot;</span>:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;messages.Add(<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;[{0}]&nbsp;{1}&nbsp;leaved&nbsp;at&nbsp;{2}&quot;</span>,&nbsp;hub.Name,&nbsp;id,&nbsp;when));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">break</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">default</span>:&nbsp;<span style=""color:green;"">//&nbsp;pong</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;messages.Add(<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;[{0}]&nbsp;{1}&quot;</span>,&nbsp;hub.Name,&nbsp;method));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">break</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>
}</pre>";
        #endregion

        #region SignalR DemoHubSample
        public static string SignalR_DemoHubSample = @"<pre style=""font-family:Consolas;font-size:13;color:black;background:silver;""><span style=""color:blue;"">using</span>&nbsp;System;

<span style=""color:blue;"">using</span>&nbsp;UnityEngine;

<span style=""color:blue;"">using</span>&nbsp;BestHTTP.SignalR;
<span style=""color:blue;"">using</span>&nbsp;BestHTTP.SignalR.Hubs;
<span style=""color:blue;"">using</span>&nbsp;BestHTTP.SignalR.Messages;
<span style=""color:blue;"">using</span>&nbsp;BestHTTP.SignalR.JsonEncoders;

<span style=""color:blue;"">class</span>&nbsp;<span style=""color:#2b91af;"">DemoHubSample</span>&nbsp;:&nbsp;<span style=""color:#2b91af;"">MonoBehaviour</span>
{
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">readonly</span>&nbsp;<span style=""color:#2b91af;"">Uri</span>&nbsp;URI&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">Uri</span>(<span style=""color:#a31515;"">&quot;http://besthttpsignalr.azurewebsites.net/signalr&quot;</span>);

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;The&nbsp;SignalR&nbsp;connection&nbsp;instance</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Connection</span>&nbsp;signalRConnection;
&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;DemoHub&nbsp;client&nbsp;side&nbsp;implementation</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">DemoHub</span>&nbsp;demoHub;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;TypedDemoHub&nbsp;client&nbsp;side&nbsp;implementation</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">TypedDemoHub</span>&nbsp;typedDemoHub;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;&nbsp;VB&nbsp;.NET&nbsp;Hub</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Hub</span>&nbsp;vbDemoHub;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Result&nbsp;of&nbsp;the&nbsp;VB&nbsp;demo&#39;s&nbsp;ReadStateValue&nbsp;call</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;vbReadStateResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Vector2</span>&nbsp;scrollPos;&nbsp;&nbsp;&nbsp;&nbsp;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;Start()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Create&nbsp;the&nbsp;hubs</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;demoHub&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">DemoHub</span>();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;typedDemoHub&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">TypedDemoHub</span>();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;vbDemoHub&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">Hub</span>(<span style=""color:#a31515;"">&quot;vbdemo&quot;</span>);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Create&nbsp;the&nbsp;SignalR&nbsp;connection,&nbsp;passing&nbsp;all&nbsp;the&nbsp;three&nbsp;hubs&nbsp;to&nbsp;it</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">Connection</span>(URI,&nbsp;demoHub,&nbsp;typedDemoHub,&nbsp;vbDemoHub);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Switch&nbsp;from&nbsp;the&nbsp;default&nbsp;encoder&nbsp;to&nbsp;the&nbsp;LitJson&nbsp;Encoder&nbsp;becouse&nbsp;it&nbsp;can&nbsp;handle&nbsp;the&nbsp;complex&nbsp;types&nbsp;too.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.JsonEncoder&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">LitJsonEncoder</span>();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Call&nbsp;the&nbsp;demo&nbsp;functions&nbsp;when&nbsp;we&nbsp;successfully&nbsp;connect&nbsp;to&nbsp;the&nbsp;server</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.OnConnected&nbsp;+=&nbsp;(connection)&nbsp;=&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">var</span>&nbsp;person&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;{&nbsp;Name&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;Foo&quot;</span>,&nbsp;Age&nbsp;=&nbsp;20,&nbsp;Address&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;{&nbsp;Street&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;One&nbsp;Microsoft&nbsp;Way&quot;</span>,&nbsp;Zip&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;98052&quot;</span>&nbsp;}&nbsp;};

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Call&nbsp;the&nbsp;demo&nbsp;functions</span>

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;demoHub.ReportProgress(<span style=""color:#a31515;"">&quot;Long&nbsp;running&nbsp;job!&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;demoHub.AddToGroups();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;demoHub.GetValue();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;demoHub.TaskWithException();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;demoHub.GenericTaskWithException();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;demoHub.SynchronousException();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;demoHub.DynamicTask();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;demoHub.PassingDynamicComplex(person);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;demoHub.SimpleArray(<span style=""color:blue;"">new</span>&nbsp;<span style=""color:blue;"">int</span>[]&nbsp;{&nbsp;5,&nbsp;5,&nbsp;6&nbsp;});
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;demoHub.ComplexType(person);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;demoHub.ComplexArray(<span style=""color:blue;"">new</span>&nbsp;<span style=""color:blue;"">object</span>[]&nbsp;{&nbsp;person,&nbsp;person,&nbsp;person&nbsp;});

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;demoHub.Overload();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;set&nbsp;some&nbsp;state</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;demoHub.State[<span style=""color:#a31515;"">&quot;name&quot;</span>]&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;Testing&nbsp;state!&quot;</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;demoHub.ReadStateValue();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;demoHub.PlainTask();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;demoHub.GenericTaskWithContinueWith();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;typedDemoHub.Echo(<span style=""color:#a31515;"">&quot;Typed&nbsp;echo&nbsp;callback&quot;</span>);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;vbDemo&nbsp;is&nbsp;not&nbsp;wrapped&nbsp;in&nbsp;a&nbsp;hub&nbsp;class,&nbsp;it&nbsp;would&nbsp;contain&nbsp;only&nbsp;one&nbsp;function</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;vbDemoHub.Call(<span style=""color:#a31515;"">&quot;readStateValue&quot;</span>,&nbsp;(hub,&nbsp;msg,&nbsp;result)&nbsp;=&gt;&nbsp;vbReadStateResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;Read&nbsp;some&nbsp;state&nbsp;from&nbsp;VB.NET!&nbsp;=&gt;&nbsp;{0}&quot;</span>,&nbsp;result.ReturnValue&nbsp;==&nbsp;<span style=""color:blue;"">null</span>&nbsp;?&nbsp;<span style=""color:#a31515;"">&quot;undefined&quot;</span>&nbsp;:&nbsp;result.ReturnValue.ToString()));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;};

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Start&nbsp;opening&nbsp;the&nbsp;signalR&nbsp;connection</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.Open();
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnDestroy()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Close&nbsp;the&nbsp;connection&nbsp;when&nbsp;we&nbsp;are&nbsp;closing&nbsp;this&nbsp;sample</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.Close();
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnGUI()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUIHelper</span>.DrawArea(<span style=""color:#2b91af;"">GUIHelper</span>.ClientArea,&nbsp;<span style=""color:blue;"">true</span>,&nbsp;()&nbsp;=&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;scrollPos&nbsp;=&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginScrollView(scrollPos,&nbsp;<span style=""color:blue;"">false</span>,&nbsp;<span style=""color:blue;"">false</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginVertical();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;demoHub.Draw();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;typedDemoHub.Draw();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Read&nbsp;State&nbsp;Value&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(20);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(vbReadStateResult);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(10);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndVertical();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndScrollView();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;});

&nbsp;&nbsp;&nbsp;&nbsp;}
}

<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Wrapper&nbsp;class&nbsp;of&nbsp;the&nbsp;&#39;TypedDemoHub&#39;&nbsp;hub</span>
<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
<span style=""color:blue;"">class</span>&nbsp;<span style=""color:#2b91af;"">TypedDemoHub</span>&nbsp;:&nbsp;<span style=""color:#2b91af;"">Hub</span>
{
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;typedEchoResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;typedEchoClientResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;TypedDemoHub()
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:<span style=""color:blue;"">base</span>(<span style=""color:#a31515;"">&quot;typeddemohub&quot;</span>)
&nbsp;&nbsp;&nbsp;&nbsp;{

&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Called&nbsp;by&nbsp;the&nbsp;Connection&nbsp;class&nbsp;to&nbsp;be&nbsp;able&nbsp;to&nbsp;set&nbsp;up&nbsp;mappings.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">override</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;Setup()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Setup&nbsp;server-called&nbsp;functions</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">base</span>.On(<span style=""color:#a31515;"">&quot;Echo&quot;</span>,&nbsp;Echo);
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Server&nbsp;Called&nbsp;Functions

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Server-called,&nbsp;client&nbsp;side&nbsp;implementation&nbsp;of&nbsp;the&nbsp;Echo&nbsp;function</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;Echo(<span style=""color:#2b91af;"">Hub</span>&nbsp;hub,&nbsp;<span style=""color:#2b91af;"">MethodCallMessage</span>&nbsp;methodCall)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;typedEchoClientResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;{0}&nbsp;#{1}&nbsp;triggered!&quot;</span>,&nbsp;methodCall.Arguments[0],&nbsp;methodCall.Arguments[1]);
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Client&nbsp;Called&nbsp;Function(s)

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Client-called,&nbsp;server&nbsp;side&nbsp;implementation&nbsp;of&nbsp;the&nbsp;Echo&nbsp;function.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;When&nbsp;the&nbsp;function&nbsp;successfully&nbsp;executed&nbsp;on&nbsp;the&nbsp;server&nbsp;the&nbsp;OnEcho_Done&nbsp;callback&nbsp;function&nbsp;will&nbsp;be&nbsp;called.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;Echo(<span style=""color:blue;"">string</span>&nbsp;msg)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">base</span>.Call(<span style=""color:#a31515;"">&quot;echo&quot;</span>,&nbsp;OnEcho_Done,&nbsp;msg);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;When&nbsp;the&nbsp;function&nbsp;successfully&nbsp;executed&nbsp;on&nbsp;the&nbsp;server&nbsp;this&nbsp;callback&nbsp;function&nbsp;will&nbsp;be&nbsp;called.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnEcho_Done(<span style=""color:#2b91af;"">Hub</span>&nbsp;hub,&nbsp;<span style=""color:#2b91af;"">ClientMessage</span>&nbsp;originalMessage,&nbsp;<span style=""color:#2b91af;"">ResultMessage</span>&nbsp;result)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;typedEchoResult&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;TypedDemoHub.Echo(string&nbsp;message)&nbsp;invoked!&quot;</span>;
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;Draw()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Typed&nbsp;callback&quot;</span>);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(20);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginVertical();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(typedEchoResult);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(typedEchoClientResult);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndVertical();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(10);
&nbsp;&nbsp;&nbsp;&nbsp;}
}

<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;A&nbsp;wrapper&nbsp;class&nbsp;for&nbsp;the&nbsp;&#39;DemoHub&#39;&nbsp;hub.</span>
<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
<span style=""color:blue;"">class</span>&nbsp;<span style=""color:#2b91af;"">DemoHub</span>&nbsp;:&nbsp;<span style=""color:#2b91af;"">Hub</span>
{
<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Private&nbsp;fields

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;These&nbsp;fields&nbsp;are&nbsp;here&nbsp;to&nbsp;store&nbsp;results&nbsp;of&nbsp;the&nbsp;function&nbsp;calls</span>

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">float</span>&nbsp;longRunningJobProgress&nbsp;=&nbsp;0f;
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;longRunningJobStatus&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;Not&nbsp;Started!&quot;</span>;
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;fromArbitraryCodeResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;groupAddedResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;dynamicTaskResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;genericTaskResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;taskWithExceptionResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;genericTaskWithExceptionResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;synchronousExceptionResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;invokingHubMethodWithDynamicResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;simpleArrayResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;complexTypeResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;complexArrayResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;voidOverloadResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;intOverloadResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;readStateResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;plainTaskResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;genericTaskWithContinueWithResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUIMessageList</span>&nbsp;invokeResults&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">GUIMessageList</span>();

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;DemoHub()
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:&nbsp;<span style=""color:blue;"">base</span>(<span style=""color:#a31515;"">&quot;demo&quot;</span>)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Called&nbsp;by&nbsp;the&nbsp;Connection&nbsp;class&nbsp;to&nbsp;be&nbsp;able&nbsp;to&nbsp;set&nbsp;up&nbsp;mappings.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">override</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;Setup()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Setup&nbsp;server-called&nbsp;functions</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">base</span>.On(<span style=""color:#a31515;"">&quot;invoke&quot;</span>,&nbsp;Invoke);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">base</span>.On(<span style=""color:#a31515;"">&quot;signal&quot;</span>,&nbsp;Signal);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">base</span>.On(<span style=""color:#a31515;"">&quot;groupAdded&quot;</span>,&nbsp;GroupAdded);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">base</span>.On(<span style=""color:#a31515;"">&quot;fromArbitraryCode&quot;</span>,&nbsp;FromArbitraryCode);
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Client&nbsp;Called&nbsp;Functions

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;ReportProgress

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;ReportProgress(<span style=""color:blue;"">string</span>&nbsp;arg)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Call(<span style=""color:#a31515;"">&quot;reportProgress&quot;</span>,&nbsp;OnLongRunningJob_Done,&nbsp;<span style=""color:blue;"">null</span>,&nbsp;OnLongRunningJob_Progress,&nbsp;arg);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnLongRunningJob_Progress(<span style=""color:#2b91af;"">Hub</span>&nbsp;hub,&nbsp;<span style=""color:#2b91af;"">ClientMessage</span>&nbsp;originialMessage,&nbsp;<span style=""color:#2b91af;"">ProgressMessage</span>&nbsp;progress)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;longRunningJobProgress&nbsp;=&nbsp;(<span style=""color:blue;"">float</span>)progress.Progress;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;longRunningJobStatus&nbsp;=&nbsp;progress.Progress.ToString()&nbsp;+&nbsp;<span style=""color:#a31515;"">&quot;%&quot;</span>;
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnLongRunningJob_Done(<span style=""color:#2b91af;"">Hub</span>&nbsp;hub,&nbsp;<span style=""color:#2b91af;"">ClientMessage</span>&nbsp;originalMessage,&nbsp;<span style=""color:#2b91af;"">ResultMessage</span>&nbsp;result)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;longRunningJobStatus&nbsp;=&nbsp;result.ReturnValue.ToString();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;MultipleCalls();
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;MultipleCalls()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">base</span>.Call(<span style=""color:#a31515;"">&quot;multipleCalls&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;DynamicTask

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;DynamicTask()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">base</span>.Call(<span style=""color:#a31515;"">&quot;dynamicTask&quot;</span>,&nbsp;OnDynamicTask_Done,&nbsp;OnDynamicTask_Failed);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnDynamicTask_Failed(<span style=""color:#2b91af;"">Hub</span>&nbsp;hub,&nbsp;<span style=""color:#2b91af;"">ClientMessage</span>&nbsp;originalMessage,&nbsp;<span style=""color:#2b91af;"">ResultMessage</span>&nbsp;result)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;dynamicTaskResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;The&nbsp;dynamic&nbsp;task&nbsp;failed&nbsp;:(&nbsp;{0}&quot;</span>,&nbsp;result.ErrorMessage);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnDynamicTask_Done(<span style=""color:#2b91af;"">Hub</span>&nbsp;hub,&nbsp;<span style=""color:#2b91af;"">ClientMessage</span>&nbsp;originalMessage,&nbsp;<span style=""color:#2b91af;"">ResultMessage</span>&nbsp;result)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;dynamicTaskResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;The&nbsp;dynamic&nbsp;task!&nbsp;{0}&quot;</span>,&nbsp;result.ReturnValue);
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;AddToGroups()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">base</span>.Call(<span style=""color:#a31515;"">&quot;addToGroups&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;GetValue()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">base</span>.Call(<span style=""color:#a31515;"">&quot;getValue&quot;</span>,&nbsp;(hub,&nbsp;msg,&nbsp;result)&nbsp;=&gt;&nbsp;genericTaskResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;The&nbsp;value&nbsp;is&nbsp;{0}&nbsp;after&nbsp;5&nbsp;seconds&quot;</span>,&nbsp;result.ReturnValue));
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;TaskWithException()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">base</span>.Call(<span style=""color:#a31515;"">&quot;taskWithException&quot;</span>,&nbsp;<span style=""color:blue;"">null</span>,&nbsp;(hub,&nbsp;msg,&nbsp;result)&nbsp;=&gt;&nbsp;taskWithExceptionResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;Error:&nbsp;{0}&quot;</span>,&nbsp;result.ErrorMessage));
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;GenericTaskWithException()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">base</span>.Call(<span style=""color:#a31515;"">&quot;genericTaskWithException&quot;</span>,&nbsp;<span style=""color:blue;"">null</span>,&nbsp;(hub,&nbsp;msg,&nbsp;result)&nbsp;=&gt;&nbsp;genericTaskWithExceptionResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;Error:&nbsp;{0}&quot;</span>,&nbsp;result.ErrorMessage));
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;SynchronousException()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">base</span>.Call(<span style=""color:#a31515;"">&quot;synchronousException&quot;</span>,&nbsp;<span style=""color:blue;"">null</span>,&nbsp;(hub,&nbsp;msg,&nbsp;result)&nbsp;=&gt;&nbsp;synchronousExceptionResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;Error:&nbsp;{0}&quot;</span>,&nbsp;result.ErrorMessage));
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;PassingDynamicComplex(<span style=""color:blue;"">object</span>&nbsp;person)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">base</span>.Call(<span style=""color:#a31515;"">&quot;passingDynamicComplex&quot;</span>,&nbsp;(hub,&nbsp;msg,&nbsp;result)&nbsp;=&gt;&nbsp;invokingHubMethodWithDynamicResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;The&nbsp;person&#39;s&nbsp;age&nbsp;is&nbsp;{0}&quot;</span>,&nbsp;result.ReturnValue),&nbsp;person);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;SimpleArray(<span style=""color:blue;"">int</span>[]&nbsp;array)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">base</span>.Call(<span style=""color:#a31515;"">&quot;simpleArray&quot;</span>,&nbsp;(hub,&nbsp;msg,&nbsp;result)&nbsp;=&gt;&nbsp;simpleArrayResult&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;Simple&nbsp;array&nbsp;works!&quot;</span>,&nbsp;array);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;ComplexType(<span style=""color:blue;"">object</span>&nbsp;person)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">base</span>.Call(<span style=""color:#a31515;"">&quot;complexType&quot;</span>,&nbsp;(hub,&nbsp;msg,&nbsp;result)&nbsp;=&gt;&nbsp;complexTypeResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;Complex&nbsp;Type&nbsp;-&gt;&nbsp;{0}&quot;</span>,&nbsp;(<span style=""color:blue;"">this</span>&nbsp;<span style=""color:blue;"">as</span>&nbsp;<span style=""color:#2b91af;"">IHub</span>).Connection.JsonEncoder.Encode(<span style=""color:blue;"">this</span>.State[<span style=""color:#a31515;"">&quot;person&quot;</span>])),&nbsp;person);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;ComplexArray(<span style=""color:blue;"">object</span>[]&nbsp;complexArray)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;We&nbsp;need&nbsp;to&nbsp;cast&nbsp;the&nbsp;object&nbsp;array&nbsp;to&nbsp;object&nbsp;to&nbsp;keep&nbsp;it&nbsp;as&nbsp;an&nbsp;array</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;http://stackoverflow.com/questions/36350/how-to-pass-a-single-object-to-a-params-object</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">base</span>.Call(<span style=""color:#a31515;"">&quot;ComplexArray&quot;</span>,&nbsp;(hub,&nbsp;msg,&nbsp;result)&nbsp;=&gt;&nbsp;complexArrayResult&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;Complex&nbsp;Array&nbsp;Works!&quot;</span>,&nbsp;(<span style=""color:blue;"">object</span>)complexArray);
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Overloads

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;Overload()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">base</span>.Call(<span style=""color:#a31515;"">&quot;Overload&quot;</span>,&nbsp;OnVoidOverload_Done);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnVoidOverload_Done(<span style=""color:#2b91af;"">Hub</span>&nbsp;hub,&nbsp;<span style=""color:#2b91af;"">ClientMessage</span>&nbsp;originalMessage,&nbsp;<span style=""color:#2b91af;"">ResultMessage</span>&nbsp;result)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;voidOverloadResult&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;Void&nbsp;Overload&nbsp;called&quot;</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Overload(101);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;Overload(<span style=""color:blue;"">int</span>&nbsp;number)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">base</span>.Call(<span style=""color:#a31515;"">&quot;Overload&quot;</span>,&nbsp;OnIntOverload_Done,&nbsp;number);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnIntOverload_Done(<span style=""color:#2b91af;"">Hub</span>&nbsp;hub,&nbsp;<span style=""color:#2b91af;"">ClientMessage</span>&nbsp;originalMessage,&nbsp;<span style=""color:#2b91af;"">ResultMessage</span>&nbsp;result)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;intOverloadResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;Overload&nbsp;with&nbsp;return&nbsp;value&nbsp;called&nbsp;=&gt;&nbsp;{0}&quot;</span>,&nbsp;result.ReturnValue.ToString());
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;ReadStateValue()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">base</span>.Call(<span style=""color:#a31515;"">&quot;readStateValue&quot;</span>,&nbsp;(hub,&nbsp;msg,&nbsp;result)&nbsp;=&gt;&nbsp;readStateResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;Read&nbsp;some&nbsp;state!&nbsp;=&gt;&nbsp;{0}&quot;</span>,&nbsp;result.ReturnValue));
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;PlainTask()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">base</span>.Call(<span style=""color:#a31515;"">&quot;plainTask&quot;</span>,&nbsp;(hub,&nbsp;msg,&nbsp;result)&nbsp;=&gt;&nbsp;plainTaskResult&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;Plain&nbsp;Task&nbsp;Result&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;GenericTaskWithContinueWith()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">base</span>.Call(<span style=""color:#a31515;"">&quot;genericTaskWithContinueWith&quot;</span>,&nbsp;(hub,&nbsp;msg,&nbsp;result)&nbsp;=&gt;&nbsp;genericTaskWithContinueWithResult&nbsp;=&nbsp;result.ReturnValue.ToString());
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Server&nbsp;Called&nbsp;Functions

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;FromArbitraryCode(<span style=""color:#2b91af;"">Hub</span>&nbsp;hub,&nbsp;<span style=""color:#2b91af;"">MethodCallMessage</span>&nbsp;methodCall)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;fromArbitraryCodeResult&nbsp;=&nbsp;methodCall.Arguments[0]&nbsp;<span style=""color:blue;"">as</span>&nbsp;<span style=""color:blue;"">string</span>;
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;GroupAdded(<span style=""color:#2b91af;"">Hub</span>&nbsp;hub,&nbsp;<span style=""color:#2b91af;"">MethodCallMessage</span>&nbsp;methodCall)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(!<span style=""color:blue;"">string</span>.IsNullOrEmpty(groupAddedResult))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;groupAddedResult&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;Group&nbsp;Already&nbsp;Added!&quot;</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">else</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;groupAddedResult&nbsp;=&nbsp;<span style=""color:#a31515;"">&quot;Group&nbsp;Added!&quot;</span>;
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;Signal(<span style=""color:#2b91af;"">Hub</span>&nbsp;hub,&nbsp;<span style=""color:#2b91af;"">MethodCallMessage</span>&nbsp;methodCall)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;dynamicTaskResult&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;The&nbsp;dynamic&nbsp;task!&nbsp;{0}&quot;</span>,&nbsp;methodCall.Arguments[0]);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;Invoke(<span style=""color:#2b91af;"">Hub</span>&nbsp;hub,&nbsp;<span style=""color:#2b91af;"">MethodCallMessage</span>&nbsp;methodCall)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;invokeResults.Add(<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;{0}&nbsp;client&nbsp;state&nbsp;index&nbsp;-&gt;&nbsp;{1}&quot;</span>,&nbsp;methodCall.Arguments[0],&nbsp;<span style=""color:blue;"">this</span>.State[<span style=""color:#a31515;"">&quot;index&quot;</span>]));
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Draw

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Display&nbsp;the&nbsp;result&#39;s&nbsp;of&nbsp;the&nbsp;function&nbsp;calls.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;Draw()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Arbitrary&nbsp;Code&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(20);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;Sending&nbsp;{0}&nbsp;from&nbsp;arbitrary&nbsp;code&nbsp;without&nbsp;the&nbsp;hub&nbsp;itself!&quot;</span>,&nbsp;fromArbitraryCodeResult));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(10);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Group&nbsp;Added&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(20);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(groupAddedResult);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(10);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Dynamic&nbsp;Task&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(20);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(dynamicTaskResult);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(10);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Report&nbsp;Progress&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(20);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginVertical();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(longRunningJobStatus);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.HorizontalSlider(longRunningJobProgress,&nbsp;0,&nbsp;100);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndVertical();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(10);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Generic&nbsp;Task&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(20);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(genericTaskResult);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(10);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Task&nbsp;With&nbsp;Exception&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(20);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(taskWithExceptionResult);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(10);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Generic&nbsp;Task&nbsp;With&nbsp;Exception&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(20);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(genericTaskWithExceptionResult);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(10);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Synchronous&nbsp;Exception&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(20);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(synchronousExceptionResult);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(10);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Invoking&nbsp;hub&nbsp;method&nbsp;with&nbsp;dynamic&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(20);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(invokingHubMethodWithDynamicResult);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(10);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Simple&nbsp;Array&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(20);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(simpleArrayResult);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(10);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Complex&nbsp;Type&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(20);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(complexTypeResult);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(10);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Complex&nbsp;Array&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(20);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(complexArrayResult);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(10);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Overloads&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(20);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginVertical();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(voidOverloadResult);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(intOverloadResult);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndVertical();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(10);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Read&nbsp;State&nbsp;Value&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(20);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(readStateResult);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(10);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Plain&nbsp;Task&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(20);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(plainTaskResult);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(10);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Generic&nbsp;Task&nbsp;With&nbsp;ContinueWith&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(20);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(genericTaskWithContinueWithResult);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(10);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Message&nbsp;Pump&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(20);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;invokeResults.Draw(<span style=""color:#2b91af;"">Screen</span>.width&nbsp;-&nbsp;40,&nbsp;270);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(10);
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>
}</pre>";
        #endregion

        #region SignalR AuthenticationSample
        public static string SignalR_AuthenticationSample = @"<pre style=""font-family:Consolas;font-size:13;color:black;background:silver;""><span style=""color:blue;"">using</span>&nbsp;System;
<span style=""color:blue;"">using</span>&nbsp;System.Collections.Generic;

<span style=""color:blue;"">using</span>&nbsp;UnityEngine;
<span style=""color:blue;"">using</span>&nbsp;BestHTTP.SignalR;
<span style=""color:blue;"">using</span>&nbsp;BestHTTP.SignalR.Hubs;
<span style=""color:blue;"">using</span>&nbsp;BestHTTP.SignalR.Messages;
<span style=""color:blue;"">using</span>&nbsp;BestHTTP.SignalR.Authentication;

<span style=""color:blue;"">class</span>&nbsp;<span style=""color:#2b91af;"">AuthenticationSample</span>&nbsp;:&nbsp;<span style=""color:#2b91af;"">MonoBehaviour</span>
{
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">readonly</span>&nbsp;<span style=""color:#2b91af;"">Uri</span>&nbsp;URI&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">Uri</span>(<span style=""color:#a31515;"">&quot;https://besthttpsignalr.azurewebsites.net/signalr&quot;</span>);

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Private&nbsp;Fields

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Reference&nbsp;to&nbsp;the&nbsp;SignalR&nbsp;Connection</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Connection</span>&nbsp;signalRConnection;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;userName&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">string</span>&nbsp;role&nbsp;=&nbsp;<span style=""color:blue;"">string</span>.Empty;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Vector2</span>&nbsp;scrollPos;

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Unity&nbsp;Events

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;Start()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Create&nbsp;the&nbsp;SignalR&nbsp;connection,&nbsp;and&nbsp;pass&nbsp;the&nbsp;hubs&nbsp;that&nbsp;we&nbsp;want&nbsp;to&nbsp;connect&nbsp;to</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">Connection</span>(URI,&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">BaseHub</span>(<span style=""color:#a31515;"">&quot;noauthhub&quot;</span>,&nbsp;<span style=""color:#a31515;"">&quot;Messages&quot;</span>),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">BaseHub</span>(<span style=""color:#a31515;"">&quot;invokeauthhub&quot;</span>,&nbsp;<span style=""color:#a31515;"">&quot;Messages&nbsp;Invoked&nbsp;By&nbsp;Admin&nbsp;or&nbsp;Invoker&quot;</span>),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">BaseHub</span>(<span style=""color:#a31515;"">&quot;authhub&quot;</span>,&nbsp;<span style=""color:#a31515;"">&quot;Messages&nbsp;Requiring&nbsp;Authentication&nbsp;to&nbsp;Send&nbsp;or&nbsp;Receive&quot;</span>),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">BaseHub</span>(<span style=""color:#a31515;"">&quot;inheritauthhub&quot;</span>,&nbsp;<span style=""color:#a31515;"">&quot;Messages&nbsp;Requiring&nbsp;Authentication&nbsp;to&nbsp;Send&nbsp;or&nbsp;Receive&nbsp;Because&nbsp;of&nbsp;Inheritance&quot;</span>),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">BaseHub</span>(<span style=""color:#a31515;"">&quot;incomingauthhub&quot;</span>,&nbsp;<span style=""color:#a31515;"">&quot;Messages&nbsp;Requiring&nbsp;Authentication&nbsp;to&nbsp;Send&quot;</span>),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">BaseHub</span>(<span style=""color:#a31515;"">&quot;adminauthhub&quot;</span>,&nbsp;<span style=""color:#a31515;"">&quot;Messages&nbsp;Requiring&nbsp;Admin&nbsp;Membership&nbsp;to&nbsp;Send&nbsp;or&nbsp;Receive&quot;</span>),&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">BaseHub</span>(<span style=""color:#a31515;"">&quot;userandroleauthhub&quot;</span>,&nbsp;<span style=""color:#a31515;"">&quot;Messages&nbsp;Requiring&nbsp;Name&nbsp;to&nbsp;be&nbsp;\&quot;User\&quot;&nbsp;and&nbsp;Role&nbsp;to&nbsp;be&nbsp;\&quot;Admin\&quot;&nbsp;to&nbsp;Send&nbsp;or&nbsp;Receive&quot;</span>));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Set&nbsp;the&nbsp;authenticator&nbsp;if&nbsp;we&nbsp;have&nbsp;valid&nbsp;fields</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(!<span style=""color:blue;"">string</span>.IsNullOrEmpty(userName)&nbsp;&amp;&amp;&nbsp;!<span style=""color:blue;"">string</span>.IsNullOrEmpty(role))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.AuthenticationProvider&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">HeaderAuthenticator</span>(userName,&nbsp;role);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Set&nbsp;up&nbsp;event&nbsp;handler</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.OnConnected&nbsp;+=&nbsp;signalRConnection_OnConnected;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Start&nbsp;to&nbsp;connect&nbsp;to&nbsp;the&nbsp;server.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.Open();
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnDestroy()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Close&nbsp;the&nbsp;connection&nbsp;when&nbsp;we&nbsp;are&nbsp;closing&nbsp;the&nbsp;sample</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.Close();
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnGUI()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUIHelper</span>.DrawArea(<span style=""color:#2b91af;"">GUIHelper</span>.ClientArea,&nbsp;<span style=""color:blue;"">true</span>,&nbsp;()&nbsp;=&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;scrollPos&nbsp;=&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginScrollView(scrollPos,&nbsp;<span style=""color:blue;"">false</span>,&nbsp;<span style=""color:blue;"">false</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginVertical();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(signalRConnection.AuthenticationProvider&nbsp;==&nbsp;<span style=""color:blue;"">null</span>)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Username&nbsp;(Enter&nbsp;&#39;User&#39;):&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;userName&nbsp;=&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.TextField(userName,&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.MinWidth(100));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Roles&nbsp;(Enter&nbsp;&#39;Invoker&#39;&nbsp;or&nbsp;&#39;Admin&#39;):&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;role&nbsp;=&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.TextField(role,&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.MinWidth(100));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">GUILayout</span>.Button(<span style=""color:#a31515;"">&quot;Log&nbsp;in&quot;</span>))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Restart();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">for</span>&nbsp;(<span style=""color:blue;"">int</span>&nbsp;i&nbsp;=&nbsp;0;&nbsp;i&nbsp;&lt;&nbsp;signalRConnection.Hubs.Length;&nbsp;++i)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;(signalRConnection.Hubs[i]&nbsp;<span style=""color:blue;"">as</span>&nbsp;<span style=""color:#2b91af;"">BaseHub</span>).Draw();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndVertical();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndScrollView();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;});
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Called&nbsp;when&nbsp;we&nbsp;successfully&nbsp;connected&nbsp;to&nbsp;the&nbsp;server.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;signalRConnection_OnConnected(<span style=""color:#2b91af;"">Connection</span>&nbsp;manager)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;call&nbsp;&#39;InvokedFromClient&#39;&nbsp;on&nbsp;all&nbsp;hubs</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">for</span>&nbsp;(<span style=""color:blue;"">int</span>&nbsp;i&nbsp;=&nbsp;0;&nbsp;i&nbsp;&lt;&nbsp;signalRConnection.Hubs.Length;&nbsp;++i)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;(signalRConnection.Hubs[i]&nbsp;<span style=""color:blue;"">as</span>&nbsp;<span style=""color:#2b91af;"">BaseHub</span>).InvokedFromClient();
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Helper&nbsp;function&nbsp;to&nbsp;do&nbsp;a&nbsp;hard-restart&nbsp;to&nbsp;the&nbsp;server.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;Restart()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Clean&nbsp;up</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.OnConnected&nbsp;-=&nbsp;signalRConnection_OnConnected;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Close&nbsp;current&nbsp;connection</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection.Close();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;signalRConnection&nbsp;=&nbsp;<span style=""color:blue;"">null</span>;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;start&nbsp;again,&nbsp;with&nbsp;authentication&nbsp;if&nbsp;we&nbsp;filled&nbsp;in&nbsp;all&nbsp;input&nbsp;fields</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Start();

&nbsp;&nbsp;&nbsp;&nbsp;}
}

<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Hub&nbsp;implementation&nbsp;for&nbsp;the&nbsp;authentication&nbsp;demo.&nbsp;All&nbsp;hubs&nbsp;that&nbsp;we&nbsp;connect&nbsp;to&nbsp;has&nbsp;the&nbsp;same&nbsp;server&nbsp;and&nbsp;client&nbsp;side&nbsp;functions.</span>
<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
<span style=""color:blue;"">class</span>&nbsp;<span style=""color:#2b91af;"">BaseHub</span>&nbsp;:&nbsp;<span style=""color:#2b91af;"">Hub</span>
{
<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Private&nbsp;Fields

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Hub&nbsp;specific&nbsp;title</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">string</span>&nbsp;Title;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:#2b91af;"">GUIMessageList</span>&nbsp;messages&nbsp;=&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">GUIMessageList</span>();

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;BaseHub(<span style=""color:blue;"">string</span>&nbsp;name,&nbsp;<span style=""color:blue;"">string</span>&nbsp;title)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:&nbsp;<span style=""color:blue;"">base</span>(name)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">this</span>.Title&nbsp;=&nbsp;title;
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;Called&nbsp;by&nbsp;the&nbsp;Connection&nbsp;class&nbsp;to&nbsp;be&nbsp;able&nbsp;to&nbsp;set&nbsp;up&nbsp;mappings.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">override</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;Setup()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Map&nbsp;the&nbsp;server-callable&nbsp;method&nbsp;names&nbsp;to&nbsp;the&nbsp;real&nbsp;functions.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;On(<span style=""color:#a31515;"">&quot;joined&quot;</span>,&nbsp;Joined);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;On(<span style=""color:#a31515;"">&quot;rejoined&quot;</span>,&nbsp;Rejoined);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;On(<span style=""color:#a31515;"">&quot;left&quot;</span>,&nbsp;Left);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;On(<span style=""color:#a31515;"">&quot;invoked&quot;</span>,&nbsp;Invoked);
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Server&nbsp;Called&nbsp;Functions

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;Joined(<span style=""color:#2b91af;"">Hub</span>&nbsp;hub,&nbsp;<span style=""color:#2b91af;"">MethodCallMessage</span>&nbsp;methodCall)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Dictionary</span>&lt;<span style=""color:blue;"">string</span>,&nbsp;<span style=""color:blue;"">object</span>&gt;&nbsp;AuthInfo&nbsp;=&nbsp;methodCall.Arguments[2]&nbsp;<span style=""color:blue;"">as</span>&nbsp;<span style=""color:#2b91af;"">Dictionary</span>&lt;<span style=""color:blue;"">string</span>,&nbsp;<span style=""color:blue;"">object</span>&gt;;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;messages.Add(<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;{0}&nbsp;joined&nbsp;at&nbsp;{1}\n\tIsAuthenticated:&nbsp;{2}&nbsp;IsAdmin:&nbsp;{3}&nbsp;UserName:&nbsp;{4}&quot;</span>,&nbsp;methodCall.Arguments[0],&nbsp;methodCall.Arguments[1],&nbsp;AuthInfo[<span style=""color:#a31515;"">&quot;IsAuthenticated&quot;</span>],&nbsp;AuthInfo[<span style=""color:#a31515;"">&quot;IsAdmin&quot;</span>],&nbsp;AuthInfo[<span style=""color:#a31515;"">&quot;UserName&quot;</span>]));
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;Rejoined(<span style=""color:#2b91af;"">Hub</span>&nbsp;hub,&nbsp;<span style=""color:#2b91af;"">MethodCallMessage</span>&nbsp;methodCall)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;messages.Add(<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;{0}&nbsp;reconnected&nbsp;at&nbsp;{1}&quot;</span>,&nbsp;methodCall.Arguments[0],&nbsp;methodCall.Arguments[1]));
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;Left(<span style=""color:#2b91af;"">Hub</span>&nbsp;hub,&nbsp;<span style=""color:#2b91af;"">MethodCallMessage</span>&nbsp;methodCall)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;messages.Add(<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;{0}&nbsp;left&nbsp;at&nbsp;{1}&quot;</span>,&nbsp;methodCall.Arguments[0],&nbsp;methodCall.Arguments[1]));
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;Invoked(<span style=""color:#2b91af;"">Hub</span>&nbsp;hub,&nbsp;<span style=""color:#2b91af;"">MethodCallMessage</span>&nbsp;methodCall)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;messages.Add(<span style=""color:blue;"">string</span>.Format(<span style=""color:#a31515;"">&quot;{0}&nbsp;invoked&nbsp;hub&nbsp;method&nbsp;at&nbsp;{1}&quot;</span>,&nbsp;methodCall.Arguments[0],&nbsp;methodCall.Arguments[1]));
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Client&nbsp;callable&nbsp;function&nbsp;implementation

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;InvokedFromClient()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">base</span>.Call(<span style=""color:#a31515;"">&quot;invokedFromClient&quot;</span>,&nbsp;OnInvoked,&nbsp;OnInvokeFailed);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnInvoked(<span style=""color:#2b91af;"">Hub</span>&nbsp;hub,&nbsp;<span style=""color:#2b91af;"">ClientMessage</span>&nbsp;originalMessage,&nbsp;<span style=""color:#2b91af;"">ResultMessage</span>&nbsp;result)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Debug</span>.Log(hub.Name&nbsp;+&nbsp;<span style=""color:#a31515;"">&quot;&nbsp;invokedFromClient&nbsp;success!&quot;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;This&nbsp;callback&nbsp;function&nbsp;will&nbsp;be&nbsp;called&nbsp;every&nbsp;time&nbsp;we&nbsp;try&nbsp;to&nbsp;access&nbsp;a&nbsp;protected&nbsp;API&nbsp;while&nbsp;we&nbsp;are&nbsp;using&nbsp;an&nbsp;non-authenticated&nbsp;connection.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">private</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnInvokeFailed(<span style=""color:#2b91af;"">Hub</span>&nbsp;hub,&nbsp;<span style=""color:#2b91af;"">ClientMessage</span>&nbsp;originalMessage,&nbsp;<span style=""color:#2b91af;"">ResultMessage</span>&nbsp;result)
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">Debug</span>.LogWarning(hub.Name&nbsp;+&nbsp;<span style=""color:#a31515;"">&quot;&nbsp;&quot;</span>&nbsp;+&nbsp;result.ErrorMessage);
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">void</span>&nbsp;Draw()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:blue;"">this</span>.Title);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(20);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;messages.Draw(<span style=""color:#2b91af;"">Screen</span>.width&nbsp;-&nbsp;20,&nbsp;100);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;}
}</pre>";
        #endregion

        #region CacheMaintenanceSample
        public static string CacheMaintenanceSample = @"<pre style=""font-family:Consolas;font-size:13;color:black;background:silver;""><span style=""color:blue;"">using</span>&nbsp;System;
<span style=""color:blue;"">using</span>&nbsp;System.Collections.Generic;

<span style=""color:blue;"">using</span>&nbsp;UnityEngine;
<span style=""color:blue;"">using</span>&nbsp;BestHTTP;
<span style=""color:blue;"">using</span>&nbsp;BestHTTP.Caching;

<span style=""color:blue;"">public</span>&nbsp;<span style=""color:blue;"">sealed</span>&nbsp;<span style=""color:blue;"">class</span>&nbsp;<span style=""color:#2b91af;"">CacheMaintenanceSample</span>&nbsp;:&nbsp;<span style=""color:#2b91af;"">MonoBehaviour</span>
{
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;An&nbsp;enum&nbsp;for&nbsp;better&nbsp;readability</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">enum</span>&nbsp;<span style=""color:#2b91af;"">DeleteOlderTypes</span>
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Days,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Hours,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Mins,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Secs
&nbsp;&nbsp;&nbsp;&nbsp;};

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Private&nbsp;Fields

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;What&nbsp;methode&nbsp;to&nbsp;call&nbsp;on&nbsp;the&nbsp;TimeSpan</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">DeleteOlderTypes</span>&nbsp;deleteOlderType&nbsp;=&nbsp;<span style=""color:#2b91af;"">DeleteOlderTypes</span>.Secs;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;The&nbsp;value&nbsp;for&nbsp;the&nbsp;TimeSpan.</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">int</span>&nbsp;value&nbsp;=&nbsp;10;

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;What&#39;s&nbsp;our&nbsp;maximum&nbsp;cache&nbsp;size</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:gray;"">///</span><span style=""color:green;"">&nbsp;</span><span style=""color:gray;"">&lt;/summary&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">int</span>&nbsp;maxCacheSize&nbsp;=&nbsp;5&nbsp;*&nbsp;1024&nbsp;*&nbsp;1024;

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#region</span>&nbsp;Unity&nbsp;Events

&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">void</span>&nbsp;OnGUI()
&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUIHelper</span>.DrawArea(<span style=""color:#2b91af;"">GUIHelper</span>.ClientArea,&nbsp;<span style=""color:blue;"">true</span>,&nbsp;()&nbsp;=&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Delete&nbsp;cached&nbsp;entities&nbsp;older&nbsp;then&quot;</span>);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(value.ToString(),&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.MinWidth(50));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;value&nbsp;=&nbsp;(<span style=""color:blue;"">int</span>)<span style=""color:#2b91af;"">GUILayout</span>.HorizontalSlider(value,&nbsp;1,&nbsp;60,&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.MinWidth(100));

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(10);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;deleteOlderType&nbsp;=&nbsp;(<span style=""color:#2b91af;"">DeleteOlderTypes</span>)(<span style=""color:blue;"">int</span>)<span style=""color:#2b91af;"">GUILayout</span>.SelectionGrid((<span style=""color:blue;"">int</span>)deleteOlderType,&nbsp;<span style=""color:blue;"">new</span>&nbsp;<span style=""color:blue;"">string</span>[]&nbsp;{&nbsp;<span style=""color:#a31515;"">&quot;Days&quot;</span>,&nbsp;<span style=""color:#a31515;"">&quot;Hours&quot;</span>,&nbsp;<span style=""color:#a31515;"">&quot;Mins&quot;</span>,&nbsp;<span style=""color:#a31515;"">&quot;Secs&quot;</span>&nbsp;},&nbsp;4);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.FlexibleSpace();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(10);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.BeginHorizontal();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(<span style=""color:#a31515;"">&quot;Max&nbsp;Cache&nbsp;Size&nbsp;(bytes):&nbsp;&quot;</span>,&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Width(150));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Label(maxCacheSize.ToString(<span style=""color:#a31515;"">&quot;N0&quot;</span>),&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Width(70));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;maxCacheSize&nbsp;=&nbsp;(<span style=""color:blue;"">int</span>)<span style=""color:#2b91af;"">GUILayout</span>.HorizontalSlider(maxCacheSize,&nbsp;1024,&nbsp;10&nbsp;*&nbsp;1024&nbsp;*&nbsp;1024);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.EndHorizontal();

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">GUILayout</span>.Space(10);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">if</span>&nbsp;(<span style=""color:#2b91af;"">GUILayout</span>.Button(<span style=""color:#a31515;"">&quot;Maintenance&quot;</span>))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">TimeSpan</span>&nbsp;deleteOlder&nbsp;=&nbsp;<span style=""color:#2b91af;"">TimeSpan</span>.FromDays(14);

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">switch</span>&nbsp;(deleteOlderType)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#2b91af;"">DeleteOlderTypes</span>.Days:&nbsp;deleteOlder&nbsp;=&nbsp;<span style=""color:#2b91af;"">TimeSpan</span>.FromDays(value);&nbsp;<span style=""color:blue;"">break</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#2b91af;"">DeleteOlderTypes</span>.Hours:&nbsp;deleteOlder&nbsp;=&nbsp;<span style=""color:#2b91af;"">TimeSpan</span>.FromHours(value);&nbsp;<span style=""color:blue;"">break</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#2b91af;"">DeleteOlderTypes</span>.Mins:&nbsp;deleteOlder&nbsp;=&nbsp;<span style=""color:#2b91af;"">TimeSpan</span>.FromMinutes(value);&nbsp;<span style=""color:blue;"">break</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:blue;"">case</span>&nbsp;<span style=""color:#2b91af;"">DeleteOlderTypes</span>.Secs:&nbsp;deleteOlder&nbsp;=&nbsp;<span style=""color:#2b91af;"">TimeSpan</span>.FromSeconds(value);&nbsp;<span style=""color:blue;"">break</span>;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:green;"">//&nbsp;Call&nbsp;the&nbsp;BeginMaintainence&nbsp;function.&nbsp;It&nbsp;will&nbsp;run&nbsp;on&nbsp;a&nbsp;thread&nbsp;to&nbsp;do&nbsp;not&nbsp;block&nbsp;the&nbsp;main&nbsp;thread.</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=""color:#2b91af;"">HTTPCacheService</span>.BeginMaintainence(<span style=""color:blue;"">new</span>&nbsp;<span style=""color:#2b91af;"">HTTPCacheMaintananceParams</span>(deleteOlder,&nbsp;(<span style=""color:blue;"">ulong</span>)maxCacheSize));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;});
&nbsp;&nbsp;&nbsp;&nbsp;}

<span style=""color:blue;"">&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>
}</pre>";
        #endregion
    }
}
