using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildStuffFeedback.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        public int SessionId { get; set; }

        public int Rating { get; set; }

        public String Comments { get; set; }

        public String Email { get; set; }

        public String FullName { get; set; }
    }
}