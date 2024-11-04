# DeveLanCacheUI_SteamDepotFinder
This application obtains the DepotId's belonging to all AppId's through SteamKit2.

Note, the token in the GitHub Secrets expires every year. So after that I need to login again.
Note to future self: I used my work phone as authenticator for the DeveLanCacheUI Account.

## How it works

Get all AppId's from public steam api.
Run them all through the SteamKit stuff to get DepotId's.
Store them in CSV file.
Cleanup CSV file.

## Related projects

| Project | Explanation |
| -- | -- |
| [DeveLanCacheUI_Backend](https://github.com/devedse/DeveLanCacheUI_Backend/) | The main project. Contains the readme. |
| [DeveLanCacheUI_Frontend](https://github.com/devedse/DeveLanCacheUI_Frontend/) | The Frontend. |
| [DeveLanCacheUI_SteamDepotFinder](https://github.com/devedse/DeveLanCacheUI_SteamDepotFinder) | A tool to generate the mapping for steam depots and games. Kinda deprecated when `Feature_DirectSteamIntegration` is set to true |
| [DeveLanCacheUI_SteamDepotFinder_Runner](https://github.com/devedse/DeveLanCacheUI_SteamDepotFinder_Runner) | Runs the SteamDepotFinder on a weekly basis. |

## Build status

| GitHubActions Builds |
|:--------------------:|
| [![GitHubActions Builds](https://github.com/devedse/DeveLanCacheUI_SteamDepotFinder/workflows/GitHubActionsBuilds/badge.svg)](https://github.com/devedse/DeveLanCacheUI_SteamDepotFinder/actions/workflows/githubactionsbuilds.yml) |

## DockerHub

| Docker Hub |
|:----------:|
| [![Docker pulls](https://img.shields.io/docker/v/devedse/develancacheui_steamdepotfinderconsoleapp)](https://hub.docker.com/r/devedse/develancacheui_steamdepotfinderconsoleapp/) |

## Code Coverage Status

| CodeCov |
|:-------:|
| [![codecov](https://codecov.io/gh/devedse/DeveLanCacheUI_SteamDepotFinder/branch/master/graph/badge.svg)](https://codecov.io/gh/devedse/DeveLanCacheUI_SteamDepotFinder) |

## Code Quality Status

| SonarQube |
|:---------:|
| [![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=DeveLanCacheUI_SteamDepotFinder&metric=alert_status)](https://sonarcloud.io/dashboard?id=DeveLanCacheUI_SteamDepotFinder) |

## Package

| NuGet |
|:-----:|
| [![NuGet](https://img.shields.io/nuget/v/DeveLanCacheUI_SteamDepotFinder.svg)](https://www.nuget.org/packages/DeveLanCacheUI_SteamDepotFinder/) |
