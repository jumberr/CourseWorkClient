using System;
using System.Collections.Generic;
using Code.Containers;
using Code.Models;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

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

        private List<GameObject> toDoGo = new List<GameObject>();
        private List<ToDoContainer> containers = new List<ToDoContainer>();

        private async void Awake()
        {
            var personID = await GetPersonID();
            var url = $"{Constants.Host}/{Constants.Api}/{Constants.ToDo}/{Constants.GetAll}/{personID}";
            
            var res = await DbService.Instance.GetResponse(url);
            var list = JsonService.FromJsonToList<ToDo>((string)res);

            for (var i = 0; i < list.Length; i++)
                InstantiateToDoLocally(list[i]);
        }

        public async void Add()
        {
            var personID = await GetPersonID();

            var url = $"{Constants.Host}/{Constants.Api}/{Constants.ToDo}/{Constants.Add}";
            var todo = new ToDo(personID, todoName.text, todoDescription.text, DatePickerControl.DateGlobal.ToString(), false);
            await DbService.Instance.PostResponse(url, todo);

            InstantiateToDoLocally(todo);
            _mediator.CloseToDoAdd();
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

        private void InstantiateToDoLocally(ToDo toDo)
        {
            var obj = Instantiate(toDoPrefab, placeToSpawnToDo);
            var cont = obj.GetComponent<ToDoContainer>();
            containers.Add(cont);
            toDoGo.Add(obj);
            SetTexts(cont, toDo);
        }

        private void SetTexts(ToDoContainer cont, ToDo todo)
        {
            cont.name.text = todo.name_ToDo;
            cont.description.text = todo.description_ToDo;
            cont.date.text = todo.end_date_ToDo;
            cont.status.text = todo.status_ToDo.ToString();
        }
    }
}