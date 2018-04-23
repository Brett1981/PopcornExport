# Usage

This .NET Core console application is made to export all the data from the original Popcorn Time API to a private Azure SQL database.

It is used as an Azure WebJob which is triggered every 6 hours to update data.

# Details

The batch exports movies, shows, animes, images and torrents from [Popcorntime.sh API](https://github.com/popcorn-official/popcorn-api) and imports everything to Azure services.

# Storage

Images and torrent files are stored on Azure Blob Storage.

Data is stored in Azure SQL Server.

# Language and tools

Built with .NET Core SDK 2.0 and Visual Studio 2017.

# Screenshot

![Capture](https://github.com/bbougot/PopcornExport/blob/master/Screenshots/Capture.png)
