using System;
using System.Collections.Generic;
using Mono.Data.SqliteClient;
using System.Data;

namespace RawrJournal.Classes
{
    public class Tag
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
        #endregion Properties

        #region Methods
        #endregion Methods
    }
}
