﻿//using DeveLanCacheUI_SteamDepotFinder.Steam;
//using QRCoder;
//using SteamKit2;
//using SteamKit2.Authentication;
//using static SteamKit2.SteamApps;

//namespace DeveLanCacheUI_SteamDepotFinder
//{
//    public class QrCodeDingLogin
//    {
//        private readonly SteamClient steamClient;
//        private readonly CallbackManager manager;
//        private bool isRunning;
//        private readonly SteamUser? steamUser;

//        private const string TokenFilePath = "token.txt";


//        //AppId;AppName;DepotId
//        string outputFolder = "output";
//        string outputFilePath = "output/app-depot-output.csv";
//        string outputFilePathCleaned = "output/app-depot-output-cleaned.csv";
//        string lastProcessedStoreFile = "lastprocessed.txt";
//        uint? lastProcessedTemp = null;



//        int deniedCount = 0;
//        int processedCount = 0;
//        private bool loggedOn;

//        public QrCodeDingLogin()
//        {
//            if (!Directory.Exists(outputFolder))
//            {
//                Directory.CreateDirectory(outputFolder);
//            }

//            // create our steamclient instance
//            steamClient = new SteamClient();

//            // create the callback manager which will route callbacks to function calls
//            manager = new CallbackManager(steamClient);

//            // get the steamuser handler, which is used for logging on after successfully connecting
//            steamUser = steamClient.GetHandler<SteamUser>();

//            // register a few callbacks we're interested in
//            // these are registered upon creation to a callback manager, which will then route the callbacks
//            // to the functions specified
//            manager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
//            manager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);

//            manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
//            manager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOff);

//            manager.Subscribe<PICSProductInfoCallback>(PicsCallback);

//            manager.Subscribe<PICSTokensCallback>(OnPICSTokens);
//        }

//        public async Task GoLogin()
//        {
//            uint? lastAppIdProcessed = null;
//            if (File.Exists(lastProcessedStoreFile))
//            {
//                var lastAppProcessed = File.ReadAllText(lastProcessedStoreFile).Trim();
//                if (!string.IsNullOrWhiteSpace(lastAppProcessed))
//                {
//                    lastAppIdProcessed = uint.Parse(lastAppProcessed);
//                }
//            }

//            Console.WriteLine($"Starting processing from app: {lastAppIdProcessed}");



//            isRunning = true;

//            Console.WriteLine("Connecting to Steam...");

//            // initiate the connection
//            steamClient.Connect();

//            while (!loggedOn)
//            {
//                manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
//            }



//            int i = 0;
//            int setSize = 1000;

//            var allSteamApps = SteamApi.SteamApiData.applist.apps;

//            if (lastAppIdProcessed != null)
//            {
//                var allSteamAppsToProcess = allSteamApps.TakeWhile(t => t.appid != lastAppIdProcessed).ToList();
//                i = allSteamAppsToProcess.Count + 1;
//            }

//            processedCount = i;

//            DateTime lastUpdate = DateTime.Now;

//            // create our callback handling loop
//            while (isRunning)
//            {
//                // in order for the callbacks to get routed, they need to be handled by the manager
//                manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));


//                //We need to wait untill all stuff has processed asyncly
//                if (processedCount + deniedCount == i)
//                {
//                    if (lastProcessedTemp != -1)
//                    {
//                        File.WriteAllText(lastProcessedStoreFile, lastProcessedTemp.ToString());
//                    }

//                    Console.WriteLine($"Progress: {i}/{allSteamApps.Length} {Math.Round(i / (double)allSteamApps.Length * 100.0, 2)}%");

//                    if (i == allSteamApps.Length)
//                    {
//                        break;
//                    }

//                    var steamApps = steamClient.GetHandler<SteamApps>();

//                    var set = allSteamApps.Skip(i).Take(setSize).ToList();
//                    var appsToGet = set.Select(t => t.appid).ToList();

//                    lastProcessedTemp = set.Last().appid;

//                    lastUpdate = DateTime.Now;
//                    await steamApps.PICSGetAccessTokens(appsToGet, new List<uint>());
//                    i += set.Count;
//                }
//                else if (lastUpdate.AddSeconds(60) < DateTime.Now)
//                {
//                    Console.WriteLine("No update within 60 seconds. Killing everything and retrying....");
//                    DisconnectAndWaitForDisconnectedMax30Seconds();
//                    throw new TimeoutException("no response, please retry");
//                }
//            }

//            DisconnectAndWaitForDisconnectedMax30Seconds();

