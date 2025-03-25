# Homeserver_GDrive
This is a repo containing a docker container that interacts with Google Drive.

## Prerequisite
This code requires that you already have a project set up in the google cloud console with the Drive APIs turned on for it and a Service Account running that has access to your drive folder(s). You will need to download the credentials for the Service Account and put the credentials.json file in the /config location

## Description
This repo contains a worker project that periodically uploads files for me to Google Drive. The worker is complete with a SQLite DB that stores records of what was uploaded and when. I put an MVC app on top of the worker service to display the most recent results to the user. The MVC App features a min page where you can get an overview of all the information, or you can get more information by going to the individual pages. This solution has been put into a docker container so that you can run it on your local docker stack.

## Install
Installing this application on your local machine should be simple. You need to make sure you have NET Core Version 8.0 and Docker/Docker Compose installed. Then you can clone the repo in Visual Studio and open the solution file. 

## Use
This project is intended to be hosted and ran in Docker with the supporting infrastructure. You can run it locally for debugging or as a one off if needed. You will need to provide a few variables in the docker compose file. Below is an example of a docker compose file, and an explanation of the different variables.

```yaml
networks:
    default:
        name: gdrive_network_name
        external: true

services:
  homeserver_gdrive:
    image: DOCKER_LOCATION_FOR_IMAGE
    container_name: Local_Server_GDrive
    environment:
        - AppSettings__DisplayName=Name_Displayed_In_MVC
        - AppSettings__DetailErrorRecordsCount=10
        - AppSettings__DetailInfoRecordsCount=20
        - AppSettings__DetailUploadRecordsCount=15
        - AppSettings__TopLevelGDriveFolder=Google_Folder_Name
        - AppSettings__GoogleApplicationName=Google_App_Name
        - AppSettings__UploadServiceDelayHours=8
        - AppSettings__DBMaintenanceDelayHours=8
    volumes:
        - /where/you/have/credentials/stored:/config
        - /location/of/files/you/want/uploaded:/media/upload
    ports:
        - 8008:8080
    restart: unless-stopped
```
| Item | Value | Notes |
| --- | --- | --- |
| AppSettings__DisplayName | string | Name to display in the MVC App |
| AppSettings__DetailErrorRecordsCount | int | Number of records to show on that page |
| AppSettings__DetailInfoRecordsCount | int | Number of records to show on that page |
| AppSettings__DetailUploadRecordsCount | int | Number of records to show on that page |
| AppSettings__TopLevelGDriveFolde | string | Folder name in Google Drive where uploads go |
| AppSettings__GoogleApplicationName | string | Name of Google Application you created |
| AppSettings__UploadServiceDelayHours | int | Delay in hours for running the upload worker |
| AppSettings__DBMaintenanceDelayHours | int | Delay in hours for running the databse maintenance worker |
| /config | string | Volume where you have put credentials.json. Also will store database |
| /media/upload | string | Volume where you want files uploaded from |

## License
[GNU GPLv3](https://choosealicense.com/licenses/gpl-3.0/)
