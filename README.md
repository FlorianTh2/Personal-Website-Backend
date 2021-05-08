# Personal-Website-Backend

Repository to create a backend for my [Frontend](https://github.com/FlorianTh2/Personal-Website). This backend includes services like loading data from github-api, loading documents from google drive api oder identity management. It is build using .net 5 and entity framework.

## Prerequisites
 - .net 5.0
 - running postgres-database (docker installation showed down below)

## Getting Started
This getting started section is a rough overview over the procedure, since you need proper github api- and google drive api credentials. This process is also describe right after this section.

 - Install mock-database
    ```
    $ docker run -p 5432:5432 \
        --name some-postgres \
        -e POSTGRES_PASSWORD=YourPassword \
        -d \
        --rm \
        postgres
    ```

 - Clone the repository

    ```
    $ git clone git@github.com:FlorianTh2/Personal-Website-Backend.git`
    ```

 - Switch to directory
 
    ```
    $ cd ./Personal-Website-Backend
    ```

 - Install of all dependencies
 
    ```
    $ dotnet restore
    ```
 
 - Enter your secrets into .\Personal-Website-Backend\PersonalWebsiteBackend\appsettings.json and leave out redis-section since not needed.

 - Run application

    ```
    $ dotnet run --project .\Personal-Website-Backend
     ```

 - Open swagger
 
    ```
    https://localhost:5000/
    ``` 

## Get Google Drive Credentials
The intention is that you have a google account, you default account with all your things, and you want to expose one folder to the public and load files from this folder. Now you have several ways to accomplish that goal. The apporoach which is used here will be described now and goes allong [that](https://stackoverflow.com/questions/23555040/access-google-drive-without-oauth2) post.

First create a google cloud account. This is used since we need this cloud account to access google drive account. Then go though the setup of activating the google drive api for this google cloud account described [here](https://developers.google.com/drive/api/v3/about-sdk) (in detail [this](https://developers.google.com/drive/api/v3/enable-drive-api) link) and for further documentation i can recommend [a](https://developers.google.com/drive/api/v3/quickstart/dotnet), [b](https://developers.google.com/drive/api/v3/quickstart/dotnet) and maybe [c](https://googleapis.dev/dotnet/Google.Apis.Drive.v3/latest/api/Google.Apis.Drive.v3.html).

Now you have google drive api enabled and your GCP account. Through this way we enabled the possiblity to access google drive programmaticly. The problem: we have our files on our private google account (which maybe is the same account we used for the GCP) and cant access this file since we have no way of authenticating with the google drive api. To authenticate with GCP we need to invest some time to GCP IAM-Services. Specificly we have to create a so called service-account at the GCP IAM Service which allowes us to generate a JWT (so we dont need to OAUTH2 [which is the only proper way to authenticate to GCP](https://developers.google.com/drive/api/v3/about-auth)). In a nutshell: you have to create a service account (in your google cloud account) and generate a access jwt token. This procedure is documented e.g. [here](https://cloud.google.com/iam/docs/service-accounts). Maybe you have to give this service account some permission, i dont know anymore.

Now you have to go to your private (non-GCP account) i.e. in your private google drive space, create a folder you want to share, adjust the permission and sharable-options for this folder and at the end share this folder with your GCP service account. Now you have 1. a account to authenticate and work with google drive api (the service account) and 2. this service account has access to your public folder in your private space.

## Get Github Credentials
This one is a bit easier. Just go to your github account + go to settings + go to developer setting + go to Personal Access Token + create your personal access token. Save this token.

## Important commands
 - Docker build image
 `$ docker build -f PersonalWebsiteBackend/Dockerfile -t personal-website-backend .`

 - Docker run image
 `$ docker run -p 8080:80 --name my-app-name -it -rm personal-website-backend`

 - Docker tag image for docker hub
 `$ docker tag personal-website-backend:latest flooth/personal-website-backend:latest`

 - Docker push image to docker hub
 `$ docker push flooth/personal-website-backend:latest`

 - If you want to provide credentials via environment variables and dont want to build image with your credentials
    ```
    $ # just remove credentials from appsettings
    $ # (+ add appsettings to .dockerignore)
    $ # + provide this env-variables when you run the container with --env or --env-file style
    ```

 - Provide credentials though environment variables inside a kubernetes context
     ```
    $ # Create a kubernetes-credentials file based on the kubernetes-credentials template file
    $ # fill in your credentials
    $ # make sure this file and your other secret files are in .dockerignore / .gitignore
    $ kubectl apply -f yourCredentialsFile.yaml
    ```

 - Check if your secrets are working (this is one approach)
 `$ kubectl get secrets/backend-secrets --template={{.data.ConnectionStrings__PersonalWebsiteBackendContextPostgre}} | base64 -d`

 - Run image in kubernetes
 `$ kubectl apply -f kubernetes-manifest.yaml`

 - Get shell into kubernetes pod/container
 `$ kubectl exec -it personal-website-backend-deployment-76bbb59b9c-vhc4c bash`

 - Rolling restart of pod/container/image in kubernetes
 `$ kubectl rollout restart`

 - Connect to kubectl from aws-console if you are using eks
 `$ aws eks --region "eu-central-1" update-kubeconfig --name "personal-website-eks-cluster-0"`

 - Start dotnet inside your container yourself (maybe to troubleshoot something)
    ```
    $ # replace old CMD command in dockerfile with following
    $ CMD /bin/sh ; sleep infinity
    ```
 
 - [optional] Add support for redis (needs minor flag-setting-changes)
 `$ docker run -p 6379:6379 redis`

 - Run tests
 `$ dotnet test`

## Build with
 - .net 5
 - entity framework 5
 - automapper
 - postgres
 - xunit
 - swagger (openApi)
 - fluentAssertions

## Additional sources
- [How to configure dockerfile for .net 5 which respects several projects](https://stackoverflow.com/a/57440303)
- [Discussion about port types which can be defined in kubernetes 1](https://stackoverflow.com/questions/49981601/difference-between-targetport-and-port-in-kubernetes-service-definition)
  - targetPort: 80  // port where httpd runs inside the webserver pod.
  - Traffic comes in on nodePort , forwards to port on the service which then routes to targetPort on the pod(s).
- [Discussion about port types which can be defined in kubernetes 1](https://community.kodekloud.com/t/targetport-vs-containerport/9292/3)
  - targetport = container port

## Acknowledgements
Thanks [Nick Chapsas](https://www.youtube.com/user/ElfocrashDev) for the awesome series about ASP.NET Core REST API. The guides and hints were great and helped me alot.
