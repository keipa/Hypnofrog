using System;
using System.Collections.Generic;
using System.Linq;
using Hypnofrog.DBModels;
using Hypnofrog.Filters;


namespace Hypnofrog.ViewModels
{
    [Culture]
    public class AchievmentChecker
    {
        public string Result { get; private set; }

        public List<Achievement> NewAchievments { get; }

        public AchievmentChecker(List<Site> sites, List<Rate> rates, List<Achievement> log, string userid)
        {
            var achievmentmask = RunChekers(sites, rates);
            var allachievments = GetAllAchievments();
            NewAchievments = new List<Achievement>();
            Result = CheckCoincidences(achievmentmask, allachievments, GetUserAchievments(log), userid);
        }


        private string CheckCoincidences(IReadOnlyList<bool> achievmentmask, IReadOnlyList<string> allachievments, ICollection<string> log, string userid)
        {
            for (var i = 0; i< achievmentmask.Count-1; i++)
            {
                if (!achievmentmask[i]) continue;
                if (log.Contains(allachievments[i])) continue;
                var achievment = new Achievement() {Name = allachievments[i], Time=DateTime.Now, User = userid};
                NewAchievments.Add(achievment);
                return allachievments[i];
            }
            return "";

        }
        
        private readonly string _descRuHasThreeSites = Resources.Resource.descRUHasThreeSites;
        private readonly string _descRuHasFiveRatingWithOnes = Resources.Resource.descRUHasFiveRatingWithOnes;
        private readonly string _descRuHasOneSite = Resources.Resource.descRUHasOneSite;
        private readonly string _descRuHasTenTags = Resources.Resource.descRUHasTenTags;
        private readonly string _descRuHasThreeSitesWithRatesGreaterThanThree = Resources.Resource.descRUHasThreeSitesWithRatesGreaterThanThree;
        private readonly string _descRuHasOneRating = Resources.Resource.descRUHasOneRating;
        private readonly string _descRuHasTenSites = Resources.Resource.descRUHasTenSites;
        private readonly string _descRuHas100Sites = Resources.Resource.descRUHas100Sites;
        private readonly string _descRuHasTenRatingsWith5Stars = Resources.Resource.descRUHasTenRatingsWith5Stars;

        private readonly string _nameHasThreeSites = Resources.Resource.nameHasThreeSites;
        private readonly string _nameHasFiveRatingWithOnes = Resources.Resource.nameHasFiveRatingWithOnes;
        private readonly string _nameHasOneSite = Resources.Resource.nameHasOneSite;
        private readonly string _nameHasTenTags = Resources.Resource.nameHasTenTags;
        private readonly string _nameHasThreeSitesWithRatesGreaterThanThree = Resources.Resource.nameHasThreeSitesWithRatesGreaterThanThree;
        private readonly string _nameHasOneRating = Resources.Resource.nameHasOneRating;
        private readonly string _nameHasTenSites = Resources.Resource.nameHasTenSites;
        private readonly string _nameHas100Sites = Resources.Resource.nameHas100Sites;
        private readonly string _nameHasTenRatingsWith5Stars = Resources.Resource.nameHasTenRatingsWith5Stars;

        public List<string> GetAllAchievmentsDescriptionsRu()
        {
            var allachievments = new List<string>
            {
                _descRuHasThreeSites,
                _descRuHasFiveRatingWithOnes,
                _descRuHasOneSite,
                _descRuHasTenTags,
                _descRuHasThreeSitesWithRatesGreaterThanThree,
                _descRuHasOneRating,
                _descRuHasTenSites,
                _descRuHas100Sites,
                _descRuHasTenRatingsWith5Stars
            };
            return allachievments;
        }

        public List<string> GetAllAchievments()
        {
            var allachievments = new List<string>
            {
                _nameHasThreeSites,
                _nameHasFiveRatingWithOnes,
                _nameHasOneSite,
                _nameHasTenTags,
                _nameHasThreeSitesWithRatesGreaterThanThree,
                _nameHasOneRating,
                _nameHasTenSites,
                _nameHas100Sites,
                _nameHasTenRatingsWith5Stars
            };
            return allachievments;
        }

        private static List<bool> RunChekers(List<Site> sites, List<Rate> rates)
        {
            var checker = new List<bool>
            {
                HasThreeSites(sites),
                HasFiveRatingWithOnes(rates),
                HasOneSite(sites),
                HasTenTags(sites),
                HasThreeSitesWithRatesGreaterThanThree(sites),
                HasOneRating(rates),
                HasTenSites(sites),
                Has100Sites(sites),
                HasTenRatingsWith5Stars(rates)
            };
            return checker;
        }

        private static List<string> GetUserAchievments(IEnumerable<Achievement> log)
        {
            return log.Select(item => item.Name).ToList();
        }

        private static bool HasThreeSites(IEnumerable<Site> sites)
        {
            return sites.Count() >= 3;
        }

        private static bool HasFiveRatingWithOnes(IEnumerable<Rate> rates)
        {
            return rates.Count(x => x.Value == 1) == 5;
        }

        private static bool HasOneSite(IEnumerable<Site> sites)
        {
            return sites.Count() == 1;
        }

        private static bool HasTenTags(IEnumerable<Site> sites)
        {
            var tags = sites.Where(item => item.Tags != null && item.Tags.Split(',').Any()).SelectMany(item => item.Tags.Split(',')).ToList();
            return tags.Count == 10;
        }

        private static bool HasThreeSitesWithRatesGreaterThanThree(IEnumerable<Site> sites)
        {
            return sites.Count(x => x.Rate > 3.0) > 3;
        }

        private static bool HasOneRating(IEnumerable<Rate> rates)
        {
            return rates.Any();
        }

        private static bool HasTenSites(IEnumerable<Site> sites)
        {
            return sites.Count() == 10;
        }

        private static bool Has100Sites(IEnumerable<Site> sites)
        {
            return sites.Count() == 10;

        }

        private static bool HasTenRatingsWith5Stars(IEnumerable<Rate> rates)
        {
            return rates.Count(x => x.Value == 5) == 10;
        }
    }
}