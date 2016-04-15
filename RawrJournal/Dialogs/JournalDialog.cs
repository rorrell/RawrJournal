using System;
using Gtk;
using RawrJournal.Classes;

namespace RawrJournal.Dialogs
{
    public class JournalDialog
    {
        #region Properties
        private Dialog _dialog = new Dialog();
        public Dialog JournalD
        {
            get { return _dialog; }
        }
        private bool _isNew = true;
        public bool IsNew
        {
            get { return _isNew; }
            set { _isNew = value; }
        }
        private Entry _nameEntry = new Entry();
        public Entry NameEntry
        {
            get { return _nameEntry; }
        }
        private Entry _keyEntry = new Entry();
        public Entry KeyEntry
        {
            get { return _keyEntry; }
        }
        private Entry _usernameEntry = new Entry();
        public Entry UsernameEntry
        {
            get { return _usernameEntry; }
        }
        private Entry _passwordEntry = new Entry();
        public Entry PasswordEntry
        {
            get { return _passwordEntry; }
        }
        private int _id = 0;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private bool _isinitialized = false;
        #endregion Properties

        #region Methods
        public void Initialize()
        {
            _id = 0;
            _nameEntry.Text = "";
            _keyEntry.Text = "";
            _usernameEntry.Text = "";
            _passwordEntry.Text = "";
            if (!_isinitialized)
            {
                _dialog.Title = "Add/Edit Journal";
                _dialog.SetPosition(WindowPosition.Center);
                Table journalTable = new Table(4, 2, false);
                journalTable.RowSpacing = 5;
                Label nameLbl = new Label("Name: ");
                Label keyLbl = new Label("Journal Password: ");
                Label usernameLbl = new Label("LJ Username: ");
                Label passwordLbl = new Label("LJ Password: ");
                _passwordEntry.Visibility = false;

                Button journalDBtn = new Button("Submit");
                Button journalDBtn2 = new Button("Cancel");

                journalDBtn.Clicked += new EventHandler(journal_submit);
                journalDBtn2.Clicked += new EventHandler(journal_close);

                journalTable.Attach(nameLbl, 0, 1, 0, 1);
                journalTable.Attach(_nameEntry, 1, 2, 0, 1);
                journalTable.Attach(keyLbl, 0, 1, 1, 2);
                journalTable.Attach(_keyEntry, 1, 2, 1, 2);
                journalTable.Attach(usernameLbl, 0, 1, 2, 3);
                journalTable.Attach(_usernameEntry, 1, 2, 2, 3);
                journalTable.Attach(passwordLbl, 0, 1, 3, 4);
                journalTable.Attach(_passwordEntry, 1, 2, 3, 4);
                _dialog.VBox.PackStart(journalTable, false, false, 0);
                _dialog.ActionArea.Add(journalDBtn);
                _dialog.ActionArea.Add(journalDBtn2);
                _dialog.ShowAll();
                _isinitialized = true;
            }
        }
        #endregion Methods

        #region Event Handlers
        void journal_submit(object obj, EventArgs args)
        {
            JournalManager jm = new JournalManager();
            Journal journal = new Journal();
            if (_isNew)
            {
                journal.Name = NameEntry.Text;
                journal.Key = KeyEntry.Text;
                journal.Username = UsernameEntry.Text;
                journal.Password = PasswordEntry.Text;
                jm.Insert(journal);
            }
            else
            {
                journal = jm.GetById(Id);
                journal.Name = NameEntry.Text;
                journal.Key = KeyEntry.Text;
                journal.Username = UsernameEntry.Text;
                journal.Password = PasswordEntry.Text;
                jm.Update(journal);
            }
            _dialog.Hide();
        }
        void journal_close(object obj, EventArgs args)
        {
            _dialog.Hide();
        }
        #endregion Event Handlers
    }
}
