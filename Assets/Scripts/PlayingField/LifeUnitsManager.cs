using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeUnitsManager : MonoBehaviour
{
    #region Singleton
    public static LifeUnitsManager instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    public List<GameObject> lifeUnitPrefabs; // live unit prefab

    private List<LifeUnit> lifeUnits; // list of all live units for controlling

    public void Start()
    {
        // Remove all demostrate live units
        RemoveAll();

        // create live units list
        lifeUnits = new List<LifeUnit>();
    }
    // show all live units with appearing animations
    public void ShowLifeUnits()
    {
        StartCoroutine(AnimatedApeearing());
    }
    public IEnumerator AnimatedApeearing()
    {
        for(int i = 0; i < ScoreManager.instance.lifeUnits; i++)
        {
            AddLifeUnit();
            yield return new WaitForSeconds(.2f);
        } // for i
    }
    public void AddLifeUnit()
    {
        // add live unit
        GameObject _lifeUnit = Instantiate(lifeUnitPrefabs.Find(t => t.name == "LifeUnit(" + PlayerManager.instance.currentMap.name + ")"), transform);
        lifeUnits.Add(_lifeUnit.GetComponent<LifeUnit>());

        // begin particle system effect without spreading
        _lifeUnit.GetComponentInChildren<ParticleSystem>().Simulate(2f);
        _lifeUnit.GetComponentInChildren<ParticleSystem>().Play();
    }
    public void RemoveLifeUnit()
    {
        // get last live unity
        LifeUnit lastLifeUnit = lifeUnits[lifeUnits.Count - 1];

        // play destroing(dying) animation
        lastLifeUnit.animator.SetTrigger("Dying");

        // remove live unity from active list
        lifeUnits.Remove(lastLifeUnit);
    }
    // init
    private void RemoveAll()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("LifeUnit"))
            Destroy(obj);
    }
}
