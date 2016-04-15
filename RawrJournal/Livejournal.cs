using System;
using System.Collections.Generic;
using System.Text;
using RawrJournal.LiveJournalApi;
using RawrJournal.Classes;
using CookComputing.XmlRpc;

namespace RawrJournal
{
    class Livejournal
    {
        private ILivejournal proxy;

        public Livejournal()
        {
            proxy = XmlRpcProxyGen.Create<ILivejournal>();
            XmlRpcClientProtocol xpc = (XmlRpcClientProtocol)proxy;
            xpc.Url = "http://www.livejournal.com/interface/xmlrpc HTTP/1.0";
        }

        public object[] getuserpics(string username, string password)
        {
            RequestLogin data = new RequestLogin();
            data.username = username;
            data.password = password;
            data.getpickws = 1;
            data.getpickwurls = 1;
            try
            {
                XmlRpcStruct picstruct = proxy.login(data);
                object[] obj = new object[] { (string[])picstruct["pickws"], (string[])picstruct["pickwurls"] };
                return obj;
            }
            catch { }
            return null;
        }

        public List<Tag> getusertags(string username, string password)
        {
            List<Tag> ljtaglist = new List<Tag>();
            GeneralRequest data = new GeneralRequest();
            data.username = username;
            data.password = password;
            try
            {
                XmlRpcStruct tagstruct = proxy.getusertags(data);
                object[] ljtags = (object[])tagstruct["tags"];
                Tag thistag;
                foreach (object obj in ljtags)
                {
                    XmlRpcStruct taginfo = (XmlRpcStruct)obj;
                    thistag = new Tag();
                    thistag.Name = taginfo["name"].ToString();
                    ljtaglist.Add(thistag);
                }
            }
            catch { }
            return ljtaglist;
        }

        public object[] getfriends(string username, string password,
            bool includefriendof, bool includegroups, bool includebdays)
        {
            List<Friend> friendlist = new List<Friend>();
            List<FriendGroup> grouplist = new List<FriendGroup>();
            RequestFriends data = new RequestFriends();
            data.username = username;
            data.password = password;
            if (includefriendof)
                data.includefriendof = 1;
            if (includegroups)
                data.includegroups = 1;
            if (includebdays)
                data.includebdays = 1;
            try
            {
                XmlRpcStruct friendstruct = proxy.getfriends(data);
                object[] friends = (object[])friendstruct["friends"];
                object[] friendofs = null;
                string friendofstring = "";
                if (includefriendof)
                {
                    friendofs = (object[])friendstruct["friendofs"];
                    foreach (object fo in friendofs)
                    {
                        XmlRpcStruct friendofstruct = (XmlRpcStruct)fo;
                        friendofstring += friendofstruct["username"].ToString() + ",";
                    }
                    friendofstring = friendofstring.Substring(0, friendofstring.Length - 1);
                }
                string[] friendofstrings = friendofstring.Split(',');
                object[] friendgroups = null;
                if (includegroups)
                    friendgroups = (object[])friendstruct["friendgroups"];

                foreach (object f in friends)
                {
                    XmlRpcStruct subfriendstruct = (XmlRpcStruct)f;
                    Friend friend = new Friend();
                    friend.Fullname = subfriendstruct["fullname"].ToString();
                    friend.Username = subfriendstruct["username"].ToString();
                    if (subfriendstruct["groupmask"] != null)
                        friend.GroupMask = Convert.ToInt32(subfriendstruct["groupmask"].ToString());
                    if (subfriendstruct["type"] != null)
                        friend.Type = subfriendstruct["type"].ToString();
                    if (includebdays && subfriendstruct["birthday"] != null)
                    {
                        string timestamp = subfriendstruct["birthday"].ToString();
                        friend.Birthyear = Convert.ToInt32(timestamp.Substring(0, 4));
                        friend.Birthmonth = Convert.ToInt32(timestamp.Substring(5, 2));
                        friend.Birthday = Convert.ToInt32(timestamp.Substring(8, 2));
                    }
                    friend.BgColor = subfriendstruct["bgcolor"].ToString();
                    friend.FgColor = subfriendstruct["fgcolor"].ToString();
                    if (includefriendof)
                    {
                        foreach (string fos in friendofstrings)
                        {
                            if (fos == subfriendstruct["username"].ToString())
                                friend.IsMutual = true;
                        }
                    }
                    friendlist.Add(friend);
                }

                if (includegroups)
                {
                    foreach (object g in friendgroups)
                    {
                        XmlRpcStruct groupstruct = (XmlRpcStruct)g;
                        FriendGroup friendgroup = new FriendGroup();
                        friendgroup.Id = Convert.ToInt32(groupstruct["id"].ToString());
                        if (groupstruct["public"].ToString() == "0")
                            friendgroup.IsPublic = false;
                        else if (groupstruct["public"].ToString() == "1")
                            friendgroup.IsPublic = true;
                        friendgroup.Name = groupstruct["name"].ToString();
                        friendgroup.SortOrder = Convert.ToInt32(groupstruct["sortorder"].ToString());
                        grouplist.Add(friendgroup);
                    }
                }
            }
            catch { }
            object o1 = (object)friendlist;
            object o2 = (object)grouplist;
            object[] oarray = new object[] { o1, o2 };
            return oarray;
        }

