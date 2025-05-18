namespace Assets.Scripts.Game.pdk.DdzEventArgs
{
    /// <summary>
    /// 服务器给的消息类型
    /// </summary>
    public enum ServDataType
    {
        None,
        OnGetGameInfo,
        OnGetRejoinData,
        OnServerResponse,
        OnUserOut,
        OnUserReady,
        OnUserJoinRoom,
    }
}