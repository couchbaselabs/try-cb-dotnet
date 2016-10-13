# Couchbase .NET travel-sample Application
This is a sample application for getting started with Couchbase Server 4.5 and the .NET SDK. The application runs a single page UI for demonstrating SQL for Documents (N1QL) and Full Text Search (FTS) querying capabilities. It uses Couchbase Server 4.5 together with ASP.NET Web API 2, Angular2 and Bootstrap.

The application is a flight planner that allows the user to search for and select a flight route (including the return flight) based on airports and dates. Airport selection is done dynamically using an angular autocomplete box bound to N1QL queries on the server side. After selecting a date, it then searches for applicable air flight routes from a previously populated database. An additional page allows users to search for Hotels using less structured keywords.

### picture

## Prerequisites
The following pieces need to be in place in order to run the application.

* Couchbase Server 4.5 or later with the travel-sample bucket
* Visual Studio 2015 Community or Professional

## Running the application
To download the application you can either download the archive or clone the repository:

$ git clone https://github.com/couchbaselabs/try-cb-dotnet.git

Open Visual Studio and open `src/try-cb-dotnet.sln` from where you downloaded or cloned the source repository. Run (F5 or Debug > Start Debugging) and if all goes well, this will start a start IIS Express running the application on http://localhost:8080.

Note that when you run the application for the first time, it will make sure that all indexes are created for best performance, so it might take a bit longer. You can follow the output on the command line.

## Configuration Options

By default the application will connect to the `travel-sample` bucket on 127.0.0.1. It will however separate user account data into the `default` bucket (and these documents can be set to expire). All these options can be modified in `src/Web.Config`.
