using UnityEngine;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.biji.EventII
{
    public class EventObject : MonoBehaviour
    {
        public delegate void RecevieDelegate(EventData data);

        public EventDelegate RdDelegate;
        void Start()
        {
            Facade.EventCenter.AddEventListener<string, EventData>(gameObject.name, OnReceive);
        }


        public virtual void OnReceive(EventData data)
        {
#if Debug_Event
            string dataStr = "";
            if (data.Data != null)
            {
                var o = data.Data as ISFSObject;
                if (o != null)
                {
                    dataStr = ((ISFSObject)data.Data).ToJson();
                }
                else
                {
                    dataStr += data;
                }
            }
            Debug.Log("[" + name + "]-recive" + "-[" + data.Name + "]-[" + dataStr + "]");
#endif
            if (RdDelegate != null&&RdDelegate.isEnabled)
            {
                RdDelegate.parameters[0] = new EventDelegate.Parameter(data);
                RdDelegate.Execute();
            }
        }

        public void SendEvent(string objName, string eName, object data)
        {
            EventData edata = new EventData
            {
                Name = eName,
                Data = data
            };
#if Debug_Event
            string dataStr = "";
            if (data != null)
            {
                var o = data as ISFSObject;
                if (o != null)
                {
                    dataStr = o.ToJson();
                }
                else
                {
                    dataStr += data;
                }
            }
            Debug.Log("[" +name + "]-send to[" + objName + "]-[" + eName + "]-[" + dataStr +"]");
#endif
            Facade.EventCenter.DispatchEvent(objName, edata);
        }

        void OnDestroy()
        {
            RdDelegate.Clear();
            Facade.EventCenter.RemoveEventListener(gameObject.name);
        }

    }


    public class EventData
    {
        public object Data;
        public string Name;
    }
}
