using System.Collections.Generic;
namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool
{
    public class EventData
    {
        public object data1;
        public object data2;
        public object data3;
        public object data4;
        public object data5;
        public object data6;

        public EventData()
        {
            this.data1 = null;
            this.data2 = null;
            this.data3 = null;
            this.data4 = null;
        }

        public EventData(object data1)
        {
            this.data1 = data1;
            this.data2 = null;
            this.data3 = null;
            this.data4 = null;
        }

        public EventData(object data1, object data2)
        {
            this.data1 = data1;
            this.data2 = data2;
            this.data3 = null;
            this.data4 = null;
        }

        public EventData(object data1, object data2, object data3)
        {
            this.data1 = data1;
            this.data2 = data2;
            this.data3 = data3;
            this.data4 = null;
        }

        public EventData(object data1, object data2, object data3, object data4)
        {
            this.data1 = data1;
            this.data2 = data2;
            this.data3 = data3;
            this.data4 = data4;
        }

        public EventData(object data1, object data2, object data3, object data4, object data5)
        {
            this.data1 = data1;
            this.data2 = data2;
            this.data3 = data3;
            this.data4 = data4;
            this.data5 = data5;
        }

        public EventData(object data1, object data2, object data3, object data4, object data5, object data6)
        {
            this.data1 = data1;
            this.data2 = data2;
            this.data3 = data3;
            this.data4 = data4;
            this.data5 = data5;
            this.data6 = data6;
        }
    }

    public class EventDispatch
    {
        private static EventDispatch _instance;
        public static EventDispatch Instance
        {
            get
            {
                if(_instance==null)
                    _instance = new EventDispatch();

                return _instance;
            }
        }

        public delegate void EventDelegate(int eventId,EventData data);

        protected class EventCall
        {
            private event EventDelegate CallBack;

            public void AddEventCall(EventDelegate call)
            {
                CallBack += call;
            }

            public void RemoveEventCall(EventDelegate call)
            {
                CallBack -= call;
            }

            public void ExecuteCall(int eventId,EventData data)
            {
                if (CallBack != null) CallBack(eventId,data);
            }

            public EventCall(EventDelegate call)
            {
                CallBack = null;
                CallBack += call;
            }
        }

        protected Dictionary<int, EventCall> EventDic = new Dictionary<int, EventCall>();

        public void RegisteEvent(int id, EventDelegate eventDelegate)
        {
            if (EventDic.ContainsKey(id))
            {
                EventDic[id].AddEventCall(eventDelegate);
            }
            else
            {
                EventCall call = new EventCall(eventDelegate);
                EventDic.Add(id, call);
            }
        }

        public void RemoveEvent(int id, EventDelegate eventDelegate)
        {
            if (EventDic.ContainsKey(id))
            {
                EventDic[id].RemoveEventCall(eventDelegate);
            }
        }

        public static void Dispatch(int id, EventData data)
        {
            if (Instance.EventDic.ContainsKey(id))
            {
                Instance.EventDic[id].ExecuteCall(id,data);
            }
        }

        public static void Dispatch(int id)
        {
            Dispatch(id, null);
        }

        public void CleareEventById(int id)
        {
            if (EventDic.ContainsKey(id))
            {
                EventDic.Remove(id);
            }
        }

        public void OnDestroy()
        {
            EventDic.Clear();
            _instance = null;
        }
    }
}