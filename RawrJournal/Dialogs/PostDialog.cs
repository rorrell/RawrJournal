using System;
using Gtk;
using RawrJournal.Classes;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Collections;

namespace RawrJournal.Dialogs
{
    public class PostDialog
    {
        #region Properties
        private Dialog _dialog = new Dialog();
        public Dialog PostD
        {
            get { return _dialog; }
        }
        private JournalEntry _jentry = new JournalEntry();
        public JournalEntry JournalEntry
        {
            get { return _jentry; }
            set { _jentry = value; }
        }
        private bool _isinitialized = false;
        private Journal journal = new Journal();
        private ComboBox userpicCB = ComboBox.NewText();
        private Image userpicImg = new Image();
        private ComboBox securityCB = ComboBox.NewText();
        private CheckButton commentsCheck = new CheckButton();
        private string[] url;
        private ScrolledWindow sw = new ScrolledWindow();
        private TreeView groupTree = new TreeView();
        private TreeStore groupStore = new TreeStore(typeof(string), typeof(int));
        #endregion Properties

        #region Methods
        public void Initialize()
        {
            journal = (new JournalManager()).GetById(_jentry.JournalId);
            object[] pics = (new Livejournal()).getuserpics(journal.Username, journal.Password);
            string[] keywords = (string[])pics[0];
            url = (string[])pics[1];
            foreach (string pic in keywords)
                userpicCB.AppendText(pic);
            object[] obj = (new Livejournal()).getfriends(journal.Username, journal.Password, true, true, true);
            List<FriendGroup> groups = (List<FriendGroup>)obj[1];
            foreach (FriendGroup group in groups)
                groupStore.AppendValues(group.Name, group.Id);
            if (!_isinitialized)
            {
                _dialog.Title = "Post Entry";
                _dialog.SetPosition(WindowPosition.Center);
                _dialog.SetSizeRequest(400, 400);
                HBox row1 = new HBox(false, 0);
                Label userpicLbl = new Label("Userpic: ");
                userpicCB.Changed += new EventHandler(userpic_changed);
                HBox row2 = new HBox(false, 0);
                Label securityLbl = new Label("Security: ");
                securityCB.AppendText("Public");
                securityCB.AppendText("Friends");
                securityCB.AppendText("Private");
                securityCB.AppendText("Other");
                securityCB.Changed += new EventHandler(security_changed);
                HBox row3 = new HBox(false, 0);
                Label commentsLbl = new Label("Disallow comments: ");

                HBox row4 = new HBox(false, 0); 
                groupTree.Selection.Mode = SelectionMode.Multiple;
                sw.Add(groupTree);
                TreeViewColumn idCol = new TreeViewColumn();
                idCol.Visible = false;
                TreeViewColumn nameCol = new TreeViewColumn();
                nameCol.Title = "Group";
                CellRendererText idColCell = new CellRendererText();
                CellRendererText nameCell = new CellRendererText();
                nameCol.PackStart(nameCell, true);
                idCol.PackStart(idColCell, true);
                groupTree.AppendColumn(nameCol);
                groupTree.AppendColumn(idCol);
                nameCol.AddAttribute(nameCell, "text", 0);
                idCol.AddAttribute(idColCell, "text", 1);
                groupTree.Model = groupStore;

                Button postDBtn = new Button("Submit");
                Button postDBtn2 = new Button("Cancel");
                postDBtn.Clicked += new EventHandler(post_submit);
                postDBtn2.Clicked += new EventHandler(post_close);
                _dialog.VBox.PackStart(row1, false, false, 0);
                _dialog.VBox.PackStart(userpicImg, false, false, 5);
                _dialog.VBox.PackStart(row2, false, false, 0);
                _dialog.VBox.PackStart(row3, false, false, 0);
                _dialog.VBox.PackStart(row4, true, true, 0);
                row1.PackStart(userpicLbl, false, false, 0);
                row1.PackStart(userpicCB, false, false, 0);
                row2.PackStart(securityLbl, false, false, 0);
                row2.PackStart(securityCB, false, false, 0);
                row3.PackStart(commentsLbl, false, false, 0);
                row3.PackStart(commentsCheck, false, false, 0);
                row4.PackStart(sw, true, true, 0);
                _dialog.ActionArea.Add(postDBtn);
                _dialog.ActionArea.Add(postDBtn2);
                _dialog.ShowAll();
                _isinitialized = true;
            }
            sw.Hide();
        }
        #endregion Methods

        #region Event Handlers
        void post_submit(object obj, EventArgs args)
        {
            string security = "public";
            int allowmask = 0;
            switch(securityCB.ActiveText)
            {
                case "Private":
                    security = "private";
                    break;
                case "Friends":
                    security = "";
                    allowmask = 1;
                    break;
                case "Other":
                    security = "usemask";
                    BitArray ba = new BitArray(32);
                    ba.SetAll(false);
                    TreePath[] paths = groupTree.Selection.GetSelectedRows();
                    TreeIter iter = new TreeIter();
                    int groupbit = 0;
                    if (paths.Length > 0)
                    {
                        foreach (TreePath path in paths)
                            if (groupTree.Model.GetIter(out iter, path))
                            {
                                groupbit = Convert.ToInt32(groupTree.Model.GetValue(iter, 1).ToString());
                                ba.Set(groupbit, true);
                            }
                        byte[] b = new byte[32];
                        ba.CopyTo(b, 0);
                        allowmask = BitConverter.ToInt32(b, 0);
                    }
                    break;
            }
            int itemid = (new Livejournal()).postevent(journal.Username, journal.Password, 
                _jentry, security, allowmask, userpicCB.ActiveText, !commentsCheck.Active);
            _jentry.IsLj = true;
            _jentry.LjId = itemid;
            (new JournalEntryManager()).Update(_jentry);
            _dialog.Hide();
        }
        void post_close(object obj, EventArgs args)
        {
            _dialog.Hide();
        }
        void userpic_changed(object obj, EventArgs args)
        {
            WebRequest req = WebRequest.Create(url[userpicCB.Active]);
            WebResponse response = req.GetResponse();
            Stream stream = response.GetResponseStream();
            MemoryStream memStream = new MemoryStream();
            byte[] buffer = new byte[1024];
            byte[] downloadedData = new byte[0];
            int bytesread = 0;
            do
            {
                bytesread = stream.Read(buffer, 0, buffer.Length);
                memStream.Write(buffer, 0, bytesread);
            } while (bytesread != 0);
            stream.Close();
            memStream.Close();
            downloadedData = memStream.ToArray();
            userpicImg.Pixbuf = new Gdk.Pixbuf(downloadedData);
            memStream.Dispose();
            stream.Dispose();
        }
        void security_changed(object obj, EventArgs args)
        {
            if (securityCB.ActiveText == "Other")
                sw.Show();
            else
                sw.Hide();
        }
        #endregion Event Handlers
    }
}
