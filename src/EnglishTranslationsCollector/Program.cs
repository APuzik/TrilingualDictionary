using Google.Apis.Auth.OAuth2;
using Google.Cloud.Translation.V2;
using MultiDictionaryCore.Core;
using MultiDictionaryCore.Core.Interfaces;
using MultiDictionaryCore.DBEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EnglishTranslationsCollector
{
    class Resp
    {
        [JsonProperty("resultNoTags")]
        public string ResultNoTags { get; set; }

        [JsonProperty("result")]
        public string Result { get; set; }
    }

    class D
    {
        [JsonProperty("d")]
        public Resp Response { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {

            //string key = File.ReadAllText(@"G:\AS\GoogleCloudKey\API Project-da0747c53711.json");
            //GoogleCredential creds = GoogleCredential.FromJson(key);
            //AdvancedTranslationClient client = AdvancedTranslationClient.Create(creds);
            //TranslationResult value = client.TranslateText(word, "en", "ru");
            //Console.WriteLine(value.TranslatedText);

            HttpClient client = new HttpClient();
            // client.BaseAddress = new Uri("http://www.translate.ru/services/soap.asmx");// https://translate.google.com/translate_a/");// "https://www.lingvolive.com/ru-ru/translate/ru-en/");
            IMultiLingualDictionary Dictionary = new MultiLingualDictionary();
            List<TermTranslation> translations = Dictionary.GetChildrenTranslations(1);
            string file = "res3.txt";
            for (int i = 18000; i < translations.Count; i++)
            {
                file = $@"D:\Projects\TrilingualDictionary\Translations\childres{i/1000}.txt";
                string word = translations[i].Value.Replace("#", "");
                string post = $@"{{ dirCode:'ru-en', template:'General', text:'{word}', lang:'ru', limit:'3000',useAutoDetect:true, key:'123', ts:'MainSite',tid:'', IsMobile:false}}";
                //                @"{ dirCode: 'ru-en', template: 'General', text: '%D0%B5%D0%B4%D0%B8%D0%BD%D0%B8%D1%86%D0%B0', lang: 'ru', limit: '3000', useAutoDetect: true, key: '123', ts: 'MainSite', tid: '', IsMobile: false}";
                HttpContent content = new StringContent(post, Encoding.UTF8);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                Task<HttpResponseMessage> res = client.PostAsync("http://www.translate.ru/services/soap.asmx/GetTranslation", content);
                res.Wait();
                string value = res.Result.Content.ReadAsStringAsync().Result;
                D result = JsonConvert.DeserializeObject<D>(value);
                if (result.Response.ResultNoTags.Length > 0)
                {
                    string[] values = result.Response.ResultNoTags.Split(new char[] { ',' });
                    if (values.Length < 3)
                    {
                        File.AppendAllText(file, $"Results not found for {word}.\r\n");
                    }
                    else
                    {
                        File.AppendAllText(file, $"Results for {word}:\r\n");
                        for (int j = 2; j < values.Length; j++)
                        {
                            File.AppendAllText(file, $"\t{values[j]}\r\n");
                        }
                    }
                }
                else
                {
                    value = result.Response.Result;
                    File.AppendAllText(file, $"Results for {word} (simple):\r\n");
                    File.AppendAllText(file, $"\t{value}\r\n");
                }
            }
            //var url = "single?client=t&sl=ru&tl=en&hl=ru&dt=t&ie=UTF-8&oe=UTF-8&otf=1&ssel=0&tsel=0&kc=7&q=один";
            //Task<string> res = client.GetStringAsync(url);// "single?client=t&sl=ru&tl=en&hl=ru&dt=at&dt=bd&dt=ex&dt=ld&dt=md&dt=qca&dt=rw&dt=rm&dt=ss&dt=t&ie=UTF-8&oe=UTF-8&source=btn&trs=1&inputm=1&ssel=3&tsel=0&kc=1&tk=787526.687207&q=%D0%B5%D0%B4%D0%B8%D0%BD%D0%B8%D1%86%D0%B0");// "%D0%B5%D0%B4%D0%B8%D0%BD%D0%B8%D1%86%D0%B0");
            //                                          //single?client=t&sl=ru&tl=en&hl=ru&dt=at&dt=bd&dt=ex&dt=ld&dt=md&dt=qca&dt=rw&dt=rm&dt=ss&dt=t&ie=UTF-8&oe=UTF-8&source=btn&trs=1&inputm=1&ssel=3&tsel=0&kc=0&tk=967758.572527&q=%D0%B5%D0%B4%D0%B8%D0%BD%D0%B8%D1%86%D0%B0
            //string value = res.Result;
            //File.AppendAllText($@"G:\AS\Projects\TrilingualDictionary\src\Translations\{word}2.html", value);
        }

        static void InsertTranslations(string folderPath)
        {
            IMultiLingualDictionary Dictionary = new MultiLingualDictionary();
            List<TermTranslation> translations = Dictionary.GetTopTranslations(1);
            int line = 0;
            int fileNum = -1;
            string[] lines = null;
            for (int i = 0; i < translations.Count; i++)
            {
                if(fileNum != i / 1000)
                {
                    fileNum = i / 1000;
                    string file = $@"D:\Projects\TrilingualDictionary\Translations\res{fileNum}.txt";
                    lines = File.ReadAllLines(file);
                }
                
                string word = translations[i].Value.Replace("#", "");
                bool isStarted = false;
                for(; line < lines.Length; line++)
                {
                    if (lines[line].Contains($"Results for {word}"))
                    {
                        isStarted = !isStarted;
                        if (!isStarted)
                            break;
                        continue;
                    }
                    else if (lines[line].Contains($"Results not found for {word}"))
                    {
                        continue;
                    }
                    else
                    {
                        //Term term = Dictionary.GetTermByWord(translations[i]);
                        string val = lines[line].Trim();
                        string other = "";
                        int index = val.IndexOf('[');
                        if(index > 0)
                        {
                            other = val.Substring(index);
                            val = val.Substring(0, index);
                        }
                        TermTranslation tt = new TermTranslation
                        {
                            LanguageId = 2,
                            Value = val,
                            TermId = translations[i].TermId
                        };

                        Dictionary.AddTranslation(tt);
                    }
                }
                
            }
        }
    }
}
