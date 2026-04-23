using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace ApiForGame
{
    internal class GameApi
    {
        private readonly HttpClient _client;
        private readonly string server = "http://192.168.0.151:8000";
        //http://10.192.111.132:8000
        //http://192.168.0.151:8000
        // API key for authenticating requests. Set via environment variable "API_KEY" or replace the placeholder below.
        private readonly string _apiKey;

        public GameApi()
        {
            _client = new HttpClient();
            // Try to read API key from environment variable, fallback to placeholder
            _apiKey = Environment.GetEnvironmentVariable("API_KEY") ?? "game_api_key_123";
        }

        #region-------PRIVATE HELPER METHODES-------

        #region FILLTER FILTHY WORDS

        // 1) roots
        private static readonly string[] ForbiddenRoots =
        {
            // English
            "sex", "porn", "genital", "masturb", "prostitut",
            "excrement", "feces", "urine", "anus",

            //  Hungarian
            "fasz",
            "pina",
            "kurva",
            "buzi",
            "geci",
            "szar",
            "picsa",
            "segg",
            "basz",
            "ribanc",
            "szop",
            "hugy",

        };

        // 2) Regex samples
        private static readonly Regex[] ForbiddenPatterns =
        {
            new Regex("f+u+[ck]+", RegexOptions.IgnoreCase),
            new Regex("sh+i+t+", RegexOptions.IgnoreCase),
            new Regex("b+i+t+c+h+", RegexOptions.IgnoreCase),
            new Regex("a+s+s+", RegexOptions.IgnoreCase),
            new Regex("d+i+c+k+", RegexOptions.IgnoreCase),
            new Regex("p+e+n+i+s+", RegexOptions.IgnoreCase),
            new Regex("v+a+g+i+n+a+", RegexOptions.IgnoreCase),
            new Regex("c+u+n+t+", RegexOptions.IgnoreCase),
            new Regex("s+e+x+", RegexOptions.IgnoreCase),
            new Regex("p+o+r+n+", RegexOptions.IgnoreCase),

            new Regex("k[\\W_]*[uúûü][\\W_]*r[\\W_]*v[\\W_]*a", RegexOptions.IgnoreCase),
            new Regex("f[\\W_]*a[\\W_]*sz", RegexOptions.IgnoreCase),
            new Regex("p[\\W_]*i[\\W_]*n[\\W_]*a", RegexOptions.IgnoreCase),
            new Regex("g[\\W_]*e[\\W_]*c[\\W_]*i", RegexOptions.IgnoreCase),
            new Regex("b[\\W_]*u[\\W_]*z[\\W_]*i", RegexOptions.IgnoreCase),
            new Regex("sz[\\W_]*a[\\W_]*r", RegexOptions.IgnoreCase),
            new Regex("p[\\W_]*i[\\W_]*cs[\\W_]*a", RegexOptions.IgnoreCase),
            new Regex("s[\\W_]*e[\\W_]*gg", RegexOptions.IgnoreCase),
            new Regex("b[\\W_]*a[\\W_]*sz", RegexOptions.IgnoreCase),
        };

        // 4) Normalization
        private static string Normalize(string input)
        {
            string s = input.ToLowerInvariant();

            s = s.Replace("@", "a")
                 .Replace("4", "a")
                 .Replace("3", "e")
                 .Replace("1", "i")
                 .Replace("!", "i")
                 .Replace("0", "o")
                 .Replace("$", "s");

            return s;
        }

        // Main methode
        public static bool ContainsForbiddenContent(string input)
        {
            string normalized = Normalize(input);

            foreach (var pattern in ForbiddenPatterns)
                if (pattern.IsMatch(normalized))
                    return true;

            foreach (var root in ForbiddenRoots)
                if (normalized.Contains(root))
                    return true;

            /*foreach (var root in ForbiddenRoots)
                if (Math.Abs(normalized.Length - root.Length) <= 2 &&
                    Levenshtein(normalized, root) <= 2)
                    return true;*/

            /*if (LooksSuspicious(normalized))
                return true;*/

            return false;
        }

        #endregion     

        private async Task<string> SendRequestAsync(HttpMethod method, string endpoint, object data = null)
        {
            string url = $"{server}/{endpoint}";

            // --- GET / DELETE esetén query string generálás ---
            if ((method == HttpMethod.Get || method == HttpMethod.Delete) && data != null)
            {
                var queryParams = new List<string>();

                if (data is Dictionary<string, object> dict)
                {
                    foreach (var kvp in dict)
                    {
                        queryParams.Add($"{kvp.Key}={WebUtility.UrlEncode(Convert.ToString(kvp.Value))}");
                    }
                }
                else
                {
                    // Anonim objektumok kezelése
                    var props = data.GetType().GetProperties();
                    foreach (var prop in props)
                    {
                        var val = prop.GetValue(data);
                        queryParams.Add($"{prop.Name}={WebUtility.UrlEncode(Convert.ToString(val))}");
                    }
                }

                if (queryParams.Count > 0)
                {
                    url += "?" + string.Join("&", queryParams);
                }
            }
            using (UnityEngine.Networking.UnityWebRequest request = new UnityEngine.Networking.UnityWebRequest(url, method.Method))
            {
                // API Kulcs beállítása
                if (!string.IsNullOrEmpty(_apiKey) && _apiKey != "REPLACE_WITH_YOUR_API_KEY")
                {
                    request.SetRequestHeader("X-Api-Key", _apiKey);
                }
                // --- POST / PUT esetén JSON body ---
                if (data != null && method != HttpMethod.Get && method != HttpMethod.Delete)
                {
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                    request.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(bodyRaw);
                    request.SetRequestHeader("Content-Type", "application/json");
                }
                request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();

                // Kérés indítása és várakozás aszinkron módon
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                // Eredmény ellenõrzése
                if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    return request.downloadHandler.text;
                }
                else
                {
                    // Ha a telefonon hiba van, a Logcatben most már TÖKÉLETESEN fogjuk látni!
                    UnityEngine.Debug.LogError($"? [API HIBA] Végpont: {endpoint} | Ok: {request.error} | Válasz: {request.downloadHandler.text}");
                    return "";
                }
            }

            //    var request = new HttpRequestMessage(method, url);

            //// Add API key header if configured
            //if (!string.IsNullOrEmpty(_apiKey) && _apiKey != "REPLACE_WITH_YOUR_API_KEY")
            //{
            //    request.Headers.Remove("X-Api-Key");
            //    request.Headers.Add("X-Api-Key", _apiKey);
            //}

            //// --- POST / PUT esetén JSON body ---
            //if (data != null && method != HttpMethod.Get && method != HttpMethod.Delete)
            //{
            //    string json = JsonConvert.SerializeObject(data);
            //    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            //}

            //var response = await _client.SendAsync(request);
            //return await response.Content.ReadAsStringAsync();
        }

        private string ValidateUserFields(object fields)
        {
            Dictionary<string, object> dict;

            if (fields is Dictionary<string, object> d)
            {
                dict = d;
            }
            else
            {
                dict = new Dictionary<string, object>();
                foreach (var prop in fields.GetType().GetProperties())
                {
                    dict[prop.Name] = prop.GetValue(fields);
                }
            }

            if (dict.TryGetValue("email", out var emailObj) && emailObj is string email)
            {
                if (!IsValidEmail(email))
                    return "Error: invaid email format";
            }

            if (dict.TryGetValue("display_name", out var nameObj) && nameObj is string displayName)
            {
                if (displayName.Length > 25)
                    return "Error: Too long display name (max 25 character).";
                /*if (ContainsForbiddenContent(displayName))
                    return "Error: Display name contains forbidden content.1";*/
            }

            return null;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private object MergeIdAndFields(string id, object fields)
        {
            var dict = new Dictionary<string, object>();

            foreach (var prop in fields.GetType().GetProperties())
            {
                dict[prop.Name] = prop.GetValue(fields);
            }

            dict["user_id"] = id;

            return dict;
        }
        private object MergeIdAndFieldsForDailys(string id, object fields)
        {
            var dict = new Dictionary<string, object>();

            foreach (var prop in fields.GetType().GetProperties())
            {
                dict[prop.Name] = prop.GetValue(fields);
            }

            dict["id"] = id;

            return dict;
        }
        #endregion

        #region-------DAILY CHALLENGE-------
        /// <summary>
        /// InsertDailyChallenge(new{...})<br/><br/>
        /// 
        /// dailyc_id = (string) <br/>
        /// data_json = (string) <br/>
        /// date = (date) <br/>
        /// </summary>
        public async Task<string> InsertDailyChallenge(object fields)
        {
            return await SendRequestAsync(HttpMethod.Post, "dailychallenges/insert.php", fields);
        }

        /// <summary>
        /// SelectDailyChallenge(c_id(int))
        /// </summary>
        public async Task<string> SelectDailyChallenge(int DCId)
        {
            return await SendRequestAsync(HttpMethod.Get, "dailychallenges/select.php", new { id = DCId });
        }

        /// <summary>
        /// UpdateDailyChallenge(c_id(int), new{columns = "..."})<br/><br/>
        /// 
        /// data_json, date
        /// </summary>
        public async Task<string> UpdateDailyChallenge(string DCId, object fields)
        {
            return await SendRequestAsync(HttpMethod.Put, "dailychallenges/update.php", MergeIdAndFieldsForDailys(DCId, fields));
        }

        /// <summary>
        /// DeleteDailyChallenge(c_id(int))
        /// </summary>
        public async Task<string> DeleteDailyChallenge(string DCId)
        {
            return await SendRequestAsync(HttpMethod.Delete, "dailychallenges/delete.php", new { id = DCId });
        }

        
        #endregion

        #region-------LEADERBOARD DAILYC-------

        /// <summary>
        /// InsertDailyCRecord(user_id(string), new{...})<br/><br/>
        /// 
        /// dailyc_time = (string) <br/>
        /// date = (string)
        /// </summary>
        public async Task<string> InsertDailyCRecord(string userId, object fields)
        {
            return await SendRequestAsync(HttpMethod.Post, "dailycrecord/insert.php", MergeIdAndFields(userId, fields));
        }

        /// <summary>
        /// SelectDailyCRecord(user_id(string), new{columns = "..."})<br/><br/>
        /// 
        /// dailyc_time, date 
        /// </summary>
        public async Task<string> SelectDailyCRecord(string userId, object fields)
        {
            return await SendRequestAsync(HttpMethod.Get, "dailycrecord/select.php", MergeIdAndFields(userId, fields));
        }

        /// <summary>
        /// SelectAllDailyCRecords()
        /// </summary>
        public async Task<string> SelectAllDailyCRecords()
        {
            return await SendRequestAsync(HttpMethod.Get, "dailycrecord/selectAll.php");
        }

        /// <summary>
        /// UpdateDailyCRecord(user_id(string), new{...})<br/><br/>
        /// 
        /// dailyc_time = (string) <br/>
        /// date = (string)
        /// </summary>
        public async Task<string> UpdateDailyCRecord(string userId, object fields)
        {
            return await SendRequestAsync(HttpMethod.Put, "dailycrecord/update.php", MergeIdAndFields(userId, fields));
        }

        #endregion

        #region-------LEADERBOARD SEEDRUN-------

        /// <summary>
        /// InsertSpeedrunRecord(user_id(string), new{...})<br/><br/>
        /// 
        /// speedrun_amount = (string) <br/>
        /// date = (string)
        /// </summary>
        public async Task<string> InsertSpeedrunRecord(string userId, object fields)
        {
            return await SendRequestAsync(HttpMethod.Post, "speedrun/insert.php", MergeIdAndFields(userId, fields));
        }

        /// <summary>
        /// SelectSpeedrunRecord(user_id(string), new{columns = "..."})<br/><br/>
        /// 
        /// speedrun_amount, date 
        /// </summary>
        public async Task<string> SelectSpeedrunRecord(string userId, object fields)
        {
            return await SendRequestAsync(HttpMethod.Get, "speedrun/select.php", MergeIdAndFields(userId, fields));
        }

        /// <summary>
        /// SelectAllSpeedrunRecords()
        /// </summary>
        public async Task<string> SelectAllSpeedrunRecords()
        {
            return await SendRequestAsync(HttpMethod.Get, "speedrun/selectAll.php");
        }

        /// <summary>
        /// UpdateSpeedrunRecord(user_id(string), new{...})<br/><br/>
        /// 
        /// speedrun_amount = (string) <br/>
        /// date = (string)
        /// </summary>
        public async Task<string> UpdateSpeedrunRecord(string userId, object fields)
        {
            return await SendRequestAsync(HttpMethod.Put, "speedrun/update.php", MergeIdAndFields(userId, fields));
        }

        #endregion

        #region-------USER ACHIEVEMENT-------

        /// <summary>
        /// InsertAchievementRecord(user_id(string), new{...})<br/><br/>
        /// 
        /// status = (string) <br/>
        /// achievement_id = (string)
        /// </summary>
        public async Task<string> InsertAchievementRecord(string userId, object fields)
        {
            return await SendRequestAsync(HttpMethod.Post, "achievement/insert.php", MergeIdAndFields(userId, fields));
        }

        /// <summary>
        /// SelectAchievementRecord(user_id(string), new{columns = "..."})<br/><br/>
        /// 
        /// status, achievement_id
        /// </summary>
        public async Task<string> SelectAchievementRecord(string userId, object fields)
        {
            return await SendRequestAsync(HttpMethod.Get, "achievement/select.php", MergeIdAndFields(userId, fields));
        }


        /// <summary>
        /// UpdateAchievementRecord(user_id(string), new{...})<br/><br/>
        /// 
        /// status = (int) <br/>
        /// achievement_id = (int)
        /// </summary>
        public async Task<string> UpdateAchievementRecord(string userId, object fields)
        {
            return await SendRequestAsync(HttpMethod.Put, "achievement/update.php", MergeIdAndFields(userId, fields));
        }

        #endregion

        #region-------USER SHOP ITEM-------

        /// <summary>
        /// InsertShopRecord(user_id(string), item_number(int))
        /// </summary>
        public async Task<string> InsertShopRecord(string userId, int itemNum)
        {
            return await SendRequestAsync(HttpMethod.Post, "shop/insert.php", new { user_id = userId, item_number = itemNum });
        }

        /// <summary>
        /// SelectShopRecord(user_id(string), new{columns = "..."})<br/><br/>
        /// 
        /// item_number 
        /// </summary>
        public async Task<string> SelectShopRecord(string userId, object fields)
        {
            return await SendRequestAsync(HttpMethod.Get, "shop/select.php", MergeIdAndFields(userId, fields));
        }

        #endregion

        #region------USER STATS--------

        /// <summary>
        /// InsertUserStat(user_id(string), new{...})<br/><br/>
        /// 
        /// coins = (int) <br/>
        /// best_speedrun_amount = (int) <br/>
        /// best_dailyc_time = (int) <br/>
        /// levels_completed = (int) <br/>
        /// </summary>
        public async Task<string> InsertUserStat(string userId, object fields)
        {
            return await SendRequestAsync(HttpMethod.Post, "stats/insert.php", MergeIdAndFields(userId, fields));
        }

        /// <summary>
        /// SelectUserStat(user_id(string), new{columns = "..."})<br/><br/>
        /// 
        /// coins, best_speedrun_amount, best_dailyc_time, levels_completed
        /// </summary>
        public async Task<string> SelectUserStat(string userId, object fields)
        {
            return await SendRequestAsync(HttpMethod.Get, "stats/select.php", MergeIdAndFields(userId, fields));
        }

        /// <summary>
        /// UpdateUserStat(user_id(string), new{...})<br/><br/>
        /// 
        /// coins = (int) <br/>
        /// best_speedrun_amount = (int) <br/>
        /// best_dailyc_time = (int) <br/>
        /// levels_completed = (int) <br/>
        /// </summary>
        public async Task<string> UpdateUserStat(string userId, object fields)
        {
            return await SendRequestAsync(HttpMethod.Put, "stats/update.php", MergeIdAndFields(userId, fields));
        }
        public async Task<string> SelectAllUserStats()
        {
            return await SendRequestAsync(HttpMethod.Get, "stats/selectAll.php");
        }
        #endregion

        #region--------USER--------

        /// <summary>
        /// InsertUser(new{...})<br/><br/>
        /// 
        /// user_id = (string) <br/>
        /// google_id = (string) <br/>
        /// email = (string) <br/>
        /// display_name = (string) <br/>
        /// status = (int) <br/>
        /// </summary>
        public async Task<string> InsertUser(object fields)
        {
            var error = ValidateUserFields(fields);
            if (error != null)
                return error;

            return await SendRequestAsync(HttpMethod.Post, "user/insert.php", fields);
        }

        /// <summary>
        /// SelectUser(user_id(string), new{columns = "..."})<br/><br/>
        /// 
        /// display_name, status
        /// </summary>
        public async Task<string> SelectUser(string userId, object fields)
        {
            return await SendRequestAsync(HttpMethod.Get, "user/select.php", MergeIdAndFields(userId, fields));
        }

        /// <summary>
        /// UpdateUser(user_id(string), new{...})<br/><br/>
        /// 
        /// google_id = (string) <br/>
        /// email = (string) <br/>
        /// display_name = (string) <br/>
        /// status = (int) <br/>
        /// </summary>
        public async Task<string> UpdateUser(string userId, object fields)
        {
            var dict = new Dictionary<string, object>();

            // A fields objektum tulajdonságait átmásoljuk a szótárba
            foreach (var prop in fields.GetType().GetProperties())
            {
                dict[prop.Name] = prop.GetValue(fields);
            }

            var error = ValidateUserFields(fields);
            if (error != null)
                return error;

            // Manuálisan hozzáadjuk a hiányzó ID-t
            dict["user_id"] = userId;

            return await SendRequestAsync(HttpMethod.Put, "user/update.php", dict);
        }

        /// <summary>
        /// DeleteUser(user_id(string))
        /// </summary>
        public async Task<string> DeleteUser(string userId)
        {
            return await SendRequestAsync(HttpMethod.Delete, "user/delete.php", new { id = userId });
        }
        /// <summary>
        /// Lekéri a felhasználót a Google azonosítója alapján (Fiók rekonstrukcióhoz)
        /// </summary>
        public async Task<string> SelectUserByGoogleId(string googleId)
        {
            // Fontos: A PHP fejlesztõnek biztosítania kell, hogy a select.php (vagy egy új végpont)
            // képes legyen visszaadni a 'user_id'-t, ha csak egy 'google_id'-t kap bemenetként!
            return await SendRequestAsync(HttpMethod.Get, "user/select.php", new { google_id = googleId });
        }
        #endregion
    }
}
