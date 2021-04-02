using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

// сердце достижений
public class AchievementManager : MonoBehaviour
{
    #region Singleton
    public static AchievementManager instance;
    void Awake()
    {
        if (instance != null)
            Destroy(instance);
        instance = this;
        DontDestroyOnLoad(instance);
    }
    #endregion

    // goal type wiil be used for distribution achievements on their types
    // делегаты для распределения всех достижений и упрощение их работы
    // все что нужно - вызывать определенный делегат, после того как загрузятся все достижения в менеджере
    // после вызова, делегат заменяется на метож, который автоматически увеличит нужное кол-во единиц до завершения
    // ! так же можно было сделать делегаты + type, если бы в разных режимах дублировались бы достижения !
    #region achievement_delegates
    public delegate void ActiveAchievement(GoalType goalType);
    public ActiveAchievement activeAchievements;
    public delegate void BoostAchievement(GoalType goalType);
    public BoostAchievement boostAchievements;
    public delegate void DeathAchievement(GoalType goalType);
    public DeathAchievement deathAchievements;
    public delegate void DiscoveryAchievement(GoalType goalType);
    public DiscoveryAchievement discoveryAchievements;
    public delegate void HardcoreAchievement(GoalType goalType);
    public HardcoreAchievement hardcoreAchievements;
    public delegate void KillAchievement(GoalType goalType);
    public KillAchievement killAchievements;
    public delegate void MagnateAchievement(GoalType goalType);
    public MagnateAchievement magnateAchievements;
    public delegate void RichmanAchievement(GoalType goalType);
    public RichmanAchievement richmanAchievements;
    public delegate void ScoreAchievement(GoalType goalType);
    public ScoreAchievement scoreAchievements;
    public delegate void MorningAchievement(GoalType goalType);
    public MorningAchievement morningAchievements;
    public delegate void NightAchievement(GoalType goalType);
    public NightAchievement nightAchievements;
    #endregion

    // все достижения будут добвлять в список, они будут храниться там даже после завершения
    // это нужно для того, чтобы проверять, завершилось ли достижения или нет
    // (это нужно будет, если после выполнения опред. достижения человек будет получать какой-то уник. подарок)
    public Text achievementAmountDisplay;
    public List<Achievement> allAchievements;
    public bool isTestMode; // тестовый мод для обнуления всех достижений каждый запуск

    void Start()
    {
        ActivateAchievements();
        UpdateAchievementAmount();

        //PlayerPrefs.DeleteAll();

        StartCoroutine(CheckForTimeAchievements(3));
    }
    void Update()
    {
        // test of new achievement toggling
        if (Input.GetKeyDown(KeyCode.A))
        {
            Achievement ach = allAchievements[UnityEngine.Random.Range(0, allAchievements.Count)];
            print(ach.GetTitle());
            AchievementInformer.instance.SetAndShowNewAchievement(ach);
        }
    }
    // send info through AchievementManager
    public void SendInfo(string info)
    {
        print(info);
    }
    // обновление информации о кол-ве выполненных достижений на панели
    public void UpdateAchievementAmount()
    {
        string amountText;
        int completedAchievements = 0;
        foreach (Achievement ach in allAchievements)
            if (!ach.isActive)
                completedAchievements++;

        amountText = completedAchievements + "/" + allAchievements.Count;
        achievementAmountDisplay.text = amountText;
    }

