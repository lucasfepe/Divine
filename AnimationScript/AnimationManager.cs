using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance { get; private set; }

    [SerializeField] private GameObject blackHole;
    [SerializeField] private GameObject neutronStar;
    [SerializeField] private GameObject whiteDwarfPrefab;
    [SerializeField] private ParticleSystem supernovaParticleSystem;
    [SerializeField] private Transform playerStardustBank;
    [SerializeField] private Transform opponentStardustBank;
    [SerializeField] private StatTextMessageScriptUI statMessagePrefab;
    private GameObject mainCanvas;
    private GameObject inspectUI;
    private GameObject endTurnUI;


    private void Awake()
    {
        mainCanvas = GameObject.Find("MainCanvas");
        inspectUI = GameObject.Find("InspectCardUI");
        endTurnUI = GameObject.Find("EndTurnUI");
        Instance = this;
    }
    
    //ugly um calling too many methods and passing parameters
    public void Supernova(Transform parent, int stardustGain, bool amOwner, bool isBlackHole = false, bool intoThinAir = false)
    {
        
        if (!isBlackHole)
        {
            GameObject neutronStar;
            neutronStar = Instantiate(this.neutronStar, mainCanvas.transform);
            NeutronStarAnimator neutronStarAnimator = neutronStar.GetComponent<NeutronStarAnimator>();
            neutronStarAnimator.SetOwner(amOwner);
            int insertIndex = endTurnUI.transform.GetSiblingIndex() + 1;
            neutronStar.transform.SetSiblingIndex(insertIndex);
            neutronStar.transform.position = new Vector2(parent.transform.position.x, parent.transform.position.y);
            neutronStarAnimator.SetDestination();
        }
        else
        {
            if (!intoThinAir) { 
            GameObject blackHole;
            blackHole = Instantiate(this.blackHole, mainCanvas.transform);
            blackHole.GetComponent<BlackHoleAnimator>().SetOwner(amOwner);
            int insertIndex = endTurnUI.transform.GetSiblingIndex() + 1;
            blackHole.transform.SetSiblingIndex(insertIndex);
            blackHole.transform.position = new Vector2(parent.transform.position.x, parent.transform.position.y);
            float blackHoleZOffset = -2000;
            blackHole.transform.localPosition = new Vector3(blackHole.transform.localPosition.x, blackHole.transform.localPosition.y, blackHoleZOffset);
            blackHole.GetComponent<BlackHoleAnimator>().SetDestination();
            }
        }

        float particleCount = Mathf.Pow(stardustGain,2.2f) + 20 * stardustGain;
        ParticleSystem particleSystemTemplate = supernovaParticleSystem.GetComponent<ParticleSystem>();

        ParticleSystem.Burst burst = particleSystemTemplate.emission.GetBurst(0);
        burst.count = particleCount;

        particleSystemTemplate.emission.SetBurst(0, burst);

        ParticleSystem particleSystemCreated = Instantiate(supernovaParticleSystem, parent);

        string message = "";
        Sprite icon = null;
        icon = Resources.Load<Sprite>("stardustIcon");

        if (stardustGain > 0)
        {
            message = "+" + stardustGain;
        }else if(stardustGain < 0)
        {
            message = stardustGain.ToString();
        }

        if(message != "") {
            float delay = 1f;
            float yoffset = 1f;
            float xoffset = 0;
        StartCoroutine(StatMessage(parent.transform, message, icon, delay, xoffset,yoffset));
            StartCoroutine(GainStardustCoroutine(stardustGain, delay));
        }


        //ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = particleSystemCreated.velocityOverLifetime;
        //float wait = 2;

        //DOTween.To(() => 1, x => x = 1, 0, wait).OnComplete(() =>
        //{

        //    if (amOwner) {
        //    velocityOverLifetime.orbitalOffsetX = playerStardustBank.position.x - parent.position.x;
        //    velocityOverLifetime.orbitalOffsetY = playerStardustBank.position.y - parent.position.y;
        //        velocityOverLifetime.radial = -10f;
        //    }
        //    else
        //    {
        //        velocityOverLifetime.orbitalOffsetX = opponentStardustBank.position.x - parent.position.x;
        //        velocityOverLifetime.orbitalOffsetY = opponentStardustBank.position.y - parent.position.y;
        //        velocityOverLifetime.radial = -10f;
        //    }


        //});
    }

    public IEnumerator GainStardustCoroutine(int stardustGain, float wait)
    {
        yield return new WaitForSeconds(wait);
        CardGameManager.Instance.AddToTempStardustIncrement(stardustGain);
        CardGameManager.Instance.CardSupernova();
        
    }

    public IEnumerator StatMessage(Transform parent, string message, Sprite icon, float wait, float xoffset = 0, float yoffset = 0)
    {
        int inspectUISiblingIdx = inspectUI.transform.GetSiblingIndex();
        yield return new WaitForSeconds(wait);
        StatTextMessageScriptUI statTextMessage = Instantiate(statMessagePrefab, mainCanvas.transform);
        statTextMessage.transform.SetSiblingIndex(inspectUISiblingIdx);
        statTextMessage.SetMessage(message);
        statTextMessage.SetImage(icon);
        statTextMessage.transform.position = new Vector3(parent.transform.position.x + xoffset, parent.transform.position.y + yoffset, 0);
        statTextMessage.transform.localPosition = new Vector3(statTextMessage.transform.localPosition.x, statTextMessage.transform.localPosition.y, 0);

        //Canvas canvas = statTextMessage.GetComponentInChildren<Canvas>();
        //RectTransform rectTransform = canvas.GetComponent<RectTransform>();
        //rectTransform.position = new Vector2(parent.position.x, parent.position.y + yoffset);
    }

    public void WhiteDwarfAppear(Transform parent)
    {
        GameObject whiteDwarf = Instantiate(whiteDwarfPrefab,mainCanvas.transform);
        int inspectUISiblingIdx = inspectUI.transform.GetSiblingIndex();
        whiteDwarf.transform.SetSiblingIndex(inspectUISiblingIdx);
        float yoffset = .2f;
        whiteDwarf.transform.position = new Vector2(parent.transform.position.x, parent.transform.position.y + yoffset);
        whiteDwarf.transform.localPosition = new Vector3(whiteDwarf.transform.localPosition.x, whiteDwarf.transform.localPosition.y, -5000);
        WhiteDwarfAnimator whiteDwarfAnimator = whiteDwarf.GetComponent<WhiteDwarfAnimator>();
        whiteDwarfAnimator.SetCard(parent.GetComponent<BaseCard>());
        whiteDwarfAnimator.SetDestination();


    }
}
