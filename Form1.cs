using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CryptoLog
{
    public partial class Form1 : Form
    {
        public static bool isChecked;

        public static string connectionString;

        public Form1()
        {
            InitializeComponent();

            ListView.CheckForIllegalCrossThreadCalls = false;

            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("Message", 500);
            listView1.Columns.Add("Date", 200);

            CheckBox.CheckForIllegalCrossThreadCalls = false;

            textBox3.Text = "5";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true && Convert.ToInt32(textBox3.Text) < 5)
            {
                MessageBox.Show("Времето за извличане не може да е по-малко от 5 секунди");
                checkBox1.Checked = false;
                return;
            }

            if (checkBox1.Checked == true)
            {
                string database = textBox1.Text;
                string catalog = textBox2.Text;
                connectionString = String.Format(@"Data Source={0}; Initial Catalog={1};User ID=sa;Password=massive", database, catalog);

                Thread childThread = new Thread(RunLog);
                childThread.Start();

                isChecked = true;

            }
            else
            {
                isChecked = false;
            }
        }

        private void RunLog()
        {
            for (; ; )
            {
                
                Thread.Sleep(Convert.ToInt32(textBox3.Text) * 1000);

                if (Form1.isChecked)
                {
                    listView1.Items.Clear();

                    Log log = new Log();

                    using (SqlConnection myConnection = new SqlConnection(Form1.connectionString))
                    {
                        string oString = "Select TOP 50 message, date_msg from log_system with(nolock) where message like '%CryptoTrace%' order by id desc";

                        SqlCommand oCmd = new SqlCommand(oString, myConnection);
                        oCmd.CommandTimeout = 1000000;

                        try
                        {
                            myConnection.Open();
                        }
                        catch (SqlException)
                        {
                            checkBox1.Checked = false;
                            return;
                        }
                        
                        using (SqlDataReader oReader = oCmd.ExecuteReader())
                        {
                            while (oReader.Read())
                            {
                                log.message = oReader["message"].ToString();
                                log.date = oReader["date_msg"].ToString();

                                string[] row = { log.message, log.date };

                                ListViewItem item = new ListViewItem(row);

                                listView1.Items.Add(item);
                            }

                            myConnection.Close();
                        }
                       
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }

    public class Log
    {
        public string message;

        public string date;
    }
}