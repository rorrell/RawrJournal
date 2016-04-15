using System;
using System.Collections.Generic;
using Mono.Data.SqliteClient;
using System.Data;

namespace RawrJournal.Classes
{
    public class Friend
    {
        #region Properties
        private string _type = "";
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }
        private string _fullname = "";
        public string Fullname
        {
            get { return _fullname; }
            set { _fullname = value; }
        }
        private string _username = "";
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }
        private string _fgcolor = "";
        public string FgColor
        {
            get { return _fgcolor; }
            set { _fgcolor = value; }
        }
        private string _bgcolor = "";
        public string BgColor
        {
            get { return _bgcolor; }
            set { _bgcolor = value; }
        }
        private int _birthday = 0;
        public int Birthday
        {
            get { return _birthday; }
            set { _birthday = value; }
        }
        private int _birthyear = 0;
        public int Birthyear
        {
            get { return _birthyear; }
            set { _birthyear = value; }
        }
        private int _birthmonth = 0;
        public int Birthmonth
        {
            get { return _birthmonth; }
            set { _birthmonth = value; }
        }
        private bool _ismutual = false;
        public bool IsMutual
        {
            get { return _ismutual; }
            set { _ismutual = value; }
        }
        private int _groupmask;
        public int GroupMask
        {
            get { return _groupmask; }
            set { _groupmask = value; }
        }
        #endregion Properties

        #region Methods
        #endregion Methods
    }
}
