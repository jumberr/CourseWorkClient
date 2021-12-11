using Code.Validators;
using TMPro;
using UnityEngine;

namespace Code.Services
{
    public class RegisterService : MonoBehaviour
    {
        [SerializeField] private GameObject registerPopup;
        
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

        public void Register()
        {
            var login = username.text;
            var pass = password.text;
            
            var result = Validate(login, pass, out var loginOk, out var passOk);

            usernameError.SetActive(false);
            passwordError.SetActive(false);
            
            if (result)
            {
                SendRequestToRegister(login, pass);
                registerPopup.SetActive(false);
            }
            else
            {
                ValidationError(loginOk, passOk);
            }
        }

        private void SendRequestToRegister(string login, string pass)
        {
            var data = new User(login, pass);
            StartCoroutine(DbService.Instance
                .Post("https://localhost:5001/api/PersonDetails/AddPerson", data));
        }

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
    }
}
