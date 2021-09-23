# Run Windows container on AKS with persistent disk

## Build docker image locally

Switch Docker to Windows Containers. Build image and push to Azure Docker Repository

```ps
docker build . -t winserver

docker login jjcontainers.azurecr.io
docker tag winserver jjcontainers.azurecr.io/winserver
docker push jjcontainers.azurecr.io/winserver
```

## Deploy to AKS

```ps
kubectl apply -f azure-disk.yaml
kubectl apply -f azure-windows-disk.yaml

kubectl describe pod winserver
kubectl get pods
```

Test drive E: on container

```ps
kubectl logs winserver
kubectl exec -it winserver -- powershell ls e:
```

## Clean-up

```ps
kubectl delete pod winserver
```
