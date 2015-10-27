using System;
using System.Threading;
using System.Windows.Forms;

namespace LinmaluMyDB
{
    public partial class Login : Form
    {
        private Main main;
        private int time;

        public Login(Main main)
        {
            InitializeComponent();
            this.main = main;
            foreach(LinmaluDBType ld in Enum.GetValues(typeof(LinmaluDBType)))
            {
                comboBox1.Items.Add(ld);
            }
            comboBox1.SelectedIndex = 0;
        }

        private delegate void SetRun();

        private void run(LinmaluDBType dt)
        {
            try
            {
                main.db = LinmaluDB.createDB(dt, textBox3.Text, textBox4.Text, textBox1.Text, textBox2.Text);
                SetRun d = () =>
                {
                    Close();
                    main.clear();
                };
                Invoke(d, new object[] { });
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "접속실패");
                if (Created)
                {
                    SetRun d = () =>
                    {
                        textBox1.Enabled = textBox2.Enabled = textBox3.Enabled = textBox4.Enabled = comboBox1.Enabled = button1.Enabled = true;
                        if (Created)
                        {
                            timer1.Stop();
                            Cursor = Cursors.Default;
                            button1.Text = "접속";
                        }
                    };
                    Invoke(d, new object[] { });
                    MessageBox.Show(this, e.ToString(), "접속실패");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Enabled = textBox2.Enabled = textBox3.Enabled = textBox4.Enabled = comboBox1.Enabled = button1.Enabled = false;
            Cursor = Cursors.AppStarting;
            time = 0;
            timer1_Tick(null, null);
            timer1.Start();
            LinmaluDBType dt = (LinmaluDBType)comboBox1.SelectedItem;
            Thread t = new Thread(() => run(dt));
            t.IsBackground = true;
            t.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            button1.Text = "접속중 : " + time + "초";
            time++;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch((LinmaluDBType)comboBox1.SelectedItem)
            {
                case LinmaluDBType.MySQL:
                    textBox4.Text = "3306";
                    break;
                case LinmaluDBType.MsSQL:
                    textBox4.Text = "1433";
                    break;
            }
        }
    }
}
