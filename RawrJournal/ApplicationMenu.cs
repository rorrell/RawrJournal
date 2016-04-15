using System;
using Gtk;
using RawrJournal.Dialogs;
using RawrJournal.Classes;

namespace RawrJournal
{
    public class ApplicationMenu
    {
        #region Properties
        private MenuBar _menuBar = new MenuBar();
        public MenuBar MB
        {
            get { return _menuBar; }
        }
        private MenuItem _editjournalItem = new MenuItem("Edit");
        public MenuItem EditJournalItem
        {
            get { return _editjournalItem; }
        }
        private MenuItem _friendsItem = new MenuItem("Manage Friends");
        public MenuItem FriendsItem
        {
            get { return _friendsItem; }
        }
        private MenuItem _importItem = new MenuItem("Import");
        public MenuItem ImportItem
        {
            get { return _importItem; }
        }
        private MenuItem _exportItem = new MenuItem("Export");
        public MenuItem ExportItem
        {
            get { return _exportItem; }
        }
        private MenuItem _newtagItem = new MenuItem("New");
        public MenuItem NewTagItem
        {
            get { return _newtagItem; }
        }
        private MenuItem _edittagItem = new MenuItem("Edit");
        public MenuItem EditTagItem
        {
            get { return _edittagItem; }
        }
        private MenuItem _deletetagItem = new MenuItem("Delete");
        public MenuItem DeleteTagItem
        {
            get { return _deletetagItem; }
        }
        private MenuItem _newentryItem = new MenuItem("New");
        public MenuItem NewEntryItem
        {
            get { return _newentryItem; }
        }
        private MenuItem _saveentryItem = new MenuItem("Save");
        public MenuItem SaveEntryItem
        {
            get { return _saveentryItem; }
        }
        private MenuItem _previewentryItem = new MenuItem("Preview");
        public MenuItem PreviewEntryItem
        {
            get { return _previewentryItem; }
        }
        private MenuItem _postentryItem = new MenuItem("Post to LJ");
        public MenuItem PostEntryItem
        {
            get { return _postentryItem; }
        }
        private MenuItem _deletejournalItem = new MenuItem("Delete");
        public MenuItem DeleteJournalItem
        {
            get { return _deletejournalItem; }
        }
        private MenuItem _deleteentryItem = new MenuItem("Delete");
        public MenuItem DeleteEntryItem
        {
            get { return _deleteentryItem; }
        }
        private MenuItem _updatetagItem = new MenuItem("Update from LJ");
        public MenuItem UpdateTagItem
        {
            get { return _updatetagItem; }
        }
        MenuItem _updatejournalItem = new MenuItem("Update from LJ");
        public MenuItem UpdateJournalItem
        {
            get { return _updatejournalItem; }
        }
        MenuItem _insertImageItem = new MenuItem("Insert Image");
        public MenuItem InsertImageItem
        {
            get { return _insertImageItem; }
        }
        #endregion Properties

        #region Methods
        public void Initialize(Gdk.Window window)
        {
            _menuBar.ParentWindow = window;
            //menu
            MenuItem journalItem = new MenuItem("Journal");
            Menu journalMenu = new Menu();
            journalItem.Submenu = journalMenu;
            MenuItem entryItem = new MenuItem("Entry");
            Menu entryMenu = new Menu();
            entryItem.Submenu = entryMenu;
            MenuItem tagItem = new MenuItem("Tag");
            Menu tagMenu = new Menu();
            tagItem.Submenu = tagMenu;
            MenuItem exitItem = new MenuItem("Exit");
            exitItem.ButtonPressEvent += exit_Click;

            #region Journal Menu
            MenuItem newjournalItem = new MenuItem("New");
            newjournalItem.ButtonPressEvent += newjournal_click;
            MenuItem lockItem = new MenuItem("Lock/Unlock");
            #endregion Journal Menu

            #region Entry Menu
            #endregion Entry Menu

            #region Tag Menu
            #endregion Tag Menu

            _menuBar.Add(journalItem);
            _menuBar.Add(entryItem);
            _menuBar.Add(tagItem);
            _menuBar.Add(exitItem);
            journalMenu.Add(newjournalItem);
            journalMenu.Add(_editjournalItem);
            journalMenu.Add(_deletejournalItem);
            journalMenu.Add(_updatejournalItem);
            journalMenu.Add(_friendsItem);
            journalMenu.Add(lockItem);
            journalMenu.Add(_importItem);
            journalMenu.Add(_exportItem);
            entryMenu.Add(_newentryItem);
            entryMenu.Add(_insertImageItem);
            entryMenu.Add(_saveentryItem);
            entryMenu.Add(_previewentryItem);
            entryMenu.Add(_postentryItem);
            entryMenu.Add(_deleteentryItem);
            tagMenu.Add(_newtagItem);
            tagMenu.Add(_edittagItem);
            tagMenu.Add(_deletetagItem);
            tagMenu.Add(_updatetagItem);
        }
        #endregion Methods

        #region Event Handlers
        void newjournal_click(object obj, EventArgs args)
        {
            ApplicationWindow.jd.IsNew = true;
            ApplicationWindow.jd.Initialize();
            ApplicationWindow.jd.JournalD.Show();
        }
        public static void exit_Click(object obj, EventArgs args)
        {
            ApplicationWindow.trayIcon.Visible = false; // this is necessary to properly clean the icon after exiting
            Application.Quit();
        }
        #endregion Event Handlers
    }
}
