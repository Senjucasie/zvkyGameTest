using UnityEngine;
using Login.Api;
using TMPro;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public LoginResponse loginResponse;

    [SerializeField] private GameObject _loginPanel;
    [SerializeField] private TMP_InputField _usernameField;
    [SerializeField] private TMP_InputField _passwordField;
    [SerializeField] private Button _loginButton;

    public void OnLoginClick()
    {
        _loginButton.interactable = false;
        _usernameField.text = _usernameField.text.Trim();

        if (_usernameField.text != string.Empty || _passwordField.text != string.Empty)
            LoginRequest(_usernameField.text, _passwordField.text);
        else
            Debug.LogError("Wrong Login Credentials Format!");
    }

    private void LoginRequest(string username, string password)
    {
        //LoginCredentials loginCredentials = new LoginCredentials()
        //{ 
        //    username = username,
        //    password = password 
        //};

        //GameApiManager.Instance.SendLoginRequest(loginCredentials, OnLoginSuccess);
        HideLoginPanel();
    }
    private void OnLoginSuccess(string response)
    {
        LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(response);
        GameApiManager.Instance.SetLoginToken(loginResponse.data);
        GetPlayerData();
    }

    private void GetPlayerData()
    {
        GameApiManager.Instance.GetPlayerData(OnReceivedPlayerData);
    }

    private void OnReceivedPlayerData()
    {
        EventManager.InvokeSetInitialBalance();
        EventManager.InvokeUpdateCreditValue();
        EventManager.InvokeSwitchBaseButtonState(BaseButtonInteractionRegistry.Instance.stateSwitchingData, false);
        HideLoginPanel();
        CheckResumeData();
    }

    private void HideLoginPanel()
    {
        _loginPanel.SetActive(false);
    }

    private void CheckResumeData() 
    {
        GameApiManager.Instance.SendResumeRequest();
    }

}
