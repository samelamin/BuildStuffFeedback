using System;
using System.Collections.Generic;

namespace BuildStuffFeedback.Models.Admin
{
    public class SessionSummary
    {
        public int Id { get; set; }


        public String Title { get; set; }

        public String Speaker { get; set; }
    }

    public class SessionDetail
    {
        public int Id { get; set; }

        public String Title { get; set; }

        public String Speaker { get; set; }

        public string Email { get; set; }
        public int Greens { get; set; }
        public int Yellows { get; set; }
        public int Reds { get; set; }
        public IEnumerable<string> Comments { get; set; }
    }

    public enum Level
    {
        Red = 1,
        Yellow =2,
        Green=3
    }
}