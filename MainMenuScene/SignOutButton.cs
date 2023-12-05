using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class SignOutButton : MonoBehaviour
{

    private AuthenticationManager authenticationManager;
    private void Awake()
    {
        authenticationManager = FindObjectOfType<AuthenticationManager>();
        GetComponent<Button>().onClick.AddListener(() =>
        {
            authenticationManager.SignOut();
            SceneLoader.Load(SceneLoader.Scene.HomeScene);
        });
    }
}
