using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace minesweeper3._0
{


    public partial class MainWindow : Window
    {
        private int gridSize = 10;  // Default to easy
        private int mineCount = 10; // Default to easy
        private int cellsLeftWithNoMines;
        private Button[,] buttons;
        private int[,] board;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            // Reset the number of cells without mines
            cellsLeftWithNoMines = (gridSize * gridSize) - mineCount;
            board = new int[gridSize, gridSize];
            buttons = new Button[gridSize, gridSize];

            // Initialize board
            Random rand = new Random();
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    board[i, j] = 0;
                }
            }

            // Place mines
            for (int i = 0; i < mineCount; i++)
            {
                int x, y;
                do
                {
                    x = rand.Next(gridSize);
                    y = rand.Next(gridSize);
                }
                while (board[x, y] == -1);
                board[x, y] = -1;
                UpdateNumbers(x, y);
            }

            // Create buttons
            GameGrid.Children.Clear();
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    Button btn = new Button
                    {
                        Width = 30,
                        Height = 30,
                        Background = Brushes.LightGray,
                        Tag = new Tuple<int, int>(i, j)
                    };
                    btn.Click += Button_Click;
                    buttons[i, j] = btn;
                    GameGrid.Children.Add(btn);
                    Grid.SetRow(btn, i);
                    Grid.SetColumn(btn, j);
                }
            }
        }

        private void UpdateNumbers(int x, int y)
        {
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i >= 0 && i < gridSize && j >= 0 && j < gridSize && board[i, j] != -1)
                    {
                        board[i, j]++;
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button? btn = sender as Button;
            var pos = (Tuple<int, int>)btn.Tag;
            int x = pos.Item1;
            int y = pos.Item2;

            if (board[x, y] == -1)
            {
                // The player clicked on a mine
                btn.Background = Brushes.Red;
                RevealAllMines();
                MessageBox.Show("Game Over!");
                InitializeGame();
                
            }
            else
            {
                // Reveal the cell content and disable it
                RevealCell(x, y);

            }
        }

        private void RevealCell(int x, int y)
        {
            // Check if the coordinates are valid and if the cell is already revealed
            if (x < 0 || y < 0 || x >= gridSize || y >= gridSize || !buttons[x, y].IsEnabled)
            {
                return;
            }

            // Disable the button so it can't be clicked again
            buttons[x, y].IsEnabled = false;
            buttons[x, y].Background = Brushes.White;

            // decrement cells with no mines 
            cellsLeftWithNoMines--; 

            // Wincondition when cells with no mines hit 0, wincon is reached.
            if (cellsLeftWithNoMines == 0)
            {
                MessageBox.Show("Winner winner chiken dinner");
                InitializeGame();

            }
            // If the cell has adjacent mines, show the number
            if (board[x, y] > 0)
            {
                buttons[x, y].Content = board[x, y].ToString();
            }
            // If the cell is empty (0), recursively reveal adjacent cells
            else if (board[x, y] == 0)
            {
                buttons[x, y].Content = ""; // No mines around, leave the button blank

                // Recursively reveal surrounding cells
                for (int i = x - 1; i <= x + 1; i++)
                {
                    for (int j = y - 1; j <= y + 1; j++)
                    {
                        // Don't reveal the center cell again, only reveal surrounding cells
                        if (i != x || j != y)
                        {
                            RevealCell(i, j);

                        }
                    }
                }
            }
        }

            private void RevealAllMines()
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (board[i, j] == -1)
                    {
                        buttons[i, j].Background = Brushes.Red;
                        buttons[i, j].Content = "*";
                    }
                }
            }
        }


        // throwing Enum to make difficulties easy 

        public enum DifficultyLevel
        {
            Easy,
            Medium,
            Hard
        }


        private void SetDifficulty(DifficultyLevel difficulty)
        {
            switch (difficulty)
            {
                case DifficultyLevel.Easy:
                    gridSize = 8;
                    mineCount = 10;
                    break;
                case DifficultyLevel.Medium:
                    gridSize = 12;
                    mineCount = 20;
                    break;
                case DifficultyLevel.Hard:
                    gridSize = 16;
                    mineCount = 40;
                    break;
            }

            InitializeGame(); 
        }


        private void MenuEasy_Click(object sender, RoutedEventArgs e)
        {
            SetDifficulty(DifficultyLevel.Easy);
        }

        private void MenuMedium_Click(object sender, RoutedEventArgs e)
        {
            SetDifficulty(DifficultyLevel.Medium);
        }

        private void MenuHard_Click(object sender, RoutedEventArgs e)
        {
            SetDifficulty(DifficultyLevel.Hard);
        }
    }
}