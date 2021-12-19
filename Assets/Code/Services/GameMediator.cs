using UnityEngine;

namespace Code.Services
{
    public class GameMediator : MonoBehaviour
    {
        [SerializeField] private GameObject todoPopup;
        [SerializeField] private GameObject birthdayPopup;
        [SerializeField] private GameObject notesPopup;

        public void OpenToDoAdd() =>
            OpenClose(todoPopup, true);
        
        public void OpenBirthdayAdd() =>
            OpenClose(birthdayPopup, true);
        
        public void OpenNotesAdd() =>
            OpenClose(notesPopup, true);

        public void CloseToDoAdd() => 
            OpenClose(todoPopup, false);
        
        public void CloseBdayAdd() => 
            OpenClose(birthdayPopup, false);
        
        public void CloseNotesAdd() => 
            OpenClose(notesPopup, false);

        private void OpenClose(GameObject go, bool value) =>
            go.SetActive(value);
    }
}