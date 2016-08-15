namespace Hypnofrog.DBModels
{
    public class Content
    {
        public int ContentId { get; set; }
        public string HtmlContent { get; set; }

        public int? PageId { get; set; }
        public virtual Page Page { get; set; }
    }
}