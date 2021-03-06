using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class DataStreamCheck : MonoBehaviour {

	private float num_of_UDP_vals;
	private float camera_translation_x_labview;
	private float camera_translation_y_labview;
	private float camera_translation_z_labview;

	// Receiving Thread
	Thread receiveThread;
	UdpClient client;
	// Initialize udp variables
	public string text = ""; //received text
	public string IP = "192.168.61.73";
	public int port; // Defined on "init 
	public string lastReceivedUDPPacket = "";
	private float timeCounter = 0;

	public GameObject MovingObject;

	public bool generateInternalUnityMovement;

	public float float_x;
	public float float_y;
	public float float_z;

	/// //////////////////////////////////////////////////////////////////////////////////////////////	///

	void Start () {
		// Find the Camera within omnity script (takes a while to load so may not grab it here)
		MovingObject = GameObject.Find ("SphereAnchor");
		////////////////////////////////////////////////////////////////////////////////////
		/// 

		// // // // // // // // UDP SETUP // // // // // // // //// // // // // // // //// // // // // // // //
		port = 6843;
		receiveThread = new Thread(
			new ThreadStart(ReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start();
		///////////////////////////////////////////////////////////////////////////////////////////////////////// 
	}

	// // // // // // // // RECEIVE DATA FUNCTION for UDP SETUP // // // // // // // //// // // // // // // //
	private void ReceiveData(){
		client = new UdpClient(port);
		while (true){
			try{
				IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = client.Receive(ref anyIP);			
				text = Encoding.UTF8.GetString(data);				
				lastReceivedUDPPacket = text;
			}
			catch (Exception err){
				print(err.ToString());
			}
		}
	}
	/// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	// Update is called once per frame
	void Update () {
		// Only runs if CameraObject is empty
		if (MovingObject == null){
			MovingObject = GameObject.Find("SphereAnchor");
		}


		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		if (generateInternalUnityMovement == true) {
			timeCounter += (Time.deltaTime) * (float)2 * (float)3.14;
			float_x = Mathf.Cos (timeCounter);
			float_y = 0;
			float_z = Mathf.Sin (timeCounter);
		}
		if (generateInternalUnityMovement == false){
			// // // // // // // // READ UDP STRING // // // // // // // //// // // // // // // //// // // // // // // //// // 
			// reads string - splits the characters based on "," - assigns the characters to the variables based on the order within the string
			char[] delimiter1 = new char[] { ',' };
			var strvalues = lastReceivedUDPPacket.Split (delimiter1, StringSplitOptions.None);
		
			foreach (string word in strvalues) {
				num_of_UDP_vals++;
				if (num_of_UDP_vals == 1) {
					float.TryParse (word, out float_x);
				}
				if (num_of_UDP_vals == 2) {
					float.TryParse (word, out float_z);
				}
			}
			num_of_UDP_vals = 0;

		}

		///// // /// /////////////// ///////////////////////////////////////
		/// 


		MovingObject.transform.position = new Vector3 (float_x, float_z, float_y);

	
	}

		// Necessary in order to properly close udp connection
		void OnDisable(){
			if (receiveThread != null)
				receiveThread.Abort();
			client.Close();
		}

}
