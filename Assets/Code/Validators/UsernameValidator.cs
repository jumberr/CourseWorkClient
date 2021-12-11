using System.Linq;
using System.Text.RegularExpressions;

namespace Code.Validators
{
    public class UsernameValidator : IValidator
    {
        // returns true if username length > 6, english and no digits
        public bool Validate(string value)
        {
            var match = Regex.IsMatch(value, "^[a-zA-Z]*$");
            return !value.Any(char.IsDigit) && value.Length >= 6 && match;
        }
    }
}