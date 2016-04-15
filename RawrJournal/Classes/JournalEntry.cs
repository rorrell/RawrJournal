using System;
using System.Collections.Generic;

namespace RawrJournal.Classes
{
    public class JournalEntry
    {
        #region Properties
        private int _id = 0;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private int _journalId = 0;
        public int JournalId
        {
            get { return _journalId; }
            set { _journalId = value; }
        }
        private bool _isLj = false;
        public bool IsLj
        {
            get { return _isLj; }
            set { _isLj = value; }
        }
        private int _ljId = 0;
        public int LjId
        {
            get { return _ljId; }
            set { _ljId = value; }
        }
        private string _subject = "";
        public string Subject
        {
            get { return _subject; }
            set { _subject = value; }
        }
        private string _body = "";
        public string Body
        {
            get { return _body; }
            set { _body = value; }
        }
        private DateTime _date = DateTime.Now;
        public DateTime EntryDate
        {
            get { return _date; }
            set { _date = value; }
        }
        private string _mood = "";
        public string Mood
        {
            get { return _mood; }
            set { _mood = value; }
        }
        private string _music = "";
        public string Music
        {
            get { return _music; }
            set { _music = value; }
        }
        private List<Tag> _tags = new List<Tag>();
        public List<Tag> Tags
        {
            get { return _tags; }
            set { _tags = value; }
        }
        #endregion Properties

        #region Methods
        #endregion Methods
    }
}
