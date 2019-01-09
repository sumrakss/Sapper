using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Miner
{

    public partial class MainWindow : Window
    {
        static int time = 0;
        static int freeCell = 0;  // число клеток незанятых минами
        static bool firstClick = true;  // 
        static int size1 = 9;  // размер поля по горизонтали
        static int size2 = 9;  // размер поля по вертикали
        static int mineLevel = 10;  // количество мин на поле
        static int[,] map = new int[size1, size2];  // карта мин
        static Button[,] buttons = new Button[size1, size2];
        static List<Button> flags = new List<Button>();  // клетки с флагами
        static int[,] cellStatus = new int[size1, size2];  
        static List<Button> visitedCell = new List<Button>(); // список проверенных клеток

        public MainWindow()
        {
            InitializeComponent();


            btnRank.Click += ButtonRankClick;
            btnNew.Click += ButtonNewClick;
            statusPanel.Content = "Мины: " + mineLevel;
            timePanel.Content = $"Время: {time}";
            ChangeLevel(9, 9, 5);
        }

        private void Timer(string value)
        {
            time = 0;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1);
            if (value == "start") timer.Start();
            if (value == "stop") timer.Stop();
        }

        private void NewGame(int m, int n)
        {
            
            firstClick = false;
            freeCell = 0;
            map = new int[size1, size2];
            cellStatus = new int[size1, size2];
            int count = 0; // количество единиц в массиве


            Random random = new Random();
            while (count < mineLevel)
            {
                int i = random.Next(size1);
                int j = random.Next(size2);
                if (map[i, j] == 0)
                {
                    if (i == m && j == n) continue; // первый ход не должен попадать на мину
                    else
                    {
                        map[i, j] = 1;
                        count++;
                    }
                }
            }

            for (int i = 0; i < size1; i++)
                for (int j = 0; j < size2; j++)
                    cellStatus[i, j] = map[i, j] == 1 ? 9 : CountMines(i, j);
        }

        private static bool CellExist(int i, int size) => !(i < 0 || i >= size); // проверяет не находится ли клетка за пределами поля

        private static int CountMines(int i, int j)
        {
            int mine = 0;
            if (CellExist(i - 1, size1) && CellExist(j - 1, size2))
                mine += map[i - 1, j - 1];
            if (CellExist(i - 1, size1))
                mine += map[i - 1, j];
            if (CellExist(i - 1, size1) && CellExist(j + 1, size2))
                mine += map[i - 1, j + 1];
            if (CellExist(j - 1, size2))
                mine += map[i, j - 1];
            if (CellExist(j + 1, size2))
                mine += map[i, j + 1];
            if (CellExist(i + 1, size1) && CellExist(j - 1, size2))
                mine += map[i + 1, j - 1];
            if (CellExist(i + 1, size1))
                mine += map[i + 1, j];
            if (CellExist(i + 1, size1) && CellExist(j + 1, size2))
                mine += map[i + 1, j + 1];
            return mine;
        }

        private void EmptySpaces(int i, int j)
        {
            if (CellExist(i - 1, size1) && CellExist(j - 1, size2))
                CellPainter(i - 1, j - 1);
            if (CellExist(i - 1, size1))
                CellPainter(i - 1, j);
            if (CellExist(i - 1, size1) && CellExist(j + 1, size2))
                CellPainter(i - 1, j + 1);
            if (CellExist(j - 1, size2))
                CellPainter(i, j - 1);
            if (CellExist(i, size1) && CellExist(j, size2))
                CellPainter(i, j);
            if (CellExist(j + 1, size2))
                CellPainter(i, j + 1);
            if (CellExist(i + 1, size1) && CellExist(j - 1, size2))
                CellPainter(i + 1, j - 1);
            if (CellExist(i + 1, size1))
                CellPainter(i + 1, j);
            if (CellExist(i + 1, size1) && CellExist(j + 1, size2))
                CellPainter(i + 1, j + 1);
        }

        private void Test(int i, int j)
        {
            buttons[i, j].Background = Brushes.White;
            if (!visitedCell.Contains(buttons[i, j]) && !flags.Contains(buttons[i, j]))
            {
                freeCell++;
                //timePanel.Content = $"Время: {time}";
                if (cellStatus[i, j] != 0)
                    buttons[i, j].Content = cellStatus[i, j];

                if (freeCell == size1 * size2 - mineLevel)
                {
                    Image("Flag.bmp");
                    Field.IsEnabled = false;  // игровое поле становится неактивным
                    MessageBox.Show("Это успех!");
                    Timer("stop");
                }
            }
            visitedCell.Add(buttons[i, j]);
        }

        private void CellPainter(int i, int j)
        {
            if (!visitedCell.Contains(buttons[i, j]))  // условие не дает открыть клетку с флагом
            {
                switch (cellStatus[i, j])
                {
                    case 0:
                        if (!visitedCell.Contains(buttons[i, j]))
                        {
                            Test(i, j);
                            EmptySpaces(i, j);
                        }
                        break;
                    case 1:
                        buttons[i, j].Foreground = Brushes.Blue;
                        Test(i, j);
                        break;
                    case 2:
                        buttons[i, j].Foreground = Brushes.Green;
                        Test(i, j);
                        break;
                    case 3:
                        buttons[i, j].Foreground = Brushes.Red;
                        Test(i, j);
                        break;
                    case 4:
                        buttons[i, j].Foreground = Brushes.DarkBlue;
                        Test(i, j);
                        break;
                    case 5:
                        buttons[i, j].Foreground = Brushes.DarkRed;
                        Test(i, j);
                        break;
                    case 6:
                        buttons[i, j].Foreground = Brushes.Aquamarine;
                        Test(i, j);
                        break;
                    case 7:
                        buttons[i, j].Foreground = Brushes.Black;
                        Test(i, j);
                        break;
                    case 8:
                        buttons[i, j].Foreground = Brushes.DarkGray;
                        Test(i, j);
                        break;
                }
            }
        }

        private void Image(string img)
        {
            ImageBrush mineImg = new ImageBrush();
            mineImg.ImageSource = new BitmapImage(new Uri(@"\\Mac\Home\Documents\MyMiner\MyMiner\MyMiner\images\" + img));
            for (int x = 0; x < size1; x++)
                for (int y = 0; y < size2; y++)
                {
                    if (flags.Contains(buttons[x, y])) // если флаг установлен неверно
                    {
                        ImageBrush testImg = new ImageBrush();
                        testImg.ImageSource = new BitmapImage(new Uri(@"\\Mac\Home\Documents\MyMiner\MyMiner\MyMiner\images\ErrMine.bmp"));
                        buttons[x, y].Background = testImg;
                    }

                    if (map[x, y] == 1)
                        buttons[x, y].Background = mineImg;
                }
        }

        private void ChangeLevel(int s1, int s2, int mineLvl)
        {
            size1 = s1;
            size2 = s2;
            mineLevel = mineLvl;
            firstClick = true;
            Field.IsEnabled = true;
            buttons = new Button[size1, size2];
            visitedCell = new List<Button>();
            flags = new List<Button>();
            Field.Children.Clear();

            for (int i = 0; i < size1; i++)
            {
                for (int j = 0; j < size2; j++)
                {
                    Button btn = new Button();
                    Field.Children.Add(btn);
                    btn.Style = FindResource("btnStyle") as Style;
                    btn.Click += MouseLeftButtonDown;
                    btn.MouseRightButtonDown += MouseRightButtonDown;
                    btn.Background = SystemColors.ControlLightBrush;
                    Grid.SetRow(btn, i);
                    Grid.SetColumn(btn, j);
                    buttons[i, j] = btn;
                }
            }

            switch (s2)
            {
                case 9:
                    MainW.Height = 293;
                    MainW.Width = 198;
                    Field.Height = 198;
                    Field.Rows = 9;
                    Field.Columns = 9;
                    break;
                case 16:
                    MainW.Height = 448;
                    MainW.Width = 352;
                    Field.Height = 352;
                    Field.Rows = 16;
                    Field.Columns = 16;
                    break;
                case 30:
                    MainW.Height = 448;
                    MainW.Width = 660;
                    Field.Height = 352;
                    Field.Rows = 16;
                    Field.Columns = 30;
                    break;
            }
            statusPanel.Content = "Мины: " + (mineLevel);
            //timePanel.Content = $"Время: {time}";
        }

        private new void MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            // координата кнопки на сетке
            var element = (UIElement)e.Source;
            int i = Grid.GetRow(element);
            int j = Grid.GetColumn(element);

            ImageBrush mineImg = new ImageBrush();
            mineImg.ImageSource = new BitmapImage(new Uri(@"\\Mac\Home\Documents\MyMiner\MyMiner\MyMiner\images\Mine.bmp"));

            Button btn = sender as Button;
            if (firstClick)
            {
                Timer("start");
                time = 0;
                //timePanel.Content = $"Время: {time}";
                NewGame(i, j);
            }

            if (!flags.Contains(btn) && !visitedCell.Contains(btn)) // если на клетке нет флага
            {
                if (cellStatus[i, j] == 9)
                {
                    Image("SMine.bmp");
                    btn.Background = mineImg;
                    Field.IsEnabled = false;  // игровое поле становится неактивным
                    Timer("stop");
                }
                else if (cellStatus[i, j] == 0)
                    EmptySpaces(i, j);
                else
                    CellPainter(i, j);
            }
        }

        private new void MouseRightButtonDown(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            ImageBrush flagImg = new ImageBrush();
            flagImg.ImageSource = new BitmapImage(new Uri(@"\\Mac\Home\Documents\MyMiner\MyMiner\MyMiner\images\Flag.bmp"));

            if (firstClick) // условие позволяет расставить флаги до первого клика
            {
                visitedCell.Add(btn);
                flags.Add(btn);
                btn.Background = flagImg;
            }

            else
            {
                if (flags.Contains(btn))
                {
                    btn.Background = SystemColors.ControlLightBrush;
                    flags.Remove(btn);
                    visitedCell.Remove(btn);
                }
                else
                {
                    if (!visitedCell.Contains(btn)) // условие не дает поставить флаг в открытую клетку
                    {
                        flags.Add(btn);
                        btn.Background = flagImg;
                        visitedCell.Add(btn);
                    }
                }
            }
            //mineLevel -= flags.Count();
            statusPanel.Content = "Мины: " + (mineLevel - flags.Count());
        }

        private void ButtonNewClick(object sender, RoutedEventArgs e)
        {
            foreach (var btn in buttons)
            {
                btn.Background = SystemColors.ControlLightBrush;
                btn.Content = "";
            }
            firstClick = true;
            Field.IsEnabled = true;  // игровое поле становится активным
            freeCell = 0;
            visitedCell = new List<Button>();
            flags = new List<Button>();

            statusPanel.Content = "Мины: " + mineLevel;
            //timePanel.Content = $"Время: {time}";
        }

        private void ButtonRankClick(object sender, RoutedEventArgs e)
        {
            switch (size2)
            {
                case 9:
                    ChangeLevel(16, 16, 40);
                    break;
                case 16:
                    ChangeLevel(16, 30, 100);
                    break;
                case 30:
                    ChangeLevel(9, 9, 10);
                    break;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            time++;
            //timePanel.Content = $"Время: {time}";
        }
    }
}