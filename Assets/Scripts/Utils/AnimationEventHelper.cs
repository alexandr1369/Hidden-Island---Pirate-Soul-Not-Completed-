using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DragonBones;

public class AnimationEventHelper : MonoBehaviour
{
    public GameObject panel; // panel for (toggling/destroying)

    public UnityArmatureComponent component; // dragon bones component(for db animations)
    public Animator animator; // animator for animations

    public Text text;
    public Text textTint;

    public void PlaySingleSound(string name)
    {
        AudioClip clip = Resources.Load<AudioClip>("Sounds/" + name);
        SoundManager.instance.PlaySingle(clip);
    }
    public void PlayDbAnimation(string name)
    {
        if (component == null) return;
        component.armature.animation.Play(name, 1);
    }
    public void PlayDbFadeInAnimation(string name)
    {
        if (component == null) return;
        component.armature.animation.FadeIn(name, .2f, 1);
    }
    public void PlayDbFadeInLoopAnimation(string name)
    {
        if (component == null) return;
        component.armature.animation.FadeIn(name, .2f);
    }
    public void PlayDbAnimationRandom(string names)
    {
        string[] _names = names.Split(' ');
        int length = _names.Length;

        component.animation.Play(_names[Random.Range(0, length)], 1);
    }
    public void PlayAnimation(string name)
    {
        if (animator == null) return;
        animator.Play(name, 0);
    }
    public void LoadGameScene(GameSceneType gameSceneType)
    {
        GameManager.instance.LoadGameScene(gameSceneType);
    }
    public void LoadAsyncGameScene(GameSceneType gameSceneType)
    {
        LoadingManager.instance.StartLoadingScene(gameSceneType);
    }
    public void AllowAsyncSceneActivation()
    {
        LoadingManager.instance.AllowSceneActivation();
    }
    public void SetText(string text)
    {
        if (this.text == null && this.textTint != null) return;
        this.text.text = this.textTint.text = text;
    }
    public void SetActive(int state)
    {
        if (panel == null) return;
        panel.SetActive(state == 0? false : true);
    }
    public void ToggleGameState(int state)
    {
        if (state == 0)
            GameManager.instance.PauseRound();
        else
            GameManager.instance.ContinueRound();
    }
    public void CommitASuicide()
    {
        Destroy(panel);
    }
    public void ToggleShipMovement()
    {
        ShipController.instance.isMovementAllowed = ShipController.instance.isMovementAllowed ? false : true;
    }
}
