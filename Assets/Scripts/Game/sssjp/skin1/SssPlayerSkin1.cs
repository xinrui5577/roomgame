namespace Assets.Scripts.Game.sssjp.skin1
{
    public class SssPlayerSkin1 : SssPlayer
    {
        protected override string GetLineTypeSriteName(int line, CardType cardType)
        {
            string typeName;
            if (line == 0 && cardType == CardType.santiao)
            {
                typeName = "chongsan";
            }
            else
            {
                typeName = cardType.ToString();
            }
            return typeName;
        }

    }
}
