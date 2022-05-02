# AKS simulate OOM kill

OOM = Ouf of Memory 

## Build and push container

```powershell
docker build . -t jjoomkill
docker tag jjoomkill jjakscontainers.azurecr.io/jjoomkill:v1

docker push jjakscontainers.azurecr.io/jjoomkill:v1
```

## Run in AKS

```powershell
kubectl apply -f deploy.yml
kubectl describe pod jjoomkill-XXXXXXXXXXX
```

Now you can check pod status - will see 
```
Reason:       OOMKilled
```