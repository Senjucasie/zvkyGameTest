using Login.Api;
using Player.Api;
using Spin.Api;
using System;
using UnityEngine;
using SimpleJSON;
using System.Collections;
using Newtonsoft;
using Newtonsoft.Json;

public class GameApiManager : MonoBehaviour
{
    private ApiService _apiService;
    private PlayerData _playerData;
    private double _currentBalance;
    private bool _isCompleteRequestPending;
    private bool _isResumeAvailable;
    private bool _isCompleted;
    public SpinDataModel spinmodel;
    [SerializeField] private ApiEndPointsSO _apiUrl;
    [SerializeField] private ApiDataSO _apiData;

    public static GameApiManager Instance;
    public PlayerData PlayerData { get => _playerData; }
    public ApiDataSO ApiData { get => _apiData; }
    public double CurrentBalance { get => _currentBalance; }

    [SerializeField] private string[] _testData;
    int _index=0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
       
        _apiService = new ApiService();
        //spinmodel = JsonUtility.FromJson<SpinDataModel>(_testData[0]);
        //Debug.Log(spinmodel.data.reelStops.Length);
    }
    #region LOGIN
    public void SendLoginRequest(LoginCredentials loginData, Action<string> callback)
    {
        string jsonData = JsonUtility.ToJson(loginData);
        //StartCoroutine(_apiService.PostAPIRequest(jsonData, _apiUrl.Login, callback));
    }

    public void SetLoginToken(string loginToken)
    {
        PlayerPrefs.SetString(_apiService.LOGINTKN, loginToken);
    }

    public void GetPlayerData(Action callback)
    {
        string jsonData = JsonUtility.ToJson(String.Empty);
        //StartCoroutine(_apiService.GetAPIRequest(jsonData, _apiUrl.PlayerInfo, (response) =>
        //{
        //    this._playerData = JsonUtility.FromJson<PlayerData>(response);
        //    _currentBalance = _playerData.data.balance;
        //    callback();
        //}));
    }
    #endregion

    #region SPIN
    public void SendSpinRequest(StateName currentState)
    {
        #region TEST WITH DUMMY DATA
        //EventManager.InvokeSpinResponse();
        //return;
        #endregion

        StartCoroutine(SendSpinRequestCoroutine(currentState));
    }
    private IEnumerator SendSpinRequestCoroutine(StateName currentState)
    {
        yield return new WaitWhile(() => _isCompleteRequestPending);
        
        switch (currentState)
        {
            case StateName.Normal:
                SendNormalSpinRequest();
                break;

            case StateName.Auto:
                SendNormalSpinRequest();
                break;

            case StateName.Scatter:
                SendScatterContinueRequest();
                break;

            case StateName.Bonus:
                SendBonusContinueRequest();
                break;
        }
        
    }

    private void SendNormalSpinRequest()
    {
        if (_isResumeAvailable)
        {
            EventManager.InvokeSpinResponse();
            _isResumeAvailable = false;
        }
        else
        {
            //SpinRequestData spinRequestData = new SpinRequestData
            //{
            //    credits = GameConstants.creditValue[BetManager.Instance.BetIndex],
            //    bet = _apiUrl.Bet,
            //    isBuyFeature = false,
            //};
            //string jsonData = JsonUtility.ToJson(spinRequestData);
            //StartCoroutine(_apiService.PostAPIRequest(jsonData, _apiUrl.Play, OnSpinDataResponse));
            OnSpinDataResponse(_testData[_index++]);
        }
    }

    private void SendScatterContinueRequest()
    {
        if (_isResumeAvailable && !_isCompleted)
        {
            EventManager.InvokeSpinResponse();
            _isResumeAvailable = false;
        }
        else
        {
            //ContinueRequestData continueRequestData = new ContinueRequestData
            //{
            //    state = "FREE"
            //};
            //string jsonData = JsonUtility.ToJson(continueRequestData);
            //StartCoroutine(_apiService.PostAPIRequest(jsonData, _apiUrl.Continue, OnSpinDataResponse));
            OnSpinDataResponse(_testData[_index++]);
        }
    }

    private void OnSpinDataResponse(string response)
    {
        _isCompleted = false;
        ApiData.ClearSpinMatrix();
        UpdateSpinParsedData(response);

        JSONNode spinData = JSON.Parse(response);
        ApiData.CreateSpinMatrix(spinData);

        EventManager.InvokeSpinResponse();
    }

    public void SendBonusContinueRequest()
    {
        if (_isResumeAvailable && !_isCompleted)
        {
            _isResumeAvailable = false;
        }
        else
        {
            //ContinueRequestData continueRequestData = new ContinueRequestData
            //{
            //    state = "WHEEL_BONUS"
            //};
            //string jsonData = JsonUtility.ToJson(continueRequestData);
            //StartCoroutine(_apiService.PostAPIRequest(jsonData, _apiUrl.Continue, OnBonusDataResponse));
            OnBonusDataResponse(_testData[_index++]);
        }
    }

    private void OnBonusDataResponse(string response)
    {
        _isCompleted = false;
        _apiData.WheelBonusData = JsonUtility.FromJson<WheelBonusDataModel>(response);
        _currentBalance = _apiData.WheelBonusData.data.balance;
    }

    public void SendSpinCompleteRequest()
    {
        if (_isCompleted) return;

        _isCompleteRequestPending = true;
        OnCompleteResponse(string.Empty);
        //StartCoroutine(_apiService.PutAPIRequest(_apiUrl.Play, OnCompleteResponse));
    }

    private void OnCompleteResponse(string response)
    {
        _isCompleteRequestPending = false;
        _isResumeAvailable = false;
    }
    #endregion

    #region RESUME

    public void SendResumeRequest()
    {
        string jsonData = JsonUtility.ToJson(string.Empty);
        //StartCoroutine(_apiService.GetAPIRequest(jsonData, _apiUrl.ResumeUrl, OnResumeResponse));
    }

    private void OnResumeResponse(string response)
    {
        if (response == String.Empty)
        {
            SetNormalButtonState();
            return;
        }

        JSONNode jsonNode = JSON.Parse(response);
        string currentState = jsonNode["data"]["state"]["current"];
        string nextState = jsonNode["data"]["state"]["next"];
        _isCompleted = jsonNode["data"]["isCompleted"];

        ApiData.ClearSpinMatrix();
        UpdateSpinParsedData(response);
        ApiData.CreateSpinMatrix(jsonNode);

        Action callbackAction = GetCallbackAction(currentState, nextState);
        _isResumeAvailable = callbackAction != null;

        if (_isResumeAvailable)
        {
            ResumePopup.Instance.ShowResumePopup(callbackAction);
            EventManager.InvokeUpdateCreditValue(jsonNode["data"]["creditsWagered"]);
        }
        else
            SetNormalButtonState();
    }

    private Action GetCallbackAction(string currentState, string nextState)
    {
        if (_isCompleted)
        {
            if (nextState == GameConstants.ApiCurrntState.Free)
            {
                return EventManager.InvokeFreeSpinOnLogin;
            }
            else if (nextState == GameConstants.ApiCurrntState.WheelBonus)
            {
                return EventManager.InvokeBonusOnLogin;
            }
            else if (nextState == GameConstants.ApiCurrntState.PickBonus)
            {
                return EventManager.InvokeBonusOnLogin;
            }
            else if (nextState == GameConstants.ApiCurrntState.ReSpin)
            {
                return EventManager.InvokeBonusOnLogin;
            }
        }
        else
        {
            switch (currentState)
            {
                case GameConstants.ApiCurrntState.Base:
                    SetNormalButtonState();
                    return EventManager.InvokeSpinButton;
                case GameConstants.ApiCurrntState.Free:
                    return EventManager.InvokeFreeSpinOnLogin;
                case GameConstants.ApiCurrntState.WheelBonus:
                    return EventManager.InvokeBonusOnLogin;
                case GameConstants.ApiCurrntState.PickBonus:
                    return EventManager.InvokeBonusOnLogin;
                case GameConstants.ApiCurrntState.ReSpin:
                    return EventManager.InvokeBonusOnLogin;
            }
        }

        return null;
    }
    #endregion

    private void UpdateSpinParsedData(string response)
    {
        ApiData.SpinParsedData = JsonUtility.FromJson<SpinDataModel>(response);
        _currentBalance = ApiData.SpinParsedData.data.balance;
    }

    private void SetNormalButtonState()
    {
        EventManager.InvokeSwitchBaseButtonState(BaseButtonInteractionRegistry.Instance.normalData);
    }

    public int GetFreeSpinCount()
    {
        return ApiData.GetFreeSpinCount(_isResumeAvailable, _isCompleted);
    }

    public int GetReSpinCount()
    {
        return ApiData.GetReSpinCount(_isResumeAvailable, _isCompleted);
    }
}
