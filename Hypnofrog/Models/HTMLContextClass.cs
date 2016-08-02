using System.Web.Mvc;

namespace Hypnofrog.Models
{
    public class HTMLContextClass
    {
        [AllowHtml]
        public string Context { get; set; }

        public HTMLContextClass()
        {

        }
    }
}