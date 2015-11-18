# .NET SDK Tutorial

The .NET SDK tutorial bridges the gap between simple and advanced concepts by walking through a complete web application.

The full source code for the tutorial is available on [GitHub]().

The primary focus of the tutorial is to explain the function and theory behind the Couchbase .NET client and how it works together with Couchbase Server, and especially the new features in Couchbase Server version 4.0 like N1QL. 

This tutorial makes use of the travel-sample data set that comes with Couchbase Server 4.0. 

The HTML/JS code that generates the web application is provided with the source code but it is not the scope of this tutorial to explain any details of the implementation. 

![Application Screen shot](content/images/Screen Shot 2015-11-05 at 11.49.06.png)

## Prerequisites and setup
You will need to have the following available/installed:

* [Visual Studio 2015](https://www.visualstudio.com/) or newer (The source code is created using VS 2015 Professional)
* Windows 8.1 or higher (to be able to install and run Visual Studio 2015)
* Although not a requirement, we recommend you have a Git client for easy source code browsing and making it easy to switch between branches (tutorial steps are split using branches)
* That's it, your ready to start.

## Installing Couchbase Server 4.0
First things first... we need to install Couchbase Server! You can chose to install it locally on your developer machine or remotely, in this tutorial we will assume that Couchbase Server 4.0 is installed locally on the developer machine along side the web site that we will create.

[Download and installation instructions for Couchbase Server](http://www.couchbase.com/nosql-databases/downloads)
 
Download Couchbase Server and follow the [instructions](http://developer.couchbase.com/documentation/server/4.0/getting-started/installing.html) for your platform to complet the installation. 

If this is the first time you setup Couchbase Server, this [detailed guide](http://developer.couchbase.com/documentation/server/4.0/install/init-setup.html#topic12527) will help you through all the steps need and explain the different options. 

**Important!** As you follow the download instructions and setup wizard, make sure you keep all the services (data, query, and index) selected and remember to install the sample bucket named `travel-sample` (introduced in CB 4.0). `travel-sample` is the data set that will be used throughout this tutorial.

![Select all services](content/images/setup-01.png)


>TIP: 
	If you already have Couchbase Server installed but did not install the travel-	sample bucket!
	
>Open the Couchbase Web Console and select 'Settings' -> 'Sample Buckets' and select the `travel-sample` check box, and then click Create. 
	 
>A 	notification box in the upper-right corner will appear and show the progress. When it disappears the bucket is ready to use.
	
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
This source code is split up in three separate parts with their individual purpose.
 
1. The 'red box' is the Couchbase Server Cluster our data storage
2. The 'green box' represents the API server, responsible to serve the data to the UI. In this case the backend is build with C# and Web API, but could be implemented in any language. We currently have tutorials for: [Java](https://github.com/couchbaselabs/try-cb-java), [Node.js](https://github.com/couchbaselabs/try-cb-nodejs) and [.net - this tutorial](https://github.com/couchbaselabs/try-cb-dotnet)
3. The 'blue box' is symbolising the UI it self, this is the part of the application that surfaces the content to the user. It's all static HTML and JavaScript build to consume data from the API. We are not going to change a single line in the UI. 

![app diagram](content/images/Screen Shot 2015-11-05 at 12.00.18.png)

This application architecture gives a very clean seperation of concerns and would effectively allow us to change the the various parts in the application architecture without influencing the other. This is of course only true if we maintain the REST API methods and responses.

In fact this allows us to change the backend API without touching the front end code. 
Therefore if you take a look at the java version [`try-cb-java`](https://github.com/couchbaselabs/try-cb-java) you quickly learn that the UI is the same, it's only the backend that has changed.

In the .NET implementation of the backend we use WEB API, as this is probably the most popular, flexible and easy way to implement a REST API endpoint in .NET.  


###Get set up for the tutorial 
To get propper setup for the tutorial and get ready for the first part, follow these steps:

* git clone https://github.com/couchbaselabs/try-cb-dotnet.git or download the source
* If you did not install Couchbase Server on `localhost` and want to connect to a remote Couchbase Server, change the `couchbaseServer` key in `web.config`. You can also change the username and password for Couchbase Server.
* Use Visual Studio 2015 to open the solution file `try-cb-dotnet.sln` in `try-cb-dotnet/src`
* The solution is configured to restore all missing `nuget`packages on every build. Therefore the only thing missing now is to build and run the solution.

>Note:
	Restoring the missing nuget packages can take some time and is also influenced 	by your network speed.

## Tutorial step 1 - 5
 
### Step 1 - Understand ASP.NET Web API 2 and .NET 
The first part of this tutorial is not about how to use the .NET Client for Couchbase Server. 

The focus in part 1 is to show how ASP.NET Web API works and to emphasise that with Web API we have an option to work with and return JSON from the REST endpoints. 

It's important to understand that the main task of the backend REST API is to return JSON. This is an important concept to understand that will greatly help you understand and work with ASP.NET Web API.
 
> Implementing the Web API methods (REST Endpoints) is all about returning the right JSON. Couchbase stores all it's documents in JSON and therefore it is a very good match as a data store for a REST API.
> 
>If you already feel comfortable working with ASP.NET Web API you can skip this step and go directly to step 2. In step 2 you will learn how to use Couchbase and the Couchbase .NET Client in your .NET projects.

In this step you will update all Web API methods to return static JSON (string values). This will allow you to run and browse the web application and get an understanding of how the code is organised.

####Step 1.1 - Implement Login 

**Where:** `UserController.cs` -> **method:** `Login(string password, string user)`

**Goals:** Return static JSON to learn how Web API works.

**Relevant Documentation Topics:** 

 * [ASP.NET Web API 2](http://www.asp.net/web-api/overview/getting-started-with-aspnet-web-api/tutorial-your-first-web-api)
 * [JWT Token](http://jwt.io/)

**Task:**

`Login(string password, string user)` is a Web API method called using JavaScript from the static HTML page `index.html`. 

The JavaScript used in the static html page expects this "Login" web api call to return a JavaScript Web Token (JWT) "success" status code token. Returning the "success" token will login the user and redirect the user to the flight booking page. 

The JWT token is used to reference and store data about the user's trips/bookings and login credentials.

The JWT response should be in a JSON format like this:
   `[{"success":"eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VyIjoiZ3Vlc3QiLCJpYXQiOjE0NDE4Njk5NTR9.5jPBtqralE3W3LPtS - j3MClTjwP9ggXSCDt3 - zZOoKU"}]`Implement the method to return a "success" JWT token allowing the user to login.
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

**Goals:** Return static JSON to learn how ASP.NET Web API works.

**Relevant Documentation Topics:** [ASP.NET Web API 2](http://www.asp.net/web-api/overview/getting-started-with-aspnet-web-api/tutorial-your-first-web-api)


**Task:**
This task is essential the same as `1.1`.

`Login(string password, string user)` is a Web API method called using JavaScript from the static HTML page `index.html`. 

The JavaScript used in the static html page expects this "Login" web api call to return a JavaScript Web Token (JWT) "success" status code token. Returning the "success" token will login the user and redirect the user to the flight booking page. 

The JWT token is used to reference and store data about the user's trips/bookings and login credentials.

The JWT response should be in a JSON format like this:
   `[{"success":"eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VyIjoiZ3Vlc3QiLCJpYXQiOjE0NDE4Njk5NTR9.5jPBtqralE3W3LPtS - j3MClTjwP9ggXSCDt3 - zZOoKU"}]`Implement the method to return a "success" JWT token allowing the user to login.
Later we will implement a JWT token issuer and store user data in Couchbase for later look-up.The token is created for the user:>Note: The login credentials for this JWT token is:
>
>username: guest
>
>password: guest

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
The JavaScript in the static html expects this "flights" Web APIcall to return
all bookings done by the logged in user. 

The JWT token is used to look-up the user and find all bookings.

In this "fake" implementation we are not going to use the JWT Token, but instead return a static list of bookings. The list should be the same for all users.

Response should be in a JSON format like this:

Bookings:
`[{"_type":"Flight","_id":"d500a3d1-2cca-43a5-8a66-f11828a35969","name":"American Airlines","flight":"AA344","date":"09/10/2015","sourceairport":"SFO","destinationairport":"LAX","bookedon":"1441881827622"},{"_type":"Flight","_id":"bf676b0d-e63b-4ff6-aade-7ac1c182b3de","name":"American Airlines","flight":"AA787","date":"09/11/2015","sourceairport":"LAX","destinationairport":"SFO","bookedon":"1441881827623"},{"_type":"Flight","_id":"f0099c24-3ad4-482e-8352-704f9cbf1a43","name":"American Airlines","flight":"AA550","date":"09/10/2015","sourceairport":"SFO","destinationairport":"LAX","bookedon":"1441881827623"}]`
            
Implement the method to return the "fake" bookings for all users.
Later we will look-up actual bookings using the JWT token, but for now a static list is what we need.

>Hint: 
>
>To simulate more than one booking you can return the same booking multiple times in a list, re-using the sample JSON above.

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
The JavaScript in the static html expects the call to the "flights" Web API method to save the flight bookings for a user. The bookings are saved in a booking's document.
 
The JWT token is used as a key to the users bookings document, this way we can use the JWT token to look-up all bookings for a user.

In this "fake" implementation we are not going to use the JWT Token, nor store any data about the bookings.

Instead we return a static value to indicate that the booking was successful, simulation the creation of the bookings document for the user.

The response should be in a JSON format like this:

Bookings:

`{"added":3}`
 
Implement the Web API method to return the "fake" `booking success` for the guest user.

Later we will revisit this method and updated it to store bookings using the JWT token, but for now a static response is what we need.

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
This is a Web API call, a method that is called from the static html page `index.html`.

JavaScript is used in the static `index.html` page to call the Web API and the expected value returned from the `findAll` Web API call is "trip" data in a JSON format like this:

Round trip: 
`[{"destinationairport":"SFO","equipment":"738","flight":"AA907","id":5746,"name":"American Airlines","sourceairport":"LAX","utc":"00:29:00","flighttime":1,"price":53},{"destinationairport":"SFO","equipment":"738","flight":"AA787","id":5746,"name":"American Airlines","sourceairport":"LAX","utc":"19:06:00","flighttime":1,"price":45},{"destinationairport":"SFO","equipment":"738","flight":"AA279","id":5746,"name":"American Airlines","sourceairport":"LAX","utc":"04:54:00","flighttime":1,"price":52},{"destinationairport":"SFO","equipment":"E75","flight":"DL856","id":21085,"name":"Delta Air Lines","sourceairport":"LAX","utc":"20:08:00","flighttime":1,"price":47},{"destinationairport":"SFO","equipment":"E75","flight":"DL273","id":21085,"name":"Delta Air Lines","sourceairport":"LAX","utc":"14:14:00","flighttime":1,"price":48},{"destinationairport":"SFO","equipment":"73W 73C 733","flight":"WN543","id":63986,"name":"Southwest Airlines","sourceairport":"LAX","utc":"22:16:00","flighttime":1,"price":44},{"destinationairport":"SFO","equipment":"73W 73C 733","flight":"WN828","id":63986,"name":"Southwest Airlines","sourceairport":"LAX","utc":"04:35:00","flighttime":1,"price":43},{"destinationairport":"SFO","equipment":"738","flight":"US086","id":59532,"name":"US Airways","sourceairport":"LAX","utc":"15:06:00","flighttime":1,"price":46},{"destinationairport":"SFO","equipment":"738","flight":"US150","id":59532,"name":"US Airways","sourceairport":"LAX","utc":"15:44:00","flighttime":1,"price":47},{"destinationairport":"SFO","equipment":"738","flight":"US437","id":59532,"name":"US Airways","sourceairport":"LAX","utc":"23:42:00","flighttime":1,"price":52},{"destinationairport":"SFO","equipment":"739 752 753 319 320 738","flight":"UA666","id":57010,"name":"United Airlines","sourceairport":"LAX","utc":"05:11:00","flighttime":1,"price":44},{"destinationairport":"SFO","equipment":"739 752 753 319 320 738","flight":"UA978","id":57010,"name":"United Airlines","sourceairport":"LAX","utc":"19:50:00","flighttime":1,"price":53},{"destinationairport":"SFO","equipment":"739 752 753 319 320 738","flight":"UA123","id":57010,"name":"United Airlines","sourceairport":"LAX","utc":"21:13:00","flighttime":1,"price":49},{"destinationairport":"SFO","equipment":"320 319","flight":"VX929","id":62018,"name":"Virgin America","sourceairport":"LAX","utc":"00:39:00","flighttime":1,"price":49},{"destinationairport":"SFO","equipment":"320 319","flight":"VX351","id":62018,"name":"Virgin America","sourceairport":"LAX","utc":"01:37:00","flighttime":1,"price":49},{"destinationairport":"SFO","equipment":"320 319","flight":"VX703","id":62018,"name":"Virgin America","sourceairport":"LAX","utc":"05:01:00","flighttime":1,"price":47},{"destinationairport":"SFO","equipment":"320 319","flight":"VX743","id":62018,"name":"Virgin America","sourceairport":"LAX","utc":"10:36:00","flighttime":1,"price":53},{"destinationairport":"SFO","equipment":"320 319","flight":"VX301","id":62018,"name":"Virgin America","sourceairport":"LAX","utc":"01:32:00","flighttime":1,"price":49}]`
            
One way trip:
`[{"destinationairport":"SFO","equipment":"738","flight":"AA787","id":5746,"name":"American Airlines","sourceairport":"LAX","utc":"19:06:00","flighttime":1,"price":48},{"destinationairport":"SFO","equipment":"738","flight":"AA279","id":5746,"name":"American Airlines","sourceairport":"LAX","utc":"04:54:00","flighttime":1,"price":49},{"destinationairport":"SFO","equipment":"738","flight":"AA907","id":5746,"name":"American Airlines","sourceairport":"LAX","utc":"00:29:00","flighttime":1,"price":51},{"destinationairport":"SFO","equipment":"E75","flight":"DL273","id":21085,"name":"Delta Air Lines","sourceairport":"LAX","utc":"14:14:00","flighttime":1,"price":51},{"destinationairport":"SFO","equipment":"E75","flight":"DL856","id":21085,"name":"Delta Air Lines","sourceairport":"LAX","utc":"20:08:00","flighttime":1,"price":53},{"destinationairport":"SFO","equipment":"73W 73C 733","flight":"WN543","id":63986,"name":"Southwest Airlines","sourceairport":"LAX","utc":"22:16:00","flighttime":1,"price":50},{"destinationairport":"SFO","equipment":"73W 73C 733","flight":"WN828","id":63986,"name":"Southwest Airlines","sourceairport":"LAX","utc":"04:35:00","flighttime":1,"price":53},{"destinationairport":"SFO","equipment":"738","flight":"US086","id":59532,"name":"US Airways","sourceairport":"LAX","utc":"15:06:00","flighttime":1,"price":53},{"destinationairport":"SFO","equipment":"738","flight":"US150","id":59532,"name":"US Airways","sourceairport":"LAX","utc":"15:44:00","flighttime":1,"price":43},{"destinationairport":"SFO","equipment":"738","flight":"US437","id":59532,"name":"US Airways","sourceairport":"LAX","utc":"23:42:00","flighttime":1,"price":48},{"destinationairport":"SFO","equipment":"739 752 753 319 320 738","flight":"UA978","id":57010,"name":"United Airlines","sourceairport":"LAX","utc":"19:50:00","flighttime":1,"price":48},{"destinationairport":"SFO","equipment":"739 752 753 319 320 738","flight":"UA666","id":57010,"name":"United Airlines","sourceairport":"LAX","utc":"05:11:00","flighttime":1,"price":51},{"destinationairport":"SFO","equipment":"739 752 753 319 320 738","flight":"UA123","id":57010,"name":"United Airlines","sourceairport":"LAX","utc":"21:13:00","flighttime":1,"price":50},{"destinationairport":"SFO","equipment":"320 319","flight":"VX743","id":62018,"name":"Virgin America","sourceairport":"LAX","utc":"10:36:00","flighttime":1,"price":48},{"destinationairport":"SFO","equipment":"320 319","flight":"VX703","id":62018,"name":"Virgin America","sourceairport":"LAX","utc":"05:01:00","flighttime":1,"price":45},{"destinationairport":"SFO","equipment":"320 319","flight":"VX301","id":62018,"name":"Virgin America","sourceairport":"LAX","utc":"01:32:00","flighttime":1,"price":49},{"destinationairport":"SFO","equipment":"320 319","flight":"VX929","id":62018,"name":"Virgin America","sourceairport":"LAX","utc":"00:39:00","flighttime":1,"price":44},{"destinationairport":"SFO","equipment":"320 319","flight":"VX351","id":62018,"name":"Virgin America","sourceairport":"LAX","utc":"01:37:00","flighttime":1,"price":46}]`

As shown above the "trip" data is spilt up in two, one way and round trip (return trip). 

Implement the method to return a "Round trip" from a destination and source airport and back.

Later we re-vist this Web API method and update it to use data from Couchbase to do the look-up, but for now a "constant" is returned.          
            
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
The JavaScript in the static html expects this `findAll` Web API call to return a
"airportname", based on the input `search` parameter.

The return data used for this static implementation should look as follows:

`[{"airportname":"San Francisco Intl"}]`

Implement the method to return a single airport name.

Later we will use Couchbase to do the look-up, but for now a constant value is returned.

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
If done correctly all Web API methods now return a static JSON value. This should enable you to be able to run and browse the application. 

All data is static but never the less it "works". In Step 2 we will update the static JSON returned in the Web API method to return actual data from Couchbase Server 4.    

### Step 2 - Understand Couchbase, Couchbase .NET SDK & N1QL
In this step we will update all ASP.NET Web API methods to return data from Couchbase Server. 

This is the first step that uses Couchbase and therefore we need to add references to the Couchbase Client, LINQ extensions and configure the Couchbase Client also know as bootstrapping. 

####Step 2.0 - Referencing & Bootstrapping Couchbase Client for .NET.

**Where:** `Solution` (this is a solution wide update)

**Goals:** Add a reference to: [CouchbaseNetClient](https://www.nuget.org/packages/CouchbaseNetClient) and [Linq2Couchbase](https://www.nuget.org/packages/Linq2Couchbase) the later is the LINQ to N1QL extensions. 

When the references are in place we need to bootstrap (configuration and initialisation) the Couchbase .NET SDK and make it globally available to the application.

**Relevant Documentation Topics:** 

* [N1QL intro](http://developer.couchbase.com/documentation/server/4.0/n1ql/n1ql-intro/data-access-using-n1ql.html)
* [Couchbase .NET Client - github](https://github.com/couchbase/couchbase-net-client)
* [Couchbase .NET Client - docs](http://developer.couchbase.com/documentation/server/4.0/sdks/dotnet-2.2/getting-started.html)
* [Linq2Couchbase - github](https://github.com/couchbaselabs/Linq2Couchbase)
* [Hello World - Couchbase .NET](http://developer.couchbase.com/documentation/server/4.0/sdks/dotnet-2.2/hello-couchbase.html)

##### 2.0 - Task 1: Referencing Couchbase Client and LINQ to N1QL Extensions
For every release of the Couchbase .NET Client a matching NuGet package with the binaries is released to the [NuGet Gallery](https://www.nuget.org/packages). 

If you are not familiar with NuGet, it’s the official and most widely supported package manager for Microsoft Visual Studio and .NET in general. NuGet is a centralised repository for package authors and consumers, and it also defines a suite of tools for authoring, publishing and consuming packages from various vendors and authors.

Using Visual Studio 2015 or later, follow these steps to get started with the Couchbase .NET SDK:

	From the IDE, right-click the solution/project to which you want to add the dependency.

![Add NuGet Ref](content/images/Screen Shot 2015-11-18 at 08.23.42.png)


	In the context menu, click `Manage NuGet Packages for ...`. The NuGet package manager view opens.

![NuGet view](content/images/Screen Shot 2015-11-18 at 08.30.52.png)

	In the search box at the top right-hand side of the dialog, type 'CouchbaseNetClient' and then press enter on your keyboard.

![NuGet install CB Client](content/images/Screen Shot 2015-11-18 at 08.34.20.png)

	In the search results, select the 'CouchbaseNetClient' package and then click Install. 
	The Visual Studio Output window will show progress and concluded with a 'Finished'.

![NuGet install progress](content/images/Screen Shot 2015-11-18 at 08.36.36.png)

	Repeat step 3-4 to install 'Linq2Couchbase'.
	
![NuGet installed packages](content/images/Screen Shot 2015-11-18 at 08.41.21.png)	
	
	Confirm that both NuGet packages have been successfully installed.
	Using the NuGET view, sarch for 'Couchbase' and set Filter to 'Installed' as shown above.
	Confirm that both 'Linq2Couchbase' and 'CouchbaseNetClient' are installed.

That’s it! NuGet has pulled in all required dependencies and reference required dependencies for Couchbase Client and LINQ to N1QL extension. 


##### 2.0 - Task 2: Bootstrapping Couchbase Client 
Bootstrapping is the popular phrase used to describe the task of initialising and configuring a library. In case of the Couchbase .NET Client we need to inform the .NET Client where to find the Couchbase Server and what buckets to use.

The Couchbase Client includes a helper class called `ClusterHelper`. This class is a singleton that can be shared globally in the application and should be kept alive for the lifetime of the application. The benefit of using `ClusterHelper` is that resources are shared across the the application and thereby setup and teardown is not done unless explicitly needed. 

It is recommended by Couchbase to use `ClusterHelper` when working with the .NET Client and part of the best practices. 

In this tutorial the application at hand is a web application and therefore it's most convenient to initialise the Couchbase Client in `Global.asax.cs` as this is the main entry point and run on application start. Initialising the Couchbase .NET Client here will ensure that it is initialised before any other code is called.

Using Visual Studio 2015 or later, follow these steps to bootstrap the Couchbase .NET Client:

	Create a new file in the folder 'App_Start' called 'CouchbaseConfig.cs'.
	
![Add new file 1](content/images/Screen Shot 2015-11-18 at 09.28.04.png)

![Add new file 2](content/images/Screen Shot 2015-11-18 at 09.29.34.png)
	

	Replace the content of 'CouchbaseConfig.cs' with this code snippet:
	
```C#
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
```
			  
	The class 'CouchbaseConfig' references a class called 'CouchbaseConfigHelper' that does not yet exist. The purpose of 'CouchbaseConfigHelper' class is to wrap calls to read the 'AppSettings' section of 'web.config' file. The 'AppSettings' section is often used to store configuration settings for an application in .NET. In this tutorial we will follow this design design practice from Microsoft.
	
	In the project root create a new code file called: 'CouchbaseConfigHelper.cs'. Right click the project, in the menu select -> 'Add'-> 'Class'.
	
![Add new file 3](content/images/Screen Shot 2015-11-18 at 09.40.22.png)

![Add new file 3](content/images/Screen Shot 2015-11-18 at 09.40.50.png)

		
	Replace the content of 'CouchbaseConfigHelper.cs' with the following code snippet:
```C#
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
```	
		
	Reading the code in 'CouchbaseConfigHelper' reveals that it's referencing a bunch of application setting keys in `web.config`, that we have still to create.	
	
	Open 'web.config' and add the missing application setting keys in the 'appSettings' section as shown in the following snippet:	

```XML	
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
```

	You can change the settings in 'web.config' to reflect your actual Couchbase Server setup. This includes adding username and password if appropriate and correct the cluster url if needed. The current configuration assumes that Couchbase Server is installed on localhost and no password protection on the bucket.

	We now have all configurations in place need to initialise the Couchbase .NET Client.
	Open the file 'Global.asax.cs' in project root. 
	
	Update the method 'Application_Start()' with a call to 'CouchbaseConfig.Initialize();' 
	This will initialise the Couchbasebase Client based on the settings from 'web.config'.

```C#
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
```
    
	The only thing missing now is disposing of resources when the application stops.
	This includes closing the connection to the Couchbase Cluster and releasing memory.
	
	Update the 'Application_End()' method to call the 'Close()' method on 'CouchbaseConfig'

```C#
		protected void Application_End()
		{
			CouchbaseConfig.Close();
		}
```
	
You're ready to start using the Couchbase .NET Client, happy coding!

####Step 2.1 

**Where:** `AirportController.cs` -> **method:** `FindAll(string search, string token)`

**Goals:** Return live data from the Travel Sample data bucket in Couchbase Server 4.0 and learn more about how to use Couchbase with .NET

**Relevant Documentation Topics:** 

* [Linq2Couchbase - github](https://github.com/couchbaselabs/Linq2Couchbase)
* [Hello World - Couchbase .NET](http://developer.couchbase.com/documentation/server/4.0/sdks/dotnet-2.2/hello-couchbase.html)

**Task:**
In the current implementation `FindAll(string search, string token)` returns static data, using the `travel-sample` bucket, Couchbase .NET Client and N1QL we will update the method to return actual data. 

The intention of `FindAll(string search, string token)` is to return a airport name based on the `search` string passed to the method.

This is a Web API call, a method that is called from the static html (index.html).
The JS in the static html expects this "findAll" web api call to return a
"airportname" in a JSON format like this:
[{"airportname":"San Francisco Intl"}]

Implement the method to return a list of airport names that match the "search" string
and return the result list.
The result list is used by the UI to show a drop-down of matching airport names the user can select from.

>Hint: use N1QL to query the "travel-sample" bucket in Couchbase and return all matching airport names. 

**Solution:**

		[HttpGet]
        [ActionName("findAll")]
        public object FindAll(string search, string token)
        {
            if (search.Length == 3)
            {
                // LAX
                var query = 
                    new QueryRequest("SELECT airportname FROM `" + CouchbaseConfigHelper.Instance.Bucket + "` WHERE faa=$1")
                    .AddPositionalParameter(search.ToUpper());

                return ClusterHelper
                    .GetBucket("travel-sample")
                    .Query<dynamic>(query)
                    .Rows;
            }
            else if (search.Length == 4)
            {
                // KLAX
                var query =
                    new QueryRequest("SELECT airportname FROM `" + CouchbaseConfigHelper.Instance.Bucket + "` WHERE icao = '$1'")
                    .AddPositionalParameter(search.ToUpper());

                return ClusterHelper
                    .GetBucket(CouchbaseConfigHelper.Instance.Bucket)
                    .Query<dynamic>(query)
                    .Rows;
            }
            else
            {
                // Los Angeles
                var query =
                    new QueryRequest("SELECT airportname FROM `" + CouchbaseConfigHelper.Instance.Bucket + "` WHERE airportname LIKE $1")
                    .AddPositionalParameter("%" + search + "%");

                return ClusterHelper
                    .GetBucket(CouchbaseConfigHelper.Instance.Bucket)
                    .Query<dynamic>(query)
                    .Rows;
            }
        }    

Test the application with various airport names and abbreviation forms: `SFO`, `KLAX`, `Los Angeles` etc.

####Step 2.2 

**Where:** `FlightPathController.cs` -> **method:** `FindAll(string from, DateTime leave, string to, string token)`

**Goals:** Return live data from the Travel Sample data bucket in Couchbase Server 4.0 and learn more about how to use Couchbase with .NET

**Relevant Documentation Topics:** 

* [Linq2Couchbase - github](https://github.com/couchbaselabs/Linq2Couchbase)
* [Hello World - Couchbase .NET](http://developer.couchbase.com/documentation/server/4.0/sdks/dotnet-2.2/hello-couchbase.html)

**Task:**
In the current implementation `FindAll(string from, DateTime leave, string to, string token)` returns static data, using the `travel-sample` bucket, Couchbase .NET Client and N1QL we will update the method to return actual data. 

The intention of `FindAll(string from, DateTime leave, string to, string token)` is to search out and find actual route data. Trip data is create by joining `airport` documents with with `routes`.

This is a Web API call, a method that is called from the static html (index.html).
The JS in the static html expects this "findAll" web api call to return a
"trip" in a JSON format.

Implement the method to return all trips that match the selected source and destination airport for the given date interval.

>HINT: Use N1QL to query the "travel-sample" bucket in Couchbase to find matching "trips"
The travel sample does not contain any "trip" documents you will need to use "join" to build a matcing result-set.
This is a two step proccess/query.

* 1: 
    This API method is called with the "from" and "to" values witch represent the full airport name.
    The "join" that we will do in step 2, needs the FAA (3 letter abbreviation for the airport name)
    Therefore the first step is to make a N1QL query to convert the airport name to a FAA 3 letter value 
    for both from and to airport.

* 2:
    This is a bit tricky as we will create a N1QL query that will use both UNNEST and JOIN. 
    Below you can find a "template" version of the query.
    This will allow you to understand the query in it's full detail and exam the parameters needed for this query.

>SELECT r.id, a.name, s.flight, s.utc, r.sourceairport, r.destinationairport, r.equipment FROM 
    `travel-sample` r 
    UNNEST r.schedule s 
    JOIN `travel-sample` a 
    ON KEYS r.airlineid 
    WHERE r.sourceairport='LAX' 
    AND r.destinationairport='SFO' 
    AND s.day=1
    ORDER BY a.name      
    
**Solution:**

		[HttpGet]
    	[ActionName("findAll")]
        public object FindAll(string from, DateTime leave, string to, string token)
        {
            string queryFrom = null;
            string queryTo = null;
            var queryLeave = (int)leave.DayOfWeek;

            // raw query
            var query1 =
                   new QueryRequest(
                       "SELECT faa as fromAirport, geo FROM `" + CouchbaseConfigHelper.Instance.Bucket + "` " +
                       "WHERE airportname = $from " +
                       "UNION SELECT faa as toAirport, geo FROM `" + CouchbaseConfigHelper.Instance.Bucket + "` " +
                       "WHERE airportname = $to")
                   .AddNamedParameter("from", from)
                   .AddNamedParameter("to", to);

            var partialResult1 = ClusterHelper
                    .GetBucket(CouchbaseConfigHelper.Instance.Bucket)
                    .Query<dynamic>(query1);

            if (partialResult1.Rows.Any())
            {
                foreach (dynamic row in partialResult1.Rows)
                {
                    if (row.fromAirport != null) queryFrom = row.fromAirport;
                    if (row.toAirport != null) queryTo = row.toAirport;
                }
            }

            // raw query
            var query2 =
                   new QueryRequest(
                       "SELECT r.id, a.name, s.flight, s.utc, r.sourceairport, r.destinationairport, r.equipment FROM " +
                       "`" + CouchbaseConfigHelper.Instance.Bucket + "` r " +
                       "UNNEST r.schedule s JOIN " +
                       "`" + CouchbaseConfigHelper.Instance.Bucket + "` " +
                       "a ON KEYS r.airlineid WHERE r.sourceairport=$from " +
                       "AND r.destinationairport=$to " +
                       "AND s.day=$leave " +
                       "ORDER BY a.name")
                   .AddNamedParameter("from", queryFrom)
                   .AddNamedParameter("to", queryTo)
                   .AddNamedParameter("leave", queryLeave);

            return ClusterHelper
                    .GetBucket(CouchbaseConfigHelper.Instance.Bucket)
                    .Query<dynamic>(query2)
                    .Rows;
        }
    }    
    
####Step 2.3

**Where:** `UserController.cs` -> **method:** `Flights(string token)`

**Goals:** Return live data from about the user's travel bookings stored in Couchbase Server 4.0 and learn more about how to use Couchbase with .NET

**Relevant Documentation Topics:** 

* [Linq2Couchbase - github](https://github.com/couchbaselabs/Linq2Couchbase)
* [Hello World - Couchbase .NET](http://developer.couchbase.com/documentation/server/4.0/sdks/dotnet-2.2/hello-couchbase.html)

**Task:**
In the current implementation `Flights(string token)` returns static data, using the `travel-sample` bucket, Couchbase .NET Client and N1QL we will update the method to return actual data stored about the users bookings. 

The intention of `Flights(string token)` is to return a users actual travel bookings. 

This is a Web API call, a method that is called from the static html (index.html).
The JS in the static html expects this "flights" web api call to return a
all bookings done by this user. 
The JWT token is used to look-up the user and find all bookings.
Response should be in a JSON format

Implement the method to return all bookings for the logged-in user.

* 1:
    Use the token to look-up the "bookings" document for the user and return all bookings.
    The document key is in the format
    bookings::token
    The great thing with using Couchbase (a JSON document store) is that we don't need to convert the value,
    we can just return the actual document stored in Couchbase.
 
>Hint: 
    Use `ClusterHelper` to Get document in the "default" bucket and retur the value. 
    
**Solution:**

		[HttpGet]
        [ActionName("flights")]
        public object Flights(string token)
        {
            return ClusterHelper
                    .GetBucket(CouchbaseConfigHelper.Instance.Bucket)
                    .Get<dynamic>("bookings::" + token)
                    .Value;
        }    

####Step 2.4

**Where:** `UserController.cs` -> **method:** `BookFlights([FromBody] dynamic request)`

**Goals:** Store live data about the user's travel bookings, persist them in Couchbase Server 4.0 and learn more about how to use Couchbase with .NET

**Relevant Documentation Topics:** 

* [Linq2Couchbase - github](https://github.com/couchbaselabs/Linq2Couchbase)
* [Hello World - Couchbase .NET](http://developer.couchbase.com/documentation/server/4.0/sdks/dotnet-2.2/hello-couchbase.html)

**Task:**
In the current implementation `BookFlights([FromBody] dynamic request)` nothing is actually stored, we just return a constant that indicates how many line items where stored. 
Using the `travel-sample` bucket, Couchbase .NET Client and N1QL we will update the method to store actual data about the users bookings. 

The intention of `BookFlights([FromBody] dynamic request)` is to store information about a users actual travel bookings. 

This is a Web API call, a method that is called from the static html (index.html).
The JS in the static html expects this "flights" web api call to save the selected flight in a booking's document.

The JWT token is used as a key to the users bookings.
In this fake implementation we are not going to use the Token, nor store any data about the bookings.
Instead we return a static value to indicate that the booking was successful.
Response should be in a JSON format like this:
Bookings:
{"added":3}

Implement the method to return the the number of successful bookings that where persisted in couchbase for the user (token)

* 1:
    First we need to get an understanding of the "dynamic request" value. 
    Add a breakpoint to this method and use "immediate window" in Visual studio to investigate the request value.
    "request" is a list of dynamic "JToken" values. It's therefore possible to "foreach" over the collection and "select" 
    the the relevant values and store them directly in Couchbase.
* 2:
    Create a foreach loop to iterate over the request collection and store all bookings to Couchbase.
    the bookings should be stored in a document with the compound key
    bookings::{token}
    The token is available in the request, request.token

* 3:
    Update the return value to reflect the actual number of stored bookings    

>Hint: 
    Use ClusterHelper to `Upsert(...)` the document in the "default" bucket. 
    
**Solution:**
    
    	[HttpPost]
        [ActionName("flights")]
        public object BookFlights([FromBody] dynamic request)
        {
            List<FlightModel> flights = new List<FlightModel>();

            foreach (var flight in request.flights)
            {
                flights.Add(new FlightModel
                {
                    name = flight._data.name,
                    bookedon = DateTime.Now.ToString(),
                    date = flight._data.date,
                    destinationairport = flight._data.destinationairport,
                    sourceairport = flight._data.sourceairport
                });
            }

             ClusterHelper
                .GetBucket(CouchbaseConfigHelper.Instance.Bucket)
                .Upsert("bookings::" + request.token, flights);

            return new { added = flights.Count };
        }

### Step 2 - Summery
In part 2 we learned how to bootstrap the .NET Couchbase Client and query data with N1QL raw string queries. We have not used LINQ 2 Couchbase yet, we will come back to that in Step 4.

### Step 3 - Login credentials and authentications 
In this step we will implement the login page to use JWT Tokens for new and exciting users.

####Step 3.1

**Where:** `UserController.cs` -> **method:** `Login(string password, string user)`

**Goals:** 
Update the Web Api method to return JWToken stored in Couchbase Server for then individual users.

**Relevant Documentation Topics:** 

* [JWT Token .NET](https://www.nuget.org/packages/JWT)

**Task:**    

This is a Web API call, a method that is called from the static html (index.html).
The JS in the static html expects this "Login" web api call to return a
"success" status code containing a JWT token. 
The JWT token is used to reference and store data about the user's trips/bookings and login credentials.
Response should be in a JSON format.

Implement the method to return a "success" allowing the user to login.

* 1:
    Use ClusterHelper to look-up the user document and validate the login.
    If the login is valid then return 
    "Success" : token
    If not, return 
    "Success" : false
    
>hint:
The user document is stored under the key: "profile::" + user in the "default" bucket.

**Solution:**

		[HttpGet]
        [ActionName("Login")]
        public object Login(string password, string user)
        {
            try
            {
                var result = ClusterHelper
                    .GetBucket("default")
                    .Get<dynamic>("profile::" + user);

                if (result.Success && result.Status == Couchbase.IO.ResponseStatus.Success && result.Exception == null && result.Value != null)
                {
                    var jsonDecodedTokenString =
                        JsonWebToken
                        .Decode(result.Value, CouchbaseConfigHelper.Instance.JWTTokenSecret, false);

                    var jwtToken = JsonConvert.DeserializeAnonymousType(jsonDecodedTokenString, new { user = "", iat = "" });

                    if (jwtToken.iat == password)
                    {
                        return new { success = result.Value };
                    }
                }
            }
            catch (Exception)
            {
                // Silence the Exception
            }

            return new { success = false };
        }


####Step 3.2

**Where:** `UserController.cs` -> **method:** `CreateLogin([FromBody] UserModel user)`

**Goals:** 
Update the Web Api method to return JWToken stored in Couchbase Server for then individual users.

**Relevant Documentation Topics:** 

* [JWT Token .NET](https://www.nuget.org/packages/JWT)

**Task:**    

This is a Web API call, a method that is called from the static html (index.html).
The JS in the static html expects this "Login" web api call to return a
"success" status code containing a JWT token. 
The JWT token is used to reference and store data about the user's trips/booking and login credentials.
Response should be in a JSON format.

Implement the method to create and return a valid JWT given the username and password.
Be sure to check if a user already exists and fail in that case by returning "success" : false

* 1: Add a reference to the nuget package [JWT Token .NET](https://www.nuget.org/packages/JWT). 

* 2: 
    Check if user document already exists and fail if it does.
* 3:
    Using this Nuget JWT library:
    https://www.nuget.org/packages/JWT
    Implement JWT token support:
    The secret key to use for encryption and hashing is:
    "UNSECURE_SECRET_TOKEN"
    Store this key in Web.Config.
* 4:
    Store the generated JWT token under the user document key:
    "profile::user" in the "default" bucket.
* 5:
    Update the method to return the actual JWT token created in step 2.

>Hint: 
Use `ClusterHelper` to store the document. 


**Solution:**

`UserController.cs`

		[HttpPost]
        [ActionName("Login")]
        public object CreateLogin([FromBody] UserModel user)
        {
            try
            {
                if (ClusterHelper.GetBucket("default").Exists("profile::" + user.User))
                {
                    throw new Exception("User already Exists!");
                }

                string jsonToken =
                    JsonWebToken
                    .Encode(
                        new { user = user.User, iat = user.Password },
                        CouchbaseConfigHelper.Instance.JWTTokenSecret,
                        JwtHashAlgorithm.HS512);

                var result = ClusterHelper
                    .GetBucket("default")
                    .Upsert<dynamic>("profile::" + user.User, jsonToken);

                if (!result.Success || result.Exception != null)
                {
                    throw new Exception("could not save user to Couchbase");
                }

                return new { success = jsonToken };
            }
            catch (Exception)
            {
                // Silence the Exception
            }

            return new { success = false };
        }

`CouchbaseConfigHelper.cs`

		.
		.
		.
		public string JWTTokenSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["JWTTokenSecret"];
            }
        }
        .
        .
        .

`web.config`

	<appSettings>
    ...
    <!-- COUCHBASE TRAVEL SAMPLE SETTINGS -->
    ...
    <add key="JWTTokenSecret" value="UNSECURE_SECRET_TOKEN" />
    <!--END -->
  </appSettings>

### Step 3 - Summery
In part 3 we added JWT Tokens and login credentials to the web site, allowing users to create profiles and store/retrieve their bookings.

### Step 4 - Using LINQ with N1QL
In this step we will update a query to use the LINQ extensions for N1QL to query the travel sample data.

####Step 4.1

**Where:** `FlightPathController.cs` -> **method:** `FindAll(string from, DateTime leave, string to, string token)`

**Goals:** 
Update the Web Api method to use LINQ with N1QL to query the data. 

**Relevant Documentation Topics:** 

* [LINQ 2 Couchbase - blog post](http://blog.couchbase.com/2015/august/introducing-linq2couchbase-developer-preview-1-the-linq-provider-for-couchbase-n1ql)

**Task:**  
This is a Web API call, a method that is called from the static html (index.html).
The JS in the static html expects this "findAll" web api call to return a
"trip" in a JSON format like this:

Implement the method to use the `Linq2Couchbase` providers Query or Lambda syntax

>HINT: LINQ requires a model to base it's query upon (needed for reflection). Therefore you will need to create a PoCo for every document you will query with LINQ.

* 1: Create a PoCo Class for the LINQ query for "Airport"

`Airport.cs`:

        [EntityTypeFilter("airport")]
        public class Airport
        {
            [JsonProperty("airportname")]
            public string Airportname { get; set; }
        
            [JsonProperty("city")]
            public string City { get; set; }
        
            [JsonProperty("country")]
            public string Country { get; set; }
        
            [JsonProperty("faa")]
            public string Faa { get; set; }
        
            [JsonProperty("geo")]
            public Geo Geo { get; set; }
        
            [JsonProperty("icao")]
            public string Icao { get; set; }
        
            [JsonProperty("id")]
            public string Id { get; set; }
        
            [JsonProperty("type")]
            public string Type { get; set; }
        
            [JsonProperty("tz")]
            public string Tz { get; set; }
        }

`Geo.cs`:

            [EntityTypeFilter("geo")]
            public class Geo
            {
                [JsonProperty("alt")]
                public double Alt { get; set; }
            ///
                [JsonProperty("lat")]
                public double Lat { get; set; }
            ///
                [JsonProperty("lon")]
                public double Lon { get; set; }
            }

* 2: Create the query using Lambda or Query syntax, read the [LINQ 2 Couchbase - blog post](http://blog.couchbase.com/2015/august/introducing-linq2couchbase-developer-preview-1-the-linq-provider-for-couchbase-n1ql) to get more information about how to query with the LINQ provider.

**Solution:**    

		public class FlightPathController : ApiController
    {
        [HttpGet]
        [ActionName("findAll")]
        public object FindAll(string from, DateTime leave, string to, string token)
        {
            // query syntax
            var airlinesQuerySyntax
                = (from fromAirport in ClusterHelper.GetBucket(CouchbaseConfigHelper.Instance.Bucket).Queryable<Airport>()
                   where fromAirport.Airportname == @from
                   select new { fromAirport = fromAirport.Faa, geo = fromAirport.Geo })
                            .ToList() // need to execute the first part of the select before call to Union
                           .Union<dynamic>(
                                    from toAirport in ClusterHelper.GetBucket(CouchbaseConfigHelper.Instance.Bucket).Queryable<Airport>()
                                    where toAirport.Airportname == to
                                    select new { toAirport = toAirport.Faa, geo = toAirport.Geo });

            // lambda syntax
            var airlinesLambdaSyntaxt
                = ClusterHelper.GetBucket(CouchbaseConfigHelper.Instance.Bucket).Queryable<Airport>()
                .Where(airline => airline.Airportname == @from)
                .Select(airline => new { fromAirport = airline.Faa, geo = airline.Geo })
                .ToList() // need to execute the first part of the select before call to Union
                .Union<dynamic>(
                        ClusterHelper.GetBucket(CouchbaseConfigHelper.Instance.Bucket).Queryable<Airport>()
                        .Where(airline => airline.Airportname == to)
                        .Select(airline => new { toAirport = airline.Faa, geo = airline.Geo })
                        );

            //var airlinesResult = airlinesLambdaSyntaxt.ToList();
            var airlinesResult = airlinesQuerySyntax.ToList();

            string queryFrom = null;
            string queryTo = null;
            var queryLeave = (int)leave.DayOfWeek;

            foreach (var row in airlinesResult)
            {
                try
                {
                    if (row.fromAirport != null) queryFrom = row.fromAirport;
                }
                catch (Exception)
                {
                    // silence the exception as this is known to throw one time,
                    // for the row that does not contain toAirport. 
                    // There is no easy way to test for the missing attribute on the 
                    // dynamic type.
                }

                try
                {
                    if (row.toAirport != null) queryTo = row.toAirport;
                }
                catch (Exception)
                {
                    // silence the exception as this is known to throw one time,
                    // for the row that does not contain fromAirport. 
                    // There is no easy way to test for the missing attribute on the 
                    // dynamic type.
                }
            }

            // raw query
            var query2 =
                   new QueryRequest(
                       "SELECT r.id, a.name, s.flight, s.utc, r.sourceairport, r.destinationairport, r.equipment FROM " +
                       "`" + CouchbaseConfigHelper.Instance.Bucket + "` r " +
                       "UNNEST r.schedule s JOIN " +
                       "`" + CouchbaseConfigHelper.Instance.Bucket + "` " +
                       "a ON KEYS r.airlineid WHERE r.sourceairport=$from " +
                       "AND r.destinationairport=$to " +
                       "AND s.day=$leave " +
                       "ORDER BY a.name")
                   .AddNamedParameter("from", queryFrom)
                   .AddNamedParameter("to", queryTo)
                   .AddNamedParameter("leave", queryLeave);

            return ClusterHelper
                    .GetBucket(CouchbaseConfigHelper.Instance.Bucket)
                    .Query<dynamic>(query2)
                    .Rows;
        }

### Step 4 - Summery
In part 4 you learned how to use LINQ with N1QL. The benefits of using LINQ with N1QL is code completion, type safety and compile time checks of you queries. On the other hand there is an ekstra step required when creating the PoCo classes.
 
### Step 5 - Done
This is the Travel sample in it's entirety, nothing needs to be updated or changed. You can find the final version here:

[Step 5/branch 5](https://github.com/couchbaselabs/try-cb-dotnet/tree/tutorial-part-5)

Use this branch as a reference when creating the previous steps or as a reference app. 

