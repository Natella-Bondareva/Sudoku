using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku2
{
    public partial class Form1 : Form
    {
        const int n = 3;
        public int[,] map = new int[n * n, n * n];
        public Button[,] buttons = new Button[n * n, n * n];
        const int sizeButton = 50;
        private int selectedNumber = 1;
        private bool pencilMode = false;
        public int lives = 3;

        private void OnNumberButtonClick(int number)
        {
            selectedNumber = number;
        }


        private void SetButtonStyles(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.BackColor = this.BackColor; 
            button.FlatAppearance.BorderColor = Color.Gray; 
            button.FlatAppearance.BorderSize = 1; 
        }
        public Form1()
        {
            InitializeComponent();
            GenerateMap();
            InitializeNumberButtons();
        }

        public void GenerateMap()
        {
            for (int i = 0; i < n * n; i++)
            {
                for (int j = 0; j < n * n; j++)
                {
                    map[i, j] = (i * n + i / n + j) % (n * n) + 1;
                    buttons[i, j] = new Button();
                }
            }

            Random r = new Random();
            for (int i = 0; i < 40; i++)
            {
                ShuffleMap(r.Next(0, 5));
            }

            CreateMap();
            HideCells();
        }
        public void HideCells()
        {
            int N = 40;
            Random r = new Random();
            while (N > 0)
            {
                for (int i = 0; i < n * n; i++)
                {
                    for (int j = 0; j < n * n; j++)
                    {
                        if (!string.IsNullOrEmpty(buttons[i, j].Text))
                        {
                            int a = r.Next(0, 3);
                            buttons[i, j].Text = a == 0 ? "" : buttons[i, j].Text;
                            buttons[i, j].Enabled = a == 0 ? true : false;

                            if (a == 0)
                                N--;
                            if (N <= 0)
                                break;
                        }
                    }
                    if (N <= 0)
                        break;
                }
            }
        }

        public void ShuffleMap(int i)
        {
            switch (i)
            {
                case 0:
                    MatrixTransposition();
                    break;
                case 1:
                    SwapRowsInBlock();
                    break;
                case 2:
                    SwapColumnsInBlock();
                    break;
                case 3:
                    SwapBlocksInRow();
                    break;
                case 4:
                    SwapBlocksInColumn();
                    break;
                default:
                    MatrixTransposition();
                    break;
            }
        }

        public void SwapBlocksInColumn()
        {
            Random r = new Random();
            var block1 = r.Next(0, n);
            var block2 = r.Next(0, n);
            while (block1 == block2)
                block2 = r.Next(0, n);
            block1 *= n;
            block2 *= n;
            for (int i = 0; i < n * n; i++)
            {
                var k = block2;
                for (int j = block1; j < block1 + n; j++)
                {
                    var temp = map[i, j];
                    map[i, j] = map[i, k];
                    map[i, k] = temp;
                    k++;
                }
            }
        }

        public void SwapBlocksInRow()
        {
            Random r = new Random();
            var block1 = r.Next(0, n);
            var block2 = r.Next(0, n);
            while (block1 == block2)
                block2 = r.Next(0, n);
            block1 *= n;
            block2 *= n;
            for (int i = 0; i < n * n; i++)
            {
                var k = block2;
                for (int j = block1; j < block1 + n; j++)
                {
                    var temp = map[j, i];
                    map[j, i] = map[k, i];
                    map[k, i] = temp;
                    k++;
                }
            }
        }

        public void SwapRowsInBlock()
        {
            Random r = new Random();
            var block = r.Next(0, n);
            var row1 = r.Next(0, n);
            var line1 = block * n + row1;
            var row2 = r.Next(0, n);
            while (row1 == row2)
                row2 = r.Next(0, n);
            var line2 = block * n + row2;
            for (int i = 0; i < n * n; i++)
            {
                var temp = map[line1, i];
                map[line1, i] = map[line2, i];
                map[line2, i] = temp;
            }
        }

        public void SwapColumnsInBlock()
        {
            Random r = new Random();
            var block = r.Next(0, n);
            var row1 = r.Next(0, n);
            var line1 = block * n + row1;
            var row2 = r.Next(0, n);
            while (row1 == row2)
                row2 = r.Next(0, n);
            var line2 = block * n + row2;
            for (int i = 0; i < n * n; i++)
            {
                var temp = map[i, line1];
                map[i, line1] = map[i, line2];
                map[i, line2] = temp;
            }
        }

        public void MatrixTransposition()
        {
            int[,] tMap = new int[n * n, n * n];
            for (int i = 0; i < n * n; i++)
            {
                for (int j = 0; j < n * n; j++)
                {
                    tMap[i, j] = map[j, i];
                }
            }
            map = tMap;
        }

        public void CreateMap()
        {
            int blockMargini = 0;
            for (int i = 0; i < n * n; i++)
            {
                int blockMarginj = 0;
                for (int j = 0; j < n * n; j++)
                {
                    Button button = new Button();
                    buttons[i, j] = button;
                    button.Size = new Size(sizeButton, sizeButton);
                    button.Text = map[i, j].ToString();
                    button.Click += OnCellPressed;
                    
                    button.Location = new Point(j * sizeButton + blockMarginj, i * sizeButton + blockMargini);

                    var value2 = (j + 1) % 3 == 0 ? blockMarginj += 5 : blockMarginj += 0;
                    SetButtonStyles(button);
                    this.Controls.Add(button);
                }
                
                var value = (i + 1) % 3 == 0 ? blockMargini += 5 : blockMargini += 0;
            }
        }

        private void OnCellPressed(object sender, EventArgs e)
        {
            Button pressedButton = sender as Button;

            int rowIndex = -1;
            int colIndex = -1;
            for (int i = 0; i < n * n; i++)
            {
                for (int j = 0; j < n * n; j++)
                {
                    if (buttons[i, j] == pressedButton)
                    {
                        rowIndex = i;
                        colIndex = j;
                        break;
                    }
                }
            }
            if (pencilMode)
            {
                string buttonText = pressedButton.Text;

                if (string.IsNullOrWhiteSpace(buttonText))
                {
                    
                    pressedButton.Text = selectedNumber.ToString();
                    pressedButton.ForeColor = Color.DarkCyan;
                }
                else 
                {
                    List<string> numbers = buttonText.Split(' ').ToList();

                    if (numbers.Contains(selectedNumber.ToString()))
                    {
                        numbers.Remove(selectedNumber.ToString());
                    }
                    else if (numbers.Count >= 1)
                    {
                        numbers.Add(selectedNumber.ToString());
                    }

                    pressedButton.Text = string.Join(" ", numbers);
                    pressedButton.ForeColor = Color.DarkCyan;
                }
            }
            else
            {
                pressedButton.Text = selectedNumber.ToString();
                CheckCellValue(rowIndex, colIndex);
            }
        }
        private List<Button> numberButtons;

        private void InitializeNumberButtons()
        {
            numberButtons = new List<Button> { button1, button2, button3, button4, button5, button6, button7, button8, button9 };

            foreach (var button in numberButtons)
            {
                int number = int.Parse(button.Text);
                button.Click += (sender, e) => OnNumberButtonClick(number);
            }
        }

        private void ReductionOfLives(int lives)
        {
            if (lives == 0)
                pictureBox1.Image = Properties.Resources.brokenHeart;
            if (lives == 1)
                pictureBox2.Image = Properties.Resources.brokenHeart;
            if (lives == 2)
                pictureBox3.Image = Properties.Resources.brokenHeart;
        }

        private void CheckCellValue(int row, int col)
        {
            var btnText = buttons[row, col].Text;
            if (btnText != map[row, col].ToString())
            {
                buttons[row, col].BackColor = Color.Red;
                lives--;
                ReductionOfLives(lives);
                if (lives == 0)
                {
                    MessageBox.Show("У Вас не залишилось життів! Ви програли!");
                    RestartTheGame();
                }
            }
            else
            {
                buttons[row, col].BackColor = this.BackColor;
                CheckIsSolutionCorrect();
            }
        }
        private void CheckIsSolutionCorrect()
        {
            for (int i = 0; i < n * n; i++)
            {
                for (int j = 0; j < n * n; j++)
                {
                    var btn = buttons[i, j].Text;
                    if (btn != map[i, j].ToString())
                    {
                        return;
                    }
                }
            }
            MessageBox.Show("Вітаю! Вирішено правильно!");
            RestartTheGame();
        }
        public void RestartTheGame()
        {
            for (int i = 0; i < n * n; i++)
            {
                for (int j = 0; j < n * n; j++)
                {
                    this.Controls.Remove(buttons[i, j]);
                }
            }
            lives = 3;
            pictureBox1.Image = Properties.Resources.Heart;
            pictureBox2.Image = Properties.Resources.Heart;
            pictureBox3.Image = Properties.Resources.Heart;
            GenerateMap();
        }
        private void PenMode_Click(object sender, EventArgs e)
        {
            pencilMode = !pencilMode;
            PenMode.Text = pencilMode ? "Режим: Олівець" : "Режим: Ручка";
        }
    }
}
