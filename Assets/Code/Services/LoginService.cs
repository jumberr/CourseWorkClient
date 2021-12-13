﻿using System;
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
        [SerializeField] private GameObject register;
        
        [SerializeField] private TMP_InputField username;
        [SerializeField] private TMP_InputField password;

        [SerializeField] private GameObject usernameError;
        [SerializeField] private GameObject passwordError;
        
        [SerializeField] private TMP_Text usernameErrorText;
        [SerializeField] private TMP_Text passwordErrorText;

        
        private IValidator _usernameValidator;
        private IValidator _passwordValidator;
        
        private void Start()
        {
            _usernameValidator = new UsernameValidator();
            _passwordValidator = new PasswordValidator();
        }

        [UsedImplicitly]
        public async void Login()
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
                    // player exist
                    Debug.Log("SUCK A COCK FUCKING NIGGER");
                    SceneManager.LoadScene("GameScene");
                }
                else
                {
                    usernameError.SetActive(true);
                    passwordError.SetActive(true);
                    usernameErrorText.text = "Check your input! Your credentials invalid. Register, please!";
                    passwordErrorText.text = "Check your input! Your credentials invalid. Register, please!";
                }
            }
            else
            {
                ValidationError(loginOk, passOk);
            }
        }

        // Check locally
        private bool Validate(string login, string pass, out bool loginOk, out bool passOk)
        {
            loginOk = _usernameValidator.Validate(login);
            passOk = _passwordValidator.Validate(pass);
            
            return  loginOk && passOk;
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
            var url = $"{Constants.Host}/{Constants.Api}/{Constants.PersonDetails}/{Constants.GetPersonByName}";
            var data = new User(login, pass);
            var result = await DbService.Instance.PostWithResponse(url, data);
            return Convert.ToBoolean(result);
        }

        public void OnClickRegister()
        {
            register.SetActive(true);
        }
    }
}
