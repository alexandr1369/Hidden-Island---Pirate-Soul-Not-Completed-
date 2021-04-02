using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreManager : MonoBehaviour
{
    #region Singleton
    public static CoreManager instance;
    void Awake()
    {
        instance = this;
    }
    #endregion

    // current cores's speed
    public float animVelocity; // current animation speed

    public float timeForChance; // how much time need to wait for chance to get effect trigger(buff/dbuff) spawned

    public List<Core> allCores; // all current active cores on a map

    private float leftTimeForChance; // how much time left for chance to get boost effect trigger(bottle) spawned
    private float spawnVelocity; // speed of mobs's spawning

    private float acceleration; // acceleration of cores during whole stage

    private List<float> yPoints; // Y pos for mobs

    // maps extra utils
    private List<List<char>> labyrinth; //` layrinth for 4th stage(demo)

    void Start()
    {
        Init();
    }
    private void Update()
    {
        // decrease the time between chancing to spawn boost effect trigger
        if(leftTimeForChance >= 0)
            leftTimeForChance -= Time.deltaTime;
    }
    #region Utils
    // init settings
    private void Init()
    {
        // set Y positions
        Vector2 worldRes = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

        // set time for chance
        timeForChance = timeForChance <= 0 ? 20 : timeForChance;

        // set time to wait until it's available to spawn boost effect trigger
        leftTimeForChance = timeForChance;

        // create cores's list
        allCores = new List<Core>();

        // get acceleration
        StageInfo _stageInfo = PlayerManager.instance.currentMapStage;
        acceleration = (_stageInfo.maxVelocity - _stageInfo.minVelocity) / _stageInfo.prefabAmount;
        yPoints = new List<float>() { worldRes.y / 1.65f, 0f, -worldRes.y / 1.65f };

        // set start animation velocity
        SetVelocity(PlayerManager.instance.currentMapStage.minVelocity);

        // create labyrinth
        if (PlayerManager.instance.currentMap.name == "Sunset Fog" && PlayerManager.instance.currentMapStage.stageIndex == 4)
            labyrinth = GetLabyrinth(PlayerManager.instance.currentMapStage.prefabAmount);
    }
    // add core to list
    public void AddCore(Core core)
    {
       allCores.Add(core);
    }
    // remove mob from list
    public void RemoveCore(Core core)
    {
        allCores.Remove(core);
    }
    // start spawning cores on the map
    public void StartAddingCores()
    {
        // get canvas transform
        Transform canvas = GameObject.Find("BackUI").transform;

        // !!!! ---[DEMO]--- !!!!
        if(PlayerManager.instance.currentMapStage.stageIndex == 4 && false)
        {
            GameObject[] cores = new GameObject[3];
            for (int i = 0; i < cores.Length; i++)
            {
                if (labyrinth[i][ScoreManager.instance.totalSpawnedCores] == '-')
                {
                    // init of prefab
                    GameObject _prefab = Resources.Load<GameObject>(@"Prefabs\Enemies\EffectPrefabs\Tnt");
                    cores[i] = Instantiate(_prefab, new Vector3(0, 0, 100), Quaternion.identity, canvas);

                    // set core's position
                    cores[i].GetComponent<RectTransform>().position = new Vector3(cores[i].transform.position.x, yPoints[i], ShipController.instance.shipPanel.position.z);

                    // add core to all active cores
                    AddCore(cores[i].transform.GetComponentInChildren<Core>());
                }
                else if (labyrinth[i][ScoreManager.instance.totalSpawnedCores] == '?')
                {
                    // init of prefab
                    GameObject _prefab = Resources.Load<GameObject>(@"Prefabs\Enemies\EffectPrefabs\LifeUnitEffect");
                    cores[i] = Instantiate(_prefab, new Vector3(0, 0, 100), Quaternion.identity, canvas);

                    // set core's position
                    cores[i].GetComponent<RectTransform>().position = new Vector3(cores[i].transform.position.x, yPoints[i], ShipController.instance.shipPanel.position.z);

                    // add core to all active cores
                    AddCore(cores[i].transform.GetComponentInChildren<Core>());
                }
            } // for i

            // increase spawn cores amount
            ScoreManager.instance.SpawnCore();

            // increase core's and ship's anim velocity
            StageInfo _stageInfo = PlayerManager.instance.currentMapStage;
            float _velocity = _stageInfo.minVelocity + acceleration * ScoreManager.instance.totalSpawnedCores; // count velocity
            SetVelocity(_velocity);
            ShipController.instance.SetVelocity(_velocity);
        }
        else
        {
            // init of prefab
            GameObject core = Instantiate(GetRandomCore(/*15*/100), new Vector3(0, 0, 100), Quaternion.identity, canvas);

            // set core's position
            core.GetComponent<RectTransform>().position = new Vector3(core.transform.position.x, yPoints[Random.Range(0, 3)], ShipController.instance.shipPanel.position.z);

            // add core to all active cores
            AddCore(core.transform.GetComponentInChildren<Core>());

            // count only animals(effects aren't a part of spawned animals)
            if (core.transform.GetComponentInChildren<Animal>() != null || core.transform.GetComponentInChildren<Chest>() != null)
            {
                // increase spawn cores amount
                ScoreManager.instance.SpawnCore();

                // increase core's and ship's anim velocity
                StageInfo _stageInfo = PlayerManager.instance.currentMapStage;

                float _velocity = _stageInfo.minVelocity + acceleration * ScoreManager.instance.totalSpawnedCores; // count velocity
                SetVelocity(_velocity);
                ShipController.instance.SetVelocity(_velocity);

                // stop spawning cores manually
                if (core.transform.GetComponentInChildren<Chest>() != null) return;
            } 
        }

        // next call of mobs
        Invoke("StartAddingCores", spawnVelocity);
    }
    public void StartAddingTnt()
    {
        // get canvas transform
        Transform canvas = GameObject.Find("BackUI").transform;

        // start adding
        GameObject[] cores = new GameObject[3];
        for (int i = 0; i < cores.Length; i++)
        {
            if (labyrinth[i][ScoreManager.instance.totalSpawnedCores] == '-')
            {
                // init of prefab
                GameObject _prefab = Resources.Load<GameObject>(@"Prefabs\Enemies\EffectPrefabs\Tnt");
                cores[i] = Instantiate(_prefab, new Vector3(0, 0, 100), Quaternion.identity, canvas);

                // set core's position
                cores[i].GetComponent<RectTransform>().position = new Vector3(cores[i].transform.position.x, yPoints[i], ShipController.instance.shipPanel.position.z);

                // add core to all active cores
                AddCore(cores[i].transform.GetComponentInChildren<Core>());
            }
            else if (labyrinth[i][ScoreManager.instance.totalSpawnedCores] == '?')
            {
                // init of prefab
                GameObject _prefab = Resources.Load<GameObject>(@"Prefabs\Enemies\EffectPrefabs\LifeUnitEffect");
                cores[i] = Instantiate(_prefab, new Vector3(0, 0, 100), Quaternion.identity, canvas);

                // set core's position
                cores[i].GetComponent<RectTransform>().position = new Vector3(cores[i].transform.position.x, yPoints[i], ShipController.instance.shipPanel.position.z);

                // add core to all active cores
                AddCore(cores[i].transform.GetComponentInChildren<Core>());
            }
        } // for i

        // increase spawn cores amount
        ScoreManager.instance.SpawnCore();

        // increase core's and ship's anim velocity
        StageInfo _stageInfo = PlayerManager.instance.currentMapStage;
        float _velocity = _stageInfo.minVelocity + acceleration * ScoreManager.instance.totalSpawnedCores; // count velocity
        SetVelocity(_velocity);
        ShipController.instance.SetVelocity(_velocity);

        // next call of mobs
        Invoke("StartAddingTnt", spawnVelocity);
    }
    // set move speed for all mobs on map
    public void SetVelocity(float velocity)
    {
        animVelocity = velocity;
        allCores.ForEach((t) => { t.ChangeAnimVelocity(animVelocity); });
        spawnVelocity = 2 / animVelocity;
    }
    // stop spawning cores on the map
    public void StopAddingCores()
    {
        CancelInvoke();
    }

    /// <param name="dropChance">Number in %</param>
    private GameObject GetRandomCore(float dropChance)
    {
        GameObject core = new GameObject();
        if(ScoreManager.instance.totalSpawnedCores == PlayerManager.instance.currentMapStage.prefabAmount 
            && PlayerManager.instance.currentMapStage.stageIndex != 5)
        {
            // load chest
            core = Resources.Load<GameObject>(@"Prefabs\Enemies\EffectPrefabs\Chests\" + PlayerManager.instance.currentMap.chestType);
        }
        else if (leftTimeForChance < 0 && Random.Range(0, 100) < dropChance)
        {
            // load effect
            List<EffectType> _effectTypes = PlayerManager.instance.currentMapStage.effectsList;
            string _effectTypeName = _effectTypes[Random.Range(0, _effectTypes.Count)].ToString();
            core = Resources.Load<GameObject>(@"Prefabs\Enemies\EffectPrefabs\" + _effectTypeName);
            leftTimeForChance = timeForChance;
        }
        else
        {
            // load animal
            core = Resources.Load<GameObject>(@"Prefabs\Enemies\AnimalPrefab\Animal");
        }

        return core;
    }
    #endregion

    #region Labyrinth
    static private List<List<char>> GetLabyrinth(int length)
    {
        // create labyrinth
        List<List<char>> lab = new List<List<char>>
            {
                new List<char>(),
                new List<char>(),
                new List<char>()
            };

        // fill it with selected symbol in
        for (int i = 0; i < lab.Count; i++)
            for (int j = 0; j < length; j++)
                lab[i].Add('-');

        // fill first wave in
        int startEntrance = Random.Range(0, 3);
        lab[startEntrance][0] = '+';

        // fill second wave in
        int _subFreeWay = Random.Range(0, 3);
        lab[startEntrance][1] = '+';

        if (_subFreeWay != startEntrance)
            if (Mathf.Abs(_subFreeWay - startEntrance) <= 1)
                lab[_subFreeWay][1] = '+';

        // fill other part in
        for (int i = 1; i < length; i++)
        {
            /*
                * - - - + + + - - - - - + + + - 
                * + + - + - + + + + + - + - + + 
                * - + + + - - - - - + + + + + - 
            */
            // check last wave for amount of free ways
            List<int> previousFreeWaysIndexes = GetFreeWaysIndexes(lab, i - 1);

            // make labyrinth
            if (previousFreeWaysIndexes.Count == 1)
            {
                // open the same index way
                lab[previousFreeWaysIndexes[0]][i] = '+';

                // try to open new ways
                int[] otherTwoWays = new int[2];
                switch (previousFreeWaysIndexes[0])
                {
                    case 0: otherTwoWays[0] = 1; otherTwoWays[1] = 2; break;
                    case 1: otherTwoWays[0] = 0; otherTwoWays[1] = 2; break;
                    case 2: otherTwoWays[0] = 0; otherTwoWays[1] = 1; break;
                } // switch

                // open second free way
                int secondFreeWayIndex = Random.Range(0, 2) == 0? otherTwoWays[0] : otherTwoWays[1];
                if (Mathf.Abs(secondFreeWayIndex - previousFreeWaysIndexes[0]) == 1)
                    lab[secondFreeWayIndex][i] = '+';

                // open third free way
                if (GetChance(50))
                {
                    if (GetChance(50))
                    {
                        int thirdFreeWayIndex = secondFreeWayIndex == otherTwoWays[0] ? otherTwoWays[1] : otherTwoWays[0];
                        if(GetChance(10))
                            lab[thirdFreeWayIndex][i] = '?';
                        // ...
                        // spawn live unit here
                        else
                            lab[thirdFreeWayIndex][i] = '+';
                    }
                }
            }
            else if (previousFreeWaysIndexes.Count == 2)
            {
                List<int> previousPreviousFreeWaysIndexes = GetFreeWaysIndexes(lab, i - 2);
                if (previousPreviousFreeWaysIndexes.Count == 1)
                {
                    if (previousPreviousFreeWaysIndexes[0] == previousFreeWaysIndexes[0])
                        lab[previousFreeWaysIndexes[1]][i] = '+';
                    else
                        lab[previousFreeWaysIndexes[0]][i] = '+';
                }
                else
                {
                    // free new ways according to the previous
                    previousFreeWaysIndexes.ForEach(t => lab[t][i] = '+');

                    // connect two free ways
                    if (GetChance(30))
                        lab[1][i] = '+';
                }
            }
            else
            {
                List<int> previousPreviousFreeWaysIndexes = GetFreeWaysIndexes(lab, i - 2);
                if (previousPreviousFreeWaysIndexes.Count != 1)
                {
                    lab[1][i] = '+';
                }
                else
                {
                    switch (previousPreviousFreeWaysIndexes[0])
                    {
                        case 0:
                        {
                            lab[2][i] = '+';
                            if (GetChance(75))
                                lab[0][i] = '+';
                        }
                        break;
                        case 1:
                        {
                            lab[0][i] = '+';
                            lab[2][i] = '+';
                        }
                        break;
                        case 2:
                        {
                            lab[0][i] = '+';
                            if (GetChance(75))
                                lab[2][i] = '+';
                        }
                        break;
                    }
                }
            }
        } // for i

        return lab;
    }
    static private bool GetChance(int percent)
    {
        // percent is 0 - 100
        if (Random.Range(0, 101) <= percent)
            return true;
        else
            return false;
    }
    static private List<int> GetFreeWaysIndexes(List<List<char>> lab, int wave)
    {
        List<int> previousFreeWaysIndexes = new List<int>();
        int _length = lab.Count;
        for (int i = 0; i < _length; i++)
            if (lab[i][wave] == '+' || lab[i][wave] == '?')
                previousFreeWaysIndexes.Add(i);

        return previousFreeWaysIndexes;
    }
    #endregion
}
public enum CoreType
{
    Animal,
    Effect
}
