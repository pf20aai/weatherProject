using System.Net;
using System.Text;

// todo: extract common elements into functoins
// todo: make more c# like

using var listener = new HttpListener();
listener.Prefixes.Add("http://localhost:80/");

listener.Start();

Console.WriteLine("Listening on port 80...");

while (true)
{
    HttpListenerContext context = listener.GetContext();
    HttpListenerRequest req = context.Request;

    Console.WriteLine($"Received {req.HttpMethod} request to {req.Url.AbsolutePath} from {req.Url}");

    if ((req.HttpMethod == "GET") && (req.Url.AbsolutePath == "/test"))
    {
        Console.WriteLine("Yeah that works");

        using HttpListenerResponse resp = context.Response;
        resp.Headers.Set("Content-Type", "text/plain");

        string data = "Hello there!";
        resp.StatusCode = (int) HttpStatusCode.OK;

        byte[] buffer = Encoding.UTF8.GetBytes(data);

        resp.ContentLength64 = buffer.Length;
        using Stream ros = resp.OutputStream;
        ros.Write(buffer, 0, buffer.Length);

        resp.Close();
    }


    else if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/test"))
    {
        System.IO.Stream body = req.InputStream;
        System.Text.Encoding encoding = req.ContentEncoding;
        System.IO.StreamReader reader = new System.IO.StreamReader(body, encoding);

        string requestData = reader.ReadToEnd();
        body.Close();
        reader.Close();


        Console.WriteLine($"Recieved Data:\n{requestData}");


        using HttpListenerResponse resp = context.Response;
        resp.Headers.Set("Content-Type", "text/plain");

        string responseData = $"Accepted data:\n{requestData}";
        resp.StatusCode = (int)HttpStatusCode.Accepted;

        byte[] buffer = Encoding.UTF8.GetBytes(responseData);
        resp.ContentLength64 = buffer.Length;
        using Stream ros = resp.OutputStream;
        ros.Write(buffer, 0, buffer.Length);

        resp.Close();
    }

    else
    {
        Console.WriteLine("Unknown path");

        using HttpListenerResponse resp = context.Response;
        resp.Headers.Set("Content-Type", "text/plain");

        string data = "Not Found";
        resp.StatusCode= (int) HttpStatusCode.NotFound;

        byte[] buffer = Encoding.UTF8.GetBytes(data);
        resp.ContentLength64 = buffer.Length;
        using Stream ros = resp.OutputStream;

        ros.Write(buffer, 0, buffer.Length);
        resp.Close();

    }

    
}