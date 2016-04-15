using System;
using Gtk;
using RawrJournal.Classes;
using System.Collections.Generic;

namespace RawrJournal.Dialogs
{
    public class JournalSelectDialog
    {
        #region Properties
        private Dialog _dialog = new Dialog();
        public Dialog JournalSelectD
        {
            get { return _dialog; }
        }
        private ComboBox _journalList = ComboBox.NewText();
        public ComboBox JournalList
        {
            get { return _journalList; }
        }
        private bool _isinitialized = false;
        #endregion Properties

        #region Methods
        public void Initialize()
        {
            if (!_isinitialized)
            {
                _dialog.Title = "Add/Edit Tag";
                _dialog.SetPosition(WindowPosition.Center);
                HBox journalDBox = new HBox(false, 0);
                Label journalDLbl = new Label("Select Journal: ");
                List<Journal> journals = (new JournalManager()).GetAll();
                foreach (Journal j in journals)
                    _journalList.AppendText(j.Name);
                Button journalDBtn = new Button("Submit");
                Button journalDBtn2 = new Button("Cancel");
                journalDBtn.Clicked += new EventHandler(journal_submit);
                journalDBtn2.Clicked += new EventHandler(journal_close);
                _dialog.VBox.PackStart(journalDBox, false, false, 0);
                journalDBox.PackStart(journalDLbl, false, false, 0);
                journalDBox.PackStart(_journalList, true, true, 0);
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
            _dialog.Hide();
        }
        void journal_close(object obj, EventArgs args)
        {
            _dialog.Hide();
        }
        #endregion Event Handlers
    }
}
