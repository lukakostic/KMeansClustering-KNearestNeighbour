using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KMeans
{
    public partial class Form1 : Form
    {
        DataTable dt;
        DataTable dtTest;

        public List<ItemObject> items;

        public string defaultVals = @"Type,    Sugar, Flour, Water, Yeast
Cake,     0.8,   0.2,  0.4,     0
Bread,    0.1,   0.8,  0.5,    0.5
Doughnut, 0.4,   0.5,  0.5,    0.2
Cream,    0.9,    0,   0.7,     0
Pastry,   0.2,   0.2,  0.2,    0.6";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dt = new DataTable("KMeans"); //Top table
            dtTest = new DataTable("KMeansTest"); //Bottom table



            dataGridView1.DataSource = dt;
            dataGridView2.DataSource = dtTest;
        }

        public void AddColumn(string name, bool isFloat)
        {
            if (isFloat)
            {
                dt.Columns.Add(name, typeof(float));
                dtTest.Columns.Add(name, typeof(float));
            }
            else
            {
                dt.Columns.Add(name, typeof(string));
                dtTest.Columns.Add(name, typeof(string));
            }
        }

        public void AddRow(string s, int rw, int cl)
        {
            dt.Rows[rw - 1].SetField(cl, s);
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e) //Load csv from file
        {
            var lines = System.IO.File.ReadAllLines(openFileDialog1.FileName);
            ParseCSV(ref lines);
        }

        public void ParseCSV(ref string[] lines)
        {
            try
            {




                for (int i = 0; i < lines.Length; i++)
                {
                    ParseCSVLine(ref lines[i], i);
                    dt.Rows.Add(dt.NewRow());
                    //ParseCSVLine(ref lines[i], i-1);

                }

                CleanEmpty();
                Memorize(); // Assign all elements & their features to a list
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public void ParseCSVLine(ref string s, int row)
        {
            string[] vals = s.Split(',');
            for (int i = 0; i < vals.Length; i++)
            {
                if (row == 0)
                {
                    if (i == 0)
                    {
                        AddColumn(vals[i], false);
                    }
                    else
                    {
                        AddColumn(vals[i], true);
                    }
                }
                else
                {

                    AddRow(vals[i], row, i);
                }
            }
            if (row == 0)
            {
                dtTest.Rows.Add(dtTest.NewRow());
                dtTest.Rows[0].SetField(0, "-------------------------");
            }
        }

        private void button2_Click(object sender, EventArgs e) //Whats a csv?
        {
            MessageBox.Show(@"A .csv (comma separated values) file
is just a normal .txt file in similar format:

Type,    Sugar, Flour, Water, Yeast
Cake,     0.8,   0.2,  0.4,     0
Bread,    0.1,   0.8,  0.5,    0.5
Doughnut, 0.4,   0.5,  0.5,    0.2
Cream,    0.9,    0,   0.7,     0
Pastry,   0.2,   0.2,  0.2,    0.6

First (0th) row is labels for data,
its not looked at by the program.

First column must be a string (text)
First column is type or classification
others are floats or ints (numbers)

you dont have to line them up
");
        }

        void CleanEmpty() //Delete empty table rows
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i].ItemArray[0].ToString() == "") dt.Rows[i].Delete();

            }
        }

        private void button1_Click(object sender, EventArgs e) //Load from file button
        {
            openFileDialog1.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var lines = defaultVals.Split(Environment.NewLine.ToCharArray());
            ParseCSV(ref lines);
        }

        private void button4_Click(object sender, EventArgs e) //Predict nearest neighbour button
        {
            try
            {
                KNearestNeighbour();
                MessageBox.Show("Done. Check the type(s) in bottom table");
            }
            catch (Exception ex) { MessageBox.Show("Error. Check if you filled all bottom fields"); }
        }

        public void KNearestNeighbour()
        {
            for (int bi = 0; bi < dtTest.Rows.Count; bi++)
            {


                var features = new float[dtTest.Rows[bi].ItemArray.Length - 1];
                var distances = new float[items.Count];
                for (int i = 1; i < dtTest.Rows[bi].ItemArray.Length; i++)
                {
                    features[i - 1] = (float)dtTest.Rows[bi].ItemArray[i];
                }

                for (int a = 0; a < items.Count; a++)
                {
                    //////////////////////////////////////////////////////////////////////Core of K-Nearest Neighbour: You just find the closest element, features are coordinates
                    var dist = 0f;
                    for (int i = 0; i < features.Length; i++)
                    {
                        dist += (features[i] - items[a].features[i]) * (features[i] - items[a].features[i]); // Square distance for each feature
                    }
                    distances[a] = (float)Math.Sqrt((double)dist); //Pythagoras to calc n-dimensional distance
                }

                int minI = 0;
                float minDist = float.MaxValue;
                for (int i = 0; i < distances.Length; i++)
                {
                    if (distances[i] < minDist)
                    {
                        minI = i;
                        minDist = distances[i];
                    }
                }

                dtTest.Rows[bi].SetField(0, items[minI].typeName);
            }
        }

        public void KMeansClustering()
        {
            var clusters = new List<ItemObject>(); //Instead of comparing to each element, we compare to center of cluster of each type
            var itemsPerCluster = new List<int>();

            for (int a = 0; a < items.Count; a++)
            {
                bool added = false;
                for (int b = 0; b < clusters.Count; b++)
                {
                    if (clusters[b].typeName == items[a].typeName)
                    {
                        for (int i = 0; i < items[a].features.Length; i++)
                        {
                            clusters[b].features[i] += items[a].features[i];
                        }

                        itemsPerCluster[b] += 1;
                        added = true;
                        break;
                    }
                }
                if (!added)
                {
                    var i = new ItemObject();
                    i.typeName = items[a].typeName;
                    i.features = items[a].features;
                    clusters.Add(i);
                    itemsPerCluster.Add(1);
                }
            }
            for (int b = 0; b < clusters.Count; b++) //Average the features for each cluster to get the center
            {
                for (int i = 0; i < clusters[b].features.Length; i++)
                {
                    clusters[b].features[i] /= (float)itemsPerCluster[b];
                }
            }

            for (int bi = 0; bi < dtTest.Rows.Count; bi++)
            {

                var features = new float[dtTest.Rows[bi].ItemArray.Length - 1];
                for (int i = 1; i < dtTest.Rows[bi].ItemArray.Length; i++)
                {
                    features[i - 1] = (float)dtTest.Rows[bi].ItemArray[i];
                }
                var distances = new float[clusters.Count];


                for (int a = 0; a < clusters.Count; a++)
                {
                    //////////////////////////////////////////////////////////////////////Core of K-Means Clustering: You just find the closest cluster, features are coordinates
                    var dist = 0f;
                    for (int i = 0; i < features.Length; i++)
                    {
                        dist += (features[i] - clusters[a].features[i]) * (features[i] - clusters[a].features[i]); // Square distance for each feature
                    }
                    distances[a] = (float)Math.Sqrt((double)dist); //Pythagoras to calc n-dimensional distance
                }

                int minI = 0;
                float minDist = float.MaxValue;
                for (int i = 0; i < distances.Length; i++)
                {
                    if (distances[i] < minDist)
                    {
                        minI = i;
                        minDist = distances[i];
                    }
                }

                dtTest.Rows[bi].SetField(0, clusters[minI].typeName);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                KMeansClustering();
                MessageBox.Show("Done. Check the type(s) in bottom table");
            }
            catch (Exception ex) { MessageBox.Show("Error. Check if you filled all bottom fields"); }
        }

        public void Memorize() //Read elements & features and add them to the list
        {
            items = new List<ItemObject>();

            for (int a = 0; a < dt.Rows.Count; a++)
            {
                var it = new ItemObject();
                it.typeName = (string)dt.Rows[a].ItemArray[0];

                it.features = new float[dt.Rows[a].ItemArray.Length - 1];

                for (int i = 1; i < dt.Rows[a].ItemArray.Length; i++)
                {
                    it.features[i - 1] = (float)dt.Rows[a].ItemArray[i];
                }

                items.Add(it);

            }


        }



        public class ItemObject
        {
            public string typeName;
            public float[] features;
        }

    }
}
