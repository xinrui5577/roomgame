using System;
using System.Linq;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class QdjtTypeBinderManager : TypeBinderManager
    {
        public override List<Type> CollectionGameLogicType()
        {
            base.CollectionGameLogicType();

            Binder<CommandHu, QdjtActionHu>();
            Binder<CommandTing, QdjtActionTing>();
            Binder<CommandReconnect, QdjtActionReconnect>(); 
            return mSuppliers.Keys.ToList();
        }

        public override Type PlayerHandExtension()
        {
            return typeof(QdjtMahjongPlayerHand);
        }
    }
}