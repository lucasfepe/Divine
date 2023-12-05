using UnityEngine;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Amazon.DynamoDBv2.Model;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices.ComTypes;

// This script just demonstrates we can use credentials from an authenticated user to call AWS APIs
public class LambdaManager : MonoBehaviour
{

    public static LambdaManager Instance { get; private set; }

    private AuthenticationManager _authenticationManager;
    private const string GET_PLAYER_DECKS = "newprojectLambdaVSC-GetPlayerDecksActualFunction-1c6ZLpOOYscB";
    private const string GET_PLAYER_ACTIVE_DECK = "newprojectLambdaVSC-GetPlayerActiveDeckFunction-Aq21LST4ouFb";
    private const string GET_PLAYER_CARDS = "newprojectLambdaVSC-GetPlayerDecksFunction-cNFxCtJD37T8";
    private const string DELETE_PLAYER_DECK = "newprojectLambdaVSC-DeletePlayerDeckFunction-9Dd05uycqd6j";
    private const string UPDATE_ACTIVE_DECK = "newprojectLambdaVSC-SetPlayerActiveDeckFunction-9v7lGNDEilvG";
    private const string DELETE_ACTIVE_DECK = "newprojectLambdaVSC-DeletePlayerActiveDeckFunction-cOVXJdfQn7VW";
    private const string GET_PLAYER_SKILL = "newprojectLambdaVSC-GetPlayerSkillFunction-ZbM2XqCAMI9b";
    private const string LOOSE_MATCH = "newprojectLambdaVSC-LooseMatchFunction-n8SxZvrhnIwo";
    private const string WIN_MATCH = "newprojectLambdaVSC-WinMatchFunction-bfIUcStAmAV7";



    public async Task<string> GetPlayerDecksLambda()
    {

        AmazonLambdaClient amazonLambdaClient = new AmazonLambdaClient(_authenticationManager.GetCredentials(), AuthenticationManager.Region);
        InvokeRequest invokeRequest = new InvokeRequest
        {
            FunctionName = GET_PLAYER_DECKS,
            InvocationType = InvocationType.RequestResponse,
            Payload = "{\"identity_token\":\"" + _authenticationManager.GetIdentityToken() + "\"}"
        };
        try
        {
            InvokeResponse response = await amazonLambdaClient.InvokeAsync(invokeRequest);
            if (response.StatusCode == 200)
            {

                // demonstrate we can get the users ID for use in our game
                string userId = _authenticationManager.GetUsersId();
                string responseJson = Encoding.ASCII.GetString(response.Payload.ToArray()).Replace("\\\"", "\"").Remove(0, 1);
                responseJson = responseJson.Remove(responseJson.Length - 1, 1);
                return responseJson;
            }
        }
        catch (Exception ex) { Debug.Log(ex); return "error"; }
        return null;
    }

    public async Task<string> GetPlayerActiveDeckLambda()
    {

        AmazonLambdaClient amazonLambdaClient = new AmazonLambdaClient(_authenticationManager.GetCredentials(), AuthenticationManager.Region);
        InvokeRequest invokeRequest = new InvokeRequest
        {
            FunctionName = GET_PLAYER_ACTIVE_DECK,
            InvocationType = InvocationType.RequestResponse,
            Payload = "{\"identity_token\":\"" + _authenticationManager.GetIdentityToken() + "\"}"
        };
        try
        {
            InvokeResponse response = await amazonLambdaClient.InvokeAsync(invokeRequest);
            if (response.StatusCode == 200)
            {

                // demonstrate we can get the users ID for use in our game
                string userId = _authenticationManager.GetUsersId();
                string responseJson = Encoding.ASCII.GetString(response.Payload.ToArray()).Replace("\\\"", "\"").Remove(0, 1);
                responseJson = responseJson.Remove(responseJson.Length - 1, 1);
                return responseJson;
            }
        }
        catch (Exception ex) { Debug.Log(ex); return "error"; }
        return null;
    }

