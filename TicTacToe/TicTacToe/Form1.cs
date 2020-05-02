using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLua;

namespace TicTacToe
{
    public partial class Form1 : Form
    {
        Dictionary<Point, Control> panels;
        Random rand = new Random();

        EventArgs e = new EventArgs();

        Timer oppositionTimer;
        Timer allianceTimer;

        Timer matchTimer;

        Timer playerAutonTimer;
        Timer enemyAutonTimer;

        Timer autonTimer;

        Lua luaCompiler = new Lua();

        int currentAutonStagePlayer = 0;
        int currentAutonStageEnemy = 0;
        public string newOwner = "Alliance";

        public bool allianceClickable = false;

        public Form1()
        {
            InitializeComponent();
        }

        public void Clicked_Square(object sender, EventArgs e)
        {
            if (allianceClickable || newOwner != "Alliance")
            {
                if(newOwner == "Alliance")
                {
                    allianceClickable = false;
                    allianceTimer.Start();
                }

                if (CheckOverFlow(newOwner))
                {
                    return;
                }

                Tower tower;

                if(sender.GetType() == typeof(Tower))
                {
                    tower = (Tower)sender;
                } else
                {
                    tower = (Tower)((Control)sender).Tag;
                }
                
                tower.AddBall(new Ball(newOwner));
                UpdateField();
            }
        }

        bool CheckOverFlow(string type)
        {
            int count = 0;
            foreach(Point combo in panels.Keys)
            {
                count += ((Tower)panels[combo].Tag).GetCountOfBallsOfType(type);
            }

            if(count < 16)
            {
                return false;
            } else
            {
                return true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            luaCompiler.LoadCLRPackage();

            luaCompiler.DoFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "test.lua"));

            panels = new Dictionary<Point, Control>();
            for(int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    
                    panels.Add(new Point(r, c), tableLayoutPanel1.GetControlFromPosition(c, r));
                    panels[new Point(r, c)].Click += Clicked_Square;
                    Tower tower = new Tower(this);
                    Console.WriteLine(tower.Winner?.ToString());
                    
                    panels[new Point(r, c)].Tag = tower;
                    luaCompiler["tower_" + r + "_" + c] = tower;
                }
            }

            oppositionTimer = new Timer();
            oppositionTimer.Interval = Convert.ToInt32(luaCompiler["enemy_turn_length"]);
            oppositionTimer.Tick += OppositionTimer_Tick;
            //oppositionTimer.Start();

            allianceTimer = new Timer();
            allianceTimer.Interval = Convert.ToInt32(luaCompiler["player_turn_length"]);
            allianceTimer.Tick += AllianceTimer_Tick;

            autonTimer = new Timer();
            autonTimer.Interval = 15000;
            autonTimer.Tick += AutonTimer_Tick;

            matchTimer = new Timer();
            matchTimer.Interval = (60 + 45) * 1000;
            matchTimer.Tick += MatchTimer_Tick;
            //matchTimer.Start();

            UpdateField();


            Action<object, EventArgs> clicked_square = new Action<object, EventArgs>(Clicked_Square);
            Action update_field = new Action(UpdateField);

            luaCompiler["e"] = new EventArgs();
            luaCompiler["clicked_square"] = clicked_square;
            luaCompiler["update_field"] = update_field;

