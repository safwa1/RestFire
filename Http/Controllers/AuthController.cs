using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestFire.Types;

namespace RestFire.Http.Controllers;

public sealed class AuthController
{
    public Credentials Credentials { get; private set; }

    private Axios _axios;
    private readonly string _apiKey;

    private static readonly Lazy<AuthController> Instance = new(() => new AuthController());

    public static AuthController Shared => Instance.Value;

    public static Credentials GetCredentials() => Shared.Credentials;

    private AuthController()
    {
        _apiKey = InfoStore.Shared.ApiKey;
    }

    private async Task<Response> UseAuth(string url, string email, string password)
    {
        _axios = new Axios(url);
        var credential = new Dictionary<string, string>
        {
            { "email", email },
            { "password", password },
            { "returnSecureToken", "true" },
        };

        var response = await _axios.PostAsync(
            "",
            JsonConvert.SerializeObject(credential)
        );

        return response;
    }

    public async Task<bool> SignupAsync(string email, string password)
    {
        var response = await UseAuth(RestFireApp.SignupBaseUrl + _apiKey, email, password);
        var signup = response.ResponseText.Contains("kind");
        return signup;
    }

    public async Task<AuthenticationState> SigninAsync(string email, string password)
    {
        var response = await UseAuth(RestFireApp.SigninBaseUrl + _apiKey, email, password);
        var jsonResponse = response.ResponseText;
        var loggedIn = jsonResponse.Contains("kind");
        if (!loggedIn) return new AuthenticationState(false);

        var responseAsJObject = JObject.Parse(jsonResponse);
        var idToken = responseAsJObject["idToken"]!.Value<string>();
        var state = new AuthenticationState(true, idToken);
        this.Credentials = new Credentials(
            email,
            password,
            state.AccessToken
        );
        InfoStore.Shared.AccessToken = state.AccessToken;
        return state;
    }

    public async Task<bool> DeleteAccountAsync(string email, string password)
    {
        var response = await SigninAsync(email, password);
        if (!response.Success) return false;
        var deleted = await DeleteAccountAsync(response.AccessToken);
        return deleted;
    }

    public async Task<bool> DeleteAccountAsync(string accessToken)
    {
        var delete = await _axios.PostAsync(
            RestFireApp.DeleteAccountUrl + _apiKey,
            JsonConvert.SerializeObject(new { idToken = accessToken })
        );

        return delete.IsSuccess;
    }
}