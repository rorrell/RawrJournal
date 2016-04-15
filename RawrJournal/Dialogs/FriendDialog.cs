using System;
using Gtk;
using RawrJournal.Classes;
using System.Collections.Generic;

namespace RawrJournal.Dialogs
{
    public class FriendDialog
    {
        #region Properties
        private Dialog _dialog = new Dialog();
        public Dialog FriendD
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
        private TreeStore notInStore = new TreeStore(typeof(string));
        private TreeStore inStore = new TreeStore(typeof(string));
        private TreeView notInTree = new TreeView();
        private TreeView inTree = new TreeView();
        private List<Friend> friends = new List<Friend>();
        private List<FriendGroup> groups = new List<FriendGroup>();
        #endregion Properties

        #region Methods
        public void Initialize()
        {
            Journal journal = (new JournalManager()).GetById(_id);
            object[] obj = (new Livejournal()).getfriends(journal.Username, journal.Password, true, true, true);
            friends = (List<Friend>)obj[0];
            groups = (new FriendGroupManager()).GetFriendsInGroup(friends, (List<FriendGroup>)obj[1]);
            ComboBox groupCB = ComboBox.NewText();
            foreach (FriendGroup group in groups)
                groupCB.AppendText(group.Name);
            TreeStore friendStore = new TreeStore(typeof(Friend));
            foreach (Friend friend in friends)
                friendStore.AppendValues(friend);
            TreeView friendTree = new TreeView();
            if (!_isinitialized)
            {
                _dialog.Title = "Manage Friends";
                _dialog.WindowPosition = WindowPosition.Center;
                _dialog.SetSizeRequest(400, 400);
                Notebook notebook = new Notebook();
                VBox friendBox = new VBox(true, 0);
                VBox groupBox = new VBox(false, 0);
                HBox panelBox = new HBox(true, 0);
                notebook.AppendPage(friendBox, new Label("Friends"));
                notebook.AppendPage(groupBox, new Label("Groups"));

                ScrolledWindow sw1 = new ScrolledWindow();
                TreeViewColumn usernameCol = new TreeViewColumn();
                usernameCol.Title = "Username";
                TreeViewColumn fullnameCol = new TreeViewColumn();
                fullnameCol.Title = "Full Name";
                CellRendererText usernameColCell = new CellRendererText();
                CellRendererText fullnameColCell = new CellRendererText();
                usernameCol.PackStart(usernameColCell, true);
                fullnameCol.PackStart(fullnameColCell, true);
                usernameCol.SetCellDataFunc(usernameColCell, new TreeCellDataFunc(RenderUsername));
                fullnameCol.SetCellDataFunc(fullnameColCell, new TreeCellDataFunc(RenderFullname));
                friendTree.AppendColumn(usernameCol);
                friendTree.AppendColumn(fullnameCol);

                ScrolledWindow sw2 = new ScrolledWindow();
                ScrolledWindow sw3 = new ScrolledWindow();
                groupCB.Changed += new EventHandler(group_change);
                notInTree = getGenericTree(notInStore);
                notInTree.Columns[0].Title = "Not In Group";
                inTree = getGenericTree(inStore);
                inTree.Columns[0].Title = "In Group";

                Button friendDBtn = new Button("Submit");
                Button friendDBtn2 = new Button("Cancel");

                friendDBtn.Clicked += new EventHandler(friend_submit);
                friendDBtn2.Clicked += new EventHandler(friend_close);

                _dialog.VBox.PackStart(notebook, true, true, 0);
                friendBox.PackStart(sw1, true, true, 0);
                groupBox.PackStart(groupCB, false, false, 0);
                groupBox.PackStart(panelBox, true, true, 0);
                panelBox.PackStart(sw2, true, true, 0);
                panelBox.PackStart(sw3, true, true, 0);
                sw1.Add(friendTree);
                sw2.Add(notInTree);
                sw3.Add(inTree);
                _dialog.ActionArea.Add(friendDBtn);
                _dialog.ActionArea.Add(friendDBtn2);
                _dialog.ShowAll();
                _isinitialized = true;
            }
            friendTree.Model = friendStore;
        }
        private void RenderUsername(TreeViewColumn col, CellRenderer cell, TreeModel model, TreeIter iter)
        {
            Friend friend = (Friend)model.GetValue(iter, 0);
            (cell as CellRendererText).Foreground = friend.FgColor;
            (cell as CellRendererText).CellBackground = friend.BgColor;
            (cell as CellRendererText).Text = friend.Username;
        }
        private void RenderFullname(TreeViewColumn col, CellRenderer cell, TreeModel model, TreeIter iter)
        {
            Friend friend = (Friend)model.GetValue(iter, 0);
            (cell as CellRendererText).Foreground = friend.FgColor;
            (cell as CellRendererText).CellBackground = friend.BgColor;
            (cell as CellRendererText).Text = friend.Fullname;
        }
        static TreeView getGenericTree(TreeStore store)
        {
            TreeView tree = new TreeView(store);
            TreeViewColumn textCol = new TreeViewColumn();
            CellRendererText textColCell = new CellRendererText();
            textCol.PackStart(textColCell, true);
            tree.AppendColumn(textCol);
            textCol.AddAttribute(textColCell, "text", 0);
            return tree;
        }
        #endregion Methods

        #region Event Handlers
        void friend_submit(object obj, EventArgs args)
        {
            _dialog.Hide();
        }
        void friend_close(object obj, EventArgs args)
        {
            _dialog.Hide();
        }
        void group_change(object obj, EventArgs args)
        {
            inStore.Clear();
            foreach (Friend friend in groups[((ComboBox)obj).Active].Friends)
                inStore.AppendValues(friend.Username);
            notInStore.Clear();
            foreach (Friend friend2 in groups[((ComboBox)obj).Active].NotFriends)
                notInStore.AppendValues(friend2.Username);
        }
        #endregion Event Handlers
    }
}
