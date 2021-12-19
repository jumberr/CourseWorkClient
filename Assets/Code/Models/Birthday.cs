using System;

namespace Code.Models
{
    [Serializable]
    public class Birthday
    {
        public Birthday(int creator, string nameBday, string dateBday, string phoneNumBday, string additionBday)
        {
            this.creator = creator;
            name_Bday = nameBday;
            date_Bday = dateBday;
            phone_num_Bday = phoneNumBday;
            addition_Bday = additionBday;
        }

        public int id;
        public int creator;
        public string name_Bday;
        public string date_Bday;
        public string phone_num_Bday;
        public string addition_Bday;
    }
}