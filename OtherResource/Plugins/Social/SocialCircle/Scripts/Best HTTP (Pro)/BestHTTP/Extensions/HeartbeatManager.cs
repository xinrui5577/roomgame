/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BestHTTP.Extensions
{
    public interface IHeartbeat
    {
        void OnHeartbeatUpdate(TimeSpan dif);
    }

    /// <summary>
    /// A manager class that can handle subscribing and unsubscribeing in the same update.
    /// </summary>
    public sealed class HeartbeatManager
    {
        private List<IHeartbeat> Heartbeats = new List<IHeartbeat>();
        private IHeartbeat[] UpdateArray;
        private DateTime LastUpdate = DateTime.MinValue;

        public void Subscribe(IHeartbeat heartbeat)
        {
            lock (Heartbeats)
            {
                if (!Heartbeats.Contains(heartbeat))
                    Heartbeats.Add(heartbeat);
            }
        }

        public void Unsubscribe(IHeartbeat heartbeat)
        {
            lock (Heartbeats)
                Heartbeats.Remove(heartbeat);
        }

        public void Update()
        {
            if (LastUpdate == DateTime.MinValue)
                LastUpdate = DateTime.UtcNow;
            else
            {
                TimeSpan dif = DateTime.UtcNow - LastUpdate;
                LastUpdate = DateTime.UtcNow;
                
                int count = 0;

                lock (Heartbeats)
                {
                    if (UpdateArray == null || UpdateArray.Length < Heartbeats.Count)
                        Array.Resize(ref UpdateArray, Heartbeats.Count);

                    Heartbeats.CopyTo(0, UpdateArray, 0, Heartbeats.Count);

                    count = Heartbeats.Count;
                }

                for (int i = 0; i < count; ++i)
                {
                    try
                    {
                        UpdateArray[i].OnHeartbeatUpdate(dif);
                    }
                    catch
                    { }
                }
            }
        }


    }
}
