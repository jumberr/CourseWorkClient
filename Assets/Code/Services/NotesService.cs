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
    public class NotesService : MonoBehaviour
    {
        [SerializeField] private GameMediator _mediator;
        [SerializeField] private Credentials _credentials;

        [SerializeField] private Transform placeToSpawnNote;
        [SerializeField] private GameObject notePrefab;
        [SerializeField] private GameObject updateWindow;

        [SerializeField] private TMP_InputField noteName;
        [SerializeField] private TMP_InputField noteGroup;
        [SerializeField] private TMP_InputField noteDescription;

        [SerializeField] private TMP_InputField updateName;
        [SerializeField] private TMP_InputField updateGroup;
        [SerializeField] private TMP_InputField updateDescription;

        private CustomContainerNotes customContainer;
        private bool accept;

        private async void Start()
        {
            customContainer = new CustomContainerNotes();

            await UniTask.WaitUntil(() => GameManager.instance.isLoaded);
            var url =
                $"{Constants.Host}/{Constants.Api}/{Constants.Note}/{Constants.GetAll}/{GameManager.instance.PersonId}";

            var res = await DbService.Instance.GetResponse(url);
            var list = JsonService.FromJsonToList<Note>((string) res);
            
            for (var i = 0; i < list.Length; i++)
                InstantiateNoteLocally(list[i], list[i].id);
        }

        public async void Add()
        {
            var personID = GameManager.instance.PersonId;

            var url = $"{Constants.Host}/{Constants.Api}/{Constants.Note}/{Constants.Add}";
            var note = new Note(personID, noteName.text, noteDescription.text, noteGroup.text);
            var obj = await DbService.Instance.PostResponse(url, note);
            var bdayDbId = Convert.ToInt32(obj);

            InstantiateNoteLocally(note, bdayDbId);
            _mediator.CloseNotesAdd();

            noteGroup.text = string.Empty;
            noteName.text = string.Empty;
            noteDescription.text = string.Empty;
        }

        private async UniTask Delete(Note note, int index)
        {
            var url =
                $"{Constants.Host}/{Constants.Api}/{Constants.Note}/{Constants.Delete}/{GameManager.instance.PersonId}/{index}";
            var obj = await DbService.Instance.PostResponse(url, note);
            var localId = Convert.ToInt32(obj);
            Destroy(customContainer.View[localId]);
            customContainer.Remove(localId);
        }

        private void InstantiateNoteLocally(Note note, int dbId)
        {
            var obj = Instantiate(notePrefab, placeToSpawnNote);
            var cont = obj.GetComponent<NoteContainer>();
            customContainer.Add(dbId, obj, cont);
            cont.deleteButton.onClick.AddListener(CallDelete);
            cont.updatePopup.onClick.AddListener(CallUpdate);
            SetTexts(cont, note);

            async void CallDelete() => await Delete(note, dbId);
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
                $"{Constants.Host}/{Constants.Api}/{Constants.Note}/{Constants.Update}/{GameManager.instance.PersonId}/{id}";

            var name = updateName.text;
            var group = updateGroup.text;
            var description = updateDescription.text;

            var note = new Note(GameManager.instance.PersonId, name, group, description);
            var obj = await DbService.Instance.PostResponse(url, note);
            var localId = Convert.ToInt32(obj);

            var container = customContainer.Containers[localId];
            container.name.text = name;
            container.group.text = group;
            container.description.text = description;

            updateWindow.SetActive(false);
            accept = false;

            updateName.text = string.Empty;
            updateGroup.text = string.Empty;
            updateDescription.text = string.Empty;
        }

        public void ApplyUpdate()
        {
            accept = true;
        }

        public void CloseUpdate()
        {
            updateWindow.SetActive(false);
        }

        private void SetTexts(NoteContainer cont, Note note)
        {
            cont.name.text = note.name_note;
            cont.group.text = note.groupName;
            cont.description.text = note.description_note;
        }
    }

    public class CustomContainerNotes
    {
        private List<int> dbId;
        private List<GameObject> view;
        private List<NoteContainer> container;

        public IReadOnlyList<int> DbId => dbId;
        public IReadOnlyList<GameObject> View => view;
        public IReadOnlyList<NoteContainer> Containers => container;

        public CustomContainerNotes()
        {
            dbId = new List<int>();
            view = new List<GameObject>();
            container = new List<NoteContainer>();
        }

        public void Add(int id, GameObject v, NoteContainer c)
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