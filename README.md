# USFT.RestClient
USFT Rest Client and example applications in C#

This solution consists of four projects.

USFT.RestClient.v1 contains everything you need to create new C# applications with the USFT Rest API v1.

USFT.RestClient.RestToCSVService is an example application using the RestClient.v1 library. It runs the GET /Location command, and saves the returned information to a CSV file.

USFT.RestClient.RestToCSVService.Install generates a MSI for installing the Rest to CSV service. It requires a visual studio template that can be acquired here: https://visualstudiogallery.msdn.microsoft.com/9abe329c-9bba-44a1-be59-0fbf6151054d

USFT.RestClient.HistoryDownloader is another example application which downloads large chunks of history and saves them as CSV files, on demand.
