using Microsoft.AppCenter.Unity;
using UnityEngine;

public enum ServerUri
{
    Local,
    Production
}

[CreateAssetMenu(fileName = "ApiEndPoints", menuName = "ScriptableObject/NewApiEndpointConfig")]

public class ApiEndPointsSO : ScriptableObject
{
    [SerializeField] private string _resumeUrl = "/game/resume";
    [SerializeField] private string _login = "/auth/login";
    [SerializeField] private string _play = "/game/buffalo/play";
    [SerializeField] private string _continue = "/game/buffalo/continue";
    [SerializeField] private string _playerInfo = "/player";

    [Space(10)]
    [SerializeField] private int _bet;

    [Space(10)]
    [Header("Base Uri")]
    [SerializeField] private ServerUri _serverUri;
    [SerializeField] private string _localUri = "http://192.168.55.77:8001/v1";
    [SerializeField] private string _productionUri = "http://34.205.135.210/v1";
    private string _baseUri;

    public string ResumeUrl { get => _baseUri + _resumeUrl; }
    public string Login { get => _baseUri + _login; }
    public string Play { get => _baseUri + _play; }
    public string Continue { get => _baseUri + _continue; }
    public string PlayerInfo { get => _baseUri + _playerInfo; }
    public int Bet { get => _bet; }

    public void OnEnable()
    {
        switch (_serverUri)
        {
            case ServerUri.Local:
                _baseUri = _localUri;
                break;
            case ServerUri.Production:
                _baseUri = _productionUri;
                break;
        }
    }
}
