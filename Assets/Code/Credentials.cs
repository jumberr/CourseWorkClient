using UnityEngine;

namespace Code
{
    [CreateAssetMenu(fileName = "Credentials", menuName = "Credentials", order = 0)]
    public class Credentials : ScriptableObject
    {
        public string login;
        public string pass;
    }
}