using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedAttack : BossAttack
{
    public Animator animator; // animator 

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "ShipPanel" && !isDying)
        {
            // check for boss dying(if he's already killed)
            if (BossManager.instance.currentBoss.IsDying) return;

            // toggle dying
            isDying = true;

            // bind current position
            animator.SetFloat("Speed", 0);

            // play dying sound
            SoundManager.instance.RandomizeSfx(dyingSounds);

            // hit the ship
            ScoreManager.instance.GetDamage();

            // play dying animation(broken bone)
            component.animation.Play("Dying", 1);

            // stop adding particles if exists
            if (ps != null)
            {
                ParticleSystem.EmissionModule emission = ps.emission;
                ParticleSystem.MainModule main = ps.main;
                emission.rateOverTime = 0;
                main.useUnscaledTime = true;
            }

            // play ship hitting animation
            // ...
        } // if
    }
}
