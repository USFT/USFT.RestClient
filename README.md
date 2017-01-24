# USFT.RestClient
USFT Rest Client and example applications in C#

## Dlls
If you just want the latest dlls without building the project yourself

[USFT.RestClient.v1.dll](http://static.usfleettracking.com/downloads/usft-client/USFT.RestClient.v1.dll)

[Newtonsoft.Json.dll (v9.0.1)](http://static.usfleettracking.com/downloads/usft-client/Newtonsoft.Json.dll) (dependency)

## Solution details

USFT.RestClient.v1 contains everything you need to create new C# applications with the USFT Rest API v1.

USFT.RestClient.RestToCSVService is an example application using the RestClient.v1 library. It runs the GET /Location command, and saves the returned information to a CSV file.

USFT.RestClient.HistoryDownloader is another example application which downloads large chunks of history and saves them as CSV files, on demand.
