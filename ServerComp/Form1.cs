using LibraryProduct;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ServerComp
{
    public partial class Form1 : Form
    {
        List<Product> answerList;
        DbContextOptions<ShopContext> options;
        DbContextOptionsBuilder<ShopContext> optionsBuilder;
        string url = "Data Source=desktop-9ek8d4r\\sqlexpress;Initial Catalog=ShopPC;Integrated Security=True;Connect Timeout=30;";
        Task task;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Text += "Server start working";
            StartServer();           
        }
        private void StartServer()
        {
            if (task != null)
                return;
            task = Task.Run(() =>
            {
                int localPort;
                if (int.TryParse(textBox1.Text, out localPort))
                {
                    UdpClient listener = new UdpClient(new IPEndPoint(IPAddress.Parse("192.168.56.1"), localPort));
                    IPEndPoint remoteEp = null;
                    try
                    {
                        while (true)
                        {
                            byte[] buff = listener.Receive(ref remoteEp);
                            string gottenCategory = Encoding.UTF8.GetString(buff);

                            answerList = GetInfo(gottenCategory);

                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine($"{buff.Length} получено от {remoteEp} как {DateTime.Now}");
                            sb.AppendLine($"Финал запроса {gottenCategory}");

                            string answerJson = JsonSerializer.Serialize<List<Product>>(answerList);

                            UdpClient client = new UdpClient(11000);
                            try
                            {
                                buff = Encoding.UTF8.GetBytes(answerJson);
                                client.Send(buff, buff.Length, remoteEp);
                            }
                            catch (SystemException ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            finally
                            {
                                client.Close();
                            }
                            sb.AppendLine("Серевер дал добро");
                            textBox2.BeginInvoke(new Action<string>(AddText), sb.ToString());
                        }
                    }
                    catch (SystemException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        listener.Close();
                    }
                }
            });
        }
        private void AddText(string str)
        {
            StringBuilder sb = new StringBuilder(); 
            sb.Append(str);
            textBox2.Text = sb.ToString();
        }
        private List<Product> GetInfo(string gottenCategory)
        {
            List<Product> goods = null;
            try
            {
                optionsBuilder = new DbContextOptionsBuilder<ShopContext>();
                optionsBuilder.UseSqlServer(url);
                options = optionsBuilder.Options;
                using(ShopContext con = new ShopContext(options))
                {
                    goods = con.Products.Where(t=>t.Category == gottenCategory).ToList();
                }
            }
            catch (SystemException ex)
            {

                MessageBox.Show(ex.Message);
            }
            return goods;
        }
    }
}