//            Console.WriteLine("Cleaning up everything...");

//            var allLines = File.ReadAllLines(outputFilePath);
//            Console.WriteLine($"Total lines: {allLines.Length}");
//            var allLinesDistinct = allLines.Distinct().ToList();
//            Console.WriteLine($"Duplicate lines removed (E.g. SteamWorks redist stuff): {allLines.Length - allLinesDistinct.Count}");

//            var selectified = allLinesDistinct.Select(t =>
//            {
//                var splitted = t.Split(';');
//                var canParseAppId = int.TryParse(splitted[0], out var appId);
//                var canParseDepotId = int.TryParse(splitted[1], out var depotId);
//                return new { Original = t, AppId = canParseAppId ? appId : 0, DepotId = canParseDepotId ? depotId : 0 };
//            }).ToList();

//            Console.WriteLine("Sorting...");

//            var selectifiedSorted = selectified.OrderBy(t => t.AppId).ThenBy(t => t.DepotId).Select(t => t.Original).ToList();

//            Console.WriteLine($"Writing output to: {outputFilePathCleaned}");
//            File.WriteAllLines(outputFilePathCleaned, selectifiedSorted);

//            Console.WriteLine("App exitted");
//        }

//        private void DisconnectAndWaitForDisconnectedMax30Seconds()
//        {
//            Console.WriteLine("Disconnecting...");
//            steamClient.Disconnect();

//            for (int y = 0; y < 30; y++)
//            {
//                if (isRunning == false)
//                {
//                    break;
//                }
//                Console.WriteLine($"Waiting for SteamClient to Disconnect... {y}");

//                // in order for the callbacks to get routed, they need to be handled by the manager
//                manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
//            }
//        }

//        async void OnConnected(SteamClient.ConnectedCallback callback)
//        {
//            //TokenStore? token = null;

//            //var envToken = Environment.GetEnvironmentVariable("STEAMTOKEN");

//            //if (envToken != null)
//            //{
//            //    token = JsonSerializer.Deserialize<TokenStore>(envToken);
//            //}
//            //else if (File.Exists(TokenFilePath))
//            //{
//            //    var tokenStoreSerialized = File.ReadAllText(TokenFilePath);
//            //    token = JsonSerializer.Deserialize<TokenStore>(tokenStoreSerialized);
//            //}

//            steamUser.LogOnAnonymous();


//            //if (token != null)
//            //{
//            //    // Logon to Steam with the access token we have received
//            //    steamUser.LogOn(new SteamUser.LogOnDetails
//            //    {
//            //        Username = token.AccountName,
//            //        AccessToken = token.RefreshToken,
//            //    });
//            //}
//            //else
//            //{

//            //    // Start an authentication session by requesting a link
//            //    var authSession = await steamClient.Authentication.BeginAuthSessionViaQRAsync(new AuthSessionDetails());

//            //    // Steam will periodically refresh the challenge url, this callback allows you to draw a new qr code
//            //    authSession.ChallengeURLChanged = () =>
//            //    {
//            //        Console.WriteLine();
//            //        Console.WriteLine("Steam has refreshed the challenge url");

//            //        DrawQRCode(authSession);
//            //    };

//            //    // Draw current qr right away
//            //    DrawQRCode(authSession);

//            //    // Starting polling Steam for authentication response
//            //    // This response is later used to logon to Steam after connecting
//            //    var pollResponse = await authSession.PollingWaitForResultAsync();

//            //    Console.WriteLine($"Logging in as '{pollResponse.AccountName}'...");

//            //    // Logon to Steam with the access token we have received
//            //    steamUser.LogOn(new SteamUser.LogOnDetails
//            //    {
//            //        Username = pollResponse.AccountName,
//            //        AccessToken = pollResponse.RefreshToken,
//            //    });

//            //    var tokenStore = new TokenStore() { AccountName = pollResponse.AccountName, RefreshToken = pollResponse.RefreshToken };
//            //    var tokenStoreSerialized = JsonSerializer.Serialize(tokenStore, new JsonSerializerOptions() { WriteIndented = true });
//            //    File.WriteAllText(TokenFilePath, tokenStoreSerialized);
//            //}
//        }

//        void OnDisconnected(SteamClient.DisconnectedCallback callback)
//        {
//            Console.WriteLine("Disconnected from Steam");

//            isRunning = false;
//        }

