using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corona_stats
{
    class OneCountry
    {
        public string country { get; set; }
        public string provinceOrState { get; set; }
        public string flag { get; set; }
        public DateTime[] timestamp = new DateTime[150];
        public int index { get; set; }
        public int[] cases=new int[150];
        public int[] deaths = new int[150];
        public int[] recovered = new int[150];
        public int[] todayCases = new int[150];
        public int[] todayDeaths = new int[150];
        public int[] todayRecovered = new int[150];
        public int[] active = new int[150];
        public int[] critical = new int[150];
        public double[] casesPerOneMillion = new double[150];

        public void addCases(int Cases)
            {
            cases[index] = Cases;
            //timestamp[index] = DateTime.UtcNow;
            if (index > 0) { addtodayCases(cases[index] - cases[index - 1]);  }
            }
        public void addDeaths(int Deaths)
        {
            deaths[index] = Deaths;
            if (index > 0) { addtodayDeaths(deaths[index] - deaths[index - 1]);  }
        }
        public void addRecovered(int Recovered)
        {
            recovered[index] = Recovered;
            if (index > 0)
            {
                addtodayRecovered(recovered[index] - recovered[index - 1]);
                addactive(cases[index] - deaths[index] - recovered[index]);
            }
        }
        public void addtodayCases(int TodayCases)
        {
           todayCases[index] = TodayCases; 
        }
        public void addtodayDeaths(int TodayDeaths)
        {
            todayDeaths[index] = TodayDeaths; 
        }
        public void addtodayRecovered(int TodayRecovered)
        {
           todayRecovered[index] = TodayRecovered; 
        }
        public void addactive(int Active)
        {
           active[index] = Active; 
        }
        public void addcritical(int Critical)
        {
           critical[index] = Critical; 
        }
        public void addcasesPerOneMillion(double CasesPerOneMillion)
        {
            casesPerOneMillion[index] = CasesPerOneMillion;
            index++;
        }
        
        public void addFlag(string Flag)
        {
            var dictionary = JsonConvert.DeserializeObject<IDictionary>(Flag);

            foreach (DictionaryEntry entry in dictionary)
            {
                if (entry.Key.Equals("flag")) { flag = "" + entry.Value; }

            }
        }

        public void addTimestamp(DateTime tmstmp)
        {
            timestamp[index] = tmstmp;
        }

        public int getCases() { return cases[index]; }
        public int getDeaths() { return deaths[index]; }
        public int getRecoveries() { return recovered[index]; }
        public int getFlag() { return flag[index]; }
        public int getTodayCases() { return todayCases[index]; }
        public int getTodayDeaths() { return todayDeaths[index]; }
        public int getTodayRecovered() { return todayRecovered[index]; }
        public int getCritical() { return critical[index]; }
        public int getActive() { return active[index]; }
        public double getCasesPerMillion() { return casesPerOneMillion[index]; }
        public DateTime getTimestamp() { return timestamp[index]; }
        public int getHighestCaseNumber() { return cases.Max();  }
        public int getLowestCaseNumber() { return cases.Min(); }
    }



}
