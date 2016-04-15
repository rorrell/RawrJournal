using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace RawrJournal.Classes
{
    class FriendGroupManager
    {
        public List<FriendGroup> GetFriendsInGroup(List<Friend> friendlist, List<FriendGroup> grouplist)
        {
            foreach (Friend friend in friendlist)
            {
                int mask = friend.GroupMask;
                byte[] bytes = BitConverter.GetBytes(mask);
                BitArray ba = new BitArray(bytes);
                for (var i = 0; i < grouplist.Count; i++)
                {
                    if (ba.Get(i + 1))
                        grouplist[i].Friends.Add(friend);
                    else
                        grouplist[i].NotFriends.Add(friend);
                }
            }
            return grouplist;
        }
    }
}
