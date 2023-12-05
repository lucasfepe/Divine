using System;
using System.Collections;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class UsernameText : MonoBehaviour
{
    private AuthenticationManager authenticationManager;
    private void Awake()
    {
        authenticationManager = FindObjectOfType<AuthenticationManager>();
#if !DEDICATED_SERVER
       
        GetComponent<TextMeshProUGUI>().text = "Welcome " + authenticationManager.GetUsername();
#endif
    }
}
