using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grabby;

public class HttpServer : MonoBehaviour
{
	[SerializeField] private GameSimulator simulator;
    private HttpListener listener;

    private void Start() {
        string port = CommandLine.GetArg("SERVER_PORT");
        string uri = $"http://*:{port}/grabby/api/game/";
        listener = new HttpListener();
        listener.Prefixes.Add(uri);
        listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
        listener.Start();
        listener.BeginGetContext(new AsyncCallback(OnGetCallback), null);
    }

	async private void OnGetCallback(IAsyncResult result) {
		HttpListenerContext context = listener.EndGetContext(result);
		var response = context.Response;
		var request = context.Request;
		context.Response.Headers.Clear();
		if(request.Url.LocalPath == "/grabby/api/game/getWonLoads") {
			string json;
			using(var reader = new StreamReader(request.InputStream, request.ContentEncoding)) {
				json = reader.ReadToEnd();
			}
			Debug.Log($"json {json}");
			UnityMainThread.wkr.AddJob(async () => {
        		APIMachineControlStateRequest request = JsonUtility.FromJson<APIMachineControlStateRequest>(json);
				GameSimulatorResponse data = await simulator.Simulate(request);
				Debug.Log($"Sending data callback");
				response.SendChunked = false;
				response.StatusCode = 200;
				response.StatusDescription = "OK";
				using(var writer = new StreamWriter(response.OutputStream, response.ContentEncoding)) {
					await writer.WriteAsync(JsonUtility.ToJson(data));
				}
				response.Close();
			});
		}
		if(listener.IsListening) {
			listener.BeginGetContext(new AsyncCallback(OnGetCallback), null);
		}
	}
  
    private void OnApplicationQuit() {
        listener.Stop();
    }
}