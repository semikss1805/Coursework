using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tetris
{
    public partial class Form1 : Form
    {
        Shape currentShape;
       int size,
            interval,
            x,
            y;
        int[,] map = new int[20, 10];
        int lineRemoved;
        int score;
        public Form1()
        {
            InitializeComponent();
            Init();
        }
        public void Init()
        {
            size = 25;
            y = 20;
            x = 10;

            score = 0;
            lineRemoved = 0;
            label1.Text = "Score: " + score;
            label2.Text = "Lines : " + lineRemoved;

            currentShape = new Shape(4, 0);
            this.KeyDown += new KeyEventHandler(KeyFunc);

            interval = 400;
            timer1.Interval = interval;
            timer1.Tick += new EventHandler(Update);

            Invalidate();
        }

        private void KeyFunc(object sender, KeyEventArgs e)
        {
            if (timer1.Enabled)
            {
                switch (e.KeyCode)
                {
                    case Keys.Down:
                        Update(this, new EventArgs());
                        break;
                    case Keys.Up:
                        if (!IsOverlapping())
                        {
                            ResetArea();
                            currentShape.RotateShape();
                            Merge();
                            Invalidate();
                        }
                        break;
                    case Keys.Left:
                        if (!CollideHor(-1))
                        {
                            ResetArea();
                            currentShape.MoveLeft();
                            Merge();
                            Invalidate();
                        }
                        break;
                    case Keys.Right:
                        if (!CollideHor(+1))
                        {
                            ResetArea();
                            currentShape.MoveRight();
                            Merge();
                            Invalidate();
                        }
                        break;
                }
            }
        }

        private void Update(object sender, EventArgs e)
        {
            timer1.Interval = interval;
            ResetArea();
            if (!Collide())
            {
                currentShape.MoveDown();
            }
            else
            {
                Merge();
                SliceMap();
                timer1.Interval = 1000;
                currentShape.ResetShape(4,0);

                if (Collide())
                {
                    PauseOrPlay(this, new EventArgs());
                    MessageBox.Show($"       GAMEOVER ( ✖ _ ✖ )\n\tScore: {score}\n\tLines :{lineRemoved}");
                    score = 0;
                    lineRemoved = 0;
                    label1.Text = "Score: " + score;
                    label2.Text = "Lines : " + lineRemoved;
                    timer1.Interval = 400;
                    for (int i = 0; i < y; i++)
                    {
                        for (int j = 0; j < x; j++)
                        {
                            map[i, j] = 0;
                        }
                    }
                }
            }
            Merge();
            Invalidate();
        }
        public void SliceMap()
        {
            int count = 0;
            int curRemovedLines = 0;
            for (int i = 0; i < y; i++)
            {
                count = 0;
                for (int j = 0; j < x; j++)
                {
                    if (map[i, j] != 0)
                        count++;
                }
                if (count == x)
                {
                    curRemovedLines++;
                    for (int k = i; k >= 1; k--)
                    {
                        for (int o = 0; o < x; o++)
                        {
                            map[k, o] = map[k - 1, o];
                        }
                    }
                }
            }
            for (int i = 0; i < curRemovedLines; i++)
            {
                score += (10 * (i + 1));
            }
            if (interval > 100)
            {
                interval -= 4 * curRemovedLines;
            }
            lineRemoved += curRemovedLines;
            label1.Text = "Score: " + score;
            label2.Text = "Lines : " + lineRemoved;
        }
        public bool IsOverlapping()
        {
            for (int i = currentShape.y; i < currentShape.y + currentShape.sizeMatrix; i++)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (j >= 0 && j <= x - 1)
                    {
                        if (i >= 0 && i <= y - 1)
                        {
                            if (map[i, j] != 0 && currentShape.matrix[i - currentShape.y, j - currentShape.x] == 0)
                            {
                                return true;
                            }
                        }
                        if (i == y)
                        {
                            return true;
                        }
                    }
                    if (j == x)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void Merge()
        {
            for (int i = currentShape.y; i < currentShape.y + currentShape.sizeMatrix; i++)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (currentShape.matrix[i - currentShape.y, j - currentShape.x] != 0)
                        map[i, j] = currentShape.matrix[i - currentShape.y, j - currentShape.x];
                }
            }
        }

        public bool Collide()
        {
            for (int i = currentShape.y + currentShape.sizeMatrix - 1; i >= currentShape.y; i--)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (currentShape.matrix[i - currentShape.y, j - currentShape.x] != 0)
                    {
                        if (i + 1 == y)
                            return true;
                        if (map[i + 1, j] != 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public bool CollideHor(int dir)
        {
            for (int i = currentShape.y; i < currentShape.y + currentShape.sizeMatrix; i++)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (currentShape.matrix[i - currentShape.y, j - currentShape.x] != 0)
                    {
                        if (j + 1 * dir > x - 1 || j + 1 * dir < 0)
                            return true;
                        if (map[i, j + 1 * dir] != 0)
                        {
                            if (j - currentShape.x + 1 * dir >= currentShape.sizeMatrix || j - currentShape.x + 1 * dir < 0)
                            {
                                return true;
                            }
                            if (currentShape.matrix[i - currentShape.y, j - currentShape.x + 1 * dir] == 0)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        public void ResetArea()
        {
            for (int i = currentShape.y; i < currentShape.y + currentShape.sizeMatrix; i++)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (i >= 0 && j >= 0 && i < y && j < x)
                    {
                        if (currentShape.matrix[i - currentShape.y, j - currentShape.x] != 0)
                        {
                            map[i, j] = 0;
                        }
                    }
                }
            }
        }
        public void DrawNextShape(Graphics e)
        {
            for (int i = 0; i < currentShape.sizeNextMatrix; i++)
            {
                for (int j = 0; j < currentShape.sizeNextMatrix; j++)
                {
                    if (currentShape.nextMatrix[i,j] == 1)
                    {
                        e.FillRectangle(Brushes.DeepSkyBlue, new Rectangle(350 + j * size + 3, 150 + i * size + 3, size - 3, size - 3));
                    }
                    if (currentShape.nextMatrix[i, j] == 2)
                    {
                        e.FillRectangle(Brushes.LightBlue, new Rectangle(375 + j * size + 3, 150 + i * size + 3, size - 3, size - 3));
                    }
                    if (currentShape.nextMatrix[i, j] == 3)
                    {
                        e.FillRectangle(Brushes.DarkOrange, new Rectangle(375 + j * size + 3, 150 + i * size + 3, size - 3, size - 3));
                    }
                    if (currentShape.nextMatrix[i, j] == 4)
                    {
                        e.FillRectangle(Brushes.BlanchedAlmond, new Rectangle(375 + j * size + 3, 150 + i * size + 3, size - 3, size - 3));
                    }
                    if(currentShape.nextMatrix[i, j] == 5)
                    {
                        e.FillRectangle(Brushes.LightGreen, new Rectangle(375 + j * size + 3, 150 + i * size + 3, size - 3, size - 3));
                    }
                    if (currentShape.nextMatrix[i, j] == 6)
                    {
                        e.FillRectangle(Brushes.Violet, new Rectangle(375 + j * size + 3, 175 + i * size + 3, size - 3, size - 3));
                    }
                    if (currentShape.nextMatrix[i, j] == 7)
                    {
                        e.FillRectangle(Brushes.FloralWhite, new Rectangle(375 + j * size + 3, 150 + i * size + 3, size - 3, size - 3));
                    }
                }
            }
        }
        public void DrawMap(Graphics e)
        {
            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    if (map[i, j] == 1)
                    {
                        e.FillRectangle(Brushes.DeepSkyBlue, new Rectangle(50 + j * size + 3, 50 + i * size + 3, size - 3, size - 3));
                    }
                    if (map[i, j] == 2)
                    {
                        e.FillRectangle(Brushes.LightBlue, new Rectangle(50 + j * size + 3, 50 + i * size + 3, size - 3, size - 3));
                    }
                    if (map[i, j] == 3)
                    {
                        e.FillRectangle(Brushes.DarkOrange, new Rectangle(50 + j * size + 3, 50 + i * size + 3, size - 3, size - 3));
                    }
                    if (map[i, j] == 4)
                    {
                        e.FillRectangle(Brushes.BlanchedAlmond, new Rectangle(50 + j * size + 3, 50 + i * size + 3, size - 3, size - 3));
                    }
                    if (map[i, j] == 5)
                    {
                        e.FillRectangle(Brushes.LightGreen, new Rectangle(50 + j * size + 3, 50 + i * size + 3, size - 3, size - 3));
                    }
                    if (map[i, j] == 6)
                    {
                        e.FillRectangle(Brushes.Violet, new Rectangle(50 + j * size + 3, 50 + i * size + 3, size - 3, size - 3));
                    }
                    if (map[i, j] == 7)
                    {
                        e.FillRectangle(Brushes.FloralWhite, new Rectangle(50 + j * size + 3, 50 + i * size + 3, size - 3, size - 3));
                    }
                }
            }
        }
        public void DrawGridForNextShape(Graphics g)
        {
            for (int i = 0; i <= 4; i++)
            {
                g.DrawLine(Pens.Black, new Point(350, 150 + i * size), new Point(350 + 4 * size, 150 + i * size));
            }
            for (int i = 0; i <= 4; i++)
            {
                g.DrawLine(Pens.Black, new Point(350 + i * size, 150), new Point(350 + i * size, 150 + 4 * size));
            }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    g.FillRectangle(Brushes.Gray, new Rectangle(350 + j * size + 1, 150 + i * size + 1, size - 1, size - 1));
                }
            }
        }
        public void DrawGrid(Graphics g)
        {

            for (int i = 0; i <= y; i++)
            {
                g.DrawLine(Pens.Black, new Point(50, 50 + i * size), new Point(50 + 8 * size, 50 + i * size));
            }
            for (int i = 0; i <= x; i++)
            {
                g.DrawLine(Pens.Black, new Point(50 + i * size, 50), new Point(50 + i * size, 50 + 16 * size));
            }
            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    g.FillRectangle(Brushes.Gray, new Rectangle(50 + j * size + 1, 50 + i * size + 1, size - 1, size - 1));
                }
            }
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            DrawGrid(e.Graphics);
            DrawMap(e.Graphics);
            DrawGridForNextShape(e.Graphics);
            DrawNextShape(e.Graphics);
        }

        public void PauseOrPlay(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                timer1.Enabled = false;
                pauseLabel.Visible = true;
                playButton.Text = "▶";
            }
            else
            {
                pauseLabel.Visible = false;
                timer1.Enabled = true;
                playButton.Text = "▉";
            }
        }
    }
}
