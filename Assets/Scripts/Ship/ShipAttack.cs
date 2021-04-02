using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

public class ShipAttack : MonoBehaviour
{
    public UnityArmatureComponent component; // db animator 
    public GameObject[] bombSmoke; // bomb smoke particles

    public AudioClip shootSound; // shoot sound

    private int bombSmokeId; // bomb smoke animation utils
    private Vector2 startPos; // bomb spawn position

    public void Start()
    {
        startPos = transform.position;
        component.AddEventListener(EventObject.FRAME_EVENT, OnFrameEventListener);
    }
    public void Update()
    {
        // boss husky animation settings
        if(component.animation.lastAnimationName == "Dying" && BossManager.instance.currentBoss.GetType() == typeof(BossHusky))
            if(!(BossManager.instance.currentBoss as BossHusky).IsIdle)
                GetComponent<Canvas>().sortingOrder = 1;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // demo
        // в дальнейшем нужно будет как-то адаптировать под всех боссов
        // пока идем только от одного босса(мопса)
        // (НО ЭТО НЕ ТОЧНО)
        string _name = collision.gameObject.name;
        if (_name.StartsWith("Boss"))
        {
            // check for husky boss's being able to be attacked
            if (_name == "BossHusky")
            {
                BossHusky _bossHusky = BossManager.instance.currentBoss as BossHusky;
                if (!_bossHusky.IsIdle) return;
            }

            // get damage
            BossManager.instance.currentBoss.GetDamage(20f);

            // set boss panel as parent transform of bomb(bullet)
            transform.SetParent(collision.transform);

            // play dying animation
            PlayDyingAnimation();
        }
    }
    // shoot manually animation
    public void Shoot(Vector3 position)
    {
        // play shooting sound
        SoundManager.instance.PlaySingle(shootSound);

        // понять почему нерпавильно работает анимация
        // move bomb to aimed position
        iTween.MoveTo(gameObject, iTween.Hash(
            "x", position.x,
            "y", position.y,
            "speed", 7.5f,
            "easetype", iTween.EaseType.easeOutCubic,
            "onupdate", "UpdateBombDyingAnimation",
            "onupdateparams", position
        ));

        // scale bomb like boss if far away
        iTween.ScaleTo(gameObject, iTween.Hash(
            "x", .5f,
            "y", .5f,
            "speed", .25f,
            "easetype", iTween.EaseType.easeOutCubic
        ));
    }

    // dragon bones event listener
    private void OnFrameEventListener(string type, EventObject eventObject)
    {
        if (eventObject.name == "DyingEnd")
            Destroy(gameObject);
    }
    // dying animation
    public void PlayDyingAnimation()
    {
        iTween.Pause(gameObject);
        component.animation.Play("Dying", 1);
    }
    // checking if bomb approximately got to needed position
    private void UpdateBombDyingAnimation(Vector3 pos)
    {
        if (Mathf.Abs(pos.x - transform.position.x) <= .3f && Mathf.Abs(pos.y - transform.position.y) <= .3f)
            PlayDyingAnimation();

        // full distance(from the beginning to the end)
        float distance = Mathf.Sqrt(Mathf.Pow(pos.x - startPos.x, 2) + Mathf.Pow(pos.y - startPos.y, 2));

        // passed distance(from the beginning to the current position)
        float passedDistance =
            Mathf.Sqrt(Mathf.Pow(transform.position.x - startPos.x, 2) +
            Mathf.Pow(transform.position.y - startPos.y, 2));

        // spawn bomb smoke every N passed distance
        if (passedDistance >= 2f && bombSmokeId == 0)
        {
            // show bomb smoke
            bombSmoke[0].SetActive(true);

            // set bomb smoke parent to main wrap
            bombSmoke[0].transform.SetParent(GameObject.Find("BackUI").transform);

            // increase bomb smoke id
            bombSmokeId++;
        }
        else if (passedDistance >= 3.5f && bombSmokeId == 1)
        {
            // show bomb smoke
            bombSmoke[1].SetActive(true);

            // set bomb smoke parent to main wrap
            bombSmoke[1].transform.SetParent(GameObject.Find("BackUI").transform);

            // increase bomb smoke id
            bombSmokeId++;
        }
        else if (passedDistance >= 5.25f && bombSmokeId == 2)
        {
            // show bomb smoke
            bombSmoke[2].SetActive(true);

            // set bomb smoke parent to main wrap
            bombSmoke[2].transform.SetParent(GameObject.Find("BackUI").transform);

            // increase bomb smoke id
            bombSmokeId++;
        }
        else if (passedDistance >= 7f && bombSmokeId == 3)
        {
            // show bomb smoke
            bombSmoke[3].SetActive(true);

            // set bomb smoke parent to main wrap
            bombSmoke[3].transform.SetParent(GameObject.Find("BackUI").transform);

            // increase bomb smoke id
            bombSmokeId++;
        }

        // bomb is alive for 8.75f world points
        if(passedDistance >= 8.75f)
            PlayDyingAnimation();
    }
}