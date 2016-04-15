using System;
using System.Collections.Generic;
using Mono.Data.SqliteClient;
using System.Data;

namespace RawrJournal.Classes
{
    public class JournalEntryManager
    {
        #region Properties
        private static RawrUtility util = new RawrUtility();
        private SqliteConnection cnn = util.SqliteConnect();
        #endregion Properties

        #region Public Methods
        public List<JournalEntry> GetAll()
        {
            cnn.Open();
            SqliteCommand cmd = new SqliteCommand("SELECT * FROM Entry ORDER BY Year DESC, Month DESC, Day DESC, Hour DESC, Minute DESC", cnn);
            SqliteDataReader reader = cmd.ExecuteReader();
            List<JournalEntry> journalentries = ReadJournalEntries(reader);
            cnn.Close();
            return journalentries;
        }
        public List<JournalEntry> Search(string keyword)
        {
            cnn.Open();
            SqliteCommand cmd = new SqliteCommand("SELECT * FROM Entry WHERE Subject LIKE '%" + keyword + "%' OR Body LIKE '%" + keyword + "%' ORDER BY Year DESC, Month DESC, Day DESC, Hour DESC, Minute DESC", cnn);
            SqliteDataReader reader = cmd.ExecuteReader();
            List<JournalEntry> journalentries = ReadJournalEntries(reader);
            cnn.Close();
            return journalentries;
        }
        public JournalEntry GetById(int id)
        {
            cnn.Open();
            SqliteCommand cmd = new SqliteCommand("SELECT * FROM Entry WHERE Id=" + id.ToString(), cnn);
            SqliteDataReader reader = cmd.ExecuteReader();
            JournalEntry journalentry = ReadJournalEntry(reader);
            cnn.Close();
            return journalentry;
        }
        public List<JournalEntry> GetByJournalId(int journalId)
        {
            RawrUtility util = new RawrUtility();
            SqliteConnection cnn = util.SqliteConnect();
            cnn.Open();
            SqliteCommand cmd = new SqliteCommand("SELECT * FROM Entry WHERE JournalId=" + journalId.ToString() + " ORDER BY Year DESC, Month DESC, Day DESC, Hour DESC, Minute DESC", cnn);
            SqliteDataReader reader = cmd.ExecuteReader();
            List<JournalEntry> entries = ReadJournalEntries(reader);
            cnn.Close();
            return entries;
        }
        public List<JournalEntry> GetByDate(int journalId, int year, int month, int day)
        {
            RawrUtility util = new RawrUtility();
            SqliteConnection cnn = util.SqliteConnect();
            cnn.Open();
            SqliteCommand cmd = new SqliteCommand("SELECT * FROM Entry WHERE JournalId=" + journalId + " AND Year=" + year + " AND Month=" + month + " AND Day=" + day, cnn);
            SqliteDataReader reader = cmd.ExecuteReader();
            List<JournalEntry> entries = ReadJournalEntries(reader);
            cnn.Close();
            return entries;
        }
        public List<JournalEntry> GetByTagId(int tagid)
        {
            RawrUtility util = new RawrUtility();
            SqliteConnection cnn = util.SqliteConnect();
            cnn.Open();
            SqliteCommand cmd = new SqliteCommand("SELECT * FROM Entry, EntryTag WHERE EntryTag.EntryId = Entry.Id AND EntryTag.TagId = " + tagid + " ORDER BY JournalId ASC, Year DESC, Month DESC, Day DESC, Hour DESC, Minute DESC", cnn);
            SqliteDataReader reader = cmd.ExecuteReader();
            List<JournalEntry> entries = ReadJournalEntries(reader);
            cnn.Close();
            return entries;
        }
        public int Insert(JournalEntry journalentry)
        {
            cnn.Open();
            SqliteCommand cmd = new SqliteCommand("INSERT INTO Entry (JournalId, IsLj, LjId, Subject, Body, Year, Month, Day, Hour, Minute, Mood, Music) VALUES (" + journalentry.JournalId + ", '" + journalentry.IsLj.ToString() + "', " + journalentry.LjId + ", :subject, :body, " + journalentry.EntryDate.Year.ToString() + ", " + journalentry.EntryDate.Month.ToString() + ", " + journalentry.EntryDate.Day.ToString() + ", " + journalentry.EntryDate.Hour.ToString() + ", " + journalentry.EntryDate.Minute.ToString() + ", :mood, :music)", cnn);
            SqliteParameter param = new SqliteParameter(":body", DbType.String);
            param.Value = journalentry.Body;
            cmd.Parameters.Add(param);
            SqliteParameter param2 = new SqliteParameter(":ljid", DbType.Int32);
            if (journalentry.LjId == 0)
                cmd.CommandText = "INSERT INTO Entry (JournalId, IsLj, Subject, Body, Year, Month, Day, Hour, Minute, Mood, Music) VALUES (" + journalentry.JournalId + ", '" + journalentry.IsLj.ToString() + "', :subject, :body, " + journalentry.EntryDate.Year.ToString() + ", " + journalentry.EntryDate.Month.ToString() + ", " + journalentry.EntryDate.Day.ToString() + ", " + journalentry.EntryDate.Hour.ToString() + ", " + journalentry.EntryDate.Minute.ToString() + ", :mood, :music)";
            cmd.Parameters.Add(param2);
            SqliteParameter param3 = new SqliteParameter(":subject", DbType.String);
            param3.Value = journalentry.Subject;
            cmd.Parameters.Add(param3);
            SqliteParameter param4 = new SqliteParameter(":mood", DbType.String);
            param4.Value = journalentry.Mood;
            cmd.Parameters.Add(param4);
            SqliteParameter param5 = new SqliteParameter(":music", DbType.String);
            param5.Value = journalentry.Music;
            cmd.Parameters.Add(param5);
            try { cmd.ExecuteNonQuery(); }
            catch { }
            int entryid = cmd.LastInsertRowID();
            if (journalentry.Tags.Count > 0)
            {
                foreach (Tag tag in journalentry.Tags)
                {
                    SqliteCommand cmd2 = new SqliteCommand("INSERT INTO EntryTag (EntryId, TagId) VALUES (" + entryid + ", " + tag.Id + ")", cnn);
                    try { cmd2.ExecuteNonQuery(); }
                    catch { }
                }
            }
            cnn.Close();
            return entryid;
        }
        public void Delete(int id)
        {
            cnn.Open();
            SqliteCommand cmd = new SqliteCommand("DELETE FROM Entry WHERE Id=" + id.ToString(), cnn);
            try { cmd.ExecuteNonQuery(); }
            catch { }
            SqliteCommand cmd2 = new SqliteCommand("DELETE FROM EntryTag WHERE EntryId=" + id.ToString(), cnn);
            try { cmd2.ExecuteNonQuery(); }
            catch { }
            cnn.Close();
        }
        public void Update(JournalEntry journalentry)
        {
            cnn.Open();
            string ljid = null;
            if (journalentry.LjId > 0)
                ljid = journalentry.LjId.ToString();
            SqliteCommand cmd = new SqliteCommand("UPDATE Entry SET JournalId=" + journalentry.JournalId + ", IsLj='" + journalentry.IsLj.ToString() + "', LjId=" + journalentry.LjId + ", Subject=:subject, Body=:body, Year=" + journalentry.EntryDate.Year.ToString() + ", Month=" + journalentry.EntryDate.Month.ToString() + ", Day=" + journalentry.EntryDate.Day.ToString() + ", Hour=" + journalentry.EntryDate.Hour.ToString() + ", Minute=" + journalentry.EntryDate.Minute + ", Mood=:mood, Music=:music WHERE Id=" + journalentry.Id.ToString(), cnn);
            SqliteParameter param = new SqliteParameter(":body", DbType.String);
            param.Value = journalentry.Body;
            cmd.Parameters.Add(param);
            if (journalentry.LjId == 0)
                cmd.CommandText = "UPDATE Entry SET JournalId=" + journalentry.JournalId + ", IsLj='" + journalentry.IsLj.ToString() + "', Subject=:subject, Body=:body, Year=" + journalentry.EntryDate.Year.ToString() + ", Month=" + journalentry.EntryDate.Month.ToString() + ", Day=" + journalentry.EntryDate.Day.ToString() + ", Hour=" + journalentry.EntryDate.Hour.ToString() + ", Minute=" + journalentry.EntryDate.Minute + ", Mood=:mood, Music=:music WHERE Id=" + journalentry.Id.ToString();
            SqliteParameter param3 = new SqliteParameter(":subject", DbType.String);
            param3.Value = journalentry.Subject;
            cmd.Parameters.Add(param3);
            SqliteParameter param4 = new SqliteParameter(":mood", DbType.String);
            param4.Value = journalentry.Mood;
            cmd.Parameters.Add(param4);
            SqliteParameter param5 = new SqliteParameter(":music", DbType.String);
            param5.Value = journalentry.Music;
            cmd.Parameters.Add(param5);
            try { cmd.ExecuteNonQuery(); }
            catch { }
            SqliteCommand cmd2 = new SqliteCommand("DELETE FROM EntryTag WHERE EntryId=" + journalentry.Id, cnn);
            try { cmd2.ExecuteNonQuery(); }
            catch { }
            if (journalentry.Tags.Count > 0)
            {
                foreach (Tag tag in journalentry.Tags)
                {
                    SqliteCommand cmd3 = new SqliteCommand("INSERT INTO EntryTag (EntryId, TagId) VALUES (" + journalentry.Id + ", " + tag.Id + ")", cnn);
                    try { cmd3.ExecuteNonQuery(); }
                    catch { }
                }
            }
            cnn.Close();
        }
        #endregion Public Methods