            luaCompiler.DoFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "test.lua"));
            autonTimer.Start();

            RunAutons();


            UpdateField();
        }

        void RunEnemyAutonStage(object sender, EventArgs e)
        {
            LuaFunction function = (LuaFunction)luaCompiler["enemy_auton"];
            LuaFunction function3 = (LuaFunction)luaCompiler["return_auton_times"];
            currentAutonStageEnemy++;

            function.Call(currentAutonStageEnemy);

            Timer t = (Timer)sender;

            t.Interval = Convert.ToInt32(function3.Call(currentAutonStageEnemy, "Opposition")[0]);
            t.Start();
        }

        void RunPlayerAutonStage(object sender, EventArgs e)
        {
            LuaFunction function = (LuaFunction)luaCompiler["player_auton"];
            LuaFunction function3 = (LuaFunction)luaCompiler["return_auton_times"];
            currentAutonStagePlayer++;

            function.Call(currentAutonStagePlayer);

            Timer t = (Timer)sender;
            
            t.Interval = Convert.ToInt32(function3.Call(currentAutonStagePlayer, "Alliance")[0]);
            t.Start();
        }

        void RunAutons()
        {
            LuaFunction function = (LuaFunction)luaCompiler["player_auton"];
            LuaFunction function2 = (LuaFunction)luaCompiler["enemy_auton"];
            LuaFunction function3 = (LuaFunction)luaCompiler["return_auton_times"];

            playerAutonTimer = new Timer
            {
                Interval = Convert.ToInt32(function3.Call(currentAutonStagePlayer, "Alliance")[0])
            };
            playerAutonTimer.Tick += RunPlayerAutonStage;
            playerAutonTimer.Start();

            enemyAutonTimer = new Timer
            {
                Interval = Convert.ToInt32(function3.Call(currentAutonStageEnemy, "Opposition")[0])
            };
            enemyAutonTimer.Tick += RunEnemyAutonStage;
            enemyAutonTimer.Start();
        }

        private void AutonTimer_Tick(object sender, EventArgs e)
        {
            MessageBox.Show("Auton Over");
            autonTimer.Enabled = false;

            enemyAutonTimer.Stop();
            playerAutonTimer.Stop();

            oppositionTimer.Start();
            allianceClickable = true;
            newOwner = "Alliance";
        }

        private void MatchTimer_Tick(object sender, EventArgs e)
        {
            matchTimer.Stop();
            if(int.Parse(label2.Text) > int.Parse(label1.Text))
            {
                MessageBox.Show("Alliance Won!");
            } else
            {
                MessageBox.Show("Opposition Won! >:(");
            }
        }

        private void AllianceTimer_Tick(object sender, EventArgs e)
        {
            allianceClickable = true;
            allianceTimer.Stop();
        }

        private void OppositionTimer_Tick(object sender, EventArgs e)
        {
            newOwner = "Opposition";

            Action<object, EventArgs> clicked_square = new Action<object, EventArgs>(Clicked_Square);

            Func<int, int, object> craft_sender_from_point = new Func<int, int, object>((x, y) => { return panels[new Point(x,y)]; });

            luaCompiler["e"] = new EventArgs();
            luaCompiler["clicked_square"] = clicked_square;
            luaCompiler["craft_sender_from_point"] = craft_sender_from_point;

            foreach (Point combo in panels.Keys)
            {
                luaCompiler["tower"] = (Tower)panels[combo].Tag;
                luaCompiler["newOwner"] = newOwner;

                luaCompiler.DoFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "test.lua"));

                LuaFunction enemy_check_tower_ai = (LuaFunction)luaCompiler["enemy_check_tower_ai"];

                object[] returns = enemy_check_tower_ai.Call();

                if(returns[0].Equals(true))
                {
                    newOwner = "Alliance";
                    return;
                }
            }

            LuaFunction no_non_neutral_or_empty_tower_enemy_ai = (LuaFunction)luaCompiler["no_non_neutral_or_empty_tower_enemy_ai"];

            no_non_neutral_or_empty_tower_enemy_ai.Call();
            newOwner = "Alliance";
        }

        void UpdateField()
        {
            foreach (Point combination in panels.Keys)
            {
                Tower tower = (Tower)panels[combination].Tag;
                panels[combination].Controls.Clear();
                foreach(Ball ball in tower.GetBalls())
                {
                    Button b = new Button
                    {
                        Text = ball.GetOwner(),
                        BackColor = Color.FromKnownColor(KnownColor.Control),
                        Size = new Size(145, 30),
                        Tag = ball
                    };

                    b.Click += Remove_Ball;

                    panels[combination].Controls.Add(b);
                }

                if (tower.Winner != null)
                {
                    switch (tower.Winner)
                    {
                        case "Alliance":
                            panels[combination].BackColor = Color.Blue;
                            break;
                        case "Opposition":
                            panels[combination].BackColor = Color.Red;
                            break;
                        default:
                            panels[combination].BackColor = Color.FromKnownColor(KnownColor.Control);
                            break;
                    }
                }
                else
                {
                    panels[combination].BackColor = Color.FromKnownColor(KnownColor.Control);
                }

            }

            CalculatePoints();
        }

        private void Remove_Ball(object sender, EventArgs e)
        {
            if (!allianceClickable && newOwner != "Opposition")
            {
                return;
            }
            Ball b = ((Ball)((Control)sender).Tag);
            Tower tower = (Tower) ((Control)sender).Parent.Tag;

            if(tower.FirstOrLast(b) == "First")
            {
                tower.RemoveTopBall();
            } else if(tower.FirstOrLast(b) == "Last")
            {
                tower.RemoveBottomBall();
            }

            UpdateField();
        }

        void CalculatePoints()
        {
            int ap = 0;
            int op = 0;
            foreach (Point combo in panels.Keys)
            {
                Tower tower = (Tower)panels[combo].Tag;
                ap += tower.AlliancePoints;
                op += tower.OppositionPoints;
            }


            ap += TicTacToe()[0];
            op += TicTacToe()[1];
            label2.Text = ap.ToString();
            label1.Text = op.ToString();
        }


        private int[] CheckSeries(Point start, Point offset)
        {
            int[] points = new int[2] { 0, 0 };
            Point s = start;
            Point s2 = new Point(s.X + offset.X, s.Y + offset.Y);
            Point s3 = new Point(s2.X + offset.X, s2.Y + offset.Y);

            string r1 = CheckTowerSeries(start, s2, s3);

            switch (r1)
            {
                case "Alliance":
                    points[0] += 6;
                    break;
                case "Opposition":
                    points[1] += 6;
                    break;
                default:
                    break;
            }

            return points;
        }
        private int[] TicTacToe()
        {
            int[] points = new int[2] { 0, 0 };

            int[] p1 = CheckSeries(new Point(0, 0), new Point(0, 1));
            int[] p2 = CheckSeries(new Point(1, 0), new Point(0, 1));
            int[] p3 = CheckSeries(new Point(2, 0), new Point(0, 1));

            int[] p4 = CheckSeries(new Point(0, 0), new Point(1, 0));
            int[] p5 = CheckSeries(new Point(0, 1), new Point(1, 0));
            int[] p6 = CheckSeries(new Point(0, 2), new Point(1, 0));

            int[] p7 = CheckSeries(new Point(0, 0), new Point(1, 1));
            int[] p8 = CheckSeries(new Point(0, 2), new Point(1, -1));

            points[0] += p1[0] + p2[0] + p3[0] + p4[0] + p5[0] + p6[0] + p7[0] + p8[0];
            points[1] += p1[1] + p2[1] + p3[1] + p4[1] + p5[1] + p6[1] + p7[1] + p8[1];
            return points;
        }

        private string CheckTowerSeries(Point t1, Point t2, Point t3)
        {
            Tower tower1 = (Tower)panels[t1].Tag;
            Tower tower2 = (Tower)panels[t2].Tag;
            Tower tower3 = (Tower)panels[t3].Tag;

            if (tower1.Winner == tower2.Winner)
            {
                if(tower3.Winner == tower1.Winner)
                {
                    return tower1.Winner;
                }
            }

            return "";
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            newOwner = "Alliance";
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            newOwner = "Opposition";
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            foreach (Point combo in panels.Keys)
            {
                Tower tower = (Tower)panels[combo].Tag;
                tower.ClearBalls();
            }
            UpdateField();
        }
    }
}
