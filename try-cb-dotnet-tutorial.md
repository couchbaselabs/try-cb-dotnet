# .NET SDK Tutorial

The .NET SDK tutorial bridges the gap between simple and advanced concepts by walking through a complete web application.

The full source code for the tutorial is available on [GitHub]().

The primary focus of the tutorial is to explain the function and theory behind the Couchbase .NET client and how it works together with Couchbase Server, and especially the new features in Couchbase Server version 4.0 like N1QL. 

This tutorial makes use of the travel-sample data set that comes with Couchbase Server 4.0. 

The HTML/JS code that generates the web application is provided with the source code but it is not the scope of this tutorial to explain any details of the implementation. 

## Prerequisites and set up
You will need to have the following available/installed:

* Visual Studio 2015 or newer (The source code is created using VS 2015 Professional)
* Windows 8.1 or higher (to be able to install and run Visual Studio 2015)
* Although not a requirement, we recommend you have a Git client for easy source code browsing and making it easy to switch between branches (tutorial steps are spil using branches)
* That's it, your ready to start.

## Installing Couchbase Server 4.0
First things first... we need to install Couchbase Server! You can chose to install it locally on your developer machine or remotely, in this tutorial we will assume that Couchbase Server 4.0 is installed locally on the developer machine along side the web site that we will create.

 and for the rest  
Download Couchbase Server and install it. As you follow the download instructions and setup wizard, make sure you keep all the services (data, query, and index) selected. Make sure aolso to install the sample bucket named travel-sample (introduced in CB 4.0) because it contains the data used in this tutorial.

>TIP: 
	If you already have Couchbase Server installed but did not install the travel-	sample bucket!
	* , open the Couchbase Web Console and select Settings > Sample 	Buckets. 
	Select the travel-sample check box, and then click Create. A 	notification box in the upper-right corner show the progress and disappears when 	the bucket is ready to use.
	
## Getting ready
### Understanding the source repo
The source code is split up into branches, every branch represents a step in the tutorial. Every step (branch) builds on the previous and the final result is in the `master` branch.

* `tutorial-part-1` is the most simple skeleton that can compile and show a UI. But it's not possible to navigate the app yet.
* `tutorial-part-2` is the result of part 1 and returns static content to allow the user to browse the site. But it returns only static data.
* `tutorial-part-3` is the result of part 2 and adds queries and live data to the site. It's now possible to navigate the site and get actual data back served from Couchbase Server 4.0.
* `tutorial-part-4` is the result of part 3 and adds user login and password storage to the site.
* `tutorial-part-5`is the result of part 4 and shows a few ekstra options in the Couchbase .NET SDK like LINQ support.
* `master` is the final result after refactoring part 5.

###A quick note on the source it self
This source code is split up in two parts, HTML/JS and .NET code.
The HTML and Javascript part is actually a static set of files that use Javascript to call a API methods implemented in on the backend. This approach allow for a very clean and elegant seperation of the UI and the data part. 
In fact this allows us to change the backend without touching the front end code. Therefore if you take a look a the java version `try-cb-java` you quickly learn that the UI is the same it's only the backend that changes.

In the .NET implementation of the backend we use WEB API, as this is the most flexible and easy way to implement an API in .NET.  


###Get set up for the tutorial 
To get propper setup for the tutorial and get ready for the first part, follow these steps:

* git clone https://github.com/couchbaselabs/try-cb-dotnet.git or download the source
* If you did not install Couchbase Server on `localhost` and want to connect to a remote Couchbase Server, change the `couchbaseServer` key in `web.config`. You can also change the username and password for Couchbase Server.
* Use Visual Studio 2015 to open the solution file `try-cb-dotnet.sln` in `try-cb-dotnet/src`
* The solution is configured to restore all missing `nuget`packages on every build. Therefore the only thing missing now is to build and run the solution.

>Note:
	Restoring the missing nuget packages can take some time and is also influenced 	by your network speed.

## Tutorial step 1 - 5
 
### Step 1 - Understand WEB API and .NET 
> Part 1 of the tutorial is not using Couchbase Server, it's only included in the tutorial to show how web api works and to emphasise that we are working with JSON. The only task of the backend API is to return JSON. This is an important understanding that will help you not only with this tutorial but whenever working with an API.
 
