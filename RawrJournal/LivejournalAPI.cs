
using System;
using CookComputing.XmlRpc;

namespace RawrJournal.LiveJournalApi
{
    public struct RequestLogin
    {
        public string username;
        public string password;
        public int getmoods;
        public int getmenus;
        public int getpickws;
        public int getpickwurls;
    }

    public struct GeneralRequest
    {
        public string username;
        public string password;
    }

    public struct RequestFriends
    {
        public string username;
        public string password;
        public int includefriendof;
        public int includegroups;
        public int includebdays;
    }

    public struct RequestEvent
    {
        public string username;
        public string password;
        public string selecttype;

        //For a selecttype of syncitems
        public DateTime lastsync;

        //For a selecttype of day
        public int year; 
        public int month; 
        public int day;

        //For a selecttype of lastn
        public int howmany;
        public string beforedate;

        //For a selecttype of one
        public int itemid;
    }

    public struct SubmitEvent
    {
        public string username;
        public string password;
        public int itemid;
        public string @event;
        public string lineendings;
        public string subject;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string security;
        public int allowmask;
        public int year;
        public int mon;
        public int day;
        public int hour;
        public int min;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public PropData props;
    }

    public struct PropData
    {
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string current_mood;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string current_music;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public bool opt_preformatted;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public bool opt_nocomments;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string picture_keyword;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string taglist;
    }

    [XmlRpcUrl("http://www.livejournal.com/interface/xmlrpc HTTP/1.0")]
    public interface ILivejournal
    {
        [XmlRpcMethod("LJ.XMLRPC.login",
           Description = "validate user's password and get base information needed for client to function")]
        XmlRpcStruct login(RequestLogin request);

        [XmlRpcMethod("LJ.XMLRPC.getusertags",
           Description = "Retrieves a list of the user's defined tags.")]
        XmlRpcStruct getusertags(GeneralRequest request);

        [XmlRpcMethod("LJ.XMLRPC.getfriends",
           Description = "Returns a list of which other LiveJournal users this user lists as their friend.")]
        XmlRpcStruct getfriends(RequestFriends request);

        [XmlRpcMethod("LJ.XMLRPC.getevents",
           Description = "Download parts of the user's journal.")]
        XmlRpcStruct getevents(RequestEvent request);

        [XmlRpcMethod("LJ.XMLRPC.postevent",
           Description = "The most important mode, this is how a user actually submits a new log entry to the server.")]
        XmlRpcStruct postevent(SubmitEvent request);

        [XmlRpcMethod("LJ.XMLRPC.editevent",
           Description = "Edit or delete a user's past journal entry")]
        XmlRpcStruct editevent(SubmitEvent request);

        [XmlRpcMethod("LJ.XMLRPC.getdaycounts",
           Description = "This mode retrieves the number of journal entries per day. ")]
        XmlRpcStruct getdaycounts(GeneralRequest request);
    }
}


