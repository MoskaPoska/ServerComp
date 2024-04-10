using LibraryProduct;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            string[] cat = new string[] { "Ноутбук", "Телефон", "Планшет", "Компьютер" };
            comboBox1.Items.AddRange(cat);
            //comboBox1.SelectedIndex = 0;
            Process.Start("ServerComp.exe");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int remotePort;
            IPAddress remoteAdress;
            if (IPAddress.TryParse(textBox1.Text, out remoteAdress) && int.TryParse(textBox2.Text, out remotePort))
            {
                UdpClient client = new UdpClient();
                try
                {
                    byte[] buff = Encoding.Default.GetBytes(comboBox1.SelectedItem.ToString());
                    IPEndPoint remoteEp = new IPEndPoint(remoteAdress, remotePort);
                    client.Send(buff, buff.Length, remoteEp);

                    buff = client.Receive(ref remoteEp);
                    string serverAnswer = Encoding.UTF8.GetString(buff);

                    List<Product> goods = JsonSerializer.Deserialize<List<Product>>(serverAnswer);
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Category\t   Title\t    Price");
                    goods.ForEach(t => sb.AppendLine($"{t.Category}{t.Title}{t.Price}"));
                    textBox3.Text += sb.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    client.Close();
                }
            }
        }
    }
}