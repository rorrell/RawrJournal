using System;
using Gtk;
using RawrJournal.Classes;

namespace RawrJournal.Dialogs
{
    public class ConfirmDialog
    {
        #region Properties
        private Dialog _dialog = new Dialog();
        public Dialog ConfirmD
        {
            get { return _dialog; }
        }
        private bool _isConfirmed = false;
        public bool IsConfirmed
        {
            get { return _isConfirmed; }
            set { _isConfirmed = value; }
        }
        private bool _isinitialized = false;
        #endregion Properties

        #region Methods
        public void Initialize()
        {
            if (!_isinitialized)
            {
                _dialog.Title = "Confirm";
                _dialog.SetPosition(WindowPosition.Center);
                Label confirmDLbl = new Label("Are you sure?");
                Button confirmDBtn = new Button("Submit");
                Button confirmDBtn2 = new Button("Cancel");
                confirmDBtn.Clicked += new EventHandler(confirm_submit);
                confirmDBtn2.Clicked += new EventHandler(confirm_close);
                _dialog.VBox.PackStart(confirmDLbl, true, true, 0);
                _dialog.ActionArea.Add(confirmDBtn);
                _dialog.ActionArea.Add(confirmDBtn2);
                _dialog.ShowAll();
                _isinitialized = true;
            }
            _isConfirmed = false;
        }
        #endregion Methods

        #region Event Handlers
        void confirm_submit(object obj, EventArgs args)
        {
            _isConfirmed = true;
            _dialog.Hide();
        }
        void confirm_close(object obj, EventArgs args)
        {
            _dialog.Hide();
        }
        #endregion Event Handlers
    }
}
