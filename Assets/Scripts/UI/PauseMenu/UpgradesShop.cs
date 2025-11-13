using UnityEngine;
using TMPro;
public class UpgradesShop : MonoBehaviour
{
    public static bool constantRhythm, fastRhytm, lifeGrowth1, lifeGrowth2, lifeGrowth3, push, tackle, chargedPunch, hook, lifeRecovery, knee, power,
        fullAmmunition, throwingKnife, throwingGlass, reuseBeer, drunkenCombat;
    private static int intuition;
    public UpgradesShop instance;
    [SerializeField] private TMP_Text intuitionTMP;
    private GameObject UpgradesZone;
    private TMP_Text constantRhythmTMP, fastRhytmTMP, lifeGrowth1TMP, lifeGrowth2TMP, lifeGrowth3TMP, pushTMP, tackleTMP, chargedPunchTMP, hookTMP, lifeRecoveryTMP, kneeTMP, powerTMP,
        fullAmmunitionTMP, throwingKnifeTMP, throwingGlassTMP, reuseBeerTMP, drunkenCombatTMP;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        intuition = GameData.intuition > 0 ? GameData.intuition : 10000;
        ChangeIntuitionTMP();
        TMPS();
    }

    void Update()
    {
        if (intuitionTMP.gameObject.activeInHierarchy)
        {
            ChangeIntuitionTMP();
        }
    }
    public void ChangeIntuition(int intu)
    {
        intuition += intu;
    }
    public void ChangeIntuitionTMP()
    {
        intuitionTMP.text = $"Intuition: {intuition}";
    }
    private void TMPS()
    {
        UpgradesZone = transform.Find("PausePanel").Find("ShopMenu").Find("UpgradesPanel").Find("UpgradesZone").gameObject;
        constantRhythmTMP = UpgradesZone.transform.Find("ConstantRythm").Find("UpgradeTMP").Find("BuyButton").Find("BuyTMP").GetComponent<TMP_Text>();
        fastRhytmTMP = UpgradesZone.transform.Find("FastRhytm").Find("UpgradeTMP").Find("BuyButton").Find("BuyTMP").GetComponent<TMP_Text>();
        lifeGrowth1TMP = UpgradesZone.transform.Find("LifeGrowth1").Find("UpgradeTMP").Find("BuyButton").Find("BuyTMP").GetComponent<TMP_Text>();
        lifeGrowth2TMP = UpgradesZone.transform.Find("LifeGrowth2").Find("UpgradeTMP").Find("BuyButton").Find("BuyTMP").GetComponent<TMP_Text>();
        lifeGrowth3TMP = UpgradesZone.transform.Find("LifeGrowth3").Find("UpgradeTMP").Find("BuyButton").Find("BuyTMP").GetComponent<TMP_Text>();
        pushTMP = UpgradesZone.transform.Find("Push").Find("UpgradeTMP").Find("BuyButton").Find("BuyTMP").GetComponent<TMP_Text>();
        tackleTMP = UpgradesZone.transform.Find("Tackle").Find("UpgradeTMP").Find("BuyButton").Find("BuyTMP").GetComponent<TMP_Text>();
        chargedPunchTMP = UpgradesZone.transform.Find("ChargedPunch").Find("UpgradeTMP").Find("BuyButton").Find("BuyTMP").GetComponent<TMP_Text>();
        hookTMP = UpgradesZone.transform.Find("Hook").Find("UpgradeTMP").Find("BuyButton").Find("BuyTMP").GetComponent<TMP_Text>();
        lifeRecoveryTMP = UpgradesZone.transform.Find("LifeRecovery").Find("UpgradeTMP").Find("BuyButton").Find("BuyTMP").GetComponent<TMP_Text>();
        kneeTMP = UpgradesZone.transform.Find("Knee").Find("UpgradeTMP").Find("BuyButton").Find("BuyTMP").GetComponent<TMP_Text>();
        powerTMP = UpgradesZone.transform.Find("Power").Find("UpgradeTMP").Find("BuyButton").Find("BuyTMP").GetComponent<TMP_Text>();
        fullAmmunitionTMP = UpgradesZone.transform.Find("FullAmmunition").Find("UpgradeTMP").Find("BuyButton").Find("BuyTMP").GetComponent<TMP_Text>();
        throwingKnifeTMP = UpgradesZone.transform.Find("ThrowingKnife").Find("UpgradeTMP").Find("BuyButton").Find("BuyTMP").GetComponent<TMP_Text>();
        throwingGlassTMP = UpgradesZone.transform.Find("ThrowingGlass").Find("UpgradeTMP").Find("BuyButton").Find("BuyTMP").GetComponent<TMP_Text>();
        reuseBeerTMP = UpgradesZone.transform.Find("ReuseBeer").Find("UpgradeTMP").Find("BuyButton").Find("BuyTMP").GetComponent<TMP_Text>();
        drunkenCombatTMP = UpgradesZone.transform.Find("DrunkenCombat").Find("UpgradeTMP").Find("BuyButton").Find("BuyTMP").GetComponent<TMP_Text>();
    }
    public void ChangeBuyButtonTMP()
    {
        if (constantRhythm)
        {
            constantRhythmTMP.text = "Already buyed";
        }
        if (fastRhytm)
        {
            fastRhytmTMP.text = "Already buyed";
        }
        if (lifeGrowth1)
        {
            lifeGrowth1TMP.text = "Already buyed";
        }
        if (lifeGrowth2)
        {
            lifeGrowth2TMP.text = "Already buyed";
        }
        if (lifeGrowth3)
        {
            lifeGrowth3TMP.text = "Already buyed";
        }
        if (push)
        {
            pushTMP.text = "Already buyed";
        }
        if (tackle)
        {
            tackleTMP.text = "Already buyed";
        }
        if (chargedPunch)
        {
            chargedPunchTMP.text = "Already buyed";
        }
        if (hook)
        {
            hookTMP.text = "Already buyed";
        }
        if (lifeRecovery)
        {
            lifeRecoveryTMP.text = "Already buyed";
        }
        if (knee)
        {
            kneeTMP.text = "Already buyed";
        }
        if (power)
        {
            powerTMP.text = "Already buyed";
        }
        if (fullAmmunition)
        {
            fullAmmunitionTMP.text = "Already buyed";
        }
        if (throwingKnife)
        {
            throwingKnifeTMP.text = "Already buyed";
        }
        if (throwingGlass)
        {
            throwingGlassTMP.text = "Already buyed";
        }
        if (reuseBeer)
        {
            reuseBeerTMP.text = "Already buyed";
        }
        if (drunkenCombat)
        {
            drunkenCombatTMP.text = "Already buyed";
        }
    }
        public void ConstantRythmBuy()
    {
        if(intuition >= 10 && !constantRhythm)
        {
            ChangeIntuition(-10);
            constantRhythm = true;
            ChangeBuyButtonTMP();
        }
    }
    public void FastRhytmBuy()
    {
        if(intuition >= 15 && !fastRhytm)
        {
            ChangeIntuition(-15);
            fastRhytm = true;
            ChangeBuyButtonTMP();
        }
    }
    public void LifeGrowth1Buy()
    {
        if(intuition >= 10 && !lifeGrowth1)
        {
            ChangeIntuition(-10);
            lifeGrowth1 = true;
            ChangeBuyButtonTMP();
        }
    }
    public void LifeGrowth2Buy()
    {
        if(intuition >= 15 && lifeGrowth1 && !lifeGrowth2)
        {
            ChangeIntuition(-15);
            lifeGrowth2 = true;
            ChangeBuyButtonTMP();
        }
    }
    public void LifeGrowth3Buy()
    {
        if(intuition >= 25 && lifeGrowth2 && !lifeGrowth3)
        {
            ChangeIntuition(-25);
            lifeGrowth3 = true;
            ChangeBuyButtonTMP();
        }
    }
    public void PushBuy()
    {
        if(intuition >= 10 && !push)
        {
            ChangeIntuition(-10);
            push = true;
            ChangeBuyButtonTMP();
        }
    }
    public void TackleBuy()
    {
        if(intuition >= 15 && !tackle)
        {
            ChangeIntuition(-15);
            tackle = true;
            ChangeBuyButtonTMP();
        }
    }
    public void ChargedPunchBuy()
    {
        if(intuition >= 10 && !chargedPunch)
        {
            ChangeIntuition(-10);
            chargedPunch = true;
            ChangeBuyButtonTMP();
        }
    }
    public void HookBuy()
    {
        if(intuition >= 10 && !hook)
        {
            ChangeIntuition(-10);
            hook = true;
            ChangeBuyButtonTMP();
        }
    }
    public void LifeRecoveryBuy()
    {
        if(intuition >= 25 && !lifeRecovery)
        {
            ChangeIntuition(-25);
            lifeRecovery = true;
            ChangeBuyButtonTMP();
        }
    }
    public void KneeBuy()
    {
        if(intuition >= 10 && !knee)
        {
            ChangeIntuition(-10);
            knee = true;
            ChangeBuyButtonTMP();
        }
    }
    public void PowerBuy()
    {
        if(intuition >= 25 && !power)
        {
            ChangeIntuition(-25);
            power = true;
            ChangeBuyButtonTMP();
        }
    }
    public void FullAmmunitionBuy()
    {
        if(intuition >= 25 && !fullAmmunition)
        {
            ChangeIntuition(-25);
            fullAmmunition = true;
            ChangeBuyButtonTMP();
        }
    }
    public void ThrowingKnifeBuy()
    {
        if(intuition >= 15 && !throwingKnife)
        {
            ChangeIntuition(-15);
            throwingKnife = true;
            ChangeBuyButtonTMP();
        }
    }
    public void ThrowingGlassBuy()
    {
        if(intuition >= 10 && !throwingGlass)
        {
            ChangeIntuition(-10);
            throwingGlass = true;
            ChangeBuyButtonTMP();
        }
    }
    public void ReuseBeerBuy()
    {
        if(intuition >= 15 && !reuseBeer)
        {
            ChangeIntuition(-15);
            reuseBeer = true;
            ChangeBuyButtonTMP();
        }
    }
    public void DrunkenCombatBuy()
    {
        if(intuition >= 25 && !drunkenCombat)
        {
            ChangeIntuition(-25);
            drunkenCombat = true;
            ChangeBuyButtonTMP();
        }
    }
}
