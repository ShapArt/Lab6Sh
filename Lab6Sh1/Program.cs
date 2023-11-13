using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;


namespace Cs_lab06
{
    class Program
    {

        struct Weather
        {
            public string Country;
            public string Name;
            public double Temp;
            public string Description;
            public Weather(string _country, string _name, double _temp, string _desc)
            {
                Country = _country;
                Name = _name;
                Temp = _temp;
                Description = _desc;
            }
        }
        static async Task Main(string[] args)
        {
            var random = new System.Random();
            double lon = random.NextDouble() * 360 - 180;
            double lat = random.NextDouble() * 180 - 90;
            string api = "69d628518916cfeb2075778ed593e6c6";
            var client = new HttpClient();
            var content = await client.GetStringAsync($"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={api}");

            List<Weather> weathers = new List<Weather>(50);

            while (weathers.Count() < 50)
            {
                lon = random.NextDouble() * 360 - 180;
                lat = random.NextDouble() * 180 - 90;
                client = new HttpClient();
                content = await client.GetStringAsync($"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={api}");
                Console.WriteLine(content);

                Regex reCountry = new Regex("(?<=\"country\":\")[^\"]+(?=\")");
                Regex reName = new Regex("(?<=\"name\":\")[^\"]+(?=\")");
                Regex reTemp = new Regex("(?<=\"temp\":)[^:]+(?=,)");
                Regex reDesc = new Regex("(?<=\"description\":\")[^\"]+(?=\")");

                MatchCollection cMatches = reCountry.Matches(content);
                MatchCollection nMatches = reName.Matches(content);
                MatchCollection tMatches = reTemp.Matches(content);
                MatchCollection dMatches = reDesc.Matches(content);

                //WriteLine(tMatches[0]);
                if (cMatches.Count != 0)
                {
                    if (nMatches.Count != 0)
                    {
                        Weather tmpWeathear = new Weather(cMatches[0].Value, nMatches[0].Value, Convert.ToDouble(tMatches[0].Value, System.Globalization.CultureInfo.InvariantCulture) - 273, dMatches[0].Value);
                        weathers.Add(tmpWeathear);
                    }
                    else
                    {
                        continue;
                    }

                }
                else
                {
                    continue;
                }
            }

            foreach (Weather obj in weathers)
            {
                Console.WriteLine($"Сountry: {obj.Country}  Name: {obj.Name} Temp: {obj.Temp}  Desc: {obj.Description}");
            }


            var tempCollection = (from i in weathers
                                  orderby i.Temp
                                  select i).ToList();


            Console.WriteLine("\n");
            Console.WriteLine($"Min temp in {tempCollection[0].Country} Temp: {tempCollection[0].Temp}");
            Console.WriteLine($"Max temp in {tempCollection[9].Country} Temp: {tempCollection[9].Temp}");

            var averTemp = (from i in weathers
                            let p = i.Temp
                            select p).Average();
            Console.WriteLine("\n");
            Console.WriteLine($"Средняя температура {averTemp}");

            var countryCount = (from i in weathers
                                group new
                                {
                                    i.Name,
                                    i.Temp,
                                    i.Description
                                }
                                by i.Country into weatherInCountr
                                orderby weatherInCountr.Key
                                select weatherInCountr).ToList();

            Console.WriteLine("\n");
            Console.WriteLine($"Количество стран {countryCount.Count}");








        }
    }
}

/*
 {"coord":{"lon":16.6193,"lat":6.6426},"weather":[{"id":804,"main":"Clouds","description":"overcast clouds","icon":"04d"}],"base":"stations","main":{"temp":298.93,"feels_like":299.57,"temp_min":298.93,"temp_max":298.93,"pressure":1015,"humidity":77,"sea_level":1015,"grnd_level":949},"visibility":10000,"wind":{"speed":1,"deg":87,"gust":1.55},"clouds":{"all":100},"dt":1699861178,"sys":{"country":"CF","sunrise":1699850575,"sunset":1699893164},"timezone":3600,"id":2387546,"name":"Bozoum","cod":200}
*/