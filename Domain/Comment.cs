using System;

namespace Domain
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public virtual AppUser Auther { get; set; }
        public virtual Activity Activity{get;set;}
        public DateTime CreateAt{ get; set;}

    }

}