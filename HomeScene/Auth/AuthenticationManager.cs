using UnityEngine;
using System.Collections.Generic;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using System;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using TMPro;

public class AuthenticationManager : MonoBehaviour
{

    // the AWS region of where your services live
    public static Amazon.RegionEndpoint Region = Amazon.RegionEndpoint.USEast1;
    private string email;
    // In production, should probably keep these in a config file
    const string IdentityPool = "us-east-1:5d4d5791-ea51-4c24-a42b-d3a61568a558"; //insert your Cognito User Pool ID, found under General Settings
    const string AppClientID = "3gqrf39ovipl4s36okr35lad9s"; //insert App client ID, found under App Client Settings
    const string userPoolId = "us-east-1_CThpLlXz4";

    private AmazonCognitoIdentityProviderClient _provider;
    private CognitoAWSCredentials _cognitoAWSCredentials;
    private static string _userid = "";
    private CognitoUser _user;

    public async Task<bool> RefreshSession()
    {

        DateTime issued = DateTime.Now;
        //gets tokens from saved file and saves it to UserSessionCache
        UserSessionCache userSessionCache = new UserSessionCache();
        SaveDataManager.LoadJsonData(userSessionCache);

        if (userSessionCache != null && userSessionCache._refreshToken != null && userSessionCache._refreshToken != "")
        {
            try
            {
                CognitoUserPool userPool = new CognitoUserPool(userPoolId, AppClientID, _provider);

                // apparently the username field can be left blank for a token refresh request
                CognitoUser user = new CognitoUser("", AppClientID, userPool, _provider);

                // The "Refresh token expiration (days)" (Cognito->UserPool->General Settings->App clients->Show Details) is the
                // amount of time since the last login that you can use the refresh token to get new tokens. After that period the refresh
                // will fail Using DateTime.Now.AddHours(1) is a workaround for https://github.com/aws/aws-sdk-net-extensions-cognito/issues/24
                user.SessionTokens = new CognitoUserSession(
                   userSessionCache.getIdToken(),
                   userSessionCache.getAccessToken(),
                   userSessionCache.getRefreshToken(),
                   issued,
                   DateTime.Now.AddDays(30)); // TODO: need to investigate further. 
                                              // It was my understanding that this should be set to when your refresh token expires...

                // Attempt refresh token call
                AuthFlowResponse authFlowResponse = await user.StartWithRefreshTokenAuthAsync(new InitiateRefreshTokenAuthRequest
                {
                    AuthFlowType = AuthFlowType.REFRESH_TOKEN_AUTH
                })
                .ConfigureAwait(false);


                // update session cache
                UserSessionCache userSessionCacheToUpdate = new UserSessionCache(
                   authFlowResponse.AuthenticationResult.IdToken,
                   authFlowResponse.AuthenticationResult.AccessToken,
                   authFlowResponse.AuthenticationResult.RefreshToken,
                   userSessionCache.getUserId());

                SaveDataManager.SaveJsonData(userSessionCacheToUpdate);
                // update credentials with the latest access token
                _cognitoAWSCredentials = user.GetCognitoAWSCredentials(IdentityPool, Region);

                _user = user;

                return true;
            }
            catch (NotAuthorizedException ne)
            {
                // https://docs.aws.amazon.com/cognito/latest/developerguide/amazon-cognito-user-pools-using-tokens-with-identity-providers.html
                // refresh tokens will expire - user must login manually every x days (see user pool -> app clients -> details)
                Debug.Log("NotAuthorizedException: " + ne);
            }
            catch (WebException webEx)
            {
                // we get a web exception when we cant connect to aws - means we are offline
                Debug.Log("WebException: " + webEx);
            }
            catch (Exception ex)
            {
                Debug.Log("Exception: " + ex);
            }
        }
        return false;
    }

    public async Task<bool> ForgotPassword(string email)
    {
        this.email = email;
        CognitoUserPool userPool = new CognitoUserPool(userPoolId, AppClientID, _provider);
        CognitoUser user = new CognitoUser(email, AppClientID, userPool, _provider);
        
        ForgotPasswordRequest forgotPasswordRequest = new ForgotPasswordRequest()
        {
            Username = email,
            ClientId = AppClientID
        };
            
        try { 
         await _provider.ForgotPasswordAsync(forgotPasswordRequest);
        }catch(Exception e)
        {
            Debug.Log(e);
        }
        return true;
    }
    public async Task<string> ConfirmResetPassword(string code, string newPass)
    {
        CognitoUserPool userPool = new CognitoUserPool(userPoolId, AppClientID, _provider);
        CognitoUser user = new CognitoUser(email, AppClientID, userPool, _provider);



        ConfirmForgotPasswordRequest confirmForgotPasswordRequest = new ConfirmForgotPasswordRequest()
        {
            Username = email,
            ClientId = AppClientID,
            ConfirmationCode = code,
            Password = newPass
        };

        try
        {
            await _provider.ConfirmForgotPasswordAsync(confirmForgotPasswordRequest);
            
            return "OK";
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return FormatFeedbackMessage(e.Message);
        }
        
    }



