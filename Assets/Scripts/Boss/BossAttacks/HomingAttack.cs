using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FadingEffect))]
public class HomingAttack : BossAttack
{
    public GameObject attackPanel; // panel for moving

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "ShipPanel" && !isDying)
        {
            // check for boss dying(if he's already killed)
            if (BossManager.instance.currentBoss.IsDying) return;

            // toggle dying
            isDying = true;

            // pause iTween animation
            iTween.Pause(attackPanel);

            // play dying sound
            SoundManager.instance.RandomizeSfx(dyingSounds);

            // hit the ship
            ScoreManager.instance.GetDamage();

            // play dying animation
            component.animation.Play("Dying", 1);

            // stop spawning fading effect
            GetComponent<FadingEffect>().ToggleActivation();

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
        }
    }
    public void AimAndShoot(Vector3 pos, float timeScale)
    {
        iTween.ScaleFrom(attackPanel, iTween.Hash(
            "scale", new Vector3(.8f, .8f),
            "speed", 1.5f * timeScale,
            "easetype", iTween.EaseType.easeOutQuad
        ));

        iTween.MoveTo(attackPanel, iTween.Hash(
            "x", pos.x,
            "y", pos.y,
            // ебучая хуйня с этим Z, он меня уже заебал, я хз откуда эта хуйня берется, пока просто буду фиксить вручную
            "speed", 1.5f * timeScale,
            "easetype", iTween.EaseType.easeOutQuad,
            "oncomplete", "CommitASuicide",
            "oncompletetarget", this.gameObject
        ));
    }
}
