using Hypnofrog.DBModels;

namespace Hypnofrog.ViewModels
{
    public class TemplateViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string HtmlTable { get; set; }

        public TemplateViewModel()
        {
        }

        public TemplateViewModel(OwnTemplate ownTemplate)
        {
            UserName = ownTemplate.UserName;
            HtmlTable = ownTemplate.HtmlRealize;
            Id = ownTemplate.OwnTemplateId;
        }
    }
}