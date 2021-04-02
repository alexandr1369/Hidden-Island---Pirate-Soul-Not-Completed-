using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

public class ShipManager : MonoBehaviour
{
    #region Singleton
    public static ShipManager instance;
    void Awake()
    {
        if (instance != null)
            print("Another instance of Singleton class. Error!");
        instance = this;
    }
    #endregion

    public UnityArmatureComponent component; // dragon bones data
    public GameObject bgEffectPrefab; // back groung effect prefab

    private ParticleSystem bgEffect; // back ground animated ship fx (if exists)
    private ShipInfo ship; // ship data

    void Start()
    {
        // настройка сортировки элементов спрайтов(shit happens)
        component.sortingMode = SortingMode.SortByOrder;

        // load start textures
        LoadShipData();

        // add db event listener for textures and effects switching
        component.AddDBEventListener(EventObject.FRAME_EVENT, OnFrameEventHandler);

        // add db event listener for sounds playing
        component.AddDBEventListener(EventObject.SOUND_EVENT, OnSoundEventHandler);

        // skip 2 seconds of simulation(skip spreading) and stop
        bgEffect.Simulate(2f);

        // then play effect
        bgEffect.Play();
    }
    // start dragon bones animation
    public void BeginDbAnimation(string name)
    {
        component.animation.FadeIn(name, .2f, 1);
    }
    // set ship animation speed
    public void SetAnimVelocity(float animspeed)
    {
        // мультиплеер скорости анимаций
        animspeed = animspeed <= 0 ? 1f : animspeed;
        component.animation.timeScale = animspeed;
    }
    // loading ship data
    public void LoadShipData()
    {
        // load particle system
        LoadParticleSystem();

        // get current ship
        ship = PlayerManager.instance.currentShip;

        // get slot id
        int slotId = GetSlotId(ship.label);

        // set sail
        component.armature.GetSlot("Sail").displayIndex = slotId;

        // set treasure
        component.armature.GetSlot("Treasure").displayIndex = slotId;

        // set hat
        component.armature.GetSlot("Hat").displayIndex = slotId;

        // set sign
        component.armature.GetSlot("Sign").displayIndex = slotId;

        // set others slots
        if (ship.isAnimated)
        {
            // set right slotId
            // 5 is number of not animated ships(with default value on following slots
            slotId -= 5;

            // set ship body
            component.armature.GetSlot("ShipBody").displayIndex = slotId;

            // set left mast front
            component.armature.GetSlot("LeftMast").displayIndex = slotId;

            // set lest mast back
            component.armature.GetSlot("LeftMastBack").displayIndex = slotId;

            // set right mast front
            component.armature.GetSlot("RightMast").displayIndex = slotId;

            // set right mast back
            component.armature.GetSlot("RightMastBack").displayIndex = slotId;

            // set steering wheel
            component.armature.GetSlot("SteeringWheel").displayIndex = slotId;

            // set ShipEffect of special ship
            SetBgEffect();

            // show ShipEffect
            bgEffect.gameObject.SetActive(true);
        }
        else
        {
            // changing slot id to default
            slotId = 0;

            // set ship body
            component.armature.GetSlot("ShipBody").displayIndex = slotId;

            // set left mast front
            component.armature.GetSlot("LeftMast").displayIndex = slotId;

            // set lest mast back
            component.armature.GetSlot("LeftMastBack").displayIndex = slotId;

            // set right mast front
            component.armature.GetSlot("RightMast").displayIndex = slotId;

            // set right mast back
            component.armature.GetSlot("RightMastBack").displayIndex = slotId;

            // set steering wheel
            component.armature.GetSlot("SteeringWheel").displayIndex = slotId;

            // hide ShipEffect
            bgEffect.gameObject.SetActive(false);
        }

        //play default animation
        component.animation.Play("Idle");
    }
    // setting bg ship ShipEffect
    public void SetBgEffect()
    {
        // get modules
        ParticleSystem.MainModule main = bgEffect.main;
        ParticleSystem.EmissionModule emission = bgEffect.emission;
        ParticleSystem.ShapeModule shape = bgEffect.shape;
        ParticleSystem.ColorOverLifetimeModule colorOverLifeTime = bgEffect.colorOverLifetime;
        ParticleSystem.RotationOverLifetimeModule rotationOverLifeTime = bgEffect.rotationOverLifetime;
        ParticleSystem.TextureSheetAnimationModule textureShitAnimation = bgEffect.textureSheetAnimation;
        ParticleSystem.TrailModule trails = bgEffect.trails;
        ParticleSystem.NoiseModule noise = bgEffect.noise;

        switch (ship.ShipEffect)
        {
            case ShipEffect.Stars:
                {
                    // set start settings
                    main.startLifetime = new ParticleSystem.MinMaxCurve(.45f, .55f);
                    main.startSpeed = new ParticleSystem.MinMaxCurve(1, 2.5f);
                    main.startSize = new ParticleSystem.MinMaxCurve(.25f, .6f);
                    main.startColor = new ParticleSystem.MinMaxGradient(new Color32(255, 255, 255, 100), new Color32(255, 255, 255, 200));
                    main.gravityModifier = 1f;
                    main.simulationSpeed = .5f;

                    // set emission
                    emission.enabled = true;

                    emission.rateOverTime = 20f;

                    // set shape
                    shape.enabled = true;

                    shape.shapeType = ParticleSystemShapeType.SingleSidedEdge;
                    shape.radius = 1.4f;
                    shape.radiusMode = ParticleSystemShapeMultiModeValue.Random;
                    shape.randomDirectionAmount = .09f;
                    shape.sphericalDirectionAmount = .35f;

                    // set color over time 
                    colorOverLifeTime.enabled = true;

                    Gradient gradient = new Gradient();
                    GradientColorKey[] colors = new GradientColorKey[]
                    {
                        new GradientColorKey(new Color(1, 1, 1), 0),
                        new GradientColorKey(new Color(1, 1, 1), .8f)
                    };
                    GradientAlphaKey[] alphas = new GradientAlphaKey[]
                    {
                        new GradientAlphaKey(0, 0),
                        new GradientAlphaKey(1, .8f)
                    };
                    gradient.SetKeys(colors, alphas);
                    colorOverLifeTime.color = new ParticleSystem.MinMaxGradient(gradient);

                    // set rotation over life time
                    rotationOverLifeTime.enabled = true;

                    rotationOverLifeTime.zMultiplier = 90f * Mathf.PI / 180; // in radians

                    // set trails
                    trails.enabled = true;

                    gradient = new Gradient();
                    colors = new GradientColorKey[]
                    {
                        new GradientColorKey(new Color(1, 1, 1), .166f),
                        new GradientColorKey(new Color(1, 1, 1), 1)
                    };
                    alphas = new GradientAlphaKey[]
                    {
                        new GradientAlphaKey(1, .33f),
                        new GradientAlphaKey(.33f, 1)
                    };
                    gradient.SetKeys(colors, alphas);
                    trails.colorOverLifetime = new ParticleSystem.MinMaxGradient(gradient);

                    trails.widthOverTrail = new ParticleSystem.MinMaxCurve(.02f, new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0, -2, 0)));

                    gradient = new Gradient();
                    colors = new GradientColorKey[]{
                        new GradientColorKey(new Color(1, 1, 1), 0),
                        new GradientColorKey(new Color32(255, 134, 134, 255), .2f),
                        new GradientColorKey(new Color32(241, 228, 119, 255), .4f),
                        new GradientColorKey(new Color32(144, 248, 113, 255), .6f),
                        new GradientColorKey(new Color32(113, 159, 252, 255), .8f),
                        new GradientColorKey(new Color32(105, 245, 240, 255), 1)
                    };
                    alphas = new GradientAlphaKey[]
                    {
                        new GradientAlphaKey(1, 0),
                        new GradientAlphaKey(1, 1)
                    };
                    gradient.SetKeys(colors, alphas);
                    trails.colorOverTrail = new ParticleSystem.MinMaxGradient(gradient);

                    // set texture sheet animation
                    textureShitAnimation.enabled = true;

                    for (int i = 0; i < textureShitAnimation.spriteCount; i++)
                        textureShitAnimation.RemoveSprite(i); // remove all previous sprites

                    textureShitAnimation.mode = ParticleSystemAnimationMode.Sprites;
                    textureShitAnimation.AddSprite(Resources.Load<Sprite>("Textures/Particles/StarParticle"));
                } break;
            case ShipEffect.Fire:
                {
                    // set start settings
                    main.startLifetime = new ParticleSystem.MinMaxCurve(.6f, 1.2f);
                    main.startSpeed = 7f;
                    main.startSize = new ParticleSystem.MinMaxCurve(1.5f, 3f);
                    main.startColor = Color.white;
                    main.gravityModifier = -2f;
                    main.simulationSpeed = .4f;

                    // set emission
                    emission.enabled = true;

                    emission.rateOverTime = 75f;

                    // set shape
                    shape.enabled = true;

                    shape.shapeType = ParticleSystemShapeType.SingleSidedEdge;
                    shape.radius = .85f;
                    shape.radiusMode = ParticleSystemShapeMultiModeValue.Random;

                    shape.position = new Vector2(-0.052354f, 1.5398f);
                    shape.randomDirectionAmount = .1f;
                    shape.randomPositionAmount = .1f;

                    // set color over time 
                    colorOverLifeTime.enabled = true;

                    Gradient gradient = new Gradient();
                    GradientColorKey[] colors = new GradientColorKey[]
                    {
                        new GradientColorKey(new Color32(43, 8, 14, 255), 0),
                        new GradientColorKey(new Color32(137, 38, 37, 255), .33f),
                        new GradientColorKey(new Color32(255, 213, 0, 255), .63f),
                        new GradientColorKey(new Color32(79, 79, 79, 255), .83f),
                        new GradientColorKey(new Color32(79, 79, 79, 255), 1f)
                    };
                    GradientAlphaKey[] alphas = new GradientAlphaKey[]
                    {
                        new GradientAlphaKey(0, 0),
                        new GradientAlphaKey(125f/255f, .29f),
                        new GradientAlphaKey(50f/255f, .63f),
                        new GradientAlphaKey(50f/255f, .83f),
                        new GradientAlphaKey(0, 1)
                    };
                    gradient.SetKeys(colors, alphas);
                    colorOverLifeTime.color = new ParticleSystem.MinMaxGradient(gradient);

                    // set noise
                    noise.enabled = true;

                    noise.strength = .1f;
                    noise.frequency = 3f;
                    noise.scrollSpeed = 2f;

                    // set texture sheet animation
                    textureShitAnimation.enabled = true;

                    for (int i = 0; i < textureShitAnimation.spriteCount; i++)
                        textureShitAnimation.RemoveSprite(i); // remove all previous sprites

                    textureShitAnimation.mode = ParticleSystemAnimationMode.Sprites;
                    textureShitAnimation.AddSprite(Resources.Load<Sprite>("Textures/Particles/FireParticle"));
                } break;
            case ShipEffect.Snow:
                {
                    // set start settings
                    main.startLifetime = new ParticleSystem.MinMaxCurve(.45f, .55f);
                    main.startSpeed = new ParticleSystem.MinMaxCurve(1, 2.5f);
                    main.startSize = new ParticleSystem.MinMaxCurve(.25f, .6f);
                    main.startColor = new ParticleSystem.MinMaxGradient(new Color32(255, 255, 255, 100), new Color32(255, 255, 255, 200));
                    main.gravityModifier = 1f;
                    main.simulationSpeed = .5f;

                    // set emission
                    emission.enabled = true;

                    emission.rateOverTime = 30f;

                    // set shape
                    shape.enabled = true;

                    shape.shapeType = ParticleSystemShapeType.SingleSidedEdge;
                    shape.radius = 1.5f;
                    shape.radiusMode = ParticleSystemShapeMultiModeValue.Random;

                    // set color over time 
                    colorOverLifeTime.enabled = true;

                    Gradient gradient = new Gradient();
                    GradientColorKey[] colors = new GradientColorKey[]
                    {
                        new GradientColorKey(new Color(1, 1, 1), 0),
                        new GradientColorKey(new Color(1, 1, 1), .8f)
                    };
                    GradientAlphaKey[] alphas = new GradientAlphaKey[]
                    {
                        new GradientAlphaKey(0, 0),
                        new GradientAlphaKey(1, .8f)
                    };
                    gradient.SetKeys(colors, alphas);
                    colorOverLifeTime.color = new ParticleSystem.MinMaxGradient(gradient);

                    // set rotation over life time
                    rotationOverLifeTime.enabled = true;

                    rotationOverLifeTime.zMultiplier = 60f * Mathf.PI / 180; // in radians

                    // set texture sheet animation
                    textureShitAnimation.enabled = true;

                    for (int i = 0; i < textureShitAnimation.spriteCount; i++)
                        textureShitAnimation.RemoveSprite(i); // remove all previous sprites

                    textureShitAnimation.mode = ParticleSystemAnimationMode.Sprites;
                    textureShitAnimation.AddSprite(Resources.Load<Sprite>("Textures/Particles/SnowParticle"));
                } break;
            case ShipEffect.Smoke:
                {
                    // set start settings
                    main.startLifetime = new ParticleSystem.MinMaxCurve(.4f, .6f);
                    main.startSpeed = new ParticleSystem.MinMaxCurve(.2f, .4f);
                    main.startSize = new ParticleSystem.MinMaxCurve(.5f, 3f);
                    main.startColor = new ParticleSystem.MinMaxGradient(new Color32(53, 53, 53, 50), new Color32(61, 245, 78, 150));
                    main.gravityModifier = .3f;
                    main.simulationSpeed = .2f;

                    // set emission
                    emission.enabled = true;

                    emission.rateOverTime = 200f;

                    // set shape
                    shape.enabled = true;

                    shape.shapeType = ParticleSystemShapeType.Circle;
                    shape.radius = 1.5f;
                    shape.arc = 360f;
                    shape.radiusMode = ParticleSystemShapeMultiModeValue.Random;

                    shape.position = new Vector2(-0.11023f, 1.5185f);
                    shape.randomDirectionAmount = .25f;
                    shape.sphericalDirectionAmount = .35f;

                    // set color over time 
                    colorOverLifeTime.enabled = true;

                    Gradient gradient = new Gradient();
                    GradientColorKey[] colors = new GradientColorKey[]
                    {
                        new GradientColorKey(new Color(1, 1, 1), 0),
                        new GradientColorKey(new Color(1, 1, 1), .5f),
                        new GradientColorKey(new Color(1, 1, 1), 1),
                    };
                    GradientAlphaKey[] alphas = new GradientAlphaKey[]
                    {
                        new GradientAlphaKey(0, 0),
                        new GradientAlphaKey(1, 0.5f),
                        new GradientAlphaKey(0, 1)
                    };
                    gradient.SetKeys(colors, alphas);
                    colorOverLifeTime.color = new ParticleSystem.MinMaxGradient(gradient);

                    // set rotation over life time
                    rotationOverLifeTime.enabled = true;

                    AnimationCurve animCurve = new AnimationCurve(new Keyframe[]
                    {
                        new Keyframe(0, 1),
                        new Keyframe(.25f, -1),
                        new Keyframe(.5f, 1),
                        new Keyframe(.75f, -1),
                        new Keyframe(1, 1)

                    });
                    rotationOverLifeTime.z = new ParticleSystem.MinMaxCurve(180f * Mathf.PI / 180f, animCurve);

                    // set noise
                    noise.enabled = true;

                    noise.strength = .2f;
                    noise.frequency = 4f;
                    noise.scrollSpeed = 2f;

                    // set texture sheet animation
                    textureShitAnimation.enabled = true;

                    for (int i = 0; i < textureShitAnimation.spriteCount; i++)
                        textureShitAnimation.RemoveSprite(i); // remove all previous sprites

                    textureShitAnimation.mode = ParticleSystemAnimationMode.Sprites;
                    textureShitAnimation.AddSprite(Resources.Load<Sprite>("Textures/Particles/SmokeParticle"));
                } break;
        }
    }
    // hide bg effect(if exists)
    public void ToggleBgEffect(bool state)
    {
        if (state)
            bgEffect.Play();
        else
            bgEffect.Stop();
    }

    // loading particle system for bg ShipEffect
    private void LoadParticleSystem()
    {
        if (bgEffect != null)
            Destroy(bgEffect.gameObject);

        GameObject particleSystem = Instantiate(bgEffectPrefab, transform);
        bgEffect = particleSystem.GetComponent<ParticleSystem>();
    }
    // getting db slot id by name
    private int GetSlotId(string name)
    {
        return new Dictionary<string, int>
        {
            { "Black Flag", 0 },
            { "Apple Eater", 1 },
            { "Warm Heart", 2 },
            { "Magic Shield", 3 },
            { "Lightning", 4 },
            { "Best Friend", 5 },
            { "Your Horror", 6 },
            { "Angry Enemy", 7 },
            { "Ice Slayer", 8 },
            { "Skyscraper", 9 },
        }[name];
    }

    // dragon bones frame listener
    private void OnFrameEventHandler(string type, EventObject eventObject)
    {
        switch (eventObject.name)
        {
            // death
            case "LoosingStart":
                {
                    // hide bg effect
                    if (ship.isAnimated)
                        ToggleBgEffect(false);

                    // set animation speed to 1
                    component.animation.timeScale = 1;

                    // get slot id
                    int slotId = GetSlotId(ship.label);
                    if (ship.isAnimated)
                        slotId -= 5;
                    else
                        slotId = 0;

                    // set right and left ship body parts
                    component.armature.GetSlot("ShipBodyLeftPart").displayIndex = slotId;
                    component.armature.GetSlot("ShipBodyRightPart").displayIndex = slotId;
                } break;
            case "LoosingCrashing":
                {
                    // TODO: add particle system(blow)
                    GameObject.Find("LoosingEffect/Stars").GetComponent<ParticleSystem>().Play();
                    GameObject.Find("LoosingEffect/Blow").GetComponent<ParticleSystem>().Play();
                    GameObject.Find("LoosingEffect/Sparks").GetComponent<ParticleSystem>().Play();
                } break;
            case "LoosingCrashed":
                {
                    // TODO: add particle system(souls)
                    GameObject.Find("LoosingEffect/Souls").GetComponent<ParticleSystem>().Play();

                    // show defeat panel
                    GameObject.Find("MainPanel/DefeatPanel").GetComponent<PlayingFieldToggler>().TogglePanel();
                } break;
            case "LoosingEnd":
                {
                    // set animation speed back to initial
                    component.animation.timeScale = ShipController.instance.animVelocity;
                } break;

            // revive
            case "RevivingStart":
                {
                    // set animation speed to 1
                    component.animation.timeScale = 1f;

                    // get slot id
                    int slotId = GetSlotId(ship.label);
                    if (ship.isAnimated)
                        slotId -= 5;
                    else
                        slotId = 0;

                    // demo
                    component.armature.GetSlot("ShipBody").displayIndex = -1;

                    // set right and left ship body parts
                    component.armature.GetSlot("ShipBodyLeftPart").displayIndex = slotId;
                    component.armature.GetSlot("ShipBodyRightPart").displayIndex = slotId;
                } break;
            case "RevivingRestored":
                {
                    // set ship body texture
                    int slotId = GetSlotId(ship.label);
                    if (ship.isAnimated)
                        slotId -= 5;
                    else
                        slotId = 0;

                    // set ship body texture
                    component.armature.GetSlot("ShipBody").displayIndex = slotId;

                    // add effect(healing zone & chrest spread)
                    GameObject.Find("RevivingEffect/HealingZone").GetComponent<ParticleSystem>().Play();
                    GameObject.Find("RevivingEffect/ChrestsSpread").GetComponent<ParticleSystem>().Play();
                } break;
            case "RevivingHideShipBody":
                {
                    component.animation.timeScale = 1.25f;

                    // hide ship body parts textures
                    component.armature.GetSlot("ShipBodyLeftPart").displayIndex = -1;
                    component.armature.GetSlot("ShipBodyRightPart").displayIndex = -1;
                } break;
            case "RevivingWingsFlap":
                {
                    // set animation speed to 2(I like it)
                    component.animation.timeScale = 1.75f;

                    // TODO: add particle system(blow)
                    GameObject.Find("RevivingEffect/WingsBlow/WingBlowLeft").GetComponent<ParticleSystem>().Play();
                    GameObject.Find("RevivingEffect/WingsBlow/WingBlowRight").GetComponent<ParticleSystem>().Play();
                } break;
            case "RevivingEnd":
                {
                    if (ship.isAnimated)
                    {
                        bgEffect.Simulate(2);
                        bgEffect.Play();
                    }

                    // set animation speed back to initial
                    component.animation.timeScale = ShipController.instance.animVelocity;

                    // turn on Idle animation
                    component.animation.Play("Idle");

                    // show bg effect
                    ToggleBgEffect(true);

                    // get camera
                    Camera mainCamera = StateManager.instance.mainCamera;

                    // set alpha of score panel to 1
                    iTween.ValueTo(gameObject, iTween.Hash(
                        "from", 0,
                        "to", 1f,
                        "time", 1.5f,
                        "easetype", iTween.EaseType.easeInOutQuad,
                        "ignoretimescale", true,
                        "onupdate", "UpdateScorePanelAlpha"
                    ));

                    // scale camera orthographic size
                    iTween.ValueTo(gameObject, iTween.Hash(
                        "from", mainCamera.orthographicSize,
                        "to", 5f,
                        "time", 1.5f,
                        "easetype", iTween.EaseType.easeInOutQuad,
                        "ignoretimescale", true,
                        "onupdate", "UpdateCameraOrthographicSize"
                    ));

                    // move camera to ship position
                    float yCameraPos = ShipController.instance.shipPanel.position.y;
                    Vector3 cameraPosition = new Vector3(0f, 0, mainCamera.transform.position.z);
                    iTween.MoveTo(mainCamera.gameObject, iTween.Hash(
                        "position", cameraPosition,
                        "time", 1.5f,
                        "ignoretimescale", true,
                        "easetype", iTween.EaseType.easeInOutQuad
                    ));

                    // move ship to the center of camera
                    iTween.MoveTo(ShipController.instance.shipPanel.gameObject, iTween.Hash(
                        "x", ShipController.instance.xShipStartPos,
                        "time", 1.5f,
                        "ignoretimescale", true,
                        "easetype", iTween.EaseType.easeInOutQuad
                    ));

                    // toggle game state with counting
                    CounterManager.instance.Count();
                    //GameObject.Find("MainPanel/CounterPanel").GetComponent<Animator>().SetTrigger("Counting");

                    // <--- some bug with Invoke() --->
                    // if timeBeforeInvoke == 3 then method will be invoked in 3 secs after pause not right now
                    // if timeBeforeInvoke == 0 then method will be invoked right now, not in 3 seconds from now
                    // then I just invoke it in a very little period of time to get what I need(0.02f is about zero but it's not)
                    // it's interesting buy .02 is about delta time(MAYBE because of pause and 1 frame = delta time it happens)
                    // and now it works fine
                    // <--- ### --->
                    // continue normal update mode of dragon bones armature component
                    GameObject.Find("ShipPanel").GetComponent<DragonBonesUpdateMode>().Invoke("ToggleUpdateMode", .02f);
                } break;
        } // switch
    }
    // dragon bones sound listener
    private void OnSoundEventHandler(string type, EventObject eventObject)
    {
        switch (eventObject.name)
        {
            // loosing
            case "LoosingShipCrashing":
                {
                    // load and play cracking sound
                    AudioClip clip = Resources.Load<AudioClip>("Sounds/ship_crack1");
                    SoundManager.instance.PlaySingle(clip);
                } break;
            case "LoosingMastFalling":
                {
                    // load and play short falling sound
                    AudioClip clip = Resources.Load<AudioClip>("Sounds/fall1");
                    SoundManager.instance.PlaySingle(clip);
                } break;

            // reviving
            case "RevivingMastFallingReverse":
                {
                    // load and play reverse falling sound
                    AudioClip clip = Resources.Load<AudioClip>("Sounds/fall1_reversed");
                    SoundManager.instance.PlaySingle(clip);
                } break;
            case "RevivingCrashingReverse":
                {
                    // load and play reverse crashing sound
                    AudioClip clip = Resources.Load<AudioClip>("Sounds/ship_crack1_reversed");
                    SoundManager.instance.PlaySingle(clip);
                } break;
            case "RevivingHealingZone":
                {
                    // load and play reviving sound
                    AudioClip clip = Resources.Load<AudioClip>("Sounds/ship_reviving1");
                    SoundManager.instance.PlaySingle(clip);
                } break;

        } // switch
    }
}
