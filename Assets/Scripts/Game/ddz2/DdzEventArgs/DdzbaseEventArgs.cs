using System;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.ddz2.DdzEventArgs
{
    public class DdzbaseEventArgs : EventArgs
    {
        public readonly ISFSObject IsfObjData;

        public DdzbaseEventArgs(ISFSObject isfData)
        {
            IsfObjData = isfData;
        }
    }
}
