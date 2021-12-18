using System;
using System.Collections.Generic;
using Code.Containers;
using Code.Models;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Services
{
    public class ToDoListService : MonoBehaviour
    {
        [SerializeField] private GameMediator _mediator;
        [SerializeField] private Credentials _credentials;

        [SerializeField] private Transform placeToSpawnToDo;
        [SerializeField] private GameObject toDoPrefab;

        [SerializeField] private TMP_InputField todoName;
        [SerializeField] private TMP_InputField todoDescription;
        [SerializeField] private TMP_InputField todoStatus;

        private CustomContainer customContainer;
        private int _personId;

        private async void Awake()
        {
            customContainer = new CustomContainer();

            _personId = await GetPersonID();
            var url = $"{Constants.Host}/{Constants.Api}/{Constants.ToDo}/{Constants.GetAll}/{_personId}";

            var res = await DbService.Instance.GetResponse(url);
            var list = JsonService.FromJsonToList<ToDo>((string) res);

            for (var i = 0; i < list.Length; i++)
                InstantiateToDoLocally(list[i], list[i].id);
        }

        public async void Add()
        {
            var personID = await GetPersonID();

            var url = $"{Constants.Host}/{Constants.Api}/{Constants.ToDo}/{Constants.Add}";
            var todo = new ToDo(personID, todoName.text, todoDescription.text, DatePickerControl.DateGlobal.ToString(),
                todoStatus.text);
            var obj = await DbService.Instance.PostResponse(url, todo);
            var todoDbId = Convert.ToInt32(obj);

            InstantiateToDoLocally(todo, todoDbId);
            _mediator.CloseToDoAdd();


            todoDescription.text = string.Empty;
            todoName.text = string.Empty;
            todoStatus.text = string.Empty;
        }
        
        private async UniTask Delete(ToDo todo, int index)
        {
            var url = $"{Constants.Host}/{Constants.Api}/{Constants.ToDo}/{Constants.Delete}/{_personId}/{index}";
            var obj = await DbService.Instance.PostResponse(url, todo);
            var localId = Convert.ToInt32(obj);
            Destroy(customContainer.View[localId]);
            customContainer.Remove(localId);
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

        private void InstantiateToDoLocally(ToDo toDo, int dbId)
        {
            var obj = Instantiate(toDoPrefab, placeToSpawnToDo);
            var cont = obj.GetComponent<ToDoContainer>();
            customContainer.Add(dbId, obj, cont);
            cont.deleteButton.onClick.AddListener(CallDelete);
            SetTexts(cont, toDo);
            
            async void CallDelete() => await Delete(toDo, dbId);
        }

        private void SetTexts(ToDoContainer cont, ToDo todo)
        {
            cont.name.text = todo.name_ToDo;
            cont.description.text = todo.description_ToDo;
            cont.date.text = todo.end_date_ToDo;
            cont.status.text = todo.status_ToDo;
        }

        public void GoToLogin()
        {
            SceneManager.LoadSceneAsync("LoginScene");
        }
    }

    public class CustomContainer
    {
        private List<int> dbId;
        private List<GameObject> view;
        private List<ToDoContainer> container;

        public IReadOnlyList<int> DbId => dbId;
        public IReadOnlyList<GameObject> View => view;
        public IReadOnlyList<ToDoContainer> Containers => container;

        public CustomContainer()
        {
            dbId = new List<int>();
            view = new List<GameObject>();
            container = new List<ToDoContainer>();
        }

        public void Add(int id, GameObject v, ToDoContainer c)
        {
            dbId.Add(id);
            view.Add(v);
            container.Add(c);
        }

        public void Remove(int id)
        {
            dbId.RemoveAt(id);
            view.RemoveAt(id);
            container.RemoveAt(id);
        }
    }
}