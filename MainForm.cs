using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using Newtonsoft.Json;

#pragma warning disable 618

namespace LIMSCodeBarPrinter
{
    public class CodeBar
    {
        public string ids;
        public string biom;
    }

    public partial class MainForm : Form
    {
        private static readonly string PortName = ConfigurationSettings.AppSettings["SerialPort"];
        private static readonly string PrinterIpAddress = ConfigurationSettings.AppSettings["PrinterIPAddress"];
        private static readonly int PrinterIpPort = int.Parse(ConfigurationSettings.AppSettings["PrinterIPPort"]);

        private readonly SerialPort _port = new SerialPort(PortName, 9600, Parity.None, 8, StopBits.One);

        public MainForm()
        {
            InitializeComponent();
            SerialPortProgram();
        }

        private void SerialPortProgram()
        {
            _port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            try
            {
                _port.Open();
            }
            catch (Exception ex)
            {
                btnPrint.Enabled = false;
                tbLog.Text = $@"Ошибка открытия COM-порта:{Environment.NewLine}{ex.Message}";
            }
        }

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var message = _port.ReadExisting();
            if (message.Length < 20)
            {
                this.tbOrder.Text = message;
                btnPrint_Click(sender, null);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            var url = $"http://10.74.22.2/api/lism/samples_by_order/{this.tbOrder.Text.Trim()}/";
            var request = WebRequest.Create(url);
            request.ContentType = "application/json; charset=utf-8";
            var response = (HttpWebResponse) request.GetResponse();

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                this.tbLog.Text = "";
                var json = sr.ReadToEnd();
                List<CodeBar> codeBars = new List<CodeBar>();
                try
                {
                    codeBars = JsonConvert.DeserializeObject<List<CodeBar>>(json);
                }
                catch (Exception exception)
                {
                    // ignored
                }

                if (codeBars.Count == 0)
                {
                    this.tbLog.Text = @"Заказ не найден в ЛИС";
                }

                foreach (var codeBar in codeBars)
                {
                    printBarCode(codeBar);
                    this.tbLog.Text += $@"{codeBar.biom} - {codeBar.ids}{Environment.NewLine}";
                }
            }
        }

        private void printBarCode(CodeBar label)
        {
            const string template = "^Q25,2\n^W46\n^H8\n^P1\n^S7\n^AD\n^C1\n^R0\n~Q+0\n^O0\n^D0\n^E18\n~R255\n" +
                                    "^L\nDy2-me-dd\nTh:m:s\nBA3,23,84,3,5,80,0,3,{0}\nAD,20,20,1,1,0,0E,{1}\nE\n";

            TcpClient client = new TcpClient();
            try
            {
                client.Connect(PrinterIpAddress, PrinterIpPort);

                var writer = new StreamWriter(client.GetStream());
                writer.Write(template, label.ids, label.biom);
                writer.Flush();

                writer.Close();
            }
            catch (Exception ex)
            {
                tbLog.Text = $@"Произошла ошибка отправки данных на принтер:{Environment.NewLine}{ex.Message}";
            }
            finally
            {
                client.Close();
            }
        }
    }
}