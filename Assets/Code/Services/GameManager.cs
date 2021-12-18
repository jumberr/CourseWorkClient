using Code.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Services
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        public int PersonId;

        [SerializeField] private Button todoButton;
        [SerializeField] private Button birthdayButton;
        [SerializeField] private Button notesButton;
        
        private GameState _state;
        public GameState State => _state;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            ChangeState(GameState.ToDoList);
            
            todoButton.onClick.AddListener( () => ChangeState(GameState.ToDoList));
            birthdayButton.onClick.AddListener( () => ChangeState(GameState.Birthday));
            notesButton.onClick.AddListener( () => ChangeState(GameState.Notes));
        }

        public void ChangeState(GameState state)
        {
            _state = state;

            switch (state)
            {
                case GameState.ToDoList:
                    todoButton.Select();
                    break;
                case GameState.Birthday:
                    birthdayButton.Select();
                    break;
                case GameState.Notes:
                    notesButton.Select();
                    break;
            }
        }
    }
}