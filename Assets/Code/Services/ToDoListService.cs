using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        [SerializeField] private GameObject updateWindow;

        [SerializeField] private TMP_InputField todoName;
        [SerializeField] private TMP_InputField todoDescription;
        [SerializeField] private TMP_InputField todoStatus;

        [SerializeField] private TMP_InputField updateName;
        [SerializeField] private TMP_InputField updateDescription;
        [SerializeField] private TMP_InputField updateStatus;

        private CustomContainer customContainer;
        private bool accept;

        private async void Start()
        {
            customContainer = new CustomContainer();

            GameManager.instance.PersonId = await GetPersonID();
            var url = $"{Constants.Host}/{Constants.Api}/{Constants.ToDo}/{Constants.GetAll}/{GameManager.instance.PersonId}";

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
            var url = $"{Constants.Host}/{Constants.Api}/{Constants.ToDo}/{Constants.Delete}/{GameManager.instance.PersonId}/{index}";
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
            cont.updatePopup.onClick.AddListener(CallUpdate);
            SetTexts(cont, toDo);

            async void CallDelete() => await Delete(toDo, dbId);
            async void CallUpdate() => await OpenUpdate(dbId);
        }

        private async Task OpenUpdate(int dbId)
        {
            updateWindow.SetActive(true);
            await UniTask.WaitUntil(() => accept);
            await UpdateValue(dbId);
        }

        private async UniTask UpdateValue(int id)
        {
            var url = $"{Constants.Host}/{Constants.Api}/{Constants.ToDo}/{Constants.Update}/{GameManager.instance.PersonId}/{id}";

            var name = updateName.text;
            var description = updateDescription.text;
            var time = DatePickerControl.DateGlobal.ToString();
            var type = updateStatus.text;

            var todo = new ToDo(GameManager.instance.PersonId, name, description, time, type);
            var obj = await DbService.Instance.PostResponse(url, todo);
            var localId = Convert.ToInt32(obj);

            var container = customContainer.Containers[localId];
            container.name.text = name;
            container.description.text = description;
            container.date.text = time;
            container.status.text = type;

            updateWindow.SetActive(false);
            accept = false;

            updateName.text = string.Empty;
            updateDescription.text = string.Empty;
            updateStatus.text = string.Empty;
        }

        public void ApplyUpdate()
        {
            accept = true;
        }

        public void CloseUpdate()
        {
            updateWindow.SetActive(false);
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