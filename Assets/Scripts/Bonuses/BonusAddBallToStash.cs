using UnityEngine;

namespace Bonuses
{
    public class BonusAddBallToStash : BonusBaseScript
    {
        protected override void initializeFields()
        {
            this.color = new Color(0.215f, 0.588f ,0);
            this.textColor = Color.white;
            this.text = "Ball";
        }

        protected override void BonusActivate()
        {
            _playerScript.addBallsToStash(1);
        }
    }
}