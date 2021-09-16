using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using Newtonsoft.Json;

#pragma warning disable 618

namespace LIMSCodeBarPrinter
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnassignedField.Global")]
    public class CodeBar
    {
        public string ids;
        public string biom;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnassignedField.Global")]
    public class Parcel
    {
        public string type;
        public string id;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnassignedField.Global")]
    public class BasicResponse
    {
        public string detail;
    }

    public partial class MainForm : Form
    {
        private static readonly string LimsUrl = ConfigurationSettings.AppSettings["LimsUrl"];
        private static readonly string LimsToken = ConfigurationSettings.AppSettings["LimsToken"];
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
            _port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            try
            {
                _port.Open();
            }
            catch (Exception ex)
            {
                btnPrint.Enabled = false;
                const string msgPortError = "Ошибка открытия COM-порта:\n{0}";
                tbLog.Text = string.Format(msgPortError, ex.Message);
            }
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var message = _port.ReadExisting();
            Console.WriteLine($@"Получена нагрузка {message}");
            // Пробуем привести полученную нагрузку от сканера ШК к типу партии биоматериала
            try
            {
                Console.WriteLine($@"Пробуем нагрузку к типу партии биоматериала: {message}");
                var parcel = JsonConvert.DeserializeObject<Parcel>(message);
                
                if (parcel.type == "parcel")
                {
                    tbOrder.Text = parcel.id;
                    try
                    {
                        var response = GetWebRequestResponse($"/api/lism/checkpoint_parcel/{parcel.id}/");
                        using (var sr = new StreamReader(response.GetResponseStream()))
                        {
                            var json = sr.ReadToEnd();
                            var result = JsonConvert.DeserializeObject<BasicResponse>(json);
                            tbLog.Text = result.detail;
                        }
                    }
                    catch (WebException exception)
                    {
                        tbLog.Text = exception.Message;
                    }
                    return;
                }
            }
            catch (Exception)
            {
                // ignored
            }
            if (message.Length >= 20) return;
            this.tbOrder.Text = message;
            BtnPrintClick(sender, null);
        }

        private static HttpWebResponse GetWebRequestResponse(string endpoint)
        {
            var url = $"{LimsUrl}{endpoint}";
            var request = WebRequest.Create(url);
            request.ContentType = "application/json; charset=utf-8";
            if (LimsToken.Length > 0)
            {
                request.Headers.Add("Authorization", $"Token {LimsToken}");
            }
            return (HttpWebResponse) request.GetResponse();
        }
        
        private void BtnPrintClick(object sender, EventArgs e)
        {
            var response = GetWebRequestResponse($"/api/lism/samples_by_order/{this.tbOrder.Text.Trim()}/");

            using (var sr = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()))
            {
                this.tbLog.Text = "";
                var json = sr.ReadToEnd();
                List<CodeBar> codeBars = new List<CodeBar>();
                try
                {
                    codeBars = JsonConvert.DeserializeObject<List<CodeBar>>(json);
                }
                catch (Exception)
                {
                    // ignored
                }

                if (codeBars.Count == 0)
                {
                    const string text = "Заказ не найден в ЛИС";
                    this.tbLog.Text = text;
                }

                foreach (var codeBar in codeBars)
                {
                    PrintBarCode(codeBar);
                    this.tbLog.Text += $@"{codeBar.biom} - {codeBar.ids}{Environment.NewLine}";
                }
            }
        }

        private void PrintBarCode(CodeBar label)
        {
            const string template = "^Q25,2\n^W46\n^H8\n^P1\n^S7\n^AD\n^C1\n^R0\n~Q+0\n^O0\n^D0\n^E18\n~R255\n" +
                                    "^L\nDy2-me-dd\nTh:m:s\nBA3,40,86,2,5,80,0,3,{0}\nAD,40,20,1,1,0,0,{1}\nE\n";

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
                const string msgPrintError = "Произошла ошибка отправки данных на принтер:\n{0}";
                tbLog.Text = string.Format(msgPrintError, ex.Message);
            }
            finally
            {
                client.Close();
            }
        }
    }
}