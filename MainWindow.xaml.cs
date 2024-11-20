using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Threading;
using Newtonsoft.Json;
using ConsoleApp3;
using ZXing;
using System.Drawing;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;
using System.Drawing.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Point = System.Windows.Point;

namespace Tyxy
{
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();
            this.Title += "    UserID：" + _Config._appAccNo.ToString();
            this.Loaded += MainWindow_Loaded;
           
        }
        /// <summary>
        /// 随机数
        /// </summary>
        private Random _random = new Random();

        //布局宽490 高210 显示宽430 高180
        //阵距4行8列 点之间的距离 X轴Y轴都是70
        /// <summary>
        /// 点信息阵距
        /// </summary>
        private PointInfo[,] _points = new PointInfo[8, 4];

        /// <summary>
        /// 计时器
        /// </summary>
        private DispatcherTimer _timer;


        /// <summary>
        /// 初始化阵距
        /// </summary>
        private void Init()
        {

            //生成阵距的点
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    double x = _random.Next(-11, 11);
                    double y = _random.Next(-6, 6);
                    _points[i, j] = new PointInfo()
                    {
                        X = i * 110,
                        Y = j * 135,
                        SpeedX = x / 24,
                        SpeedY = y / 24,
                        DistanceX = _random.Next(35, 106),
                        DistanceY = _random.Next(20, 40),
                        MovedX = 0,
                        MovedY = 0,
                        PolygonInfoList = new List<PolygonInfo>()
                    };
                }
            }

            //byte r = (byte)_random.Next(0, 11);
            //byte g = (byte)_random.Next(100, 201);
            //int intb = g + _random.Next(50, 101);

            byte r = (byte)_random.Next(200, 256); // 红色较高
            byte g = (byte)_random.Next(50, 150);  // 绿色适中
            int intb = g + _random.Next(150, 255); // 蓝色较高
            if (intb > 255)
                intb = 255;
            byte b = (byte)intb;

            //上一行取2个点 下一行取1个点
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Polygon poly = new Polygon();
                    poly.Points.Add(new Point(_points[i, j].X, _points[i, j].Y));
                    _points[i, j].PolygonInfoList.Add(new PolygonInfo() { PolygonRef = poly, PointIndex = 0 });
                    poly.Points.Add(new Point(_points[i + 1, j].X, _points[i + 1, j].Y));
                    _points[i + 1, j].PolygonInfoList.Add(new PolygonInfo() { PolygonRef = poly, PointIndex = 1 });
                    poly.Points.Add(new Point(_points[i + 1, j + 1].X, _points[i + 1, j + 1].Y));
                    _points[i + 1, j + 1].PolygonInfoList.Add(new PolygonInfo() { PolygonRef = poly, PointIndex = 2 });
                    poly.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(r, g, (byte)b));
                    SetColorAnimation(poly);
                    layout.Children.Add(poly);
                }
            }

            //上一行取1个点 下一行取2个点
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Polygon poly = new Polygon();
                    poly.Points.Add(new Point(_points[i, j].X, _points[i, j].Y));
                    _points[i, j].PolygonInfoList.Add(new PolygonInfo() { PolygonRef = poly, PointIndex = 0 });
                    poly.Points.Add(new Point(_points[i, j + 1].X, _points[i, j + 1].Y));
                    _points[i, j + 1].PolygonInfoList.Add(new PolygonInfo() { PolygonRef = poly, PointIndex = 1 });
                    poly.Points.Add(new Point(_points[i + 1, j + 1].X, _points[i + 1, j + 1].Y));
                    _points[i + 1, j + 1].PolygonInfoList.Add(new PolygonInfo() { PolygonRef = poly, PointIndex = 2 });
                    poly.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(r, g, (byte)b));
                    SetColorAnimation(poly);
                    layout.Children.Add(poly);
                }
            }
        }

        /// <summary>
        /// 设置颜色动画
        /// </summary>
        /// <param name="polygon">多边形</param>
        private void SetColorAnimation(UIElement polygon)
        {
            //颜色动画的时间 1-4秒随机
            Duration dur = new Duration(new TimeSpan(0, 0, _random.Next(1, 5)));
            //故事版
            Storyboard sb = new Storyboard()
            {
                Duration = dur
            };
            sb.Completed += (S, E) => //动画执行完成事件
            {
                //颜色动画完成之后 重新set一个颜色动画
                SetColorAnimation(polygon);
            };
            //颜色动画
            ////颜色的RGB
            //byte r = (byte)_random.Next(0, 11);
            //byte g = (byte)_random.Next(100, 201);
            //int intb = g + _random.Next(50, 101);
            // 设置颜色为粉色的 RGB 范围
            byte r = (byte)_random.Next(200, 256); // 红色较高
            byte g = (byte)_random.Next(50, 150);  // 绿色适中
            int intb = g + _random.Next(150, 255); // 蓝色较高

            if (intb > 255)
                intb = 255;
            byte b = (byte)intb;
            ColorAnimation ca = new ColorAnimation()
            {
                To = System.Windows.Media.Color.FromRgb(r, g, b),
                Duration = dur
            };
            Storyboard.SetTarget(ca, polygon);
            Storyboard.SetTargetProperty(ca, new PropertyPath("Fill.Color"));
            sb.Children.Add(ca);
            sb.Begin(this);
        }

        /// <summary>
        /// 多边形变化动画
        /// </summary>
        void PolyAnimation(object sender, EventArgs e)
        {
            //不改变阵距最外边一层的点
            for (int i = 1; i < 7; i++)
            {
                for (int j = 1; j < 3; j++)
                {
                    PointInfo pointInfo = _points[i, j];
                    pointInfo.X += pointInfo.SpeedX;
                    pointInfo.Y += pointInfo.SpeedY;
                    pointInfo.MovedX += pointInfo.SpeedX;
                    pointInfo.MovedY += pointInfo.SpeedY;
                    if (pointInfo.MovedX >= pointInfo.DistanceX || pointInfo.MovedX <= -pointInfo.DistanceX)
                    {
                        pointInfo.SpeedX = -pointInfo.SpeedX;
                        pointInfo.MovedX = 0;
                    }
                    if (pointInfo.MovedY >= pointInfo.DistanceY || pointInfo.MovedY <= -pointInfo.DistanceY)
                    {
                        pointInfo.SpeedY = -pointInfo.SpeedY;
                        pointInfo.MovedY = 0;
                    }
                    //改变多边形的点
                    foreach (PolygonInfo pInfo in _points[i, j].PolygonInfoList)
                    {
                        pInfo.PolygonRef.Points[pInfo.PointIndex] = new Point(pointInfo.X, pointInfo.Y);
                    }
                }
            }
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
            //注册帧动画
            _timer = new System.Windows.Threading.DispatcherTimer();
            _timer.Tick += new EventHandler(PolyAnimation);
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 24);//一秒钟刷新24次
            _timer.Start();
            try {
                //Console.WriteLine(GetCookies.GetCookie());
                cookiestextbox.Text = GetCookies.GetCookie();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ShowChildWindow("错误", ex.Message);
               
            }

            _cookies = cookiestextbox.Text.Replace("\n", "; ").Replace("\r", "").Replace(" ", "; ");
            //Console.BackgroundColor = ConsoleColor.Green;
            
            Console.WriteLine(_cookies);
            

        }
        static int _appAccNo { get; set; }
        static string _cookies = "";
        static int _ID { get; set; }
        private SemaphoreSlim _semaphore;
        bool ISOK = false;
        List<Thread> Threads = new List<Thread>();
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sliderValueText.Text = "线程数量：" + (int)slider.Value;
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            ISOK = !ISOK;
            _ID = int.Parse(IDTextbox.Text);
            _cookies = cookiestextbox.Text.Replace("\n", "; ").Replace("\r", "").Replace(" ", "; ");
            if (ISOK)
            {
                button.Content = "Starting...";


                int maxThreads = (int)slider.Value; // 最大线程数
                _semaphore = new SemaphoreSlim(maxThreads); // 并发数
                for (int i = 0; i < maxThreads; i++)
                {
                    try
                    {

                        Thread thread = new Thread(() => SendRequestSync());
                        thread.Start();
                        Threads.Add(thread);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"请求失败: {ex}");
                    }
                }
                button.Content = "Stop";
            }
            else
            {
                button.Content = "Stoping...";
                foreach (var item in Threads)
                {
                    item.Abort();
                }
                button.Content = "Start";

            }

        }
        private void SendRequestSync()
        {
            SendRequestAsync(_ID).GetAwaiter().GetResult();
        }
        //private async Task SendRequest(HttpClient client)
        //{
        //    DateTime nextDay = DateTime.Now.AddDays(1);
        //    string beginTime = nextDay.ToString("yyyy-MM-dd 08:00:00");
        //    string endTime = nextDay.ToString("yyyy-MM-dd 21:30:00");
        //    var data = new
        //    {
        //        sysKind = 8,
        //        appAccNo = 10080,
        //        memberKind = 1,
        //        resvMember = new[] { 10080 },
        //        resvBeginTime = beginTime,
        //        resvEndTime = endTime,
        //        testName = "",
        //        resvProperty = 0,
        //        resvDev = new[] { 474 },
        //        memo = ""
        //    };
        //    string jsonData = JsonSerializer.Serialize(data);

        //    while (ISOK)
        //    {

        //        _ = Task.Run(async () =>
        //        {
        //            try
        //            {

        //                var request = new HttpRequestMessage(HttpMethod.Post, "/unifoundic/ic-web/reserve")
        //                {
        //                    Content = new StringContent(jsonData, Encoding.UTF8, "application/json")
        //                };

        //                // 请求头
        //                request.Headers.Add("Host", "rh.tyu.edu.cn");
        //                request.Headers.Add("Connection", "keep-alive");
        //                request.Headers.Add("Accept", "application/json, text/plain, */*");
        //                request.Headers.Add("Cookie", _cookies);
        //                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.5304.110 Safari/537.36 Language/zh ColorScheme/Light wxwork/4.1.30 (MicroMessenger/6.2) WindowsWechat MailPlugin_Electron WeMail embeddisk wwmver/3.26.13.685");
        //                request.Headers.Add("token", "775f487988aa41b69ae3e204b3a76eb1");
        //                request.Headers.Add("Origin", "https://rh.tyu.edu.cn");



        //                _ = wrapPanel.Dispatcher.Invoke(async () =>
        //                {
        //                    Ellipse dot = new Ellipse
        //                    {
        //                        Width = 6,
        //                        Height = 6,
        //                        Margin = new Thickness(1),
        //                        Stroke = new SolidColorBrush(Colors.Gray), // 灰色边框
        //                        StrokeThickness = 1,
        //                        Fill = new SolidColorBrush(Colors.Black) // 初始颜色
        //                    };
        //                    wrapPanel.Children.Add(dot);
        //                    // 自动滚动到 ScrollViewer 的底部
        //                    scrollViewer.ScrollToEnd();
        //                    string responseContent = null;
        //                    await Task.Run(async () =>
        //                    {
        //                        // 发送请求并获取响应
        //                        HttpResponseMessage response = await client.SendAsync(request);
        //                        responseContent = await response.Content.ReadAsStringAsync();

        //                    });
        //                    // 根据响应内容设置圆点颜色
        //                    if (responseContent.Contains("未登录"))
        //                    {
        //                        dot.Fill = new SolidColorBrush(Colors.Red);
        //                    }
        //                    else if (responseContent.Contains("成功"))
        //                    {
        //                        dot.Fill = new SolidColorBrush(Colors.Green);
        //                        MessageBox.Show(responseContent);
        //                    }
        //                    else if (responseContent.Contains("不在提前预约时间范围内"))
        //                    {
        //                        dot.Fill = new SolidColorBrush(Colors.Yellow);
        //                    }
        //                    else
        //                    {
        //                        dot.Fill = new SolidColorBrush(Colors.Blue); 
        //                    }
        //                });
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine($"请求失败: {ex.Message}");
        //                wrapPanel.Dispatcher.Invoke(() =>
        //                {
        //                    Ellipse dot = new Ellipse
        //                    {
        //                        Width = 10,
        //                        Height = 10,
        //                        Margin = new Thickness(2),
        //                        Fill = new SolidColorBrush(Colors.Red), 
        //                        Stroke = new SolidColorBrush(Colors.Gray),
        //                        StrokeThickness = 1
        //                    };
        //                    wrapPanel.Children.Add(dot);
        //                });
        //            }
        //        });

        //        await Task.Delay(50);
        //    }
        //}
        public bool ShowChildWindow(string a, string b)
        {
            // 显示遮罩层
            Overlay.Visibility = Visibility.Visible;

            // 创建新的子窗口
            var childWindow = new MsgBox(a, b);

            // 计算子窗口的位置
            childWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            childWindow.Left = this.Left + (this.Width - childWindow.Width) / 2;
            childWindow.Top = this.Top + (this.Height - childWindow.Height) / 2;

            // 监听子窗口的关闭事件
            childWindow.Closed += ChildWindow_Closed;
            childWindow.Owner = this;
            // 显示子窗口并等待其关闭
            childWindow.ShowDialog();

            return childWindow.Result;
            // 检查子窗口的返回值
        }
        private void ChildWindow_Closed(object sender, EventArgs e)
        {
            // 隐藏遮罩层
            Overlay.Visibility = Visibility.Collapsed;
        }

        public async Task SendRequestAsync(int SetID)
        {
            // 设置请求的 URL
            var url = "https://rh.tyu.edu.cn/unifoundic/ic-web/reserve";

            // 计算第二天的日期
            var nextDay = DateTime.Now.AddDays(1);
            string beginTime = nextDay.ToString("yyyy-MM-dd 08:00:00");
            string endTime = nextDay.ToString("yyyy-MM-dd 21:30:00");

            // 设置请求数据
            var data = new
            {
                sysKind = 8,
                appAccNo = _Config._appAccNo,
                memberKind = 1,
                resvMember = new[] { _Config._appAccNo },
                resvBeginTime = beginTime,
                resvEndTime = endTime,
                testName = "",
                resvProperty = 0,
                resvDev = new[] { SetID },
                memo = ""
            };

            // 将请求数据序列化为 JSON 字符串
            string jsonData = JsonConvert.SerializeObject(data);
            while (ISOK)
            {
                HttpClientHandler handler = new HttpClientHandler()
                {
                    // 这里处理 Cookie
                    CookieContainer = new CookieContainer(),
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true // 禁用 SSL 证书验证
                };

                HttpClient client = new HttpClient(handler);

                // 设置 Cookie
                var cookies = handler.CookieContainer;
                cookies.Add(new Uri("https://rh.tyu.edu.cn/"), new Cookie("ic-cookie", _cookies.Split('=')[1]));

                // 创建请求消息
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(jsonData, Encoding.UTF8, "application/json")
                };

                // 设置请求头
              //  requestMessage.Headers.Add("Host", "rh.tyu.edu.cn");
              //  requestMessage.Headers.Add("Connection", "keep-alive");
                requestMessage.Headers.Add("Accept", "application/json, text/plain, */*");
               // requestMessage.Headers.Add("User-Agent", "Mozilla/5.0");
                //requestMessage.Headers.Add("token", "775f487988aa41b69ae3e204b3a76eb1");
                requestMessage.Headers.Add("Origin", "https://rh.tyu.edu.cn");


                try
                {

                    

                    _ = wrapPanel.Dispatcher.Invoke(async () =>
                        {
                            Ellipse dot = new Ellipse
                            {
                                Width = 10,
                                Height = 10,
                                Margin = new Thickness(1),
                                Stroke = new SolidColorBrush(Colors.Gray), // 灰色边框
                                StrokeThickness = 1,
                                Fill = new SolidColorBrush(Colors.Black) // 初始颜色
                            };
                            wrapPanel.Children.Add(dot);
                            string responseContent =null;
                            await Task.Run(async () =>
                            {
                                // 发送请求并获取响应
                                var response = await client.SendAsync(requestMessage);
                                string bodyText = await response.Content.ReadAsStringAsync();
                                responseContent = bodyText;



                            });
                            // 自动滚动到 ScrollViewer 的底部
                            scrollViewer.ScrollToEnd();
                            //解析
                            var body = JsonConvert.DeserializeObject<dynamic>(responseContent);


                            responseContent = body.message;
                            string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                            // 根据响应内容设置圆点颜色
                            if (responseContent.Contains("未登录"))
                            {
                                dot.Fill = new SolidColorBrush(Colors.Red);
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"[{currentTime}] {responseContent}");
                            }
                            else if (responseContent.Contains("成功"))
                            {
                                dot.Fill = new SolidColorBrush(Colors.Green);
                                foreach (var item in Threads)
                                {
                                    item.Abort();
                                }
                                button.Content = "Start";
                                ISOK = false;
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"[{currentTime}] {responseContent}");
                                MessageBox.Show(responseContent);

                            }
                            else if (responseContent.Contains("不在提前预约时间范围内"))
                            {
                                dot.Fill = new SolidColorBrush(Colors.Yellow);
                               
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine($"[{currentTime}] {responseContent}");
                            }
                            else
                            {
                                dot.Fill = new SolidColorBrush(Colors.Blue);
                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.WriteLine($"[{currentTime}] {responseContent}");
                            }
                        });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Request failed: {ex.Message}");
                }
                await Task.Delay(100);

            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 设置你要启动的程序路径
                string programPath = @"位置选取器.exe";  // 修改为实际程序路径

                // 启动进程
                System.Diagnostics.Process.Start(programPath);
            }
            catch (Exception ex)
            {
                // 错误处理
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cookiestextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // 构造分享WiFi的字符串内容
            string wifiString = $"tyxy|{cookiestextbox.Text}|{IDTextbox.Text}|{_Config._appAccNo}";

            var qrWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 300,
                    Height = 300,
                    Margin = 3    // 设置白边为0
                }
            };
            var bitmap = qrWriter.Write(wifiString);
            // 创建透明背景图像
            var transparentBitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    // 将白色背景改为透明
                    var pixel = bitmap.GetPixel(x, y);
                    if (pixel.R == 255 && pixel.G == 255 && pixel.B == 255)
                    {
                        transparentBitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0)); // 透明
                    }
                    else
                    {
                        transparentBitmap.SetPixel(x, y, Color.Black); // 黑色方块
                    }
                }
            }
            // 将 System.Drawing.Bitmap 转换为 BitmapImage
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream memory = new MemoryStream())
            {
                transparentBitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png); // 保存到内存流
                memory.Position = 0; // 重置流的位置

                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory; // 将内存流设置为源
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze(); // 冻结以便跨线程使用
            }
            QRImage.Source = bitmapImage;
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch { }
        }

        private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // 打开 GitHub 的链接
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://github.com/Laster-dev/Tyxy",
                UseShellExecute = true
            });
        }

        private void Image_MouseLeftButtonDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Environment.Exit(0);
        }
    }
    /// <summary>
    /// 阵距点信息
    /// </summary>
    public class PointInfo
    {
        /// <summary>
        /// X坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y坐标
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// X轴速度 wpf距离单位/二十四分之一秒
        /// </summary>
        public double SpeedX { get; set; }

        /// <summary>
        /// Y轴速度 wpf距离单位/二十四分之一秒
        /// </summary>
        public double SpeedY { get; set; }

        /// <summary>
        /// X轴需要移动的距离
        /// </summary>
        public double DistanceX { get; set; }

        /// <summary>
        /// Y轴需要移动的距离
        /// </summary>
        public double DistanceY { get; set; }

        /// <summary>
        /// X轴已经移动的距离
        /// </summary>
        public double MovedX { get; set; }

        /// <summary>
        /// Y轴已经移动的距离
        /// </summary>
        public double MovedY { get; set; }

        /// <summary>
        /// 多边形信息列表
        /// </summary>
        public List<PolygonInfo> PolygonInfoList { get; set; }
    }

    /// <summary>
    /// 多边形信息
    /// </summary>
    public class PolygonInfo
    {
        /// <summary>
        /// 对多边形的引用
        /// </summary>
        public Polygon PolygonRef { get; set; }

        /// <summary>
        /// 需要改变的点的索引
        /// </summary>
        public int PointIndex { get; set; }
    }

}
