using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SpaceWars
{
    public partial class Game : Page
    {
        private GameDetails details = new GameDetails();
        private List<Enemy> enemies = new List<Enemy>();
        private List <Border> bullets = new List<Border>();
        private Random random = new Random();
        private DispatcherTimer gameTimer = new DispatcherTimer();
        private DispatcherTimer regen = new DispatcherTimer();
        private DispatcherTimer shooterTimer = new DispatcherTimer();
        private DispatcherTimer progressTimer = new DispatcherTimer();
        private bool left = false;
        private bool right = false;


        public Game()
        {
            InitializeComponent();
            gameTimer.Interval = TimeSpan.FromMilliseconds(24);
            gameTimer.Tick += new EventHandler(GameTick);
            gameTimer.Start();
            shooterTimer.Interval = TimeSpan.FromMilliseconds(1000);
            shooterTimer.Tick += new EventHandler(ShootTick);
            shooterTimer.Start();
            regen.Interval = TimeSpan.FromMilliseconds(3000);
            regen.Tick += new EventHandler(regenhp);
            regen.Start();
            progressTimer.Interval = TimeSpan.FromMilliseconds(300);
            progressTimer.Tick += new EventHandler(progressTick);
            progressTimer.Start();
            this.PreviewKeyDown += Grid_KeyDown;
            this.PreviewKeyUp += Grid_KeyUp;
            this.Focusable = true;
            this.Focus();
        }



        //----------------------------------------- HP


        private void regenhp(object? sender, EventArgs e)
        {
            if((details.ActualHP + details.Hpregen) <= details.Maxhp)
            {
                details.ActualHP += details.Hpregen;
            }
            else
            {
                details.ActualHP = details.Maxhp;
            }

        }





        //------------------------------- Egyéb metódusok
        private void GameTick(object? sender, EventArgs e)
        {
            if(left)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player)-details.Playerspeed);
            }
            if (right)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + details.Playerspeed);
            }
            generateEnemy();
            moveEnemy();
            moveBullet();
            hitBullet();
            visualize();

        }

        private void progressTick(object? sender, EventArgs e)
        {
            checkObjects();
            progress();
        }
        private void progress()
        {
            if(details.Score == details.Progressed )
            {
                details.Enemyspeed += 0.1;
                details.Progressed *= 2;
                details.Maxenemys += 1;
            }


            if (details.Money < details.Upgradecosts[0])
            {
                Horizontalspeedbtn.IsEnabled = false;
            }
            else
            {
                Horizontalspeedbtn.IsEnabled = true;
            }
            if (details.Money < details.Upgradecosts[1])
            {
                Income.IsEnabled = false;
            }
            else
            {
                Income.IsEnabled = true;
            }
            if (details.Money < details.Upgradecosts[2] && details.Firerate > 500)
            {
                Firerate.IsEnabled = false;
            }
            else
            {
                Firerate.IsEnabled = true;
            }
            if (details.Money < details.Upgradecosts[3])
            {
                MaxHP.IsEnabled = false;
            }
            else
            {
                MaxHP.IsEnabled= true;
            }
            if (details.Money < details.Upgradecosts[4])
            {
                HP_Regen.IsEnabled = false;
            }
            else
            {
                HP_Regen.IsEnabled = true;
            }

        }

        private void visualize()
        {
            if(details.ActualHP > 0)
            {
                actualHPvisual.Width = (int)((details.ActualHP / (double)details.Maxhp) * 700);
            }
            else
            {
                actualHPvisual.Width = 0;
            }

            actualhptext.Content = details.ActualHP + " HP";
            MaxHP_Text.Content = details.Maxhp + " HP";
            Score.Content = "Score: " + details.Score;
            Money.Content = details.Money + " Coin";

        }


        private void checkObjects()
        {
            List<Enemy> enemyids = new List<Enemy>();
            List<Border> bulletids = new List<Border>();
            foreach (Border bullet in bullets)
            {
                if(Canvas.GetTop(bullet)+bullet.ActualHeight < 0)
                {
                    bulletids.Add(bullet);
                }
            }

            foreach(Enemy enemy in enemies)
            {
                if (Canvas.GetTop(enemy) > gamecanvas.ActualHeight)
                {
                    enemyids.Add(enemy);
                }
            }

            foreach (Border bulletke in bulletids)
            {
                gamecanvas.Children.Remove(bulletke);
                bullets.RemoveAt(bullets.IndexOf(bulletke));
            }

            foreach (Enemy enemi in enemyids)
            {
                details.ActualHP -= 10;
                gamecanvas.Children.Remove(enemi);
                enemies.RemoveAt(enemies.IndexOf(enemi));
            }
        }


        //--------------------------------------------------- Lövés


        private void ShootTick(object? sender, EventArgs e)
        {
            Rectangle rectangle = new Rectangle
            {
                Width = 10,
                Height = 20,
                Fill = new SolidColorBrush(Colors.White)
            };
            Border border = new Border
            {
                Width = 5,
                Height = 20,
                BorderBrush = Brushes.Red,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(2),
                Effect = new BlurEffect
                {
                    Radius = 2,
                    KernelType = KernelType.Gaussian,
                    RenderingBias = RenderingBias.Quality
                }
            };

            border.Child = rectangle;
            Canvas.SetLeft(border, Canvas.GetLeft(player) + (player.Width / 2));
            Canvas.SetTop(border, gamecanvas.ActualHeight - (player.Height));
            gamecanvas.Children.Add(border);
            bullets.Add(border);
        }
        private void moveBullet()
        {
            foreach (Border bullet in bullets)
            {
                Canvas.SetTop(bullet, Canvas.GetTop(bullet) - 2);
            }
        }



        private void hitBullet()
        {
            List<Enemy> enemyids = new List<Enemy>();
            List<Border> bulletids = new List<Border>();
            foreach (Enemy enemys in enemies)
            {
                foreach(Border bullet in bullets)
                {
                    if (enemys.intersectBullet(bullet))
                    {
                        enemyids.Add(enemys);
                        bulletids.Add(bullet);
                    }
                }
            }

            foreach(Border bulletke in bulletids)
            {
                if (bullets.IndexOf(bulletke) > -1)
                {
                    gamecanvas.Children.Remove(bulletke);
                    bullets.RemoveAt(bullets.IndexOf(bulletke));
                }

            }
            foreach ( Enemy enemi in enemyids)
            {
                if (enemies.IndexOf(enemi) > -1)
                {
                    details.Score += 10;
                    details.Money += (int)details.Income;
                    gamecanvas.Children.Remove(enemi);
                    enemies.RemoveAt(enemies.IndexOf(enemi));
                }

            }

        }

        //-------------------------------------------------------- Enemy-k hez kötődő metódusok
        private void moveEnemy()
        {
            foreach (Enemy enemy in enemies)
            {
                Canvas.SetTop(enemy, Canvas.GetTop(enemy) + details.Enemyspeed);
            }
        }

        private void generateEnemy()
        {

            if (enemies.Count <= details.Maxenemys)
            {
                Enemy enemy = new Enemy();
                Canvas.SetTop(enemy, 0);
                Canvas.SetLeft(enemy, random.Next(0, (int)(gamecanvas.ActualWidth - enemy.Width)));
                if (!intersectEnemySpawn(enemy))
                {
                    gamecanvas.Children.Add(enemy);


                    enemies.Add(enemy);
                }

            }
        }

        private bool intersectEnemySpawn(Enemy enemy)
        {
            bool intersect = false;

            foreach(Enemy enemys in enemies)
            {
                if (enemy.intersectEnemy(enemys))
                {
                    intersect = true;
                    break;
                }
            }

            return intersect;
        }



        //-----------------------------------------------Player mozgása
        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A)
            {
                left = true;
                
            }
            if (e.Key == Key.D)
            {
                right = true;
            }
        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A)
            {
                left = false;
            }
            if (e.Key == Key.D)
            {
                right = false;
            }
        }


        //---------------------------------------------------------------Upgradek
        private void Horizontalspeedbtn_Click(object sender, RoutedEventArgs e)
        {
            details.Playerspeed += 2;
            details.Money -= details.Upgradecosts[0];
            details.Upgradecosts[0] *= 2; 
        }

        private void Income_Click(object sender, RoutedEventArgs e)
        {
            details.Income *= 1.5;
            details.Money -= details.Upgradecosts[1];
            details.Upgradecosts[1] *= 2;
        }

        private void Firerate_Click(object sender, RoutedEventArgs e)
        {
            details.Firerate -= 50;
            shooterTimer.Interval = TimeSpan.FromMilliseconds(details.Firerate);
            details.Money -= details.Upgradecosts[2];
            details.Upgradecosts[2] *= 2;
        }

        private void MaxHP_Click(object sender, RoutedEventArgs e)
        {
            details.Maxhp +=50;
            details.Money -= details.Upgradecosts[3];
            details.Upgradecosts[3] *= 2;
        }

        private void HP_Regen_Click(object sender, RoutedEventArgs e)
        {
            details.Hpregen += 5;
            details.Money -= details.Upgradecosts[4];
            details.Upgradecosts[4] *= 2;
        }
    }





    //--------------------------------------------------Osztályok
    public class Enemy : Image
    {
        BitmapImage bitmapImage = new BitmapImage();
        public Enemy()
        {
            Height = 50;
            Width = 50;
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri("/Resources/enemy.png", UriKind.Relative);
            bitmapImage.EndInit();
            Source = bitmapImage;

            
        }

        public bool intersectEnemy(Enemy enemy)
        {

            if (Canvas.GetTop(enemy) < enemy.ActualHeight)
            {
                if (Canvas.GetLeft(this) >= Canvas.GetLeft(enemy) && Canvas.GetLeft(this) <= Canvas.GetLeft(enemy) + Width)
                {
                    return true;
                }

                if (Canvas.GetLeft(this) <= Canvas.GetLeft(enemy) && Canvas.GetLeft(this) + Width >= Canvas.GetLeft(enemy))
                {
                    return true;
                }
            }



            return false;
        }
        public bool intersectBullet(Border bullet)
        {
            bool intersecting = false;

            if(Canvas.GetTop(bullet) >= Canvas.GetTop(this) && Canvas.GetTop(bullet) <= Canvas.GetTop(this)+Height)
            {
                if (Canvas.GetLeft(this) <= Canvas.GetLeft(bullet) && Canvas.GetLeft(this) + Width >= Canvas.GetLeft(bullet))
                {
                    intersecting = true;
                }
                if (Canvas.GetLeft(this) <= Canvas.GetRight(bullet) && Canvas.GetLeft(this) + Width >= Canvas.GetRight(bullet))
                {
                    intersecting = true;
                }
            }

            return intersecting;
        }

    }

    public class GameDetails
    {
        private int maxenemys;
        private int score;
        private int money;
        private int maxhp;
        private int hpregen;
        private int playerspeed;
        private double enemyspeed;
        private double income;
        private int firerate;
        private int actualHP;
        private int progressed;
        int[] upgradecosts;


        public GameDetails()
        {
            Maxenemys = 3;
            Score = 0;
            Money = 0;
            Playerspeed = 10;
            Income = 10;
            enemyspeed = 1;
            Firerate = 1000;
            Maxhp = 100;
            hpregen = 0;
            actualHP = 100;
            Progressed = 30;
            firerate = 1000;
            upgradecosts = [10, 10, 10, 10, 10];
        }

        public int Maxenemys { get => maxenemys; set => maxenemys = value; }
        public int Score { get => score; set => score = value; }
        public int Money { get => money; set => money = value; }
        public int Maxhp { get => maxhp; set => maxhp = value; }
        public int Hpregen { get => hpregen; set => hpregen = value; }
        public int Playerspeed { get => playerspeed; set => playerspeed = value; }
        public double Enemyspeed { get => enemyspeed; set => enemyspeed = value; }
        public double Income { get => income; set => income = value; }
        public int ActualHP { get => actualHP; set => actualHP = value; }
        public int Progressed { get => progressed; set => progressed = value; }
        public int Firerate { get => firerate; set => firerate = value; }
        public int[] Upgradecosts { get => upgradecosts; set => upgradecosts = value; }
    }
}
