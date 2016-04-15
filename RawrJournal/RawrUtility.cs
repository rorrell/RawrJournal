using System;
using System.Xml;
using Mono.Data.SqliteClient;
using Gtk;
using System.Collections;
using RawrJournal.Classes;
using System.IO;
using System.Collections.Generic;

namespace RawrJournal
{
    public class RawrUtility
    {
        #region Properties
        private enum Groups : uint
        {
        }
        #endregion Properties

        #region Public Methods
        public string[] GetXmlArray(string filename, string tagPath, string tagName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("../../" + filename);
            XmlNodeList nodeList = doc.SelectNodes(tagPath);
            string[] values = new string[nodeList.Count];
            int count = 0;
            foreach (XmlNode node in nodeList)
            {
                values[count] = node.Attributes[tagName].Value;
                count++;
            }
            return values;
        }

        public SqliteConnection SqliteConnect()
        {
            SqliteConnection cnn = new SqliteConnection("URI=file:rawrjournal.s3db");
            return cnn;
        }

        public int SetActiveText(ComboBox list, string text)
        {
            int index = -1;
            TreeIter iter = new TreeIter();
            if (list.Model.GetIterFirst(out iter))
            {
                for (var i = 0; i < list.Model.IterNChildren(); i++)
                {
                    if (list.Model.GetValue(iter, 0).ToString() == text)
                        index = i;
                    list.Model.IterNext(ref iter);
                }
            }
            return index;
        }

        public static void OnTrayIconPopup(object o, EventArgs args)
        { // this function is called by ApplicationWindow.cs
            Menu trayMenu = new Menu();
            ImageMenuItem menuItemQuit = new ImageMenuItem("Exit");
            Gtk.Image appimg = new Gtk.Image(Stock.Quit, IconSize.Menu);
            menuItemQuit.Image = appimg;
            trayMenu.Add(menuItemQuit);
            // Quit the application when quit has been clicked.
            menuItemQuit.Activated += ApplicationMenu.exit_Click;
            trayMenu.ShowAll();
            trayMenu.Popup();
        }

        public DateTime ConvertDateString(string datestring)
        {
            DateTime date = new DateTime(Convert.ToInt32(datestring.Substring(0, 4)),
                Convert.ToInt32(datestring.Substring(5, 2)), Convert.ToInt32(datestring.Substring(8, 2)),
                Convert.ToInt32(datestring.Substring(11, 2)), Convert.ToInt32(datestring.Substring(14, 2)), 0);
            return date;
        }

        public string CreateDateString(DateTime date)
        {
            string datestring = date.Year.ToString() + "-" +
                date.Month.ToString().PadLeft(2, '0') + "-" +
                date.Day.ToString().PadLeft(2, '0') + " " +
                date.Hour.ToString().PadLeft(2, '0') + ":" +
                date.Minute.ToString().PadLeft(2, '0') + ":" +
                date.Second.ToString().PadLeft(2, '0');
            return datestring;
        }

        public List<JournalEntry> ImportEntries(string filepath, Journal journal)
        {
            //string filename = filepath.Substring(filepath.LastIndexOf('\\') + 1);
            //string path = filepath.Substring(0, filepath.LastIndexOf('\\'));
            //string sourceFile = System.IO.Path.Combine(path, filename);
            //string destFile = System.IO.Path.Combine("UploadPath", filename);
            //File.Copy(sourceFile, destFile, true);
            //XmlTextReader reader = new XmlTextReader(@"UploadPath/test.xml");
            XmlTextReader reader = new XmlTextReader(filepath);
            List<JournalEntry> entries = new List<JournalEntry>();
            JournalEntry entry = new JournalEntry();
            bool first = true;
            string tag = "";
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.
                        tag = reader.Name;
                        switch (reader.Name)
                        {
                            case "comment":
                                reader.Skip();
                                break;
                            case "entry":
                                if (first)
                                {
                                    first = false;
                                    entry.JournalId = journal.Id;
                                }
                                else
                                {
                                    entries.Add(entry);
                                    entry = new JournalEntry();
                                    entry.JournalId = journal.Id;
                                }
                                break;
                            default:
                                reader.MoveToContent();
                                break;
                        }
                        break;
                    case XmlNodeType.Text: //Display the text in each element.
                        switch (tag)
                        {
                            case "itemid":
                                reader.MoveToContent();
                                entry.IsLj = true;
                                entry.LjId = Convert.ToInt32(reader.Value);
                                break;
                            case "eventtime":
                                entry.EntryDate = ConvertDateString(reader.Value);
                                break;
                            case "subject":
                                entry.Subject = reader.Value;
                                break;
                            case "event":
                                entry.Body = reader.Value;
                                break;
                            case "current_mood":
                                entry.Mood = reader.Value;
                                break;
                            case "current_music":
                                entry.Music = reader.Value;
                                break;
                            case "taglist":
                                entry.Tags = (new TagManager()).ProcessTagString(reader.Value);
                                break;
                        }
                        break;
                    case XmlNodeType.EndElement: //Display the end of the element.
                        break;
                }
            }
            reader.Close();
            //File.Delete(destFile);
            return entries;
        }

        public void ExportEntries(string filepath, Journal journal)
        {
            XmlTextWriter writer = new XmlTextWriter(filepath, System.Text.Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();
            writer.WriteStartElement("livejournal");

            foreach (JournalEntry entry in journal.Entries)
            {
                writer.WriteStartElement("entry");
                if (entry.IsLj)
                {
                    writer.WriteStartElement("itemid");
                    writer.WriteString(entry.LjId.ToString());
                    writer.WriteEndElement();
                }
                writer.WriteStartElement("eventtime");
                writer.WriteString(CreateDateString(entry.EntryDate));
                writer.WriteEndElement();
                writer.WriteStartElement("subject");
                writer.WriteString(entry.Subject);
                writer.WriteEndElement();
                writer.WriteStartElement("event");
                writer.WriteString(entry.Body);
                writer.WriteEndElement();
                if (entry.Mood.Trim().Length > 0)
                {
                    writer.WriteStartElement("current_mood");
                    writer.WriteString(entry.Mood);
                    writer.WriteEndElement();
                }
                if (entry.Music.Trim().Length > 0)
                {
                    writer.WriteStartElement("current_music");
                    writer.WriteString(entry.Music);
                    writer.WriteEndElement();
                }
                if (entry.Tags.Count > 0)
                {
                    writer.WriteStartElement("taglist");
                    writer.WriteString((new TagManager()).CreateString(entry.Tags));
                    writer.WriteEndElement();
                }
            }
            writer.Flush();
            writer.Close();
        }
        #endregion Public Methods
    }
}
