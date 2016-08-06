using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hypnofrog;
using Hypnofrog.DBModels;

namespace Hypnofrog.ViewModels
{
    public class AchievmentChecker
    {
        public string Result { get; set; }

        public List<DBModels.Achievement> NewAchievments { get; set; }

        public AchievmentChecker(List<DBModels.Site> sites, List<DBModels.Rate> rates, List<DBModels.Achievement> log, string userid)
        {
            List<bool> achievmentmask = RunChekers(sites, rates);
            List<string> allachievments = GetAllAchievments();
            NewAchievments = new List<Achievement>();
            Result = CheckCoincidences(achievmentmask, allachievments, GetUserAchievments(log), userid);
        }


        private string CheckCoincidences(List<bool> achievmentmask, List<string> allachievments, List<string> log, string userid)
        {
            for (int i = 0; i< achievmentmask.Count()-1; i++)
            {
                if (achievmentmask[i])
                {
                    if (!log.Contains(allachievments[i]))
                    {
                        DBModels.Achievement achievment = new Achievement() {Name = allachievments[i], Time=DateTime.Now, User = userid};
                        NewAchievments.Add(achievment);
                        return allachievments[i];
                    }
                }
            }
            return "";

        }

        //иметь три сайта - В некотором государстве
        //иметь 5 рейтингов с единицами - Я Дартаньян
        //один  сайт - В некотором царстве...
        //иметь не менее десяти тегов - Архивариус
        //три сайта с рейтингом не менее 3 - Три богатыря
        // иметь рейтинг - Держи печеньку
        //иметь 10 сайтов - Кочегар
        //иметь 100 сайтов - Воу Воу Палехче
        //10 рейтингов с 5 - Беспринциптный


        private string descRUHasThreeSites = "Иметь три сайта.";
        private string descRUHasFiveRatingWithOnes = "Поставить 5 отметок с единицами";
        private string descRUHasOneSite = "Написать один сайт";
        private string descRUHasTenTags = "Иметь на сайтах 10 тегов";
        private string descRUHasThreeSitesWithRatesGreaterThanThree = "Иметь три сайта с рейтингом не менее 3";
        private string descRUHasOneRating = "Поставить одну оценку";
        private string descRUHasTenSites = "Написать 10 сайтов";
        private string descRUHas100Sites = "Написать 100 сайтов";
        private string descRUHasTenRatingsWith5Stars = "Поставить 10 отметок с пятёрками";

        private string nameHasThreeSites = "В некотором государстве...";
        private string nameHasFiveRatingWithOnes = "Я Дартаньян";
        private string nameHasOneSite = "В некотором царстве...";
        private string nameHasTenTags = "Архивариус";
        private string nameHasThreeSitesWithRatesGreaterThanThree = "Три богатыря";
        private string nameHasOneRating = "Держи печеньку";
        private string nameHasTenSites = "Кочегар";
        private string nameHas100Sites = "Воу Воу Палехче";
        private string nameHasTenRatingsWith5Stars = "Беспринциптный";

        public List<string> GetAllAchievmentsDescriptionsRU()
        {
            List<string> allachievments = new List<string>();
            allachievments.Add(descRUHasThreeSites);
            allachievments.Add(descRUHasFiveRatingWithOnes);
            allachievments.Add(descRUHasOneSite);
            allachievments.Add(descRUHasTenTags);
            allachievments.Add(descRUHasThreeSitesWithRatesGreaterThanThree);
            allachievments.Add(descRUHasOneRating);
            allachievments.Add(descRUHasTenSites);
            allachievments.Add(descRUHas100Sites);
            allachievments.Add(descRUHasTenRatingsWith5Stars);
            return allachievments;
        }

        public List<string> GetAllAchievments()
        {
            List<string> allachievments = new List<string>();
            allachievments.Add(nameHasThreeSites);
            allachievments.Add(nameHasFiveRatingWithOnes);
            allachievments.Add(nameHasOneSite);
            allachievments.Add(nameHasTenTags);
            allachievments.Add(nameHasThreeSitesWithRatesGreaterThanThree);
            allachievments.Add(nameHasOneRating);
            allachievments.Add(nameHasTenSites);
            allachievments.Add(nameHas100Sites);
            allachievments.Add(nameHasTenRatingsWith5Stars);
            return allachievments;
        }

        private List<bool> RunChekers(List<DBModels.Site> sites, List<DBModels.Rate> rates)
        {
            List<bool> checker = new List<bool>();
            checker.Add(HasThreeSites(sites));
            checker.Add(HasFiveRatingWithOnes(rates));
            checker.Add(HasOneSite(sites));
            checker.Add(HasTenTags(sites));
            checker.Add(HasThreeSitesWithRatesGreaterThanThree(sites));
            checker.Add(HasOneRating(rates));
            checker.Add(HasTenSites(sites));
            checker.Add(Has100Sites(sites));
            checker.Add(HasTenRatingsWith5Stars(rates));
            return checker;
        }

        public List<string> GetUserAchievments(List<DBModels.Achievement> log)
        {
            List<string> userachievments = new List<string>();
            foreach (var item in log)
            {
                userachievments.Add(item.Name);
            }
            return userachievments;
        }

        private bool HasThreeSites(List<DBModels.Site> sites)
        {
            return sites.Count() == 3;
        }

        private bool HasFiveRatingWithOnes(List<DBModels.Rate> rates)
        {
            return rates.Where(x => x.Value == 1).Count() == 5;
        }

        private bool HasOneSite(List<DBModels.Site> sites)
        {
            return sites.Count() == 1;
        }

        private bool HasTenTags(List<DBModels.Site> sites)
        {
            List<string> tags = new List<string>();
            foreach (var item in sites)
                if (item.Tags.Split(',').Count()>0)
                    foreach (var tag in item.Tags.Split(','))
                        tags.Add(tag);
            return tags.Count() == 10;
        }

        private bool HasThreeSitesWithRatesGreaterThanThree(List<DBModels.Site> sites)
        {
            return sites.Where(x => x.Rate > 3.0).Count() > 3;
        }

        private bool HasOneRating(List<DBModels.Rate> rates)
        {
            return rates.Count() == 1;
        }

        private bool HasTenSites(List<DBModels.Site> sites)
        {
            return sites.Count() == 10;
        }

        private bool Has100Sites(List<DBModels.Site> sites)
        {
            return sites.Count() == 10;

        }

        private bool HasTenRatingsWith5Stars(List<DBModels.Rate> rates)
        {
            return rates.Where(x => x.Value == 5).Count() == 10;
        }
    }
}