using System;
using System.Collections.Generic;
using Mono.Data.SqliteClient;
using System.Data;

namespace RawrJournal.Classes
{
    public class Journal
    {
        #region Properties
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private string _username;
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }
        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        private bool _islocked;
        public bool IsLocked
        {
            get { return _islocked; }
            set { _islocked = value; }
        }
        private string _key;
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }
        private List<JournalEntry> _entries = new List<JournalEntry>();
        public List<JournalEntry> Entries
        {
            get { return _entries; }
            set { _entries = value; }
        }
        #endregion Properties

        #region Methods
        #endregion Methods
    }
}