    // активация незавершенных достижений
    private void ActivateAchievements()
    {
        foreach(Achievement ach in allAchievements)
        {
            // TODO: на релизе сделать проверку на переменную в PlayerPrefs(если ее нету - обнулить и добавить)
            if (isTestMode)
            {
                ach.isActive = true;
                ach.goal.currentAmount = 0;
            }
            // если достижение уже было завершенно, повторно его не активировать
            if (!ach.isActive) continue;
            switch (ach.goal.goalType)
            {
                case GoalType.Active: activeAchievements += ach.DoGoal; break;
                case GoalType.Boost: boostAchievements += ach.DoGoal; break;
                case GoalType.Death: deathAchievements += ach.DoGoal; break;
                case GoalType.Discovery: discoveryAchievements += ach.DoGoal; break;
                case GoalType.Hardcore: hardcoreAchievements += ach.DoGoal; break;
                case GoalType.Kill: killAchievements += ach.DoGoal; break;
                case GoalType.Magnate: magnateAchievements += ach.DoGoal; break;
                case GoalType.Richman: richmanAchievements += ach.DoGoal; break;
                case GoalType.Score: scoreAchievements += ach.DoGoal; break;
                case GoalType.TheLark: morningAchievements += ach.DoGoal; break;
                case GoalType.TheOwl: nightAchievements += ach.DoGoal; break;
                default: break;
            }   
        }
    }
    // проверка на выполнение достижений связанных со временем
    private IEnumerator CheckForTimeAchievements(float sec)
    {
        while (true)
        {
            // getting current time
            DateTime time = DateTime.Now;
            DateTime lastTimeNightAchCompleted, // night ach
                lastTimeMorningAchCompleted, // morning ach
                lastTimeActiveAchCompleted; // active ach

            // load ach last time datetime
            if (PlayerPrefs.HasKey("MorningAchievementCompleted"))
                lastTimeMorningAchCompleted = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString("MorningAchievementCompleted")));
            else
                lastTimeMorningAchCompleted = time.AddDays(-1);

            if (PlayerPrefs.HasKey("NightAchievementCompleted"))
                lastTimeNightAchCompleted = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString("NightAchievementCompleted")));
            else
                lastTimeNightAchCompleted = time.AddDays(-1);

            if(PlayerPrefs.HasKey("ActiveAchievementCompleted"))
                lastTimeActiveAchCompleted = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString("ActiveAchievementCompleted")));
            else
                lastTimeActiveAchCompleted = time.AddDays(-1);

            // night
            if (time.Hour >= 0 && time.Hour < 4 && time.Subtract(lastTimeNightAchCompleted).Days >= 1)
            {
                print("night ach");
                // взять название достижения и привязать к нему дату получения достижения
                string _host = time.ToBinary().ToString();
                PlayerPrefs.SetString("NightAchievementCompleted", _host);
                nightAchievements?.Invoke(GoalType.TheOwl);
            }
            // morning
            else if (time.Hour >= 4 && time.Hour < 8 && time.Subtract(lastTimeMorningAchCompleted).Days >= 1)
            {
                print("morning ach");
                string _host = time.ToBinary().ToString();
                PlayerPrefs.SetString("MorningAchievementCompleted", _host);
                morningAchievements?.Invoke(GoalType.TheLark);
            } // if else

            // active
            if (time.Subtract(lastTimeActiveAchCompleted).Days >= 1)
            {
                print("active ach");
                string _host = time.ToBinary().ToString();
                PlayerPrefs.SetString("ActiveAchievementCompleted", _host);
                activeAchievements?.Invoke(GoalType.Active);
            } // if

            yield return new WaitForSeconds(sec);
        } // while
    } // CheckForTimeAchievements
    // получение даты из интернета(крашит если вызывать из лупа StartCoroutine)
    private DateTime? GetNetDateTime()
    {
        try { 
            var myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.microsoft.com");
            var response = myHttpWebRequest.GetResponse();
            string todaysDates = response.Headers["date"];
            return DateTime.ParseExact(todaysDates,
                "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                CultureInfo.InvariantCulture.DateTimeFormat,
                DateTimeStyles.AssumeUniversal);
        }
        catch
        {
            return null;
        }
    }
    // getting date from string 'd' or 'dd.MM.yyyy'
    private DateTime? GetDateTimeFromString(string dateTime)
    {
        // pattern shoud be dd.MM.yyyy or 'd'
        int day, month, year;
        string[] timeArr = dateTime.Split('.');

        // check for length(shoud be 3 parameters
        if (timeArr.Length != 3) return null;

        // get values
        day = int.Parse(timeArr[0]);
        month = int.Parse(timeArr[1]);
        year = int.Parse(timeArr[2]);

        // check for correct parameters
        if ((day < 1 || day > 31) || (month < 1 || month > 12) || (year < 2000 || year > 2113))
            return null;

        // create and return DateTime
        // I don't care about time that's why return just dd MM yyyy
        return new DateTime(year, month, day);
    }
}
