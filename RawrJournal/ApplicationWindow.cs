using Gtk;
using System;
using System.Xml;
using RawrJournal.Classes;
using System.Collections.Generic;
using iTunesLib;
using RawrJournal.Dialogs;
using Mono.Data.SqliteClient;
using System.IO;
namespace RawrJournal
{
    public class ApplicationWindow
    {
        #region Public Properties
        public static StatusIcon trayIcon; // Tray Icon
        public static TagDialog td = new TagDialog();
        public static JournalDialog jd = new JournalDialog();
        public static JournalSelectDialog jsd = new JournalSelectDialog();
        public static ConfirmDialog cd = new ConfirmDialog();
        public static PostDialog pd = new PostDialog();
        public static Journal activeJournal = new Journal();
        public static FriendDialog fd = new FriendDialog();
        public static LjTagDialog ltd = new LjTagDialog();
        public static LjEntryDialog led = new LjEntryDialog();
        #endregion Public Properties

        #region Properties
        Window window = new Window("RawrJournal");
        RawrUtility util = new RawrUtility();
        JournalManager jm = new JournalManager();
        JournalEntryManager em = new JournalEntryManager();
        TreeStore journals = new TreeStore(typeof(string), typeof(string), typeof(int), typeof(int));
        TreeView journalTree = new TreeView();
        TagManager tm = new TagManager();
        TreeStore tags = new TreeStore(typeof(int), typeof(string));
        Calendar calendar = new Calendar();
        TreeView tagTree = new TreeView();
        Entry ljid = new Entry();
        Entry entryid = new Entry();
        ComboBox month = ComboBox.NewText();
        ComboBox day = ComboBox.NewText();
        Entry year = new Entry();
        Entry hours = new Entry();
        Entry mins = new Entry();
        ComboBoxEntry moodList = ComboBoxEntry.NewText();
        Entry subjectLine = new Entry();
        static TextBuffer bodyBuffer = new TextBuffer(new TextTagTable());
        TextView body = new TextView(bodyBuffer);
        Entry tagEntry = new Entry();
        Entry music = new Entry();
        Entry keyword = new Entry();
        TreeStore results = new TreeStore(typeof(string), typeof(string), typeof(int), typeof(int));
        TreeView resultsTree = new TreeView();
        #endregion Properties

