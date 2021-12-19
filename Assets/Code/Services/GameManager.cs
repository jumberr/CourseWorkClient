using System;
using Code.Models;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Services
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        [SerializeField] private Credentials _credentials;
        [HideInInspector] public int PersonId;

        [SerializeField] private GameMediator _mediator;
        [SerializeField] private Button addButton;
        
        [SerializeField] private Button todoButton;
        [SerializeField] private Button birthdayButton;
        [SerializeField] private Button notesButton;

        [SerializeField] private GameObject todoList;
        [SerializeField] private GameObject birthdayList;
        [SerializeField] private GameObject notesList;
        
        private GameState _state;
        public GameState State => _state;
        public bool isLoaded;
        

        private async void Awake()
        {
            instance = this;
            PersonId = await GetPersonID();
            isLoaded = true;
        }

        private void Start()
        {
            ChangeState(GameState.Birthday);
            
            todoButton.onClick.AddListener( () => ChangeState(GameState.ToDoList));
            birthdayButton.onClick.AddListener( () => ChangeState(GameState.Birthday));
            notesButton.onClick.AddListener( () => ChangeState(GameState.Notes));
        }

        private void ChangeState(GameState state)
        {
            _state = state;

            addButton.onClick.RemoveAllListeners();
            switch (state)
            {
                case GameState.ToDoList:
                    todoButton.Select();
                    todoList.SetActive(true);
                    birthdayList.SetActive(false);
                    notesList.SetActive(false);
                    
                    addButton.onClick.AddListener( () => _mediator.OpenToDoAdd());
                    break;
                case GameState.Birthday:
                    birthdayButton.Select();
                    todoList.SetActive(false);
                    birthdayList.SetActive(true);
                    notesList.SetActive(false);
                    
                    addButton.onClick.AddListener( () => _mediator.OpenBirthdayAdd());
                    break;
                case GameState.Notes:
                    notesButton.Select();
                    todoList.SetActive(false);
                    birthdayList.SetActive(false);
                    notesList.SetActive(true);
                    
                    addButton.onClick.AddListener( () => _mediator.OpenNotesAdd());
                    break;
            }
        }
        
        private async UniTask<int> GetPersonID()
        {
            var uri = $"{Constants.Host}/{Constants.Api}/{Constants.Person}/{Constants.GetID}";
            var person = new User(_credentials.login, _credentials.pass);
            var obj = await DbService.Instance.GetResponse(uri, person);
            var res = (string) obj;
            var result = Convert.ToInt32(res);
            return result;
        }
    }
}