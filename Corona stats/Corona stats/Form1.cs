using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Globalization;

namespace Corona_stats
{
    public partial class Form1 : Form
    {
        private string tot;
        List<OneCountry> Landen = new List<OneCountry>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            string responseBody;
            List<Dictionary<string, dynamic>> responseBodyJson = new List<Dictionary<string, dynamic>>();
            Task.Run(async () =>
            {
                var response = await client.GetAsync("https://corona.lmao.ninja/countries");
                responseBody = response.Content.ReadAsStringAsync().Result;
                responseBodyJson = JsonConvert.DeserializeObject<List<Dictionary<string, dynamic>>>(responseBody);
            }).Wait();

            //
            // get information from the timeseries
            //

            String url = "https://raw.githubusercontent.com/datasets/covid-19/master/data/time-series-19-covid-combined.csv";
            System.Net.WebClient client2 = new System.Net.WebClient();
            String csv = client2.DownloadString(url);


            

            string[] lines = csv.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            lines = lines.Skip(1).ToArray();

            int ind = new int();
            DateTime date1 = new DateTime();
            OneCountry land = new OneCountry();
            foreach (string line in lines)
            {
                string[] entries = line.Split(',');
                
                // empty line?
                if (line != "")
                {

                    // check country excists?
                    if (land.country == entries[1] && land.provinceOrState == entries[2])
                    {
                        // check date is used?
                        DateTime date2 = DateTime.ParseExact(entries[0], "yyyy-MM-dd", null);
                        if (date1 != date2)
                        {
                            ind++;
                            date1 = date2;
                        }

                    }
                    else
                    {
                        if (ind != 0) { Landen.Add(land); }
                        //new country
                        land = new OneCountry();
                        land.country = entries[1];
                        land.provinceOrState = entries[2];
                        ind = 0;
                        date1 = DateTime.ParseExact(entries[0], "yyyy-MM-dd", null);
                    }

                    //}

                    land.index = ind;
                    land.addTimestamp(date1);
                    if (entries[entries.Length - 3] != "") { land.addCases(Int32.Parse(entries[entries.Length - 2])); }
                    //if (entries[entries.Length - 2] != "") { land.addRecovered(Int32.Parse(entries[entries.Length - 2])); }
                    if (entries[entries.Length - 1] != "") { land.addDeaths(Int32.Parse(entries[entries.Length - 1])); }
                    

                }
            }

            //
            // get information from corona.lmao.ninja/countries
            //
            Boolean checkCountry = false;

            foreach (IDictionary dictionary in responseBodyJson)
            {
                foreach (DictionaryEntry entry in dictionary)
                {
                    if (entry.Key.Equals("country") && !checkCountry)
                    {
                        foreach (OneCountry land2 in Landen)
                        {
                            if (land2.country == ("" + entry.Value) && land2.provinceOrState=="") {land =land2; land.index++; land.addTimestamp(DateTime.Today); checkCountry = true; break; }
                        }
                        if (!checkCountry)
                        { //country doesn't exist yet 



                          // Landen.Add(land);
                        }
                    }
                   
                    if (checkCountry)
                    {
                        if (entry.Key.Equals("countryInfo") && entry.Value != null) { land.addFlag("" + entry.Value); }
                        if (entry.Key.Equals("cases") && entry.Value != null) { land.addCases(int.Parse("" + entry.Value)); }
                        if (entry.Key.Equals("todayCases") && entry.Value != null) { land.addtodayCases(int.Parse("" + entry.Value)); }
                        if (entry.Key.Equals("deaths") && entry.Value != null) { land.addDeaths(int.Parse("" + entry.Value)); }
                        if (entry.Key.Equals("todayDeaths") && entry.Value != null) { land.addtodayDeaths(int.Parse("" + entry.Value)); }
                        if (entry.Key.Equals("recovered") && entry.Value != null) { land.addRecovered(int.Parse("" + entry.Value)); }
                        if (entry.Key.Equals("active") && entry.Value != null) { land.addactive(int.Parse("" + entry.Value)); }
                        if (entry.Key.Equals("critical") && entry.Value != null) { land.addcritical(int.Parse("" + entry.Value)); }
                        if (entry.Key.Equals("casesPerOneMillion") && entry.Value != null) { land.addcasesPerOneMillion(double.Parse("" + entry.Value)); }
                    }  
                                
                }
                checkCountry = false;
                           //if (entry.Key.Equals("country") && entry.Value != null) { land.country = "" + entry.Value; }
                           
            }

