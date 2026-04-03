using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using viwik.CantoneseLearning.Model;

namespace viwik.CantoneseLearning.DataAccess
{
    public class DbObjectsFetcher
    {
        public static async Task<IEnumerable<MandarinWord>> GetMandarinWords()
        {
            string sql = "select * from MandarinWord";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<MandarinWord>(sql));
            }
        }

        public static async Task<IEnumerable<CantoneseWord>> GetCantoneseWords()
        {
            string sql = "select * from CantoneseWord";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<CantoneseWord>(sql));
            }
        }

        public static async Task<(IEnumerable<V_CantoneseWordSyllable> CantoneseSyllables, IEnumerable<V_MandarinWordSyllable> MandarinSyllables)> GetSyllables(string content)
        {
            var words = content.Select(item => item.ToString()).ToArray();
            string wordsIn = string.Join(',', words.Select(item => $"'{DbUtitlity.GetSafeValue(item)}'"));

            string cantoneseSql = $"select * from V_CantoneseWordSyllable where Word in({wordsIn}) and Hidden=0";
            string mandarinSql = $"select * from V_MandarinWordSyllable where Word in({wordsIn})";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                var cantoneseSyllables = (await connection.QueryAsync<V_CantoneseWordSyllable>(cantoneseSql));
                var mandarinSyllables = (await connection.QueryAsync<V_MandarinWordSyllable>(mandarinSql));

                return (cantoneseSyllables, mandarinSyllables);
            }
        }

        public static async Task<IEnumerable<V_SyllableRelation>> GetSyllableRelations(SyllableType type, string syllable)
        {
            string syllableType = "";

            switch (type)
            {
                case SyllableType.Mandarin:
                    syllableType = "Syllable_M";
                    break;
                case SyllableType.Cantonese_YP:
                    syllableType = "Syllable";
                    break;
                case SyllableType.Cantonese_GP:
                    syllableType = "Syllable_GP";
                    break;
                default:
                    syllableType = "Syllable_M";
                    break;
            }

            string sql = $"select * from V_SyllableRelation where {syllableType} = @Syllable order by Consonant,Vowel";

            Dictionary<string, object> para = new Dictionary<string, object>();

            para.Add("@Syllable", DbUtitlity.GetParameterValue(syllable));

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<V_SyllableRelation>(sql, para));
            }
        }

        public static async Task<IEnumerable<MandarinVowel>> GetMandarinVowels()
        {
            string sql = "select * from MandarinVowel";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<MandarinVowel>(sql));
            }
        }

        public static async Task<IEnumerable<V_CantoneseConsonant_YPGP>> GetCantoneseConsonant_YPGPs()
        {
            string sql = "select * from V_CantoneseConsonant_YPGP order by Consonant_YP";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<V_CantoneseConsonant_YPGP>(sql));
            }
        }

        public static async Task<IEnumerable<V_CantoneseVowel_YPGP>> GetCantoneseVowel_YPGPs()
        {
            string sql = "select * from V_CantoneseVowel_YPGP order by Vowel_YP";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<V_CantoneseVowel_YPGP>(sql));
            }
        }

        public static async Task<IEnumerable<V_CantoneseSyllableAlt>> GetCantoneseSyllableAlts(int syllableId)
        {
            string sql = "select * from V_CantoneseSyllableAlt where SyllableId=@SyllableId";

            Dictionary<string, object> para = new Dictionary<string, object>();

            para.Add("@SyllableId", DbUtitlity.GetParameterValue(syllableId.ToString()));

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<V_CantoneseSyllableAlt>(sql, para));
            }
        }

        public static async Task<IEnumerable<Mandarin2Cantonese>> GetMandarin2Cantoneses()
        {
            string sql = "select * from Mandarin2Cantonese order by Id";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<Mandarin2Cantonese>(sql));
            }
        }

        public static async Task<IEnumerable<V_Mandarin2Cantonese>> GetVMandarin2Cantoneses(bool excludeIgnored = false)
        {
            string sql = "select * from V_Mandarin2Cantonese";

            if (excludeIgnored)
            {
                sql += " where IgnoreWhenTranslate=0";
            }

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<V_Mandarin2Cantonese>(sql));
            }
        }


        public static async Task<IEnumerable<V_CantoneseExample>> GetVCantoneseExamples()
        {
            string sql = "select * from V_CantoneseExample";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<V_CantoneseExample>(sql));
            }
        }

        public static async Task<IEnumerable<CantoneseSynonym>> GetCantoneseSynonyms()
        {
            string sql = "select * from CantoneseSynonym";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<CantoneseSynonym>(sql));
            }
        }

        public static async Task<IEnumerable<CantoneseSentencePattern>> GetCantoneseSentencePatterns()
        {
            string sql = "select * from CantoneseSentencePattern";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<CantoneseSentencePattern>(sql));
            }
        }

        public static async Task<bool> HasUserDataTable(string dbFilePath, string tableName)
        {
            using (var connection = DbUtitlity.CreateDbConnection(dbFilePath))
            {
                string sql = $"SELECT 1 FROM sqlite_schema WHERE type= 'table' AND name=@Name";

                Dictionary<string, object> para = new Dictionary<string, object>();

                para.Add("@Name", DbUtitlity.GetParameterValue(tableName));

                bool? existing = (await connection.QueryAsync<bool>(sql, para))?.FirstOrDefault();

                return existing == true;
            }
        }

        public static async Task<IEnumerable<MediaFavoriteCategory>> GetMediaFavoriteCategories(string dbFilePath = null)
        {
            using (var connection = DbUtitlity.CreateDbConnection(dbFilePath))
            {
                string sql = "SELECT * from MediaFavoriteCategory order by Priority";

                return await connection.QueryAsync<MediaFavoriteCategory>(sql);
            }
        }

        public static async Task<IEnumerable<MediaFavorite>> GetMediaFavorites(string dbFilePath = null)
        {
            using (var connection = DbUtitlity.CreateDbConnection(dbFilePath))
            {
                string sql = "SELECT * from MediaFavorite";

                return await connection.QueryAsync<MediaFavorite>(sql);
            }
        }

        public static async Task<IEnumerable<MediaAccessHistory>> GetMediaAccessHistories(string dbFilePath = null)
        {
            using (var connection = DbUtitlity.CreateDbConnection(dbFilePath))
            {
                string sql = "SELECT * from MediaAccessHistory";

                return await connection.QueryAsync<MediaAccessHistory>(sql);
            }
        }

        public static async Task<IEnumerable<CantonesePlatform>> GetCantonesePlatforms()
        {
            string sql = "select * from CantonesePlatform";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<CantonesePlatform>(sql));
            }
        }

        public static async Task<IEnumerable<CantoneseMediaType>> GetCantoneseMediaTypes()
        {
            string sql = "select * from CantoneseMediaType";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<CantoneseMediaType>(sql));
            }
        }

        public static async Task<IEnumerable<CantoneseTeacher>> GetCantoneseTeachers()
        {
            string sql = "select * from CantoneseTeacher";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<CantoneseTeacher>(sql));
            }
        }

        public static async Task<IEnumerable<CantoneseSubject>> GetCantoneseSubjects()
        {
            string sql = $"select * from CantoneseSubject order by Priority";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<CantoneseSubject>(sql));
            }
        }

        public static async Task<CantoneseSubject> GetCantoneseSubject(int subjectId)
        {
            string sql = $"select * from CantoneseSubject where Id={subjectId}";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<CantoneseSubject>(sql))?.FirstOrDefault();
            }
        }

        public static async Task<CantoneseSubject> GetCantoneseSubjectByEnName(string enName)
        {
            string sql = "select * from CantoneseSubject where Name_EN=@EnName";

            Dictionary<string, object> para = new Dictionary<string, object>();
            para.Add("EnName", enName);

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<CantoneseSubject>(sql, para)).FirstOrDefault();
            }
        }

        public static async Task<IEnumerable<V_CantoneseSubjectMedia>> GetVCantoneseSubjectMedias(int subjectId)
        {
            string sql = $"select * from V_CantoneseSubjectMedia where SubjectId={subjectId} order by Priority";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<V_CantoneseSubjectMedia>(sql));
            }
        }

        public static async Task<IEnumerable<CantoneseTopic>> GetCantoneseTopics(int subjectId)
        {
            string sql = $"select * from CantoneseTopic where SubjectId={subjectId} order by Priority";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<CantoneseTopic>(sql));
            }
        }

        public static async Task<IEnumerable<CantoneseTopicDetail>> GetCantoneseTopicDetails(int topicId, string keyword = null)
        {
            keyword = DbUtitlity.GetSafeValue(keyword);

            string keywordCondition = string.IsNullOrEmpty(keyword) ? "" : $"and Name like '%{keyword}%'";

            string sql = $"select * from CantoneseTopicDetail where TopicId={topicId} {keywordCondition} order by Priority";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<CantoneseTopicDetail>(sql));
            }
        }

        public static async Task<CantoneseMediaExtraInfo> GetCantoneseMediaExtraInfo(int medialId)
        {
            string sql = $"select * from CantoneseMediaExtraInfo where MediaId={medialId}";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<CantoneseMediaExtraInfo>(sql))?.FirstOrDefault();
            }
        }

        public static async Task<MediaFavorite> GetMediaFavoriteByMediaId(int mediaId)
        {
            string sql = $"select * from MediaFavorite where MediaId={mediaId}";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<MediaFavorite>(sql))?.FirstOrDefault();
            }
        }

        public static async Task<IEnumerable<V_CantoneseTopicDetailMedia>> GetVCantoneseTopicDetailMedias(int topicId, string keyword = null)
        {
            keyword = DbUtitlity.GetSafeValue(keyword);

            string keywordCondition = string.IsNullOrEmpty(keyword) ? "" : $"and ((MediaTitleExt is not null and MediaTitleExt like '%{keyword}%') or (MediaTitleExt is null and MediaTitle like '%{keyword}%'))";

            string sql = $"select * from V_CantoneseTopicDetailMedia where TopicId={topicId} {keywordCondition} order by Priority";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<V_CantoneseTopicDetailMedia>(sql));
            }
        }

        public static async Task<IEnumerable<V_CantoneseTopicDetailMediaPlayTime>> GetVCantoneseTopicDetailMediaPlayTimes(int topicDetailMediaId)
        {
            string sql = $"select * from V_CantoneseTopicDetailMediaPlayTime where TopicDetailMediaId={topicDetailMediaId} order by StartTime";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<V_CantoneseTopicDetailMediaPlayTime>(sql));
            }
        }

        public static async Task<IEnumerable<V_CantoneseTopicDetailMediaPlayTime>> GetVCantoneseSubjectMediaPlayTimes(int subjectMediaId)
        {
            string sql = $"select * from V_CantoneseSubjectMediaPlayTime where SubjectMediaId={subjectMediaId} order by StartTime";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<V_CantoneseTopicDetailMediaPlayTime>(sql));
            }
        }

        public static async Task<IEnumerable<V_CantoneseVocabularyMedia>> GetVVocabularyMedias(int vocabularyId)
        {
            string sql = $"select * from V_CantoneseVocabularyMedia where VocabularyId={vocabularyId} order by Priority";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<V_CantoneseVocabularyMedia>(sql));
            }
        }

        public static async Task<IEnumerable<V_CantoneseVocabularyMediaPlayTime>> GetVCantoneseVocabularyMediaPlayTimes(int vocabularyMediaId)
        {
            string sql = $"select * from V_CantoneseVocabularyMediaPlayTime where VocabularyMediaId={vocabularyMediaId} order by StartTime";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<V_CantoneseVocabularyMediaPlayTime>(sql));
            }
        }

        public static async Task<IEnumerable<CantoneseConsonant>> GetCantoneseConsonants()
        {
            string sql = "select * from CantoneseConsonant order by Priority";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<CantoneseConsonant>(sql));
            }
        }

        public static async Task<IEnumerable<CantoneseVowel>> GetCantoneseVowels()
        {
            string sql = $@"select * from CantoneseVowel order by Priority";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<CantoneseVowel>(sql));
            }
        }

        public static async Task<IEnumerable<V_CantoneseConsonantMedia>> GetVCantoneseConsonantMedias(int constantId)
        {
            string sql = $"select * from V_CantoneseConsonantMedia where ConsonantId={constantId} order by Priority";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<V_CantoneseConsonantMedia>(sql));
            }
        }

        public static async Task<IEnumerable<V_CantoneseVowelMedia>> GetVCantoneseVowelMedias(int vowelId)
        {
            string sql = $"select * from V_CantoneseVowelMedia where VowelId={vowelId} order by Priority";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<V_CantoneseVowelMedia>(sql));
            }
        }

        public static async Task<IEnumerable<V_CantoneseConsonantMediaPlayTime>> GetVCantoneseConsonantMediaPlayTimes(int consonantMediaId)
        {
            string sql = $"select * from V_CantoneseConsonantMediaPlayTime where ConsonantMediaId={consonantMediaId} order by StartTime";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<V_CantoneseConsonantMediaPlayTime>(sql));
            }
        }

        public static async Task<IEnumerable<V_CantoneseVowelMediaPlayTime>> GetVCantoneseVowelMediaPlayTimes(int vowelMediaId)
        {
            string sql = $"select * from V_CantoneseVowelMediaPlayTime where VowelMediaId={vowelMediaId} order by StartTime";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return (await connection.QueryAsync<V_CantoneseVowelMediaPlayTime>(sql));
            }
        }

        public static async Task<IEnumerable<V_MediaAccessHistory>> GetVMediaAccessHistories()
        {
            using (var connection = DbUtitlity.CreateDbConnection())
            {
                string sql = "SELECT * from V_MediaAccessHistory";

                return await connection.QueryAsync<V_MediaAccessHistory>(sql);
            }
        }

        public static async Task<IEnumerable<V_MediaFavorite>> GetVMediaFavorites()
        {
            string sql = $"select * from V_MediaFavorite";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return await connection.QueryAsync<V_MediaFavorite>(sql);
            }
        }

        public static async Task<bool> IsMediaFavoriteCategoryNameExisting(bool isAdd, string name, int? id)
        {
            using (var connection = DbUtitlity.CreateDbConnection())
            {
                string sql = $"select 1 from MediaFavoriteCategory where Name=@Name";

                if (!isAdd)
                {
                    sql += $" and Id<>{id}";
                }

                Dictionary<string, object> para = new Dictionary<string, object>();
                para.Add("@Name", name);

                return (await connection.QueryAsync<bool>(sql, para))?.FirstOrDefault() == true;
            }
        }

        public static async Task<bool> IsMediaFavoriteCategoryBeRefering(List<int> ids)
        {
            using (var connection = DbUtitlity.CreateDbConnection())
            {
                string sql = $"select count(1) as Num from MediaFavorite where CategoryId in({string.Join(",", ids)})";

                int? num = (await connection.QueryAsync<int>(sql))?.FirstOrDefault();

                return num > 0;
            }
        }

        public static async Task<IEnumerable<CantoneseEqualsMandarin>> GetCantoneseEqualsMandarins()
        {
            string sql = $"select * from CantoneseEqualsMandarin";

            using (var connection = DbUtitlity.CreateDbConnection())
            {
                return await connection.QueryAsync<CantoneseEqualsMandarin>(sql);
            }
        }
    }
}
