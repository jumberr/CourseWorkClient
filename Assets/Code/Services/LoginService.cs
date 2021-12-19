using System;
using Code.Models;
using Code.Validators;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Services
{
    public class LoginService : MonoBehaviour
    {
        [SerializeField] private Credentials _credentials;

        [SerializeField] private GameObject play;
        [SerializeField] private GameObject login;
        [SerializeField] private GameObject register;

        [SerializeField] private TMP_InputField username;
        [SerializeField] private TMP_InputField password;

        [SerializeField] private GameObject usernameError;
        [SerializeField] private GameObject passwordError;

        [SerializeField] private TMP_Text usernameErrorText;
        [SerializeField] private TMP_Text passwordErrorText;


        private IValidator _usernameValidator;
        private IValidator _passwordValidator;

        private void Awake()
        {
            _usernameValidator = new UsernameValidator();
            _passwordValidator = new PasswordValidator();
        }

        private async void Start()
        {
            var res = await CheckUserCredentials(_credentials.login, _credentials.pass);
            if (res)
                AutomateLogin();
        }

        private async UniTask<bool> Login()
        {
            var login = username.text;
            var pass = password.text;
            // checks locally
            var result = Validate(login, pass, out var loginOk, out var passOk);

            usernameError.SetActive(false);
            passwordError.SetActive(false);

            if (result)
            {
                // checks in DB
                var res = await CheckUserCredentials(login, pass);
                if (res)
                {
                    _credentials.login = login;
                    _credentials.pass = pass;
                    //LoadGameAfterLogin();
                    return true;
                }
                else
                {
                    usernameError.SetActive(true);
                    passwordError.SetActive(true);
                    usernameErrorText.text = "Check your input! Your credentials invalid. Register, please!";
                    passwordErrorText.text = "Check your input! Your credentials invalid. Register, please!";
                    return false;
                }
            }
            else
            {
                ValidationError(loginOk, passOk);
                return false;
            }
        }

        public void AutomateLogin()
        {
            login.SetActive(false);
        }

        public async void OpenPlayButton()
        {
            var res = await Login();
            if (res)
                login.SetActive(false);
        }

        public void SwitchUser()
        {
            login.SetActive(true);
        }

        public void GoToGame()
        {
            LoadGameAfterLogin();
        }

        private void LoadGameAfterLogin()
        {
            SceneManager.LoadSceneAsync("GameScene");
        }

        // Check locally
        private bool Validate(string login, string pass, out bool loginOk, out bool passOk)
        {
            loginOk = _usernameValidator.Validate(login);
            passOk = _passwordValidator.Validate(pass);

            return loginOk && passOk;
        }

        private void ValidationError(bool loginOk, bool passOk)
        {
            if (!loginOk)
            {
                // Draw login error
                usernameError.SetActive(true);
                usernameErrorText.text = "Check your input! Text should be length 6+, english language, no digits";
            }

            if (!passOk)
            {
                // Draw pass error
                passwordError.SetActive(true);
                passwordErrorText.text = "Check your input! Text should be length 6+, english language";
            }
        }

        // Send request to db to check
        private async UniTask<bool> CheckUserCredentials(string login, string pass)
        {
            var url = $"{Constants.Host}/{Constants.Api}/{Constants.Person}/{Constants.Get}";
            var data = new User(login, pass);
            var result = await DbService.Instance.GetResponse(url, data);
            return Convert.ToBoolean(result);
        }

        public void OnClickRegister()
        {
            register.SetActive(true);
        }
    }
}