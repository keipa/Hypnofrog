using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Hypnofrog.Models;
using Hypnofrog.ViewModels;

namespace Hypnofrog.SearchLucene
{
    public class SearchUsers
    {
        private RAMDirectory _directory;

        public SearchUsers()
        {
            _directory = new RAMDirectory();
        }

        private void _addToLuceneIndex(ApplicationUser sampleData, IndexWriter writer)
        {
            var searchQuery = new TermQuery(new Term("Id", sampleData.Id.ToString()));
            writer.DeleteDocuments(searchQuery);
            var doc = new Document();
            doc.Add(new Field("UserId", sampleData.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("UserName", sampleData.UserName, Field.Store.YES, Field.Index.ANALYZED));
            writer.AddDocument(doc);
        }

        public void AddUpdateLuceneIndex(IEnumerable<ApplicationUser> sampleDatas)
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                foreach (var sampleData in sampleDatas) _addToLuceneIndex(sampleData, writer);
                analyzer.Close();
                writer.Dispose();
            }
        }

        public void ClearLuceneIndexRecord(int record_id)
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                var searchQuery = new TermQuery(new Term("Id", record_id.ToString()));
                writer.DeleteDocuments(searchQuery);
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
                    writer.DeleteAll();
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

        private ApplicationUser _mapLuceneDocumentToData(Document doc)
        {
            return new ApplicationUser
            {
                Id = doc.Get("Id"),
                UserName = doc.Get("UserName")

            };
        }

        private IEnumerable<ApplicationUser> _mapLuceneToDataList(IEnumerable<Document> hits)
        {
            return hits.Select(_mapLuceneDocumentToData).ToList();
        }

        private IEnumerable<ApplicationUser> _mapLuceneToDataList(IEnumerable<ScoreDoc> hits,
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

        private IEnumerable<UserView> _search(string searchQuery, string searchField = "")
        {
            // validation
            if (string.IsNullOrEmpty(searchQuery.Replace("*", "").Replace("?", ""))) return new List<UserView>();

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
                    return UserView.GetUserViews(results);
                }
                else
                {
                    var parser = new MultiFieldQueryParser
                        (Lucene.Net.Util.Version.LUCENE_30, new[] { "Id", "UserName" }, analyzer);
                    var query = parseQuery(searchQuery, parser);
                    var hits = searcher.Search(query, null, hits_limit, Sort.RELEVANCE).ScoreDocs;
                    var results = _mapLuceneToDataList(hits, searcher);
                    analyzer.Close();
                    searcher.Dispose();
                    return UserView.GetUserViews(results);
                }
            }
        }

        public IEnumerable<UserView> Search(string input, string fieldName = "")
        {
            if (string.IsNullOrEmpty(input)) return new List<UserView>();

            var terms = input.Trim().Replace("-", " ").Split(' ')
                .Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim() + "*");
            input = string.Join(" ", terms);

            return _search(input, fieldName);
        }

        public IEnumerable<UserView> Search(string input)
        {
            if (string.IsNullOrEmpty(input)) return new List<UserView>();

            return _search(input);
        }
    }

}