        private List<JournalEntry> GetEventInfo(object[] events)
        {
            List<JournalEntry> ljentrylist = new List<JournalEntry>();
            JournalEntry thisentry;
            foreach (object obj in events)
            {
                XmlRpcStruct eventinfo = (XmlRpcStruct)obj;
                thisentry = new JournalEntry();
                thisentry.IsLj = true;
                thisentry.LjId = Convert.ToInt32(eventinfo["itemid"].ToString());
                thisentry.EntryDate = (new RawrUtility()).ConvertDateString(eventinfo["eventtime"].ToString());
                thisentry.Subject = eventinfo["subject"].ToString();
                thisentry.Body = eventinfo["event"].ToString();
                XmlRpcStruct propinfo = (XmlRpcStruct)eventinfo["props"];
                if (propinfo["current_mood"] != null)
                    thisentry.Mood = propinfo["current_mood"].ToString();
                if (propinfo["current_music"] != null)
                    thisentry.Music = propinfo["current_music"].ToString();
                if (propinfo["taglist"] != null)
                    thisentry.Tags = (new TagManager()).ProcessTagString(propinfo["taglist"].ToString());
                ljentrylist.Add(thisentry);
            }
            return ljentrylist;
        }

        public List<JournalEntry> syncevents(string username, string password, DateTime lastsync)
        {
            List<JournalEntry> ljentrylist = new List<JournalEntry>();
            RequestEvent data = new RequestEvent();
            data.username = username;
            data.password = password;
            data.selecttype = "syncitems";
            data.lastsync = lastsync;
            try
            {
                XmlRpcStruct entrystruct = proxy.getevents(data);
                object[] ljevents = (object[])entrystruct["events"];
                ljentrylist = GetEventInfo(ljevents);
            }
            catch { }
            return ljentrylist;
        }

        public List<JournalEntry> geteventsbyday(string username, string password, DateTime day)
        {
            List<JournalEntry> ljentrylist = new List<JournalEntry>();
            RequestEvent data = new RequestEvent();
            data.username = username;
            data.password = password;
            data.selecttype = "day";
            data.year = day.Year;
            data.month = day.Month;
            data.day = day.Day;
            try
            {
                XmlRpcStruct entrystruct = proxy.getevents(data);
                object[] ljevents = (object[])entrystruct["events"];
                ljentrylist = GetEventInfo(ljevents);
            }
            catch { }
            return ljentrylist;
        }

        public List<JournalEntry> getlastnentries(string username, string password, int n, DateTime beforedate)
        {
            List<JournalEntry> ljentrylist = new List<JournalEntry>();
            RequestEvent data = new RequestEvent();
            data.username = username;
            data.password = password;
            data.selecttype = "lastn";
            data.howmany = n;
            data.beforedate = (new RawrUtility()).CreateDateString(beforedate);
            try
            {
            XmlRpcStruct entrystruct = proxy.getevents(data);
            object[] ljevents = (object[])entrystruct["events"];
            ljentrylist = GetEventInfo(ljevents);
            }
            catch { }
            return ljentrylist;
        }

