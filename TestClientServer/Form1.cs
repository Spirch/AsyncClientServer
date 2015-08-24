using AsyncClientServer;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace TestClientServer
{
	internal delegate void AppendTextDelegate(string msg);
	internal delegate void HandleTitleChange(string msg);
	
	public partial class Form1 : Form
	{

		private const string AddressTest = "127.0.0.1";
		private const int PortTest = 54321;

		private Client client;
		private Server server;
		private Random rnd;

		private RandomNumberGenerator secureRnd;

		private bool isClient;

		public Form1(bool client, bool enableTimer, bool randomData, bool disconnect, bool append)
		{
			InitializeComponent();
			this.Text = client ? "client" : "Server";
			rnd = new Random();
			secureRnd = RandomNumberGenerator.Create();

			isClient = client;
			chkTimer.Checked = enableTimer;
			chkData.Checked = randomData;
			chkDisconnect.Checked = disconnect;
			chkAppend.Checked = append;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			textBox2.KeyDown += new KeyEventHandler(textBox2_KeyDown);

			if (chkTimer.Checked && !isClient)
			{
				HandleServer();
			}

			timer1.Interval = rnd.Next(500, 2000);
			timer1.Enabled = chkTimer.Checked;
		}

		void textBox2_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				Send(textBox2.Text);
				textBox2.Text = "";
			}
		}

		private void button3_Click(object sender, EventArgs e)
		{
			if (client != null)
			{
				this.Text = "Client - Closed";
				client.Close();
			}
		}

		private void button4_Click(object sender, EventArgs e)
		{
			if (server != null)
			{
				timer1.Enabled = false;
				this.Text = "Server - Closed";
				server.Stop();
			}
		}

		private void Send(string msg)
		{
			if (!string.IsNullOrWhiteSpace(msg))
			{ 
				if (client != null)
				{
					client.Send(msg);
				}
				else if (server != null)
				{
					HandleNewMsg(Environment.NewLine + msg);
					server.SendAll(msg);
				}
			}
		}

		private void HandleTitleChange(string msg)
		{
			if (this.InvokeRequired)
			{
				this.BeginInvoke(new HandleTitleChange(HandleTitleChange), new object[] { msg });
			}
			else
			{
				this.Text = msg;
			}
		}

		private void HandleNewMsg(string msg)
		{			
			if(chkAppend.Checked)
			{
				if (this.InvokeRequired)
				{
					this.BeginInvoke(new AppendTextDelegate(HandleNewMsg), new object[] { msg });
				}
				else
				{
					textBox1.AppendText(msg);
				}
			}
		}

		#region "Client"

		private void button2_Click(object sender, EventArgs e)
		{
			HandleClient();
		}

		private void HandleClient()
		{
			if (client == null)
			{
				client = new Client();
				client.MessageReceived += client_OnMessageReceived;
				client.Disconnected += client_OnConnectionClosed;
				client.Connected += client_OnConnected;
				client.SocketError += client_OnSocketError;
				client.ClientIdReceived += client_OnReceivedClientId;
				client.ListClientIdReceived += client_OnReceivedListClientId;
			}

			client.Connect(AddressTest, PortTest);
		}


		void client_OnReceivedListClientId(Client sender, ListClientIdEventArgs eventArgs)
		{
			this.Invoke(new Action(() =>
			{
				listBox1.Items.Clear();
				listBox1.Items.AddRange(eventArgs.Id.Select(x => (object)x).ToArray());
			}));			
		}

		void client_OnReceivedClientId(Client sender, ClientIdEventArgs eventArgs)
		{
			HandleTitleChange("Client - Connected - " + client.Id);
		}

		private void client_OnSocketError(Client sender, SocketErrorEventArgs eventArgs)
		{
			MessageBox.Show(eventArgs.Exception.Message + Environment.NewLine + eventArgs.Exception.StackTrace);
		}

		void client_OnConnected(Client sender, EventArgs eventArgs)
		{
			HandleTitleChange("Client - Connected - Unknown Id");
			HandleNewMsg(Environment.NewLine + "Connected!");
		}

		private void client_OnConnectionClosed(Client sender, EventArgs eventArgs)
		{
			timer1.Enabled = false;
			HandleTitleChange("Client - Disconnected");
			HandleNewMsg(Environment.NewLine + "Disconnected!");
		}

		private void client_OnMessageReceived(Client client, ReceivedEventArgs eventArgs)
		{
			if (eventArgs.Message != null)
			{
				HandleNewMsg(Environment.NewLine + Encoding.UTF8.GetString(eventArgs.Message));
			}
		}
		#endregion

		#region "Server"

		private void button1_Click(object sender, EventArgs e)
		{
			HandleServer();
		}

		private void HandleServer()
		{
			if (server == null)
			{
				server = new Server();
				server.MessageReceived += server_OnMessageReceived;
				server.Disconnected += server_OnConnectionClosed;
				server.Connected += server_OnConnected;
				server.SocketError += server_OnSocketError;
			}
			this.Text = "Server - listening";
			server.Start(AddressTest, PortTest);
		}

		private void server_OnSocketError(Server sender, ServerSocketErrorEventArgs eventArgs)
		{
			MessageBox.Show(eventArgs.Exception.Message + Environment.NewLine + eventArgs.Exception.StackTrace);
		}

		void server_OnConnected(Server sender, ServerEventArgs eventArgs)
		{
			HandleNewMsg(Environment.NewLine + eventArgs.Client.Id.ToString() + ": Connected!");
			server.Send(eventArgs.Client.Id, "Hello ClientId " + eventArgs.Client.Id.ToString());
		}

		private void server_OnConnectionClosed(Server sender, ServerEventArgs eventArgs)
		{
			HandleNewMsg(Environment.NewLine + eventArgs.Client.Id.ToString() + ": Disconnected!");
		}

		private void server_OnMessageReceived(Server sender, ServerReceivedEventArgs eventArgs)
		{
			if (eventArgs.Message != null)
			{
				var text = Encoding.UTF8.GetString(eventArgs.Message);
				text = string.Format("From Client Id {0}: {1}", eventArgs.Client.Id, text);
				HandleNewMsg(Environment.NewLine + text);
				if (eventArgs.MessageKind == MessageKind.Message)
				{
					server.SendAll(text);
				}
			}
		}
		#endregion

		private void button5_Click(object sender, EventArgs e)
		{
			if (client != null)
				Task.Factory.StartNew(() => {
					for (int i = 0; i < 5; ++i)
					{
						string val = "";

						for (int j = 0; j < 42; ++j)
						{
							val = string.Format("{0}{1}",  val, j) ;
							client.Send(i.ToString("000") + " " + val);
						}
					}
				},TaskCreationOptions.LongRunning);
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			timer1.Interval = rnd.Next(400, 1500);

			if (chkDisconnect.Checked && client != null && client.IsConnected())
			{
				var reconnect = new byte[1];

				secureRnd.GetBytes(reconnect);

				if (reconnect[0] <= 10)
				{
					client.Close();
					timer1.Enabled = true;
					return;
				}
			}

			if (server == null && client == null || (client != null && !client.IsConnected()))
			{
				HandleClient();
			}

			if(chkData.Checked)
			{
				var msg = new byte[rnd.Next(25, 80)];

				secureRnd.GetBytes(msg);

				Send(DateTime.Now.ToLongTimeString() + " : " + (server != null ? "server" : "client") + " : " + Convert.ToBase64String(msg));
			}


			//timer1.Enabled = false;
		}

		private void button6_Click(object sender, EventArgs e)
		{
			if(client != null)
			{
				MessageBox.Show(client.Id.ToString());
			}
		}

		private void chkTimer_CheckedChanged(object sender, EventArgs e)
		{
			timer1.Enabled = chkTimer.Checked;
		}

		private void button7_Click(object sender, EventArgs e)
		{
			if (client != null)
			{
				client.RequestListOfConnectedCliendId();
			}
		}
	}
}
