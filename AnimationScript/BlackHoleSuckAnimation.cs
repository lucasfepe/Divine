using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlackHoleSuckAnimation : MonoBehaviour
{

    [SerializeField] private GameObject blackHolePrefab;

    private GameObject mainCanvas;
    private GameObject endTurnUI;

    private int zOffset = -3000;
    private static int offsetIdx = 0;
    private int offset = 250;

    private void Awake()
    {
        mainCanvas = GameObject.Find("MainCanvas");
        endTurnUI = GameObject.Find("EndTurnUI");
    }
    private void Start()
    {
        CardGameManager.Instance.OnBeginTurn += CardGameManage_OnBeginTurn;
    }

    private void CardGameManage_OnBeginTurn(object sender, System.EventArgs e)
    {
        offsetIdx = 0;
    }

    public void Play()
    {
        GameObject blackHole = Instantiate(blackHolePrefab, mainCanvas.transform);

        float blackHoleZOffset = zOffset - offset * offsetIdx++;
        BlackHoleAnimator blackHoleAnimator = blackHole.GetComponent<BlackHoleAnimator>();
        blackHoleAnimator.MakeStationary();
        blackHoleAnimator.SetCard(GetComponent<BaseCard>());
        float yOffsetToCentre = .2f;

        int insertIdx = endTurnUI.transform.GetSiblingIndex() + 1;
        blackHole.transform.SetSiblingIndex(insertIdx);

        blackHole.transform.position = new Vector3(transform.position.x, transform.position.y + yOffsetToCentre, 0);
        blackHole.transform.localPosition = new Vector3(blackHole.transform.localPosition.x, blackHole.transform.localPosition.y, blackHoleZOffset);
    }

    public void Consume()
    {
        GameObject blackHole = Instantiate(blackHolePrefab, mainCanvas.transform);
        BlackHoleAnimator blackHoleAnimator = blackHole.GetComponent<BlackHoleAnimator>();
        float blackHoleZOffset = zOffset - offset * offsetIdx++;
        blackHoleAnimator.JustKilled();
        blackHoleAnimator.SetCard(GetComponent<BaseCard>());
        float yOffsetToCentre = .2f;

        int insertIdx = endTurnUI.transform.GetSiblingIndex() + 1;
        blackHole.transform.SetSiblingIndex(insertIdx);

        blackHole.transform.position = new Vector3(transform.position.x, transform.position.y + yOffsetToCentre, 0);
        blackHole.transform.localPosition = new Vector3(blackHole.transform.localPosition.x, blackHole.transform.localPosition.y, blackHoleZOffset);

    }
}