        #region Private Methods
        private JournalEntry ReadJournalEntry(SqliteDataReader reader)
        {
            JournalEntry journalentry = new JournalEntry();
            while (reader.Read())
            {
                journalentry.Id = reader.GetInt32(0);
                journalentry.JournalId = reader.GetInt32(1);
                journalentry.IsLj = reader.GetBoolean(2);
                if (reader.GetValue(3) != null && reader.GetString(3) != "")
                    journalentry.LjId = reader.GetInt32(3);
                else
                    journalentry.LjId = 0;
                journalentry.Subject = reader.GetString(4);
                journalentry.Body = reader.GetString(5);
                journalentry.EntryDate = new DateTime(reader.GetInt32(6), reader.GetInt32(7), reader.GetInt32(8), reader.GetInt32(9), reader.GetInt32(10), 0);
                journalentry.Mood = reader.GetString(11);
                journalentry.Music = reader.GetString(12);
                journalentry.Tags = (new TagManager()).GetByEntryId(journalentry.Id);
            }
            return journalentry;
        }
        private List<JournalEntry> ReadJournalEntries(SqliteDataReader reader)
        {
            List<JournalEntry> entries = new List<JournalEntry>();
            JournalEntry journalentry;
            while (reader.Read())
            {
                journalentry = new JournalEntry();
                journalentry.Id = reader.GetInt32(0);
                journalentry.JournalId = reader.GetInt32(1);
                journalentry.IsLj = reader.GetBoolean(2);
                if (reader.GetValue(3) != null && reader.GetString(3) != "")
                    journalentry.LjId = reader.GetInt32(3);
                else
                    journalentry.LjId = 0;
                journalentry.Subject = reader.GetString(4);
                journalentry.Body = reader.GetString(5);
                journalentry.EntryDate = new DateTime(reader.GetInt32(6), reader.GetInt32(7), reader.GetInt32(8), reader.GetInt32(9), reader.GetInt32(10), 0);
                journalentry.Mood = reader.GetString(11);
                journalentry.Music = reader.GetString(12);
                journalentry.Tags = (new TagManager()).GetByEntryId(journalentry.Id);
                entries.Add(journalentry);
            }
            return entries;
        }
        #endregion Private Methods
    }
}
