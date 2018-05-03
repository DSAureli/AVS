using AVS.Properties;
using Microsoft.Win32;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AVS
{
	public class AVS : Form, IMMNotificationClient
	{
		[MTAThread]
		public static void Main()
		{
			var mutex = new System.Threading.Mutex(true, "AVS", out bool result);

			if (!result)
			{
				MessageBox.Show("Another instance of AVS is already running", "AVS - Auto Volume Switcher", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			Application.Run(new AVS());
			GC.KeepAlive(mutex);
		}

		RegistryKey dsKey = null;

		static MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
		static MMDevice device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

		static bool enabled = true;
		static bool isHeadphoneMode = false;

		static Icon[] icons = { Resources.IconS, Resources.IconH };
		static String[] actions = { "Enable", "Disable" };

		static NotifyIcon trayIcon;
		static ContextMenu trayMenu;

		void Error(string error = "No valid registry key found")
		{
			MessageBox.Show(error, "AVS - Auto Volume Switcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
			throw new Exception(error);
		}

		void SaveConfig()
		{
			int currVol = (int)(device.AudioEndpointVolume.MasterVolumeLevelScalar * 100);

			if (isHeadphoneMode)
				Settings.Default.SpeakersVol = currVol;
			else
				Settings.Default.HeadphonesVol = currVol;

			Settings.Default.Save();
		}

		new void Update()
		{
			if (!enabled || isHeadphoneMode == (isHeadphoneMode = (int)dsKey.GetValue("SpeakerMode") == 0))
				return;

			SaveConfig();

			var vol = isHeadphoneMode ? Settings.Default.HeadphonesVol : Settings.Default.SpeakersVol;
			vol = vol < 0 ? 0 : (vol > 100 ? 100 : vol);
			device.AudioEndpointVolume.MasterVolumeLevelScalar = vol / 100.0f;

			trayIcon.Icon = icons[Convert.ToInt16(isHeadphoneMode)];
		}

		public AVS()
		{
			RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\{4d36e96c-e325-11ce-bfc1-08002be10318}", false);

			if (regKey == null)
				Error();

			foreach (var subKeyName in regKey.GetSubKeyNames())
			{
				using (var subKey = regKey.OpenSubKey(subKeyName, false))
				{
					if ((dsKey = subKey.OpenSubKey("DriverSettings")) != null)
						break;
				}
			}

			if (dsKey == null)
				Error();

			enumerator.RegisterEndpointNotificationCallback(this);

			trayMenu = new ContextMenu(new[]
			{
				new MenuItem("Info", (object sender, EventArgs e) =>
				{
					MessageBox.Show("Automagically changes master volume when headphones are\n" +
									"plugged into the front panel of the Creative Sound Blaster Z.\n\n" +
									"Coded by DSAureli - 2016-2018",
									"AVS - Auto Volume Switcher", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}),
				new MenuItem("-"),
				new MenuItem("Disable", (object sender, EventArgs e) =>
				{
					enabled = !enabled;
					trayMenu.MenuItems[2].Text = actions[Convert.ToInt16(enabled)];
				}),
				new MenuItem("Exit", (object sender, EventArgs e) => Application.Exit())
			});

			trayIcon = new NotifyIcon
			{
				Text = "AVS",
				Icon = icons[Convert.ToInt16(isHeadphoneMode)]
			};

			trayIcon.ContextMenu = trayMenu;
			trayIcon.Visible = true;

			Update();
		}

		protected override void OnLoad(EventArgs e)
		{
			Visible = false;
			ShowInTaskbar = false;

			base.OnLoad(e);
		}

		protected override void Dispose(bool isDisposing)
		{
			if (isDisposing)
				trayIcon.Dispose();

			isHeadphoneMode = !isHeadphoneMode;
			SaveConfig();

			base.Dispose(isDisposing);
		}

		void IMMNotificationClient.OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key) => Update();
		void IMMNotificationClient.OnDeviceStateChanged(string deviceId, DeviceState newState) { }
		void IMMNotificationClient.OnDeviceAdded(string pwstrDeviceId) { }
		void IMMNotificationClient.OnDeviceRemoved(string deviceId) { }
		void IMMNotificationClient.OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId) { }
	}
}