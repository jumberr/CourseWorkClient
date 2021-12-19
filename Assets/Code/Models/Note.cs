using System;

namespace Code.Models
{
    [Serializable]
    public class Note
    {
        public Note(int creator, string nameNote, string groupName, string descriptionNote)
        {
            this.creator = creator;
            name_note = nameNote;
            this.groupName = groupName;
            description_note = descriptionNote;
        }
        
        public int id;
        public int creator;
        public string name_note;
        public string groupName;
        public string description_note;
    }
}