    public async Task<string> Login(string email, string password)
    {
        this.email = email;
        CognitoUserPool userPool = new CognitoUserPool(userPoolId, AppClientID, _provider);
        CognitoUser user = new CognitoUser(email, AppClientID, userPool, _provider);

        InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest()
        {
            Password = password
        };

        try
        {
            AuthFlowResponse authFlowResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);

            _userid = await GetUserIdFromProvider(authFlowResponse.AuthenticationResult.AccessToken);

            UserSessionCache userSessionCache = new UserSessionCache(
               authFlowResponse.AuthenticationResult.IdToken,
               authFlowResponse.AuthenticationResult.AccessToken,
               authFlowResponse.AuthenticationResult.RefreshToken,
               _userid);

            SaveDataManager.SaveJsonData(userSessionCache);

            // This how you get credentials to use for accessing other services.
            // This IdentityPool is your Authorization, so if you tried to access using an
            // IdentityPool that didn't have the policy to access your target AWS service, it would fail.
            _cognitoAWSCredentials = user.GetCognitoAWSCredentials(IdentityPool, Region);

            _user = user;

            return "OK";
        }
        catch (Exception e)
        {
            
            
            
            return FormatFeedbackMessage(e.Message);
        }
    }
    private string FormatFeedbackMessage(string feedbackMessage)
    {
        if(feedbackMessage == "User is not confirmed.") {
            return "Activate the account by clicking the link sent to your email to login.";
        }
        int idx = feedbackMessage.IndexOf("'");
        if (idx >= 0)
        {
            string field = feedbackMessage.Substring(idx, feedbackMessage.Substring(idx).IndexOf("'"));
        }
        bool laterCut = false;
        int length = feedbackMessage.IndexOf(".");
        if (length < 0)
        {
            laterCut = true;
            length = feedbackMessage.IndexOf(":") + 1;
        }
        string message = "";
        if (!laterCut)
        {
            message = feedbackMessage.Substring(0, length);
        }
        else
        {
            message = feedbackMessage.Substring(length);
        }
        message = message.Replace("Member", "Information provided");
        if (message.Contains("[\\S]"))
        {
            message = "No spaces allowed or too short";
        }
        return message;
    }
    public async Task<string> Signup(string username, string email, string password)
    {

        SignUpRequest signUpRequest = new SignUpRequest()
        {
            ClientId = AppClientID,
            Username = email,
            Password = password

        };

        // must provide all attributes required by the User Pool that you configured
        List<AttributeType> attributes = new List<AttributeType>()
      {
         new AttributeType(){
            Name = "email", Value = email
         },
         new AttributeType(){
            Name = "preferred_username", Value = username
         }
      };
        signUpRequest.UserAttributes = attributes;

        try
        {
            SignUpResponse sighupResponse = await _provider.SignUpAsync(signUpRequest);
            return "OK";
        }
        catch (Exception e)
        {
            
            return FormatFeedbackMessage(e.Message);
        }
    }

    // Make the user's unique id available for GameLift APIs, linking saved data to user, etc
    public string GetUsersId()
    {
        if (_userid == null || _userid == "")
        {
            // load userid from cached session 
            UserSessionCache userSessionCache = new UserSessionCache();
            SaveDataManager.LoadJsonData(userSessionCache);
            _userid = userSessionCache.getUserId();
        }
        return _userid;
    }

    // we call this once after the user is authenticated, then cache it as part of the session for later retrieval 
    private async Task<string> GetUserIdFromProvider(string accessToken)
    {
        string subId = "";

        Task<GetUserResponse> responseTask =
           _provider.GetUserAsync(new GetUserRequest
           {
               AccessToken = accessToken
           });

        GetUserResponse responseObject = await responseTask;

        // set the user id
        foreach (var attribute in responseObject.UserAttributes)
        {
            if (attribute.Name == "sub")
            {
                subId = attribute.Value;
                break;
            }
        }

        return subId;
    }

   
    public async void SignOut()
    {
        await _user.GlobalSignOutAsync();

        // Important! Make sure to remove the local stored tokens 
        UserSessionCache userSessionCache = new UserSessionCache("", "", "", "");
        SaveDataManager.SaveJsonData(userSessionCache);

    }

    // access to the user's authenticated credentials to be used to call other AWS APIs
    public CognitoAWSCredentials GetCredentials()
    {
        return _cognitoAWSCredentials;
    }

    // access to the user's access token to be used wherever needed - may not need this at all.
    public string GetAccessToken()
    {
        UserSessionCache userSessionCache = new UserSessionCache();
        SaveDataManager.LoadJsonData(userSessionCache);
        return userSessionCache.getAccessToken();
    }

    public string GetIdentityToken()
    {
        UserSessionCache userSessionCache = new UserSessionCache();
        SaveDataManager.LoadJsonData(userSessionCache);
        return userSessionCache.getIdToken();
    }
    
    

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _provider = new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials(), Region);
    }

    public string GetUsername()
    {

        
        string decode = GetIdentityToken().Split(".")[1];
        int padlength = 4 - decode.Length % 4;
        if (padlength < 4)
        {
            decode += new string('=', padlength);
        }
        var bytes = Convert.FromBase64String(decode);
        ASCIIEncoding ascii = new ASCIIEncoding();
        string tokenJson = ascii.GetString(bytes);
        IdToken idToken = JsonUtility.FromJson<IdToken>(tokenJson);
        return idToken.preferred_username;
    }
}