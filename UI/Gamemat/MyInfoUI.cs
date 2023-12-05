using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MyInfoUI : MonoBehaviour
{
    public static MyInfoUI Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI username;
    [SerializeField] private TextMeshProUGUI city;
    [SerializeField] private TextMeshProUGUI country;
    [SerializeField] private TextMeshProUGUI skill;
    private AuthenticationManager authenticationManager;

    private async void Awake()
    {
        Instance = this;
        authenticationManager = FindObjectOfType<AuthenticationManager>();
        username.text = authenticationManager.GetUsername();
        int skillInt = await LambdaManager.Instance.GetPlayerSkillLambda();
        skill.text = "skill: " + skillInt.ToString();
    }
    
}