            // read csv file and write over existing 

            string filename = "ItalySpainBelgiumNetherlands.csv";
            List<string> linesFile = File.ReadAllLines(filename).ToList();
            linesFile.RemoveAt(0);

            foreach (string line in linesFile)
            {
                string[] entries = line.Split(',');
                // each line update in an existing OneCountry-instance 
                // empty line?
                if (line != "")
                {
                    // search country

                    // loop trough Landen 
                    OneCountry land3 = Landen.Find(x => x.country == entries[0]);
                    // country exists? --> break
                    // end loop

                    // yess country exists
                    if (land3.country != null)
                    {
                        // search date
                        // loop trough country.timestamp[] 
                        int index3 = Array.FindIndex(land3.timestamp, (element => element == DateTime.ParseExact(entries[1], "yyyy-MM-dd", null)));
                        // date exists -->break
                        // end loop

                        // date exists
                        if (index3 > 0)
                        {
                            // use the index from that country.timestamp
                            land3.index = index3;
                          
                        }
                        else
                        {
                            // date doesn't exist
                            // use the last index; index++;
                            index3 = Array.FindLast(land3.cases, (element => element > 0));

                        }
                       
                    }
                    else
                    {
                        //no country doesn't exists
                        //make new OneCountry addcountry
                        land3 = new OneCountry();
                        // add country and date

                    }

                    //fillup class properties  
                    if (entries[2] != "") { land3.addCases(int.Parse("" + entries[2])); }// cases
                    if (entries[3] != "") { land3.addDeaths(int.Parse("" + entries[3])); } // deaths
                    if (entries[4] != "") { land3.addRecovered(int.Parse("" + entries[4])); } // recovered
                    if (entries[5] != "") { land3.addtodayCases(int.Parse("" + entries[5])); } // todaycases
                    if (entries[6] != "") { land3.addtodayDeaths(int.Parse("" + entries[6])); } // todaydeaths
                    if (entries[7] != "") { land3.addtodayRecovered(int.Parse("" + entries[7])); } // today recoveries
                    if (entries[8] != "") { land3.addactive(int.Parse("" + entries[8])); } // active
                    if (entries[9] != "") { land3.addcritical(int.Parse("" + entries[9])); } // critical
                    if (entries[10] != "") { land3.addcasesPerOneMillion(double.Parse("" + entries[10])); } //casesPerOneMillion
                    // if addcountry --> Landen.add(addcountry)


                }




            }

            //write json file

            try
            {
                var jsonToWrite = JsonConvert.SerializeObject(Landen, Formatting.Indented);

                using (var writer = new StreamWriter("COVID19.json"))
                {
                    writer.Write(jsonToWrite);
                }
            }
            catch
            {
                //ignore
            }

            // view   
            Landen.Sort((x, y) => x.getHighestCaseNumber().CompareTo(y.getHighestCaseNumber()));
           
            foreach (OneCountry country in Landen)
            {
                // look for highest cases index
                int maxvalue = country.cases.Max();
                int indexView = Array.FindIndex(country.cases, element => element == maxvalue);
                
                country.index = indexView;

                tot = tot + country.getTimestamp() + Environment.NewLine;
                tot = tot + "country:           " + country.country + Environment.NewLine;
                tot = tot + "cases:             " + country.getCases() + Environment.NewLine;
                tot = tot + "todaycases:        " + country.getTodayCases() + Environment.NewLine;
                tot = tot + "deaths:            " + country.getDeaths() + Environment.NewLine;
                tot = tot + "todaydeaths:       " + country.getTodayDeaths() + Environment.NewLine;
                tot = tot + "recovered:         " + country.getRecoveries() + Environment.NewLine;
                tot = tot + "active:            " + country.getActive() + Environment.NewLine;
                tot = tot + "critical:          " + country.getCritical() + Environment.NewLine;
                tot = tot + "cases per million: " + country.getCasesPerMillion() + Environment.NewLine;
                tot = tot + Environment.NewLine;
            }
            for (int x = 133; x < 142; x++)
            {
                chart1.Series["cases"].Points.AddXY(Landen[x].country, Landen[x].getCases());
                chart1.Series["cases"].Points[x - 133].ToolTip = "" + Landen[x].country + Environment.NewLine + "cases: " + Landen[x].getCases() + Environment.NewLine + "recovered: " + Landen[x].getRecoveries();
                chart1.Series["recovered"].Points.AddXY(Landen[x].country, Landen[x].getRecoveries());
                chart1.Series["recovered"].Points[x - 133].ToolTip = "" + Landen[x].country + Environment.NewLine + "cases: " + Landen[x].getCases() + Environment.NewLine + "recovered: " + Landen[x].getRecoveries();
            }

