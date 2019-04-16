<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Net</Namespace>
</Query>

void Main()
{
	var filePath = @"C:\Users\gsuyama\Desktop\TelogFiles";
	var filesQuantity = 300;
	CreateFiles(filePath, filesQuantity);
}

public static int minimumSizeFile = 26288384;
public static int maximumSizeFile = 38288384;

public static void CreateFiles(string path, int filesQuantity) 
{
	var filesList = new List<string>();
	for (int i = 1; i <= filesQuantity; i++)
	{
		var time = DateTime.Now.ToString("ddMMyyyy_HHmm");
		var fileName = $"file_{i}_{time}.txt";
		var fullFileName = Path.Combine(path, fileName);
		
		using(var fileStream = new StreamWriter(fullFileName))
		{
			filesList.Add(fullFileName);
		}
	}
	FillFiles(filesList);
}

public static void FillFiles(List<string> files)
{
	var resquestFillFiles = files.Select(FillFilesAsync);
	
	try
	{
		Task.WhenAll(resquestFillFiles).Wait();
	}
	catch(Exception ex)
	{
		Console.WriteLine(ex);
	}
}

public static async Task FillFilesAsync(string fullFileName)
{
	var loremIpsum = LoremIpsum.Instance.Content;
	var random = new Random();
	var sw = new StreamWriter(fullFileName);
	await sw.WriteAsync(loremIpsum);
	while(sw.BaseStream.Length < random.Next(minimumSizeFile, maximumSizeFile))
	{
		await sw.WriteAsync(loremIpsum);
	}
}

public static string RequestLoremIpsum()
{
	var request = WebRequest.CreateHttp("https://loripsum.net/api").GetResponse().GetResponseStream();
	string response;
	using (var sw = new StreamReader(request))
	{
		response = sw.ReadToEnd();
		sw.Close();
	}
	return response;
}

public class LoremIpsum
{
	public static LoremIpsum Instance = new LoremIpsum();
	public string Content {get;set;}
	
	private LoremIpsum()
	{
		Content = RequestLoremIpsum();
	}
}
// Define other methods and classes here