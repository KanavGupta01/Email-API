using System.Collections.Generic;

namespace EmailSenderV5.Models
{
    public class ViewTemplate
    {
        public string id { get; set; }
        public string name { get; set; }
        public string generation { get; set; }
        public string updated_at { get; set; }
        public List<Version> versions { get; set; }
    }

    public class Version
    {
        public string html_content { get; set; }
    }
}