    public async Task<int> GetPlayerSkillLambda()
    {

        AmazonLambdaClient amazonLambdaClient = new AmazonLambdaClient(_authenticationManager.GetCredentials(), AuthenticationManager.Region);
        InvokeRequest invokeRequest = new InvokeRequest
        {
            FunctionName = GET_PLAYER_SKILL,
            InvocationType = InvocationType.RequestResponse,
            Payload = "{\"identity_token\":\"" + _authenticationManager.GetIdentityToken() + "\"}"
        };
        try
        {
            InvokeResponse response = await amazonLambdaClient.InvokeAsync(invokeRequest);
            if (response.StatusCode == 200)
            {

                // demonstrate we can get the users ID for use in our game
                string userId = _authenticationManager.GetUsersId();
                string responseJson = Encoding.ASCII.GetString(response.Payload.ToArray());
                
                return Int32.Parse(responseJson);
            }
        }
        catch (Exception ex) { Debug.Log(ex); return -999; }
        return -999;
    }

    public async Task<string> GetPlayerCardsLambda()
    {


        AmazonLambdaClient amazonLambdaClient = new AmazonLambdaClient(_authenticationManager.GetCredentials(), AuthenticationManager.Region);

        InvokeRequest invokeRequest = new InvokeRequest
        {
            FunctionName = GET_PLAYER_CARDS,
            InvocationType = InvocationType.RequestResponse,
            Payload = "{\"identity_token\":\"" + _authenticationManager.GetIdentityToken() + "\"}"
        };
        try { 
        InvokeResponse response = await amazonLambdaClient.InvokeAsync(invokeRequest);

        if (response.StatusCode == 200)
        {

            // demonstrate we can get the users ID for use in our game
            string userId = _authenticationManager.GetUsersId();
            string responseJson = Encoding.ASCII.GetString(response.Payload.ToArray()).Replace("\\\"", "\"").Remove(0,1);
                responseJson = responseJson.Remove(responseJson.Length - 1, 1);
            return responseJson;
            

        }
        }catch(Exception ex) { Debug.Log(ex);return "error"; }
        return null;

    }

    public async Task<bool> DeletePlayerDeckLambda(string decksJson, bool isActiveDeck)
    {
        string idToken = _authenticationManager.GetIdentityToken();
        AmazonLambdaClient amazonLambdaClient = new AmazonLambdaClient(_authenticationManager.GetCredentials(), AuthenticationManager.Region);
        Debug.Log("decks: " + decksJson.Replace("\"", "\\\""));
        InvokeRequest invokeRequest = new InvokeRequest
        {
            FunctionName = DELETE_PLAYER_DECK,
            InvocationType = InvocationType.RequestResponse,
            Payload = 
            "{\"identity_token\":\"" + idToken + "\"," +
            "\"decks\":\"" + decksJson.Replace("\"","\\\"") + "\"}"
        };
        try
        {
            InvokeResponse response = await amazonLambdaClient.InvokeAsync(invokeRequest);

            
        }
        catch (InvalidRequestContentException ex) { Debug.Log(ex); Debug.Log(ex.Message);Debug.Log(ex.InnerException); return false; }

        if (isActiveDeck)
        {
           await DeleteActiveDeckLambda();
        }
        
            return true;
        
    }

