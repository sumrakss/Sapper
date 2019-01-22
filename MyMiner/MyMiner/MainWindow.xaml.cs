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
        static int size1;  // размер поля по горизонтали
        static int size2;  // размер поля по вертикали
        static int mineLevel;  // количество мин на поле
        static int[,] map;  // карта мин
        static int[,] cellStatus;
        static List<Button> flags;  // клетки с флагами
        static List<Button> visitedCell; // список проверенных клеток
        static Button[,] buttons;
        private static int freeCell = 0;  // число клеток не занятых минами
        static bool firstClick;  // 
        static int time;
        private DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            btnRank.Click += ButtonRankClick;
            btnNew.Click += ButtonNewClick;
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = TimeSpan.FromSeconds(1);
            statusPanel.Content = $"Мины: {10}";
            timePanel.Content = $"Время: {0}";
            ChangeLevel(9, 9, 10); // уровень сложности по-умолчанию
        }

        private void NewGame(int m, int n)
        {
            Random random = new Random();
            int count = 0; // количество единиц в массиве
            while (count < mineLevel) // добавляем в map единицы случайным образом по количеству mineLevel
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
            // 0-8 количество мин в соседних клетках; 9 - в клетке мина
            for (int i = 0; i < size1; i++)
            {
                for (int j = 0; j < size2; j++)
                    cellStatus[i, j] = map[i, j] == 1 ? 9 : CountMines(i, j);
            }
        }

        // проверяет не находится ли клетка за пределами поля
        private static bool CellExist(int i, int size) => !(i < 0 || i >= size); 

        // считает мины вокруг клетки
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
                if (cellStatus[i, j] != 0)
                    buttons[i, j].Content = cellStatus[i, j];

                if (freeCell == size1 * size2 - mineLevel)
                {
                    timer.Stop();
                    Image("Flag.bmp");
                    Field.IsEnabled = false;  // игровое поле становится неактивным
                    MessageBox.Show("Это успех!");
                }
            }
            visitedCell.Add(buttons[i, j]);
        }

        private void CellPainter(int i, int j)
        {
            if (!visitedCell.Contains(buttons[i, j]))  // условие не позволяет открыть клетку с флагом
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

        // в случае выйгрыша на местах мин должны стоять изображение флагов
        // в случе пройгрыша - изображение мин
        private void Image(string img)
        {
            ImageBrush cellImage = new ImageBrush();
            cellImage.ImageSource = new BitmapImage(new Uri("Images/" + img, UriKind.Relative));
            for (int x = 0; x < size1; x++)
                for (int y = 0; y < size2; y++)
                {
                    if (flags.Contains(buttons[x, y])) // если флаг установлен неверно
                    {
                        ImageBrush testImg = new ImageBrush();
                        testImg.ImageSource = new BitmapImage(new Uri("Images/ErrMine.bmp", UriKind.Relative));
                        buttons[x, y].Background = testImg;
                    }

                    if (map[x, y] == 1)
                        buttons[x, y].Background = cellImage;
                }
        }

        // s1 - высота поля; s2 - ширина поля; mineLvl - количество мин на поле 
        private void ChangeLevel(int s1, int s2, int mineLvl)
        {
            timer.Stop();
            size1 = s1;
            size2 = s2;
            mineLevel = mineLvl;
            map = new int[size1, size2];
            cellStatus = new int[size1, size2];
            flags = new List<Button>();
            visitedCell = new List<Button>();
            buttons = new Button[size1, size2];
            freeCell = 0;
            firstClick = true;
            time = 0;
            Field.IsEnabled = true;
            Field.Children.Clear();  // очистить поле
            // заполнить поле кнопками в соответсвии с размером поля
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

            ResizeWindow(s2);
            statusPanel.Content = $"Мины: {mineLevel}";
            timePanel.Content = $"Время: {time}";
        }

        private void ResizeWindow(int size)
        {
            switch (size)
            {
                case 9:
                    MainW.SizeToContent = SizeToContent.WidthAndHeight; // размер окна под содержимое
                    Field.Height = 207;
                    Field.Width = 207;
                    Field.Rows = 9;
                    Field.Columns = 9;
                    break;
                case 16:
                    MainW.SizeToContent = SizeToContent.WidthAndHeight;
                    Field.Height = 368;
                    Field.Width = 368;
                    Field.Rows = 16;
                    Field.Columns = 16;
                    break;
                case 30:
                    MainW.SizeToContent = SizeToContent.WidthAndHeight;
                    Field.Height = 368;
                    Field.Width = 690;
                    Field.Rows = 16;
                    Field.Columns = 30;
                    break;
            }
        }

        private new void MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            // координата кнопки на сетке
            var element = (UIElement)e.Source;
            int i = Grid.GetRow(element);
            int j = Grid.GetColumn(element);

            ImageBrush mineImg = new ImageBrush();
            mineImg.ImageSource = new BitmapImage(new Uri("Images/Mine.bmp", UriKind.Relative));

            Button btn = sender as Button;
            if (firstClick)
            {
                firstClick = false;
                timer.Start();
                NewGame(i, j);
            }

            if (!flags.Contains(btn) && !visitedCell.Contains(btn)) // если на клетке нет флага
            {
                if (cellStatus[i, j] == 9)
                {
                    timer.Stop();
                    Image("SMine.bmp");
                    btn.Background = mineImg;
                    Field.IsEnabled = false;  // игровое поле становится неактивным
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
            flagImg.ImageSource = new BitmapImage(new Uri("Images/Flag.bmp", UriKind.Relative));
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
            statusPanel.Content = $"Мины: {mineLevel - flags.Count()}";
        }

        private void ButtonNewClick(object sender, RoutedEventArgs e)
        {
            ChangeLevel(size1, size2, mineLevel);
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
            timePanel.Content = $"Время: {time}";
        }
    }
}
