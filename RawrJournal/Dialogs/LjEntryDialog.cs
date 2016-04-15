using System;
using System.Collections.Generic;
using Gtk;
using RawrJournal.Classes;

namespace RawrJournal.Dialogs
{
    public class LjEntryDialog
    {
        #region Properties
        private Dialog _dialog = new Dialog();
        public Dialog EntryD
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
        private TreeStore entryStore = new TreeStore(typeof(string), typeof(string), typeof(int));
        private TreeView entryTree = new TreeView();
        private int[] downloadList = new int[25];
        private int[] deleteList = new int[25];
        private Journal journal = new Journal();
        private List<JournalEntry> entries = new List<JournalEntry>();
        #endregion Properties

        #region Methods
        public void Initialize()
        {
            journal = (new JournalManager()).GetById(_id); 
            entries = (new Livejournal()).getlastnentries(journal.Username, journal.Password, 25, DateTime.Now);
            foreach (JournalEntry entry in entries)
                entryStore.AppendValues(entry.EntryDate.ToString(), entry.Subject, entry.LjId);
            if (!_isinitialized)
            {
                _dialog.Title = "Update Entries from LJ";
                _dialog.SetPosition(WindowPosition.Center);
                _dialog.SetSizeRequest(500, 400);

                entryTree.Selection.Mode = SelectionMode.Multiple;
                ScrolledWindow sw = new ScrolledWindow();
                TreeViewColumn idCol = new TreeViewColumn();
                idCol.Visible = false;
                TreeViewColumn dateCol = new TreeViewColumn();
                dateCol.Title = "Date";
                TreeViewColumn subjectCol = new TreeViewColumn();
                subjectCol.Title = "Subject";
                TreeViewColumn textCol = new TreeViewColumn();
                CellRendererText idColCell = new CellRendererText();
                CellRendererText dateCell = new CellRendererText();
                CellRendererText subjectCell = new CellRendererText();
                dateCol.PackStart(dateCell, true);
                subjectCol.PackStart(subjectCell, true);
                idCol.PackStart(idColCell, true);
                entryTree.AppendColumn(dateCol);
                entryTree.AppendColumn(subjectCol);
                entryTree.AppendColumn(idCol);
                dateCol.AddAttribute(dateCell, "text", 0);
                subjectCol.AddAttribute(subjectCell, "text", 1);
                idCol.AddAttribute(idColCell, "text", 2);
                entryTree.Model = entryStore;

                Button entryDBtn = new Button("Submit");
                Button entryDBtn2 = new Button("Cancel");
                entryDBtn.Clicked += new EventHandler(entry_submit);
                entryDBtn2.Clicked += new EventHandler(entry_close);
                sw.Add(entryTree);
                _dialog.VBox.PackStart(sw, true, true, 0);
                _dialog.ActionArea.Add(entryDBtn);
                _dialog.ActionArea.Add(entryDBtn2);
                _dialog.ShowAll();
                _isinitialized = true;
            }
        }
        #endregion Methods

        #region Event Handlers
        void entry_submit(object obj, EventArgs args)
        {
            Livejournal lj = new Livejournal();
            JournalEntryManager em = new JournalEntryManager();
            TreeSelection selection = entryTree.Selection;
            TreePath[] paths = selection.GetSelectedRows();
            TreeIter iter = new TreeIter();
            JournalEntry entry = new JournalEntry();
            int itemid = 0;
            if (paths.Length > 0)
            {
                foreach(TreePath path in paths)
                if (entryTree.Model.GetIter(out iter, path))
                {
                    itemid = Convert.ToInt32(entryTree.Model.GetValue(iter, 2).ToString());
                    foreach (JournalEntry e in entries)
                        if (e.LjId == itemid)
                        {
                            entry = e;
                            break;
                        }
                    entry.JournalId = journal.Id;
                    em.Insert(entry);
                }
            }
            _dialog.Hide();
        }
        void entry_close(object obj, EventArgs args)
        {
            _dialog.Hide();
        }
        #endregion Event Handlers
    }
}
