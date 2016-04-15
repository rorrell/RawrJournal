using System;
using System.Collections.Generic;
using Mono.Data.SqliteClient;
using System.Data;

namespace RawrJournal.Classes
{
    public class TagManager
    {
        #region Properties
        private static RawrUtility util = new RawrUtility();
        private SqliteConnection cnn = util.SqliteConnect();
        #endregion Properties

        #region Public Methods
        public string CreateString(List<Tag> tags)
        {
            string tagstring = "";
            if (tags.Count > 0)
            {
                foreach (Tag tag in tags)
                    tagstring += tag.Name + ", ";
                tagstring = tagstring.Substring(0, tagstring.Length - 2);
            }
            return tagstring;
        }
        public List<Tag> ProcessTagString(string tagstring)
        {
            List<Tag> taglist = new List<Tag>();
            string[] tagarray = tagstring.Split(',');
            Tag tag = null;
            string mystring = "";
            foreach (string t in tagarray)
            {
                mystring = t.Trim();
                tag = GetByName(mystring);
                if (tag.Id == 0 && tag.Name == "")
                {
                    tag.Name = mystring;
                    Insert(tag);
                    tag = GetByName(mystring);
                }
                taglist.Add(tag);
            }
            return taglist;
        }
        public List<Tag> GetAll()
        {
            cnn.Open();
            SqliteCommand cmd = new SqliteCommand("SELECT * FROM Tag ORDER BY Name", cnn);
            SqliteDataReader reader = cmd.ExecuteReader();
            List<Tag> tags = ReadTags(reader);
            cnn.Close();
            return tags;
        }
        public Tag GetById(int id)
        {
            cnn.Open();
            SqliteCommand cmd = new SqliteCommand("SELECT * FROM Tag WHERE Id=" + id.ToString(), cnn);
            SqliteDataReader reader = cmd.ExecuteReader();
            Tag tag = ReadTag(reader);
            cnn.Close();
            return tag;
        }
        public Tag GetByName(string name)
        {
            cnn.Open();
            SqliteCommand cmd = new SqliteCommand("SELECT * FROM Tag WHERE Name='" + name + "'", cnn);
            SqliteDataReader reader = cmd.ExecuteReader();
            Tag tag = ReadTag(reader);
            cnn.Close();
            return tag;
        }
        public List<Tag> GetByEntryId(int entryId)
        {
            cnn.Open();
            SqliteCommand cmd = new SqliteCommand("SELECT t.Id, t.Name FROM Tag t, EntryTag et WHERE et.EntryId = " + entryId + " AND t.Id = et.TagId", cnn);
            SqliteDataReader reader = cmd.ExecuteReader();
            List<Tag> tags = ReadTags(reader);
            cnn.Close();
            return tags;
        }
        public void Insert(Tag tag)
        {
            if (tag.Name.Trim() == "")
                return;
            cnn.Open();
            SqliteCommand cmd = new SqliteCommand("INSERT INTO Tag (Name) VALUES (:name)", cnn);
            SqliteParameter param = new SqliteParameter(":name", DbType.String);
            param.Value = tag.Name;
            cmd.Parameters.Add(param);
            try { cmd.ExecuteNonQuery(); }
            catch { }
            cnn.Close();
        }
        public void Delete(int id)
        {
            cnn.Open();
            SqliteCommand cmd = new SqliteCommand("DELETE FROM Tag WHERE Id=" + id.ToString(), cnn);
            try { cmd.ExecuteNonQuery(); }
            catch { }
            cnn.Close();
        }
        public void DeleteAll()
        {
            cnn.Open();
            SqliteCommand cmd = new SqliteCommand("DELETE FROM Tag", cnn);
            try { cmd.ExecuteNonQuery(); }
            catch { }
            cnn.Close();
        }
        public void Update(Tag tag)
        {
            if (tag.Name.Trim() == "")
                return;
            cnn.Open();
            SqliteCommand cmd = new SqliteCommand("UPDATE Tag SET Name=:name WHERE Id=" + tag.Id.ToString(), cnn);
            SqliteParameter param = new SqliteParameter(":name", DbType.String);
            param.Value = tag.Name;
            cmd.Parameters.Add(param);
            try { cmd.ExecuteNonQuery(); }
            catch { }
            cnn.Close();
        }
        #endregion Public Methods

        #region Private Methods
        private Tag ReadTag(SqliteDataReader reader)
        {
            Tag tag = new Tag();
            while (reader.Read())
            {
                tag.Id = reader.GetInt32(0);
                tag.Name = reader.GetString(1);
            }
            return tag;
        }
        private List<Tag> ReadTags(SqliteDataReader reader)
        {
            List<Tag> tags = new List<Tag>();
            Tag tag;
            while (reader.Read())
            {
                tag = new Tag();
                tag.Id = reader.GetInt32(0);
                tag.Name = reader.GetString(1);
                tags.Add(tag);
            }
            return tags;
        }
        #endregion Private Methods
    }
}
