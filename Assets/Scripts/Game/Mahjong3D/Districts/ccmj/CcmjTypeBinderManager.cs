using System.Collections.Generic;
using System.Linq;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class CcmjTypeBinderManager : TypeBinderManager
    {
        public override List<Type> CollectionGameLogicType()
        {
            base.CollectionGameLogicType();

            Binder<CommandCpg, CcmjActionCpg>();           
            Binder<MahjongGameData, CcmjMahjongUserInfo>();
            Binder<CommandReconnect, CcmjActionReconnect>();
            return mSuppliers.Keys.ToList();
        }

        public override Type PlayerHandExtension()
        {
            return typeof(CcmjMahjongPlayerHand);
        }
    }
}