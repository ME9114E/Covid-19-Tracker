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

namespace Corona_stats
{
    public partial class Form1 : Form
    {
        private string tot;

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


            String url = "https://raw.githubusercontent.com/datasets/covid-19/master/data/time-series-19-covid-combined.csv";
            System.Net.WebClient client2 = new System.Net.WebClient();
            String csv = client2.DownloadString(url);


            List<OneCountry> Landen = new List<OneCountry>();

            string[] lines = csv.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            lines = lines.Skip(1).ToArray();

            int ind = new int();
            DateTime date1 = new DateTime();
            OneCountry land = new OneCountry();
            foreach (string line in lines)
            {
                string[] entries = line.Split(',');


                //check bestaan er al landen
                //if (Landen.Count == 0)
                //{
                //    ind = 0;
                //    date1 = DateTime.ParseExact(entries[0], "yyyy-MM-dd", null);
                //}
                //else
                //{
                // check bestaat land al
                if (line != "")
                {


                    if (land.country == entries[1] && land.provinceOrState == entries[2])
                    {
                        // bestaat datum al?
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
                        //nieuw land
                        land = new OneCountry();
                        land.country = entries[1];
                        land.provinceOrState = entries[2];
                        ind = 0;
                        date1 = DateTime.ParseExact(entries[0], "yyyy-MM-dd", null);
                    }

                    //}

                    land.index = ind;
                    land.addTimestamp(date1);
                    if (entries[5] != "") { land.addCases(Int32.Parse(entries[5])); }
                    if (entries[6] != "") { land.addRecovered(Int32.Parse(entries[6])); }
                    if (entries[7] != "") { land.addDeaths(Int32.Parse(entries[7])); }
                    

                }
            }

            // https://raw.githubusercontent.com/datasets/covid-19/master/data/time-series-19-covid-combined.csv

            //string[] landen = { "Belgium", "Italy", "Spain", "Netherlands" };

            //foreach (string land in landen)
            //    {
            //        String url = "https://corona.lmao.ninja/countries/"+land;
            //        System.Net.WebClient client = new System.Net.WebClient();
            //        String json = client.DownloadString(url);

            //        var dictionary = JsonConvert.DeserializeObject<IDictionary>(json);

            //        foreach (DictionaryEntry entry in dictionary)
            //            {
            //                tot = tot + (entry.Key + ": " + entry.Value) +Environment.NewLine;
            //            }

            //        tot = tot + Environment.NewLine;
            //    }



            foreach (IDictionary dictionary in responseBodyJson)
            {
                //    OneCountry land = new OneCountry();

                //    foreach (DictionaryEntry entry in dictionary)
                //    {
                //        //tot = tot + (entry.Key + ": " + entry.Value) + Environment.NewLine;
                //        // int index = land.cases.Length;
                //        if (entry.Key.Equals("country") && entry.Value != null) { land.country = "" + entry.Value; }
                //        if (entry.Key.Equals("countryInfo") && entry.Value != null) { land.addFlag("" + entry.Value); }
                //        if (entry.Key.Equals("cases") && entry.Value != null) { land.addCases(int.Parse("" + entry.Value)); }
                //        if (entry.Key.Equals("todayCases") && entry.Value != null) { land.addtodayCases(int.Parse("" + entry.Value)); }
                //        if (entry.Key.Equals("deaths") && entry.Value != null) { land.addDeaths(int.Parse("" + entry.Value)); }
                //        if (entry.Key.Equals("todayDeaths") && entry.Value != null) { land.addtodayDeaths(int.Parse("" + entry.Value)); }
                //        if (entry.Key.Equals("recovered") && entry.Value != null) { land.addRecovered(int.Parse("" + entry.Value)); }
                //        if (entry.Key.Equals("active") && entry.Value != null) { land.addactive(int.Parse("" + entry.Value)); }
                //        if (entry.Key.Equals("critical") && entry.Value != null) { land.addcritical(int.Parse("" + entry.Value)); }
                //        if (entry.Key.Equals("casesPerOneMillion") && entry.Value != null) { land.addcasesPerOneMillion(int.Parse("" + entry.Value)); }

                //    }
                //    Landen.Add(land);

                //}
                //foreach (OneCountry country in Landen)
                //{

                //    tot = tot + country.getTimestamp() + Environment.NewLine;
                //    tot = tot + "country:           " + country.country + Environment.NewLine;
                //    tot = tot + "cases:             " + country.getCases() + Environment.NewLine;
                //    tot = tot + "todaycases:        " + country.getTodayCases() + Environment.NewLine;
                //    tot = tot + "deaths:            " + country.getDeaths() + Environment.NewLine;
                //    tot = tot + "todaydeaths:       " + country.getTodayDeaths() + Environment.NewLine;
                //    tot = tot + "recovered:         " + country.getRecoveries() + Environment.NewLine;
                //    tot = tot + "active:            " + country.getActive() + Environment.NewLine;
                //    tot = tot + "critical:          " + country.getCritical() + Environment.NewLine;
                //    tot = tot + "cases per million: " + country.getCasesPerMillion() + Environment.NewLine;
                //    tot = tot + Environment.NewLine;
                //}
                //for (int x = 1; x < 9; x++)
                //{
                //    chart1.Series["cases"].Points.AddXY(Landen[x].country, Landen[x].getCases());
                //    chart1.Series["cases"].Points[x - 1].ToolTip = "" + Landen[x].country + Environment.NewLine + "cases: " + Landen[x].getCases() + Environment.NewLine + "recovered: " + Landen[x].getRecoveries();
                //    //chart1.Series["cases"].Points[x - 1].Label = "" + Landen[x].getCases();
                //    chart1.Series["recovered"].Points.AddXY(Landen[x].country, Landen[x].getRecoveries());
                //    //chart1.Series["recovered"].Points[x - 1].Label= "" + Landen[x].getRecoveries();
                //    chart1.Series["recovered"].Points[x - 1].ToolTip = "" + Landen[x].country + Environment.NewLine + "cases: " + Landen[x].getCases() + Environment.NewLine + "recovered: " + Landen[x].getRecoveries();
                //}

                //for (int x = 1; x < 8; x++)
                //{
                //    chart2.Series["cases"].Points.AddXY(Landen[x].country, Landen[x].getCasesPerMillion());
                //    chart2.Series["cases"].Points[x - 1].ToolTip = "" + Landen[x].country + Environment.NewLine + "cases: " + Landen[x].getCases() + Environment.NewLine + "cases per million " + Landen[x].getCasesPerMillion();
                //}
                //textBox1.Text = tot;
            }


        }
    }
    }

    

