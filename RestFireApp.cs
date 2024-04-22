using RestFire.Http.Controllers;
using RestFire.Types;

namespace RestFire;

public static class RestFireApp
{
    #region -> Constants
    public const string SignupBaseUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=";
    public const string SigninBaseUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=";
    public const string DeleteAccountUrl = "https://identitytoolkit.googleapis.com/v1/accounts:delete?key=";
    public static readonly AuthController Auth = AuthController.Shared;
    private static readonly InfoStore InfoStore = InfoStore.Shared;
    #endregion

    public static void Create(string apiKey, string databaseName)
    {
        InfoStore.ApiKey = apiKey;
        InfoStore.DatabaseName = databaseName;
    }
}