using System;
using System.Collections.Generic;
using Mono.Data.SqliteClient;
using System.Data;

namespace RawrJournal.Classes
{
    public class JournalManager
    {
        #region Properties
        private static RawrUtility util = new RawrUtility();
        private SqliteConnection cnn = util.SqliteConnect();
        private JournalEntryManager entryManager = new JournalEntryManager();
        #endregion Properties

        #region Public Methods
        public List<Journal> GetAll()
        {
            cnn.Open();
            SqliteCommand cmd = new SqliteCommand("SELECT * FROM Journal ORDER BY Name", cnn);
            SqliteDataReader reader = cmd.ExecuteReader();
            List<Journal> journals = ReadJournals(reader);
            cnn.Close();
            return journals;
        }
        public Journal GetById(int id)
        {
            cnn.Open();
            SqliteCommand cmd = new SqliteCommand("SELECT * FROM Journal WHERE Id=" + id.ToString(), cnn);
            SqliteDataReader reader = cmd.ExecuteReader();
            Journal journal = ReadJournal(reader);
            cnn.Close();
            return journal;
        }
        public Journal GetByName(string name)
        {
            cnn.Open();
            SqliteCommand cmd = new SqliteCommand("SELECT * FROM Journal WHERE Name='" + name + "'", cnn);
            SqliteDataReader reader = cmd.ExecuteReader();
            Journal journal = ReadJournal(reader);
            cnn.Close();
            return journal;
        }
        public void Insert(Journal journal)
        {
            if (journal.Name.Trim() == "")
                return;
            cnn.Open();
            SqliteCommand cmd = new SqliteCommand("INSERT INTO Journal (Name, Username, Password, IsLocked, Key) VALUES (:name, '" + journal.Username + "', '" + journal.Password + "', '" + journal.IsLocked + "', '" + journal.Key + "')", cnn);
            SqliteParameter param = new SqliteParameter(":name", DbType.String);
            param.Value = journal.Name;
            cmd.Parameters.Add(param);
            try { cmd.ExecuteNonQuery(); }
            catch { }
            cnn.Close();
        }
        public void Delete(int id)
        {
            cnn.Open();
            List<JournalEntry> entries = (new JournalEntryManager()).GetByJournalId(id);
            if (entries.Count > 0)
            {
                foreach (JournalEntry entry in entries)
                    (new JournalEntryManager()).Delete(entry.Id);
            }
            SqliteCommand cmd = new SqliteCommand("DELETE FROM Journal WHERE Id=" + id, cnn);
            try { cmd.ExecuteNonQuery(); }
            catch { }
            cnn.Close();
        }
        public void Update(Journal journal)
        {
            if (journal.Name.Trim() == "")
                return;
            cnn.Open();
            SqliteCommand cmd = new SqliteCommand("UPDATE Journal SET Name=:name, Username='" + journal.Username + "', Password='" + journal.Password + "', IsLocked='" + journal.IsLocked + "', Key='" + journal.Key + "' WHERE Id=" + journal.Id.ToString(), cnn);
            SqliteParameter param = new SqliteParameter(":name", DbType.String);
            param.Value = journal.Name;
            cmd.Parameters.Add(param);
            try { cmd.ExecuteNonQuery(); }
            catch { }
            cnn.Close();
        }
        #endregion Public Methods

        #region Private Methods
        private Journal ReadJournal(SqliteDataReader reader)
        {
            Journal journal = new Journal();
            while (reader.Read())
            {
                journal.Id = reader.GetInt32(0);
                journal.Name = reader.GetString(1);
                journal.Username = reader.GetString(2);
                journal.Password = reader.GetString(3);
                journal.IsLocked = reader.GetBoolean(4);
                journal.Key = reader.GetString(5);
                journal.Entries = entryManager.GetByJournalId(journal.Id);
            }
            return journal;
        }
        private List<Journal> ReadJournals(SqliteDataReader reader)
        {
            List<Journal> journals = new List<Journal>();
            Journal journal;
            while (reader.Read())
            {
                journal = new Journal();
                journal.Id = reader.GetInt32(0);
                journal.Name = reader.GetString(1);
                journal.Username = reader.GetString(2);
                journal.Password = reader.GetString(3);
                journal.IsLocked = reader.GetBoolean(4);
                journal.Key = reader.GetString(5);
                journal.Entries = entryManager.GetByJournalId(journal.Id);
                journals.Add(journal);
            }
            return journals;
        }
        #endregion Private Methods
    }
}