//        async void OnLoggedOn(SteamUser.LoggedOnCallback callback)
//        {
//            if (callback.Result != EResult.OK)
//            {
//                Console.WriteLine("Unable to logon to Steam: {0} / {1}", callback.Result, callback.ExtendedResult);

//                isRunning = false;
//                return;
//            }

//            Console.WriteLine("Successfully logged on!");
//            loggedOn = true;


//            // at this point, we'd be able to perform actions on Steam


//            // for this sample we'll just log off
//            //steamUser.LogOff();
//        }

//        void OnLoggedOff(SteamUser.LoggedOffCallback callback)
//        {
//            Console.WriteLine("Logged off of Steam: {0}", callback.Result);
//        }


//        void OnPICSTokens(PICSTokensCallback callback)
//        {

//            foreach (var denied in callback.AppTokensDenied)
//            {
//                var theApp = SteamApi.SteamAppDict[denied];
//                var outputString = ToOutputStringSanitized(theApp.appid.ToString(), theApp.name, "denied");
//                //Console.WriteLine(outputString);
//                File.AppendAllLines(outputFilePath, new List<string>() { outputString });

//                deniedCount++;
//            }

//            var allPicsRequests = callback.AppTokens.Select(t => new PICSRequest(t.Key, t.Value)).ToList();


//            var steamApps = steamClient.GetHandler<SteamApps>();
//            steamApps.PICSGetProductInfo(allPicsRequests, new List<PICSRequest>(), false);
//        }

//        private void PicsCallback(PICSProductInfoCallback callback)
//        {
//            foreach (var a in callback.Apps)
//            {
//                var depots = a.Value.KeyValues["depots"];

//                foreach (var dep in depots.Children)
//                {
//                    if (uint.TryParse(dep.Name, out var _) && dep.Value == null)
//                    {
//                        var worked = SteamApi.SteamAppDict.TryGetValue(a.Key, out var appNameThing);

//                        string appName = worked ? appNameThing!.name : "unknown";

//                        if (dep.Children.Any(t => t.Name == "depotfromapp"))
//                        {
//                            var depfromappString = dep.Children.First(t => t.Name == "depotfromapp").AsString();

//                            //Some apps have some strange characters in the depot id's: https://steamdb.info/app/1106980/depots/
//                            var depfromappStringNumberified = new string(depfromappString?.Where(t => char.IsDigit(t)).ToArray());
//                            var worked2 = uint.TryParse(depfromappStringNumberified, out var depfromapp);

//                            //Assume that if depfromapp == 0, it's a redistributable that we've already obtained elsewhere
//                            //Example: https://steamdb.info/app/2203540/depots/
//                            if (worked2 && depfromapp != 0)
//                            {
//                                var worked3 = SteamApi.SteamAppDict.TryGetValue(depfromapp, out var appNameThing2);
//                                string appName2 = worked3 ? appNameThing2!.name : "unknown";

//                                var outputString = ToOutputStringSanitized(depfromappStringNumberified, appName2, dep.Name);
//                                //Console.WriteLine(outputString);
//                                File.AppendAllLines(outputFilePath, new List<string>() { outputString });
//                            }
//                        }
//                        else
//                        {
//                            var outputString = ToOutputStringSanitized(a.Key.ToString(), appName, dep.Name);
//                            //Console.WriteLine(outputString);
//                            File.AppendAllLines(outputFilePath, new List<string>() { outputString });
//                        }
//                    }
//                }

//                processedCount++;
//            }
//        }

//        public string ToOutputStringSanitized(string appId, string appName, string depotId)
//        {
//            if (appId == "0")
//            {

//            }

//            appId = appId ?? "";
//            appName = appName ?? "";
//            depotId = depotId ?? "";

//            return $"{appId.Replace(";", ":")};{appName.Replace(";", ":")};{depotId.Replace(";", ":")}";
//        }


//        void DrawQRCode(QrAuthSession authSession)
//        {
//            Console.WriteLine($"Challenge URL: {authSession.ChallengeURL}");
//            Console.WriteLine();

//            // Encode the link as a QR code
//            var qrGenerator = new QRCodeGenerator();
//            var qrCodeData = qrGenerator.CreateQrCode(authSession.ChallengeURL, QRCodeGenerator.ECCLevel.L);
//            var qrCode = new AsciiQRCode(qrCodeData);
//            var qrCodeAsAsciiArt = qrCode.GetGraphic(1, drawQuietZones: false);

//            Console.WriteLine("Use the Steam Mobile App to sign in via QR code:");
//            Console.WriteLine(qrCodeAsAsciiArt);
//        }
//    }
//}
