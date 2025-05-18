using System.Collections.Generic;
using System.Linq;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class QzmjTypeBinderManager : TypeBinderManager
    {
        public override List<Type> CollectionGameLogicType()
        {
            base.CollectionGameLogicType();

            Binder<CommandOperate, QzmjActionOperate>();
            Binder<CommandHu, QzmjActionHu>();
            return mSuppliers.Keys.ToList();
        }
    }
}