    public async Task<bool> WinMatchLambda()
    {
        string idToken = _authenticationManager.GetIdentityToken();
        AmazonLambdaClient amazonLambdaClient = new AmazonLambdaClient(_authenticationManager.GetCredentials(), AuthenticationManager.Region);
        InvokeRequest invokeRequest = new InvokeRequest
        {
            FunctionName = WIN_MATCH,
            InvocationType = InvocationType.RequestResponse,
            Payload =
            "{\"identity_token\":\"" + idToken + "\"}"
        };
        try
        {
            InvokeResponse response = await amazonLambdaClient.InvokeAsync(invokeRequest);


        }
        catch (InvalidRequestContentException ex) { Debug.Log(ex); Debug.Log(ex.Message); Debug.Log(ex.InnerException); return false; }

        

        return true;

    }
    public async Task<bool> LooseMatchLambda()
    {
        string idToken = _authenticationManager.GetIdentityToken();
        AmazonLambdaClient amazonLambdaClient = new AmazonLambdaClient(_authenticationManager.GetCredentials(), AuthenticationManager.Region);
        InvokeRequest invokeRequest = new InvokeRequest
        {
            FunctionName = LOOSE_MATCH,
            InvocationType = InvocationType.RequestResponse,
            Payload =
            "{\"identity_token\":\"" + idToken + "\"}"
        };
        try
        {
            InvokeResponse response = await amazonLambdaClient.InvokeAsync(invokeRequest);


        }
        catch (InvalidRequestContentException ex) { Debug.Log(ex); Debug.Log(ex.Message); Debug.Log(ex.InnerException); return false; }



        return true;

    }

    public async Task<bool> SavePlayerDeckLambda(string decksJson)
    {
        string idToken = _authenticationManager.GetIdentityToken();
        AmazonLambdaClient amazonLambdaClient = new AmazonLambdaClient(_authenticationManager.GetCredentials(), AuthenticationManager.Region);
        InvokeRequest invokeRequest = new InvokeRequest
        {
            FunctionName = DELETE_PLAYER_DECK,
            InvocationType = InvocationType.RequestResponse,
            Payload =
            "{\"identity_token\":\"" + idToken + "\"," +
            "\"decks\":\"" + decksJson.Replace("\"","\\\"") + "\"}"
        };
        try
        {
            InvokeResponse response = await amazonLambdaClient.InvokeAsync(invokeRequest);
            if (response.StatusCode == 200)
            {
                // demonstrate we can get the users ID for use in our game
                return true;
            }
        }
        catch (Exception ex) { Debug.Log(ex); return false; }
        return false;
    }


    
    public async Task<bool> UpdateActiveDeckLambda(string newActiveDeck)
    {
        string idToken = _authenticationManager.GetIdentityToken();
        AmazonLambdaClient amazonLambdaClient = new AmazonLambdaClient(_authenticationManager.GetCredentials(), AuthenticationManager.Region);
        InvokeRequest invokeRequest = new InvokeRequest
        {
            FunctionName = UPDATE_ACTIVE_DECK,
            InvocationType = InvocationType.RequestResponse,
            Payload =
            "{\"identity_token\":\"" + idToken + "\"," +
            "\"activeDeck\":\"" + newActiveDeck + "\"}"
        };
        try
        {
            InvokeResponse response = await amazonLambdaClient.InvokeAsync(invokeRequest);
            if (response.StatusCode == 200)
            {
                // demonstrate we can get the users ID for use in our game
                return true;
            }
        }
        catch (Exception ex) { Debug.Log(ex); return false; }
        return false;
    }
    public async Task<bool> DeleteActiveDeckLambda()
    {
        AmazonLambdaClient amazonLambdaClient = new AmazonLambdaClient(_authenticationManager.GetCredentials(), AuthenticationManager.Region);
        InvokeRequest invokeRequest = new InvokeRequest
        {
            FunctionName = DELETE_ACTIVE_DECK,
            InvocationType = InvocationType.RequestResponse,
            Payload = "{\"identity_token\":\"" + _authenticationManager.GetIdentityToken() + "\"}"
        };
        try
        {
            InvokeResponse response = await amazonLambdaClient.InvokeAsync(invokeRequest);
            if (response.StatusCode == 200)
            {
                // demonstrate we can get the users ID for use in our game
                return true;
            }
        }
        catch (Exception ex) { Debug.Log(ex); return false; }
        return false;
    }
    private void onMainMenuClick()
    {
        SceneManager.LoadScene("LoginScene");
    }

    void Awake()
    {
#if DEDICATED_SERVER
        Debug.Log("DEDICATED_SERVER 6.8 lambda");

#endif
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _authenticationManager = FindObjectOfType<AuthenticationManager>();
    }

    void Start()
    {
       
    }
}