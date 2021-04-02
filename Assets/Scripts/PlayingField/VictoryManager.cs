using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VictoryManager : MonoBehaviour
{
    // mark images of all texts
    public Image totalScoreMarkImage;
    public Image totalDeadCoresImage;
    public Image totalObtainedEffectsImage;

    // tmps of all texts
    public TextMeshProUGUI totalScoreText;
    public TextMeshProUGUI totalScoreTextShadow;
    public TextMeshProUGUI totalDeadCoresText;
    public TextMeshProUGUI totalDeadCoresTextShadow;
    public TextMeshProUGUI totalObtainedEffectsText;
    public TextMeshProUGUI totalObtainedEffectsTextShadow;

    public Animator contentHoverAnimator; // animated of c.h.(when text is about it's mask)

    public ScrollRect scrollRect; // scroller of all content
    private float yContentStartPosition; // start top y of content of scroll rect

    public GameObject[] summingUpPanelMarks; // summing up marks 
    private int selectedIndex; // selected index of summing up mark

    // utils for text
    private bool[] threeNumbersFlags;
    private bool[] sixNumbersFlags;

    private Mark[] textsMarks;

    public void Start()
    {
        Init();
    }
    public void Update()
    {
        // check for content hover
        if (scrollRect.verticalNormalizedPosition < .99 && !contentHoverAnimator.GetBool("Showing"))
            contentHoverAnimator.SetBool("Showing", true);
        else if (scrollRect.verticalNormalizedPosition >= .99 && contentHoverAnimator.GetBool("Showing"))
            contentHoverAnimator.SetBool("Showing", false);
    }
    public void Init()
    {
        // summing up marks init
        textsMarks = new Mark[3];
        selectedIndex = -1;

        // animating text utils init
        threeNumbersFlags = new bool[3];
        sixNumbersFlags = new bool[3];

        // prevent vertical scrolling
        PreventVerticalScrolling();

        // get start top y of content
        yContentStartPosition = scrollRect.content.transform.localPosition.y;

        // total score meterial init
        totalScoreMarkImage.material.SetFloat("_DissolveAmount", 1);

        // total score meterial init
        totalDeadCoresImage.material.SetFloat("_DissolveAmount", 1);

        // total score meterial init
        totalObtainedEffectsImage.material.SetFloat("_DissolveAmount", 1);
    }
    public void StartCountingTotalScoreAmount()
    {
        // animate text counting
        int _totalScoreAmount = 6000;
        //int _totalScoreAmount = ScoreManager.instance.score;
        AnimateTextNumber(_totalScoreAmount, "SetTotalScoreTexts");

        // get mark texture according to score's rate and set it to Sprite Graph _MainTexture
        Texture _markTexture = GetTexture("Score");
        totalScoreMarkImage.material.SetTexture("_MainTexture", _markTexture);

        // animate mark appearing(with dissolve sprite graph)
        StartCoroutine(Invoke(() =>
        {
            iTween.ValueTo(gameObject, iTween.Hash(
                "from", 1,
                "to", 0,
                "time", 1.5f,
                "easetype", iTween.EaseType.easeOutQuad,
                "ignoretimescale", true,
                "onupdate", "SetTotalScoreMarkDissolveAmount",
                "oncomplete", "SetTotalScoreMarkImageTexture"
            ));
        }, 1f));

        // invoke total dead cores counting animating in N seconds
        StartCoroutine(Invoke(StartCountingTotalDeadCoresAmount, 1.5f));
    }
    public void StartCountingTotalDeadCoresAmount()
    {
        // animate text counting
        int _totalDeadCoresAmount = 248;
        //int _totalDeadCoresAmount = ScoreManager.instance.totalDeadCores;
        AnimateTextNumber(_totalDeadCoresAmount, "SetTotalDeadCoresTexts");

        // get mark texture according to score's rate and set it to Sprite Graph _MainTexture
        Texture _markTexture = GetTexture("DeadCores");
        totalDeadCoresImage.material.SetTexture("_MainTexture", _markTexture);

        // animate mark appearing(with dissolve sprite graph)
        StartCoroutine(Invoke(() =>
        {
            iTween.ValueTo(gameObject, iTween.Hash(
                "from", 1,
                "to", 0,
                "time", 1.5f,
                "easetype", iTween.EaseType.easeOutQuad,
                "ignoretimescale", true,
                "onupdate", "SetTotalDeadCoresMarkDissolveAmount",
                "oncomplete", "SetTotalDeadCoresMarkImageTexture"
            ));
        }, 1f));

        // invoke total dead cores counting animating in N seconds
        StartCoroutine(Invoke(StartCountingTotalObtainedEffectsAmount, 1.5f));
    }
    public void StartCountingTotalObtainedEffectsAmount()
    {
        // animate text counting
        int _totalObtainedEffectsAmount = 8;
        //int _totalObtainedEffectsAmount = ScoreManager.instance.totalObtainedEffects;
        AnimateTextNumber(_totalObtainedEffectsAmount, "SetTotalObtainedEffectsTexts");

        // get mark texture according to score's rate and set it to Sprite Graph _MainTexture
        Texture _markTexture = GetTexture("ObtainedEffects");
        totalObtainedEffectsImage.material.SetTexture("_MainTexture", _markTexture);

        // animate mark appearing(with dissolve sprite graph)
        StartCoroutine(Invoke(() =>
        {
            iTween.ValueTo(gameObject, iTween.Hash(
                "from", 1,
                "to", 0,
                "time", 1f,
                "easetype", iTween.EaseType.easeOutQuad,
                "ignoretimescale", true,
                "onupdate", "SetTotalObtainedEffectsDissolveAmount",
                "oncomplete", "SetTotalObtainedEffectsMarkImageTexture"
            ));
        }, 1f));

        // move content farther
        StartCoroutine(Invoke(MoveToBtnMenu, 1.5f));
    }

    private void MoveToBtnMenu()
    {
        MoveTo("BtnMenu", null, 1.25f);
        StartCoroutine(Invoke(SumUpTheResult, 1f));
    }
    private void SumUpTheResult()
    {
        // sum up the total result
        int _totalScoreMarkNum = (int)textsMarks[0];
        int _totalDeadCoresMarkNum = (int)textsMarks[1];
        int _totalObtainedEffectsMarkNum = (int)textsMarks[2];
        float _summingUpResult = (_totalDeadCoresMarkNum + _totalDeadCoresMarkNum + _totalObtainedEffectsMarkNum) / 3;

        if (1f < _summingUpResult && _summingUpResult < 1.5f)
            selectedIndex = 3;
        else if (1.5f < _summingUpResult && _summingUpResult < 2.5f)
            selectedIndex = 2;
        else if (2.5f < _summingUpResult && _summingUpResult < 3.5f)
            selectedIndex = 1;
        else if (3.5f < _summingUpResult)
            selectedIndex = 0;

        // demo
        // TODO: понять почему оно не правильно считает результат, исправить
        print("Result: " + _summingUpResult);   
        print("Index: " + selectedIndex);

        // hide all marks
        foreach (GameObject mark in summingUpPanelMarks)
            mark.SetActive(false);

        // select mark
        GameObject _summingUpPanelMark = summingUpPanelMarks[selectedIndex];
        _summingUpPanelMark.SetActive(true);

        // animate scaling
        iTween.ScaleFrom(_summingUpPanelMark, iTween.Hash(
            "x", .75f,
            "y", .75f,
            "time", .75f,
            "easetype", iTween.EaseType.easeOutBack,
            "ignoretimescale", true
        ));

        // animate appearing(color alpha changing)
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", 0,
            "to", 1,
            "time", .75f,
            "easetype", iTween.EaseType.easeInOutQuad,
            "ignoretimescale", true,
            "onupdate", "SetSummingUpMarkImageColorAlpha",
            "oncomplete", "AllowVerticalScrolling"
        ));
    }
    private void SetSummingUpMarkImageColorAlpha(float value)
    {
        // по хорошему нужно было отдельно картиночки добавить, но да пошло оно нахуй
        Image _img = summingUpPanelMarks[selectedIndex].GetComponent<Image>();
        Color _color = _img.color;
        _color.a = value;
        _img.color = _color;

    }

    #region animation utils

    private void MoveTo(string name, string callback = null, float time = .5f, iTween.EaseType easeType = iTween.EaseType.easeInOutBack)
    {
        print(name);
        // get transform of needed panel inside the content
        GameObject _panel = GameObject.Find("VictoryPanel/Main/InfoPanel/Mask/Content/" + name);
        Transform _panelTransform = _panel.transform;
        RectTransform _panelRectTransform = _panel.GetComponent<RectTransform>();

        // get top and bottom edges of mask(the visible part of content)
        float topY = GameObject.Find("InfoPanel/Mask").transform.position.y + GameObject.Find("InfoPanel/Mask").GetComponent<RectTransform>().rect.height / 2;
        float bottomY = GameObject.Find("InfoPanel/Mask").transform.position.y - GameObject.Find("InfoPanel/Mask").GetComponent<RectTransform>().rect.height / 2;

        // get top and bottom edges of required panel
        float panelTopY = _panelTransform.position.y + _panelRectTransform.rect.height / 2;
        float panelBottomY = _panelTransform.position.y - _panelRectTransform.rect.height / 2;

        // get direction of scroll rect moving (up/down) and move scoll rect according to calculated values
        float yLeakedAxisLength, verticalNormalizedPosition = 1f;
        bool moveRequired = false;
        if (panelTopY > topY)
        {
            // get vertical normalized position
            yLeakedAxisLength = Mathf.Abs(panelTopY - topY);
            verticalNormalizedPosition = Mathf.Abs((scrollRect.content.transform.localPosition.y - yLeakedAxisLength + yContentStartPosition) / (2 * yContentStartPosition));
            moveRequired = true;
        }
        else if (panelBottomY < bottomY)
        {
            // get vertical normalized position
            yLeakedAxisLength = Mathf.Abs(panelBottomY - bottomY);
            verticalNormalizedPosition = Mathf.Abs((scrollRect.content.transform.localPosition.y + yLeakedAxisLength + yContentStartPosition) / (2 * yContentStartPosition));
            moveRequired = true;
        }

        if (moveRequired)
        {
            // demo
            if (name == "BtnMenu")
                print(verticalNormalizedPosition);

                // move scroll
            if (callback != null)
                iTween.ValueTo(gameObject, iTween.Hash(
                    "from", scrollRect.verticalNormalizedPosition,
                    "to", verticalNormalizedPosition,
                    "time", time,
                    "easetype", easeType,
                    "ignoretimescale", true,
                    "onupdate", "SetVerticalNormalizedPosition",
                    "oncomplete", callback
                ));
            else
                iTween.ValueTo(gameObject, iTween.Hash(
                    "from", scrollRect.verticalNormalizedPosition,
                    "to", verticalNormalizedPosition,
                    "time", time,
                    "easetype", easeType,
                    "ignoretimescale", true,
                    "onupdate", "SetVerticalNormalizedPosition"
                ));
        }
    }
    private void PreventVerticalScrolling()
    {
        scrollRect.vertical = false;
    }
    private void AllowVerticalScrolling()
    {
        scrollRect.vertical = true;
    }
    private void SetVerticalNormalizedPosition(float value)
    {
        scrollRect.verticalNormalizedPosition = value;
    }

    #endregion

    #region animated text counting utility

    // text animating with counting it's number from 0 to N
    private void AnimateTextNumber(int totalAmount, string updateCallback)
    {
        // add numbers animatedly to that clean place
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", 0,
            "to", totalAmount,
            "time", 1f,
            "easetype", iTween.EaseType.easeInOutQuad,
            "ignoretimescale", true,
            "onupdate", updateCallback
        ));
    }

    // text animating utility
    private void SetTotalScoreTexts(float value)
    {
        // init
        const float _numberRatio = 2f;
        string _text = totalScoreText.text;
        int _usedAmountOfNumbers = GetAmountOfNumbers(_text);
        int _valueAmountOfNumbers = ((int)value).ToString().Length;

        // get index
        int _index = _text.IndexOfAny("0123456789".ToCharArray());

        if(_usedAmountOfNumbers == 3 && !threeNumbersFlags[0])
        {
            threeNumbersFlags[0] = true;
            _text = _text.Insert(_text.LastIndexOf('.'), ".");
            ++_index;
        }
        if(_usedAmountOfNumbers == 6 && !sixNumbersFlags[0])
        {
            sixNumbersFlags[0] = true;
            _text = _text.Insert(_text.LastIndexOf('.'), ".");
            ++_index;
        }

        // works for the number which is only increasing
        if (_valueAmountOfNumbers > _usedAmountOfNumbers)
        {
            // clear needed amount of dots in front of number
            float _lastRatioParam = _usedAmountOfNumbers * _numberRatio, _newRatioParam = _valueAmountOfNumbers * _numberRatio; 
            int _currentNeededAmountOfDotsToReduce = (int)(_newRatioParam - _lastRatioParam);

            _index -= _currentNeededAmountOfDotsToReduce;
            _text = _text.Remove(_index, _currentNeededAmountOfDotsToReduce);
        }

        // replace first number in text with value
        string _host = ((int)value).ToString();
        _text = _text.Remove(_index);
        _text = _text.Insert(_index, _host);

        // set text
        totalScoreText.text = totalScoreTextShadow.text = _text;
    }
    private void SetTotalDeadCoresTexts(float value)
    {
        // init
        const float _numberRatio = 2f;
        string _text = totalDeadCoresTextShadow.text;
        int _usedAmountOfNumbers = GetAmountOfNumbers(_text);
        int _valueAmountOfNumbers = ((int)value).ToString().Length;

        // get index
        int _index = _text.IndexOfAny("0123456789".ToCharArray());

        if (_usedAmountOfNumbers == 3 && !threeNumbersFlags[1])
        {
            threeNumbersFlags[1] = true;
            _text = _text.Insert(_text.LastIndexOf('.'), ".");
            ++_index;
        }
        if (_usedAmountOfNumbers == 6 && !sixNumbersFlags[1])
        {
            sixNumbersFlags[1] = true;
            _text = _text.Insert(_text.LastIndexOf('.'), ".");
            ++_index;
        }

        // works for the number which is only increasing
        if (_valueAmountOfNumbers > _usedAmountOfNumbers)
        {
            // clear needed amount of dots in front of number
            float _lastRatioParam = _usedAmountOfNumbers * _numberRatio, _newRatioParam = _valueAmountOfNumbers * _numberRatio;
            int _currentNeededAmountOfDotsToReduce = (int)(_newRatioParam - _lastRatioParam);

            _index -= _currentNeededAmountOfDotsToReduce;
            _text = _text.Remove(_index, _currentNeededAmountOfDotsToReduce);
        }

        // replace first number in text with value
        string _host = ((int)value).ToString();
        _text = _text.Remove(_index);
        _text = _text.Insert(_index, _host);

        // set text
        totalDeadCoresText.text = totalDeadCoresTextShadow.text = _text;
    }
    private void SetTotalObtainedEffectsTexts(float value)
    {
        // init
        const float _numberRatio = 2f;
        string _text = totalObtainedEffectsText.text;
        int _usedAmountOfNumbers = GetAmountOfNumbers(_text);
        int _valueAmountOfNumbers = ((int)value).ToString().Length;

        // get index
        int _index = _text.IndexOfAny("0123456789".ToCharArray());

        if (_usedAmountOfNumbers == 3 && !threeNumbersFlags[2])
        {
            threeNumbersFlags[2] = true;
            _text = _text.Insert(_text.LastIndexOf('.'), ".");
            ++_index;
        }
        if (_usedAmountOfNumbers == 6 && !sixNumbersFlags[2])
        {
            sixNumbersFlags[2] = true;
            _text = _text.Insert(_text.LastIndexOf('.'), ".");
            ++_index;
        }

        // works for the number which is only increasing
        if (_valueAmountOfNumbers > _usedAmountOfNumbers)
        {
            // clear needed amount of dots in front of number
            float _lastRatioParam = _usedAmountOfNumbers * _numberRatio, _newRatioParam = _valueAmountOfNumbers * _numberRatio;
            int _currentNeededAmountOfDotsToReduce = (int)(_newRatioParam - _lastRatioParam);

            _index -= _currentNeededAmountOfDotsToReduce;
            _text = _text.Remove(_index, _currentNeededAmountOfDotsToReduce);
        }

        // replace first number in text with value
        string _host = ((int)value).ToString();
        _text = _text.Remove(_index);
        _text = _text.Insert(_index, _host);

        // set text
        totalObtainedEffectsText.text = totalObtainedEffectsTextShadow.text = _text;
    }

    // get amount of numbers in text(string)
    private int GetAmountOfNumbers(string text)
    {
        int _amount = 0;
        for (int i = 0; i < text.Length; i++)
            if (text[i].ToString().IndexOfAny("0123456789".ToCharArray()) == 0)
                _amount++;
        return _amount;
    }

    #endregion

    #region animated mark appearing utility

    // setting dissolve amount of lit graph through the material
    private void SetTotalScoreMarkDissolveAmount(float value) => totalScoreMarkImage.material.SetFloat("_DissolveAmount", value);
    private void SetTotalDeadCoresMarkDissolveAmount(float value) => totalDeadCoresImage.material.SetFloat("_DissolveAmount", value);
    private void SetTotalObtainedEffectsDissolveAmount(float value) => totalObtainedEffectsImage.material.SetFloat("_DissolveAmount", value);

    // setting material mark texture of lit graph
    private void SetTotalScoreMarkImageTexture()
    {
        totalScoreMarkImage.sprite = GetSprite("Score");
        totalScoreMarkImage.material = null;
    }
    private void SetTotalDeadCoresMarkImageTexture()
    {
        totalDeadCoresImage.sprite = GetSprite("DeadCores");
        totalDeadCoresImage.material = null;
    }
    private void SetTotalObtainedEffectsMarkImageTexture()
    {
        totalObtainedEffectsImage.sprite = GetSprite("ObtainedEffects");
        totalObtainedEffectsImage.material = null;
    }

    // mark getting accoring to the progress
    private Mark GetMark(string type)
    {
        Mark mark = new Mark();
        switch (type)
        {
            case "Score": 
            {
                float _score = 6000f;
                //float _score = ScoreManager.instance.score;
                float _minScoreLimit = GetScoreLimit(LimitType.Min);
                float _maxScoreLimit = GetScoreLimit(LimitType.Max);
                float _firstCheckpoint = _minScoreLimit + (_maxScoreLimit - _minScoreLimit) / 4;
                float _secondCheckpoint = _minScoreLimit + (_maxScoreLimit - _minScoreLimit) * 2 / 4;
                float _thirdCheckpoint = _minScoreLimit + (_maxScoreLimit - _minScoreLimit) * 3 / 4;

                if (_minScoreLimit <= _score && _score < _firstCheckpoint)
                    mark = Mark.Bad;
                else if (_firstCheckpoint <= _score && _score < _secondCheckpoint)
                    mark = Mark.Normal;
                else if (_secondCheckpoint <= _score && _score < _thirdCheckpoint)
                    mark = Mark.Good;
                else if (_thirdCheckpoint <= _score && _score < _maxScoreLimit)
                    mark = Mark.Best;

                // save total amount mark
                textsMarks[0] = mark;
            } break;
            case "DeadCores": 
            {
                int _spawnedCores = 250;
                int _deadCores = 248;
                //float _spawnedCores = ScoreManager.instance.totalSpawnedCores;
                //float _deadCores = ScoreManager.instance.totalDeadCores;

                if (_deadCores == _spawnedCores - 3)
                    mark = Mark.Bad;
                else if (_deadCores == _spawnedCores - 2)
                    mark = Mark.Normal;
                else if (_deadCores == _spawnedCores - 1)
                    mark = Mark.Good;
                else if (_deadCores == _spawnedCores)
                    mark = Mark.Best;

                // save dead cores amount mark
                textsMarks[1] = mark;
            } break;
            case "ObtainedEffects": 
            {
                int _spawnedEffects = 9;
                int _obtainedEffects = 8;
                //int _spawnedEffects = ScoreManager.instance.totalSpawnedEffects;
                //int _obtainedEffects = ScoreManager.instance.totalObtainedEffects;
                float _spawnedEffectsRatio = _spawnedEffects / 4;

                if (0 <= _obtainedEffects && _obtainedEffects < _spawnedEffects)
                    mark = Mark.Bad;
                else if (_spawnedEffectsRatio <= _obtainedEffects && _obtainedEffects < _spawnedEffectsRatio * 2)
                    mark = Mark.Normal;
                else if (_spawnedEffectsRatio * 2 <= _obtainedEffects && _obtainedEffects < _spawnedEffectsRatio * 3)
                    mark = Mark.Good;
                else if (0 <= _obtainedEffects && _obtainedEffects < _spawnedEffectsRatio * 4)
                    mark = Mark.Best;

                // save obtained effects amount mark
                textsMarks[2] = mark;
            } break;
        }
        return mark;
    }

    // texture getting according to the mark type
    private Texture GetTexture(string type)
    {
        Mark mark = GetMark(type);
        string textureName = string.Empty;
        switch (mark)
        {
            case Mark.Best: textureName = "WinningBestMark"; break;
            case Mark.Good: textureName = "WinningGoodMark"; break;
            case Mark.Normal: textureName = "WinningNormalMark"; break;
            case Mark.Bad: textureName = "WinningBadMark"; break;
        }
        return Resources.Load<Texture>("Textures/PlayingField/WinningPanel/" + textureName);
    } 

    // sprite getting according to the mark type
    private Sprite GetSprite(string type)
    {
        Mark mark = GetMark(type);
        string spriteName = string.Empty;
        switch (mark)
        {
            case Mark.Best: spriteName = "WinningBestMark"; break;
            case Mark.Good: spriteName = "WinningGoodMark"; break;
            case Mark.Normal: spriteName = "WinningNormalMark"; break;
            case Mark.Bad: spriteName = "WinningBadMark"; break;
        }
        return Resources.Load<Sprite>("Textures/PlayingField/WinningPanel/" + spriteName);
    }

    // getting score limit according to the limit type
    private float GetScoreLimit(LimitType limitType)
    {
        // get amount of cores
        int cores = 250;
        //int cores = PlayerManager.instance.currentMapStage.prefabAmount;

        // get avarage point for each core
        int avaragePoint = 4;
        //int avaragePoint = PlayerManager.instance.currentMap.scorePoints[0] + PlayerManager.instance.currentMap.scorePoints[2] / 2;

        // host
        float multiplier = 1f;
        float score = 0f;

        // get result
        if (limitType == LimitType.Max)
        {
            for (int i = 0; i < cores; i++)
            {
                if (multiplier != 5) multiplier += 0.1f;
                score += avaragePoint * multiplier;
            }
        }
        else if(limitType == LimitType.Min)
        {
            for (int i = 0; i < cores; i++)
            {
                int _rate = cores / 4;
                if (i == _rate || i == _rate * 2 || i == _rate * 3 || i == _rate * 4)
                    multiplier = 1f;

                if (multiplier != 5) multiplier += 0.1f;
                score += avaragePoint * multiplier;
            }
        }

        return score;
    }

    #endregion

    private IEnumerator Invoke(System.Action action, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        action?.Invoke();
    }
}
public enum Mark
{
    Bad = 1,
    Good,
    Normal,
    Best
}
public enum LimitType
{
    Min,
    Max
}
