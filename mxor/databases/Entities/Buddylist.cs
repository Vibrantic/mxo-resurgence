using System;
using System.Collections.Generic;

#nullable disable

namespace mxor.databases.Entities
{
    public partial class Buddylist
    {
        public int Id { get; set; }
        public int CharId { get; set; }
        public int FriendId { get; set; }
        public sbyte IsIgnored { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
