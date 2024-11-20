using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;

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
using System.Threading;

using System.Xml;
using Newtonsoft.Json;
using System.Security.Policy;
using System.Security.Cryptography;
using ConsoleApp3;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Xml.Linq;
using Zen.Barcode;
using System.Drawing;
using System.IO;

namespace Tyxy
{
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Title += "    UserID：" + _Config._appAccNo.ToString();
            this.Loaded += MainWindow_Loaded;
            
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
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

            // 使用 Zen.Barcode 生成二维码
            CodeQrBarcodeDraw qrcode = BarcodeDrawFactory.CodeQr;
            System.Drawing.Image img = qrcode.Draw(wifiString, 20); // 40 是二维码大小比例

            // 创建新的位图，用来处理颜色替换
            Bitmap bitmap = new Bitmap(img);

            // 遍历每个像素，替换白色部分为指定的颜色（43,43,43）
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    // 获取当前像素颜色
                    System.Drawing.Color pixelColor = bitmap.GetPixel(x, y);

                    // 如果是白色（判断标准为255,255,255），将其替换为指定的颜色
                    if (pixelColor.R == 255 && pixelColor.G == 255 && pixelColor.B == 255)
                    {
                       // bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(0, 0, 0));
                    }
                }
            }
            // 遍历每个像素，替换白色部分为指定的颜色（43,43,43）
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    // 获取当前像素颜色
                    System.Drawing.Color pixelColor = bitmap.GetPixel(x, y);

                    if (pixelColor.R == 255 && pixelColor.G == 255 && pixelColor.B == 255)
                    {
                        bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(0, 0, 0, 0)); // 设置为完全透明
                    }

                }
            }
            // 将 System.Drawing.Bitmap 转换为 BitmapImage
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png); // 保存到内存流
                memory.Position = 0; // 重置流的位置

                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory; // 将内存流设置为源
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze(); // 冻结以便跨线程使用
            }
            QRImage.Source = bitmapImage;
        }
    }
}
