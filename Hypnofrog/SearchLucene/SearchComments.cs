using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Hypnofrog.DBModels;
using Hypnofrog.ViewModels;

namespace Hypnofrog.SearchLucene
{
    public class SearchComments
    {
        private RAMDirectory _directory;

        public SearchComments()
        {
            _directory = new RAMDirectory();
        }

        private void _addToLuceneIndex(Comment sampleData, IndexWriter writer)
        {
            // remove older index entry
            var searchQuery = new TermQuery(new Term("Id", sampleData.SiteId.ToString()));
            writer.DeleteDocuments(searchQuery);

            // add new index entry
            var doc = new Document();

            // add lucene fields mapped to db fields
            doc.Add(new Field("SiteId", sampleData.SiteId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Avatar", sampleData.UserAvatar, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Userid", sampleData.UserId, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("CreationTime", sampleData.CreationTime.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Text", sampleData.Text, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("CommentId", sampleData.CommentId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));


            // add entry to index
            writer.AddDocument(doc);
        }

        public void AddUpdateLuceneIndex(IEnumerable<Comment> sampleDatas)
        {
            // init lucene
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                // add data to lucene search index (replaces older entry if any)
                foreach (var sampleData in sampleDatas) _addToLuceneIndex(sampleData, writer);

                // close handles
                analyzer.Close();
                writer.Dispose();
            }
        }

        public void ClearLuceneIndexRecord(int record_id)
        {
            // init lucene
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                // remove older index entry
                var searchQuery = new TermQuery(new Term("Id", record_id.ToString()));
                writer.DeleteDocuments(searchQuery);

                // close handles
                analyzer.Close();
                writer.Dispose();
            }
        }

        public bool ClearLuceneIndex()
        {
            try
            {
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                using (var writer = new IndexWriter(_directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    // remove older index entries
                    writer.DeleteAll();

                    // close handles
                    analyzer.Close();
                    writer.Dispose();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public void Optimize()
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                analyzer.Close();
                writer.Optimize();
                writer.Dispose();
            }
        }

        private Comment _mapLuceneDocumentToData(Document doc)
        {
            return new Comment
            {
                SiteId = Convert.ToInt32(doc.Get("SiteId")),
                UserAvatar = doc.Get("Avatar"),
                UserId = doc.Get("Userid"),
                Text = doc.Get("Text"),
                CreationTime = Convert.ToDateTime(doc.Get("CreationTime")),
                CommentId = Convert.ToInt32(doc.Get("CommentId"))
            };
        }

        private IEnumerable<Comment> _mapLuceneToDataList(IEnumerable<Document> hits)
        {
            return hits.Select(_mapLuceneDocumentToData).ToList();
        }

        private IEnumerable<Comment> _mapLuceneToDataList(IEnumerable<ScoreDoc> hits,
            IndexSearcher searcher)
        {
            return hits.Select(hit => _mapLuceneDocumentToData(searcher.Doc(hit.Doc))).ToList();
        }

        private static Query parseQuery(string searchQuery, QueryParser parser)
        {
            Query query;
            try
            {
                query = parser.Parse(searchQuery.Trim());
            }
            catch (ParseException)
            {
                query = parser.Parse(QueryParser.Escape(searchQuery.Trim()));
            }
            return query;
        }

        private IEnumerable<CommentViewModel> _search(string searchQuery, string searchField = "")
        {
            // validation
            if (string.IsNullOrEmpty(searchQuery.Replace("*", "").Replace("?", ""))) return new List<CommentViewModel>();

            // set up lucene searcher
            using (var searcher = new IndexSearcher(_directory, false))
            {
                var hits_limit = 1000;
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

                // search by single field
                if (!string.IsNullOrEmpty(searchField))
                {
                    var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, searchField, analyzer);
                    var query = parseQuery(searchQuery, parser);
                    var hits = searcher.Search(query, hits_limit).ScoreDocs;
                    var results = _mapLuceneToDataList(hits, searcher);
                    analyzer.Close();
                    searcher.Dispose();
                    List<CommentViewModel> comments = new List<CommentViewModel>();
                    foreach(var elem in results)
                    {
                        comments.Add(new CommentViewModel(elem));
                    }
                    return comments;
                }
                // search by multiple fields (ordered by RELEVANCE)
                else
                {
                    var parser = new MultiFieldQueryParser
                        (Lucene.Net.Util.Version.LUCENE_30, new[] { "SiteId", "Avatar", "Userid", "Text", "CommentId" }, analyzer);
                    var query = parseQuery(searchQuery, parser);
                    var hits = searcher.Search(query, null, hits_limit, Sort.RELEVANCE).ScoreDocs;
                    var results = _mapLuceneToDataList(hits, searcher);
                    analyzer.Close();
                    searcher.Dispose();
                    List<CommentViewModel> comments = new List<CommentViewModel>();
                    foreach (var elem in results)
                    {
                        comments.Add(new CommentViewModel(elem));
                    }
                    return comments;
                }
            }
        }

        public IEnumerable<CommentViewModel> Search(string input, string fieldName = "")
        {
            if (string.IsNullOrEmpty(input)) return new List<CommentViewModel>();

            var terms = input.Trim().Replace("-", " ").Split(' ')
                .Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim() + "*");
            input = string.Join(" ", terms);

            return _search(input, fieldName);
        }

        public IEnumerable<CommentViewModel> Search(string input)
        {
            if (string.IsNullOrEmpty(input)) return new List<CommentViewModel>();

            return _search(input);
        }
    }

}