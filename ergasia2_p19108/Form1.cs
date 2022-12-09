using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ergasia2_p19108
{
    public partial class Form1 : Form
    {
        //το connection string για την βάση δεδομένων
        String connectionString = "Data Source=Shapes.db;Version=3;";
        SQLiteConnection conn;
        Graphics g;
        Pen p;
        int x1 = 0;
        int y1 = 0;
        //μεταβλητές boolean που δείχνουν τι σχήμα σχεδιάζει ο χρήστης
        bool flag = false;
        bool freestyle = false;
        bool drawline = false;
        bool circle = false;
        bool rectangle = false;
        bool ellipse = false;
        int circlewidth;
        //string που μεταφέρει τι είδους σχήμα σχεδίασε ο χρήστης στην βάση δεδομένων
        string shape = "freestyle";
        int increment = 0;
        //λίστες που αποθηκεύουν τις συντεταγμένες των σχεδίων που γίνονται μέσα από την εφαρμογή
        List<Point> points1 = new List<Point>();
        List<Point> points2 = new List<Point>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            conn = new SQLiteConnection(connectionString);
            g = this.CreateGraphics();
            p = new Pen(Color.Black);
            comboBox1.SelectedItem = "Pencil";
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (flag == true && freestyle == true)
            {
                //για το freestyle παίρνω συνέχεια την προηγούυμενη τοποθεσία στην οποία βρσικόταν το ποντίκι και την ζωγραφίζω
                //εφόσον ζωγραφίζω γραμμές οι οποίες ενώνουν σημεία που είναι το ένα δίπλα στο άλλο φαίνεται σαν ελεύθερη ζωγραφική
                g.DrawLine(p, x1, y1, e.X, e.Y);
                x1 = e.X;
                y1 = e.Y;
            }
            if (drawline == true && flag == true)
            {
                g.DrawLine(p, x1, y1, e.X, e.Y);
                flag = false;
            }
            if (circle == true && flag == true)
            {
                //για να γίνει κύκλος εφόσον δεν υπάρχει συνάρτηση DrawCircle παίρνουμε την DrawEllipse και βάζουμε ίδιο width και height
                if(e.X - x1 > e.Y - y1)
                {
                    circlewidth = e.X - x1;
                }
                else
                {
                    circlewidth = e.Y - y1;
                }
                g.DrawEllipse(p, Math.Min(e.X, x1), Math.Min(e.Y, y1), circlewidth, circlewidth);
                flag = false;
            }
            if (rectangle == true && flag == true)
            {
                g.DrawRectangle(p, Math.Min(e.X, x1), Math.Min(e.Y, y1), Math.Abs(e.X - x1), Math.Abs(e.Y - y1));
                flag = false;
            }
            if(ellipse == true && flag == true)
            {
                g.DrawEllipse(p, Math.Min(e.X, x1), Math.Min(e.Y, y1), Math.Abs(e.X - x1), Math.Abs(e.Y - y1));
                flag = false;
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            //όσο ο χρήστης κάνει κλικ με το ποντίκι του παίρνουμε τις συντεταγμένες του ποντικιού του
            if (freestyle)
            {
                x1 = e.X;
                y1 = e.Y;
                flag = true;
            }
            if (drawline)
            {
                x1 = e.X;
                y1 = e.Y;
            }
            if (circle)
            {
                x1 = e.X;
                y1 = e.Y;
            }
            if (rectangle)
            {
                x1 = e.X;
                y1 = e.Y;
            }
            if (ellipse)
            {
                x1 = e.X;
                y1 = e.Y;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (freestyle)
            {
                g.DrawLine(p, x1, y1, e.X, e.Y);
                flag = false;
            }
            if (drawline)
            {
                flag = true;
            }
            if (circle)
            {
                flag = true;
            }
            if (rectangle)
            {
                flag = true;
            }
            if (ellipse)
            {
                flag = true;
            }
            //ανεβάζουμε το σχήμα που ζωγράφισε ο χρήστης στη βάση δεδομένων
            shapeTimestamp(shape);
        }

        //κάνουμε true την boolean μεταβλητή που αντιστοιχεί σε κάθε κουμπί ενώ κάνουμε false τις άλλες και μετονομάζουμε και αντιστοιχα το shape
        private void button1_Click(object sender, EventArgs e)
        {
            freestyle = true;
            drawline = false;
            circle = false;
            rectangle = false;
            ellipse = false;
            shape = "freestyle";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            drawline = true;
            freestyle = false;
            circle = false;
            rectangle = false;
            ellipse = false;
            shape = "line";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            circle = true;
            drawline = false;
            freestyle = false;
            rectangle = false;
            ellipse = false;
            shape = "circle";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            rectangle = true;
            drawline = false;
            freestyle = false;
            circle = false;
            ellipse = false;
            shape = "rectangle";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //άμα πατήσει OK τότε το χρώμα αλλάζει σε αυτό που επιλέχθηκε
            if (colorDialog1.ShowDialog() == DialogResult.OK)
                p.Color = colorDialog1.Color;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //αλλάζουμε το width ανάλογα την επιλογή του χρήστη
            if (comboBox1.SelectedItem.ToString().Equals("Pencil"))
            {
                p.Width = 1;
            }
            else if (comboBox1.SelectedItem.ToString().Equals("Paintbrush"))
            {
                p.Width = 3;
            }
            else if (comboBox1.SelectedItem.ToString().Equals("Graffiti"))
            {
                p.Width = 10;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ellipse = true;
            rectangle = false;
            drawline = false;
            freestyle = false;
            circle = false;
            shape = "ellipse";
        }

        void shapeTimestamp(String shape)
        {
            //άμα το shape έιναι σχήμα και όχι freestyle τότε το εισάγουμε στην βάση δεδομένων 
            if (!shape.Equals("freestyle"))
            {
                conn.Open();
                String insertQuery = "Insert into Shapes(shape, timestamp) values('" + shape + "','" + DateTime.Now.ToString() + "')";
                SQLiteCommand cmd = new SQLiteCommand(insertQuery, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void togglePanelVisibilityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //άμα είναι visible το panel το κάνουμε invisible και αντιστροφα
            togglePanelVisibilityToolStripMenuItem.Checked = !togglePanelVisibilityToolStripMenuItem.Checked;
            panel1.Visible = !panel1.Visible;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //εξαφανίζουμε ότι είχε ζωγραφιστεί στην φόρμα
            this.Invalidate();
            //οι συντεταγμένες για να ζωγραφιστεί ένα σπίτι
            points1.Add(new Point(300, 300));
            points1.Add(new Point(400, 200));
            points1.Add(new Point(400, 300));
            points1.Add(new Point(400, 300));
            points1.Add(new Point(250, 200));
            points1.Add(new Point(250, 200));
            points1.Add(new Point(450, 200));
            points1.Add(new Point(333, 250));
            points1.Add(new Point(333, 250));
            points1.Add(new Point(366, 250));
            points2.Add(new Point(300, 200));
            points2.Add(new Point(300, 200));
            points2.Add(new Point(300, 300));
            points2.Add(new Point(400, 200));
            points2.Add(new Point(450, 200));
            points2.Add(new Point(350, 150));
            points2.Add(new Point(350, 150));
            points2.Add(new Point(333, 300));
            points2.Add(new Point(366, 250));
            points2.Add(new Point(366, 300));
            //ενεργοποιούμε τον timer
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //ζωγραφίζουμε γραμμη γραμμή ότι υπαρχει στις λίστες
            g.DrawLine(p, points1[increment], points2[increment]);
            //μεταβαίνουμε στο επόμενο στοιχείο
            increment++;
            //άμα φτάσουμε στο τελευταίο στοιχείο της λίστας κάνουμε το increment πάλι 0, αδειάζουμε τις λίστες και κάνουμε disable τον timer
            if (increment == points1.Count)
            {
                increment = 0;
                points1.Clear();
                points2.Clear();
                timer1.Enabled = false;
            }
        }

        private void exitAppToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.Invalidate();
            //οι συντεταγμένες για να ζωγραφιστεί η φράση "Hi"
            points1.Add(new Point(300, 300));
            points1.Add(new Point(333, 200));
            points1.Add(new Point(333, 200));
            points1.Add(new Point(333, 233));
            points1.Add(new Point(366, 200));
            points1.Add(new Point(366, 200));
            points1.Add(new Point(400, 300));
            points1.Add(new Point(400, 300));
            points1.Add(new Point(366, 266));
            points1.Add(new Point(366, 266));
            points1.Add(new Point(333, 300));
            points1.Add(new Point(300, 300));
            points1.Add(new Point(433, 300));
            points1.Add(new Point(466, 233));
            points1.Add(new Point(466, 233));
            points1.Add(new Point(433, 300));
            points1.Add(new Point(433, 220));
            points1.Add(new Point(466, 200));
            points1.Add(new Point(466, 200));
            points1.Add(new Point(433, 220));

            points2.Add(new Point(300, 200));
            points2.Add(new Point(300, 200));
            points2.Add(new Point(333, 233));
            points2.Add(new Point(366, 233));
            points2.Add(new Point(366, 233));
            points2.Add(new Point(400, 200));
            points2.Add(new Point(400, 200));
            points2.Add(new Point(366, 300));
            points2.Add(new Point(366, 300));
            points2.Add(new Point(333, 266));
            points2.Add(new Point(333, 266));
            points2.Add(new Point(333, 300));
            points2.Add(new Point(433, 233));
            points2.Add(new Point(433, 233));
            points2.Add(new Point(466, 300));
            points2.Add(new Point(466, 300));
            points2.Add(new Point(433, 200));
            points2.Add(new Point(433, 200));
            points2.Add(new Point(466, 220));
            points2.Add(new Point(466, 220));
            timer1.Enabled = true;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.Invalidate();
            //οι συντεταγμένες για να ζωγραφιστεί ένα αστέρι
            points1.Add(new Point(175,200));
            points1.Add(new Point(200,300));
            points1.Add(new Point(250,150));
            points1.Add(new Point(250,150));
            points1.Add(new Point(175, 200));

            points2.Add(new Point(325,200));
            points2.Add(new Point(325,200));
            points2.Add(new Point(200,300));
            points2.Add(new Point(300,300));
            points2.Add(new Point(300,300));

            timer1.Enabled = true;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.Invalidate();
            //οι συντεταγμένες για να ζωγραφιστεί ένα διαμάντι
            points1.Add(new Point(300, 200));
            points1.Add(new Point(300, 200));
            points1.Add(new Point(460, 200));
            points1.Add(new Point(300, 200));
            points1.Add(new Point(440, 175));
            points1.Add(new Point(440, 175));
            points1.Add(new Point(320, 175));
            points1.Add(new Point(440, 175));
            points1.Add(new Point(330, 200));
            points1.Add(new Point(430, 200));

            points2.Add(new Point(460, 200));
            points2.Add(new Point(380, 300));
            points2.Add(new Point(380, 300));
            points2.Add(new Point(320, 175));
            points2.Add(new Point(320, 175));
            points2.Add(new Point(460, 200));
            points2.Add(new Point(380, 300));
            points2.Add(new Point(380, 300));
            points2.Add(new Point(380, 175));
            points2.Add(new Point(380, 175));
            timer1.Enabled = true;
        }

        private void clearDrawingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //εξαφανίζουμε ότι είχε ζωγραφιστεί στην φόρμα
            this.Invalidate();
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //δείχνουμε στον χρήστη με MessageBox ότι υπάρχει μέσα στην Βάση Δεδομένων
            conn.Open();
            String query1 = "Select * from Shapes";
            SQLiteCommand cmd = new SQLiteCommand(query1, conn);
            SQLiteDataReader reader = cmd.ExecuteReader();
            StringBuilder builder = new StringBuilder();
            while (reader.Read())
            {
                builder.Append(reader.GetString(1))
                    .Append(" , ")
                    .Append(reader.GetString(2))
                    .Append(Environment.NewLine);
            }
            MessageBox.Show(builder.ToString(), "Shapes Database");
            conn.Close();
        }
    }
}
