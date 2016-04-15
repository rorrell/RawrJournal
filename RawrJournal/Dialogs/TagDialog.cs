using System;
using Gtk;
using RawrJournal.Classes;

namespace RawrJournal.Dialogs
{
    public class TagDialog
    {
        #region Properties
        private Dialog _dialog = new Dialog();
        public Dialog TagD
        {
            get { return _dialog; }
        }
        private Entry _tagDEntry = new Entry();
        public Entry TagEntry
        {
            get { return _tagDEntry; }
        }
        private bool _isNew = true;
        public bool IsNew
        {
            get { return _isNew; }
            set { _isNew = value; }
        }
        private string _initialText = "";
        public string InitialText
        {
            get { return _initialText; }
            set { _initialText = value; }
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
                HBox tagDBox = new HBox(false, 0);
                Label tagDLbl = new Label("Name: ");
                Button tagDBtn = new Button("Submit");
                Button tagDBtn2 = new Button("Cancel");
                tagDBtn.Clicked += new EventHandler(tag_submit);
                tagDBtn2.Clicked += new EventHandler(tag_close);
                _dialog.VBox.PackStart(tagDBox, false, false, 0);
                tagDBox.PackStart(tagDLbl, false, false, 0);
                tagDBox.PackStart(_tagDEntry, true, true, 0);
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
            Tag tag = new Tag();
            if (_isNew)
            {
                tag.Name = TagEntry.Text;
                tm.Insert(tag);
            }
            else
            {
                tag = tm.GetByName(InitialText);
                tag.Name = TagEntry.Text;
                tm.Update(tag);
            }
            _dialog.Hide();
        }
        void tag_close(object obj, EventArgs args)
        {
            _dialog.Hide();
        }
        #endregion Event Handlers
    }
}