> Implementing the API is al about returning the right JSON. Couchbase stores all it's documents in JSON and therefore is a very good match for an API that returns JSON.

>Feel free to skip this step and go directly to step 2, to start learning how to use Couchbase and the Couchbase SDK in your .NET Apps.

In this step you will update all WEB API methods to return static JSON (string values). This will allow you to run and browse the web application and get an understanding of how the code is organised.

####Step 1.1 

**Where:** `UserController.cs` -> **method:** `Login(string password, string user)`

**Goals:** Return static JSON to learn how WEB API works.

**Relevant Documentation Topics:** [ASP.NET WEB API 2](http://www.asp.net/web-api/overview/getting-started-with-aspnet-web-api/tutorial-your-first-web-api)

**Task:**

This is a Web API call, a method that is called from the static html (index.html).The JavaScript in the static html expects this "Login" web api call to return a "success" status code containing a JWT token. 

The JWT token is used to reference and store data about the user's trips/bookings and login credentials.

The response should be in a JSON format like this:
   `[{"success":"eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VyIjoiZ3Vlc3QiLCJpYXQiOjE0NDE4Njk5NTR9.5jPBtqralE3W3LPtS - j3MClTjwP9ggXSCDt3 - zZOoKU"}]`Implement the method to return a "success" allowing the user to login.
Later we will implement a JWT token issuer and store user data in Couchbase for later look-up.The token is created for the user:>Note: The login credentials for this JWT token is:
>
>username: guest
>
>password: guest

**Solution:**

Update the Login method to return the JWT token value:

		[HttpGet]
        [ActionName("Login")]
        public object Login(string password, string user)
        {
       
            return new { 
            	success = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VyIjoiZ3Vlc3QiLCJpYXQiOjE0NDE4Njk5NTR9.5jPBtqralE3W3LPtS - j3MClTjwP9ggXSCDt3 - zZOoKU" 
            	};
        }
        
####Step 1.2

**Where:** `UserController.cs` -> **method:** `Login([FromBody] UserModel user)`

**Goals:** Return static JSON to learn how WEB API works.

**Relevant Documentation Topics:** [ASP.NET WEB API 2](http://www.asp.net/web-api/overview/getting-started-with-aspnet-web-api/tutorial-your-first-web-api)


**Task:**
This is a Web API call, a method that is called from the static html (index.html).
The JS in the static html expectes this "Login" web api call to return a
"success" status code containing a JWT token. 

The JWT token is used to reference and store data about the user's trips/booking and login credentials.
Response should be in a json format like this:
Round trip: 

`[{"success":"eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VyIjoiZ3Vlc3QiLCJpYXQiOjE0NDE4Njk5NTR9.5jPBtqralE3W3LPtS - j3MClTjwP9ggXSCDt3 - zZOoKU"}]`

Implement the method to return a "success" faking the creation of a new user and allowing the user to login.

>Later we will implement a JWT token issuer and store user data in Couchbase for later look-up.
       
>The token is created for the user:
>
>username: guest
>
>passowrd: guest

**Solution:**

	[HttpPost]
	[ActionName("Login")]
    public object CreateLogin([FromBody] UserModel user)
    {        
            return new { success = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VyIjoiZ3Vlc3QiLCJpYXQiOjE0NDE4Njk5NTR9.5jPBtqralE3W3LPtS - j3MClTjwP9ggXSCDt3 - zZOoKU" };
    }

####Step 1.3

**Where:** `UserController.cs` -> **method:** `Flights(string token)`

**Goals:** Return static JSON to learn how WEB API works.

**Relevant Documentation Topics:** [ASP.NET WEB API 2](http://www.asp.net/web-api/overview/getting-started-with-aspnet-web-api/tutorial-your-first-web-api)

**Task:**

This is a Web API call, a method that is called from the static html (index.html).
The js in the static html expects this "flights" web api call to return a
all bookings done by this user. 

The JWT token is used to look-up the user and find all bookings.

In this fake implementation we are not going to use the Token, but instead return a static list of bookings.

Response should be in a json format like this:

Bookings:
`[{"_type":"Flight","_id":"d500a3d1-2cca-43a5-8a66-f11828a35969","name":"American Airlines","flight":"AA344","date":"09/10/2015","sourceairport":"SFO","destinationairport":"LAX","bookedon":"1441881827622"},{"_type":"Flight","_id":"bf676b0d-e63b-4ff6-aade-7ac1c182b3de","name":"American Airlines","flight":"AA787","date":"09/11/2015","sourceairport":"LAX","destinationairport":"SFO","bookedon":"1441881827623"},{"_type":"Flight","_id":"f0099c24-3ad4-482e-8352-704f9cbf1a43","name":"American Airlines","flight":"AA550","date":"09/10/2015","sourceairport":"SFO","destinationairport":"LAX","bookedon":"1441881827623"}]`
            
Implement the method to return the fake "bookings" for the guest user.
Later we will look-up bookings with the JWT token, but for now a static list is what we need.

>Hint: 
>
>return the same booking multiple times in a list, re-using the sample JSON above.

**Solution:**

	[HttpGet]
   	[ActionName("flights")]
    public object Flights(string token)
    {
           
    	return new List<dynamic>
    	{
    		new {
    		_type="Flight",_id="f0099c24-3ad4-482e-8352-704f9cbf1a43",name="American Airlines",flight="AA550",date="09/10/2015",sourceairport="SFO",destinationairport="LAX",bookedon=1441881827623},
                new {_type="Flight",_id="f0099c24-3ad4-482e-8352-704f9cbf1a43",name="American Airlines",flight="AA550",date="09/10/2015",sourceairport="SFO",destinationairport="LAX",bookedon=1441881827623},
                new {_type="Flight",_id="f0099c24-3ad4-482e-8352-704f9cbf1a43",name="American Airlines",flight="AA550",date="09/10/2015",sourceairport="SFO",destinationairport="LAX",bookedon=1441881827623},
            };
        }

####Step 1.4

**Where:** `UserController.cs` -> **method:** `BookFlights([FromBody] dynamic request)`

**Goals:** Return static JSON to learn how WEB API works.

**Relevant Documentation Topics:** [ASP.NET WEB API 2](http://www.asp.net/web-api/overview/getting-started-with-aspnet-web-api/tutorial-your-first-web-api)

**Task:**    
This is a Web API call, a method that is called from the static html (index.html).
The JS in the static html expects this "flights" web api call to save the selected flight in a booking's document.
 
The JWT token is used as a key to the users bookings.
In this fake implementation we are not going to use the Token, nor store any data about the bookings.
Instead we return a static value to indicate that the bokking was successfull.
Response should be in a JSON format like this:

Bookings:

`{"added":3}`
 
Implement the method to return the fake "booking success" for the guest user.
Later we will store bookings using the JWT token, but for now a static response is what we need.

**Solution:**

	[HttpPost]
    [ActionName("flights")]
    public object BookFlights([FromBody] dynamic request)
    {
 		return new { added = 3 };
    }

####Step 1.5

**Where:** `FlightPathController.cs` -> **method:** `FindAll(string from, DateTime leave, string to, string token)`

**Goals:** Return static JSON to learn how WEB API works.

**Relevant Documentation Topics:** [ASP.NET WEB API 2](http://www.asp.net/web-api/overview/getting-started-with-aspnet-web-api/tutorial-your-first-web-api)

**Task:**  

Task:
This is a Web API call, a method that is called from the static html (index.html).
The JS in the static html expects this "findAll" web api call to return a "trip" in a JSON format like this:

Round trip: 
`[{"destinationairport":"SFO","equipment":"738","flight":"AA907","id":5746,"name":"American Airlines","sourceairport":"LAX","utc":"00:29:00","flighttime":1,"price":53},{"destinationairport":"SFO","equipment":"738","flight":"AA787","id":5746,"name":"American Airlines","sourceairport":"LAX","utc":"19:06:00","flighttime":1,"price":45},{"destinationairport":"SFO","equipment":"738","flight":"AA279","id":5746,"name":"American Airlines","sourceairport":"LAX","utc":"04:54:00","flighttime":1,"price":52},{"destinationairport":"SFO","equipment":"E75","flight":"DL856","id":21085,"name":"Delta Air Lines","sourceairport":"LAX","utc":"20:08:00","flighttime":1,"price":47},{"destinationairport":"SFO","equipment":"E75","flight":"DL273","id":21085,"name":"Delta Air Lines","sourceairport":"LAX","utc":"14:14:00","flighttime":1,"price":48},{"destinationairport":"SFO","equipment":"73W 73C 733","flight":"WN543","id":63986,"name":"Southwest Airlines","sourceairport":"LAX","utc":"22:16:00","flighttime":1,"price":44},{"destinationairport":"SFO","equipment":"73W 73C 733","flight":"WN828","id":63986,"name":"Southwest Airlines","sourceairport":"LAX","utc":"04:35:00","flighttime":1,"price":43},{"destinationairport":"SFO","equipment":"738","flight":"US086","id":59532,"name":"US Airways","sourceairport":"LAX","utc":"15:06:00","flighttime":1,"price":46},{"destinationairport":"SFO","equipment":"738","flight":"US150","id":59532,"name":"US Airways","sourceairport":"LAX","utc":"15:44:00","flighttime":1,"price":47},{"destinationairport":"SFO","equipment":"738","flight":"US437","id":59532,"name":"US Airways","sourceairport":"LAX","utc":"23:42:00","flighttime":1,"price":52},{"destinationairport":"SFO","equipment":"739 752 753 319 320 738","flight":"UA666","id":57010,"name":"United Airlines","sourceairport":"LAX","utc":"05:11:00","flighttime":1,"price":44},{"destinationairport":"SFO","equipment":"739 752 753 319 320 738","flight":"UA978","id":57010,"name":"United Airlines","sourceairport":"LAX","utc":"19:50:00","flighttime":1,"price":53},{"destinationairport":"SFO","equipment":"739 752 753 319 320 738","flight":"UA123","id":57010,"name":"United Airlines","sourceairport":"LAX","utc":"21:13:00","flighttime":1,"price":49},{"destinationairport":"SFO","equipment":"320 319","flight":"VX929","id":62018,"name":"Virgin America","sourceairport":"LAX","utc":"00:39:00","flighttime":1,"price":49},{"destinationairport":"SFO","equipment":"320 319","flight":"VX351","id":62018,"name":"Virgin America","sourceairport":"LAX","utc":"01:37:00","flighttime":1,"price":49},{"destinationairport":"SFO","equipment":"320 319","flight":"VX703","id":62018,"name":"Virgin America","sourceairport":"LAX","utc":"05:01:00","flighttime":1,"price":47},{"destinationairport":"SFO","equipment":"320 319","flight":"VX743","id":62018,"name":"Virgin America","sourceairport":"LAX","utc":"10:36:00","flighttime":1,"price":53},{"destinationairport":"SFO","equipment":"320 319","flight":"VX301","id":62018,"name":"Virgin America","sourceairport":"LAX","utc":"01:32:00","flighttime":1,"price":49}]`
            
One way trip:
`[{"destinationairport":"SFO","equipment":"738","flight":"AA787","id":5746,"name":"American Airlines","sourceairport":"LAX","utc":"19:06:00","flighttime":1,"price":48},{"destinationairport":"SFO","equipment":"738","flight":"AA279","id":5746,"name":"American Airlines","sourceairport":"LAX","utc":"04:54:00","flighttime":1,"price":49},{"destinationairport":"SFO","equipment":"738","flight":"AA907","id":5746,"name":"American Airlines","sourceairport":"LAX","utc":"00:29:00","flighttime":1,"price":51},{"destinationairport":"SFO","equipment":"E75","flight":"DL273","id":21085,"name":"Delta Air Lines","sourceairport":"LAX","utc":"14:14:00","flighttime":1,"price":51},{"destinationairport":"SFO","equipment":"E75","flight":"DL856","id":21085,"name":"Delta Air Lines","sourceairport":"LAX","utc":"20:08:00","flighttime":1,"price":53},{"destinationairport":"SFO","equipment":"73W 73C 733","flight":"WN543","id":63986,"name":"Southwest Airlines","sourceairport":"LAX","utc":"22:16:00","flighttime":1,"price":50},{"destinationairport":"SFO","equipment":"73W 73C 733","flight":"WN828","id":63986,"name":"Southwest Airlines","sourceairport":"LAX","utc":"04:35:00","flighttime":1,"price":53},{"destinationairport":"SFO","equipment":"738","flight":"US086","id":59532,"name":"US Airways","sourceairport":"LAX","utc":"15:06:00","flighttime":1,"price":53},{"destinationairport":"SFO","equipment":"738","flight":"US150","id":59532,"name":"US Airways","sourceairport":"LAX","utc":"15:44:00","flighttime":1,"price":43},{"destinationairport":"SFO","equipment":"738","flight":"US437","id":59532,"name":"US Airways","sourceairport":"LAX","utc":"23:42:00","flighttime":1,"price":48},{"destinationairport":"SFO","equipment":"739 752 753 319 320 738","flight":"UA978","id":57010,"name":"United Airlines","sourceairport":"LAX","utc":"19:50:00","flighttime":1,"price":48},{"destinationairport":"SFO","equipment":"739 752 753 319 320 738","flight":"UA666","id":57010,"name":"United Airlines","sourceairport":"LAX","utc":"05:11:00","flighttime":1,"price":51},{"destinationairport":"SFO","equipment":"739 752 753 319 320 738","flight":"UA123","id":57010,"name":"United Airlines","sourceairport":"LAX","utc":"21:13:00","flighttime":1,"price":50},{"destinationairport":"SFO","equipment":"320 319","flight":"VX743","id":62018,"name":"Virgin America","sourceairport":"LAX","utc":"10:36:00","flighttime":1,"price":48},{"destinationairport":"SFO","equipment":"320 319","flight":"VX703","id":62018,"name":"Virgin America","sourceairport":"LAX","utc":"05:01:00","flighttime":1,"price":45},{"destinationairport":"SFO","equipment":"320 319","flight":"VX301","id":62018,"name":"Virgin America","sourceairport":"LAX","utc":"01:32:00","flighttime":1,"price":49},{"destinationairport":"SFO","equipment":"320 319","flight":"VX929","id":62018,"name":"Virgin America","sourceairport":"LAX","utc":"00:39:00","flighttime":1,"price":44},{"destinationairport":"SFO","equipment":"320 319","flight":"VX351","id":62018,"name":"Virgin America","sourceairport":"LAX","utc":"01:37:00","flighttime":1,"price":46}]`

Implement the method to return a "Round trip" meaning a destination and source airport.
Later we will use Couchbase to do the look-up but for now a "constant" is returned.          
            
**Solution:**    

	[HttpGet]
    [ActionName("findAll")]
    public object FindAll(string from, DateTime leave, string to, string token)
    {
    	return new List<dynamic>
        {
        	new { destinationairport="SFO",equipment=738,flight="AA907",id=5746,name="American Airlines",sourceairport="LAX",utc="00:29:00",flighttime=1,price=53},
        	new { destinationairport="SFO",equipment=738,flight="AA907",id=5746,name="American Airlines",sourceairport="LAX",utc="00:29:00",flighttime=1,price=53},
        	new { destinationairport="SFO",equipment=738,flight="AA907",id=5746,name="American Airlines",sourceairport="LAX",utc="00:29:00",flighttime=1,price=53}
            };
        }

####Step 1.6

**Where:** `AirportController.cs` -> **method:** `FindAll(string search, string token)`

**Goals:** Return static JSON to learn how WEB API works.

**Relevant Documentation Topics:** [ASP.NET WEB API 2](http://www.asp.net/web-api/overview/getting-started-with-aspnet-web-api/tutorial-your-first-web-api)

**Task:**    

This is a Web API call, a method that is called from the static html (index.html).
The JS in the static html expectes this "findAll" web api call to return a
"airportname" in a JSON format like this:

`[{"airportname":"San Francisco Intl"}]`

Implement the method to return a single airport name.
Later we will use Couchbase to do the look-up but for now a "constant" is returned.

**Solution:**

	[HttpGet]
   	[ActionName("findAll")]
    public object FindAll(string search, string token)
    {
        return new List<dynamic>()
        {
            new {airportname = "San Francisco Intl"}
        };
    }
### Step 1 - Summery
You should now be able to run and browse the application in your browser. All data is static but never the less it "works". In Step 2 we will update the static JSON returned to actual data.    

### Step 2 - Understand Couchbase, Couchbase .NET SDK & N1QL
In this step we will update all WEB API methods to return data from Couchbase. This is the first step that uses Couchbase and therefore we need to add references to the Couchbase Client and LINQ extensions.

####Step 2.0 - Referencing & Bootstrapping the Couchbase Client.

**Where:** `Solution` (this is a solution wide update)

**Goals:** Add a reference to: [CouchbaseNetClient](https://www.nuget.org/packages/CouchbaseNetClient) and [Linq2Couchbase](https://www.nuget.org/packages/Linq2Couchbase) the later is the LINQ to N1QL extensions. 
When the references are in place we need to bootstrap the Couchbase SDK and make it globally available in the solution/web application.

**Relevant Documentation Topics:** 

* [N1QL intro](http://developer.couchbase.com/documentation/server/4.0/n1ql/n1ql-intro/data-access-using-n1ql.html)
* [Couchbase .NET Client - github](https://github.com/couchbase/couchbase-net-client)
* [Couchbase .NET Client - docs](http://developer.couchbase.com/documentation/server/4.0/sdks/dotnet-2.2/getting-started.html)
* [Linq2Couchbase - github](https://github.com/couchbaselabs/Linq2Couchbase)
* [Hello World - Couchbase .NET](http://developer.couchbase.com/documentation/server/4.0/sdks/dotnet-2.2/hello-couchbase.html)

**Task (Add references):**
For every release, we package the binaries and store the latest version in NuGet. If you are not familiar with NuGet, it’s the official and most widely supported package manager for Microsoft Visual Studio and .NET in general. NuGet is a centralized repository for package authors and consumers, and it also defines a suite of tools for authoring and consuming packages.

Using Visual Studio 2015 or later, follow these steps to get started with the Couchbase .NET SDK:

1. From the IDE, right-click the solution/project to which you want to add the dependency.
2. In the context menu, click Manage NuGet Packages. The NuGet package manager modal dialog opens.
3. In the search box at the top right-hand side of the dialog, type CouchbaseNetClient and then press enter on your keyboard.
4. In the search results, select the `CouchbaseNetClient` package and then click Install.
5. Repeat step 3-4 to install `Linq2Couchbase`.

That’s it! NuGet will pull in all required dependencies and reference them. 

**Task (Bootstrap):**
Before we can start using the Couchbase SDK it needs to be initialised and configured to point to the right cluster and buckets.

Bootstrapping is the process for initialising and configuring the Couchbase Client for use in the application.

The Couchbase Client includes a helper class called `ClusterHelper`. This class is a singleton that can be shared globally in the application and should always be kept alive for the lifetime of the application. 

The application we use is a web application and therefore it's most convenient to initialise the Couchbase Client in `Global.asax.cs` as this is run on application start.

1. Create a new file in the folder `App_Start` called `CouchbaseConfig.cs`.
2. Replace the content of `CouchbaseConfig.cs` with this code snippet:

		using System;
		using System.Collections.Generic;
		using System.Linq;
		using System.Text;
		using System.Threading.Tasks;
		using Couchbase;
		using Couchbase.Configuration.Client;

		namespace try_cb_dotnet.App_Start
		{
    		public static class CouchbaseConfig
    		{
        		public static void Initialize()
        		{
            		var config = new ClientConfiguration();
            		config.BucketConfigs.Clear();

            		config.Servers = new List<Uri>(new Uri[] { new Uri(CouchbaseConfigHelper.Instance.Server) });

            		config.BucketConfigs.Add(
                		CouchbaseConfigHelper.Instance.Bucket,
	                new BucketConfiguration
	                {
	                    BucketName = CouchbaseConfigHelper.Instance.Bucket,
	                    Username = CouchbaseConfigHelper.Instance.User,
	                    Password = CouchbaseConfigHelper.Instance.Password
	                });

            		config.BucketConfigs.Add(
                	"default",
                	new BucketConfiguration
                	{
                    	BucketName = "default",
                    	Username = CouchbaseConfigHelper.Instance.User,
                    	Password = CouchbaseConfigHelper.Instance.Password
                	});

            		ClusterHelper.Initialize(config);
        		}

		       public static void Close()
		       {
		           ClusterHelper.Close();
		       }
      		}
		}   
3. The class `CouchbaseConfig` references a class called `CouchbaseConfigHelper`, the purpose of this class is to wrap calls to read `web.config` for application configurations and settings.
4. In the project root create a new code files called: `CouchbaseConfigHelper.cs`.
5. Replace the content of `CouchbaseConfigHelper.cs` with:

		using System;
		using System.Collections.Generic;
		using System.Configuration;
		using System.Linq;
		using System.Web;
		
		namespace try_cb_dotnet
		{
		    public class CouchbaseConfigHelper
		    {
		        public CouchbaseConfigHelper()
		        {
		        }
		
		        private static CouchbaseConfigHelper instance = null;
		        public static CouchbaseConfigHelper Instance
		        {
		            get { if (instance == null) { instance = new CouchbaseConfigHelper(); } return instance; }
		        }
		
		        public string Bucket
		        {
		            get
		            {
		                return ConfigurationManager.AppSettings["couchbaseBucketName"];
		            }
		        }
		
		        public string Server
		        {
		            get
		            {
		                return ConfigurationManager.AppSettings["couchbaseServer"];
		            }
		        }
		
		        public string Password
		        {
		            get
		            {
		                return ConfigurationManager.AppSettings["couchbasePassword"];
		            }
		        }
		
		        public string User
		        {
		            get
		            {
		                return ConfigurationManager.AppSettings["couchbaseUser"];
		            }
		        }
		    }
		}
6. Reading the code in `CouchbaseConfigHelper` reveals that it's referencing a bunch of application setting keys in `web.config`, that we have still to create.
7. Open `web.config` and add the missing application setting keys:

		<configuration>
		  <configSections>
		    ...
		  </configSections>
		  <connectionStrings>
		    ....
		  </connectionStrings>
		  <appSettings>
		    <add key="webpages:Version" value="3.0.0.0" />
		    <add key="webpages:Enabled" value="false" />
		    <add key="ClientValidationEnabled" value="true" />
		    <add key="UnobtrusiveJavaScriptEnabled" value="true" />
		    <!-- COUCHBASE TRAVEL SAMPLE SETTINGS -->
		    <add key="couchbaseBucketName" value="travel-sample" />
		    <add key="couchbaseServer" value="http://localhost:8091" />
		    <add key="couchbasePassword" value="" />
		    <add key="couchbaseUser" value="" />
		    <!--END -->
		  </appSettings>
		  ...
	  	</configuration>

8. Change the settings in `web.config` to reflect your actual Couchbase setup. Add username and password if appropriate and correct the cluster url if needed.
9. Open the file `Global.asax.cs` 
10. Update the method `Application_Start()` with a call to `CouchbaseConfig.Initialize();` to initialise the Couchbasebase Client.

		protected void Application_Start()
    	{
	        // Initialize Couchbase & ClusterHelper
	        CouchbaseConfig.Initialize();
	
	        AreaRegistration.RegisterAllAreas();
	        GlobalConfiguration.Configure(WebApiConfig.Register);
	        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
	        RouteConfig.RegisterRoutes(RouteTable.Routes);
	        BundleConfig.RegisterBundles(BundleTable.Bundles);
	    }
11. The only thing missing now is disposing of resources when the application stops.
12. Update the `Application_End()` method to call the `Close()` method on `CouchbaseConfig`

		protected void Application_End()
		{
			CouchbaseConfig.Close();
		}
		
You're ready to start coding!

####Step 2.1 

**Where:** `.cs` -> **method:** `method`

**Goals:** Return live data from the Travel Sample data bucket in Couchbase Server 4.0 and learn more about how to use Couchbase with .NET

**Relevant Documentation Topics:** 

* [Linq2Couchbase - github](https://github.com/couchbaselabs/Linq2Couchbase)
* [Hello World - Couchbase .NET](http://developer.couchbase.com/documentation/server/4.0/sdks/dotnet-2.2/hello-couchbase.html)

**Task:**

**Solution:**
    