using UnityEngine;

namespace Bonuses
{
    public static class BonusFactory
    {
        public static System.Type getBonusScript()
        {
            return Random.Range(1, 6) switch
            {
                1 => typeof(BonusSlow),
                2 => typeof(BonusFast),
                3 => typeof(BonusAddBallToStash),
                4 => typeof(BonusAdd2Balls),
                5 => typeof(BonusAdd10Balls),
                _ => typeof(BonusBaseScript)
            };
        }
    }
}