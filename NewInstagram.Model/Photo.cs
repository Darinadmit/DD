using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewInstagram.Model
{
    public class Photo
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public DateTime Date { get; set; }

        public byte[] Image { get; set; }

        public string[] Hashtags { get; set; }

        public User[] Marks { get; set; }
    }
}
