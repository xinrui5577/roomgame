using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.brnn3d
{
    public class LuziInfoUIMgr : MonoBehaviour
    {
        public LuziInfoUI TheLuziInfoUI;
       
        private bool[] luzi_info;
        private int luziSize = -1;

        private int _tmpLuzi = -1;

        public LuziInfoUIMgr(bool[] luziInfo)
        {
            luzi_info = luziInfo;
        }

        public void SetLuziInfoUIData()
        {
            _tmpLuzi = luziSize;
            SetLuziInfoUIDataEx();
        }
        //index > 0 往右 反之往左
        public void SetLuziInfoUIData(int index)
        {
            if (luzi_info == null)
            {
                return;
            }
            if (index > 0)
            {
                if (_tmpLuzi < luziSize)
                    _tmpLuzi++;
            }
            else
            {
                if (_tmpLuzi > 10)
                    _tmpLuzi--;
            }
            SetLuziInfoUIDataEx();
        }
        void SetLuziInfoUIDataEx()
        {
            if (_tmpLuzi < 0 || _tmpLuzi > 71)
            {
                return;
            }
            var gameMgr = App.GetGameManager<Brnn3DGameManager>();
            gameMgr.ThePaiMode.History();
        }


        public void SetLuziInfoUIDataEy(int id, int index, bool data)
        {
            switch (id)
            {
                case 0:
                    {
                        TheLuziInfoUI.SetEastImg(index, data);
                    }
                    break;
                case 1:
                    {
                        TheLuziInfoUI.SetSouthImg(index, data);
                    }
                    break;
                case 2:
                    {
                        TheLuziInfoUI.SetWestImg(index, data);
                    }
                    break;
                case 3:
                    {
                        TheLuziInfoUI.SetNorthImg(index, data);
                    }
                    break;
            }
        }

    }
}

