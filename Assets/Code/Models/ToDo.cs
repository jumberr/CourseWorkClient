using System;

namespace Code.Models
{
    [Serializable]
    public class ToDo
    {
        public ToDo(int creator, string nameToDo, string descriptionToDo, string endDateToDo, bool statusToDo)
        {
            this.creator = creator;
            name_ToDo = nameToDo;
            description_ToDo = descriptionToDo;
            end_date_ToDo = endDateToDo;
            status_ToDo = statusToDo;
        }

        public ToDo(int creator, string nameToDo, string descriptionToDo)
        {
            this.creator = creator;
            name_ToDo = nameToDo;
            description_ToDo = descriptionToDo;
        }

        public int id;
        public int creator;
        public string name_ToDo;
        public string end_date_ToDo;
        public bool status_ToDo;
        public string description_ToDo;
    }
}