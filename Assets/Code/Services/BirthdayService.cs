using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Containers;
using Code.Models;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Code.Services
{
    public class BirthdayService : MonoBehaviour
    {
        [SerializeField] private GameMediator _mediator;
        [SerializeField] private Credentials _credentials;

        [SerializeField] private Transform placeToSpawnBday;
        [SerializeField] private GameObject bdayPrefab;
        [SerializeField] private GameObject updateWindow;

        [SerializeField] private TMP_InputField bdayName;
        [SerializeField] private TMP_InputField bdayAddition;
        [SerializeField] private TMP_InputField bdayPhone;

        [SerializeField] private TMP_InputField updateName;
        [SerializeField] private TMP_InputField updateAddition;
        [SerializeField] private TMP_InputField updatePhone;

        private CustomContainerBirthday customContainer;
        private bool accept;

        private async void Start()
        {
            customContainer = new CustomContainerBirthday();

            //GameManager.instance.PersonId = await GetPersonID();
            await UniTask.WaitUntil(() => GameManager.instance.isLoaded);
            var url =
                $"{Constants.Host}/{Constants.Api}/{Constants.Birthday}/{Constants.GetAll}/{GameManager.instance.PersonId}";

            var res = await DbService.Instance.GetResponse(url);
            var list = JsonService.FromJsonToList<Birthday>((string) res);
            
            for (var i = 0; i < list.Length; i++)
                InstantiateBdayLocally(list[i], list[i].id);
        }

        public async void Add()
        {
            var personID = GameManager.instance.PersonId;

            var url = $"{Constants.Host}/{Constants.Api}/{Constants.Birthday}/{Constants.Add}";
            var bday = new Birthday(personID, bdayName.text, DatePickerControl.DateGlobal.ToString(),
                bdayPhone.text, bdayAddition.text);
            var obj = await DbService.Instance.PostResponse(url, bday);
            var bdayDbId = Convert.ToInt32(obj);

            InstantiateBdayLocally(bday, bdayDbId);
            _mediator.CloseBdayAdd();

            bdayAddition.text = string.Empty;
            bdayName.text = string.Empty;
            bdayPhone.text = string.Empty;
        }

        private async UniTask Delete(Birthday bday, int index)
        {
            var url =
                $"{Constants.Host}/{Constants.Api}/{Constants.Birthday}/{Constants.Delete}/{GameManager.instance.PersonId}/{index}";
            var obj = await DbService.Instance.PostResponse(url, bday);
            var localId = Convert.ToInt32(obj);
            Destroy(customContainer.View[localId]);
            customContainer.Remove(localId);
        }

        private void InstantiateBdayLocally(Birthday bday, int dbId)
        {
            var obj = Instantiate(bdayPrefab, placeToSpawnBday);
            var cont = obj.GetComponent<BirthdayContainer>();
            customContainer.Add(dbId, obj, cont);
            cont.deleteButton.onClick.AddListener(CallDelete);
            cont.updatePopup.onClick.AddListener(CallUpdate);
            SetTexts(cont, bday);

            async void CallDelete() => await Delete(bday, dbId);
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
            var url =
                $"{Constants.Host}/{Constants.Api}/{Constants.Birthday}/{Constants.Update}/{GameManager.instance.PersonId}/{id}";

            var name = updateName.text;
            var addition = updateAddition.text;
            var time = DatePickerControl.DateGlobal.ToString();
            var phone = updatePhone.text;

            var bday = new Birthday(GameManager.instance.PersonId, name, time, phone, addition);
            var obj = await DbService.Instance.PostResponse(url, bday);
            var localId = Convert.ToInt32(obj);

            var container = customContainer.Containers[localId];
            container.name.text = name;
            container.addition.text = addition;
            container.date.text = time;
            container.phone.text = phone;

            updateWindow.SetActive(false);
            accept = false;

            updateName.text = string.Empty;
            updateAddition.text = string.Empty;
            updatePhone.text = string.Empty;
        }

        public void ApplyUpdate()
        {
            accept = true;
        }

        public void CloseUpdate()
        {
            updateWindow.SetActive(false);
        }

        private void SetTexts(BirthdayContainer cont, Birthday bday)
        {
            cont.name.text = bday.name_Bday;
            cont.addition.text = bday.addition_Bday;
            cont.date.text = bday.date_Bday;
            cont.phone.text = bday.phone_num_Bday;
        }

        //public void GoToLogin()
        //{
        //    SceneManager.LoadSceneAsync("LoginScene");
        //}
    }

    public class CustomContainerBirthday
    {
        private List<int> dbId;
        private List<GameObject> view;
        private List<BirthdayContainer> container;

        public IReadOnlyList<int> DbId => dbId;
        public IReadOnlyList<GameObject> View => view;
        public IReadOnlyList<BirthdayContainer> Containers => container;

        public CustomContainerBirthday()
        {
            dbId = new List<int>();
            view = new List<GameObject>();
            container = new List<BirthdayContainer>();
        }

        public void Add(int id, GameObject v, BirthdayContainer c)
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