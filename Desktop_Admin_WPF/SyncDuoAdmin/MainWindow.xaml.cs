using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
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
using Newtonsoft.Json;

namespace SyncDuoAdmin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Users> Users { get; set; } = new ObservableCollection<Users>();
        public ObservableCollection<Achievement> Achievements { get; set; } = new ObservableCollection<Achievement>();
        public ObservableCollection<DailyCR> DailyCRs { get; set; } = new ObservableCollection<DailyCR>();
        public ObservableCollection<Speedrun> Speedruns { get; set; } = new ObservableCollection<Speedrun>();
        public ObservableCollection<Shop> Shops { get; set; } = new ObservableCollection<Shop>();
        public ObservableCollection<Dailys> DailysList { get; set; } = new ObservableCollection<Dailys>();
        public ObservableCollection<Stats> StatsList { get; set; } = new ObservableCollection<Stats>();

        //Maze editor related
        private LevelsData levelsData;
        private int OldLevelIndex;
        private Level EditingLevel;

        public MainWindow()
        {
            InitializeComponent();

            usersDataGrid.ItemsSource = Users;
            userStatsDataGrid.ItemsSource = StatsList;
            userAchievementsDataGrid.ItemsSource = Achievements;
            leaderboardDailycDataGrid.ItemsSource = DailyCRs;
            leaderboardSpeedrunDataGrid.ItemsSource = Speedruns;
            userShopItemDataGrid.ItemsSource = Shops;

            select_table.SelectionChanged += SelectTable_SelectionChanged;

            UpdateColumnListVisibility();

            // Maze editor related - load levels from JSON and display
            LoadBoardsFromJson("levels.json");
            DisplayBoards(BoardsListContainer);

        }

        private readonly SyncDuoApiAdmin api = new SyncDuoApiAdmin();

        #region Tab selection changed handler to load data when a tab is selected
        private async void tabcontrol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            

            if (e.Source is TabControl tabControl)
            {
                TabItem tabItem = tabControl.SelectedItem as TabItem;
                if (tabItem != null)
                {
                    switch (tabItem.Header.ToString())
                    {
                        case "users":
                            //NormalizeJsonData("users");
                            var selectUser = await api.SelectAllUsers();
                            ParseUsers(selectUser);
                            //MessageBox.Show("Select: " + selectUser);
                            //MessageBox.Show("User tab selected");
                            //User_Click(sender, e);
                            break;
                        case "user_stats":
                            var selectUserstat = await api.SelectAllUserStats();
                            ParseStats(selectUserstat);
                            //MessageBox.Show("Select: " + selectUserstat);
                            //await MessageBox.Show(api.SelectAllUsers().ToString());
                            //mazeEditor_Click(sender, e);
                            break;
                        case "user_achievements":
                            var selectUserachievements = await api.SelectAllAchievementRecords();
                            ParseAchievements(selectUserachievements);
                            //exit_Click(sender, e);
                            break;
                        case "user_shop_item":
                            var selectUsershop = await api.SelectAllShopRecords();
                            ParseShops(selectUsershop);
                            //home_Click(sender, e);
                            break;
                        case "leaderboard_speedrun":
                            var selectSpeedrun = await api.SelectAllSpeedrunRecords();
                            ParseSpeedruns(selectSpeedrun);
                            //home_Click(sender, e);
                            break;
                        case "leaderboard_dailyc":
                            var selectDailys = await api.SelectAllDailyCRecords();
                            ParseDailyCR(selectDailys);
                            //home_Click(sender, e);
                            break;
                    }
                }
            }
        }



        // New: handle selection change in the select_table ListBox
        private void SelectTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateColumnListVisibility();
        }

        private void UpdateColumnListVisibility()
        {
            // Hide all column ListBoxes first
            users.Visibility = Visibility.Collapsed;
            leaderboard_dailyc.Visibility = Visibility.Collapsed;
            leaderboard_speedrun.Visibility = Visibility.Collapsed;
            //user_achievements.Visibility = Visibility.Collapsed;
            //user_shop_item.Visibility = Visibility.Collapsed;
            user_stats.Visibility = Visibility.Collapsed;

            // Determine selected table
            var selectedItem = select_table.SelectedItem as ListBoxItem;
            string selected = selectedItem?.Content?.ToString();

            if (string.IsNullOrEmpty(selected))
            {
                // nothing selected - keep defaults
                return;
            }

            switch (selected)
            {
                case "users":
                    users.Visibility = Visibility.Visible;
                    break;
                case "leaderboard_dailyc":
                    leaderboard_dailyc.Visibility = Visibility.Visible;
                    break;
                case "leaderboard_speedrun":
                    leaderboard_speedrun.Visibility = Visibility.Visible;
                    break;
                //case "user_achievements":
                //    user_achievements.Visibility = Visibility.Visible;
                //    break;
                //case "user_shop_item":
                //    user_shop_item.Visibility = Visibility.Visible;
                //    break;
                case "user_stats":
                    user_stats.Visibility = Visibility.Visible;
                    break;
                default:
                    // Unknown selection - do nothing
                    break;
            }
        }

        #endregion

        #region JSON parsing methods for each table
        public void ParseUsers(string json)
        {
            Users.Clear();

            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                JsonElement data = doc.RootElement.GetProperty("data");

                foreach (JsonElement item in data.EnumerateArray())
                {
                    Users.Add(new Users
                    {
                        user_id = item.GetProperty("user_id").GetString(),
                        game_id = item.TryGetProperty("game_id", out var game) ? game.GetString() : null,
                        google_id = item.TryGetProperty("google_id", out var google) ? google.GetString() : null,
                        display_name = item.GetProperty("display_name").GetString(),
                        created_at = item.GetProperty("created_at").GetString(),
                        last_login = item.GetProperty("last_login").GetString(),
                        status = item.GetProperty("status").GetInt32()
                    });
                }
            }
        }
        

        // ACHIEVEMENT
        public void ParseAchievements(string json)
        {
            Achievements.Clear();

            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                JsonElement data = doc.RootElement.GetProperty("data");

                foreach (JsonElement item in data.EnumerateArray())
                {
                    Achievements.Add(new Achievement
                    {
                        user_id = item.GetProperty("user_id").GetString(),
                        achievement_id = item.GetProperty("achievement_id").GetInt32(),
                        status = item.GetProperty("status").GetInt32()
                    });
                }
            }
        }


        // DAILY CR
        public void ParseDailyCR(string json)
        {
            DailyCRs.Clear();

            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                JsonElement data = doc.RootElement.GetProperty("data");

                foreach (JsonElement item in data.EnumerateArray())
                {
                    DailyCRs.Add(new DailyCR
                    {
                        user_id = item.GetProperty("user_id").GetString(),
                        dailyc_time = (float)item.GetProperty("dailyc_time").GetDouble(),
                        date = item.GetProperty("date").GetString()
                    });
                }
            }
        }


        // SPEEDRUN
        public void ParseSpeedruns(string json)
        {
            Speedruns.Clear();

            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                JsonElement data = doc.RootElement.GetProperty("data");

                foreach (JsonElement item in data.EnumerateArray())
                {
                    Speedruns.Add(new Speedrun
                    {
                        user_id = item.GetProperty("user_id").GetString(),
                        speedrun_amount = item.GetProperty("speedrun_amount").GetInt32(),
                        date = item.GetProperty("date").GetString()
                    });
                }
            }
        }


        // SHOP
        public void ParseShops(string json)
        {
            Shops.Clear();

            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                JsonElement data = doc.RootElement.GetProperty("data");

                foreach (JsonElement item in data.EnumerateArray())
                {
                    Shops.Add(new Shop
                    {
                        user_id = item.GetProperty("user_id").GetString(),
                        item_number = item.GetProperty("item_number").GetInt32()
                    });
                }
            }
        }


        // DAILYS
        public void ParseDailys(string json)
        {
            DailysList.Clear();

            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                JsonElement data = doc.RootElement.GetProperty("data");

                foreach (JsonElement item in data.EnumerateArray())
                {
                    DailysList.Add(new Dailys
                    {
                        dailyc_id = item.GetProperty("dailyc_id").GetString(),
                        data_json = item.GetProperty("data_json").GetString()
                    });
                }
            }
        }


        // STATS
        public void ParseStats(string json)
        {
            StatsList.Clear();

            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                JsonElement data = doc.RootElement.GetProperty("data");

                foreach (JsonElement item in data.EnumerateArray())
                {
                    StatsList.Add(new Stats
                    {
                        user_id = item.GetProperty("user_id").GetString(),
                        coins = item.GetProperty("coins").GetInt32(),
                        best_speedrun_amount = item.GetProperty("best_speedrun_amount").GetInt32(),
                        best_dailyc_time = item.GetProperty("best_dailyc_time").GetDouble(),
                        levels_completed = item.GetProperty("levels_completed").GetInt32()
                    });
                }
            }
        }
        #endregion

        #region click handlers for main menu buttons
        private void User_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("User tab selected");
        }

        private void AdminPage_Click(object sender, RoutedEventArgs e)
        {
            HomeInvisible();
            tabControl.Visibility = Visibility.Visible;
        }

        private void mazeEditor_Click(object sender, RoutedEventArgs e)
        {
            HomeInvisible();
            MyTabControl.Visibility = Visibility.Visible;
            footer.Visibility = Visibility.Visible;
            home.Visibility = Visibility.Visible;
            Submit_Button.Visibility = Visibility.Visible;

        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        private void home_Click(object sender, RoutedEventArgs e)
        {
            HomeVisible();
        }

        private void HomeInvisible()
        {
            dataViewer.Visibility = Visibility.Collapsed;
            mazeEditor.Visibility = Visibility.Collapsed;
            exit.Visibility = Visibility.Collapsed;
            mazeEditor.Visibility = Visibility.Collapsed;
            home.Visibility = Visibility.Visible;
            //reload.Visibility = Visibility.Visible;
            footer.Visibility = Visibility.Visible;     
            editor.Visibility = Visibility.Collapsed;
        }

        private void HomeVisible()
        {
            dataViewer.Visibility = Visibility.Visible;
            mazeEditor.Visibility = Visibility.Visible;
            exit.Visibility = Visibility.Visible;
            home.Visibility = Visibility.Collapsed;
            //reload.Visibility = Visibility.Collapsed;
            footer.Visibility = Visibility.Collapsed;
            tabControl.Visibility = Visibility.Collapsed;
            enterUID.Visibility = Visibility.Collapsed;
            enterV.Visibility = Visibility.Collapsed;
            selectC.Visibility = Visibility.Collapsed;
            selectT.Visibility = Visibility.Collapsed;
            EUID.Visibility = Visibility.Collapsed;
            select_table.Visibility = Visibility.Collapsed;
            users.Visibility = Visibility.Collapsed;
            leaderboard_dailyc.Visibility = Visibility.Collapsed;
            leaderboard_speedrun.Visibility = Visibility.Collapsed;
            //user_achievements.Visibility = Visibility.Collapsed;
            //user_shop_item.Visibility = Visibility.Collapsed;
            user_stats.Visibility = Visibility.Collapsed;
            apply.Visibility = Visibility.Collapsed;
            home.Visibility = Visibility.Collapsed;
            footer.Visibility = Visibility.Collapsed;
            editor.Visibility = Visibility.Visible;
            value.Visibility = Visibility.Collapsed;
            MyTabControl.Visibility = Visibility.Collapsed;
            Submit_Button.Visibility = Visibility.Collapsed;

        }

        private void editor_Click(object sender, RoutedEventArgs e)
        {
            enterUID.Visibility = Visibility.Visible;
            enterV.Visibility = Visibility.Visible;
            selectC.Visibility = Visibility.Visible;
            selectT.Visibility = Visibility.Visible;
            EUID.Visibility = Visibility.Visible;
            select_table.Visibility = Visibility.Visible;
            users.Visibility = Visibility.Visible;
            home.Visibility = Visibility.Visible;
            apply.Visibility = Visibility.Visible;
            footer.Visibility = Visibility.Visible;
            value.Visibility = Visibility.Visible;
            editor.Visibility = Visibility.Collapsed;
            dataViewer.Visibility = Visibility.Collapsed;
            mazeEditor.Visibility = Visibility.Collapsed;
            exit.Visibility = Visibility.Collapsed;
        }

        private async void apply_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = select_table.SelectedItem as ListBoxItem;
            string selected = selectedItem?.Content?.ToString();

            string userId = EUID.Text;
            string newValue = value.Text;

            var selectedItemUsers = users.SelectedItem as ListBoxItem;
            string UsersItem = selectedItemUsers?.Content?.ToString();
            var selectedItemLeaderboardDailyC = leaderboard_dailyc.SelectedItem as ListBoxItem;
            string LeaderboardDailyCItem = selectedItemLeaderboardDailyC?.Content?.ToString();
            var selectedItemLeaderboardSpeedrun = leaderboard_speedrun.SelectedItem as ListBoxItem;
            string LeaderboardSpeedrunItem = selectedItemLeaderboardSpeedrun?.Content?.ToString();
            var selectedItemUserStats = user_stats.SelectedItem as ListBoxItem;
            string UserStatsItem = selectedItemUserStats?.Content?.ToString();

            var apiCommand = "";

            switch (selected)
            {
                // ========================= USERS =========================
                case "users":
                    switch (UsersItem)
                    {
                        case "user_id":
                            await api.UpdateUser(userId, new { user_id = newValue });
                            break;

                        case "game_id":
                            await api.UpdateUser(userId, new { game_id = newValue });
                            break;

                        case "google_id":
                            await api.UpdateUser(userId, new { google_id = newValue });
                            break;

                        case "display_name":
                            apiCommand = await api.UpdateUser(userId, new { display_name = newValue });
                            MessageBox.Show(apiCommand);
                            break;

                        case "created_at":
                            await api.UpdateUser(userId, new { created_at = newValue });
                            break;

                        case "last_login":
                            await api.UpdateUser(userId, new { last_login = newValue });
                            break;

                        case "status":
                            await api.UpdateUser(userId, new { status = newValue });
                            break;
                    }

                    MessageBox.Show($"USERS → user_id: {userId}, column: {UsersItem}, new value: {newValue}");
                    break;

                // ========================= LEADERBOARD DAILY CHALLENGE =========================
                case "leaderboard_dailyc":
                    switch (LeaderboardDailyCItem)
                    {
                        case "dailyc_time":
                            apiCommand = await api.UpdateDailyCRecord(userId, new { dailyc_time = newValue });
                            MessageBox.Show(apiCommand);
                            break;

                        case "date":
                            await api.UpdateDailyCRecord(userId, new { date = newValue });
                            break;
                    }

                    MessageBox.Show($"LEADERBOARD_DAILYC → user_id: {userId}, column: {UsersItem}, new value: {newValue}");
                    break;

                // ========================= LEADERBOARD SPEEDRUN =========================
                case "leaderboard_speedrun":
                    switch (LeaderboardSpeedrunItem)
                    {
                        case "speedrun_amount":
                            apiCommand = await api.UpdateSpeedrunRecord(userId, new { speedrun_amount = newValue });
                            MessageBox.Show(apiCommand);
                            break;

                        case "date":
                            await api.UpdateSpeedrunRecord(userId, new { date = newValue });
                            break;
                    }

                    MessageBox.Show($"LEADERBOARD_SPEEDRUN → user_id: {userId}, column: {UsersItem}, new value: {newValue}");
                    break;

                // ========================= USER STATS =========================
                case "user_stats":
                    switch (UserStatsItem)
                    {
                        case "coins":
                            MessageBox.Show("Selected column: coins");
                            apiCommand = await api.UpdateUserStat(userId, new { coins = Int32.Parse(newValue) });
                            MessageBox.Show(apiCommand);
                            break;

                        case "best_speedrun_amount":
                            await api.UpdateUserStat(userId, new { best_speedrun_amount = newValue });
                            break;

                        case "best_dailyc_time":
                            await api.UpdateUserStat(userId, new { best_dailyc_time = newValue });
                            break;

                        case "levels_completed":
                            await api.UpdateUserStat(userId, new { levels_completed = newValue });
                            break;
                    }

                    MessageBox.Show($"USER_STATS → user_id: {userId}, column: {UsersItem}, new value: {newValue}");
                    break;

                default:
                    MessageBox.Show("Ismeretlen tábla lett kiválasztva.");
                    break;
            }
        }
        #endregion

        #region Maze editor related methods
        public void LoadBoardsFromJson(string filename)
        {
            try
            {
                if (!File.Exists(filename))
                {
                    MessageBox.Show($"File '{filename}' not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string levelsJsonContent = File.ReadAllText(filename);
                levelsData = JsonConvert.DeserializeObject<LevelsData>(levelsJsonContent);

                if (levelsData != null && levelsData.levels != null)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading boards from JSON: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisplayBoards(StackPanel container)
        {
            container.Children.Clear();

            foreach (var level in levelsData.levels)
            {
                StackPanel BoardsListItem = new StackPanel
                {
                    Width = this.Width - 40,
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(),
                };
                StackPanel mazeA = new StackPanel
                {
                    Height = 100,
                };
                StackPanel mazeB = new StackPanel
                {
                    Height = 100,
                };
                GenerateComplexBoard(level, mazeA, mazeB, (int)mazeA.Height);
                BoardsListItem.Children.Add(mazeA);
                BoardsListItem.Children.Add(mazeB);
                TextBlock details = new TextBlock
                {
                    Text = level.levelName + "     |      " + level.width + "*" + level.height + "      |      EnemiesA: " + level.enemyPositionsA.Count() + "; EnemiesB: " + level.enemyPositionsB.Count(),
                    Margin = new Thickness(5),
                    Padding = new Thickness(10),
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 14,
                };

                BoardsListItem.Children.Add(details);
                Button editLevelButton = new Button
                {
                    Content = "Edit",
                    Margin = new Thickness(5),
                    Padding = new Thickness(10),
                    Height = 40,
                };
                editLevelButton.Click += (sender, e) => LoadLeveltoEdit(level);
                BoardsListItem.Children.Add(editLevelButton);
                container.Children.Add(BoardsListItem);
            }
        }

        private void LoadLeveltoEdit(Level level)
        {
            MyTabControl.SelectedIndex = 1;
            OldLevelIndex = levelsData.levels.IndexOf(level);
            EditingLevel = level.Clone();
            LoadLevelDetails(EditingLevel);
        }
        private void LoadLevelDetails(Level level)
        {
            BoardID_Input.Text = level.levelName.Split('_')[1];
            BoardSize_Input.Text = level.width.ToString();
            StartPosAX_Input.Text = level.startPosA.x.ToString();
            StartPosAY_Input.Text = level.startPosA.y.ToString();
            StartPosBX_Input.Text = level.startPosB.x.ToString();
            StartPosBY_Input.Text = level.startPosB.y.ToString();
            FinishPosAX_Input.Text = level.finishPosA.x.ToString();
            FinishPosAY_Input.Text = level.finishPosA.y.ToString();
            FinishPosBX_Input.Text = level.finishPosB.x.ToString();
            FinishPosBY_Input.Text = level.finishPosB.y.ToString();
            EnemyPosAList_Input.Text = string.Join("", level.enemyPositionsA.Select(pos => $"({pos.x};{pos.y})"));
            EnemyPosBList_Input.Text = string.Join("", level.enemyPositionsB.Select(pos => $"({pos.x};{pos.y})"));
            MazeLayoutA_Input.Text = "";
            MazeLayoutB_Input.Text = "";
            for (int i = 0; i < level.width * level.height; i++)
            {
                MazeLayoutA_Input.Text += level.mazeLayoutA[i].ToString() + ";";
                MazeLayoutB_Input.Text += level.mazeLayoutB[i].ToString() + ";";
                if (i % level.width == level.width - 1)
                {
                    MazeLayoutA_Input.Text += "\n";
                    MazeLayoutB_Input.Text += "\n";
                }
            }


            GenerateComplexBoard(level, BoardAContainer, BoardBContainer, (int)BoardAContainer.ActualHeight);
        }
        private void GenerateComplexBoard(Level level, StackPanel boardAContainer, StackPanel boardBContainer, int containerHeight)
        {
            Debug.WriteLine($"Generating boards for level: {level.levelName}");
            GenerateSingleBoard(level.width, level.height, level.mazeLayoutA, level.startPosA, level.finishPosA, level.enemyPositionsA, boardAContainer, containerHeight);
            GenerateSingleBoard(level.width, level.height, level.mazeLayoutB, level.startPosB, level.finishPosB, level.enemyPositionsB, boardBContainer, containerHeight);

        }
        private void GenerateSingleBoard(int width, int height, List<int> mazeLayout, Position startPos, Position finishPos, List<Position> enemyPositions, StackPanel boardContainer, int containerHeight)
        {
            boardContainer.Children.Clear();
            for (int y = 0; y < height; y++)
            {
                StackPanel row = new StackPanel { Orientation = Orientation.Horizontal };
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    int cellsize = (int)containerHeight / height;
                    var cellcolor = Brushes.Green;
                    if (mazeLayout[index] == 0)
                    {
                        cellcolor = Brushes.LightGray;
                    }
                    Border cell = new Border
                    {
                        Width = cellsize,
                        Height = cellsize,
                        Background = cellcolor,
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(0.5)
                    };
                    if (startPos.x == x && startPos.y == y)
                    {
                        cell.Background = Brushes.LightSkyBlue;
                    }
                    else if (finishPos.x == x && finishPos.y == y)
                    {
                        cell.Background = Brushes.White;

                    }
                    else if (enemyPositions.Any(pos => pos.x == x && pos.y == y))
                    {
                        cell.Background = Brushes.Red;
                    }
                    row.Children.Add(cell);
                }

                boardContainer.Children.Add(row);
            }
        }
        private bool isNumber(string input)
        {
            int result;
            return int.TryParse(input, out result);
        }
        private void BoardID_Input_LostFocus(object sender, RoutedEventArgs e)
        {
            string input = BoardID_Input.Text;
            if (isNumber(input) && int.Parse(input) > 0)
            {
                EditingLevel.levelName = "Level_" + BoardID_Input.Text;
            }
            else
            {
                BoardID_Input.Text = levelsData.levels[OldLevelIndex].levelName.Split('_')[1];
            }

        }

        private void BoardSize_Input_LostFocus(object sender, RoutedEventArgs e)
        {
            string input = BoardSize_Input.Text;
            if (isNumber(input) && int.Parse(input) > 5)
            {
                int newSize = int.Parse(input);
                int oldSize = EditingLevel.height;
                EditingLevel.width = newSize;
                EditingLevel.height = newSize;
                EditingLevel.mazeLayoutA = ExtendMazeLayout(EditingLevel.mazeLayoutA, newSize, oldSize);
                EditingLevel.mazeLayoutB = ExtendMazeLayout(EditingLevel.mazeLayoutB, newSize, oldSize);
                EditingLevel.startPosA = RepositionEntity(newSize, EditingLevel.mazeLayoutA, EditingLevel.startPosA);
                EditingLevel.startPosB = RepositionEntity(newSize, EditingLevel.mazeLayoutB, EditingLevel.startPosB);
                EditingLevel.finishPosA = RepositionEntity(newSize, EditingLevel.mazeLayoutA, EditingLevel.finishPosA);
                EditingLevel.finishPosB = RepositionEntity(newSize, EditingLevel.mazeLayoutB, EditingLevel.finishPosB);
                for (int i = 0; i < EditingLevel.enemyPositionsA.Count(); i++)
                {
                    EditingLevel.enemyPositionsA[i] = RepositionEntity(newSize, EditingLevel.mazeLayoutA, EditingLevel.enemyPositionsA[i]);

                }
                for (int i = 0; i < EditingLevel.enemyPositionsB.Count(); i++)
                {
                    EditingLevel.enemyPositionsB[i] = RepositionEntity(newSize, EditingLevel.mazeLayoutB, EditingLevel.enemyPositionsB[i]);

                }
                LoadLevelDetails(EditingLevel);
            }
            else
            {
                BoardSize_Input.Text = levelsData.levels[OldLevelIndex].width.ToString();
            }

        }
        private Position RepositionEntity(int newSize, List<int> layout, Position oldPosition)
        {
            Position newPos = new Position { x = Math.Min(oldPosition.x, newSize - 2), y = Math.Min(oldPosition.y, newSize - 2) };
            layout[newPos.y * newSize + newPos.x] = 0;

            return newPos;
        }
        private List<int> ExtendMazeLayout(List<int> Layout, int newSize, int oldSize)
        {
            List<List<int>> newMazeLayoutA = new List<List<int>>();//készítünk egy új layout-ot, és az feltöltjük teljesen 1-esekkel.

            for (int i = 0; i < newSize; i++)
            {
                newMazeLayoutA.Add(new List<int>());
                for (int j = 0; j < newSize; j++)
                {

                    newMazeLayoutA[i].Add(1);
                }
            }
            int smalerSize = Math.Min(newSize, oldSize);
            for (int i = 0; i < smalerSize - 1; i++)
            {
                for (int j = 0; j < smalerSize - 1; j++)
                {
                    newMazeLayoutA[i][j] = Layout[i * oldSize + j];

                }

            }
            return newMazeLayoutA.SelectMany(row => row).ToList();
        }
        private void StartPosAX_Input_LostFocus(object sender, RoutedEventArgs e)
        {
            string input = StartPosAX_Input.Text;
            if (isNumber(input) && int.Parse(input) > 0 && int.Parse(input) < EditingLevel.width - 1)
            {
                int newX = int.Parse(input);
            }
        }

        private void StartPosAY_Input_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void StartPosBX_Input_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void StartPosBY_Input_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void FinishPosAX_Input_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void FinishPosAY_Input_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void FinishPosBX_Input_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void FinishPosBY_Input_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void EnemyPosAList_Input_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void EnemyPosBList_Input_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void MazeLayoutA_Input_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void MazeLayoutB_Input_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }
}
