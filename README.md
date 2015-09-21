# try-cb-dotnet
Couchbase Travel Sample app written in ASP.NET

A sample application and dataset for getting started with Couchbase 4.0. 

The application runs a single page UI for demonstrating query capabilities. The application uses Couchbase Server + asp.net web api + Express + Angular and boostrap. 

The application is a flight planner that allows the user to search for and select a flight route (including return flight) based on airports and dates. Airport selection is done dynamically using an angular typeahead bound to cb server query. 

Date selection uses date time pickers and then searches for applicable air flight routes from a previously populated database.

# Installation and Configuration
Copy the source code to a folder of your choice and open the solution file (.sln) in Visual Studio 2015, compile and run.

# Open Issues and missing features
* Missing JWT token implementation to support login.
* Only valid user is guest/guest
* Using Linq2Couchbase for queries.
