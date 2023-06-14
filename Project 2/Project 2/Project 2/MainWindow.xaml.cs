using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace BabbleSample
{
    public partial class MainWindow : Window
    {
        private string input;                                 // input file
        private string[] words;                               // input file broken into array of words
        private int wordCount = 200;                          // number of words to babble
        private Dictionary<string, ArrayList> myHashTable = new Dictionary<string, ArrayList>();    // input file stored into a hashtable

        public MainWindow()
        {
            InitializeComponent();
        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.FileName = "Sample"; // Default file name
            ofd.DefaultExt = ".txt"; // Default file extension
            ofd.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            if ((bool)ofd.ShowDialog())
            {
                textBlock1.Text = "Loading file " + ofd.FileName + "\n";
                input = System.IO.File.ReadAllText(ofd.FileName);  // read file
                words = Regex.Split(input, @"\s+");       // split into array of words
            }
        }

        private void analyzeInput(int order)
        {
            if (order > 0)
            {
                MessageBox.Show("Analyzing at order: " + order);
                myHashTable.Clear();        // clear the hashtable before storing new data into it
            }

            // Analyze the input file in the first-order
            if (order == 1)
            {
                analyze_first();
                dump();
            }

            // Analyze the input file in the second-order
            else if (order == 2)
            {
                analyze_second();
                dump();
            }

            // Analyze the input file in the third-order
            else if (order == 3)
            {
                analyze_third();
                dump();
            }

            // Analyze the input file in the fourth-order
            else if (order == 4)
            {
                analyze_fourth();
                dump();
            }

            // Analyze the input file in the fifth-order
            else if (order == 5)
            {
                analyze_fifth();
                dump();
            }
        }

        private void babbleButton_Click(object sender, RoutedEventArgs e)
        {
            // Babble the file and output the result.

            textBlock1.Text = "";

            int step = 0;   // keep track of the word count after each time the loop runs

            for (int i = 0; i < Math.Min(wordCount, words.Length); i += step)

                foreach (KeyValuePair<string, ArrayList> item in myHashTable)
                {
                    // output the key
                    textBlock1.Text += item.Key + " ";

                    // pick and output a random value from the value list
                    var random = new Random();
                    ArrayList list = item.Value;
                    int index = random.Next(list.Count);
                    textBlock1.Text += list[index] + " ";

                    // calculate the number of words being outputted in this loop and update the step variable
                    string[] key_item = Regex.Split(item.Key, @"\s+");
                    int key_step = key_item.Length;
                    int value_step = list.Count;
                    step = key_step + value_step;
                }
        }

        private void orderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            analyzeInput(orderComboBox.SelectedIndex);
        }

        private void analyze_first()
        {
            // Analyze the file in the first-order
            foreach (int i in Enumerable.Range(0, words.Length - 1))
            {
                string word = words[i];
                if (!myHashTable.ContainsKey(word))         // check if the word is in the hashtable as a key
                    myHashTable.Add(word, new ArrayList()); // if not, create a new (key,value) pair
                myHashTable[word].Add(words[i + 1]);        // add the next word into the arraylist as one possible successor
            }
        }

        private void analyze_second()
        {
            // Analyze the file in the second-order
            foreach (int i in Enumerable.Range(0, words.Length - 2))
            {
                string word = words[i] + " " + words[i + 1];
                if (!myHashTable.ContainsKey(word))           // check if the 2-word sequence is in the hashtable as a key
                    myHashTable.Add(word, new ArrayList());
                myHashTable[word].Add(words[i + 2]);          // add the word after the sequence into the arraylist
            }
        }

        private void analyze_third()
        {
            // Analyze the file in the third-order
            foreach (int i in Enumerable.Range(0, words.Length - 3))
            {
                string word = words[i] + " " + words[i + 1] + " " + words[i + 2];
                if (!myHashTable.ContainsKey(word))           // check if the 3-word sequence is in the hashtable as a key
                    myHashTable.Add(word, new ArrayList());
                myHashTable[word].Add(words[i + 3]);
            }
        }

        private void analyze_fourth()
        {
            // Analyze the file in the fourth-order
            foreach (int i in Enumerable.Range(0, words.Length - 4))
            {
                string word = words[i] + " " + words[i + 1] + " " + words[i + 2] + " " + words[i + 3];
                if (!myHashTable.ContainsKey(word))           // check if the 4-word sequence is in the hashtable as a key
                    myHashTable.Add(word, new ArrayList());
                myHashTable[word].Add(words[i + 4]);
            }
        }

        private void analyze_fifth()
        {
            // Analyze the file in the fifth-order
            foreach (int i in Enumerable.Range(0, words.Length - 5))
            {
                string word = words[i] + " " + words[i + 1] + " " + words[i + 2] + " " + words[i + 3] + " " + words[i + 4];
                if (!myHashTable.ContainsKey(word))           // check if the 5-word sequence is in the hashtable as a key
                    myHashTable.Add(word, new ArrayList());
                myHashTable[word].Add(words[i + 5]);
            }
        }

        private void dump()
        {
            // Output all the unique keys and corresponding values
            textBlock1.Text = "";
            foreach (KeyValuePair<string, ArrayList> entry in myHashTable)
            {
                textBlock1.Text += "{" + entry.Key + "} ->";    // output the key
                foreach (string name in entry.Value)            // output all the values in the same line
                    textBlock1.Text += " " + name;
                textBlock1.Text += "\n";                        // create a new line
            }
        }
    }
}