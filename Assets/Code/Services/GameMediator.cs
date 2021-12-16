using UnityEngine;

namespace Code.Services
{
    public class GameMediator : MonoBehaviour
    {
        [SerializeField] private GameObject todoPopup;

        public void OpenToDoAdd() =>
            OpenClose(todoPopup, true);

        public void CloseToDoAdd() => 
            OpenClose(todoPopup, false);

        private void OpenClose(GameObject go, bool value) =>
            go.SetActive(value);
    }
}