        #region Methods
        public void Initialize()
        {
            //window
            window.Destroyed += new EventHandler(delete_event);
            window.Resize(1000, 625);
            window.WindowPosition = WindowPosition.Center;

            //outerBox contains everything else in the window
            VBox outerBox = new VBox(false, 0);
            window.Add(outerBox);

            #region Menu
            ApplicationMenu applicationMenu = new ApplicationMenu();
            applicationMenu.Initialize(window.GdkWindow);
            applicationMenu.EditJournalItem.ButtonPressEvent += new ButtonPressEventHandler(edit_journal);
            applicationMenu.FriendsItem.ButtonPressEvent += new ButtonPressEventHandler(manage_friends);
            applicationMenu.ImportItem.ButtonPressEvent += new ButtonPressEventHandler(import_entries);
            applicationMenu.ExportItem.ButtonPressEvent += new ButtonPressEventHandler(export_entries);
            applicationMenu.NewTagItem.ButtonPressEvent += new ButtonPressEventHandler(new_tag);
            applicationMenu.EditTagItem.ButtonPressEvent += new ButtonPressEventHandler(edit_tag);
            applicationMenu.DeleteTagItem.ButtonPressEvent += new ButtonPressEventHandler(delete_tag);
            applicationMenu.NewEntryItem.ButtonPressEvent += new ButtonPressEventHandler(clear_entry);
            applicationMenu.SaveEntryItem.ButtonPressEvent += new ButtonPressEventHandler(save_entry);
            applicationMenu.PreviewEntryItem.ButtonPressEvent += new ButtonPressEventHandler(web_preview);
            applicationMenu.PostEntryItem.ButtonPressEvent += new ButtonPressEventHandler(post_entry);
            applicationMenu.DeleteJournalItem.ButtonPressEvent += new ButtonPressEventHandler(delete_journal);
            applicationMenu.DeleteEntryItem.ButtonPressEvent += new ButtonPressEventHandler(delete_entry);
            applicationMenu.UpdateTagItem.ButtonPressEvent += new ButtonPressEventHandler(update_tags);
            applicationMenu.UpdateJournalItem.ButtonPressEvent += new ButtonPressEventHandler(update_journal);
            applicationMenu.InsertImageItem.ButtonPressEvent += new ButtonPressEventHandler(insert_image);
            #endregion Menu

            outerBox.PackStart(applicationMenu.MB, true, true, 0);

            jsd.JournalSelectD.Hidden += new EventHandler(jsd_hide);
            cd.ConfirmD.Hidden += new EventHandler(cd_hide);
            ltd.TagD.Hidden += new EventHandler(refresh_tags);
            led.EntryD.Hidden += new EventHandler(led_hide);

            #region Tray Icon
            trayIcon = new StatusIcon(new Gdk.Pixbuf("../../Images/tray.png"));
            trayIcon.Visible = true;

            // Show/Hide the window (even from the Panel/Taskbar) when the TrayIcon has been clicked.
            trayIcon.Activate += delegate { window.Visible = !window.Visible; };
            // Show a pop up menu when the icon has been right clicked.
            trayIcon.PopupMenu += RawrUtility.OnTrayIconPopup;

            // A Tooltip for the Icon
            trayIcon.Tooltip = "Rawr!Journal";
            #endregion Tray Icon

            #region Panels
            //mainBox holds all the panels
            HBox mainBox = new HBox(false, 0);
            outerBox.PackStart(mainBox, true, true, 0);

            //the content panels
            VBox leftBox = new VBox(false, 0);
            leftBox.SetSizeRequest(225, 600);
            leftBox.BorderWidth = 5;
            VBox centerBox = new VBox(false, 0);
            centerBox.SetSizeRequest(450, 600);
            centerBox.BorderWidth = 5;
            VBox rightBox = new VBox(true, 0);
            rightBox.SetSizeRequest(200, 600);
            rightBox.BorderWidth = 5;

            mainBox.PackStart(leftBox, true, true, 0);
            mainBox.PackStart(centerBox, true, true, 0);
            mainBox.PackEnd(rightBox, true, true, 0);
            #endregion Panels

            #region List of Journals
            jd.JournalD.Hidden += new EventHandler(refresh_journals);
            Frame journalFrame = new Frame("Journal Explorer");
            journalFrame.SetSizeRequest(225, 375);
            ScrolledWindow journalWindow = new ScrolledWindow();
            List<Journal> journalList = jm.GetAll();
            foreach (Journal j in journalList)
            {
                TreeIter journalIter = journals.AppendValues(j.Name, "", j.Id, 0);
                foreach (JournalEntry je in j.Entries)
                {
                    journals.AppendValues(journalIter, je.EntryDate.ToString(), je.Subject, j.Id, je.Id);
                }
            }
            leftBox.PackStart(journalFrame, false, false, 0);
            journalFrame.Add(journalWindow);
            journalTree = getJournalTree(journals);
            journalTree.RowActivated += new RowActivatedHandler(entry_activated);
            journalWindow.Add(journalTree);
            #endregion List of Journals

            #region Calendar
            Frame calendarFrame = new Frame("Calendar");
            calendarFrame.SetSizeRequest(225, 210);
            calendar.MonthChanged += new EventHandler(month_changed);
            calendar.DaySelected += new EventHandler(calday_selected);
            if (journalList != null && journalList.Count > 0)
                LoadCalendar(journalList[0].Id);

            leftBox.PackEnd(calendarFrame, false, true, 0);
            calendarFrame.Add(calendar);
            #endregion Calendar

            #region Entry Editor
            //containers
            Frame entryFrame = new Frame("Entry");
            centerBox.PackStart(entryFrame, true, true, 0);
            entryFrame.SetSizeRequest(225, 375);
            VBox entryBox = new VBox(false, 0);
            entryBox.BorderWidth = 8;
            entryFrame.Add(entryBox);
            HBox row1 = new HBox(false, 5);
            HBox row2 = new HBox(false, 5);
            HBox row3 = new HBox(false, 5);
            HBox row4 = new HBox(false, 5);
            HBox row5 = new HBox(false, 5);

            //date entry
            Label dateLbl = new Label("Date: ");
            month.AppendText("January");
            month.AppendText("February");
            month.AppendText("March");
            month.AppendText("April");
            month.AppendText("May");
            month.AppendText("June");
            month.AppendText("July");
            month.AppendText("August");
            month.AppendText("September");
            month.AppendText("October");
            month.AppendText("November");
            month.AppendText("December");
            month.Active = DateTime.Now.Month - 1;
            month.Changed += new EventHandler(date_changed);
            year.Text = DateTime.Now.Year.ToString();
            year.WidthChars = 4;
            year.Changed += new EventHandler(date_changed);
            setMonthDropdown();
            day.WidthRequest = 65;
            day.Active = DateTime.Now.Day - 1;
            hours.WidthChars = 3;
            hours.Text = DateTime.Now.Hour.ToString();
            mins.WidthChars = 3;
            mins.Text = DateTime.Now.Minute.ToString();

            //mood
            Label moodLbl = new Label("Mood: ");
            string[] moods = util.GetXmlArray("Data/moods.xml", "moodarray/mood", "name");
            foreach (string mood in moods)
                moodList.AppendText(mood);

            //subject
            Label subjectLbl = new Label("Subject: ");

            //body
            body.WrapMode = WrapMode.Word;
            ScrolledWindow bodyWindow = new ScrolledWindow();
            bodyWindow.Add(body);

            //tags
            Label tagLbl = new Label("Tags: ");

            //music
            Label musicLbl = new Label("Music: ");
            Button detectBtn = new Button("Detect");
            detectBtn.WidthRequest = 100;
            detectBtn.Clicked += new EventHandler(detect_music);

            //buttons
            Button saveBtn = new Button("Save");
            Button previewBtn = new Button("Preview");
            Button postBtn = new Button("Post");
            Button clearBtn = new Button("Clear");
            Button deleteBtn = new Button("Delete");
            saveBtn.Clicked += new EventHandler(save_entry);
            previewBtn.Clicked += new EventHandler(web_preview);
            postBtn.Clicked += new EventHandler(post_entry);
            clearBtn.Clicked += new EventHandler(clear_entry);
            deleteBtn.Clicked += new EventHandler(delete_loadedentry);

            ljid.IsEditable = false;
            ljid.NoShowAll = true;
            entryid.IsEditable = false;
            entryid.NoShowAll = true;

            //packing
            row1.PackStart(dateLbl, false, false, 0);
            row1.PackStart(month, false, false, 0);
            row1.PackStart(day, false, false, 0);
            row1.PackStart(year, false, false, 0);
            row1.PackStart(hours, false, false, 0);
            row1.PackStart(mins, false, false, 0);
            row1.PackStart(moodLbl, false, false, 0);
            row1.PackStart(moodList, true, true, 0);
            row2.PackStart(subjectLbl, false, false, 0);
            row2.PackStart(subjectLine, true, true, 0);
            row3.PackStart(tagLbl, false, false, 0);
            row3.PackStart(tagEntry, true, true, 0);
            row4.PackStart(musicLbl, false, false, 0);
            row4.PackStart(music, true, true, 0);
            row4.PackStart(detectBtn, false, false, 0);
            row5.PackStart(saveBtn, true, true, 0);
            row5.PackStart(previewBtn, true, true, 0);
            row5.PackStart(postBtn, true, true, 0);
            row5.PackStart(clearBtn, true, true, 0);
            row5.PackStart(deleteBtn, true, true, 0);
            row5.PackStart(ljid, false, false, 0);
            row5.PackStart(entryid, false, false, 0);
            entryBox.PackStart(row1, false, false, 5);
            entryBox.PackStart(row2, false, false, 5);
            entryBox.PackStart(bodyWindow, true, true, 0);
            entryBox.PackStart(row3, false, false, 5);
            entryBox.PackStart(row4, false, false, 5);
            entryBox.PackStart(row5, false, false, 5);
            #endregion Entry Editor

            #region Search
            //find results
            Frame findFrame = new Frame("Search");
            findFrame.SetSizeRequest(225, 210);
            VBox findBox = new VBox(false, 5);
            findBox.BorderWidth = 8;
            HBox criteriaBox = new HBox(false, 0);
            Label keywordLbl = new Label("Keyword: ");
            Button searchBtn = new Button("Search");
            searchBtn.Clicked += new EventHandler(search_entries);
            searchBtn.WidthRequest = 75;
            ScrolledWindow resultsWindow = new ScrolledWindow();
            resultsTree = getJournalTree(results);
            resultsTree.RowActivated += new RowActivatedHandler(entry_activated);

            centerBox.PackEnd(findFrame, false, true, 0);
            findFrame.Add(findBox);
            criteriaBox.PackStart(keywordLbl, false, false, 0);
            criteriaBox.PackStart(keyword, true, true, 0);
            criteriaBox.PackStart(searchBtn, false, false, 0);
            findBox.PackStart(criteriaBox, false, true, 0);
            findBox.PackStart(resultsWindow, true, true, 0);
            resultsWindow.Add(resultsTree);
            #endregion Search

            #region Tag List
            td.TagD.Hidden += new EventHandler(refresh_tags);
            Frame tagFrame = new Frame("Tag List");
            VBox tagBox = new VBox(false, 0);
            HBox tagBtnBox = new HBox(true, 0);
            Button newTagBtn = new Button("New");
            newTagBtn.Clicked += new EventHandler(new_tag);
            Button editTagBtn = new Button("Edit");
            editTagBtn.Clicked += new EventHandler(edit_tag);
            Button deleteTagBtn = new Button("Delete");
            deleteTagBtn.Clicked += new EventHandler(delete_tag);
            ScrolledWindow tagWindow = new ScrolledWindow();
            List<Tag> tagList = tm.GetAll();
            foreach (Tag tag in tagList)
                tags.AppendValues(tag.Id, tag.Name);
            tagTree = getGenericTree(tags);
            tagTree.HeadersVisible = false;
            tagTree.Selection.Changed += new EventHandler(tag_select);
            tagTree.RowActivated += new RowActivatedHandler(tag_activated);

            rightBox.PackStart(tagFrame, false, true, 0);
            tagFrame.Add(tagBox);
            tagBox.PackStart(tagWindow, true, true, 0);
            tagBox.PackStart(tagBtnBox, false, true, 0);
            tagBtnBox.PackStart(newTagBtn, true, true, 0);
            tagBtnBox.PackStart(editTagBtn, true, true, 0);
            tagBtnBox.PackStart(deleteTagBtn, true, true, 0);
            tagWindow.Add(tagTree);
            #endregion Tag List

            #region Feed Window
            Frame rssFrame = new Frame("Rss Feeds");
            ScrolledWindow rssWindow = new ScrolledWindow();
            TreeStore feeds = new TreeStore(typeof(string), typeof(string), typeof(int));
            TreeIter feedIter = feeds.AppendValues("Friends Page");
            feeds.AppendValues(feedIter, "7/25/2008", "Yay!", 1);

            rightBox.PackEnd(rssFrame, false, true, 0);
            rssFrame.Add(rssWindow);
            rssWindow.Add(getJournalTree(feeds));
            #endregion Feed Window

            window.ShowAll();
            Journal journal = GetSelectedJournal();
        }
        Journal GetSelectedJournal()
        {
            TreeSelection selection = journalTree.Selection;
            TreePath[] paths = selection.GetSelectedRows();
            TreeIter iter = new TreeIter();
            int journalId = 0;
            Journal journal = new Journal();
            if (paths.Length > 0)
            {
                if (journalTree.Model.GetIter(out iter, paths[0]))
                {
                    journalId = Convert.ToInt32(journalTree.Model.GetValue(iter, 2).ToString());
                    journal = jm.GetById(journalId);
                }
            }
            return journal;
        }
        JournalEntry GetSelectedEntry()
        {
            TreeSelection selection = journalTree.Selection;
            TreePath[] paths = selection.GetSelectedRows();
            TreeIter iter = new TreeIter();
            int entryId = 0;
            JournalEntry entry = new JournalEntry();
            if (paths.Length > 0)
            {
                if (journalTree.Model.GetIter(out iter, paths[0]))
                {
                    entryId = Convert.ToInt32(journalTree.Model.GetValue(iter, 3).ToString());
                    if (entryId > 0)
                        entry = em.GetById(entryId);
                }
            }
            return entry;
        }
        void setMonthDropdown()
        {
            int active = day.Active;
            day.Clear();
            CellRendererText cell = new CellRendererText();
            day.PackStart(cell, false);
            day.AddAttribute(cell, "text", 0);
            ListStore dayStore = new ListStore(typeof(string));
            day.Model = dayStore;
            int yearN = Convert.ToInt32(year.Text.ToString());
            for (int i = 1; i <= DateTime.DaysInMonth(yearN, month.Active + 1); i++)
            {
                dayStore.AppendValues(i.ToString());
            }
            day.Active = active;
        }
        static TreeView getJournalTree(TreeStore store)
        {
            TreeView tree = new TreeView(store);
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
            tree.AppendColumn(dateCol);
            tree.AppendColumn(subjectCol);
            tree.AppendColumn(idCol);
            dateCol.AddAttribute(dateCell, "text", 0);
            subjectCol.AddAttribute(subjectCell, "text", 1);
            idCol.AddAttribute(idColCell, "text", 2);
            return tree;
        }
        static TreeView getGenericTree(TreeStore store)
        {
            TreeView tree = new TreeView(store);
            TreeViewColumn idCol = new TreeViewColumn();
            idCol.Visible = false;
            TreeViewColumn textCol = new TreeViewColumn();
            CellRendererText idColCell = new CellRendererText();
            CellRendererText textColCell = new CellRendererText();
            idCol.PackStart(idColCell, true);
            textCol.PackStart(textColCell, true);
            tree.AppendColumn(idCol);
            tree.AppendColumn(textCol);
            idCol.AddAttribute(idColCell, "text", 0);
            textCol.AddAttribute(textColCell, "text", 1);
            return tree;
        }
        void LoadCalendar(int journalId)
        {
            calendar.ClearMarks();
            List<JournalEntry> entries = em.GetByJournalId(journalId);
            foreach (JournalEntry entry in entries)
            {
                if (entry.EntryDate.Year == calendar.Year && entry.EntryDate.Month == calendar.Month + 1)
                    calendar.MarkDay(Convert.ToUInt32(entry.EntryDate.Day));
            }
        }
        void LoadEntry(JournalEntry entry)
        {
            ljid.Text = entry.LjId.ToString();
            entryid.Text = entry.Id.ToString();
            month.Active = entry.EntryDate.Month - 1;
            day.Active = entry.EntryDate.Day - 1;
            year.Text = entry.EntryDate.Year.ToString();
            hours.Text = entry.EntryDate.Hour.ToString();
            mins.Text = entry.EntryDate.Minute.ToString();
            moodList.Active = util.SetActiveText(moodList, entry.Mood);
            if (moodList.Active == -1)
                moodList.Entry.Text = entry.Mood;
            subjectLine.Text = entry.Subject;
            bodyBuffer.Text = entry.Body;
            music.Text = entry.Music;
            tagEntry.Text = (new TagManager()).CreateString(entry.Tags);

            TextIter startiter = new TextIter();
            TextIter startiter2 = new TextIter();
            TextIter enditer = new TextIter();
            TextIter enditer2 = bodyBuffer.StartIter;
            while (enditer2.ForwardSearch("<pixbuf file='", TextSearchFlags.TextOnly, out startiter, out enditer, bodyBuffer.EndIter))
            {
                bool found2 = enditer.ForwardSearch("'>", TextSearchFlags.TextOnly, out startiter2, out enditer2, bodyBuffer.EndIter);
                if (found2)
                    bodyBuffer.InsertPixbuf(ref enditer2, new Gdk.Pixbuf(@"UploadPath\" + bodyBuffer.GetText(startiter2, enditer, false)));
            }
        }
        JournalEntry CollectEntry()
        {
            JournalEntry entry = new JournalEntry();
            if (entryid.Text.Trim().Length > 0 && Convert.ToInt32(entryid.Text) > 0)
            {
                entry = em.GetById(Convert.ToInt32(entryid.Text));
            }
            if (ljid.Text.Trim().Length > 0 && Convert.ToInt32(ljid.Text) > 0)
            {
                entry.IsLj = true;
                entry.LjId = Convert.ToInt32(ljid.Text);
            }
            entry.EntryDate = new DateTime(Convert.ToInt32(year.Text),
                Convert.ToInt32(month.Active + 1), Convert.ToInt32(day.ActiveText),
                Convert.ToInt32(hours.Text), Convert.ToInt32(mins.Text), 0);
            entry.Mood = moodList.ActiveText;
            entry.Music = music.Text;
            entry.Subject = subjectLine.Text;
            entry.Body = bodyBuffer.Text;
            entry.Tags = tm.ProcessTagString(tagEntry.Text);
            return entry;
        }
        void ClearEntry()
        {
            year.Text = DateTime.Now.Year.ToString();
            month.Active = DateTime.Now.Month - 1;
            day.Active = DateTime.Now.Day - 1;
            hours.Text = DateTime.Now.Hour.ToString();
            mins.Text = DateTime.Now.Minute.ToString();
            moodList.Active = -1;
            moodList.Entry.Text = "";
            subjectLine.Text = "";
            bodyBuffer.Clear();
            music.Text = "";
            tagEntry.Text = "";
            ljid.Text = "";
            entryid.Text = "";
        }
        #endregion Methods

        #region Event Handlers
        static void delete_event(object obj, EventArgs args)
        {
            trayIcon.Visible = false; // this is necessary to properly clean the icon after exiting
            Application.Quit();
        }
        void cd_hide(object obj, EventArgs args)
        {
            if (cd.IsConfirmed)
                ClearEntry();
        }
        void month_changed(object obj, EventArgs args)
        {
            LoadCalendar((GetSelectedJournal()).Id);
        }
        void calday_selected(object obj, EventArgs args)
        {
            List<JournalEntry> entries = em.GetByDate((GetSelectedJournal()).Id, calendar.Year, calendar.Month + 1, calendar.Day);
            results.Clear();
            int journalid = 0;
            TreeIter journalIter = new TreeIter();
            Journal journal = new Journal();
            foreach (JournalEntry entry in entries)
            {
                if (entry.JournalId != journalid)
                {
                    journal = jm.GetById(entry.JournalId);
                    journalIter = results.AppendValues(journal.Name, "", journal.Id, 0);
                    journalid = entry.JournalId;
                }
                results.AppendValues(journalIter, entry.EntryDate.ToString(), entry.Subject, entry.JournalId, entry.Id);
            }
        }
        void led_hide(object obj, EventArgs args)
        {
            refresh_journals(null, null);
        }

        #region Journal Handlers
        void edit_journal(object obj, EventArgs args)
        {
            jd.IsNew = false;
            jd.Initialize();
            Journal journal = GetSelectedJournal();
            jd.Id = journal.Id;
            jd.NameEntry.Text = journal.Name;
            jd.UsernameEntry.Text = journal.Username;
            jd.PasswordEntry.Text = journal.Password;
            jd.KeyEntry.Text = journal.Key;
            jd.JournalD.Show();
        }
        void refresh_journals(object obj, EventArgs args)
        {
            journals.Clear();
            List<Journal> journalList = jm.GetAll();
            foreach (Journal j in journalList)
            {
                TreeIter journalIter = journals.AppendValues(j.Name, "", j.Id, 0);
                foreach (JournalEntry je in j.Entries)
                {
                    journals.AppendValues(journalIter, je.EntryDate.ToString(), je.Subject, j.Id, je.Id);
                }
            }
        }
        void jsd_hide(object obj, EventArgs args)
        {
            JournalEntry entry = CollectEntry();
            Journal journal = jm.GetByName(jsd.JournalList.ActiveText);
            if (journal.Id != 0)
            {
                entry.JournalId = journal.Id;
                em.Insert(entry);
                refresh_journals(null, null);
                ClearEntry();
            }
        }
        void manage_friends(object obj, EventArgs args)
        {
            Journal journal = GetSelectedJournal();
            fd.Id = journal.Id;
            fd.Initialize();
            fd.FriendD.Show();
        }
        void delete_journal(object obj, EventArgs args)
        {
            Journal journal = GetSelectedJournal();
            jm.Delete(journal.Id);
            refresh_journals(null, null);
        }
        void update_journal(object obj, EventArgs args)
        {
            led.Id = (GetSelectedJournal()).Id;
            led.Initialize();
            led.EntryD.Show();
        }
        #endregion Journal Handlers

        #region Entry Handlers
        void date_changed(object obj, EventArgs args)
        {
            setMonthDropdown();
        }
        void detect_music(object obj, EventArgs args)
        {
            iTunesAppClass myItunes = new iTunesAppClass();
            IITTrack myTrack = myItunes.CurrentTrack;
            if (myTrack != null)
                music.Text = myTrack.Artist + " - " + myTrack.Name;
        }
        void entry_activated(object obj, RowActivatedArgs args)
        {
            TreeView tree = (TreeView)obj;
            TreeIter iter = new TreeIter();
            int id = 0;
            int journalId = 0;
            if (tree.Model.GetIter(out iter, args.Path))
            {
                id = Convert.ToInt32(tree.Model.GetValue(iter, 3).ToString());
                if (id > 0)
                {
                    JournalEntry entry = em.GetById(id);
                    if (entry.Id > 0)
                        LoadEntry(entry);
                }
                journalId = Convert.ToInt32(tree.Model.GetValue(iter, 2).ToString());
                if (journalId > 0)
                {
                    LoadCalendar(journalId);
                }
            }
        }
        void save_entry(object obj, EventArgs args)
        {
            if (entryid.Text.Trim().Length == 0 || Convert.ToInt32(entryid.Text) == 0)
            {
                jsd.Initialize();
                jsd.JournalSelectD.Show();
            }
            else
            {
                JournalEntry entry = CollectEntry();
                em.Update(entry);
                refresh_journals(null, null);
                ClearEntry();
            }
        }
        void web_preview(object obj, EventArgs args)
        {
            //string MozEnvVar = System.Environment.GetEnvironmentVariable("GECKOSHILLA_BASEPATH");
            //if (MozEnvVar != null && MozEnvVar.Length != 0)
            //{
            //    Gecko.WebControl.CompPath = MozEnvVar;
            //}
            //Dialog webD = new Dialog();
            //webD.Title = "test";
            //Gecko.WebControl wc = new Gecko.WebControl();
            //webD.VBox.PackStart(wc, true, true, 2);
            //wc.LoadUrl("http://www.yahoo.com");
            //webD.ShowAll();

            //PreviewWindow pw = new PreviewWindow();
            //pw.MarkupString = bodyBuffer.Text;
            //pw.Initialize();
        }
        void post_entry(object obj, EventArgs args)
        {
            pd.JournalEntry = CollectEntry();
            pd.JournalEntry.JournalId = (GetSelectedJournal()).Id;
            pd.JournalEntry.Id = em.Insert(pd.JournalEntry);
            pd.Initialize();
            pd.PostD.Show();
        }
        void clear_entry(object obj, EventArgs args)
        {
            cd.Initialize();
            cd.ConfirmD.Show();
        }
        void delete_loadedentry(object obj, EventArgs args)
        {
            JournalEntry entry = CollectEntry();
            em.Delete(entry.Id);
            ClearEntry();
            refresh_journals(null, null);
        }
        void search_entries(object obj, EventArgs args)
        {
            List<JournalEntry> entries = em.Search(keyword.Text);
            results.Clear();
            int journalid = 0;
            TreeIter journalIter = new TreeIter();
            Journal journal = new Journal();
            foreach (JournalEntry entry in entries)
            {
                if (entry.JournalId != journalid)
                {
                    journal = jm.GetById(entry.JournalId);
                    journalIter = results.AppendValues(journal.Name, "", journal.Id, 0);
                    journalid = entry.JournalId;
                }
                results.AppendValues(journalIter, entry.EntryDate.ToString(), entry.Subject, entry.JournalId, entry.Id);
            }
        }
        void import_entries(object obj, EventArgs args)
        {
            List<JournalEntry> entries = new List<JournalEntry>();
            FileChooserDialog fc = new FileChooserDialog("Import", window, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Select", ResponseType.Accept);
            if (fc.Run() == (int)ResponseType.Accept)
                entries = util.ImportEntries(fc.Filename, GetSelectedJournal());
            fc.Destroy();
            foreach (JournalEntry entry in entries)
                em.Insert(entry);
            refresh_journals(null, null);
        }
        void export_entries(object obj, EventArgs args)
        {
            FileChooserDialog fc = new FileChooserDialog("Export", window, FileChooserAction.Save, "Cancel", ResponseType.Cancel, "Save", ResponseType.Apply);
            if (fc.Run() == (int)ResponseType.Apply)
                util.ExportEntries(fc.Filename, GetSelectedJournal());
            fc.Destroy();
        }
        void delete_entry(object obj, EventArgs args)
        {
            JournalEntry entry = GetSelectedEntry();
            if (entry.Id > 0)
                em.Delete(entry.Id);
            refresh_journals(null, null);
        }
        void insert_image(object obj, EventArgs args)
        {
            string filename = "";
            FileChooserDialog fc = new FileChooserDialog("Insert Picture", window, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Select", ResponseType.Accept);
            if (fc.Run() == (int)ResponseType.Accept)
            {
                if (!Directory.Exists("UploadPath"))
                {
                    Directory.CreateDirectory("UploadPath");
                }
                filename = fc.Filename.Substring(fc.Filename.LastIndexOf('\\') + 1);
                File.Copy(fc.Filename, @"UploadPath\" + filename);
                TextIter iter = body.Buffer.GetIterAtOffset(body.Buffer.CursorPosition);
                bodyBuffer.InsertPixbuf(ref iter, new Gdk.Pixbuf(@"UploadPath\" + filename));

                iter = bodyBuffer.StartIter;
                while (iter.Pixbuf == null && !iter.IsEnd)
                    iter.ForwardCursorPosition();
                if (iter.Pixbuf != null)
                    bodyBuffer.Insert(ref iter, "<pixbuf file='" + filename + "'>");
            }
            fc.Destroy();
        }
        #endregion Entry Handlers

        #region Tag Handlers
        void new_tag(object obj, EventArgs args)
        {
            td.IsNew = true;
            td.Initialize();
            td.TagD.Show();
        }
        void edit_tag(object obj, EventArgs args)
        {
            TreeSelection selection = tagTree.Selection;
            TreePath[] paths = selection.GetSelectedRows();
            TreeIter iter = new TreeIter();
            string text = "";
            if (tagTree.Model.GetIter(out iter, paths[0]))
            {
                text = tagTree.Model.GetValue(iter, 1).ToString();
                td.IsNew = false;
                td.TagEntry.Text = text;
                td.InitialText = text;
                td.Initialize();
                td.TagD.Show();
            }
        }
        void delete_tag(object obj, EventArgs args)
        {
            TreeSelection selection = tagTree.Selection;
            TreePath[] paths = selection.GetSelectedRows();
            TreeIter iter = new TreeIter();
            int[] ids = new int[paths.Length];
            for (var i = 0; i < paths.Length; i++)
            {
                if (tagTree.Model.GetIter(out iter, paths[i]))
                {
                    //ids[i] = Convert.ToInt32(tagTree.Model.GetValue(iter, 0).ToString());
                    tm.Delete(Convert.ToInt32(tagTree.Model.GetValue(iter, 0).ToString()));
                }
            }
            tags.Clear();
            List<Tag> tagList = tm.GetAll();
            foreach (Tag t in tagList)
                tags.AppendValues(t.Id, t.Name);
        }
        void refresh_tags(object obj, EventArgs args)
        {
            tags.Clear();
            List<Tag> tagList = tm.GetAll();
            foreach (Tag t in tagList)
                tags.AppendValues(t.Id, t.Name);
        }
        void tag_activated(object obj, RowActivatedArgs args)
        {
            TreeIter iter = new TreeIter();
            string text = "";
            if (tagTree.Model.GetIter(out iter, args.Path))
            {
                text = tagTree.Model.GetValue(iter, 1).ToString();
                if (tagEntry.Text == "")
                    tagEntry.Text = text;
                else
                    tagEntry.Text += ", " + text;
            }
        }
        void update_tags(object obj, EventArgs args)
        {
            ltd.Id = (GetSelectedJournal()).Id;
            ltd.Initialize();
            ltd.TagD.Show();
        }
        void tag_select(object obj, EventArgs args)
        {
            TreeSelection selection = tagTree.Selection;
            TreePath[] paths = selection.GetSelectedRows();
            TreeIter iter = new TreeIter();
            int tagid = 0;
            if (tagTree.Model.GetIter(out iter, paths[0]))
                tagid = Convert.ToInt32(tagTree.Model.GetValue(iter, 0).ToString());
            if (tagid > 0)
            {
                List<JournalEntry> entries = em.GetByTagId(tagid);
                results.Clear();
                int journalid = 0;
                TreeIter journalIter = new TreeIter();
                Journal journal = new Journal();
                foreach (JournalEntry entry in entries)
                {
                    if (entry.JournalId != journalid)
                    {
                        journal = jm.GetById(entry.JournalId);
                        journalIter = results.AppendValues(journal.Name, "", journal.Id, 0);
                        journalid = entry.JournalId;
                    }
                    results.AppendValues(journalIter, entry.EntryDate.ToString(), entry.Subject, entry.JournalId, entry.Id);
                }
            }
        }
        #endregion Tag Handlers
        #endregion Event Handlers
    }
}