            textBox1.Text = tot;
            

            foreach (OneCountry Land in Landen)
            {
                string item = Land.country;
                if (Land.provinceOrState!="") { item = item + ", " + Land.provinceOrState; }

                listBox1.Items.Add(item);
                
            }
            
            if (listBox1.SelectedItem != null)
            {
                OneCountry chck1Country = Landen.Find(x => x.country.Equals(listBox1.SelectedItem));

              
                int targetNumber = 150;
                // startpoint cases closed to 150
                var nearest = chck1Country.cases.OrderBy(x => Math.Abs((long)x - targetNumber)).First();
                int indexMin = Array.FindIndex(chck1Country.cases, element => element == nearest);

                // endpoint cases=Max
                int indexMax = Array.FindIndex(chck1Country.cases, element => element == chck1Country.cases.Max());

                for (int indexview3 = indexMin; indexview3 < (indexMax+1); indexview3++) 
                {

                    chck1Country.index = indexview3;
                    chart3.Series[chck1Country.country + "active"].Points.AddXY(chck1Country.getTimestamp(), chck1Country.getCases() );
                    chart3.Series[chck1Country.country + "closed case"].Points.AddXY(chck1Country.getTimestamp(), (chck1Country.getTodayRecovered() + chck1Country.getTodayDeaths() ) );
                }
            }

           
            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            OneCountry chck1Country = new OneCountry();

            if (listBox1.Text.Contains(","))
            {
                string[] state = listBox1.SelectedItem.ToString().Split(',');
                state[1] = state[1].Substring(1);
                chck1Country = Landen.Find(x => x.provinceOrState.Equals(state[1]));
            }
            else
            {
                chck1Country =Landen.Find(x => x.country.Equals(listBox1.SelectedItem));
            }
            
            foreach (var series in chart3.Series)
            {
                series.Points.Clear();
            }
            chart3.Series.Clear();

            chart3.Series.Add(chck1Country.country + " active");
            chart3.Series.Add(chck1Country.country + " closed case");

            chart3.Series[chck1Country.country + " active"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            int targetNumber = 150;
            
            // startpoint cases closed to 150
            var nearest = chck1Country.cases.OrderBy(x => Math.Abs((long)x - targetNumber)).First();
           

            int indexMin = Array.FindIndex(chck1Country.cases, element => element == nearest);
            if (chck1Country.cases[indexMin] == 0) { indexMin = 0; } //startingcases were higher then 2 times targetnumber --> 0
            // endpoint cases=Max
            int indexMax = Array.FindIndex(chck1Country.cases, element => element == chck1Country.cases.Max());

            for (int indexview3 = indexMin; indexview3 < (indexMax + 1); indexview3++)
            {

                chck1Country.index = indexview3;
                chart3.Series[chck1Country.country + " active"].Points.AddXY(chck1Country.getTimestamp(), chck1Country.getCases());
                chart3.Series[chck1Country.country + " closed case"].Points.AddXY(chck1Country.getTimestamp(), (chck1Country.getTodayRecovered() + chck1Country.getTodayDeaths()));
                //chart3.Series["Belgium closed cases"].Points.AddXY(chck1Country.getTimestamp(), (chck1Country.getTodayCases() + chck1Country.getTodayDeaths()));
                //chart3.Series["Belgium closed cases"].Points.AddXY(chck1Country.getTimestamp(), (chck1Country.getTodayCases() + chck1Country.getTodayDeaths()));
            }
        }
    }
    }
    

    

