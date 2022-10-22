using UnityEngine;

namespace Bonuses
{
    public class BonusAdd2Balls : BonusBaseScript
    {
        protected override void initializeFields()
        {
            this.color = Color.blue;
            this.textColor = Color.white;
            this.text = "+2";
        }

        protected override void BonusActivate()
        {
            _playerScript.addBallsToGame(2);
        }
    }
}