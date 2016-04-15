using System;
using System.Collections.Generic;
using Gtk;
using RawrJournal.Classes;

namespace RawrJournal.Dialogs
{
    public class LjTagDialog
    {
        #region Properties
        private Dialog _dialog = new Dialog();
        public Dialog TagD
        {
            get { return _dialog; }
        }
        private int _id = 0;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private bool _isinitialized = false;
        private CheckButton check = new CheckButton("Delete existing first?");
        #endregion Properties

        #region Methods
        public void Initialize()
        {
            if (!_isinitialized)
            {
                _dialog.Title = "Update Tags from LJ";
                _dialog.SetPosition(WindowPosition.Center);
                Button tagDBtn = new Button("Submit");
                Button tagDBtn2 = new Button("Cancel");
                tagDBtn.Clicked += new EventHandler(tag_submit);
                tagDBtn2.Clicked += new EventHandler(tag_close);
                _dialog.VBox.PackStart(check, false, false, 0);
                _dialog.ActionArea.Add(tagDBtn);
                _dialog.ActionArea.Add(tagDBtn2);
                _dialog.ShowAll();
                _isinitialized = true;
            }
        }
        #endregion Methods

        #region Event Handlers
        void tag_submit(object obj, EventArgs args)
        {
            TagManager tm = new TagManager();
            Journal journal = (new JournalManager()).GetById(_id);
            if (check.Active)
                tm.DeleteAll();
            List<Tag> tags = (new Livejournal()).getusertags(journal.Username, journal.Password);
            string tagstring = tm.CreateString(tags);
            tags = tm.ProcessTagString(tagstring);
            _dialog.Hide();
        }
        void tag_close(object obj, EventArgs args)
        {
            _dialog.Hide();
        }
        #endregion Event Handlers
    }
}
