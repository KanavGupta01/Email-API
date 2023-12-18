using System.Collections.Generic;

namespace EmailSenderV5.Models
{
    public class GetTemplates
    {
        public List<Template> templates { get; set; }
    }

    public class Template
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}