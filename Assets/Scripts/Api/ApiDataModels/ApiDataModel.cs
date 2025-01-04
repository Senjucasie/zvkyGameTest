using System;

namespace Login.Api
{
    public class LoginCredentials
    {
        public string username;
        public string password;
    }

    [Serializable]
    public class LoginResponse
    {
        public bool status;
        public string message;
        public double balance;
        public string data;
    }
}

namespace Player.Api
{
    [Serializable]
    public class PlayerData
    {
        public bool status;
        public string message;
        public Data data;

        [Serializable]
        public class Data
        {
            public int player_id;
            public string username;
            public string fullname;
            public string email;
            public double balance;
        }
    }
}