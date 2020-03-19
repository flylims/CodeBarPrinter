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
            this.tbOrder.Text = _port.ReadExisting();
            btnPrint_Click(sender, null);
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
                List<CodeBar> codeBars = JsonConvert.DeserializeObject<List<CodeBar>>(json);
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
            var zplString =
                $"^XA^FO40,50^BY2^B3N,,100,Y,N^FD{label.ids}^FS^CFA,15^FO0,15^FB368,0,16,C,0^FD{label.biom}^FS^XZ";
            
            TcpClient client = new TcpClient();
            try
            {
                client.Connect(PrinterIpAddress, PrinterIpPort);

                var writer = new StreamWriter(client.GetStream());
                writer.Write(zplString);
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