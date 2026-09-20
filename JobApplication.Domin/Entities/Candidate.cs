using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Domin.Entities
{
    public class Candidate
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; } 

        public string Name { get; set; }


        public string Email { get; set; } 

        public string CvUrl { get; set; }
    }
}
