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
 
### Step 1