        public JournalEntry getoneevent(string username, string password, int itemid)
        {
            List<JournalEntry> ljentrylist = new List<JournalEntry>();
            RequestEvent data = new RequestEvent();
            data.username = username;
            data.password = password;
            data.selecttype = "one";
            data.itemid = itemid;
            try
            {
                XmlRpcStruct entrystruct = proxy.getevents(data);
                object[] ljevents = (object[])entrystruct["events"];
                ljentrylist = GetEventInfo(ljevents);
            }
            catch { }
            return ljentrylist[0];
        }

        public int postevent(string username, string password, JournalEntry entry, string security,
            int allowmask, string picture, bool allowcomments)
        {
            int itemid = 0;
            SubmitEvent data = new SubmitEvent();
            data.username = username;
            data.password = password;
            data.@event = entry.Body;
            data.lineendings = "pc";
            data.year = entry.EntryDate.Year;
            data.mon = entry.EntryDate.Month;
            data.day = entry.EntryDate.Day;
            data.hour = entry.EntryDate.Hour;
            data.min = entry.EntryDate.Minute;
            data.subject = entry.Subject;
            data.security = security;
            data.allowmask = allowmask;
            data.props.current_mood = entry.Mood;
            data.props.current_music = entry.Music;
            data.props.opt_nocomments = !allowcomments;
            data.props.opt_preformatted = true;
            data.props.picture_keyword = picture;
            data.props.taglist = (new TagManager()).CreateString(entry.Tags);
            try
            {
                XmlRpcStruct entrystruct = proxy.postevent(data);
                itemid = Convert.ToInt32(entrystruct["itemid"].ToString());
            }
            catch { }
            return itemid;
        }

        public bool editevent(string username, string password, JournalEntry entry, string security,
            int allowmask, string picture, bool allowcomments)
        {
            bool success = true;
            SubmitEvent data = new SubmitEvent();
            data.username = username;
            data.password = password;
            data.itemid = entry.LjId;
            data.@event = entry.Body;
            data.lineendings = "pc";
            data.year = entry.EntryDate.Year;
            data.mon = entry.EntryDate.Month;
            data.day = entry.EntryDate.Day;
            data.hour = entry.EntryDate.Hour;
            data.min = entry.EntryDate.Minute;
            data.subject = entry.Subject;
            data.security = security;
            data.allowmask = allowmask;
            data.props.current_mood = entry.Mood;
            data.props.current_music = entry.Music;
            data.props.opt_nocomments = !allowcomments;
            data.props.opt_preformatted = true;
            data.props.picture_keyword = picture;
            data.props.taglist = (new TagManager()).CreateString(entry.Tags);
            try
            {
                XmlRpcStruct entrystruct = proxy.postevent(data);
            }
            catch { success = false; }
            return success;
        }

        public bool deleteevent(string username, string password, int itemid)
        {
            bool success = true;
            SubmitEvent data = new SubmitEvent();
            JournalEntry entry = new JournalEntry();
            data.username = username;
            data.password = password;
            data.itemid = itemid;
            data.@event = entry.Body;
            data.lineendings = "pc";
            data.year = entry.EntryDate.Year;
            data.mon = entry.EntryDate.Month;
            data.day = entry.EntryDate.Day;
            data.hour = entry.EntryDate.Hour;
            data.min = entry.EntryDate.Minute;
            data.subject = "";
            try
            {
                XmlRpcStruct entrystruct = proxy.editevent(data);
            }
            catch { success = false; }
            return success;
        }

        public object getdaycounts(string username, string password)
        {
            GeneralRequest data = new GeneralRequest();
            data.username = username;
            data.password = password;
            try
            {
                XmlRpcStruct countstruct = proxy.getdaycounts(data);
                object[] counts = (object[])countstruct["daycounts"];
            }
            catch { }
            return new object { };
        }
    }
}
