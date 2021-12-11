using System.Text.RegularExpressions;

namespace Code.Validators
{
    public class PasswordValidator : IValidator
    {
        // returns true if username length > 6, english
        public bool Validate(string value)
        {
            var match = Regex.IsMatch(value, "^[a-zA-Z0-9]*$");
            return value.Length >= 6 && match;
        }
    }
}