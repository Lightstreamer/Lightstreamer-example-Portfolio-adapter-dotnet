# Lightstreamer - Portfolio Demo - .NET Adapter #
<!-- START DESCRIPTION lightstreamer-example-portfolio-adapter-dotnet -->
The *Portfolio Demo* simulate a portfolio management: it shows a list of stocks included in a portfolio and provide a simple order entry form. Changes to portfolio contents, as a result of new orders, are displayed on the page in real-time. In addition to that, the *Full Version of the Portfolio Demo* also shows, for each stock in the portfolio, the current price, updated in real-time from a market data feed.

This project shows the .Net Data Adapter and Metadata Adapters for the *Portfolio Demo* and how they can be plugged into Lightstreamer Server. It also shows how to integrate the [Lightstreamer - Stock-List Demo - .NET Adapter](https://github.com/Lightstreamer/Lightstreamer-example-StockList-adapter-dotnet) into the same Adapter Set to support the full version of the *Portfolio Demo*.   

<!-- END DESCRIPTION lightstreamer-example-helloworld-adapter-dotnet -->

As an example of [Clients Using This Adapter](https://github.com/Lightstreamer/Lightstreamer-example-Portfolio-adapter-dotnet#clients-using-this-adapter), you may refer to the [Basic Portfolio Demo - HTML Client](https://github.com/Lightstreamer/Lightstreamer-example-Portfolio-client-javascript#basic-portfolio-demo---html-client) and view the corresponding [Basic Portfolio Demo Live Demo](http://demos.lightstreamer.com/PortfolioDemo_Basic/), or you may refer to the [Portfolio Demo - HTML Client](https://github.com/Lightstreamer/Lightstreamer-example-Portfolio-client-javascript#portfolio-demo---html-client) and view the corresponding [Portfolio Demo Live Demo](http://demos.lightstreamer.com/PortfolioDemo/) for the full version of the *Portfolio Demo*.

## Details

This project contains the source code and all the resources needed to install a .NET version of the Portfolio Data and Metadata Adapters.

### .NET Interfaces

Lightstreamer Server exposes native Java Adapter interfaces. The .NET interfaces are added through the [Lightstreamer Adapter Remoting Infrastructure (**ARI**)](http://www.lightstreamer.com/docs/adapter_generic_base/ARI%20Protocol.pdf). 

*The Architecture of Adapter Remoting Infrastructure for .NET.*

![General Architecture](generalarchitecture_new.png)

You'll find more details about the *SDK for .NET Adapters* at [.NET Interfaces](https://github.com/Lightstreamer/Lightstreamer-example-HelloWorld-adapter-dotnet/blob/master/README.md#net-interfaces) in the [Lightstreamer - "Hello World" Tutorial - .NET Adapter](https://github.com/Lightstreamer/Lightstreamer-example-HelloWorld-adapter-dotnet) project.

### Dig the Code

This project includes the implementation of the [IDataProvider](http://www.lightstreamer.com/docs/adapter_dotnet_api/Lightstreamer_Interfaces_Data_IDataProvider.html) interface and the [IMetadataProvider](http://www.lightstreamer.com/docs/adapter_dotnet_api/Lightstreamer_Interfaces_Metadata_IMetadataProvider.html) interface for the *Portfolio Demo*. 

The application is divided into 6 main classes.

* `PortfolioDataAdapter.cs`: this is a C#/.NET porting of the [Lightstreamer - Portfolio Demo - Java Adapter](https://github.com/Lightstreamer/Lightstreamer-example-Portfolio-adapter-java). It inherits from the `IDataProvider` interface and calls back Lightstreamer through the `IItemEventListener` interface. Use it as a starting point to implement your custom data adapter in case of `COMMAND` mode subscription.
* `PortfolioMetadataAdapter.cs`: this is a C#/.NET porting of the Metadata Adapter in [Lightstreamer - Portfolio Demo - Java Adapter](https://github.com/Lightstreamer/Lightstreamer-example-Portfolio-adapter-java).
   It inherits from the `LiteralBasedProvider`, a simple Metadata Adapter already included in the .NET Adapter SDK binaries, which is enough for all demo clients;
   see also [Lightstreamer - Reusable Metadata Adapters - .NET Adapter](https://github.com/Lightstreamer/Lightstreamer-example-ReusableMetadata-adapter-dotnet).
   In addition, it implements the `NotifyUserMessage` method, to handle `sendMessage` requests from the Portfolio Demo client. This allows the Portfolio Demo client to use `sendMessage` to submit buy/sell orders to the (simulated) portfolio feed used by the Portfolio Data Adapter.
* `PortfolioFeed.cs`: used to receive data from the simulated portfolio feed in an asynchronous way.
* `NotificationQueue.cs`: used to provide an executor of tasks in a single dedicated thread.
* `StandaloneAdaptersLauncher.cs`: this is a stand-alone executable that launches both the Data Adapter and the Metadata Adapter for the .NET Portfolio Demo example. It redirects sockets connections from Lightstreamer to the .NET Servers implemented in the LS .NET SDK library and does not rely on the .NET Server wrapper provided.
* `Log4NetLogging.cs`: used by the stand-alone executable to forward the log produced by the LS .NET SDK library to the application logging system, based on log4net.

Check out the sources for further explanations.
<!-- END DESCRIPTION lightstreamer-example-portfolio-adapter-dotnet -->

## Install

### Install the Basic Portfolio Demo
If you want to install a basic version of the *.Net Portfolio Demo* in your local Lightstreamer Server, follow the steps below.

* Download the [latest Lightstreamer distribution](http://download.lightstreamer.com/#current) (see the [Compatibility Notes](https://github.com/Lightstreamer/Lightstreamer-example-Portfolio-adapter-dotnet#lightstreamer-compatibility-notes) for more details about the correct version; Lightstreamer Server comes with a free non-expiring demo license for 20 connected users) from [Lightstreamer Download page](http://www.lightstreamer.com/download.htm), and install it, as explained in the `GETTING_STARTED.TXT` file in the installation home directory.
* Get the `deploy.zip` file of the [latest release](https://github.com/Lightstreamer/Lightstreamer-example-Portfolio-adapter-dotnet/releases) and unzip it
* Plug the *Proxy Data Adapter* and the *Proxy MetaData Adapter* into the Server: go to the `Deployment_LS` folder and copy the `DotNetPortfolio` directory and all of its files to the `adapters` folder of your Lightstreamer Server installation. Refer to the comments embedded in the generic adapters.xml file template, `DOCS-SDKs/adapter_remoting_infrastructure/doc/adapter_conf_template/adapters.xml`, for further details on configuration options.
* Launch *Lightstreamer Server*. The Server startup will complete only after a successful connection between the Proxy Adapters and the Remote Adapters.
* Launch the *Remote .NET Adapter Server*. Run the `DotNetCustomServer.bat` script under the `Deployment_DotNet_Server(custom)` directory. The script runs the `DotNetPortfolioDemoLauncher_N2.exe` Custom Launcher, which hosts both the Remote Data Adapter and the Remote Metadata Adapter for the .NET Portfolio Demo. In case of need, the .NET Server prints on the log a help page if run with the following syntax: "DotNetServer /help". Please note that the .NET Server connects to Proxy Adapters, not vice versa.
* Test the Adapter, launching the [Basic Portfolio Demo - HTML Client](https://github.com/Lightstreamer/Lightstreamer-example-Portfolio-client-javascript#basic-portfolio-demo---html-client), listed in [Clients Using This Adapter](https://github.com/Lightstreamer/Lightstreamer-example-Portfolio-adapter-dotnet#clients-using-this-adapter).
    * To make the [Basic Portfolio Demo - HTML Client](https://github.com/Lightstreamer/Lightstreamer-example-Portfolio-client-javascript#basic-portfolio-demo---html-client) front-end pages get data from the newly installed Adapter Set, you need to modify the front-end pages and set the required Adapter Set name to PORTFOLIODEMO_REMOTE when creating the LightstreamerClient instance. So edit the `lsClient.js` file of the Basic Portfolio Demo front-end deployed under `Lightstreamer/pages/PortfolioDemo_Basic` and replace:<BR/>
    ```
    var lsClient = new LightstreamerClient(protocolToUse+"//localhost:"+portToUse,"PORTFOLIODEMO");
    ```
    with:<BR/>
    ```
    var lsClient = new LightstreamerClient(protocolToUse+"//localhost:"+portToUse,"PORTFOLIODEMO_REMOTE");
    ```

    (you don't need to reconfigure the Data Adapter name, as it is the same in both Adapter Sets).
    * As the referred Adapter Set has changed, make sure that the front-end no longer shares the Engine with other demos.
So a line like this:<BR/>
`lsClient.connectionSharing.enableSharing("PortfolioDemoCommonConnection", "ATTACH", "CREATE");`<BR/>
should become like this:<BR/>
`lsClient.connectionSharing.enableSharing("RemotePortfolioConnection", "ATTACH", "CREATE");`
    * Open a browser window and go to: [http://localhost:8080/PortfolioDemo_Basic](http://localhost:8080/PortfolioDemo_Basic)


### Install the Portfolio Demo
To work with full functionality, the [Portfolio Demo - HTML Client](https://github.com/Lightstreamer/Lightstreamer-example-Portfolio-client-javascript#portfolio-demo---html-client), needs both the *PORTFOLIO_ADAPTER*, from the *Portfolio Demo*, and the *QUOTE_ADAPTER*, from the *Stock-List Demo* (see [Lightstreamer StockList Demo Adapter for .NET](https://github.com/Lightstreamer/Lightstreamer-example-StockList-adapter-dotnet)). 
If you want to install a full version of the *Portfolio Demo* in your local Lightstreamer Server, you have to deploy the *PORTFOLIO_ADAPTER* and the *QUOTE_ADAPTER* together in the same Adapter Set. 
To allow the two adapters to coexist within the same Adapter Set, please follow the steps below.

* Download the [latest Lightstreamer distribution](http://download.lightstreamer.com/#current) (see the [Compatibility Notes](https://github.com/Lightstreamer/Lightstreamer-example-Portfolio-adapter-dotnet#lightstreamer-compatibility-notes) for more details about the correct version; Lightstreamer Server comes with a free non-expiring demo license for 20 connected users) from [Lightstreamer Download page](http://www.lightstreamer.com/download.htm), and install it, as explained in the `GETTING_STARTED.TXT` file in the installation home directory.
* Get the `deploy.zip` file of the [latest release](https://github.com/Lightstreamer/Lightstreamer-example-Portfolio-adapter-dotnet/releases) and unzip it
* Plug the *Proxy Data Adapter* and the *Proxy MetaData Adapter* into the Server: go to the `Full_Deployment_LS` folder and copy the `DotNetFullPortfolio` directory and all of its files to the `adapters` folder of your Lightstreamer Server installation. Refer to the comments embedded in the generic adapters.xml file template, `DOCS-SDKs/adapter_remoting_infrastructure/doc/adapter_conf_template/adapters.xml`, for further details on configuration options.
* Launch *Lightstreamer Server*. The Server startup will complete only after a successful connection between the Proxy Adapters and the Remote Adapters.
* Launch the *Remote .NET Adapter Server*. Run the `DotNetServers.bat` script under the `Full_Deployment_DotNet_Server` directory. The script runs the `DotNetPortfolioDemoLauncher_N2.exe` Custom Launcher, which hosts both the Remote Data Adapter and the Remote Metadata Adapter for the .NET Portfolio Demo. In case of need, the .NET Server prints on the log a help page if run with the following syntax: "DotNetServer /help". Please note that the .NET Server connects to Proxy Adapters, not vice versa.
* Test the Adapter, launching the [Portfolio Demo - HTML Client](https://github.com/Lightstreamer/Lightstreamer-example-Portfolio-client-javascript#portfolio-demo---html-client), listed in [Clients Using This Adapter](https://github.com/Lightstreamer/Lightstreamer-example-Portfolio-adapter-dotnet#clients-using-this-adapter).
    * To make the [Portfolio Demo - HTML Client](https://github.com/Lightstreamer/Lightstreamer-example-Portfolio-client-javascript#portfolio-demo---html-client) front-end pages get data from the newly installed Adapter Set, you need to modify the front-end pages and set the required Adapter Set name to PORTFOLIODEMO_REMOTE when creating the LightstreamerClient instance. So edit the `lsClient.js` file of the Portfolio Demo front-end deployed under `Lightstreamer/pages/PortfolioDemo` and replace:<BR/>
`var lsClient = new LightstreamerClient(protocolToUse+"//localhost:"+portToUse,"FULLPORTFOLIODEMO");`<BR/>
with:<BR/>
`var lsClient = new LightstreamerClient(protocolToUse+"//localhost:"+portToUse,"FULLPORTFOLIODEMO_REMOTE");`<BR/>
(you don't need to reconfigure the Data Adapter name, as it is the same in both Adapter Sets).
    * As the referred Adapter Set has changed, make sure that the front-end no longer shares the Engine with other demos.
So a line like this:<BR/>
`lsClient.connectionSharing.enableSharing("PortfolioDemoCommonConnection", "ATTACH", "CREATE");`<BR/>
should become like this:<BR/>
`lsClient.connectionSharing.enableSharing("RemotePortfolioConnection", "ATTACH", "CREATE");`
    * Open a browser window and go to: [http://localhost:8080/PortfolioDemo](http://localhost:8080/PortfolioDemo)

## Build

To build your own version of `DotNetPortfolioDemo_N2.dll`, instead of using the one provided in the `deploy.zip` file from the [Install](https://github.com/Lightstreamer/Lightstreamer-example-Portfolio-adapter-dotnet#install) section above, follow these steps:
* Download this project.
* Create a project for a library target and name it "DotNetPortfolioDemo_N2",
* Include in the project the sources `src/src_adapters`.
* Get the Lightstreamer .NET Adapter Server library `DotNetAdapter_N2.dll` file from the `DOCS-SDKs/sdk_adapter_dotnet/lib` folder of the [latest Lightstreamer distribution](http://download.lightstreamer.com/#current), and copy it into the `lib` directory.
* Include in the project the reference to `DotNetAdapter_N2.dll` from the `lib` folder.
* Build Solution

### Build the Stand-Alone Launcher
To build your own version of the Stand-Alone Launcher, follow these steps:
* Create a project for a console application target and name it "DotNetPortfolioDemoLauncher_N2".
* Include in the project the source `src/StandaloneAdaptersLauncher.cs`
* Include references to the Lightstreamer .NET Adapter Server library binaries (see above) and .NET Portfolio Demo Data Adapter binaries you have built in the previous step. 
* Make sure that the entry point of the executable is the ServerMain class.
* Build Solution

## See Also 

* [Adapter Remoting Infrastructure Network Protocol Specification](http://www.lightstreamer.com/docs/adapter_generic_base/ARI%20Protocol.pdf)
* [.NET Adapter API Reference](http://www.lightstreamer.com/docs/adapter_dotnet_api/frames.html)

### Clients Using This Adapter 
<!-- START RELATED_ENTRIES -->
* [Lightstreamer - Portfolio Demos - HTML Clients](https://github.com/Lightstreamer/Lightstreamer-example-Portfolio-client-javascript)
* [Lightstreamer - Portfolio Demo - Flex Client](https://github.com/Lightstreamer/Lightstreamer-example-Portfolio-client-flex)
* [Lightstreamer - Portfolio Demo - Dojo Toolkit Client](https://github.com/Lightstreamer/Lightstreamer-example-Portfolio-client-dojo)

<!-- END RELATED_ENTRIES -->

### Related Projects
* [Lightstreamer - Portfolio Demo - Java Adapter](https://github.com/Lightstreamer/Lightstreamer-example-Portfolio-adapter-java)
* [Lightstreamer - StockList Demo - .NET Adapter](https://github.com/Lightstreamer/Lightstreamer-example-StockList-adapter-dotnet)

## Lightstreamer Compatibility Notes

* Compatible with Lightstreamer SDK for .NET Adapters version 1.9.
* For source code of this example compatible with Lightstreamer SDK for .NET Adapters version 1.7, please refer to [this tree branch](https://github.com/Lightstreamer/Lightstreamer-example-Portfolio-adapter-dotnet/tree/b74faae97a192f939ef4fcf1ea83f3feedea08be).
