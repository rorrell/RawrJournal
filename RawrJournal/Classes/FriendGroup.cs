using System;
using System.Collections.Generic;
using Mono.Data.SqliteClient;
using System.Data;

namespace RawrJournal.Classes
{
    public class FriendGroup
    {
        #region Properties
        private int _id = 0;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _name = "";
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private bool _ispublic = false;
        public bool IsPublic
        {
            get { return _ispublic; }
            set { _ispublic = value; }
        }
        private int _sortorder = 0;
        public int SortOrder
        {
            get { return _sortorder; }
            set { _sortorder = value; }
        }
        private List<Friend> _friends = new List<Friend>();
        public List<Friend> Friends
        {
            get { return _friends; }
            set { _friends = value; }
        }
        private List<Friend> _notfriends = new List<Friend>();
        public List<Friend> NotFriends
        {
            get { return _notfriends; }
            set { _notfriends = value; }
        }
        #endregion Properties

        #region Methods
        #endregion Methods
    }
}
